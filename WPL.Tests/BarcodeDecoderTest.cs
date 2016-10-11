using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPL.Models;
using System.Drawing;

namespace WPL.Tests
{
    [TestClass]
    public class BarcodeDecoderTest
    {
        [TestMethod]
        public void DecoderTest()
        {
            BarCodeDecoder decoder = new BarCodeDecoder();
            Bitmap bitmap = new Bitmap("captured.png");
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = ms.ToArray();
                var imageData = Convert.ToBase64String(byteImage); //Get Base64
                var result = decoder.Read(imageData);

                Assert.AreEqual(result, "096619321841");
                
            }
        }

        [TestMethod]
        public void ProductSearchTest()
        {
            Repository rep = new Repository();
            var upc = "016138";
            var model = rep.Search(upc);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Upc, upc);
        }
    }
}
