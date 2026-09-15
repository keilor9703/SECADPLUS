using Oracle.ManagedDataAccess.Client;
using SecadDatos.Areas.MinutaSivicc;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SecadModelos.Areas.MinutaSivicc;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class TurnosController : Controller
    {
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strMensaje = "", strBotonFinal = "";
        private GestorMinuta gestor = new GestorMinuta();
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private CADUSUARIO usuarioActual;
        private CADUNITURNO uniturno;
        private CADMEDIOSDISPONIBLE medioDisponible;
        private CADPERDISPONIBLE personal;
        private CADARMDISPONIBLE armaDisp;
        private CADVEHIDISPONIBLE vehiDisp;
        private CADRADIODISPONIBLE radDisp;
        private Validador validador = new Validador();
        private GestorAuditoria auditoria = new GestorAuditoria();       

        // GET: Turnos
        /// <summary>
        /// Presenta la lista de sitios de grabación activos
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Turnos/Inicio")]
        public ActionResult Inicio(string strConsulta = null)
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
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.Consulta = strConsulta;
                ViewBag.SitioID = 0;
                if (!usuarioActual.USUARIO.Equals("USR_CAD"))
                {
                    ViewBag.SitioID = usuarioActual.SITIO_GRABACION;
                }

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite presentar la lista de entidades asociadas a un sitio de grabación
        /// </summary>
        /// <param name="intSitioGrabacion"></param>
        /// <param name="strNombre"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Entidades(int intSitioGrabacion, string strNombre, string strConsulta = null)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }                
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { intSitioGrabacion = intSitioGrabacion, strNombre = strNombre, strConsulta = strConsulta });
                ViewBag.NombreUbicacion = "Turnos-Entidades";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                var vFuerzas = (from t in db.V_FUERZAS where t.SITIO_GRABA.Equals(intSitioGrabacion) select t).ToList();
                if (!usuarioActual.CONDICION.Equals("A"))
                {
                    vFuerzas = (from t in db.V_FUERZAS where t.SITIO_GRABA.Equals(intSitioGrabacion) && t.ID == usuarioActual.CADCANA_FUERZA_ID select t).ToList();
                }
                ViewBag.NombreSitio = strNombre;
                ViewBag.Consulta = strConsulta;
                return View(vFuerzas);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite presentar la lista de entidades asociadas a un sitio de grabación
        /// </summary>
        /// <param name="intSitioGrabacion"></param>
        /// <param name="strNombre"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Turnos(int intSitioGrabacion, string strNombre, int intEntidad, string strEntidad, string strConsulta = null, string strMensaje = null, int intTurno = 0, int intSitioGrabacionTurno = 0, long? lgEvento = null, long? lgEventoMedio = null)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }

                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }
                ViewBag.HorasFuerza = (from registro in db.CADFUERZAS
                                       where registro.SITIO_GRABA == intSitioGrabacion
                                       && registro.ID == intEntidad
                                       select new { registro.HORAS_TURNO }).FirstOrDefault().HORAS_TURNO;
                ViewBag.TIPO_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TURNO_VIGILANCIA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.Sitio = strNombre;
                ViewBag.Consulta = strConsulta;
                ViewBag.SitioId = intSitioGrabacion;
                ViewBag.Entidad = strEntidad;
                ViewBag.EntidadId = intEntidad;
                ViewBag.TurnoEvento = intTurno;
                ViewBag.SitioGrabaTurno = intSitioGrabacionTurno;
                if (lgEvento == 0)
                {
                    ViewBag.TurnoEvento = 0;
                    ViewBag.SitioGrabaTurno = 0;
                }

                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;

                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite consultar la lista de elementos existentes en al base de datos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerTurnos(int dcSitio, int dcfuerza, string strNombre, string strEntidad, string strConsulta = null, int intTurno = 0)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadTurnos> elementos = new List<Ext_CadTurnos>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                decimal dcTurnoId = 0;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"_UNIDADES\" data-placement='bottom' data-original-title='Clic para gestionar Unidades'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Unidades en Turno </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"location.href='_REPORTEGEO'\" data-placement='bottom' data-original-title='Clic para generar el parte geonumerico'> <i class='icon icon-location4'> </i> Parte Geonumerico </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"location.href='_IMPMINUTA'\" data-placement='bottom' data-original-title='Clic para imprimir turno'> <i class='icon icon-printer'> </i> Imprimir Minuta </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_COPIAR\" data-placement='bottom' data-original-title='Clic para copiar el turno'> <i class='icon icon-copy'> </i> Copiar Turno </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"location.href='_EDITAR'\" data-placement='bottom' data-original-title='Clic para editar'> <i class='icon icon-pencil'> </i> Editar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"EliminarRegistroGeneral('_ELIMINAR')\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li></ul> </li></ul>";
                if (!string.IsNullOrEmpty(strConsulta))
                {
                    strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"_CONDUCTAS\" data-placement='bottom' data-original-title='Clic para consultar Conductas'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Conductas Asociadas</button> </li>" +
                    "</ul> </li></ul>";
                }
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //cambio daniel

                if (intTurno != 0)
                    parametros.ConsultaValor = intTurno.ToString();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.TURNO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.CLASE, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.TIPO,'" + parametros.ConsultaValor + "', 'i')  "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.TURNO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CLASE" + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.FECHAINICIA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.FECHATERMINA " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.FECHAINICIA DESC ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", dcSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", dcfuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_turnos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    dcTurnoId = Convert.ToDecimal(fila["TURNO"].ToString());
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_EDITAR", (Url.Action("Editar", new { lgTurnoId = dcTurnoId, intSitio = dcSitio, intEntidad = dcfuerza, strSitio = strNombre, strEntidad = strEntidad })).ToString());
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", (Url.Action("Eliminar", new { intSitio = dcSitio, lgConsecutivo = dcTurnoId, strSitio = strNombre, intEntidad = dcfuerza, strEntidad = strEntidad })).ToString());
                    strBotonFinal = strBotonFinal.Replace("_UNIDADES", "AbrirUnidades('" + fila["CLASE_TURNO"].ToString() + "','" + fila["HORA_INICIA"].ToString() + "','" + fila["HORA_TERMINA"].ToString() + "', '" + dcTurnoId.ToString() + "','" + (Url.Action("UnidadesTurno", new { intSitio = dcSitio, lgTurno = dcTurnoId, strSitio = strNombre, strEntidad = strEntidad, FechaI = fila["HORA_INICIA"].ToString(), FechaF = fila["HORA_TERMINA"].ToString() })).ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_CONDUCTAS", "ConsultarConductas(" + dcTurnoId.ToString() + "," + dcSitio.ToString() + ")");
                    strBotonFinal = strBotonFinal.Replace("_COPIAR", "CopiarTurno(" + dcTurnoId.ToString() + "," + dcSitio.ToString() + ", '" + fila["HORA_INICIA"].ToString() + "', '" + fila["HORA_TERMINA"].ToString() + "','" + fila["TIPO_TURNO"].ToString() + "','" + fila["CLASE_TURNO"].ToString() + "', '" + fila["FECHAACTUAL"].ToString() + "')");//fecha inicia, final, tipo turno, clase turno
                    strBotonFinal = strBotonFinal.Replace("_REPORTEGEO", (Url.Action("ReporteParteGeonumerico", "Reportes", new { strFechaInicio = fila["HORA_INICIA"].ToString(), strFechaFinal = fila["HORA_TERMINA"].ToString() })).ToString());
                    strBotonFinal = strBotonFinal.Replace("_IMPMINUTA", (Url.Action("ReporteMinutaServicioAtencion", "Reportes", new { intSitio = dcSitio, intEntidad = dcfuerza, intTurno = dcTurnoId })).ToString());
                    elementos.Add(new Ext_CadTurnos(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite realizar la consulta de las unidades en servicio asociadas al turno seleccionado
        /// </summary>
        /// <param name="intSitio">Sitio de grabación asociado al turno de servicio</param>
        /// <param name="lgTurno">Código del turno de servicio</param>
        /// <param name="strNombre">Nombre del sitio de grabación</param>
        /// <param name="strEntidad">Nombre de la entidad</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerUnidadesServicio(int intSitio, long lgTurno, string strNombre, string strEntidad)
        {
            try
            {
                CADTURNO turno;
                turno = gestor.ConsultarTurno(intSitio, lgTurno);
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtUniTurno> elementos = new List<ExtUniTurno>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                decimal dcTurnoId = 0;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +

                    "<li> <button class='btn btn-default ' type='button' " +
                    " data-popup='tooltip' onclick=\"_EDITAR\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Editar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_MEDIOS\" data -placement='bottom' data-original-title='Clic para consultar los Medios asociados'> <i class='icon-list'></i> Ver </button> </li>" +
                    "</ul> </li></ul>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.MEDIO, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.MEDIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.CONSIGNAS " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.MEDIO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                //if (blFiltro) strLimites = " AND ";
                strLimites += " DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "UNIDADES_SERVICIO", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_codigo", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_minuta", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    dcTurnoId = Convert.ToDecimal(fila["TURNO"].ToString());
                    strBotonFinal = strBoton.Replace("_EDITAR", "ConsultarConsigna(" + Convert.ToDecimal(fila["FUERZA"].ToString()) + "," + Convert.ToDecimal(fila["CODIGO"].ToString()) + ",'" + fila["MEDIO"].ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", ("EliminarUnidad(" + fila["FUERZA"].ToString() + "," + fila["CODIGO"].ToString() + ")"));
                    strBotonFinal = strBotonFinal.Replace("_MEDIOS", "ConsultarMedios(" + fila["FUERZA"].ToString() + ", " + fila["CODIGO"].ToString() + "," + turno.CADFUERZ_ID + ",'" + fila["MEDIO"].ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_UNIDADES", (Url.Action("Medios", new { intSitio = intSitio, lgTurno = dcTurnoId, strSitio = strNombre, strEntidad = strEntidad , unidadrmr = Convert.ToDecimal(fila["CODIGO"].ToString()) })).ToString());
                    elementos.Add(new ExtUniTurno(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
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
        [HttpPost]
        public JsonResult ObtenerMediosDisponibles(int intSitio, long lgTurno, int intUndeCodigo, int intFuerza)
        {
            try
            {
                //Consulta el turno seleccionado
                Session["cadunip_codigo"] = intUndeCodigo;
                ViewBag.cadunip_codigo = intUndeCodigo;
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadMedioDisponible> elementos = new List<Ext_CadMedioDisponible>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strEditar = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default ' type='button' " +
                    " data-popup='tooltip' onclick=\"_EDITAR\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Editar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_PERSONAS\" data-placement='bottom' data-original-title='Clic para consultar las personas asociadas'> <i class='icon-list'></i> Personas </button> </li>" +
                    "</ul> </li></ul>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CONSECUTIVO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.PATRULLA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.TIPO, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.CANAL, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.CANAL " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.ESTADO " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
               // if (blFiltro) strLimites = " AND ";
                strLimites += " DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "MEDIOSDISPONIBLES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", intFuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_codigo", intUndeCodigo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                //RMR 08-12-2020 DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_minuta", strConexion, "Nonquery");
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.p_info_minuta", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonFinal = "";
                    // strBoton = strBoton.Replace("_EDITAR", "ConsultarConsigna(" +
                    // Convert.ToDecimal(intFuerza.ToString()) + "," +
                    // Convert.ToDecimal(fila["CODIGO"].ToString()) + ",'" + fila["MEDIO"].ToString()
                    // + "')");
                    strBotonFinal = strBoton.Replace("_ELIMINAR", ("EliminarMedioDisponible(" + fila["CONSECUTIVO"].ToString() + ")").ToString());
                    strBotonFinal = strBotonFinal.Replace("_PERSONAS", "ConsultarPerMedios(" + intSitio.ToString() + "," + fila["CONSECUTIVO"].ToString() + ",'" + fila["PATRULLA"].ToString() + "')");
                    strEditar = "EditarMedioDisp(" + fila["CONSECUTIVO"].ToString() + ", " + fila["PATRULLAID"].ToString() + ",'" + fila["PATRULLA"].ToString() + "', " + fila["CANALFUERZA"].ToString() + ",";
                    strEditar += " " + fila["CANALID"].ToString() + ", '" + fila["CANAL"].ToString() + "', '" + fila["TIPOID"].ToString() + "', '" + fila["ESTADOID"].ToString() + "')";
                    strBotonFinal = strBotonFinal.Replace("_EDITAR", strEditar);
                    elementos.Add(new Ext_CadMedioDisponible(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
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
        [HttpPost]
        public JsonResult ObtenerUnidadesDisponibles(int intSitio, long lgTurno, string strNombre, string strEntidad)
        {
            try
            {
                CADTURNO turno;
                turno = gestor.ConsultarTurno(intSitio, lgTurno);
                var fuerza = (from registro in db.CADFUERZAS
                              where registro.ID == turno.CADFUERZ_ID
                              select new { registro.CADUNDE_CODIGO, registro.CADUNDE_FUERZA }).FirstOrDefault();
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtUniTurno> elementos = new List<ExtUniTurno>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                int intCodigoId = 0;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarUnidad(_CODIGO,'_DESCRIPCION', _FUERZA)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (JERARQUIA.CODIGO = " + parametros.ConsultaValor + " OR JERARQUIA.FUERZA = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(JERARQUIA.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY JERARQUIA.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY JERARQUIA.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY JERARQUIA.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY JERARQUIA.FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "UNIDADES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", fuerza.CADUNDE_FUERZA.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", turno.CLASE_TURNO, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_codigo", fuerza.CADUNDE_CODIGO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_minuta", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    intCodigoId = Convert.ToInt32(fila["CODIGO"].ToString());
                    strBotonFinal = strBoton.Replace("_CODIGO", intCodigoId.ToString());
                    elementos.Add(new ExtUniTurno(fila, strBotonFinal, true));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: Turnos/Create
        [Authorize]
        public ActionResult Crear(int intSitio, int intEntidad, string strSitio, string strEntidad)
        {
            try
            {
                ViewBag.SITIO = intSitio;
                ViewBag.SITIONOMBRE = strSitio;
                ViewBag.ENTIDAD = intEntidad;
                ViewBag.ENTIDADNOMBRE = strEntidad;
                ViewBag.CLASE_TURNO = null;
                ViewBag.TIPO_TURNO = null;
                //Genera la lista de clase de turno
                ViewBag.HorasFuerza = (from registro in db.CADFUERZAS
                                       where registro.SITIO_GRABA == intSitio
                                       && registro.ID == intEntidad
                                       select new { registro.HORAS_TURNO }).FirstOrDefault().HORAS_TURNO;
                ViewBag.CLASE_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "CLASE_TURNO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                //Genera la lista de tipo de turno
                ViewBag.TIPO_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TURNO_VIGILANCIA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { intSitio = intSitio, intEntidad = intEntidad, strSitio = strSitio, strEntidad = strEntidad });
                ViewBag.NombreUbicacion = "Turnos-Crear";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////



                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Turnos/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear([Bind(Include = "SITIO_GRABA,TURNO,HORA_INICIA,HORA_TERMINA,TIPO_TURNO,CONSIGNAS,CADFUERZ_ID,FECHA_GRABACION,FECHA_MODIFICACION,USUA_MODIFICA,CLASE_TURNO,DIA_INICIA,DIA_TERMINA")] CADTURNO cADTURNO)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (!validador.ValidarUsuarioMinuta(usuarioActual))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_MINUTA_CREAR_TURNO;
                    return RedirectToAction("Turnos", "Turnos", new { intSitioGrabacion = cADTURNO.SITIO_GRABA, strNombre = Request.Form["SitioNombre"], intEntidad = cADTURNO.CADFUERZ_ID, strEntidad = Request.Form["EntidadNombre"] });
                }
                if (ModelState.IsValid)
                {
                    cADTURNO.TURNO = gestor.ObtenerTurnoConsecutivo();
                    cADTURNO.FECHA_GRABACION = gestor.ObtenerTiempoServidor;
                    contexto.CADTURNOS.Add(cADTURNO);
                    await contexto.SaveChangesAsync();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADTURNO.TURNO.ToString());
                    return RedirectToAction("Turnos", new { intSitioGrabacion = cADTURNO.SITIO_GRABA, strNombre = Request.Form["SitioNombre"], intEntidad = cADTURNO.CADFUERZ_ID, strEntidad = Request.Form["EntidadNombre"], strMensaje = strMensaje });
                }
                //Carga datos comunes
                ViewBag.SITIO = cADTURNO.SITIO_GRABA;
                ViewBag.SITIONOMBRE = Request.Form["SitioNombre"];
                ViewBag.ENTIDAD = cADTURNO.CADFUERZ_ID;
                ViewBag.ENTIDADNOMBRE = Request.Form["EntidadNombre"];
                ViewBag.CLASE_TURNO = null;
                ViewBag.TIPO_TURNO = null;
                //Genera la lista de clase de turno
                ViewBag.HorasFuerza = (from registro in db.CADFUERZAS
                                       where registro.SITIO_GRABA == cADTURNO.SITIO_GRABA
                                       && registro.ID == cADTURNO.CADFUERZ_ID
                                       select new { registro.HORAS_TURNO }).FirstOrDefault().HORAS_TURNO;
                ViewBag.CLASE_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "CLASE_TURNO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADTURNO.CLASE_TURNO);
                //Genera la lista de tipo de turno
                ViewBag.TIPO_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TURNO_VIGILANCIA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADTURNO.TIPO_TURNO);
                return View(cADTURNO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Turnos/Editar")]
        public async Task<ActionResult> Editar(long lgTurnoId, int intSitio, int intEntidad, string strSitio, string strEntidad)
        {
            try
            {
                if (lgTurnoId <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CADTURNO cADTURNO = await (from registro in contexto.CADTURNOS
                                           where registro.SITIO_GRABA == intSitio && registro.TURNO == lgTurnoId
                                           select registro).FirstOrDefaultAsync();
                if (cADTURNO == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE.ToString();
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.SITIO = intSitio;
                ViewBag.SITIONOMBRE = strSitio;
                ViewBag.ENTIDAD = intEntidad;
                ViewBag.ENTIDADNOMBRE = strEntidad;
                ViewBag.CLASE_TURNO = null;
                ViewBag.TIPO_TURNO = null;
                //Genera la lista de clase de turno
                ViewBag.HorasFuerza = (from registro in db.CADFUERZAS
                                       where registro.SITIO_GRABA == intSitio
                                       && registro.ID == intEntidad
                                       select new { registro.HORAS_TURNO }).FirstOrDefault().HORAS_TURNO;
                ViewBag.CLASE_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "CLASE_TURNO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADTURNO.CLASE_TURNO);
                //Genera la lista de tipo de turno
                ViewBag.TIPO_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TURNO_VIGILANCIA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADTURNO.TIPO_TURNO);
                return View(cADTURNO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Turnos/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar([Bind(Include = "SITIO_GRABA,TURNO,HORA_INICIA,HORA_TERMINA,TIPO_TURNO,CONSIGNAS,CADFUERZ_ID,FECHA_GRABACION,FECHA_MODIFICACION,USUA_MODIFICA,CLASE_TURNO,DIA_INICIA,DIA_TERMINA")] CADTURNO cADTURNO)
        {
            try
            {

                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (!validador.ValidarUsuarioMinuta(usuarioActual))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_MINUTA_CREAR_TURNO;
                    return RedirectToAction("Turnos", "Turnos", new { intSitioGrabacion = cADTURNO.SITIO_GRABA, strNombre = Request.Form["SitioNombre"], intEntidad = cADTURNO.CADFUERZ_ID, strEntidad = Request.Form["EntidadNombre"] });
                }
                /*SE DESHABILITA POR PETICION DEL LOS USUARIOS FUNCIONALES*/
                /*
                dcCantidadAsociada = validador.VerificarTurnoAsociaciones(cADTURNO.TURNO);
                if (dcCantidadAsociada > 0)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_TURNO_CON_RESTRICCION.Replace("TURNO", cADTURNO.TURNO.ToString()).Replace("CANTIDAD", dcCantidadAsociada.ToString());
                    return RedirectToAction("Turnos", "Turnos", new { intSitioGrabacion = cADTURNO.SITIO_GRABA, strNombre = Request.Form["SitioNombre"], intEntidad = cADTURNO.CADFUERZ_ID, strEntidad = Request.Form["EntidadNombre"] });
                }*/
                if (ModelState.IsValid)
                {
                    contexto.Entry(cADTURNO).State = EntityState.Modified;
                    await contexto.SaveChangesAsync();



                    auditoria.RegistrarAuditoria<CADTURNO>(cADTURNO, "CADTURNO", "SITIO_GRABA=" + cADTURNO.SITIO_GRABA.ToString() + ",TURNO=" + cADTURNO.TURNO
                      , TipoAuditoria.Modificar);

                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADTURNO.TURNO.ToString());
                    return RedirectToAction("Turnos", new { intSitioGrabacion = cADTURNO.SITIO_GRABA, strNombre = Request.Form["SitioNombre"], intEntidad = cADTURNO.CADFUERZ_ID, strEntidad = Request.Form["EntidadNombre"], strMensaje = strMensaje });
                }
                //Carga datos comunes
                ViewBag.SITIO = cADTURNO.SITIO_GRABA;
                ViewBag.SITIONOMBRE = Request.Form["SitioNombre"];
                ViewBag.ENTIDAD = cADTURNO.CADFUERZ_ID;
                ViewBag.ENTIDADNOMBRE = Request.Form["EntidadNombre"];
                ViewBag.CLASE_TURNO = null;
                ViewBag.TIPO_TURNO = null;
                //Genera la lista de clase de turno
                ViewBag.HorasFuerza = (from registro in db.CADFUERZAS
                                       where registro.SITIO_GRABA == cADTURNO.SITIO_GRABA
                                       && registro.ID == cADTURNO.CADFUERZ_ID
                                       select new { registro.HORAS_TURNO }).FirstOrDefault().HORAS_TURNO;
                ViewBag.CLASE_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "CLASE_TURNO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADTURNO.CLASE_TURNO);
                //Genera la lista de tipo de turno
                ViewBag.TIPO_TURNO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TURNO_VIGILANCIA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADTURNO.TIPO_TURNO);
                return View(cADTURNO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Turnos/Delete/5
        [Authorize]
        public ActionResult Eliminar(int intSitio, long lgConsecutivo, string strSitio, int intEntidad, string strEntidad)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (!validador.ValidarUsuarioMinuta(usuarioActual))
                {
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_MINUTA_CREAR_TURNO;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }
                ViewBag.SITIONOMBRE = strSitio;
                ViewBag.ENTIDAD = intEntidad;
                ViewBag.ENTIDADNOMBRE = strEntidad;
                CADTURNO cADTURNO = (from registro in contexto.CADTURNOS
                                     where registro.SITIO_GRABA == intSitio
                                     && registro.TURNO == lgConsecutivo
                                     select registro).FirstOrDefault();
                if (cADTURNO == null)
                {
                    return HttpNotFound();
                }
                return View("Eliminar", null, cADTURNO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                return View("Eliminar", null, null);
            }
        }

        [Authorize]
        public JsonResult EliminarUnidad(int intSitio, long lgConsecutivo, int intEntidad, int intUnidadCodigo)
        {
            try
            {
                uniturno = (from registro in contexto.CADUNITURNOS
                            where registro.CADTURN_SITIOGRABA == intSitio
                            && registro.CADTURN_TURNO == lgConsecutivo
                            && registro.CADUNID_FUERZA == intEntidad
                            && registro.CADUNIP_CODIGO == intUnidadCodigo
                            select registro).FirstOrDefault();
                contexto.CADUNITURNOS.Remove(uniturno);
                contexto.SaveChanges();


                CADUNITURNO uniturno_copy = new CADUNITURNO();
                uniturno_copy = (CADUNITURNO)auditoria.GenerarCopiaAuditoria(uniturno);

                auditoria.RegistrarAuditoria<CADUNITURNO>(uniturno_copy, "CADUNITURNO", "CADTURN_SITIOGRABA=" + uniturno.CADTURN_SITIOGRABA.ToString() + ",CADTURN_TURNO=" + uniturno.CADTURN_TURNO
                     + ",CADUNID_FUERZA=" + uniturno.CADUNID_FUERZA + ",CADUNIP_CODIGO=" + uniturno.CADUNIP_CODIGO, TipoAuditoria.Eliminar);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Unidad de Turno Seleccionado") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult EliminarMedioDisponible(int intSitio, long lgConsecutivo)
        {
            try
            {
                /*medioDisponible = (from registro in contexto.CADMEDIOSDISPONIBLES
                                   where registro.SITIO_GRABA == intSitio
                                   && registro.CONSECUTIVO == lgConsecutivo
                                   select registro).FirstOrDefault();
                contexto.CADMEDIOSDISPONIBLES.Remove(medioDisponible);
                contexto.SaveChanges();*/
                string psalida = "";
                string popcion = "";
                GestorGeneral objGestorGral;
                objGestorGral = new GestorGeneral();
                popcion = "03";
                //strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", medioDisponible.CONSECUTIVO.ToString());
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_OPCION", popcion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_SITIO_GRABA", Convert.ToString(intSitio), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_CONSECUTIVO", Convert.ToString(lgConsecutivo), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_ESTADO", "0", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PTIPO", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_SITIOGRABA", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_TURNO", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDEFUERZA", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDECODIGO", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADPATRU_CODIGO", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_FUERZ_ID", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_CODIGO", "0", typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_SALIDA", "", typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.INS_CADMEDIOSDISPONIBLES", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    psalida = Convert.ToString(parameter.Item2.ToString());
                }
                CADMEDIOSDISPONIBLE medioDisponible_copy = new CADMEDIOSDISPONIBLE();
                medioDisponible_copy = (CADMEDIOSDISPONIBLE)auditoria.GenerarCopiaAuditoria(medioDisponible);

                auditoria.RegistrarAuditoria<CADMEDIOSDISPONIBLE>(medioDisponible_copy, "CADMEDIOSDISPONIBLE", "SITIO_GRABA=" + medioDisponible.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + medioDisponible.CONSECUTIVO
                    , TipoAuditoria.Eliminar);



                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", lgConsecutivo.ToString()) });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult EliminarPersona(int intSitio, long lgConsecutivo, string strCedula)
        {
            try
            {
                if (validador.ValidarEquipoPersona(strCedula, lgConsecutivo))
                {
                    return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_PERSONA_EQUIPO_ASOCIADO });
                }
                personal = (from registro in contexto.CADPERDISPONIBLES
                            where registro.CADMEDISP_SITIOGRABA == intSitio
                            && registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                            && registro.CADPOLI_CEDU_EMPLEADO == strCedula
                            select registro).FirstOrDefault();
                contexto.CADPERDISPONIBLES.Remove(personal);
                contexto.SaveChanges();



                CADPERDISPONIBLE personal_copy = new CADPERDISPONIBLE();
                personal_copy = (CADPERDISPONIBLE)auditoria.GenerarCopiaAuditoria(personal);

                auditoria.RegistrarAuditoria<CADPERDISPONIBLE>(personal_copy, "CADPERDISPONIBLE", "CADMEDISP_SITIOGRABA=" + personal.CADMEDISP_SITIOGRABA.ToString() + ",CADMEDISP_CONSECUTIVO=" + personal.CADMEDISP_CONSECUTIVO
                     + ",CADPOLI_CEDU_EMPLEADO=" + personal.CADPOLI_CEDU_EMPLEADO, TipoAuditoria.Eliminar);


                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strCedula) });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult EliminarEquipo(int intSitio, long lgConsecutivo, string strCedula, string strSerie, string strOpcion)
        {
            try
            {
                //Evalua el equipo a elminar
                switch (strOpcion)
                {
                    case "ARMAS":
                        armaDisp = (
                            from registro in contexto.CADARMDISPONIBLES
                            where registro.CADARM_NUME_SERIE == strSerie
                            && registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                            && registro.CADMEDISP_SITIOGRABA == intSitio
                            && registro.CADPOLI_CEDU_EMPLEADO == strCedula
                            select registro
                            ).FirstOrDefault();
                        contexto.CADARMDISPONIBLES.Remove(armaDisp);
                        break;

                    case "RADIOS":
                        radDisp = (from registro in contexto.CADRADIODISPONIBLES
                                   where registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                                   && registro.CADMEDISP_SITIOGRABA == intSitio
                                   && registro.CADPOLI_CEDU_EMPLEADO == strCedula
                                   && registro.CADRAD_NUME_SERIE == strSerie
                                   select registro
                                   ).FirstOrDefault();
                        contexto.CADRADIODISPONIBLES.Remove(radDisp);
                        break;

                    case "VEHICULOS":
                        vehiDisp = (from registro in contexto.CADVEHIDISPONIBLES
                                    where registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                                    && registro.CADMEDISP_SITIOGRABA == intSitio
                                    && registro.CADVEHI_NUMESIGLA == strSerie
                                    && registro.POLI_CEDU_EMPLEADO == strCedula
                                    select registro
                            ).FirstOrDefault();
                        contexto.CADVEHIDISPONIBLES.Remove(vehiDisp);
                        break;
                }
                contexto.SaveChanges();
                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Unidad de Turno Seleccionado") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // POST: Turnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int intSitio, long lgConsecutivo, string strSitio, int intEntidad, string strEntidad)
        {
            try
            {
                CADTURNO cADTURNO = (from registro in contexto.CADTURNOS
                                     where registro.SITIO_GRABA == intSitio
                                     && registro.TURNO == lgConsecutivo
                                     select registro).FirstOrDefault();
                contexto.CADTURNOS.Remove(cADTURNO);
                await contexto.SaveChangesAsync();


                CADTURNO cADTURNO_copy = new CADTURNO();
                cADTURNO_copy = (CADTURNO)auditoria.GenerarCopiaAuditoria(cADTURNO);

                auditoria.RegistrarAuditoria<CADTURNO>(cADTURNO_copy, "CADTURNO", "SITIO_GRABA=" + cADTURNO.SITIO_GRABA.ToString() + ",TURNO=" + cADTURNO.TURNO
                   , TipoAuditoria.Eliminar);


                return RedirectToAction("Turnos", "Turnos", new { intSitioGrabacion = intSitio, strNombre = strSitio, intEntidad = intEntidad, strEntidad = strEntidad, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", lgConsecutivo.ToString());
                // ViewData["MENSAJE"] =Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strNombre);
                return RedirectToAction("Turnos", "Turnos", new { intSitioGrabacion = intSitio, strNombre = strSitio, intEntidad = intEntidad, strEntidad = strEntidad });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult UnidadesTurno(int intSitio, long lgTurno, string strSitio, string strEntidad, string FechaI, string FechaF)
        {


            ViewBag.TurnoID = lgTurno;
            ViewBag.SitioID = intSitio;
            ViewBag.FechaI = FechaI;
            ViewBag.FechaF = FechaF;
            /*adicion codigo rmr 03-03-2020*/
            var ListarEstacion = new DbMinuta().F_GetUnidades().ToList();
            ListarEstacion.Add(new MdEstaciones { Id = 0, Descripcion = "[Seleccione Estación]" });
            ListarEstacion = ListarEstacion.OrderBy(x => x.Descripcion).ToList();
            ViewBag.ddlUnidades = new SelectList(ListarEstacion, "Id", "Descripcion");
            /*adicion codigo rmr 03-03-2020*/
            /*adicion codigo rmr 13-05-2020*/
            var ListarEstacion2 = new DbMinuta().F_GetUnidades2(intSitio, Convert.ToInt32(lgTurno)).ToList();
            ListarEstacion2.Add(new MdEstaciones { Id = 0, Descripcion = "[Seleccione Estación]" });
            ListarEstacion2 = ListarEstacion2.OrderBy(x => x.Descripcion).ToList();
            ViewBag.ddlUnidades2 = new SelectList(ListarEstacion2, "Id", "Descripcion");
            /*adicion codigo rmr 13-05-2020*/

            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                CADTURNO turno = gestor.ConsultarTurno(intSitio, lgTurno);

                if (validador.ValidarTurno(turno))
                {
                    ViewBag.Sitio = turno.SITIO_GRABA;
                    ViewBag.Turno = turno.TURNO;
                    ViewBag.SitioNombre = strSitio;
                    ViewBag.Entidad = strEntidad;
                    ViewBag.EntidadId = turno.CADFUERZ_ID;
                    ViewBag.TipoMedio = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_PATRULLA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.CODIGO), "CODIGO", "DESCRIPCION");
                    ViewBag.EstadoMedio = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "ESTADO_PATRULLA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.CODIGO), "CODIGO", "DESCRIPCION");
                    ViewBag.Cargos = new SelectList(contexto.CADCARGOS.Where(c => c.CARGO_MINUTA == "P").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.CODIGO), "CODIGO", "DESCRIPCION");


                    ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                    string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                    string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                    ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { intSitio = intSitio, lgTurno = lgTurno, strSitio = strSitio, strEntidad = strEntidad });
                    ViewBag.NombreUbicacion = "Turnos-Unidades";
                    ///////////////////////////Variables BreadCrumb////////////////////////////////////////

                    return View("UnidadesTurno", "BaseRecepcion");
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = validador.ObtenerMensaje();
                    return RedirectToAction("Turnos", new { intSitioGrabacion = intSitio, strNombre = strSitio, intEntidad = turno.CADFUERZ_ID, strEntidad = strEntidad });
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerSiguienteFecha(string strTipoTurno, int intEntidad)
        {
            try
            {
                string strFecha_turno;
                GestorGeneral gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_tipo_turno", strTipoTurno, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_entidad", intEntidad.ToString(), typeof(decimal)));
                strFecha_turno = gestorGeneral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_obtener_hora_turno", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                if (!string.IsNullOrEmpty(strFecha_turno))
                {
                    return Json(new { result = true, datos = strFecha_turno.ToString() });
                }
                return Json(new { result = false, datos = "No se pudo obtener una fecha para el turno de servicio" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        //Metodo Json que trae funcionarios en las minutas
        public JsonResult F_GetMinuta(int UnidadId)
        {
            string a = "";
            var retorno = new DbMinuta().F_GetMinuta(UnidadId); /// .OrderByDescending(x => Convert.ToDateTime(x.Fecha));

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        //Metodo Json que trae funcionarios en las minutas 2 F_GetMinutaNew(int V_UnidadID,string vFechaInicial,string vfechaFinal)
        public JsonResult F_GetMinutaNew(int UnidadId)
        {

            string vFechaInicial = Convert.ToString(Session["vstrFechaInicia"]);
            string vfechaFinal = Convert.ToString(Session["vstrFechaFin"]);

            var retorno = new DbMinuta().F_GetMinutaNew(UnidadId, vFechaInicial, vfechaFinal); /// .OrderByDescending(x => Convert.ToDateTime(x.Fecha));

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }


        [Authorize]
        [HttpPost]
        public JsonResult GuardarUnidad(string[] strParametros)
        {
            try
            {
                bool blResultado = true;
                CADUNITURNO uniTurno;
                strMensaje = "";
                if (strParametros.Length <= 0)
                {
                    blResultado = false;
                    strMensaje = "No se han proporcionado los parámetros necesarios para completar la acción";
                }
                else
                {
                    //Verifica si el registro existe

                    uniTurno = gestor.ConsultarUniTurno(strParametros);
                    if (uniTurno != null)
                    {
                        uniTurno.CONSIGNAS = strParametros[4];
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Unidad en Servicio");
                        contexto.Entry(uniTurno).State = EntityState.Modified;
                    }
                    else
                    {
                        uniTurno = new CADUNITURNO();
                        uniTurno.CADTURN_SITIOGRABA = Convert.ToInt32(strParametros[0]);
                        uniTurno.CADTURN_TURNO = Convert.ToInt64(strParametros[1]);
                        uniTurno.CADUNID_FUERZA = Convert.ToInt32(strParametros[2]);
                        uniTurno.CADUNIP_CODIGO = Convert.ToInt32(strParametros[3]);
                        uniTurno.CONSIGNAS = strParametros[4];
                        contexto.CADUNITURNOS.Add(uniTurno);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Unidad en Servicio");
                    }
                    contexto.SaveChanges();
                }
                return Json(new { result = blResultado, datos = strMensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarConsigna(string[] strParametros)
        {
            try
            {
                int intSitio = 0, intFuerza = 0, intCodigo = 0;
                long lgTurno = 0;
                bool blResultado = true;
                if (strParametros.Length <= 0)
                {
                    blResultado = false;
                    strMensaje = "No se han proporcionado los parámetros necesarios para completar la acción";
                }
                else
                {
                    intSitio = Convert.ToInt32(strParametros[0]);
                    lgTurno = Convert.ToInt64(strParametros[1]);
                    intFuerza = Convert.ToInt32(strParametros[2]);
                    intCodigo = Convert.ToInt32(strParametros[3]);
                    CADUNITURNO uniTurno = (from registro in contexto.CADUNITURNOS
                                            where registro.CADTURN_SITIOGRABA == intSitio
                                            && registro.CADTURN_TURNO == lgTurno
                                            && registro.CADUNID_FUERZA == intFuerza
                                            && registro.CADUNIP_CODIGO == intCodigo
                                            select registro).FirstOrDefault();
                    if (uniTurno == null)
                    {
                        blResultado = false;
                        strMensaje = "El registro consultado no existe, por favor actualice la tabla";
                    }
                    else
                    {
                        strMensaje = uniTurno.CONSIGNAS;
                    }
                }
                return Json(new { result = blResultado, datos = strMensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite realizar la consulta de los canales disponibles para ser asociados a un medios de atención
        /// </summary>
        /// <param name="intFuerza">Codigo de la entidad</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerCanalesMedios(int intFuerza, string strCanal=null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadCanales> elementos = new List<Ext_CadCanales>();
                JsonResult resultados;
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarCanal(_CODIGO,'_DESCRIPCION', _FUERZA)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                string strBotonFinal;
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (string.IsNullOrEmpty(strCanal))
                {
                    if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                    {
                        blFiltro = true;
                        //Verifica si se debe buscar por código o descripción
                        blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                        if (blControl)
                        {
                            strQuery = " AND (codigo = " + parametros.ConsultaValor + " OR cadfuerz_id = " + parametros.ConsultaValor + ") ";
                        }
                        else
                        {
                            strQuery = " AND  (REGEXP_LIKE(descripcion, '" + parametros.ConsultaValor + "', 'i') )";
                        }
                    }
                    //Verifica si se deben realizar ordenamientos
                    switch (parametros.IndiceOrdenamiento)
                    {
                        default:
                            strOrdenamiento += " ORDER BY codigo " + parametros.TipoOrdenamiento + " ";
                            break;

                        case 2:
                            strOrdenamiento += " ORDER BY descripcion " + parametros.TipoOrdenamiento + " ";
                            break;

                        case 3:
                            strOrdenamiento += " ORDER BY codigo " + parametros.TipoOrdenamiento + " ";
                            break;

                        case 4:
                            strOrdenamiento += " ORDER BY cadfuerz_id " + parametros.TipoOrdenamiento + " ";
                            break;
                    }
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                }else
                {
                    strQuery = " AND UPPER(descripcion) =UPPER('"+strCanal+"') ";
                    strOrdenamiento += " ORDER BY codigo desc";
                }                
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "CANALES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", intFuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_codigo", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_minuta", strConexion, "Nonquery");
                if (!string.IsNullOrEmpty(strCanal))
                {
                    if (dttLugar.Rows.Count == 1)
                    {
                        resultados = Json(new { result = true, datos = new { CODIGO = dttLugar.Rows[0]["CODIGO"].ToString(), DESCRIPCION = dttLugar.Rows[0]["DESCRIPCION"].ToString(), FUERZA = dttLugar.Rows[0]["CADFUERZ_ID"].ToString() } });
                    }
                    else if (dttLugar.Rows.Count > 1)
                    {
                        resultados = Json(new { result = false, datos = "2" });
                    }
                    else
                    {
                        resultados = Json(new { result = false, datos = "" });
                    }
                    resultados.MaxJsonLength = int.MaxValue;
                    return resultados;
                }
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CODIGO", fila["CODIGO"].ToString());
                    strBotonFinal = strBotonFinal.Replace("_DESCRIPCION", fila["DESCRIPCION"].ToString());
                    strBotonFinal = strBotonFinal.Replace("_FUERZA", fila["CADFUERZ_ID"].ToString());
                    elementos.Add(new Ext_CadCanales(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
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
        [HttpPost]
        public JsonResult ObtenerPatrullasMedios(int intSitio, long lgTurno, int intUndeCodigo, int intFuerza, string strPatrulla=null)
        {
            try
            {
                CADTURNO turno = gestor.ConsultarTurno(intSitio, lgTurno);
                //Consulta el turno seleccionado
                JsonResult resultados;
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadPatrulla> elementos = new List<Ext_CadPatrulla>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false; 
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarUnidadMedio(_CODIGO,'_DESCRIPCION', _FUERZA, _UNIDAD)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();

                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (string.IsNullOrEmpty(strPatrulla))
                {
                    if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                    {
                        blFiltro = true;
                        //Verifica si se debe buscar por código o descripción
                        blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                        if (blControl)
                        {
                            strQuery = " AND (CODIGO = " + parametros.ConsultaValor + " OR CADUNID_FUERZA = " + parametros.ConsultaValor + " OR CADUNID_CODIGO = " + parametros.ConsultaValor + ") ";
                        }
                        else
                        {
                            strQuery = " AND  (REGEXP_LIKE(DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') )";
                        }
                    }
                    //Verifica si se deben realizar ordenamientos
                    switch (parametros.IndiceOrdenamiento)
                    {
                        default:
                            strOrdenamiento += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                            break;

                        case 2:
                            strOrdenamiento += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                            break;

                        case 3:
                            strOrdenamiento += " ORDER BY CADUNID_FUERZA " + parametros.TipoOrdenamiento + " ";
                            break;

                        case 4:
                            strOrdenamiento += " ORDER BY CADUNID_CODIGO " + parametros.TipoOrdenamiento + " ";
                            break;
                    }
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                }else {
                    strQuery = " AND  (UPPER(DESC_PATRULLA) LIKE '" + strPatrulla.ToUpper() + "%' )";
                    strOrdenamiento += " ORDER BY DESC_PATRULLA desc ";
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "PATRULLASDISPONIBLES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", intFuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", turno.CLASE_TURNO, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", turno.SITIO_GRABA.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", turno.TURNO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_codigo", intUndeCodigo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_minuta", strConexion, "Nonquery");
                if (!string.IsNullOrEmpty(strPatrulla) ) {
                    if (dttLugar.Rows.Count == 1)
                    {
                        resultados = Json(new { result = true, datos = new { CODIGO = dttLugar.Rows[0]["CODIGO"].ToString(), DESCRIPCION = dttLugar.Rows[0]["PATRULLA"].ToString(), FUERZA = dttLugar.Rows[0]["FUERZA"].ToString(), UNIDAD = dttLugar.Rows[0]["UNIDAD"].ToString() } });
                    }else if(dttLugar.Rows.Count > 1)
                    {
                        resultados = Json(new { result = false, datos = "2"});
                    }else {
                        resultados = Json(new { result = false, datos = "" });
                    }                    
                    resultados.MaxJsonLength = int.MaxValue;
                    return resultados;
                }
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CODIGO", fila["CODIGO"].ToString());
                    strBotonFinal = strBotonFinal.Replace("_DESCRIPCION", fila["PATRULLA"].ToString());
                    strBotonFinal = strBotonFinal.Replace("_FUERZA", fila["FUERZA"].ToString());
                    strBotonFinal = strBotonFinal.Replace("_UNIDAD", fila["UNIDAD"].ToString());
                    elementos.Add(new Ext_CadPatrulla(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
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
        [HttpPost]
        public JsonResult GuardarMedio(string[] strParametros)
        {
            try
            {
                //rmr 30-12-2020
                string psalida = "";
                string popcion = "";
                //int pconsecutivo = 0;
                GestorGeneral objGestorGral;
                objGestorGral = new GestorGeneral();
                GestorGeneral objGestorGral1;
                objGestorGral1 = new GestorGeneral();
                GestorGeneral objGestorGral2;
                objGestorGral2 = new GestorGeneral();
                //fin rmr 30-12-2020
                string strMensaje = "";
                JsonResult resultados;
                CADMEDIOSDISPONIBLE medioDisponible;
                int intSitio = Convert.ToInt32(strParametros[0]);
                long lgTurno = Convert.ToInt64(strParametros[1]);
                double dblConsecutivo = Convert.ToDouble(strParametros[2]);
                CADTURNO turno = gestor.ConsultarTurno(intSitio, lgTurno);
                if (!validador.ValidaHoraTurno(turno))
                {
                    resultados = Json(new { result = false, datos = "Usted no puede insertar medios de atención para un turno que ya paso.." });
                }
                else if (strParametros.Length > 0)
                {
                    
                    popcion = "02";                    
                    //strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", medioDisponible.CONSECUTIVO.ToString());
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_OPCION", popcion, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_SITIO_GRABA", strParametros[0], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_CONSECUTIVO", Convert.ToString(dblConsecutivo), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_ESTADO", strParametros[5], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PTIPO", strParametros[3], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_SITIOGRABA", strParametros[4], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_TURNO", strParametros[1], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDEFUERZA", strParametros[6], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDECODIGO", strParametros[7], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADPATRU_CODIGO", strParametros[8], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_FUERZ_ID", strParametros[9], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_CODIGO", strParametros[10], typeof(decimal)));
                    objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_SALIDA", "", typeof(string)));
                    objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.INS_CADMEDIOSDISPONIBLES", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                    foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                    {
                        psalida = Convert.ToString(parameter.Item2.ToString());
                    }
                    //busca de registro rmr 30-12-2020
                    /*
                    popcion = "02";
                    objGestorGral1.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral1.ParamsSalida = new List<Tuple<string, object, Type>>();
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_OPCION", Convert.ToString(popcion), typeof(string)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_SITIO_GRABA", Convert.ToString(intSitio), typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_CONSECUTIVO", Convert.ToString(dblConsecutivo), typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_ESTADO", strParametros[5], typeof(string)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PTIPO", strParametros[3], typeof(string)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_SITIOGRABA", strParametros[4], typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_TURNO", strParametros[1], typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDEFUERZA", strParametros[6], typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDECODIGO", strParametros[7], typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADPATRU_CODIGO", strParametros[8], typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_FUERZ_ID", strParametros[9], typeof(decimal)));
                    objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_CODIGO", strParametros[10], typeof(decimal)));
                    objGestorGral1.ParamsSalida.Add(new Tuple<string, object, Type>("P_SALIDA", "", typeof(string)));
                    objGestorGral1.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.INS_CADMEDIOSDISPONIBLES", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                    foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                    {
                        psalida = Convert.ToString(parameter.Item2.ToString());
                    }*/
                    //busca el registro
                    /*
                    medioDisponible = (from registro in contexto.CADMEDIOSDISPONIBLES
                                       where registro.SITIO_GRABA == intSitio
                                       && registro.CONSECUTIVO == dblConsecutivo
                                       select registro).FirstOrDefault();*/
                    //if (medioDisponible != null)
                    if (psalida != "123")//rmr 30-12-2020
                    {
                        //actualiza el registro
                        /*
                        medioDisponible.TIPO = strParametros[3];
                        medioDisponible.ESTADO = strParametros[5];
                        medioDisponible.CADUNITUR_UNDEFUERZA = Convert.ToInt32(strParametros[6]);
                        medioDisponible.CADUNITUR_UNDECODIGO = Convert.ToInt32(strParametros[7]);
                        medioDisponible.CADPATRU_CODIGO = Convert.ToInt16(strParametros[8]);
                        medioDisponible.CADCANA_FUERZ_ID = Convert.ToInt32(strParametros[9]);
                        medioDisponible.CADCANA_CODIGO = Convert.ToByte(strParametros[10]);
                        */
                        //actualiza registro rmr 30-12-2020
                        popcion = "01";
                        objGestorGral1.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                        objGestorGral1.ParamsSalida = new List<Tuple<string, object, Type>>();
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_OPCION", popcion, typeof(string)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_SITIO_GRABA", Convert.ToString(intSitio), typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_CONSECUTIVO", Convert.ToString(dblConsecutivo), typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_ESTADO", strParametros[5], typeof(string)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PTIPO", strParametros[3], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_SITIOGRABA", strParametros[4], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_TURNO", strParametros[1], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDEFUERZA", strParametros[6], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDECODIGO", strParametros[7], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADPATRU_CODIGO", strParametros[8], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_FUERZ_ID", strParametros[9], typeof(decimal)));
                        objGestorGral1.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_CODIGO", strParametros[10], typeof(decimal)));
                        objGestorGral1.ParamsSalida.Add(new Tuple<string, object, Type>("P_SALIDA", "", typeof(string)));
                        objGestorGral1.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.INS_CADMEDIOSDISPONIBLES", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                        foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                        {
                            psalida = Convert.ToString(parameter.Item2.ToString());
                        }
                        //strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", medioDisponible.CONSECUTIVO.ToString());
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", dblConsecutivo.ToString());
                        //contexto.Entry(medioDisponible).State = EntityState.Modified;
                    }
                    else
                    {

                        //Crea un nuevo registro rmr 30-12-2020
                        dblConsecutivo = Convert.ToDouble(gestor.ObtenerMedioDisponibleConsecutivo());
                        popcion = "00";
                        medioDisponible = new CADMEDIOSDISPONIBLE();
                        //strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", medioDisponible.CONSECUTIVO.ToString());
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", dblConsecutivo.ToString());//rmr 30/12/2020
                        objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                        objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_OPCION", popcion, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_SITIO_GRABA", strParametros[0], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_CONSECUTIVO", Convert.ToString(dblConsecutivo), typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_ESTADO", strParametros[5], typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PTIPO", strParametros[3], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_SITIOGRABA", strParametros[4], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_TURNO", strParametros[1], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDEFUERZA", strParametros[6], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADUNITUR_UNDECODIGO", strParametros[7], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADPATRU_CODIGO", strParametros[8], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_FUERZ_ID", strParametros[9], typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_PCADCANA_CODIGO", strParametros[10], typeof(decimal)));
                        objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_SALIDA", "", typeof(string)));
                        objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.INS_CADMEDIOSDISPONIBLES", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                        foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                        {
                            psalida = Convert.ToString(parameter.Item2.ToString());                            
                        }
                        //Crea un nuevo registro rmr 30-12-2020
                        //Crea un nuevo registro
                        /*
                        medioDisponible = new CADMEDIOSDISPONIBLE();
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", medioDisponible.CONSECUTIVO.ToString());
                        medioDisponible.SITIO_GRABA = Convert.ToInt32(strParametros[0]);//sitio_graba-1
                        medioDisponible.CONSECUTIVO = gestor.ObtenerMedioDisponibleConsecutivo();//consecutivo-2
                        medioDisponible.CADUNITUR_TURNO = Convert.ToInt32(strParametros[1]);//cadunitur_turno-6
                        medioDisponible.TIPO = strParametros[3];//tipo-4
                        medioDisponible.CADUNITUR_SITIOGRABA = Convert.ToInt32(strParametros[4]);//cadunitur_sitiograba-5
                        medioDisponible.ESTADO = strParametros[5];//estado-3
                        medioDisponible.CADUNITUR_UNDEFUERZA = Convert.ToInt32(strParametros[6]);//cadunitur_undefuerza-7
                        medioDisponible.CADUNITUR_UNDECODIGO = Convert.ToInt32(strParametros[7]);//cadunitur_undecodigo-8
                        //medioDisponible.CADPATRU_CODIGO = Convert.ToInt16(strParametros[8]); RMR 08-12-2020
                        int algo = Convert.ToInt32(strParametros[8]);
                        medioDisponible.CADPATRU_CODIGO = Convert.ToInt16(algo);//cadpatru_codigo-9
                        medioDisponible.CADCANA_FUERZ_ID = Convert.ToInt32(strParametros[9]);//cadcana_fuerz_id-10
                        medioDisponible.CADCANA_CODIGO = Convert.ToByte(strParametros[10]);//cadcana_codigo
                        contexto.CADMEDIOSDISPONIBLES.Add(medioDisponible);*/
                    }
                    contexto.SaveChanges();
                    resultados = Json(new { result = true, datos = strMensaje });
                }
                else
                {
                    resultados = Json(new { result = false, datos = "No se ha enviado ningún parámetro" });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = GestorBase.EX_CODIGO });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerPersonasMedioDisp(int intSitio, long lgConsecutivo)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadPerMedios> elementos = new List<Ext_CadPerMedios>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                int intCodigoId = 0;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +

                    "<li> <button class='btn btn-default ' type='button' " +
                    " data-popup='tooltip' onclick=\"_EDITAR\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Editar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_MEDIOS\" data-placement='bottom' data-original-title='Clic para consultar los medios asignados'> <i class='icon-list'></i> Medios por Persona </button> </li>" +
                    "</ul> </li></ul>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = "  WHERE (INTERNO.CADPOLI_CEDU_EMPLEADO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.EMPLEADO, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.CARGO, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CADPOLI_CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CADPOLI_CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.CARGO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.ORDENAMIENTO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                //if (blFiltro) { strLimites += " AND "; }
                strLimites += " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "PERSONAS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", lgConsecutivo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", "02", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_codigo", "0", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_minuta", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonFinal = "";
                    strBotonFinal = strBoton.Replace("_CODIGO", intCodigoId.ToString());
                    strBotonFinal = strBotonFinal.Replace("_EDITAR", "EditarPersonal('" + fila["CADPOLI_CEDU_EMPLEADO"].ToString() + "','" + fila["EMPLEADO"].ToString() + "'," + (!string.IsNullOrEmpty(fila["CODI_CARGO"].ToString()) ? fila["CODI_CARGO"].ToString() : "0") + "," + ((!string.IsNullOrEmpty(fila["ORDENAMIENTO"].ToString())) ? fila["ORDENAMIENTO"].ToString() : "0") + ",'" + fila["OBSERVACION"].ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_MEDIOS", "CargarTablaMedios('" + fila["CADPOLI_CEDU_EMPLEADO"].ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarPersonaMedio(" + fila["CADPOLI_CEDU_EMPLEADO"].ToString() + ",'" + fila["EMPLEADO"].ToString() + "')");
                    elementos.Add(new Ext_CadPerMedios(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite consultar los empleados disponibles en una entidad especifíca
        /// </summary>
        /// <param name="intEntidadId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerPersonasXEntidad(int intEntidadId)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadPolicia> elementos = new List<Ext_CadPolicia>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionaPersona(_CEDULA,'_NOMBRE')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CEDU_EMPLEADO = " + parametros.ConsultaValor + " OR INTERNO.CADUNID_FUERZA = " + parametros.ConsultaValor + " OR INTERNO.CADUNID_CODIGO= " + parametros.ConsultaValor + " OR INTERNO.NUME_PLACA =" + parametros.ConsultaValor + " ) ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.NOMB_EMPLEADO, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.GRAD_ALFABETICO, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CEDU_EMPLEADO  " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CEDU_EMPLEADO  " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.NOMB_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.GRAD_ALFABETICO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.CADUNID_FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.CADUNID_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.NUME_PLACA " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", intEntidadId.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_policiasXentidad", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new Ext_CadPolicia(fila, strBoton.Replace("_CEDULA", fila["CEDU_EMPLEADO"].ToString()).Replace("_NOMBRE", fila["NOMB_EMPLEADO"].ToString())));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite cargas las tablas de medios asociados a un empleado
        /// </summary>
        /// <param name="strParametros"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerMediosPersona(string[] strParametros)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<_TablaMinutaMedios> elementos = new List<_TablaMinutaMedios>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                string strCedula = "NOT NULL";
                bool blFiltro = false;
                JsonResult resultados;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +

                    "<li> <button class='btn btn-default ' type='button' " +
                    " data-popup='tooltip' onclick=\"_EDITAR\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Editar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li>" +
                    "</ul> </li></ul>";

                if (strParametros.Length < 4)
                {
                    resultados = Json(new { result = false, datos = "Parámetros insuficientes para completar esta acción." });
                }
                else
                {
                    objGestorGral = new GestorGeneral();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    //Verifica si se debe realizar búsqueda de registros
                    if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                    {
                        blFiltro = true;
                        strQuery = " AND (REGEXP_LIKE(DATOS.SERIE, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(DATOS.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                    if (!strParametros[3].ToString().Equals("0") || !string.IsNullOrEmpty(strParametros[3]))
                    {
                        strCedula = strParametros[3];
                    }
                    //Identifica el tipo de medio y genera el ordenamiento correspondiente
                    strOrdenamiento = ConstruirComplementoConsulta(strParametros[2], parametros);
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", strParametros[2], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strParametros[0], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consecutivo", strParametros[1], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                    DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultarmediosempleado", strConexion, "Nonquery");
                    //var dttFuerza = gestor.GetFuerza(strConexion);
                    foreach (DataRow fila in dttLugar.Rows)
                    {
                        strBotonFinal = strBoton;
                        strBotonFinal = strBotonFinal.Replace("_EDITAR", "EditarEquipamiento(" + fila["CONSECUTIVO"].ToString() + ",'" + fila["SERIE"].ToString() + "', '" + fila["DESCRIPCION"].ToString() + "', '" + fila["DESC_UNIDAD"].ToString() + "','" + strParametros[2] + "')");
                        strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarEquipo('" + fila["SERIE"].ToString() + "', '" + strParametros[2] + "')");
                        elementos.Add(new _TablaMinutaMedios(strParametros[2], fila, strBotonFinal));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                        parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    }
                    //Verifica si se ha aplicado algun filtro a  la consulta
                    if (!blFiltro)
                    {
                        parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                    }
                    resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                }
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
        [HttpPost]
        public JsonResult ObtenerMediosDisponiblesEmpleado(string[] strParametros)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<_TablaMinutaMedios> elementos = new List<_TablaMinutaMedios>();
                GestorGeneral objGestorGral;
                CADFUERZA entidad;
                int intSitio = 0, intEntidadId = 0;
                long lgTurno = 0;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                string strOpcion = "";
                bool blFiltro = false;
                JsonResult resultados;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarEquipo('_SERIE' ,'_NOMBRE', '_MEDIO')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                if (strParametros.Length < 4)
                {
                    resultados = Json(new { result = false, datos = "Parámetros insuficientes para completar esta acción." });
                }
                else
                {
                    strOpcion = strParametros[2];
                    objGestorGral = new GestorGeneral();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    //Verifica si se debe realizar búsqueda de registros
                    if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                    {
                        blFiltro = true;
                        strQuery = " AND (REGEXP_LIKE(DATOS.SERIE, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(DATOS.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                    intSitio = Convert.ToInt32(strParametros[0]);
                    lgTurno = Convert.ToInt32(strParametros[1]);
                    //consulta el turno y entidad

                    intEntidadId = Convert.ToInt32(strParametros[4]);
                    entidad = (from registro in db.CADFUERZAS
                               where registro.ID == intEntidadId
                               select registro).FirstOrDefault();
                    //Identifica el tipo de medio y genera el ordenamiento correspondiente
                    strOrdenamiento = ConstruirComplementoConsulta(strParametros[2], parametros);
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", strParametros[2], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strParametros[0], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", strParametros[1], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno_unidad", strParametros[3], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad_secundaria", entidad.CADUNDE_CODIGO.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", entidad.CADUNDE_FUERZA.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                    DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultarvehiarmrad", strConexion, "Nonquery");
                    //var dttFuerza = gestor.GetFuerza(strConexion);
                    foreach (DataRow fila in dttLugar.Rows)
                    {
                        elementos.Add(new _TablaMinutaMedios(strParametros[2], fila, strBoton.Replace("_SERIE", fila["SERIE"].ToString()).Replace("_NOMBRE", fila["DESCRIPCION"].ToString()).Replace("_MEDIO", strParametros[2])));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                        parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    }
                    //Verifica si se ha aplicado algun filtro a  la consulta
                    if (!blFiltro)
                    {
                        parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                    }
                    resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite generar de manera dinamica el ordenamiento para las tablas de vehiculos, armas y radios
        /// </summary>
        /// <param name="strOpcion"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        [Authorize]
        private string ConstruirComplementoConsulta(string strOpcion, DatatableForm parametros)
        {
            try
            {
                string strOrdenamiento = "";

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY DATOS.SERIE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        if (strOpcion.Equals("RADIOS"))
                            break;
                        strOrdenamiento += " ORDER BY DATOS.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY DATOS.DESC_UNIDAD " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                return strOrdenamiento;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite guardar o actualiza la un empleado asociado a un medio de atención
        /// </summary>
        /// <param name="strParametros"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult GuardarPersonal(string[] strParametros)
        {
            try
            {
                string strMensaje = "";
                byte btOrdenamiento = 0;
                short shCargo = 0;
                JsonResult resultados;
                CADPERDISPONIBLE personalDisp;
                int intSitio = Convert.ToInt32(strParametros[0]);
                long lgTurno = Convert.ToInt64(strParametros[1]);
                long lgConsecutivo = Convert.ToInt64(strParametros[2]);
                string strCedula = (strParametros[3]);
                if (!string.IsNullOrEmpty(strParametros[4]))
                    shCargo = Convert.ToInt16(strParametros[4]);
                if (!string.IsNullOrEmpty(strParametros[6]))
                {
                    btOrdenamiento = Convert.ToByte(strParametros[6]);
                }

                CADTURNO turno = gestor.ConsultarTurno(intSitio, lgTurno);
                if (strParametros.Length > 0)
                {
                    //busca el registro
                    personalDisp = (from registro in contexto.CADPERDISPONIBLES
                                    where registro.CADMEDISP_SITIOGRABA == intSitio
                                    && registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                                    && registro.CADPOLI_CEDU_EMPLEADO == strCedula
                                    select registro).FirstOrDefault();
                    if (personalDisp != null)
                    {
                        //actualiza el registro
                        if (!string.IsNullOrEmpty(strParametros[4]))
                            personalDisp.CODI_CARGO = shCargo;
                        personalDisp.OBSERVACION = strParametros[5];
                        if (!string.IsNullOrEmpty(strParametros[6]))
                            personalDisp.ORDENAMIENTO = btOrdenamiento;
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", strParametros[7]);
                        contexto.Entry(personalDisp).State = EntityState.Modified;
                    }
                    else
                    {
                        //Crea un nuevo registro
                        personalDisp = new CADPERDISPONIBLE();
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", strParametros[7]);
                        personalDisp.CADMEDISP_CONSECUTIVO = lgConsecutivo;
                        personalDisp.CADMEDISP_SITIOGRABA = intSitio;
                        personalDisp.CADPOLI_CEDU_EMPLEADO = strCedula;
                        if (!string.IsNullOrEmpty(strParametros[4]))
                            personalDisp.CODI_CARGO = shCargo;
                        personalDisp.OBSERVACION = strParametros[5];
                        if (!string.IsNullOrEmpty(strParametros[6]))
                            personalDisp.ORDENAMIENTO = btOrdenamiento;
                        contexto.CADPERDISPONIBLES.Add(personalDisp);
                    }
                    contexto.SaveChanges();
                    resultados = Json(new { result = true, datos = strMensaje });
                }
                else
                {
                    resultados = Json(new { result = false, datos = "No se ha enviado ningún parámetro" });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = GestorBase.EX_CODIGO });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="strParametros"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult GuardarEquipoVehiculo(string[] strParametros)
        {
            try
            {
                bool blResultado = true;
                CADVEHIDISPONIBLE vehiculo;
                long lgConsecutivo = Convert.ToInt64(strParametros[1]);
                int intSitio = Convert.ToInt32(strParametros[0]);
                string strCedula = strParametros[2], strSerie = strParametros[3], strObservacion = strParametros[4];
                strMensaje = "";
                if (strParametros.Length <= 4)
                {
                    blResultado = false;
                    strMensaje = "No se han proporcionado los parámetros necesarios para completar la acción";
                }
                else
                {
                    //Verifica si el registro existe
                    vehiculo = (from registro in contexto.CADVEHIDISPONIBLES
                                where registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                                && registro.CADMEDISP_SITIOGRABA == intSitio
                                && registro.CADVEHI_NUMESIGLA == strSerie
                                && registro.POLI_CEDU_EMPLEADO == strCedula
                                select registro).FirstOrDefault();
                    if (vehiculo != null)
                    {
                        vehiculo.OBSERVACION = strObservacion;
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", vehiculo.CADVEHI_NUMESIGLA);
                        contexto.Entry(vehiculo).State = EntityState.Modified;
                    }
                    else
                    {
                        vehiculo = new CADVEHIDISPONIBLE();
                        vehiculo.CADMEDISP_CONSECUTIVO = lgConsecutivo;
                        vehiculo.CADMEDISP_SITIOGRABA = intSitio;
                        vehiculo.CADVEHI_NUMESIGLA = strSerie;
                        vehiculo.POLI_CEDU_EMPLEADO = strCedula;
                        vehiculo.OBSERVACION = strObservacion;
                        contexto.CADVEHIDISPONIBLES.Add(vehiculo);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", vehiculo.CADVEHI_NUMESIGLA);
                    }
                    contexto.SaveChanges();
                }
                return Json(new { result = blResultado, datos = strMensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GuardarEquipoArma(string[] strParametros)
        {
            try
            {
                bool blResultado = true;
                CADARMDISPONIBLE arma;
                long lgConsecutivo = Convert.ToInt64(strParametros[1]);
                int intSitio = Convert.ToInt32(strParametros[0]);
                string strCedula = strParametros[2], strSerie = strParametros[3], strObservacion = strParametros[4];
                strMensaje = "";
                if (strParametros.Length <= 4)
                {
                    blResultado = false;
                    strMensaje = "No se han proporcionado los parámetros necesarios para completar la acción";
                }
                else
                {
                    //Verifica si el registro existe
                    arma = (from registro in contexto.CADARMDISPONIBLES
                            where registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                            && registro.CADMEDISP_SITIOGRABA == intSitio
                            && registro.CADARM_NUME_SERIE == strSerie
                            && registro.CADPOLI_CEDU_EMPLEADO == strCedula
                            select registro).FirstOrDefault();
                    if (arma != null)
                    {
                        arma.OBSERVACION = strObservacion;
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", arma.CADARM_NUME_SERIE);
                        contexto.Entry(arma).State = EntityState.Modified;
                    }
                    else
                    {
                        arma = new CADARMDISPONIBLE();
                        arma.CADMEDISP_CONSECUTIVO = lgConsecutivo;
                        arma.CADMEDISP_SITIOGRABA = intSitio;
                        arma.CADARM_NUME_SERIE = strSerie;
                        arma.CADPOLI_CEDU_EMPLEADO = strCedula;
                        arma.OBSERVACION = strObservacion;
                        contexto.CADARMDISPONIBLES.Add(arma);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", arma.CADARM_NUME_SERIE);
                    }
                    contexto.SaveChanges();
                }
                return Json(new { result = blResultado, datos = strMensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GuardarEquipoRadio(string[] strParametros)
        {
            try
            {
                bool blResultado = true;
                CADRADIODISPONIBLE radio;
                long lgConsecutivo = Convert.ToInt64(strParametros[1]);
                int intSitio = Convert.ToInt32(strParametros[0]);
                string strCedula = strParametros[2], strSerie = strParametros[3], strObservacion = strParametros[4];
                strMensaje = "";
                if (strParametros.Length <= 4)
                {
                    blResultado = false;
                    strMensaje = "No se han proporcionado los parámetros necesarios para completar la acción";
                }
                else
                {
                    //Verifica si el registro existe
                    radio = (from registro in contexto.CADRADIODISPONIBLES
                             where registro.CADMEDISP_CONSECUTIVO == lgConsecutivo
                             && registro.CADMEDISP_SITIOGRABA == intSitio
                             && registro.CADRAD_NUME_SERIE == strSerie
                             && registro.CADPOLI_CEDU_EMPLEADO == strCedula
                             select registro).FirstOrDefault();
                    if (radio != null)
                    {
                        radio.OBSERVACION = strObservacion;
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", radio.CADRAD_NUME_SERIE);
                        contexto.Entry(radio).State = EntityState.Modified;
                    }
                    else
                    {
                        radio = new CADRADIODISPONIBLE();
                        radio.CADMEDISP_CONSECUTIVO = lgConsecutivo;
                        radio.CADMEDISP_SITIOGRABA = intSitio;
                        radio.CADRAD_NUME_SERIE = strSerie;
                        radio.CADPOLI_CEDU_EMPLEADO = strCedula;
                        radio.OBSERVACION = strObservacion;
                        contexto.CADRADIODISPONIBLES.Add(radio);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", radio.CADRAD_NUME_SERIE);
                    }
                    contexto.SaveChanges();
                }
                return Json(new { result = blResultado, datos = strMensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [SecadPlusAutorizador("/Turnos/ServiciosPrestados")]
        public ActionResult ServiciosPrestados()
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
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }

                //return RedirectToAction("Inicio", "Turnos", new { intSitioGrabacion= usuarioActual.SITIO_GRABACION, strNombre, intEntidad, strEntidad,strConsulta = "CADCONDTUROS"});
                return RedirectToAction("Inicio", "Turnos", new { strConsulta = "CADCONDTUROS" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConductasAsociadas(int dcSitio, long lgTurno, int intEntidad, string strSitio, string strEntidad)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadCondTurno> elementos = new List<ExtCadCondTurno>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button'  data-popup='tooltip' onclick=\"_PERSONAS\" data-placement='bottom' data-original-title='Clic para ver las personas involucradas'> <i class='icon icon-pencil'> </i> Personas </button> </li> "
                    + "<li> <button class='btn btn-default ' type='button'  data-popup='tooltip' onclick=\"_BIENES\" data-placement='bottom' data-original-title='Clic para ver los bienes involucrados'> <i class='icon icon-pencil'> </i> Bienes </button> </li> "
                    + "<li> <button class='btn btn-default ' type='button'  data-popup='tooltip' onclick=\"_COMPARENDOS\" data-placement='bottom' data-original-title='Clic para ver los comparendos involucrados'> <i class='icon icon-pencil'> </i> Comparendos </button> </li> "
                    + "</ul></li></ul>";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CADCONDCODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.TIPO, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.DESCRIPCION,'" + parametros.ConsultaValor + "', 'i')  "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CADCONDCODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CADCONDCODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", dcSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultarconductaturno", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarConducta(" + fila["CADTURNSITIOGRABA"].ToString() + "," + fila["CADTURNTURNO"].ToString() + "," + fila["CADCONDCODIGO"].ToString() + ")");
                    strBotonFinal = strBotonFinal.Replace("_PERSONAS", "AbrirVentana('PERSONAS','" + fila["PERSONAS"].ToString() + "','" + Url.Action("Inicio", "ConductasPersona", new { intSitio = fila["CADTURNSITIOGRABA"].ToString(), lgTurno = fila["CADTURNTURNO"].ToString(), lgConducta = fila["CADCONDCODIGO"].ToString(), intEntidad = intEntidad }) + "')");
                    strBotonFinal = strBotonFinal.Replace("_BIENES", "AbrirVentana('BIENES','" + fila["BIENES"].ToString() + "','" + Url.Action("Inicio", "CondTurnBienes", new { intSitio = fila["CADTURNSITIOGRABA"].ToString(), lgTurno = fila["CADTURNTURNO"].ToString(), lgConducta = fila["CADCONDCODIGO"].ToString(), intEntidad = intEntidad }) + "')");
                    strBotonFinal = strBotonFinal.Replace("_COMPARENDOS", "AbrirVentana('COMPARENDOS','" + fila["COMPARENDOS"].ToString() + "','" + Url.Action("Inicio", "CondTurnComparendo", new { intSitio = fila["CADTURNSITIOGRABA"].ToString(), lgTurno = fila["CADTURNTURNO"].ToString(), lgConducta = fila["CADCONDCODIGO"].ToString(), intEntidad = intEntidad }) + "')");
                    elementos.Add(new ExtCadCondTurno(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
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
        [HttpPost]
        public JsonResult ObtenerConductas(string strTipo)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadConducta> elementos = new List<ExtCadConducta>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarConducta(_CODIGO,'_DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE ( REGEXP_LIKE(INTERNO.DESCRIPCION,'" + parametros.ConsultaValor + "', 'i')  "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo", strTipo, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_conductasturno", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_DESCRIPCION", fila["DESCRIPCION"].ToString());
                    elementos.Add(new ExtCadConducta(fila, strBotonFinal));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
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
        [HttpPost]
        public JsonResult GuardarConductaTurno(string[] strParametros)
        {
            try
            {
                bool blResultado = true;
                JsonResult resultados = new JsonResult();
                CADCONDTURNO dato;
                long lgConsecutivo = Convert.ToInt64(strParametros[1]);
                int intSitio = Convert.ToInt32(strParametros[0]);
                int intConducta = Convert.ToInt32(strParametros[2]);
                string strTipo = strParametros[3];
                strMensaje = "";
                resultados.MaxJsonLength = int.MaxValue;
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                    return resultados;
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validador.IdentificarCliente(Request, Session))
                    {
                        resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")" });
                        return resultados;
                    }
                }
                else if (strParametros.Length < 3)
                {
                    blResultado = false;
                    strMensaje = "No se han proporcionado los parámetros necesarios para completar la acción";
                }
                else
                {
                    //Verifica si el registro existe
                    dato = (from registro in contexto.CADCONDTURNOes
                            where registro.CADCONDCODIGO == intConducta
                            && registro.CADTURNSITIOGRABA == intSitio
                            && registro.CADTURNTURNO == lgConsecutivo
                            select registro).FirstOrDefault();
                    if (dato != null)
                    {
                        if (validador.ValidarAsociadosConducta(dato))
                        {
                            dato.CADCONDTIPO = strTipo;
                            strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", dato.CADCONDCODIGO.ToString());
                            contexto.Entry(dato).State = EntityState.Modified;
                        }
                        else
                        {
                            strMensaje = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Conducta seleccionada");
                            blResultado = false;
                        }
                    }
                    else
                    {
                        dato = new CADCONDTURNO();
                        dato.CADTURNSITIOGRABA = intSitio;
                        dato.CADCONDTIPO = strTipo;
                        dato.CADTURNTURNO = lgConsecutivo;
                        dato.CADCONDCODIGO = intConducta;
                        dato.USUA_GRABA = usuarioActual.USUARIO;
                        contexto.CADCONDTURNOes.Add(dato);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", dato.CADCONDCODIGO.ToString());
                    }
                    contexto.SaveChanges();
                }
                resultados= Json(new { result = blResultado, datos = strMensaje });
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
        public JsonResult EliminarConductaAsociada(int intSitio, long lgTurno, long lgConducta)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                CADCONDTURNO conductaAsociada = (from registro in contexto.CADCONDTURNOes
                                                 where registro.CADTURNSITIOGRABA == intSitio
                                                 && registro.CADCONDCODIGO == lgConducta
                                                 && registro.CADTURNTURNO == lgTurno
                                                 select registro).FirstOrDefault();
                //varifica que el registro no tenga datos asociados
                if (conductaAsociada != null)
                {
                    if (validador.ValidarAsociadosConducta(conductaAsociada))
                    {
                        contexto.CADCONDTURNOes.Remove(conductaAsociada);
                        contexto.SaveChanges();
                        //Almacena la auditoría



                        CADCONDTURNO conductaAsociada_copy = new CADCONDTURNO();
                        conductaAsociada_copy = (CADCONDTURNO)auditoria.GenerarCopiaAuditoria(conductaAsociada);


                        auditoria.RegistrarAuditoria<CADCONDTURNO>(conductaAsociada_copy, "CADCONDTURNO", "LLAVE COMPUESTA: SITIO:" + conductaAsociada.CADTURNSITIOGRABA.ToString() + " | TURNO:" + conductaAsociada.CADTURNTURNO.ToString() + " | CONDUCTA:" + conductaAsociada.CADCONDCODIGO.ToString(), TipoAuditoria.Eliminar);



                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                        resultado = Json(new { result = true, datos = strMensaje });
                    }
                    else
                    {
                        strMensaje = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Conducta seleccionada");
                        resultado = Json(new { result = false, datos = strMensaje });
                    }
                }
                else
                {
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    resultado = Json(new { result = false, datos = strMensaje });
                }

                return resultado;
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Unidad de Turno Seleccionado") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult CopiarTurno(long lgTurno, int intSitio, string strTipo, string strFechaInicia, string strFechaTermina)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                string strResultado = "";
                GestorGeneral objGestorGral = new GestorGeneral();
                long lgConsecutivo = gestor.ObtenerTurnoConsecutivo();

                if (Session["USUARIO_SECAD"] == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                }
                else
                {
                    usuarioActual = (CADUSUARIO)(Session["USUARIO_SECAD"]);
                }
                CADTURNO turnoSeleccionado = (from registro in contexto.CADTURNOS
                                              where registro.TURNO == lgTurno
                                              && registro.SITIO_GRABA == intSitio
                                              select registro).FirstOrDefault();
                if (turnoSeleccionado == null)
                {
                    resultados = Json(new { result = false, datos = "El turno seleccionado ya no existe" });
                }
                else
                {
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno_copiado", lgTurno.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno_consec", lgConsecutivo.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", strFechaInicia.ToString(), typeof(DateTime)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_termina", strFechaTermina.ToString(), typeof(DateTime)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consignas", turnoSeleccionado.CONSIGNAS, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza_id", turnoSeleccionado.CADFUERZ_ID.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", turnoSeleccionado.CLASE_TURNO, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo_turno", strTipo, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", usuarioActual.USUARIO, typeof(string)));

                    objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_resultado", null, typeof(string)));

                    objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_copiar_turno", strConexion, "MULTIPLE");
                    foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                    {
                        strResultado = parameter.Item2.ToString();
                    }
                    if (strResultado.Equals("null"))
                    {
                        strResultado = "Turno copiado de manera exitosa, nuevo turno con consecutivo (" + lgConsecutivo.ToString() + ").";
                        resultados = Json(new { result = true, datos = strResultado });
                    }
                    else
                    {
                        resultados = Json(new { result = false, datos = strResultado });
                    }
                }
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
        public JsonResult TraerMinutaSivic(long lgTurno, int intSitio, string strFechaInicia, string strFechaTermina)
        {
            try
            {
                //turno = ViewBag.cadunip_codigo;
                Session["lgTurno"] = lgTurno;
                Session["intSitio"] = intSitio;
                Session["strFechaInicia"] = strFechaInicia;
                Session["strFechaTermina"] = strFechaTermina;
                //Session["cadunip_codigo"]
                string varpcntminutas = "";
                string varpcntcuadrantes = "";
                string varpcntpersonasins = "";
                string varpcntpersonasupd = "";
                string valorsessioncadunip_codigo = Convert.ToString(Session["cadunip_codigo"]).ToString();
                String cadenastrFechaInicia = strFechaInicia; //Original Text
                String[] resultcadenastrFechaInicia = cadenastrFechaInicia.Split(':');
                string residuostrFechaInicia = resultcadenastrFechaInicia[0] + " " + resultcadenastrFechaInicia[1] + ":" + resultcadenastrFechaInicia[2] + ":" + resultcadenastrFechaInicia[3];

                String cadenastrFechaTermina = strFechaTermina; //Original Text
                String[] resultcadenastrFechaTermina = cadenastrFechaTermina.Split(':');
                string nuevaHora=adicion_segundo(resultcadenastrFechaTermina[1] + ":" + resultcadenastrFechaTermina[2] + ":" + resultcadenastrFechaTermina[3]);
                string residuostrFechaTermina = resultcadenastrFechaTermina[0] + " " + nuevaHora;


                DateTime nuevaFecha = Convert.ToDateTime(resultcadenastrFechaTermina[0]+" "+ resultcadenastrFechaTermina[1] + ":" + resultcadenastrFechaTermina[2] + ":" + resultcadenastrFechaTermina[3]);
                nuevaFecha = nuevaFecha.AddSeconds(1);


                JsonResult resultados = new JsonResult();
                string strResultado = "";
                GestorGeneral objGestorGral = new GestorGeneral();
                long lgConsecutivo = gestor.ObtenerTurnoConsecutivo();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PUNIDADSECAD", Convert.ToString(Session["cadunip_codigo"]).ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PHORAINICIO", residuostrFechaInicia.ToString(), typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PHORATERMINO", nuevaFecha.ToString(), typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PSITIOGRABA", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PTURNOSECAD", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCNTMINUTAS", null, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCNTCUADRANTES", null, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCNTPERSONASINS", null, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCNTPERSONASUPD", null, typeof(decimal)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadintegminutasecadsivicc.p_integracionsiviccsecad", strConexion, "MULTIPLE");
                

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    //'Resultado Integracion:'||chr(10)||'Cnt Minutas: '||VCNTMINUTAS||' Cuadrantes: '||VCNTCUADRANTES||' Personas ins: '||VCNTPERSONASINS||' Personas Upd: '||VCNTPERSONASUPD,
                    
                    if(parameter.Item1.Equals("PCNTMINUTAS"))
                    {
                        varpcntminutas = Convert.ToString(parameter.Item2.ToString());
                    }
                    if (parameter.Item1.Equals("PCNTCUADRANTES"))
                    {
                        varpcntcuadrantes = Convert.ToString(parameter.Item2.ToString());
                    }
                    if (parameter.Item1.Equals("PCNTPERSONASINS"))
                    {
                        varpcntpersonasins = Convert.ToString(parameter.Item2.ToString());
                    }
                    if (parameter.Item1.Equals("PCNTPERSONASUPD"))
                    {
                        varpcntpersonasupd = Convert.ToString(parameter.Item2.ToString());
                    }

                }

                var DatosResult = "Resultado Integracion: Cnt Minutas: " + varpcntminutas.ToString() + " Cuadrantes: " + varpcntcuadrantes.ToString() + " Personas ins: " + varpcntpersonasins.ToString() + " Personas Upd: " + varpcntpersonasupd.ToString();
                                
                if (DatosResult.Equals("null"))
                {
                    strResultado = "RES (" + lgConsecutivo.ToString() + ").";
                    //resultados = Json(new { result = true, datos = strResultado });
                }
                else
                {
                    strResultado = DatosResult;

                    //return resultados = Json(new { result = false, datos = strResultado });
                }

                return Json(new { result = true, datos = strResultado }, JsonRequestBehavior.AllowGet);

                //resultados.MaxJsonLength = int.MaxValue;
                //return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
        public JsonResult RecargarMinutaSivic(long lgTurno, int intSitio, string strFechaInicia, string strFechaTermina)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                string strResultado = "";
                GestorGeneral objGestorGral = new GestorGeneral();
                //long lgConsecutivo = gestor.ObtenerTurnoConsecutivo();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_sivicc_minutas.f_getminutas", strConexion, "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    strResultado = "Recarga minutas ejecutada con exito";
                }
                strResultado = "Recarga minutas ejecutada con exito";
                var DatosResult = strResultado;
                if (DatosResult.Equals("null"))
                {
                    strResultado = "RES (" + 0 + ").";
                    //resultados = Json(new { result = true, datos = strResultado });
                }
                else
                {
                    strResultado = DatosResult;
                    //return resultados = Json(new { result = false, datos = strResultado });
                }
                return Json(new { result = true, datos = strResultado }, JsonRequestBehavior.AllowGet);                
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public string adicion_segundo(string hora)
        {
            string nueva_hora = "";
            switch (hora)
            {
                case "13:59:59":
                    nueva_hora = "14:00:00";
                    break;
                case "21:59:59":
                    nueva_hora = "22:00:00";
                    break;
                case "06:59:59":
                    nueva_hora = "07:00:00";
                    break;
                default:
                    nueva_hora = "00:00:00";
                    break;
            }
            return nueva_hora;

        }

        [Authorize]
        [HttpPost]
        public JsonResult VerificarTurnoTruncado(string strSitio, string strTurno, string strClase, string strFechaInicia, string strFechaFin)
        {
            try
            {
                Session["vstrFechaInicia"] = strFechaInicia;
                Session["vstrFechaFin"] = strFechaFin;
                JsonResult resultado = new JsonResult();
                GestorGeneral gestorGeneral = new GestorGeneral();
                decimal dcCantidad = 0;
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", strTurno, typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase", strClase, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha_inicia", strFechaInicia, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha_fin", strFechaFin, typeof(string)));
                dcCantidad = gestorGeneral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADFIXES.f_verificar_turno_truncado", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));

                resultado = Json(new { result = true, datos = dcCantidad });
                resultado.MaxJsonLength = int.MaxValue;
                return resultado;
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Unidad de Turno Seleccionado") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                contexto.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Minutas()
        {

            return View();
        }

    }
}