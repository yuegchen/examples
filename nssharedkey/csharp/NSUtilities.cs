using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace NS_SK
{
    static class NSUtilities
    {
        public const int server_port = 11000;
        public const int Alice_port = 11001;
        public const int Bob_port = 11010;
        public const int loop = 50;
       
        public static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    
        public static string getString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

    public static Int64 getNonce()
        {
        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] result = new byte[64];
            rngCsp.GetBytes(result);
            return BitConverter.ToInt64(result, 0);
        }
    public static byte[] getKey(int size)
        {
        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] result = new byte[size];
            rngCsp.GetBytes(result);
            return result;
        }
    public static byte[] Encrypt(byte[] plaintext, byte[] key)
        {
            byte[] ciphertext = null;
            byte[] combinedIvCt = null;
        using (Aes aes_cipher = Aes.Create())
        {
        aes_cipher.KeySize = 256; //actually the default
        aes_cipher.Mode = CipherMode.CBC; //actually the default
        aes_cipher.Key = key;
        //Console.WriteLine(BitConverter.ToString(aes_cipher.IV));
        ICryptoTransform aes_encryptor = aes_cipher.CreateEncryptor();
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, aes_encryptor, CryptoStreamMode.Write))
            {
            cs.Write(plaintext, 0, plaintext.Length);
            }
                    ciphertext = ms.ToArray();
                    combinedIvCt = new byte[aes_cipher.IV.Length + ciphertext.Length];
            Array.Copy(aes_cipher.IV, 0, combinedIvCt, 0, aes_cipher.IV.Length);
            Array.Copy(ciphertext, 0, combinedIvCt, aes_cipher.IV.Length, ciphertext.Length);
                }
            }
            return combinedIvCt;
        }
    
        public static byte[] Decrypt(byte[] combinedIvCt, byte[] key)
        {
            byte[] plaintext = null;
        using (Aes aes_cipher = Aes.Create())
        {
        aes_cipher.KeySize = 256; //actually the default
        aes_cipher.Mode = CipherMode.CBC; //actually the default
        aes_cipher.Key = key;
        byte[] iv = new byte[aes_cipher.BlockSize/8];
        byte[] ciphertext = new byte[combinedIvCt.Length - iv.Length];
        Array.Copy(combinedIvCt, iv, iv.Length);
        Array.Copy(combinedIvCt, iv.Length, ciphertext, 0, ciphertext.Length);
        aes_cipher.IV = iv;
        //Console.WriteLine(BitConverter.ToString(aes_cipher.IV));
        ICryptoTransform aes_decryptor = aes_cipher.CreateDecryptor();
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, aes_decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(ciphertext, 0, ciphertext.Length);
                    }
                    plaintext = ms.ToArray();
                }   
            }
            return plaintext;
        }
    }
}
