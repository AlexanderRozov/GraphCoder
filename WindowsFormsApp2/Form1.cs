using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Setup();
        }
        private int i;
        private FileInfo[] pics;
        private byte[] bytesToEncrypt;

        private void Setup()
        {
            ofd.Filter = "Image Files(*.bmp, *.jpg, *.png, *.tif, *.gif, *.psd) | *.bmp; *.jpg; *.png; *.tif; *.gif; *.psd";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenImage();
        }

        private void OpenImage()
        {
            int width = 0, height = 0;
            string fileName="";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.FileName;
                SetSizeMode(fileName);
                pictureBox1.ImageLocation = fileName;
                
                
                string[] extensions = new[] { ".bmp", ".jpg", ".png", ".tif", ".gif", ".psd" };
                pics = new DirectoryInfo(Path.GetDirectoryName(fileName)).GetFiles().
                       Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
               i = 0;
                foreach (var item in pics)
                {
                    if (item.FullName == fileName)
                    {
                        break;
                    }
                    else
                    {
                        imageDataGridView.Columns.Add(item.ToString(),i.ToString());
                        i++;
                    }
                    
                }
                SetText(pics[i].Name);


              //  width = pictureBox1.Image.Width;
              //  height = pictureBox1.Image.Height;

            }
           
            byte[] imageData;
           
            var originalImage = Image.FromFile(pictureBox1.ImageLocation);
            using (var ms = new MemoryStream())
            {
                originalImage.Save(ms, ImageFormat.Jpeg);
                imageData = ms.ToArray();
            }
            
            // Convert back to image.
            using (var ms = new MemoryStream(imageData))
            {
                Image image = Image.FromStream(ms);
                image.Save(@"j:\fefdfd2.jpg");
            }

                 pictureBox1.Image = Image.FromFile(fileName);
                 ImageToString();
                 Clipher c = new Clipher(ImageToString());
                 string encryptedImage="";
                 foreach (byte i in c.encrypted)
                 {
                     encryptedImage += i;
                     textBox2.Text +=" " + i.ToString();


                 }
         /*   Bitmap a = new Bitmap(width,height);
            int k = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    a.SetPixel(x, y, Color.FromArgb(22));
                    k++;
                }
                */


        }
    
        string ImageToString()
        {
            string s="";
            byte[] b;
            b = ImageToByte(pictureBox1.Image);

            foreach (byte i in b)
            {

                textBox1.Text += " " + i.ToString();
                s += i.ToString();

            }
            return s;
        }
        private void SetSizeMode(string fileName)
        {
            if (Image.FromFile(fileName).Width >= pictureBox1.Width ||
                Image.FromFile(fileName).Height >= pictureBox1.Height)
            {
                if (pictureBox1.SizeMode != PictureBoxSizeMode.Zoom)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            else
            {
                if (pictureBox1.SizeMode != PictureBoxSizeMode.CenterImage)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }
            }
        }
        private void SetText(string text)
        {
            Text = text + " - MyViewer";
        }

        private void Next_button_Click(object sender, EventArgs e)
        {
            NextImageLoad();
        }

        private void NextImageLoad()
        {
            if (pictureBox1.Image != null)
            {
                i++;
                if (pics.Length == i)
                {
                    i = 0;
                }
                string fileName = pics[i].FullName;
                pictureBox1.ImageLocation = fileName;
                SetSizeMode(fileName);
                SetText(pics[i].Name);

            }
        }

        private void Prev_button_Click(object sender, EventArgs e)
        {
            PrivImageLoad();
        }

        private void PrivImageLoad()
        {
            if (pictureBox1.Image != null)
            {
                if (i == 0)
                {
                    i = pics.Length;
                }
                i--;
                string fileName = pics[i].FullName;
                pictureBox1.ImageLocation = fileName;
                SetSizeMode(fileName);
                SetText(pics[i].Name);
            }

        }
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        public static byte[] ImageToByte2(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
      

        public static Bitmap ConvertByteToImage(byte[] bytes)
        {
            return (new Bitmap(Image.FromStream(new MemoryStream(bytes))));
        }
    }
}

