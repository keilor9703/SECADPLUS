using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace SECADPLUS.Models.Vms
{
    /// <summary>
    /// Cliente de la REST API de ISS SecurOS (SecurOS REST API Programming Guide).
    /// Usa autenticación Basic con un usuario técnico de SecurOS con permiso de solo visualización
    /// sobre las cámaras: con ese usuario SECAD no puede mover ni controlar las cámaras.
    /// </summary>
    public class ClienteSecurOS
    {
        private readonly string strUrlBase;
        private readonly string strAutorizacion;
        private readonly int intTimeOutMs;

        public ClienteSecurOS()
        {
            strUrlBase = (ConfigurationManager.AppSettings["VMS_SecurOS_Url"] ?? "").TrimEnd('/');
            string strUsuario = ConfigurationManager.AppSettings["VMS_SecurOS_Usuario"] ?? "";
            string strClave = ConfigurationManager.AppSettings["VMS_SecurOS_Clave"] ?? "";
            if (string.IsNullOrEmpty(strUrlBase) || string.IsNullOrEmpty(strUsuario))
            {
                throw new ConfigurationErrorsException("La integración VMS no está configurada (VMS_SecurOS_Url / VMS_SecurOS_Usuario).");
            }
            strAutorizacion = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(strUsuario + ":" + strClave));
            int intSegundos;
            intTimeOutMs = (int.TryParse(ConfigurationManager.AppSettings["VMS_SecurOS_TimeOutSegundos"], out intSegundos) ? intSegundos : 15) * 1000;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// GET /api/v1/cameras: catálogo de cámaras con coordenadas y estado.
        /// </summary>
        public List<CamaraVms> ObtenerCamaras()
        {
            List<CamaraVms> camaras = new List<CamaraVms>();
            JObject respuesta = JObject.Parse(Encoding.UTF8.GetString(Get("/api/v1/cameras", "application/json")));
            JArray datos = respuesta["data"] as JArray;
            if (datos == null)
            {
                return camaras;
            }
            foreach (JToken item in datos)
            {
                string strTipo = (string)item["type"];
                if (!string.IsNullOrEmpty(strTipo) && !strTipo.Equals("CAM", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                JToken settings = item["settings"];
                JToken status = item["status"];
                double? latitud, longitud;
                LeerCoordenadas(settings, out latitud, out longitud);
                camaras.Add(new CamaraVms
                {
                    IdVms = (string)item["id"],
                    Nombre = ((string)item["name"] ?? "").Trim(),
                    Latitud = latitud,
                    Longitud = longitud,
                    Estado = status == null ? null : (string)status["state"],
                    Habilitada = status == null || status["enabled"] == null || (bool)status["enabled"],
                    Ptz = settings != null && !string.IsNullOrEmpty((string)settings["telemetry_id"])
                });
            }
            return camaras;
        }

        /// <summary>
        /// GET /api/v2/cameras/{id}/image: cuadro actual de la cámara en JPEG, escalado al tamaño pedido.
        /// </summary>
        public byte[] ObtenerImagen(string strIdVms, int intAncho, int intAlto)
        {
            string strRuta = "/api/v2/cameras/" + Uri.EscapeDataString(strIdVms) + "/image";
            if (intAncho > 0 && intAlto > 0)
            {
                strRuta += "?scale_x=" + intAncho + "&scale_y=" + intAlto;
            }
            return Get(strRuta, "image/jpeg");
        }

        /// <summary>
        /// Las coordenadas llegan como texto: settings.latitude / settings.longitude
        /// o, en versiones anteriores, settings.coordinates con el formato "lat, lon".
        /// </summary>
        private static void LeerCoordenadas(JToken settings, out double? latitud, out double? longitud)
        {
            latitud = null;
            longitud = null;
            if (settings == null)
            {
                return;
            }
            latitud = ConvertirNumero((string)settings["latitude"]);
            longitud = ConvertirNumero((string)settings["longitude"]);
            if (latitud.HasValue && longitud.HasValue)
            {
                return;
            }
            string strCoordenadas = (string)settings["coordinates"];
            if (string.IsNullOrWhiteSpace(strCoordenadas))
            {
                return;
            }
            string[] partes = strCoordenadas.Split(',');
            if (partes.Length == 2)
            {
                latitud = ConvertirNumero(partes[0]);
                longitud = ConvertirNumero(partes[1]);
            }
        }

        public static double? ConvertirNumero(string strValor)
        {
            double dbValor;
            if (string.IsNullOrWhiteSpace(strValor))
            {
                return null;
            }
            if (double.TryParse(strValor.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out dbValor))
            {
                return dbValor;
            }
            return null;
        }

        private byte[] Get(string strRuta, string strAccept)
        {
            HttpWebRequest peticion = (HttpWebRequest)WebRequest.Create(strUrlBase + strRuta);
            peticion.Method = "GET";
            peticion.Accept = strAccept;
            peticion.Timeout = intTimeOutMs;
            peticion.ReadWriteTimeout = intTimeOutMs;
            peticion.Headers[HttpRequestHeader.Authorization] = strAutorizacion;
            try
            {
                using (HttpWebResponse respuesta = (HttpWebResponse)peticion.GetResponse())
                using (Stream flujo = respuesta.GetResponseStream())
                using (MemoryStream memoria = new MemoryStream())
                {
                    flujo.CopyTo(memoria);
                    return memoria.ToArray();
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse respuesta = ex.Response as HttpWebResponse;
                string strDetalle = respuesta == null ? ex.Status.ToString() : ((int)respuesta.StatusCode).ToString();
                // La ruta nunca incluye credenciales, por lo que puede quedar en el log.
                throw new InvalidOperationException("SecurOS no respondió a " + strRuta + " (" + strDetalle + ").", ex);
            }
        }
    }
}
