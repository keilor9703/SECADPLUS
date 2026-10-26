using Newtonsoft.Json.Linq;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Vms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    /// <summary>
    /// Integración SECAD - VMS ISS SecurOS, caso de uso 1: desde un caso, ver las cámaras cercanas
    /// y abrir su video en vivo dentro de SECAD (solo visualización, sin control de la cámara).
    /// </summary>
    public class IntegracionVmsController : Controller
    {
        private Validador validar = new Validador();
        private GestorIntegracionVms gestor = new GestorIntegracionVms();

        /// <summary>
        /// Cámaras más cercanas a la ubicación del pedido, ordenadas por distancia.
        /// </summary>
        [SecadPlusAutorizador("/IntegracionVms/CamarasCercanas")]
        [HttpPost]
        public JsonResult CamarasCercanas(int intSitio, long lgLlamada)
        {
            try
            {
                CADUSUARIO usuario = validar.VerificarUsuario();
                string strError = ValidarAcceso(usuario, intSitio);
                if (strError != null)
                {
                    return Json(new { result = false, datos = strError });
                }
                double dbLatitud, dbLongitud;
                if (!gestor.ObtenerUbicacionPedido(intSitio, lgLlamada, out dbLatitud, out dbLongitud))
                {
                    return Json(new { result = false, datos = "El caso no tiene coordenadas. Ubique el caso en el mapa para consultar las cámaras cercanas." });
                }
                gestor.SincronizarSiVencido(intSitio);
                List<CamaraCercana> camaras = gestor.ConsultarCamarasCercanas(intSitio, dbLatitud, dbLongitud);
                return Json(new
                {
                    result = true,
                    datos = camaras.Select(c => new
                    {
                        id = c.IdVms,
                        nombre = c.Nombre,
                        distancia = c.DistanciaMetros,
                        enLinea = c.EnLinea,
                        estado = c.Estado,
                        latitud = c.Latitud,
                        longitud = c.Longitud
                    }),
                    caso = new { latitud = dbLatitud, longitud = dbLongitud }
                });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Miniatura actual de una cámara del censo. La imagen se pide a SecurOS desde el servidor,
        /// así las credenciales del VMS nunca llegan al navegador.
        /// </summary>
        [SecadPlusAutorizador("/IntegracionVms/CamarasCercanas")]
        [HttpGet]
        public ActionResult Miniatura(string idCamara)
        {
            try
            {
                CADUSUARIO usuario = validar.VerificarUsuario();
                if (usuario == null || ValidarAcceso(usuario, usuario.SITIO_GRABACION) != null
                    || gestor.ObtenerNombreCamara(usuario.SITIO_GRABACION, idCamara) == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                return File(new ClienteSecurOS().ObtenerImagen(idCamara, 320, 180), "image/jpeg");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
            }
        }

        /// <summary>
        /// Registra la visualización contra el caso y entrega el enlace temporal de video.
        /// </summary>
        [SecadPlusAutorizador("/IntegracionVms/AbrirVideo")]
        [HttpPost]
        public JsonResult AbrirVideo(string idCamara, int intSitio, long lgLlamada)
        {
            try
            {
                CADUSUARIO usuario = validar.VerificarUsuario();
                string strError = ValidarAcceso(usuario, intSitio);
                if (strError != null)
                {
                    return Json(new { result = false, datos = strError });
                }
                string strNombre = gestor.ObtenerNombreCamara(intSitio, idCamara);
                if (strNombre == null)
                {
                    return Json(new { result = false, datos = "La cámara no existe o no está activa." });
                }
                gestor.RegistrarVisualizacion(intSitio, lgLlamada, idCamara, strNombre, usuario.USUARIO,
                    Request.UserHostAddress, Session["MAQUINA"] == null ? null : Session["MAQUINA"].ToString());
                return Json(new
                {
                    result = true,
                    datos = new
                    {
                        nombre = strNombre,
                        urlWhep = GestorIntegracionVms.UrlWhep(idCamara),
                        token = GestorIntegracionVms.EmitirToken(idCamara, usuario.USUARIO)
                    }
                });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Fuerza la sincronización del censo de cámaras con el VMS.
        /// </summary>
        [SecadPlusAutorizador("/IntegracionVms/Sincronizar")]
        [HttpPost]
        public JsonResult Sincronizar()
        {
            try
            {
                CADUSUARIO usuario = validar.VerificarUsuario();
                string strError = ValidarAcceso(usuario, usuario == null ? -1 : usuario.SITIO_GRABACION);
                if (strError != null)
                {
                    return Json(new { result = false, datos = strError });
                }
                int intCamaras = gestor.Sincronizar(usuario.SITIO_GRABACION);
                return Json(new { result = true, datos = "Censo actualizado: " + intCamaras + " cámaras recibidas del VMS." });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Autorización externa del gateway de video (MediaMTX, authMethod: http).
        /// MediaMTX envía aquí cada solicitud de lectura; solo se permite leer la ruta de la cámara
        /// para la que SECAD emitió un token vigente. Cualquier otra acción se rechaza.
        /// La llamada la hace el gateway (no el navegador) y se valida con una clave compartida.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AutorizarStream(string clave)
        {
            try
            {
                if (!GestorIntegracionVms.ClaveGatewayValida(clave) || !IpGatewayPermitida(Request.UserHostAddress))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                string strCuerpo;
                Request.InputStream.Position = 0;
                using (StreamReader lector = new StreamReader(Request.InputStream))
                {
                    strCuerpo = lector.ReadToEnd();
                }
                JObject solicitud = JObject.Parse(strCuerpo);
                string strAccion = (string)solicitud["action"];
                string strRuta = (string)solicitud["path"];
                if (!"read".Equals(strAccion, StringComparison.Ordinal) || string.IsNullOrEmpty(strRuta))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                // El navegador envía el token como "Authorization: Bearer" y también en la URL (?token=).
                string strToken = (string)solicitud["token"];
                if (string.IsNullOrEmpty(strToken))
                {
                    strToken = HttpUtility.ParseQueryString((string)solicitud["query"] ?? "")["token"];
                }
                if (GestorIntegracionVms.ValidarToken(strToken, strRuta) == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        /// <summary>
        /// Devuelve null si el usuario puede usar la integración en el sitio; si no, el mensaje de error.
        /// </summary>
        private static string ValidarAcceso(CADUSUARIO usuario, int intSitio)
        {
            if (usuario == null)
            {
                return Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
            }
            if (usuario.SITIO_GRABACION != intSitio)
            {
                return "El caso no pertenece a su sitio de grabación.";
            }
            if (!GestorIntegracionVms.EstaHabilitada(intSitio))
            {
                return "La integración de cámaras no está habilitada para este sitio.";
            }
            return null;
        }

        private static bool IpGatewayPermitida(string strIp)
        {
            string strPermitidas = ConfigurationManager.AppSettings["VMS_Gateway_IpPermitidas"] ?? "";
            if (string.IsNullOrWhiteSpace(strPermitidas))
            {
                return true;
            }
            return strPermitidas.Split(',').Select(i => i.Trim()).Contains(strIp);
        }
    }
}
