using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace WindowsFormsApp2
{
    public class Clipher
    {
        public string decrypt;
        readonly string Original;
        public byte[] encrypted;
        private byte[] imageData;

        public Clipher(string _original)
        {
            Original = _original;
          //  Original = "Here is some data to encrypt!";
            string decrypt;

            // Create a new instance of the Rijndael 
            // class.  This generates a new key and initialization  
            // vector (IV). 
            using (Rijndael myRijndael = Rijndael.Create())
            {
                // Encrypt the string to an array of bytes. 
                encrypted = EncryptStringToBytes(Original, myRijndael.Key, myRijndael.IV);

                // Decrypt the bytes to a string. 
                decrypt = DecryptStringFromBytes(encrypted, myRijndael.Key, myRijndael.IV);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", Original);
                Console.WriteLine("Round Trip: {0}", decrypt);
            }
        }

        public Clipher(byte[] imageData)
        {
            this.imageData = imageData;
        }

        public  byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
           
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
          
            using (Rijndael rijAlg = Rijndael.Create())
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

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        public  string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
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

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
