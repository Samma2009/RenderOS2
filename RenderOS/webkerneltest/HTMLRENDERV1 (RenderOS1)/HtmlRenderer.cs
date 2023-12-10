using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using CosmosTTF;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webkerneltest
{
    public class HtmlRenderer
    {

        public Canvas canvas;

        int TopToBottomGrid = 0;

        [ManifestResourceStream(ResourceName = "webkerneltest.UbuntuMono-Regular.ttf")]
        static byte[] font;

        public HtmlRenderer(uint widht,uint height)
        {

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(widht,height,ColorDepth.ColorDepth32));

            RadianceOS.Render.Canvas.setup(canvas);

            TTFManager.RegisterFont("UMR", font);

        }

        public void Render(string code) 
        {

            canvas.Clear(Color.White);

            var bred = code.Replace("<br>", "%ln%");

            var a = bred.Split(new char[] {'<','>'},StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < a.Length; i++)
            {

                if (!a[i].Contains("/"))
                {

                    if (a[i].StartsWith("h1") && !a[i].Contains("/"))
                    {
                        foreach (var item in a[i + 1].Split("%ln%"))
                        {
                            if (!item.StartsWith("/"))
                            {
                                canvas.DrawStringTTF(item, "UMR", Color.Black, 40, 10, TopToBottomGrid + 40);

                                TopToBottomGrid += 45;
                            }
                        }
                    }
                    else if (a[i].StartsWith("h2") && !a[i].Contains("/"))
                    {
                        foreach (var item in a[i + 1].Split("%ln%"))
                        {
                            if (!item.StartsWith("/"))
                            {
                                canvas.DrawStringTTF(item, "UMR", Color.Black, 30, 10, TopToBottomGrid + 30);

                                TopToBottomGrid += 35;
                            }
                        }
                    }
                    else if (a[i].StartsWith("h3")&&!a[i].Contains("/"))
                    {
                        foreach (var item in a[i + 1].Split("%ln%"))
                        {
                            if (!item.StartsWith("/"))
                            {
                                canvas.DrawStringTTF(item, "UMR", Color.Black, 20, 10, TopToBottomGrid + 20);

                                TopToBottomGrid += 25;
                            }
                        }
                    }
                    else if (a[i].StartsWith("h4") && !a[i].Contains("/"))
                    {
                        foreach (var item in a[i + 1].Split("%ln%"))
                        {

                            if (!item.StartsWith("/"))
                            {
                                canvas.DrawStringTTF(item, "UMR", Color.Black, 15, 10, TopToBottomGrid + 10);

                                TopToBottomGrid += 15;
                            }

                        }
                    }
                    else if (a[i].StartsWith("button") && !a[i].Contains("/"))
                    {

                        canvas.DrawFilledRectangle(Color.Gray,10, TopToBottomGrid,70,20);
                        canvas.DrawStringTTF(a[i+1], "UMR", Color.Black, 15, 10, TopToBottomGrid + 15);
                        TopToBottomGrid += 25;

                    }

                }

            }

            canvas.Display();

        }

    }
}
