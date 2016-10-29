using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class WPL : DbContext
    {
        // Your context has been configured to use a 'WPL' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'WPL.Models.WPL' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'WPL' 
        // connection string in the application configuration file.
        public WPL()
            : base("name=DefaultConnection")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Product> Products { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string SkuCode { get; set; }
        public decimal RetailPrice { get; set; }
        public int Inventory { get; set; }
        [MaxLength(400)]
        public string Location { get; set; }
    }
}