using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MVC2013.Src.Seguridad.Util
{
    public class CryptoKeyUtil
    {
        public static string CreatePrivateKey(int id)
        {
            string timestamp = DateTime.UtcNow.ToString("ddMMyyyyHHmmssff", CultureInfo.InvariantCulture);
            string input = id + timestamp;
            MD5 md5 = MD5.Create();
            byte[] buffer = Encoding.Default.GetBytes(input);
            byte[] hash = md5.ComputeHash(buffer);
            Guid result = new Guid(hash);
            return result.ToString().Replace("-", "").ToUpper();
        }

        public static string CreatePublicKey(string privateKey)
        {
            MD5 md5 = MD5.Create();
            byte[] buffer = Encoding.Default.GetBytes(privateKey);
            byte[] hash = md5.ComputeHash(buffer);
            Guid result = new Guid(hash);
            return result.ToString().Replace("-", "").ToUpper();
        }
    }
}