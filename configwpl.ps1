# this script will set up IIS Web Site WPL and set needed parameters
# To run it, in administrator command prompt: powersehll .\configscoria.ps1 -identity xxxx -password xxxxx
# if the parameters are not provided, it will be asked during the execution
# the script must be in the current folder or a absoulte path
#parameters
[CmdletBinding()]

param(
  #[Parameter(Mandatory=$True)]
  [string]$identity,
  #[Parameter(Mandatory=$True)]
  [string]$password,
  [string]$dbserver = '(local)',
  [string]$siteName = 'wpl',
  [string]$physicalPath,
  [int]$port = 22225,
  [int]$sslport = 22226
)

if (-not $physicalPath)
{
    $physicalPath = (Get-Item env:'MRS_HOME').Value
    if (-not $physicalPath)
    {
        $physicalPath = 'c:\mpos\wpl'
    }
    else
    {
        $physicalPath = $physicalPath + '\wpl'
    }
}

#allow to run local created scripts
set-executionPolicy RemoteSigned -ErrorAction stop

#import module in case it is not imported
Import-Module "WebAdministration" -ErrorAction stop

write-host "Start  configuring WPL..." -ForegroundColor Yellow

####### configur web.config ############################################################

#get dbserver and dbname from ODBC registration
$node32 = 'HKLM:\SOFTWARE\Wow6432Node'
$exist = get-item $node32
if ( -not $exist )
{
    $node32 = 'HKLM:\SOFTWARE'
}
$mrsodbc = get-ItemProperty $node32\MRS\ODBC -Name DatabaseName -ErrorAction stop
if ( -not $mrsodbc ) 
{
    Write-Host 'can not get odbc key from registry, configue odbc on the host first'
    exit
}
$odbcname = $mrsodbc.DatabaseName

$dbname = (Get-ItemProperty $node32\ODBC\ODBC.INI\$odbcname -Name Database).Database
$dbserver= (Get-ItemProperty $node32\ODBC\ODBC.INI\$odbcname -Name Server).Server

$webConfig = $physicalPath + '\Web.config'
$doc = (Get-Content $webConfig) -as [Xml]


#set connnection strings
$root = $doc.get_DocumentElement();
$scoriaCon = $root.connectionStrings.add | where {$_.name -eq 'CrimsonDbEntities'}
$defaultCon = $root.connectionStrings.add | where {$_.name -eq 'DefaultConnection'}
$newCon = $scoriaCon.connectionString.Replace('$dbserver', $dbserver).Replace('$dbname', $dbname);
$newCon1 = $defaultCon.connectionString.Replace('$dbserver', $dbserver).Replace('$dbname', $dbname);

$scoriaCon.connectionString = $newCon
$defaultCon.connectionString = $newCon1

$doc.Save($webConfig)

Write-host 'web.config was configured with:' $dbserver 'and' $dbname -ForegroundColor Green

########################################################################################


write-host 'iis configureation...'
if ($identity -and $password) 
{
    #todo:validate the credential
} 
else
{
    #get credential from user input for the account runing the app pool
    Write-host ' Get runing account for the app pool' -ForegroundColor Yellow
    $cred = Get-Credential -ErrorAction Stop
    if ($cred)
    {
        $identity = $cred.UserName
        $password = $cred.GetNetworkCredential().Password
        $cred = ''      
    }
    else
    {
        write-host ' no credential providered, stop!' -ForegroundColor Red
        exit
    }
}

############ create app pool ######################
write-host 'create apppool:' $siteName  -ForegroundColor Yellow
Remove-WebAppPool $siteName -ErrorAction SilentlyContinue
$pool = New-WebAppPool $siteName
write-host $pool.name 'created' -ForegroundColor Green

write-host 'set runing account for apppool'  -ForegroundColor Yellow
$pool.processModel.userName = $identity
$pool.processModel.password = $password
$pool.processModel.identityType = 3
#$pool.enable32BitAppOnWin64 = $True
$pool | Set-Item -ErrorAction Stop

write-host 'appPool account was set to:' $identity –foregroundcolor green
##########################################################################

############ create the web site if web site exist, force to create #######
write-host 'create website' $siteName 'web path:' $physicalPath  -ForegroundColor Yellow
$site = New-Website -Name $siteName -Port $port -physicalPath $physicalPath -ApplicationPool $pool.name -Force:$True
New-WebBinding $site.name -Protocol "https" -Port $sslport

############################################################################

#start web site
write-host 'start appPoll and website...'  -ForegroundColor Yellow
start-WebAppPool $pool.name
start-WebSite $site.name
write-host 'web site: ' $siteName 'is ready'  -ForegroundColor green

#Start-Process -FilePath $url

write-host 'configuration completed!' -ForegroundColor Green
