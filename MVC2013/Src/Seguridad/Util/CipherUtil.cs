using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MVC2013.Src.Seguridad.Util
{
    public class CipherUtil
    {

        private const string key = "d1e8ec2ac800b5d109dbbaafcd9e0bf0";

        /// <summary>
        /// Encrypts a given password and returns the encrypted data
        /// as a base64 string.
        /// </summary>
        /// <param name="plainText">An unencrypted string that needs
        /// to be secured.</param>
        /// <returns>A base64 encoded string that represents the encrypted
        /// binary data.
        /// </returns>
        /// <remarks>This solution is not really secure as we are
        /// keeping strings in memory. If runtime protection is essential,
        /// <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="plainText"/>
        /// is a null reference.</exception>
        public static string Encrypt(string decrypted)
        {
            //arreglo de bytes donde guardaremos la llave
            byte[] keyArray;

            //arreglo de bytes donde guardaremos el texto
            //que vamos a encriptar
            byte[] arrayToEncrypt = UTF8Encoding.UTF8.GetBytes(decrypted);

            //se utilizan las clases de encriptación
            //provistas por el Framework
            //Algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

            //se guarda la llave para que se le realice
            //hashing
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            //Algoritmo 3DAS
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            //se empieza con la transformación de la cadena
            ICryptoTransform cTransform = tdes.CreateEncryptor();

            //arreglo de bytes donde se guarda la
            //cadena cifrada
            byte[] ArrayResultado = cTransform.TransformFinalBlock(arrayToEncrypt, 0, arrayToEncrypt.Length);
            tdes.Clear();
            
            //se regresa el resultado en forma de una cadena
            return Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
        }

        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="cipher">A base64 encoded string that was created
        /// through the <see cref="Encrypt(string)"/> or
        /// <see cref="Encrypt(SecureString)"/> extension methods.</param>
        /// <returns>The decrypted string.</returns>
        /// <remarks>Keep in mind that the decrypted string remains in memory
        /// and makes your application vulnerable per se. If runtime protection
        /// is essential, <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        /// is a null reference.</exception>
        public static string Decrypt(string encrypted)
        {
            byte[] keyArray;

            //convierte el texto en una secuencia de bytes
            byte[] Array_a_Descifrar = Convert.FromBase64String(encrypted);

            //se llama a las clases que tienen los algoritmos
            //de encriptación se le aplica hashing
            //algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            keyArray = hashmd5.ComputeHash(
            UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();  
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);
            tdes.Clear();

            //se regresa en forma de cadena
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    

    }
}