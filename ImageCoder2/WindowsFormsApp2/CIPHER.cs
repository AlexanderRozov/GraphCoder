using System;
using System.Security.Cryptography;
using System.IO;

namespace WindowsFormsApp2
{
    /// <summary>
    /// Класс реализует шифрование по аналогичное методу https://ru.wikipedia.org/wiki/Advanced_Encryption_Standard
    /// </summary>
    public class CIPHER
    {
        public string decrypt; // данные для дешифровки
        readonly string Original; // данные для шифровки
        public byte[] encrypted; // зашифрованные биты
        private byte[] imageData;

        public CIPHER(string _original) // конструктор класса принимает стороку данных созданую их изображения
        {
            Original = _original;
      
            string decrypt;

           // Создаёт новый экземпляр класса Rijndael.Это генерирует новый ключ и вектор инициализации
            using (Rijndael myRijndael = Rijndael.Create())
            {
              
                encrypted = EncryptStringToBytes(Original, myRijndael.Key, myRijndael.IV);

           
                decrypt = DecryptStringFromBytes(encrypted, myRijndael.Key, myRijndael.IV);

            
                Console.WriteLine("Original:   {0}", Original);
                Console.WriteLine("Round Trip: {0}", decrypt);
            }
        }
        /// <summary>
        /// функция шифрования
        /// </summary>
        /// <param name="plainText">текст для шифровки</param>
        /// <param name="Key">ключ</param>
        /// <param name="IV">вектор инициализаций</param>
        /// <returns></returns>
        public  byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {

            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");// проверка на ошибку 1
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key"); // проверка на ошибку 2
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key"); 
            byte[] encrypted; /// массиив куда положим шифрованное
          
            using (Rijndael rijAlg = Rijndael.Create()) // передаём паараметры фукции
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

             
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                           //кодируем переданный текст
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }
        /// <summary>
        /// функция дешифрования
        /// </summary>
        /// <param name="plainText">текст для шифровки</param>
        /// <param name="Key">ключ</param>
        /// <param name="IV">вектор инициализаций</param>
        /// <returns></returns>
        public string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an Rijndael object 
            // with the specified key and IV. 
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                           
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
