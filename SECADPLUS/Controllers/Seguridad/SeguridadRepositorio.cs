using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SECADPLUS.Controllers.Seguridad
{
    public class SeguridadRepositorio
    {
        public string GenerateSHA256(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            SHA256Managed sHA256 = new SHA256Managed();
            byte[] hash = sHA256.ComputeHash(bytes);

            string hashString = string.Empty;
            foreach (byte x in hash)
                hashString += string.Format("{0:x2}", x);

            return hashString;
        }

        #region Rindjael

        public string GenerateKey()
        {
            SymmetricAlgorithm symmetricAlgorithm = new RijndaelManaged();
            symmetricAlgorithm.KeySize = 128;
            symmetricAlgorithm.GenerateKey();

            return Convert.ToBase64String(symmetricAlgorithm.Key);
        }


        public string Encrypt(string message, string key)
        {
            SymmetricAlgorithm symmetricAlgorithm = new RijndaelManaged();
            symmetricAlgorithm.KeySize = 128;
            symmetricAlgorithm.Key = Convert.FromBase64String(key);
            symmetricAlgorithm.Mode = CipherMode.ECB;

            ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();

            Byte[] data = Encoding.UTF8.GetBytes(message);

            var dataEncrypted = cryptoTransform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(dataEncrypted);
        }


        public string Decript(string message, string key)
        {
            SymmetricAlgorithm symmetricAlgorithm = new RijndaelManaged();
            symmetricAlgorithm.KeySize = 128;
            symmetricAlgorithm.Key = Convert.FromBase64String(key);
            symmetricAlgorithm.Mode = CipherMode.ECB;

            ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
            string mensajeSinEspacios = message.Replace(" ", "");
            Byte[] data = Convert.FromBase64String(mensajeSinEspacios);
            Byte[] dataDecrypted = cryptoTransform.TransformFinalBlock(data, 0, data.Length);

            return Encoding.UTF8.GetString(dataDecrypted);
        }
        #endregion

        public string ConvertirBytesBase64(string _dato)
        {
            byte[] conversionBytes = System.Text.Encoding.UTF8.GetBytes(_dato);
            string resultado = System.Convert.ToBase64String(conversionBytes);
            return resultado;
        }

        public string ConvertirBase64Bytes(string _dato)
        {
            byte[] conversionBase64 = System.Convert.FromBase64String(_dato);
            string resultado = System.Text.Encoding.UTF8.GetString(conversionBase64);
            return resultado;
        }
    }
}