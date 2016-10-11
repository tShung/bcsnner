using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WPL.Models
{
    public class ScannModel
    {
        public string Upc { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        //[DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal Price { get; set; }
        [DisplayName("In Stock")]
        public int Qty { get; set; }
        [DisplayName("Store")]
        public string Location { get; set; }
    }
}