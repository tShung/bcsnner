using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WPL.Models
{
    public class Repository : IProduct
    {
        public Repository() 
        {
        }

        public ScannModel Search(string upc)
        {
            return new ScannModel { Upc = upc, Name = "Can not find this item" };
        }
    }
}