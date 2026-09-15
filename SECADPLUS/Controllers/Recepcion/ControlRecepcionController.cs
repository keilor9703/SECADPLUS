using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class ControlRecepcionController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: ControlRecepcion
        [SecadPlusAutorizador("/ControlRecepcion/Index")]
        public ActionResult Index()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalUsuario = usr.USUARIO;
                //ViewBag.globalSitio = usr.SITIO_GRABACION;
                //ViewBag.globalIdentificacion = usr.IDENTIFICACION;
                //ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;
                //ViewBag.globalCanal = usr.CADCANA_CODIGO;

                var tiempoRefresco = db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "TIEMPO_CONTROL_USUARIOS").Select(c => c.DESCRIPCION).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoRefresco))
                {
                    tiempoRefresco = "15000"; // 15 segundos por defecto

                    ViewBag.MENSAJE_EXITOSO = "Revise en cadreferencias el valor para TIEMPO_CONTROL_USUARIOS.. Se establecen por defecto 60 segundos..";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }

                ViewBag.tiempoRefresco = int.Parse(tiempoRefresco);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerControlRecepcion()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "";
                string strLimite = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<ControlRecepcion> listaControl = new List<ControlRecepcion>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 4)
                {
                    blFiltro = true;
                    strQuery += " WHERE REGEXP_LIKE (USUARIO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (LOGIN, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (EQUIPO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (UBICACION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (RECIBIDAS, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (EFECTIVAS, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (ULTIMA_FECHA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (PEDIDO_SIN_ENVIO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (NOMBRE, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (TURNO, '" + parametros.ConsultaValor + "', 'i') ";
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_control_recepcion", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaControl.Add(new ControlRecepcion(fila));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaControl });
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