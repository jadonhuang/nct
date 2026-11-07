using Nct.Cache;
using Nct.Extentions;
using Nct.Ioc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nct.Helpers
{

    public class CheckCodeInfo
    {

        public byte[] Img { get; set; }

        public string Id { get; set; }


    }
    public static class CheckCodeHelper
    {

        private static readonly Color[] Colors = [Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkGreen, Color.DarkBlue];

        public static bool Verify(string checkCodeId, string value)
        {

            var cache = IocManager.Instance.Resolve<ICache>();
            var cacheValue = cache.Get<string>(checkCodeId);
            cache.Remove(checkCodeId);
            return cacheValue != null && cacheValue.IgnoreCaseEqual(value);

        }

        public static CheckCodeInfo Create(bool useMathCal = true, int width = 80, int height = 30, int fontSize = 18)
        {

            var endValue = 10;
            var codeInfo = new CheckCodeInfo();
            var id = Guid.NewGuid().ToString("n");
            string checkCode;
            string result = null;
            byte[] img;

            if (useMathCal)
            {
                var random = Random.Shared;
                var a = random.Next(0, endValue);
                var b = random.Next(1, endValue);
                var opFlag = random.Next(0, 4);
                var op = "";
                switch (opFlag)
                {
                    case 0:
                        op = "+";
                        result = (a + b).ToString();
                        break;

                    case 1:
                        op = "-";
                        if (b > a)
                        {
                            (b, a) = (a, b);
                        }
                        result = (a - b).ToString();
                        break;


                    case 2:
                        op = "*";
                        result = (a * b).ToString();
                        break;

                    case 3:
                        op = "/";
                        if (a % b != 0)
                        {

                            var divisibles = new List<int>();

                            for (var i = 1; i <= a; i++)
                            {
                                if (a % i == 0)
                                {
                                    divisibles.Add(i);
                                }
                            }

                            b = divisibles[random.Next(divisibles.Count)];

                        }
                        result = (a / b).ToString();
                        break;

                }

                checkCode = $"{a}{op}{b}=?";
                img = CreateImage(checkCode, width, height, fontSize, false);

            }
            else
            {
                checkCode = CommonHelper.GetRandom(4);
                result = checkCode;
                img = CreateImage(checkCode, width, height, fontSize);
            }


            codeInfo.Id = id;
            codeInfo.Img = img;

            if (IocManager.Instance.IsRegistered<ICache>())
            {
                var cache = IocManager.Instance.Resolve<ICache>();
                cache.Set(id, result);
            }
            return codeInfo;


        }


        public static byte[] CreateImage(string checkCode, int width = 80, int height = 30, int fontSize = 18, bool drawInterferenceLine = true, bool drawInterferencePoint = true)
        {


            var r = Random.Shared;
            using var image = new Image<Rgba32>(width, height);
            var fontName = "Microsoft YaHei";

            if (!SystemFonts.Families.Any(m => m.Name == fontName))
                fontName = SystemFonts.Families.First().Name;

            var font = SystemFonts.CreateFont(fontName, fontSize, FontStyle.Bold);
            image.Mutate(ctx =>
            {
                // 白底背景
                ctx.Fill(Color.White);

                var codeLen = checkCode.Length;
                var maxWidth = 0f;
                var maxHeight = 0;

                for (int i = 0; i < codeLen; i++)
                {
                    var size = TextMeasurer.Measure(checkCode[i].ToString(), new TextOptions(font));
                    if (size.Width > maxWidth) maxWidth = size.Width;
                    if (size.Height > maxHeight) maxHeight = (int)Math.Ceiling(size.Height);

                }

                var totalWidth = maxWidth * codeLen;
                var space = (width - totalWidth) / (codeLen);
                if (space < 0)
                    space = 0;

                var minY = 1;
                var maxY = height - maxHeight - minY;

                // 画验证码
                for (int i = 0; i < codeLen; i++)
                {

                    ctx.DrawText(checkCode[i].ToString()
                        , font
                        , Colors[r.Next(Colors.Length)]
                        , new PointF(i * maxWidth + (i + 1) * space, r.Next(minY, maxY)));
                }

                if (drawInterferenceLine)
                {
                    // 画干扰线
                    for (int i = 0; i < 4; i++)
                    {
                        var pen = new Pen(Colors[r.Next(Colors.Length)], 1);
                        var p1 = new PointF(r.Next(width), r.Next(height));
                        var p2 = new PointF(r.Next(width), r.Next(height));
                        ctx.DrawLines(pen, p1, p2);
                    }

                }

                if (drawInterferencePoint)
                {
                    // 画噪点
                    for (int i = 0; i < 20; i++)
                    {
                        var pen = new Pen(Colors[r.Next(Colors.Length)], 1);
                        var p1 = new PointF(r.Next(width), r.Next(height));
                        var p2 = new PointF(p1.X + 1f, p1.Y + 1f);
                        ctx.DrawLines(pen, p1, p2);
                    }
                }
            });

            using (var ms = new MemoryStream())
            {
                image.SaveAsPng(ms);
                return ms.ToArray();
            }



        }






    }
}
