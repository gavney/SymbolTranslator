using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnimatedGif;



namespace SymbolTranslator
{
    public partial class Form1 : Form
    {   
        Translator translator = new Translator();
        Bitmap image = null;

        string extension = null;
        public Form1()
        {
            InitializeComponent();
            
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            if ((image != null) & (textBoxPattern.Text.Length == 5))
            {
                if (checkBoxPicture.Checked)
                {
                    saveFileDialog.Filter = "Изображения (*.jpg; *.png, *.gif)|*.jpg; *.png; *.gif; |All files(*.*)|*.*";
                    saveFileDialog.FileName = extension == ".gif" ? "symbol.gif" : "symbol.jpg";
                }
                else
                {
                    saveFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                    saveFileDialog.FileName = "symbol.txt";
                }
                saveFileDialog.ShowDialog();            
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                extension = Path.GetExtension(openFileDialog.FileName);

                image = new Bitmap(openFileDialog.FileName);

                pictureBox.Image = new Bitmap(image, new Size(122, 113));

                labelRes.Text = image.Width.ToString() + "x" + image.Height.ToString();
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Bad image");
            }
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int width = (int)numericUpDownX.Value;
            int height = (int)numericUpDownY.Value;

            if (checkBoxPicture.Checked)
            {
                // Если гифка
                if (extension == ".gif")
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(makeGIF));
                    thread.Start(new Size(width, height));
                }
                // Если фотка
                else
                {
                    Bitmap result = translator.translateOnPhoto(image,
                        new Size(width, height), textBoxPattern.Text);

                    result.Save(saveFileDialog.FileName, image.RawFormat);
                }
            }
            // Если текст
            else
            {
                string result = translator.translate(image,
                    new Size(width, height), textBoxPattern.Text);

                File.WriteAllText(saveFileDialog.FileName, result);
            }
            saveFileDialog.Dispose();
        }

        private void makeGIF(object data)
        {
            Size size = (Size)data;
            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);

            buttonTranslate.Invoke((MethodInvoker)(() => buttonTranslate.Visible = false));
            progressBar1.Invoke((MethodInvoker)(() => progressBar1.Maximum = image.GetFrameCount(dimension)));

            using (var gif = AnimatedGif.AnimatedGif.Create(saveFileDialog.FileName, 33))
            {
                for (int i = 0; i < image.GetFrameCount(dimension); i++)
                {

                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = i));

                    image.SelectActiveFrame(dimension, i);

                    Bitmap frame = translator.translateOnPhoto(image, size, textBoxPattern.Text);

                    gif.AddFrame(frame, delay: -1, quality: GifQuality.Bit8);

                    frame.Dispose();

                }
            }
            buttonTranslate.Invoke((MethodInvoker)(() => buttonTranslate.Visible = true));
        }
    }
}
