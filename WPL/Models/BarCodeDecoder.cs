using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.Common;

namespace WPL.Models
{
    public class BarCodeDecoder
    {
        public string CodeType { get; private set; }
        public string CodeContent { get; private set; }

        public string Read(string imageData)
        {
            // create a barcode reader instance
            IBarcodeReader reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            // load a bitmap
            var barcodeBitmap = imageData.Base64StringToBitmap();
            // detect and decode the barcode inside the bitmap
            var result = reader.Decode(barcodeBitmap);
            // do something with the result
            if (result != null)
            {
                CodeType = result.BarcodeFormat.ToString();
                CodeContent = result.Text;
                return CodeContent;
            }
            return null;
        }
    }
}