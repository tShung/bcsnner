using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPL.Models
{
    interface IProduct
    {
        ScannModel Search(string upc);
    }
}
