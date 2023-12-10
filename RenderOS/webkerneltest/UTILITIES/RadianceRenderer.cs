using CosmosTTF;
using System;
using System.Drawing;
using System.Linq;

namespace RadianceOS.Render
{
    public static class Canvas
    {
        public static Cosmos.System.Graphics.Canvas canvas;

        public static void setup(Cosmos.System.Graphics.Canvas canv)
        {

            canvas = canv;

        }

        public static void DrawImageAlpha(Cosmos.System.Graphics.Image image, int x, int y)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = Color.FromArgb(image.RawData[i + j * image.Width]);
                    if (color.A == 0)
                        continue;
                    canvas.DrawPoint(color, x + i, y + j);
                }
            }
        }

        public static void DrawCenteredTTFString(this Cosmos.System.Graphics.Canvas canv,string myString, int WinLengh, int WinPosX, int WinPosY, int space, Color color, string fontName, int fontSize)
        {
            string[] strings = myString.Split(new string[] { "\n" }, StringSplitOptions.None).Select(s => s.Trim()).ToArray();
            for (int i = 0; i < strings.Length; i++)
            {
                int lengh = TTFManager.GetTTFWidth(strings[i], fontName, fontSize);

                int posX = (WinLengh - lengh) / 2;
                canv.DrawStringTTF(strings[i], fontName, color, fontSize, posX + WinPosX, WinPosY + i * space);
            }
        }

    }
}