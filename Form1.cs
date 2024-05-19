using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace PavImgConverter
{
    public partial class Form1 : Form
    {

        private Bitmap currentImage;
        private Bitmap loadedImage;
        private ImageProcessor processor;
        public Form1()
        {
            InitializeComponent();
            AttachConsole();
            convertAndSaveBtn.Enabled = false;
            edgeBtn.Enabled = false;
            ditheringBtn.Enabled = false;
            gammaBtn.Enabled = false;
            redTxtBox.Enabled = false;
            greenTxtBox.Enabled = false;
            blueTxtBox.Enabled = false;
            processor = new ImageProcessor();
        }

        private void loadMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    loadedImage = new Bitmap(openFileDialog.FileName);
                    currentImage = new Bitmap(loadedImage);
                    long bitmapSizeInBytes = GetBitmapSizeInBytes(currentImage);
                    Console.WriteLine($"Bitmap size in bytes: {bitmapSizeInBytes}");
                    imageBox.Image = currentImage;
                    convertAndSaveBtn.Enabled = currentImage != null;
                }
            }
        }

        private long GetBitmapSizeInBytes(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                return memoryStream.Length;
            }
        }

        private void pavMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pav Files|*.pav";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap img = processor.LoadFromPavFile(openFileDialog.FileName);
                    loadedImage = new Bitmap(img);
                    currentImage = loadedImage;
                    imageBox.Image = currentImage;
                    edgeBtn.Enabled = true;
                    ditheringBtn.Enabled = true;
                    gammaBtn.Enabled = true;
                    redTxtBox.Enabled = true;
                    greenTxtBox.Enabled = true;
                    blueTxtBox.Enabled = true;
                }
            }
        }

        private void convertAndSaveBtn_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                YUV[,] yuvImage = processor.ConvertRGBtoYUV(currentImage);
                var (yChannel, uChannel, vChannel) = processor.Downsample(yuvImage);

                int alphabetSize = 256;
                byte[] compressedData = processor.Compress(yChannel, uChannel, vChannel, alphabetSize);

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Pav Files|*.pav";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        processor.SaveToPavFile(saveFileDialog.FileName, compressedData, currentImage.Width, currentImage.Height);
                        MessageBox.Show("Image converted and saved successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please load an image first.");
            }
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private void AttachConsole()
        {
            AllocConsole();
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(standardOutput);
        }

        private void ditheringBtn_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap ditheredImage = processor.ApplyDithering(currentImage);
                imageBox.Image = ditheredImage;
                currentImage = ditheredImage;
            }
            else
            {
                MessageBox.Show("Please load an image first.");
            }
        }

        private void restoreImgBtn_Click(object sender, EventArgs e)
        {
            if (loadedImage != null)
            {
                currentImage = new Bitmap(loadedImage);
                imageBox.Image = currentImage;
            }
            else
            {
                MessageBox.Show("No original image to restore.");
            }
        }

        private void edgeBtn_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                Bitmap edgeDetectImg = processor.ApplyHomogeneityEdgeDetection(currentImage);
                imageBox.Image = edgeDetectImg;
                currentImage = edgeDetectImg;
            }
            else
            {
                MessageBox.Show("Please load an image first.");
            }
        }

        private void gammaBtn_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                if (double.TryParse(redTxtBox.Text, out double red) &&
                    double.TryParse(greenTxtBox.Text, out double green) &&
                    double.TryParse(blueTxtBox.Text, out double blue))
                {
                    Bitmap gammaCorrectedImage = new Bitmap(currentImage);
                    ImageProcessor.ApplyGamma(gammaCorrectedImage, red, green, blue);
                    imageBox.Image = gammaCorrectedImage;
                    currentImage = gammaCorrectedImage;
                    MessageBox.Show("Gamma correction applied successfully.");
                }
                else
                {
                    MessageBox.Show("Please enter valid gamma values.");
                }
            }
            else
            {
                MessageBox.Show("Please load an image first.");
            }
        }
    }
}
