using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class IncidentesAntecedentesController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private Validador validar = new Validador();
        private CADUSUARIO usuarioActual;
        // GET: IncidentesAntecedentes
        public ActionResult Index(string strMensaje, string strTipo)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////

                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
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

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;
                ViewBag.sitio = usuarioActual.SITIO_GRABACION;

                return View("Index", "BaseRecepcion");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarIncidentes(string strSitio)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<IncidentesModel> elementos = new List<IncidentesModel>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                DateTime dtControl;
                bool blFiltro = false, blControl = false, blDtControl = false; ;
                string strBoton = "<a class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para ver el pedido' href='_RUTA' target='_blank'>"
                + "<i class='icon icon-eye'></i></a>";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor.Trim(), out dblControl);
                    //valida si debe filtrar por fecha
                    blDtControl = DateTime.TryParse(parametros.ConsultaValor.Trim(), out dtControl);
                    if (blDtControl)
                    {
                        strQuery = " AND (I.FECHA_GRABA =  to_date('" + parametros.ConsultaValor + "','DD/MM/YYYY HH24:MI:SS')) ";
                    }
                    else if (blControl)
                    {
                        strQuery = " AND ( I.PERS_CONSECUTIVO = " + parametros.ConsultaValor + " OR I.VEHI_CONSECUTIVO= " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " AND UPPER(I.USUA_GRABA) ='" + parametros.ConsultaValor.ToUpper() + "' ";
                    }

                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.PERS_CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.VEHI_CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.PATRULLA_SOLICITADA " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.INTEGRANTES_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.MOTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.COMENTARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrdenamiento += " ORDER BY INTERNO.USUA_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_GRABACION DESC  ";
                        break;
                }
                strLimites = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_incidentes_antecedentes", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);

                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new IncidentesModel(fila));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString()); ;
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
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
        [Authorize]
        public JsonResult GetPersonasIncidentes(string idUsuario)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var incidentes = gestor.ConsultarPersonasIncidentes(idUsuario, strConexion);

                var personas = (from DataRow dr in incidentes.Rows
                                select new
                                {
                                    CODIGO = dr["CONSECUTIVO"],
                                    DESCRIPCION = dr["CONSECUTIVO"] + " - " + dr["DATO"] + " - " + dr["SITIO_GRABA"],
                                }).ToList();

                return Json(personas);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        [Authorize]
        public JsonResult GetVehiculosIncidentes(string idUsuario)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var incidentes = gestor.ConsultarVehiculosIncidentes(idUsuario, strConexion);

                var vehiculos = (from DataRow dr in incidentes.Rows
                                 select new
                                 {
                                     CODIGO = dr["CONSECUTIVO"],
                                     DESCRIPCION = dr["CONSECUTIVO"] + " - " + dr["DATO"] + " - " + dr["SITIO_GRABA"],
                                 }).ToList();

                return Json(vehiculos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        // GET: IncidentesAntecedentes/Create
        public ActionResult Create()
        {
            try
            {
                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                var idSitio = usr.SITIO_GRABACION;

                // El sitio de grabación por medio de autenticación  //Validar que el usuario tenga un sitio de grabación para realizar todo.
                //ViewBag.SITIO_GRABA = 3947;
                ViewBag.SITIO_GRABA = idSitio;
                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = 0;

                List<SelectListItem> listaPersonas = new List<SelectListItem>();
                listaPersonas.Insert(0, (new SelectListItem { Text = "", Value = "" }));
                ViewBag.PERS_CONSECUTIVO = listaPersonas;

                List<SelectListItem> listaVehiculos = new List<SelectListItem>();
                listaVehiculos.Insert(0, (new SelectListItem { Text = "", Value = "" }));
                ViewBag.VEHI_CONSECUTIVO = listaVehiculos;

                List<SelectListItem> listaMotivos = new List<SelectListItem>();
                listaMotivos.Insert(0, (new SelectListItem { Text = "ERROR DE DIGITACIÓN", Value = "ED" }));
                listaMotivos.Insert(1, (new SelectListItem { Text = "ERROR DE PATRULLA", Value = "EP" }));
                listaMotivos.Insert(2, (new SelectListItem { Text = "HOMONIMO", Value = "HM" }));
                listaMotivos.Insert(3, (new SelectListItem { Text = "ANTECEDENTE SOLICITADO", Value = "AS" }));
                listaMotivos.Insert(4, (new SelectListItem { Text = "REALIZA PROCEDIMIENTO", Value = "RP" }));
                ViewBag.MOTIVO = listaMotivos;

                ViewBag.FECHA_GRABACION = DateTime.Now;

                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.USUA_GRABA = usuario;
                //ViewBag.USUA_GRABA = "USR_CAD";

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: IncidentesAntecedentes/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,TIPO_ANTECEDENTE,PERS_SITGR,PERS_CONSECUTIVO,VEHI_SITGR,VEHI_CONSECUTIVO,PATRULLA_SOLICITA,INTEGRANTES_PATRULLA,MOTIVO,COMENTARIO,FECHA_GRABACION,USUA_GRABA,PERS_ANIO_CONSULTA,VEHI_ANIO_CONSULTA")] CADINCIDENTES_ANTECEDENTE cADINCIDENTES_ANTECEDENTE)
        {
            try
            {
                var PERS_DESC = Request["PERS_DESC"];
                var VEHI_DESC = Request["VEHI_DESC"];

                var CODIGO = db.CADINCIDENTES_ANTECEDENTE.Where(c => c.SITIO_GRABA == cADINCIDENTES_ANTECEDENTE.SITIO_GRABA).Select(c => c.CONSECUTIVO).ToList().Max(c => c);
                CODIGO += 1;

                cADINCIDENTES_ANTECEDENTE.CONSECUTIVO = CODIGO;

                //User.Identity.GetUserName() cambiarlo por una nueva autenticación, hablar con pablo usr_cad.pk_cad.p_datos_usuarios_personas
                var comentario = "CONSULTA REALIZADA: " + PERS_DESC + " " + VEHI_DESC +
                             "Usuario: " + cADINCIDENTES_ANTECEDENTE.USUA_GRABA + ". " + User.Identity.GetUserName() + Environment.NewLine + cADINCIDENTES_ANTECEDENTE.COMENTARIO;

                cADINCIDENTES_ANTECEDENTE.COMENTARIO = comentario;

                cADINCIDENTES_ANTECEDENTE.FECHA_GRABACION = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.CADINCIDENTES_ANTECEDENTE.Add(cADINCIDENTES_ANTECEDENTE);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADINCIDENTES_ANTECEDENTE>(cADINCIDENTES_ANTECEDENTE, "CADINCIDENTES_ANTECEDENTE", cADINCIDENTES_ANTECEDENTE.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                List<SelectListItem> listaPersonas = new List<SelectListItem>();
                listaPersonas.Insert(0, (new SelectListItem { Text = "", Value = "" }));
                ViewBag.PERS_CONSECUTIVO = listaPersonas;

                List<SelectListItem> listaVehiculos = new List<SelectListItem>();
                listaVehiculos.Insert(0, (new SelectListItem { Text = "", Value = "" }));
                ViewBag.VEHI_CONSECUTIVO = listaVehiculos;

                List<SelectListItem> listaMotivos = new List<SelectListItem>();
                listaMotivos.Insert(0, (new SelectListItem { Text = "ERROR DE DIGITACIÓN", Value = "ED" }));
                listaMotivos.Insert(1, (new SelectListItem { Text = "ERROR DE PATRULLA", Value = "EP" }));
                listaMotivos.Insert(2, (new SelectListItem { Text = "HOMONIMO", Value = "HM" }));
                listaMotivos.Insert(3, (new SelectListItem { Text = "ANTECEDENTE SOLICITADO", Value = "AS" }));
                listaMotivos.Insert(4, (new SelectListItem { Text = "REALIZA PROCEDIMIENTO", Value = "RP" }));
                ViewBag.MOTIVO = listaMotivos;

                ViewBag.FECHA_GRABACION = DateTime.Now;

                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.USUA_GRABA = cADINCIDENTES_ANTECEDENTE.USUA_GRABA;
                //ViewBag.USUA_GRABA = "USR_CAD";

                return View(cADINCIDENTES_ANTECEDENTE);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
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