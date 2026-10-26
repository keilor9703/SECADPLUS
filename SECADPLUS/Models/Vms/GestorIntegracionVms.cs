using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SECADPLUS.Models.Vms
{
    /// <summary>
    /// Lógica de la integración SECAD - VMS ISS SecurOS (caso de uso 1):
    /// censo local de cámaras, cámaras cercanas al caso, enlaces temporales de video y auditoría.
    /// El video nunca pasa por SECAD: el navegador lo recibe del gateway de video (MediaMTX),
    /// que pregunta a SECAD si cada token es válido.
    /// </summary>
    public class GestorIntegracionVms
    {
        private const double RADIO_TIERRA_METROS = 6371000d;
        private static readonly Regex REGEX_ID_CAMARA = new Regex("^[A-Za-z0-9_-]{1,64}$");
        private static readonly object bloqueoSincronizacion = new object();
        private static readonly Dictionary<int, DateTime> ultimaSincronizacion = new Dictionary<int, DateTime>();

        private readonly string strEsquema = Resources.RecursoGeneral.BaseDats_Usuario;

        #region Configuración

        public static bool EstaHabilitada(int intSitio)
        {
            string strHabilitado = ConfigurationManager.AppSettings["VMS_Habilitado"] ?? "NO";
            return strHabilitado.Equals("SI", StringComparison.OrdinalIgnoreCase) && intSitio == SitioVms;
        }

        /// <summary>Sitio de grabación al que pertenecen las cámaras del VMS configurado.</summary>
        public static int SitioVms
        {
            get { return LeerEntero("VMS_SitioGraba", -1); }
        }

        public static bool EsIdCamaraValido(string strIdVms)
        {
            return !string.IsNullOrEmpty(strIdVms) && REGEX_ID_CAMARA.IsMatch(strIdVms);
        }

        private static int LeerEntero(string strClave, int intDefecto)
        {
            int intValor;
            return int.TryParse(ConfigurationManager.AppSettings[strClave], out intValor) ? intValor : intDefecto;
        }

        private static string Conexion()
        {
            return Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        }

        #endregion Configuración

        #region Censo de cámaras

        /// <summary>
        /// Sincroniza el censo si la última sincronización es más antigua que VMS_MinutosSincronizacion.
        /// Si el VMS no responde, se sigue trabajando con el censo guardado.
        /// </summary>
        public void SincronizarSiVencido(int intSitio)
        {
            int intMinutos = LeerEntero("VMS_MinutosSincronizacion", 10);
            lock (bloqueoSincronizacion)
            {
                DateTime dtUltima;
                if (ultimaSincronizacion.TryGetValue(intSitio, out dtUltima) && DateTime.Now - dtUltima < TimeSpan.FromMinutes(intMinutos))
                {
                    return;
                }
                // Se marca antes de sincronizar para no reintentar en cada petición si el VMS está caído.
                ultimaSincronizacion[intSitio] = DateTime.Now;
            }
            try
            {
                Sincronizar(intSitio);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name, "SincronizarSiVencido");
            }
        }

        /// <summary>
        /// Trae el catálogo del VMS y actualiza CADVMS_CAMARA. Las cámaras que ya no existen en el VMS
        /// quedan inactivas. Devuelve la cantidad de cámaras recibidas.
        /// </summary>
        public int Sincronizar(int intSitio)
        {
            List<CamaraVms> camaras = new ClienteSecurOS().ObtenerCamaras().Where(c => EsIdCamaraValido(c.IdVms)).ToList();
            string strMerge =
                "MERGE INTO " + strEsquema + ".CADVMS_CAMARA C " +
                "USING (SELECT :p_sitio SITIO_GRABA, :p_id ID_VMS FROM DUAL) S " +
                "ON (C.SITIO_GRABA = S.SITIO_GRABA AND C.ID_VMS = S.ID_VMS) " +
                "WHEN MATCHED THEN UPDATE SET C.NOMBRE = :p_nombre, C.LATITUD = :p_latitud, C.LONGITUD = :p_longitud, " +
                "  C.ESTADO = :p_estado, C.HABILITADA = :p_habilitada, C.PTZ = :p_ptz, C.ACTIVA = 'S', C.FECHA_SINCRONIZACION = :p_fecha " +
                "WHEN NOT MATCHED THEN INSERT (SITIO_GRABA, ID_VMS, NOMBRE, LATITUD, LONGITUD, ESTADO, HABILITADA, PTZ, ACTIVA, FECHA_SINCRONIZACION) " +
                "  VALUES (:p_sitio, :p_id, :p_nombre, :p_latitud, :p_longitud, :p_estado, :p_habilitada, :p_ptz, 'S', :p_fecha)";
            DateTime dtInicio = DateTime.Now;

            using (OracleConnection conexion = new OracleConnection(Conexion()))
            {
                conexion.Open();
                using (OracleTransaction transaccion = conexion.BeginTransaction())
                {
                    foreach (CamaraVms camara in camaras)
                    {
                        using (OracleCommand comando = new OracleCommand(strMerge, conexion))
                        {
                            comando.BindByName = true;
                            comando.Parameters.Add("p_sitio", OracleDbType.Int32).Value = intSitio;
                            comando.Parameters.Add("p_id", OracleDbType.Varchar2).Value = camara.IdVms;
                            comando.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = Recortar(camara.Nombre, 200);
                            comando.Parameters.Add("p_latitud", OracleDbType.Decimal).Value = CoordenadaValida(camara.Latitud, 90) ? (object)camara.Latitud.Value : DBNull.Value;
                            comando.Parameters.Add("p_longitud", OracleDbType.Decimal).Value = CoordenadaValida(camara.Longitud, 180) ? (object)camara.Longitud.Value : DBNull.Value;
                            comando.Parameters.Add("p_estado", OracleDbType.Varchar2).Value = (object)Recortar(camara.Estado, 30) ?? DBNull.Value;
                            comando.Parameters.Add("p_habilitada", OracleDbType.Char).Value = camara.Habilitada ? "S" : "N";
                            comando.Parameters.Add("p_ptz", OracleDbType.Char).Value = camara.Ptz ? "S" : "N";
                            comando.Parameters.Add("p_fecha", OracleDbType.Date).Value = dtInicio;
                            comando.ExecuteNonQuery();
                        }
                    }
                    using (OracleCommand comando = new OracleCommand(
                        "UPDATE " + strEsquema + ".CADVMS_CAMARA SET ACTIVA = 'N' " +
                        "WHERE SITIO_GRABA = :p_sitio AND (FECHA_SINCRONIZACION IS NULL OR FECHA_SINCRONIZACION < :p_fecha)", conexion))
                    {
                        comando.BindByName = true;
                        comando.Parameters.Add("p_sitio", OracleDbType.Int32).Value = intSitio;
                        comando.Parameters.Add("p_fecha", OracleDbType.Date).Value = dtInicio;
                        comando.ExecuteNonQuery();
                    }
                    transaccion.Commit();
                }
            }
            return camaras.Count;
        }

        /// <summary>Verifica que la cámara exista y esté activa en el censo del sitio.</summary>
        public string ObtenerNombreCamara(int intSitio, string strIdVms)
        {
            if (!EsIdCamaraValido(strIdVms))
            {
                return null;
            }
            using (OracleConnection conexion = new OracleConnection(Conexion()))
            using (OracleCommand comando = new OracleCommand(
                "SELECT NOMBRE FROM " + strEsquema + ".CADVMS_CAMARA WHERE SITIO_GRABA = :p_sitio AND ID_VMS = :p_id AND ACTIVA = 'S'", conexion))
            {
                comando.BindByName = true;
                comando.Parameters.Add("p_sitio", OracleDbType.Int32).Value = intSitio;
                comando.Parameters.Add("p_id", OracleDbType.Varchar2).Value = strIdVms;
                conexion.Open();
                object resultado = comando.ExecuteScalar();
                return resultado == null || resultado == DBNull.Value ? null : resultado.ToString();
            }
        }

        #endregion Censo de cámaras

        #region Cámaras cercanas

        /// <summary>
        /// Coordenadas del caso tomadas del pedido (CADPEDIDO.LATITUD_CASO / LONGITUD_CASO).
        /// </summary>
        public bool ObtenerUbicacionPedido(int intSitio, long lgLlamada, out double dbLatitud, out double dbLongitud)
        {
            dbLatitud = 0;
            dbLongitud = 0;
            using (OracleConnection conexion = new OracleConnection(Conexion()))
            using (OracleCommand comando = new OracleCommand(
                "SELECT LATITUD_CASO, LONGITUD_CASO FROM " + strEsquema + ".CADPEDIDO WHERE SITIO_GRABA = :p_sitio AND NUME_LLAMADA = :p_llamada", conexion))
            {
                comando.BindByName = true;
                comando.Parameters.Add("p_sitio", OracleDbType.Int32).Value = intSitio;
                comando.Parameters.Add("p_llamada", OracleDbType.Int64).Value = lgLlamada;
                conexion.Open();
                using (OracleDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        return false;
                    }
                    double? latitud = ConvertirCoordenadaPedido(lector["LATITUD_CASO"] as string);
                    double? longitud = ConvertirCoordenadaPedido(lector["LONGITUD_CASO"] as string);
                    if (!CoordenadaValida(latitud, 90) || !CoordenadaValida(longitud, 180) || (latitud == 0 && longitud == 0))
                    {
                        return false;
                    }
                    dbLatitud = latitud.Value;
                    dbLongitud = longitud.Value;
                    return true;
                }
            }
        }

        /// <summary>
        /// Las N cámaras activas más cercanas dentro del radio. Primero se filtra por un recuadro
        /// (usa el índice de LATITUD/LONGITUD) y luego se ordena por distancia real (semiverseno).
        /// </summary>
        public List<CamaraCercana> ConsultarCamarasCercanas(int intSitio, double dbLatitud, double dbLongitud)
        {
            int intRadio = LeerEntero("VMS_RadioBusquedaMetros", 1000);
            int intCantidad = LeerEntero("VMS_CantidadCamaras", 6);
            double dbDeltaLatitud = intRadio / 111320d;
            double dbDeltaLongitud = intRadio / (111320d * Math.Max(Math.Cos(dbLatitud * Math.PI / 180d), 0.01));

            List<CamaraCercana> camaras = new List<CamaraCercana>();
            using (OracleConnection conexion = new OracleConnection(Conexion()))
            using (OracleCommand comando = new OracleCommand(
                "SELECT ID_VMS, NOMBRE, LATITUD, LONGITUD, ESTADO, HABILITADA FROM " + strEsquema + ".CADVMS_CAMARA " +
                "WHERE SITIO_GRABA = :p_sitio AND ACTIVA = 'S' " +
                "AND LATITUD BETWEEN :p_lat_min AND :p_lat_max AND LONGITUD BETWEEN :p_lon_min AND :p_lon_max", conexion))
            {
                comando.BindByName = true;
                comando.Parameters.Add("p_sitio", OracleDbType.Int32).Value = intSitio;
                comando.Parameters.Add("p_lat_min", OracleDbType.Decimal).Value = dbLatitud - dbDeltaLatitud;
                comando.Parameters.Add("p_lat_max", OracleDbType.Decimal).Value = dbLatitud + dbDeltaLatitud;
                comando.Parameters.Add("p_lon_min", OracleDbType.Decimal).Value = dbLongitud - dbDeltaLongitud;
                comando.Parameters.Add("p_lon_max", OracleDbType.Decimal).Value = dbLongitud + dbDeltaLongitud;
                conexion.Open();
                using (OracleDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        double dbLat = Convert.ToDouble(lector["LATITUD"], CultureInfo.InvariantCulture);
                        double dbLon = Convert.ToDouble(lector["LONGITUD"], CultureInfo.InvariantCulture);
                        double dbDistancia = DistanciaMetros(dbLatitud, dbLongitud, dbLat, dbLon);
                        if (dbDistancia > intRadio)
                        {
                            continue;
                        }
                        string strEstado = lector["ESTADO"] as string;
                        camaras.Add(new CamaraCercana
                        {
                            IdVms = lector["ID_VMS"].ToString(),
                            Nombre = lector["NOMBRE"] as string,
                            Latitud = dbLat,
                            Longitud = dbLon,
                            Estado = strEstado,
                            EnLinea = "S".Equals(lector["HABILITADA"] as string) && !EstadoSinSenal(strEstado),
                            DistanciaMetros = (int)Math.Round(dbDistancia)
                        });
                    }
                }
            }
            return camaras.OrderBy(c => c.DistanciaMetros).Take(intCantidad).ToList();
        }

        public static double DistanciaMetros(double dbLat1, double dbLon1, double dbLat2, double dbLon2)
        {
            double dbFi1 = dbLat1 * Math.PI / 180d, dbFi2 = dbLat2 * Math.PI / 180d;
            double dbDeltaFi = (dbLat2 - dbLat1) * Math.PI / 180d;
            double dbDeltaLambda = (dbLon2 - dbLon1) * Math.PI / 180d;
            double a = Math.Sin(dbDeltaFi / 2) * Math.Sin(dbDeltaFi / 2) +
                       Math.Cos(dbFi1) * Math.Cos(dbFi2) * Math.Sin(dbDeltaLambda / 2) * Math.Sin(dbDeltaLambda / 2);
            return RADIO_TIERRA_METROS * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static bool EstadoSinSenal(string strEstado)
        {
            return "DETACHED".Equals(strEstado, StringComparison.OrdinalIgnoreCase)
                || "DISARM_DETACHED".Equals(strEstado, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Cámaras cercanas

        #region Auditoría

        /// <summary>
        /// Deja constancia de quién abrió qué cámara, cuándo y contra qué caso.
        /// Se registra antes de entregar el enlace de video.
        /// </summary>
        public void RegistrarVisualizacion(int intSitio, long lgLlamada, string strIdVms, string strNombreCamara,
            string strUsuario, string strIp, string strMaquina)
        {
            using (OracleConnection conexion = new OracleConnection(Conexion()))
            using (OracleCommand comando = new OracleCommand(
                "INSERT INTO " + strEsquema + ".CADVMS_VISUALIZACION " +
                "(CONSECUTIVO, SITIO_GRABA, NUME_LLAMADA, ID_VMS, NOMBRE_CAMARA, USUARIO, DIRECCION_IP, MAQUINA, FECHA) " +
                "VALUES (" + strEsquema + ".SEQ_CADVMS_VISUALIZACION.NEXTVAL, :p_sitio, :p_llamada, :p_id, :p_nombre, :p_usuario, :p_ip, :p_maquina, SYSDATE)", conexion))
            {
                comando.BindByName = true;
                comando.Parameters.Add("p_sitio", OracleDbType.Int32).Value = intSitio;
                comando.Parameters.Add("p_llamada", OracleDbType.Int64).Value = lgLlamada;
                comando.Parameters.Add("p_id", OracleDbType.Varchar2).Value = strIdVms;
                comando.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = (object)Recortar(strNombreCamara, 200) ?? DBNull.Value;
                comando.Parameters.Add("p_usuario", OracleDbType.Varchar2).Value = Recortar(strUsuario, 50);
                comando.Parameters.Add("p_ip", OracleDbType.Varchar2).Value = (object)Recortar(strIp, 45) ?? DBNull.Value;
                comando.Parameters.Add("p_maquina", OracleDbType.Varchar2).Value = (object)Recortar(strMaquina, 100) ?? DBNull.Value;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        #endregion Auditoría

        #region Enlaces temporales de video

        /// <summary>
        /// Ruta del gateway para una cámara. Debe coincidir con la ruta configurada en mediamtx.yml.
        /// </summary>
        public static string RutaGateway(string strIdVms)
        {
            return "securos_" + strIdVms;
        }

        /// <summary>
        /// URL WHEP (WebRTC) del gateway para la cámara.
        /// </summary>
        public static string UrlWhep(string strIdVms)
        {
            string strGateway = (ConfigurationManager.AppSettings["VMS_Gateway_Url"] ?? "").TrimEnd('/');
            return strGateway + "/" + RutaGateway(strIdVms) + "/whep";
        }

        /// <summary>
        /// Token firmado (HMAC-SHA256) que autoriza ver una sola cámara durante VMS_TokenSegundos.
        /// No contiene credenciales del VMS.
        /// </summary>
        public static string EmitirToken(string strIdVms, string strUsuario)
        {
            int intSegundos = LeerEntero("VMS_TokenSegundos", 60);
            long lgVence = (long)(DateTime.UtcNow.AddSeconds(intSegundos) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            string strCarga = string.Join("|", "v1", strIdVms, (strUsuario ?? "").Replace("|", ""), lgVence.ToString(CultureInfo.InvariantCulture), Guid.NewGuid().ToString("N"));
            string strCargaB64 = Base64Url(Encoding.UTF8.GetBytes(strCarga));
            return strCargaB64 + "." + Base64Url(Firmar(strCargaB64));
        }

        /// <summary>
        /// Valida firma, vigencia y que el token corresponda a la ruta pedida al gateway.
        /// </summary>
        public static TokenVideo ValidarToken(string strToken, string strRuta)
        {
            if (string.IsNullOrEmpty(strToken) || strToken.Length > 1024)
            {
                return null;
            }
            string[] partes = strToken.Split('.');
            if (partes.Length != 2)
            {
                return null;
            }
            byte[] firmaRecibida;
            string strCarga;
            try
            {
                firmaRecibida = DesdeBase64Url(partes[1]);
                strCarga = Encoding.UTF8.GetString(DesdeBase64Url(partes[0]));
            }
            catch (FormatException)
            {
                return null;
            }
            if (!IgualesTiempoConstante(Firmar(partes[0]), firmaRecibida))
            {
                return null;
            }
            string[] campos = strCarga.Split('|');
            long lgVence;
            if (campos.Length != 5 || campos[0] != "v1" || !long.TryParse(campos[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out lgVence))
            {
                return null;
            }
            DateTime dtVence = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(lgVence);
            if (dtVence < DateTime.UtcNow || !RutaGateway(campos[1]).Equals(strRuta, StringComparison.Ordinal))
            {
                return null;
            }
            return new TokenVideo { IdVms = campos[1], Usuario = campos[2], Vence = dtVence };
        }

        /// <summary>Compara la clave compartida con el gateway sin filtrar información por tiempos.</summary>
        public static bool ClaveGatewayValida(string strClave)
        {
            string strEsperada = ConfigurationManager.AppSettings["VMS_Gateway_ClaveAutorizacion"] ?? "";
            if (strEsperada.Length < 16 || string.IsNullOrEmpty(strClave))
            {
                return false;
            }
            return IgualesTiempoConstante(Encoding.UTF8.GetBytes(strClave), Encoding.UTF8.GetBytes(strEsperada));
        }

        private static byte[] Firmar(string strTexto)
        {
            string strClave = ConfigurationManager.AppSettings["VMS_Token_Clave"] ?? "";
            if (strClave.Length < 32)
            {
                throw new ConfigurationErrorsException("VMS_Token_Clave debe tener al menos 32 caracteres.");
            }
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(strClave)))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(strTexto));
            }
        }

        private static bool IgualesTiempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            int intDiferencia = 0;
            for (int i = 0; i < a.Length; i++)
            {
                intDiferencia |= a[i] ^ b[i];
            }
            return intDiferencia == 0;
        }

        private static string Base64Url(byte[] datos)
        {
            return Convert.ToBase64String(datos).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] DesdeBase64Url(string strTexto)
        {
            string strBase64 = strTexto.Replace('-', '+').Replace('_', '/');
            switch (strBase64.Length % 4)
            {
                case 2: strBase64 += "=="; break;
                case 3: strBase64 += "="; break;
            }
            return Convert.FromBase64String(strBase64);
        }

        #endregion Enlaces temporales de video

        #region Utilidades

        private static double? ConvertirCoordenadaPedido(string strValor)
        {
            if (string.IsNullOrWhiteSpace(strValor))
            {
                return null;
            }
            // En el pedido las coordenadas se guardan como texto; se aceptan punto o coma decimal.
            return ClienteSecurOS.ConvertirNumero(strValor.Trim().Replace(',', '.'));
        }

        private static bool CoordenadaValida(double? valor, double dbLimite)
        {
            return valor.HasValue && !double.IsNaN(valor.Value) && Math.Abs(valor.Value) <= dbLimite;
        }

        private static string Recortar(string strTexto, int intLongitud)
        {
            if (strTexto == null)
            {
                return null;
            }
            return strTexto.Length <= intLongitud ? strTexto : strTexto.Substring(0, intLongitud);
        }

        #endregion Utilidades
    }
}
