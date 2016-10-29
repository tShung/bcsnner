using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WPL.Models
{
    public class Repository : IProduct
    {
        private WPL _db;

        public Repository() 
        {
            _db = new WPL();
        }

        public ScannModel Search(string upc)
        {
            if (string.IsNullOrEmpty(upc))
                return new ScannModel { Upc = "No barcode recognized" };

            var query = from p in _db.Products
                        where p.SkuCode == upc
                        select new ScannModel
                        {
                            Upc = upc,
                            Name = p.Name,
                            Price = p.RetailPrice,
                            Qty = p.Inventory,
                            Location = p.Location
                        };
            var model = query.FirstOrDefault();
            return model == null ? new ScannModel { Upc = upc, Name = "Can not find this item" } : model;
        }
    }
}