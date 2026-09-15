using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class IngresarMapaController : Controller
    {
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: IngresarMapa
        [SecadPlusAutorizador("/IngresarMapa/Index")]
        public ActionResult Index(string strOrigen = null, string strNumeLlamada = null)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                GestorBase.ConsultarUsuarioSecad();
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.MapaBase = "";
                ViewBag.MapaBaseSecundario = "";
                ViewBag.MapaBase = ConfigurationManager.AppSettings["MapaBasePrincipal"].ToString();
                ViewBag.MapaBaseSecundario = ConfigurationManager.AppSettings["MapaBaseSecundario"].ToString();
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                if (string.IsNullOrEmpty(strNumeLlamada))
                {
                    ViewBag.Nume_Llamada = -1;
                }
                else
                {
                    ViewBag.Nume_Llamada = strNumeLlamada;
                }

                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalUsuario = usr.USUARIO;

                //---------------------------------------------------------------------------------------------------------//
                //traer tiempos de recarga y consulta.
                var tiempoRecarda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == usr.USUARIO && c.TIPO == "TIEMPO_RECARGA").Select(c => c.PARAMETRO).FirstOrDefault();
                var tiempoBusqueda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == usr.USUARIO && c.TIPO == "TIEMPO_DB_CONSULTA").Select(c => c.PARAMETRO).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoRecarda) || string.IsNullOrWhiteSpace(tiempoBusqueda))
                {
                    tiempoRecarda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == "TODOS LOS USUARIOS" && c.TIPO == "TIEMPO_RECARGA").Select(c => c.PARAMETRO).FirstOrDefault();
                    tiempoBusqueda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == "TODOS LOS USUARIOS" && c.TIPO == "TIEMPO_DB_CONSULTA").Select(c => c.PARAMETRO).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(tiempoRecarda) || string.IsNullOrWhiteSpace(tiempoBusqueda))
                    {
                        GestorBase.MSN_ALERTA_COMUN = "No hay parametros asignados para el o los usuarios, revise...";
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.ORIGEN = "";
                if (!string.IsNullOrEmpty(strOrigen))
                    ViewBag.ORIGEN = strOrigen;
                ViewBag.tiempoRecarda = tiempoRecarda;
                ViewBag.tiempoBusqueda = tiempoBusqueda;
                ViewBag.USUARIO = new SelectList(new[] { new { value = usr.USUARIO, text = usr.USUARIO },
                                                         new { value = "TODOS LOS USUARIOS" , text = "TODOS LOS USUARIOS" }
                                                             }, "value", "text");
                ViewBag.UsuarioSeleccion = "None";
                if (!string.IsNullOrEmpty(strNumeLlamada))
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = usr.USUARIO, text = usr.USUARIO } }, "value", "text");
                    ViewBag.UsuarioSeleccion = usr.USUARIO;
                }

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult MapaRecepcion(string strNumeLlamada = null)
        {
            if (Session["USUARIO_SECAD"] == null)
            {
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(strNumeLlamada))
            {
                ViewBag.Nume_Llamada = -1;
            }
            else
            {
                ViewBag.Nume_Llamada = strNumeLlamada;
            }

            //---------------------------------------------------------------------------------------------------------//
            //traer usuario autenticado - almacenar datos de usuario en variable globales.
            CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
            ViewBag.globalSitio = usr.SITIO_GRABACION;
            ViewBag.globalUsuario = usr.USUARIO;
            ViewBag.MapaBase = "";
            ViewBag.MapaBaseSecundario = "";
            ViewBag.MapaBase = ConfigurationManager.AppSettings["MapaBasePrincipal"].ToString();
            ViewBag.MapaBaseSecundario = ConfigurationManager.AppSettings["MapaBaseSecundario"].ToString();
            //---------------------------------------------------------------------------------------------------------//
            //traer tiempos de recarga y consulta.
            var tiempoRecarda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == usr.USUARIO && c.TIPO == "TIEMPO_RECARGA").Select(c => c.PARAMETRO).FirstOrDefault();
            var tiempoBusqueda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == usr.USUARIO && c.TIPO == "TIEMPO_DB_CONSULTA").Select(c => c.PARAMETRO).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(tiempoRecarda) || string.IsNullOrWhiteSpace(tiempoBusqueda))
            {
                tiempoRecarda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == "TODOS LOS USUARIOS" && c.TIPO == "TIEMPO_RECARGA").Select(c => c.PARAMETRO).FirstOrDefault();
                tiempoBusqueda = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == "TODOS LOS USUARIOS" && c.TIPO == "TIEMPO_DB_CONSULTA").Select(c => c.PARAMETRO).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoRecarda) || string.IsNullOrWhiteSpace(tiempoBusqueda))
                {
                    GestorBase.MSN_ALERTA_COMUN = "No hay parametros asignados para el o los usuarios, revise...";
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.tiempoRecarda = tiempoRecarda;
            ViewBag.tiempoBusqueda = tiempoBusqueda;
            ViewBag.USUARIO = new SelectList(new[] { new { value = usr.USUARIO, text = usr.USUARIO },
                                                     new { value = "TODOS LOS USUARIOS" , text = "TODOS LOS USUARIOS" }
                                                         }, "value", "text");
            ViewBag.UsuarioSeleccion = "None";
            if (!string.IsNullOrEmpty(strNumeLlamada))
            {
                ViewBag.USUARIO = new SelectList(new[] { new { value = usr.USUARIO, text = usr.USUARIO } }, "value", "text");
                ViewBag.UsuarioSeleccion = usr.USUARIO;
            }
            //return PartialView("Index",null);
            return View("Index", "MapaBase", null);
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarCapasMapa(string strUsuario)
        {
            try
            {
                var listaCapas = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == strUsuario && c.TIPO == "CAPA").OrderBy(c => c.CODIGO).Select(c => new { URL = c.PARAMETRO, NOMBRE = c.NOMBRE }).ToList();

                if (listaCapas.Count == 0)
                    listaCapas = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == "TODOS LOS USUARIOS" && c.TIPO == "CAPA").OrderBy(c => c.CODIGO).Select(c => new { URL = c.PARAMETRO, NOMBRE = c.NOMBRE }).ToList();

                var resultados = Json(new { result = true, lista = listaCapas });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarCasosMapa(int idSitio, int tiempoBusqueda, string strFecha, string strConsecutivo, string strUsuario, string strNume_Llamada = null, string strOrigen = null)
        {
            try
            {
                /*string strQuery = "", strComplementoFecha = "", strProcedimiento = ".PK_CADPLUS.p_lista_casos_mapa";
                if (!string.IsNullOrEmpty(strOrigen))
                {
                    strProcedimiento = ".PK_CADPLUS.p_lista_casos_mapa_desp";
                }
                if (strUsuario == "TODOS LOS USUARIOS")
                    strQuery = " WHERE CADPEDI_SITIOGRABA = " + idSitio + " AND TIME_ BETWEEN (sysdate - (" + tiempoBusqueda + "/24)) AND sysdate " + strComplementoFecha;
                else
                    strQuery = " WHERE CADPEDI_SITIOGRABA = " + idSitio + " AND TIME_ BETWEEN (sysdate - (" + tiempoBusqueda + "/24)) AND sysdate AND USUARIO_GRABA = '" + strUsuario + "' " + strComplementoFecha;                
                DataTable dttDatos;
                List<MapaPedidosUbicacion> listaUbicaciones = new List<MapaPedidosUbicacion>();

                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + strProcedimiento, strConexion, "Nonquery");

                foreach (DataRow fila in dttDatos.Rows)
                {
                    listaUbicaciones.Add(new MapaPedidosUbicacion(fila));
                }

                var resultados = Json(new { result = true, lista = listaUbicaciones });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;*/
                var resultados = Json(new { result = true, lista = "" });
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarPedidoMapa(int idSitio, long numeLlamada)
        {
            try
            {
                string strQuery = " WHERE CADPEDI_SITIOGRABA = " + idSitio + " AND CADPEDI_NUMELLAMADA = " + numeLlamada + " ";
                DataTable dttLugar;
                List<MapaPedidosUbicacionDatos> listaUbicaciones = new List<MapaPedidosUbicacionDatos>();

                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_datos_pedidos_mapa", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaUbicaciones.Add(new MapaPedidosUbicacionDatos(fila));
                }

                var resultados = Json(new { result = true, lista = listaUbicaciones });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}