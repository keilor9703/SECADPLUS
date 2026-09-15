using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SECADPLUS.Models
{
    public class ClsEncriptar
    {
        public static string Encriptar(string TextoSinCifrar)
        {
            string EncryptionKey = "MAKV2SPBNI99212"; // Clave de cifrado (debe ser mantenida en secreto)
            byte[] clearBytes = Encoding.Unicode.GetBytes(TextoSinCifrar); // Convierte el texto a bytes usando codificación Unicode
            using (Aes encryptor = Aes.Create()) // Crea un objeto AES para cifrar
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32); // Genera una clave de 32 bytes
                encryptor.IV = pdb.GetBytes(16);  // Genera un vector de inicialización (IV) de 16 bytes
                using (MemoryStream ms = new MemoryStream()) // Crea un flujo de memoria para almacenar los datos cifrados
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)) // Crea un flujo de cifrado
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length); // Escribe los bytes claros en el flujo de cifrado
                        cs.Close(); // Cierra el flujo de cifrado
                    }
                    TextoSinCifrar = Convert.ToBase64String(ms.ToArray()); // Convierte los bytes cifrados a una cadena Base64
                }
            }
            return TextoSinCifrar; // Devuelve el texto cifrado en formato Base64
        }

        public static string Desencriptar(string TextoCifrado)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212"; // Clave de cifrado (debe ser la misma usada para cifrar)
                TextoCifrado = TextoCifrado.Replace(" ", "+"); // Reemplaza espacios por signos de más (requerido para Base64)
                byte[] cipherBytes = Convert.FromBase64String(TextoCifrado); // Convierte la cadena Base64 a bytes cifrados
                using (Aes encryptor = Aes.Create()) // Crea un objeto AES para descifrar
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32); // Genera una clave de 32 bytes
                    encryptor.IV = pdb.GetBytes(16);  // Genera un vector de inicialización (IV) de 16 bytes
                    using (MemoryStream ms = new MemoryStream()) // Crea un flujo de memoria para almacenar los datos descifrados
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)) // Crea un flujo de descifrado
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length); // Escribe los bytes cifrados en el flujo de descifrado
                            cs.Close(); // Cierra el flujo de descifrado
                        }
                        TextoCifrado = Encoding.Unicode.GetString(ms.ToArray()); // Convierte los bytes descifrados a una cadena usando codificación Unicode
                    }
                }
                return TextoCifrado; // Devuelve el texto descifrado
            }
            catch (Exception)
            {
                TextoCifrado = "123"; // Valor por defecto en caso de error
                return TextoCifrado; // Devuelve el valor por defecto
            }
        }

    }
}