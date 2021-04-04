using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Exercise1.Common.Security
{
    public class AesCryptoUtil //: IAesCryptoUtil
    {
        private static byte[] Key = new byte[] {
            0x2F, 0xFA, 0x72, 0x68, 0x7, 0x3C, 0xF6, 0xCC, 0x18, 0x8E, 0x20, 0xD7, 0x7E, 0x71, 0x20, 0x7E, 0x65, 0x98, 0xDB, 0xCE, 0xDF, 0x8, 0xE5, 0x57, 0x95, 0xB0, 0xDB, 0xC1, 0x83, 0x41, 0x15, 0x6A
        };
        private static byte[] IV = new byte[] { 
            0x96, 0xA0, 0x20, 0xA5, 0xD6, 0x43, 0xC8, 0x9D, 0xB1, 0x7E, 0x8D, 0xCE, 0xA1, 0x9F, 0x35, 0xFD 
        };

        public static string Encrypt(string text)
        {
            byte[] buff = EncryptStringToBytes_Aes(text);
            return Convert.ToBase64String(buff);
        }

        public static string Decrypt(string base64String)
        {
            byte[] buff = Convert.FromBase64String(base64String);
            return DecryptStringFromBytes_Aes(buff);
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
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

        private static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

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

        // public static byte[] GetHash(string inputString)
        // {
        //     using (HashAlgorithm algorithm = SHA256.Create())
        //         return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        // }

        // public static string GetHashString(string inputString)
        // {
        //     StringBuilder sb = new StringBuilder();
        //     foreach (byte b in GetHash(inputString))
        //         sb.Append(b.ToString("X2"));

        //     return sb.ToString();
        // }

        public static string GetStringSha256Hash(string inputString)
        {
            if (String.IsNullOrEmpty(inputString))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(inputString);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}