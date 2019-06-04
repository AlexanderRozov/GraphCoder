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
            Setup(); // функция настроек
        }
        private int i;
        private FileInfo[] pics;

        /// <summary>
        /// функция настроек выбор файла
        /// </summary>
        private void Setup()
        {
            ofd.Filter = "Image Files(*.bmp, *.jpg, *.png, *.tif, *.gif, *.psd) | *.bmp; *.jpg; *.png; *.tif; *.gif; *.psd";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenImage(); // функция открывающая изображение
        }
        /// <summary>
        ///  функция открывающая изображение и 
        ///  ПРОИЗВОДЯЩАЯ КОДИРОВАНИЕ
        ///  ИДЕЯ СОСТОИТ В СЛЕДУЮЩЕМ КАЖДЫЙ ПИКСЕЛЮ ИЗОБРАЖЕНИЯ СТВИТЬСЯ В СООТВЕСТВИЕ НОМЕР - ЦВЕТ К ПРИМЕРУ (1 ,144 И ТД)
        ///  БЕРУТСЯ ЭТИ ЕОМЕРА И КОДИРУЮТСЯ МЕТОДОМ ОПИСАННЫМ В ФАЙЛЕ CIPHER (ШИФР ПОЯСНЕНИ Я ПОНЕ МУ МОЖЕТЕ ПРОЧИТАТЬ В САМОМ ФАЙЛЕ)
        /// </summary>
        private void OpenImage()
        {
          
            string fileName=""; // имя файла
            if (ofd.ShowDialog() == DialogResult.OK) // проверка на открытие
            {
                fileName = ofd.FileName; // тол имя которое ооткрыли присваиваем фалу
                SetSizeMode(fileName); // масштабируем изображение
                picture_box.ImageLocation = fileName; // выбираем файл на диске
                
                
                string[] extensions = new[] { ".bmp", ".jpg", ".png", ".tif", ".gif", ".psd" };// работаем с нужными файлами
                pics = new DirectoryInfo(Path.GetDirectoryName(fileName)).GetFiles(). 
                       Where(f => extensions.Contains(f.Extension.ToLower())).ToArray(); // делаем список всех изображений
               i = 0;
                foreach (var item in pics)
                {
                    if (item.FullName == fileName)
                    {
                        break;
                    }
                    else
                    {
                        
                        i++;
                    }
                    
                }
                SetText(pics[i].Name);


              //  width = pictureBox1.Image.Width;
              //  height = pictureBox1.Image.Height;

            }
           
            byte[] imageData; // данные для кодирования
           
            var originalImage = Image.FromFile(picture_box.ImageLocation); // берём оригинальную картинку для кодирования
            using (var ms = new MemoryStream()) // создаём поток данныных для кодирования using для того чтобы программыа не повисла если что то пойдёт не так
            {
                originalImage.Save(ms, ImageFormat.Jpeg); //  сохраняем картинку
                imageData = ms.ToArray(); // переводим её в массив  типа byte т.е тех чисел от (0-255 о которых говорилось выше)
            }
            
            ///это для обратного ковертирования
            using (var ms = new MemoryStream(imageData))
            {
                Image image = Image.FromStream(ms);
                image.Save(@"j:\fefdfd2.jpg");
            }

                 picture_box.Image = Image.FromFile(fileName);
                 ImageToString(); // преобразовываем картику в биты которые покажем во второй вкладке "представление битов"
                 CIPHER c = new CIPHER(ImageToString()); //класс шифра принимает информацию о картинке в виде битов и кодирует биты
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
        //  преобразует изображение в набор символов
        string ImageToString()
        {
            string s="";
            byte[] b;
            b = ImageToByte(picture_box.Image);

            foreach (byte i in b)
            {

                textBox1.Text += " " + i.ToString();
                s += i.ToString();

            }
            return s;
        }
        // фукция устанавиливет размер изображения рисуемого в picture_box
        private void SetSizeMode(string fileName)
        {
            if (Image.FromFile(fileName).Width >= picture_box.Width ||
                Image.FromFile(fileName).Height >= picture_box.Height)
            {
                if (picture_box.SizeMode != PictureBoxSizeMode.Zoom)
                {
                    picture_box.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            else
            {
                if (picture_box.SizeMode != PictureBoxSizeMode.CenterImage)
                {
                    picture_box.SizeMode = PictureBoxSizeMode.CenterImage;
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
        /// <summary>
        /// 
        /// загружает следущщую картинку в катологе
        /// </summary>
        private void NextImageLoad()
        {
            if (picture_box.Image != null)
            {
                i++;
                if (pics.Length == i)
                {
                    i = 0;
                }
                string fileName = pics[i].FullName;
                picture_box.ImageLocation = fileName;
                SetSizeMode(fileName);
                SetText(pics[i].Name);

            }
        }

        private void Prev_button_Click(object sender, EventArgs e)
        {
            PrivImageLoad();
        }
        /// <summary>
        /// 
        /// загружает предыдущую картинку в катологе
        /// </summary>
        private void PrivImageLoad()
        {
            if (picture_box.Image != null)
            {
                if (i == 0)
                {
                    i = pics.Length;
                }
                i--;
                string fileName = pics[i].FullName;
                picture_box.ImageLocation = fileName;
                SetSizeMode(fileName);
                SetText(pics[i].Name);
            }

        }
        /// <summary>
        /// непосредственно преобразует картинку в набор битов т.е массив типа byte[]
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
      
    }
}

