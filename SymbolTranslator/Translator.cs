using System.Drawing;
namespace SymbolTranslator
{
    internal class Translator
    {
        public Translator() { }

        public string translate(Bitmap image, Size size, string pattern)
        {
            string result = null;

            image = new Bitmap(image, size);

            for (int y = 0; y < image.Height; y++)
            {
                result += "\n";
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    double colorNum = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

                    if (colorNum > 0.9)
                        result += pattern[0];
                    else if (colorNum > 0.6)
                        result += pattern[1];
                    else if (colorNum > 0.4)
                        result += pattern[2];
                    else if (colorNum > 0.2)
                        result += pattern[3];
                    else
                        result += pattern[4];
                }
            }
            return result;
        }

        public Bitmap translateOnPhoto(Bitmap image, Size size, string pattern)
        {
            Bitmap newImage = new Bitmap(size.Width*5, size.Height*8);
            
            using (var graphics = Graphics.FromImage(newImage))
                graphics.Clear(Color.Black);

            string text = translate(image, size, pattern);

            Graphics graphImage = Graphics.FromImage(newImage);

            graphImage.DrawString(text, new Font("Lucida Console", 6, FontStyle.Regular),
            new SolidBrush(Color.White), new RectangleF(0, -4, newImage.Width, newImage.Height),
            new StringFormat(StringFormatFlags.NoWrap));

            return newImage;
        }
    }
}
