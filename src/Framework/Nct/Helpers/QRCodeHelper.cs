using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.ImageSharp.Rendering;
using ZXing.QrCode;


namespace Nct.Helpers
{
    public static class QRCodeHelper
    {

        public static byte[] Create(string data, int width = 200, int height = 200, Color? foreground = null)
        {

            var writer = new ZXing.ImageSharp.BarcodeWriter<Rgba32>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = width,
                    Height = height,
                    Margin = 1
                },

            };

            foreground ??= Color.Black;
            var render = new ImageSharpRenderer<Rgba32> { Foreground = foreground.Value };
            BitMatrix matrix = writer.Encode(data);
            var image = render.Render(matrix, writer.Format, data, writer.Options);

            // var image = writer.WriteAsImageSharp<Rgba32>(data);

            using (var ms = new MemoryStream())
            {
                image.Save(ms, new PngEncoder());
                return ms.ToArray();
            }

        }





        public static string GetBase64Img(string data, int width = 200, int height = 200, Color? foreground = null)
        {

            var bytes = Create(data, width, height, foreground);
            var base64 = Convert.ToBase64String(bytes);

            base64 = $"data:image/png;base64,{base64}";
            return base64;

        }

        public static string DecodeImg(string img)
        {

            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(img))
            {

                var reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>
                {
                    Options = new DecodingOptions
                    {
                        CharacterSet = "UTF-8",

                    }

                };
                ZXing.Result r = reader.Decode(image);
                if (r != null)
                {
                    return r.Text;
                }

            }

            return null;


        }








    }
}
