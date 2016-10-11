#include ..\makefile.inc

!ifdef RELEASE_DIR
RELEASE_Web=$(RELEASE_DIR)\wpl
!else
RELEASE_Web=\mpos\wpl
!endif

TARGET_PROJECT=WPL\WPL.csproj

all : build

build:
   @echo build ScoriaManager.csproj ...
   @msbuild $(TARGET_PROJECT) /t:Rebuild /p:configuration=Release /p:platform="AnyCPU" /fl

install install_ent install_demo install_pro install_std install_credit install_credit_pro:
    @echo publishing ScoriaManager...
    @msbuild $(TARGET_PROJECT) /t:PipelinePreDeployCopyAllFilesToOneFolder /p:configuration=Release /p:platform="AnyCPU" /p:_PackageTempDir=$(RELEASE_Web) /fl

cleanall: clean

clean :
   @echo clean up...
   @msbuild $(TARGET_PROJECT) /t:clean /p:configuration=Release /p:platform="AnyCPU" /fl