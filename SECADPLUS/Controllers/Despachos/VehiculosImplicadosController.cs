using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class VehiculosImplicadosController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        // GET: VehiculosImplicados
        [SecadPlusAutorizador("/VehiculosImplicados/Index")]
        public ActionResult Index(int? idCons, string strMensaje, string strTipo)
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

                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var maquina = db.CADCONEXIONs.Where(c => c.CADUSUA_USUARIO == usr.USUARIO && c.VIGENTE == "S").OrderByDescending(c => c.SESION_ID).Select(c => c.MAQUINA).First();

                if (string.IsNullOrWhiteSpace(maquina))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }

                var mensaje = "";
                var tipo = "";

                if (string.IsNullOrEmpty(strMensaje))
                    mensaje = string.Empty;
                else
                    mensaje = strMensaje;

                if (string.IsNullOrEmpty(strMensaje))
                    tipo = string.Empty;
                else
                    tipo = strTipo;

                if (idCons == null)
                {
                    idCons = 0;
                }

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;

                //traer usuario autenticado
                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                ViewBag.SitioID = usr.SITIO_GRABACION;

                var cADBIENES = new List<CADBIENE>();
                var cADBIENES_IMPLICADOS = db.CADBIENES_IMPLICADOS.Include(c => c.CADBIENE).Include(c => c.CADLUGARES_GEOGRAFICOS).Include(c => c.CADCASO).Include(c => c.CADBARRIO).Where(c => c.CADBIEN_SITIOGRABA == usr.SITIO_GRABACION && c.CADBIEN_CONSECUTIVO == idCons);

                var variosModelos = new Tuple<List<CADBIENE>, List<CADBIENES_IMPLICADOS>>
                    (cADBIENES.ToList(), cADBIENES_IMPLICADOS.ToList())
                { };

                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarVehiculos(int intSitio)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_V_Vehiculos> elementos = new List<Ext_V_Vehiculos>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;
                string strBoton = "";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CONSECUTIVO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.CLASE, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.LINEA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.PLACA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.USUA_TRANSACCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CLASE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.MARCA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.LINEA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.COLOR " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.PLACA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdenamiento += " ORDER BY INTERNO.CHASIS " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 9:
                        strOrdenamiento += " ORDER BY INTERNO.MOTOR " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 10:
                        strOrdenamiento += " ORDER BY INTERNO.SERIE " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 11:
                        strOrdenamiento += " ORDER BY INTERNO.OBSERVACIONES " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 12:
                        strOrdenamiento += " ORDER BY INTERNO.USUA_TRANSACCION " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 13:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_TRANSACCION " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_vehi_implicados", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"VerCasos(" + intSitio + "," + fila["CONSECUTIVO"].ToString() + ")\" data -placement='bottom' data-original-title='Clic para ver Casos'> <i class='icon icon-trash'>" +
                    "</i> Ver casos </button> </li>" +
                    "</ul> </li></ul>";
                    elementos.Add(new Ext_V_Vehiculos(fila, strBoton));
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });

                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public ActionResult GetCasos(int idSitio, long idBien)
        {
            try
            {
                var cADBIENES_IMPLICADOS = db.CADBIENES_IMPLICADOS.Include(c => c.CADBIENE).Include(c => c.CADLUGARES_GEOGRAFICOS).Include(c => c.CADCASO).Include(c => c.CADBARRIO).Where(c => c.CADBIEN_SITIOGRABA == idSitio && c.CADBIEN_CONSECUTIVO == idBien).OrderBy(c => c.CONSECUTIVO);
                return PartialView("_casos", cADBIENES_IMPLICADOS.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}