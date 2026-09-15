using Oracle.ManagedDataAccess.Client;
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
    public class RevistasController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: Revistas
        [SecadPlusAutorizador("/Revistas/Index")]
        public ActionResult Index(string strMensaje, string strTipo)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;
                ViewBag.globalCanal = usr.CADCANA_CODIGO;
                ViewBag.globalUsuario = usr.USUARIO;

                GestorDespachos gestorDespachos = new GestorDespachos();
                var turnoS = gestorDespachos.TraerTurno(usr.CADCANA_FUERZA_ID.ToString(), "02", "f_nume_turno");
                var turnoA = gestorDespachos.TraerTurno(usr.CADCANA_FUERZA_ID.ToString(), "03", "f_nume_turno_accesorio");

                if (turnoS == "0" || turnoA == "0")
                {
                    GestorBase.MSN_ALERTA_COMUN = "No hay turno definido para esta hora revise..";
                    return RedirectToAction("Index", "Home");
                }

                if (turnoS == "-1" || turnoA == "-1")
                {
                    GestorBase.MSN_ALERTA_COMUN = "Revise turnos definidos para esta hora..";
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

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerRevistas(int idSitio, int idFuerza)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " ";
                string strLimite = "", strOrderna = "";

                //string strBotonDestino = "";

                string strBoton = "<ul class='icons-list'> " +
                                        "<li class='dropdown'> " +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                                "<i class='icon-menu9'></i> " +
                                            "</button> " +
                                            "<ul class='dropdown-menu dropdown-menu-left'> " +
                                                "<li> " +
                                                    "<a class='btn btn-xs btn-default' href='_Reporte' target='_blank' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar' " +
                                                             // "onclick=\"window.location.href =
                                                             // '/BienesRelacionadosEvento/Edit?intSitioGraba=SITIO_GRABA&intConsecutivo=_CONSECUTIVO'\">
                                                             // " +
                                                             "> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Imprimir Revista " +
                                                    "</a> " +
                                                "</li> " +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_RevistasModel> listaRevistas = new List<_RevistasModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (DSP_SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (FECHA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (DSP_CADSITI_CODIGO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (DSP_CADPATR_CODIGO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (OBSERVACIONES, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY DSP_SITIO_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY FECHA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY DSP_CADSITI_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY DSP_CADPATR_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY OBSERVACIONES " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_revistas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    //strBotonDestino = strBoton.Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());

                    strBotonDestino = strBoton.Replace("_Reporte", (Url.Action("ReporteRevistas", "Reportes", new { intSitioGraba = fila["SITIO_GRABA"].ToString(), intEntidad = fila["CADFUERZ_ID"].ToString(), intConsecutivo = fila["CONSECUTIVO"].ToString() })).ToString());

                    listaRevistas.Add(new _RevistasModel("", strBotonDestino, fila["CONSECUTIVO"].ToString(), fila["DSP_SITIO_GRABA"].ToString(), fila["FECHA"].ToString(),
                                                         fila["DSP_CADSITI_CODIGO"].ToString(), fila["DSP_CADPATR_CODIGO"].ToString(), fila["OBSERVACIONES"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRevistas });
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
        public JsonResult AsignarHoraServidor()
        {
            try
            {
                GestorGeneral gestorGeneral = new GestorGeneral();
                var fecha = gestorGeneral.ObtenerTiempoServidor?? DateTime.Now;

                var hora = fecha.ToString("dd'/'MM'/'yyyy HH:mm:ss");

                var resultados = Json(new { result = true, datos = hora });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerSitios(int idSitio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = " WHERE SITIO_GRABA = " + idSitio + " ";
                string strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorSitio('CONSECUTIVO','DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_AtencionCasosModel> listaCasos = new List<_AtencionCasosModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    //blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    //if (blControl)
                    //{
                    strQuery += " AND (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_sitios", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaCasos.Add(new _AtencionCasosModel("", strBotonDestino, fila["CONSECUTIVO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCasos });
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
        public JsonResult VerPatrullas(int idFuerza, int idCanal, int idSitio, int turnoS, int turnoA)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = " WHERE CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO =  " + idCanal + " AND SITIO_GRABA = " + idSitio + " AND CADUNITUR_TURNO IN (" + turnoS + "," + turnoA + ")";
                string strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorPatrulla(CADUNID_FUERZA,CADUNID_CODIGO,CODIGO,'DESC_PATRULLA')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_PatrullasModel> listaPatrullas = new List<_PatrullasModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    //blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    //if (blControl)
                    //{
                    strQuery += " AND (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO =  " + idCanal + " AND SITIO_GRABA = " + idSitio + " AND CADUNITUR_TURNO IN (" + turnoS + "," + turnoA + ") AND REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_patrullas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CADUNID_FUERZA", fila["CADUNID_FUERZA"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CADUNID_CODIGO", fila["CADUNID_CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESC_PATRULLA", fila["DESC_PATRULLA"].ToString());

                    listaPatrullas.Add(new _PatrullasModel("", strBotonDestino, fila["CADUNID_FUERZA"].ToString(), fila["CADUNID_CODIGO"].ToString(), fila["CODIGO"].ToString(),
                                                           fila["DESC_PATRULLA"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPatrullas });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: Revistas/Create
        [SecadPlusAutorizador("/Revistas/Create")]
        public ActionResult Create()
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
                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;
                ViewBag.globalCanal = usr.CADCANA_CODIGO;
                ViewBag.globalUsuario = usr.USUARIO;

                ViewBag.USUA_USUARIO = usr.USUARIO;
                ViewBag.CADFUERZ_ID = usr.CADCANA_FUERZA_ID;
                ViewBag.CADSITI_SITIOGRABA = usr.SITIO_GRABACION;

                GestorDespachos gestorDespachos = new GestorDespachos();
                var turnoS = gestorDespachos.TraerTurno(usr.CADCANA_FUERZA_ID.ToString(), "02", "f_nume_turno");
                var turnoA = gestorDespachos.TraerTurno(usr.CADCANA_FUERZA_ID.ToString(), "03", "f_nume_turno_accesorio");

                if (turnoS == "0" || turnoA == "0")
                {
                    GestorBase.MSN_ALERTA_COMUN = "No hay turno definido para esta hora revise..";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.globalTurnoS = turnoS;
                ViewBag.globalTurnoA = turnoA;

                var existe = dbDes.CADREVISTAS.OrderByDescending(c => c.CONSECUTIVO).Select(c => c.CONSECUTIVO).FirstOrDefault();

                ViewBag.CONSECUTIVO = existe + 1;

                //var CONSECUTIVO = dbDes.CADREVISTAS.Max(c => c.CONSECUTIVO);
                //CONSECUTIVO += 1;

                //ViewBag.CONSECUTIVO = CONSECUTIVO;

                ViewBag.SITIO_GRABA = usr.SITIO_GRABACION;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Revistas/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADPATR_FUERZA,CADPATR_UNDE,CADPATR_CODIGO,FECHA,CADSITI_CODIGO,OBSERVACIONES,USUA_USUARIO,CADFUERZ_ID,CADSITI_SITIOGRABA,FECHA_GRABACION")] CADREVISTA cADREVISTA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existe = dbDes.CADREVISTAS.OrderByDescending(c => c.CONSECUTIVO).Select(c => c.CONSECUTIVO).FirstOrDefault();

                    cADREVISTA.CONSECUTIVO = existe + 1;
                    cADREVISTA.FECHA_GRABACION = DateTime.Now;

                    dbDes.CADREVISTAS.Add(cADREVISTA);
                    dbDes.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADREVISTA>(cADREVISTA, "CADREVISTAS", cADREVISTA.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADREVISTA.CONSECUTIVO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;
                ViewBag.globalCanal = usr.CADCANA_CODIGO;
                ViewBag.globalUsuario = usr.USUARIO;

                ViewBag.USUA_USUARIO = usr.USUARIO;
                ViewBag.CADFUERZ_ID = usr.CADCANA_FUERZA_ID;
                ViewBag.CADSITI_SITIOGRABA = usr.SITIO_GRABACION;

                GestorDespachos gestorDespachos = new GestorDespachos();
                var turnoS = gestorDespachos.TraerTurno(usr.CADCANA_FUERZA_ID.ToString(), "02", "f_nume_turno");
                var turnoA = gestorDespachos.TraerTurno(usr.CADCANA_FUERZA_ID.ToString(), "03", "f_nume_turno_accesorio");

                if (turnoS == "0" || turnoA == "0")
                {
                    GestorBase.MSN_ALERTA_COMUN = "No hay turno definido para esta hora revise..";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.globalTurnoS = turnoS;
                ViewBag.globalTurnoA = turnoA;

                //var CONSECUTIVO = dbDes.CADREVISTAS.Max(c => c.CONSECUTIVO);
                //CONSECUTIVO += 1;

                //ViewBag.CONSECUTIVO = CONSECUTIVO;

                ViewBag.SITIO_GRABA = usr.SITIO_GRABACION;
                ViewBag.DSP_CADSITI_CODIGO = Request["DSP_CADSITI_CODIGO"].ToString();
                ViewBag.DSP_CADPATR_CODIGO = Request["DSP_CADPATR_CODIGO"].ToString();
                return View(cADREVISTA);
            }
            catch (DbUpdateException ex)
            {
                var mensaje = "No se puede crear, restricción de integridad." + ex;
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
                dbDes.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}