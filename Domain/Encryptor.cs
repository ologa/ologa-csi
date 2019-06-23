using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VPPS.CSI.Domain
{
    public class Encryptor
    {
        public static byte[] Encrypt(string input)
        {
            byte[] reslt = new byte[100000];
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider key = new DESCryptoServiceProvider();

            // encrypt user input to compare
            string k = "WLFPSMXR";
            Byte[] key1 = new byte[16];
            key1 = Encoding.UTF8.GetBytes(k);
            byte[] vec = new byte[16];

            CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(key1, vec), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(encStream);
            sw.WriteLine(input);
            sw.Close();
            encStream.Close();
            reslt = ms.ToArray();
            ms.Close();

            return reslt;
        }
    }
}
