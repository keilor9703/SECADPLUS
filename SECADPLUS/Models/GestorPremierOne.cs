using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace SECADPLUS.Models
{
    /// <summary>
    /// Cliente HTTP para la integración SECAD -> PremierOne.
    /// Se encarga únicamente de la autenticación (JWT) y del envío del caso
    /// al endpoint receive-call-json. La construcción del payload y la carga
    /// de datos del caso se realizan en el controlador (RecepcionController).
    ///
    /// Toda la parametrización (URLs, credenciales, identidad del origen) se
    /// lee desde Web.config (appSettings con prefijo "PremierOne_").
    /// </summary>
    public class GestorPremierOne : GestorBase
    {
        #region Configuración (Web.config)

        public static string LoginUrl
        {
            get { return ConfigurationManager.AppSettings["PremierOne_LoginUrl"]; }
        }

        public static string ReceiveUrl
        {
            get { return ConfigurationManager.AppSettings["PremierOne_ReceiveUrl"]; }
        }

        private static string Usuario
        {
            get { return ConfigurationManager.AppSettings["PremierOne_Username"]; }
        }

        private static string Clave
        {
            get { return ConfigurationManager.AppSettings["PremierOne_Password"]; }
        }

        /// <summary>
        /// Código de la fuerza/entidad asociada al canal "PremierOne".
        /// Cuando EnviarLlamada retorna este idFuerzaEntidad, se dispara el envío.
        /// </summary>
        public static decimal FuerzaId
        {
            get
            {
                decimal valor;
                return decimal.TryParse(ConfigurationManager.AppSettings["PremierOne_FuerzaId"], out valor) ? valor : 0;
            }
        }

        /// <summary>
        /// Interruptor general para habilitar/deshabilitar el envío sin recompilar.
        /// </summary>
        public static bool Habilitado
        {
            get
            {
                bool valor;
                return bool.TryParse(ConfigurationManager.AppSettings["PremierOne_Habilitado"], out valor) && valor;
            }
        }

        public static int SourceId
        {
            get
            {
                int valor;
                return int.TryParse(ConfigurationManager.AppSettings["PremierOne_SourceId"], out valor) ? valor : 1;
            }
        }

        public static string SourceSystem
        {
            get { return ConfigurationManager.AppSettings["PremierOne_SourceSystem"] ?? "SECADDECUN"; }
        }

        public static string RecordingDb
        {
            get { return ConfigurationManager.AppSettings["PremierOne_RecordingDb"] ?? "3947"; }
        }

        public static string CallNumberPrefix
        {
            get { return ConfigurationManager.AppSettings["PremierOne_CallNumberPrefix"] ?? "DECUN-"; }
        }

        public static string FuenteTransaccion
        {
            get { return ConfigurationManager.AppSettings["PremierOne_FuenteTransaccion"] ?? "DECUN"; }
        }

        public static string SistemaInformacion
        {
            get { return ConfigurationManager.AppSettings["PremierOne_SistemaInformacion"] ?? "SECAD"; }
        }

        #endregion

        /// <summary>
        /// Autentica contra PremierOne y devuelve el token JWT.
        /// Lanza WebException/Exception si falla (manejada por el llamador).
        /// </summary>
        public string ObtenerToken()
        {
            string cuerpo = JsonConvert.SerializeObject(new { username = Usuario, password = Clave });
            string respuesta = EjecutarPost(LoginUrl, cuerpo, null);

            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return null;
            }

            JObject json = JObject.Parse(respuesta);
            // El servicio puede devolver la propiedad como "token" o "Token".
            JToken token = json["token"] ?? json["Token"];
            return token != null ? token.ToString() : null;
        }

        /// <summary>
        /// Envía el caso (payload ya serializado en JSON) al endpoint de PremierOne.
        /// Reautentica una sola vez si recibe 401. Devuelve el cuerpo de la respuesta
        /// en 'respuestaServicio' y true/false según el resultado HTTP.
        /// </summary>
        public bool EnviarCaso(string jsonPayload, out string respuestaServicio)
        {
            respuestaServicio = "";
            try
            {
                string token = ObtenerToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    respuestaServicio = "No fue posible obtener el token de autenticación de PremierOne.";
                    return false;
                }

                try
                {
                    respuestaServicio = EjecutarPost(ReceiveUrl, jsonPayload, token);
                    return true;
                }
                catch (WebException exWeb)
                {
                    // 401 => token vencido: reintenta una vez con token nuevo.
                    HttpWebResponse errResponse = exWeb.Response as HttpWebResponse;
                    if (errResponse != null && errResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        token = ObtenerToken();
                        respuestaServicio = EjecutarPost(ReceiveUrl, jsonPayload, token);
                        return true;
                    }
                    respuestaServicio = LeerErrorWeb(exWeb);
                    return false;
                }
            }
            catch (WebException exWeb)
            {
                respuestaServicio = LeerErrorWeb(exWeb);
                return false;
            }
            catch (Exception ex)
            {
                respuestaServicio = "Error enviando caso a PremierOne: " + ex.Message;
                return false;
            }
        }

        #region Utilidades HTTP

        /// <summary>
        /// POST JSON genérico. Si 'token' no es nulo agrega el header Authorization Bearer.
        /// </summary>
        private string EjecutarPost(string url, string jsonBody, string token)
        {
            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Authorization", "Bearer " + token);
            }

            byte[] datos = Encoding.UTF8.GetBytes(jsonBody ?? "");
            request.ContentLength = datos.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(datos, 0, datos.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private string LeerErrorWeb(WebException ex)
        {
            try
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    string cuerpo = "";
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        cuerpo = reader.ReadToEnd();
                    }
                    return "HTTP " + (int)response.StatusCode + " " + response.StatusCode + " - " + cuerpo;
                }
                return "Error de conexión con PremierOne: " + ex.Status;
            }
            catch
            {
                return "Error de conexión con PremierOne: " + ex.Message;
            }
        }

        #endregion
    }
}
