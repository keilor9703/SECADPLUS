using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class CondTurnComparendoController : Controller
    {
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private CADUSUARIO usuarioActual;
        private Validador validar = new Validador();
        private CADTURNO turno;
        private GestorAuditoria auditoria = new GestorAuditoria();
        private string strMensaje = "";

        [Authorize]
        public ActionResult Inicio(int intSitio, long lgTurno, long lgConducta, int intEntidad, long? lgEvento = null, long? lgEventoMedio = null, string strFecha = null, string strMensaje = null)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
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
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.CONSECUTIVO = (from registro in contexto.CADCONDTURNOCOMPs
                                       where registro.SITIO_GRABA == intSitio
                                       select new { registro.CONSECUTIVO }).OrderByDescending(c => c.CONSECUTIVO).FirstOrDefault().CONSECUTIVO;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.EventoFecha = strFecha;
                ViewBag.Editar = 0;



                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { intSitio = intSitio, lgTurno = lgTurno, lgConducta = lgConducta,
                    intEntidad = intEntidad,
                    lgEvento = lgEvento,
                    lgEventoMedio = lgEventoMedio
              
                });
                ViewBag.NombreUbicacion = "Turnos-Comparendos";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConductaComparendos(int intSitio, long lgTurno, long lgConducta, int intEntidad)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadCondComparendo> elementos = new List<ExtCadCondComparendo>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"_MEDIOS\" data-placement='bottom' data-original-title='Clic para gestionar Medios'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Medios </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' " +
                    " data-popup='tooltip' onclick=\"AbrirEditarNormal('_EDITAR')\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Editar </button> </li><li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li></ul> </li></ul>";

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
                        strQuery = " WHERE (INTERNO.CODIGO = " + parametros.ConsultaValor + " ) ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.CANAL,'" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.LINEA,'" + parametros.ConsultaValor + "', 'i') "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.CADEVENNUMEEVENTO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_CONDUCTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.CANTIDAD " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_conducta", lgConducta.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_conductas_comparendo", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_MEDIOS", "ConsultarMediosComparendos(" + fila["SITIO_GRABA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + ")");
                    strBotonFinal = strBotonFinal.Replace("_EDITAR", (Url.Action("Editar", new { intSitio = fila["SITIO_GRABA"].ToString(), lgConsecutivo = fila["CONSECUTIVO"].ToString(), strInfraccion = fila["DESCRIPCION"].ToString(), lgTurno = lgTurno, intEntidad = intEntidad, lgConducta = fila["CADCONDTURNCOND"].ToString() })).ToString());
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarComparendo(" + fila["SITIO_GRABA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + ",'" + fila["DESCRIPCION"].ToString() + "')");
                    elementos.Add(new ExtCadCondComparendo(fila, strBotonFinal));
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
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public ActionResult Crear(int intSitio, long lgTurno, int intEntidad, long lgConducta)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.CONSECUTIVO = (from registro in contexto.CADCONDTURNOCOMPs
                                       where registro.SITIO_GRABA == intSitio
                                       select new { registro.CONSECUTIVO }).OrderByDescending(c => c.CONSECUTIVO).FirstOrDefault().CONSECUTIVO;
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.Entidad = intEntidad;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ConductaTurnoBien/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCONDTURNSITGR,CADCONDTURNTURNO,CADCONDTURNCOND,CODI_INFRACCION,CANTIDAD,CADEVENSITIOGRABA,CADEVENNUMEEVENTO,OBSERVACIONES,FECHA_CONDUCTA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION")] CADCONDTURNOCOMP cADCONDTURNOCOMP)
        {
            try
            {
                string strNombre = "", strMensaje = "";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
                strNombre = Session["MAQUINA"].ToString();
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == cADCONDTURNOCOMP.SITIO_GRABA
                         && registros.TURNO == cADCONDTURNOCOMP.CADCONDTURNTURNO
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.SitioId = cADCONDTURNOCOMP.SITIO_GRABA;
                ViewBag.Turno = cADCONDTURNOCOMP.CADCONDTURNTURNO;
                ViewBag.Conducta = cADCONDTURNOCOMP.CADCONDTURNCOND;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOCOMP.SITIO_GRABA).DESCRIPCION;
                ViewBag.Entidad = Request.Form["hdEntidad"].ToString();
                ViewBag.EntidadId = Request.Form["hdEntidadId"].ToString();
                ViewBag.Evento = Request.Form["hdEvento"].ToString();
                ViewBag.EventoMedio = Request.Form["hdEventoMedio"].ToString();
                ViewBag.EventoFecha = Request.Form["hdFechaEvento"].ToString();
                ViewBag.CONSECUTIVO = cADCONDTURNOCOMP.CONSECUTIVO;
                ViewBag.Editar = 0;
                if (ModelState.IsValid)
                {
                    cADCONDTURNOCOMP.MAQUINA_TRANSACCION = strNombre;
                    cADCONDTURNOCOMP.USUA_TRANSACCION = usuarioActual.USUARIO;
                    cADCONDTURNOCOMP.SITIO_GRABA = usuarioActual.SITIO_GRABACION;
                    contexto.CADCONDTURNOCOMPs.Add(cADCONDTURNOCOMP);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADCONDTURNOCOMP.CONSECUTIVO.ToString());
                    return RedirectToAction("Inicio", "CondTurnComparendo", new { intSitio = cADCONDTURNOCOMP.SITIO_GRABA, lgTurno = cADCONDTURNOCOMP.CADCONDTURNTURNO, lgConducta = cADCONDTURNOCOMP.CADCONDTURNCOND, intEntidad = Request.Form["hdEntidadId"].ToString(), strMensaje = strMensaje });
                }
                return View("Inicio", cADCONDTURNOCOMP);
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Editar(int intSitio, long lgConsecutivo, string strInfraccion, long lgTurno, int intEntidad, long lgConducta, long? lgEvento = null, long? lgEventoMedio = null, string strFecha = null)
        {
            try
            {
                CADCONDTURNOCOMP comparendo;
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }
                //consulta el comparendo
                comparendo = (from registro in contexto.CADCONDTURNOCOMPs
                              where registro.SITIO_GRABA == intSitio
                              && registro.CONSECUTIVO == lgConsecutivo
                              select registro).FirstOrDefault();
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.Infraccion = strInfraccion;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEvento;
                ViewBag.EventoFecha = strFecha;
                ViewBag.Editar = 1;
                return View("Inicio", comparendo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCONDTURNSITGR,CADCONDTURNTURNO,CADCONDTURNCOND,CODI_INFRACCION,CANTIDAD,CADEVENSITIOGRABA,CADEVENNUMEEVENTO,OBSERVACIONES,FECHA_CONDUCTA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION")] CADCONDTURNOCOMP cADCONDTURNOCOMP)
        {
            try
            {
                string strNombre = "", strMensaje = "";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
                strNombre = Session["MAQUINA"].ToString();
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == cADCONDTURNOCOMP.SITIO_GRABA
                         && registros.TURNO == cADCONDTURNOCOMP.CADCONDTURNTURNO
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.SitioId = cADCONDTURNOCOMP.SITIO_GRABA;
                ViewBag.Turno = cADCONDTURNOCOMP.CADCONDTURNTURNO;
                ViewBag.Conducta = cADCONDTURNOCOMP.CADCONDTURNCOND;
                ViewBag.Entidad = Request.Form["hdEntidad"].ToString();
                ViewBag.Infraccion = Request.Form["hdInfDesc"].ToString();
                ViewBag.SitioId = cADCONDTURNOCOMP.SITIO_GRABA;
                ViewBag.Turno = cADCONDTURNOCOMP.CADCONDTURNTURNO;
                ViewBag.Conducta = cADCONDTURNOCOMP.CADCONDTURNCOND;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOCOMP.SITIO_GRABA).DESCRIPCION;
                ViewBag.EntidadId = Request.Form["hdEntidadId"].ToString();
                ViewBag.Evento = Request.Form["hdEvento"].ToString();
                ViewBag.EventoMedio = Request.Form["hdEventoMedio"].ToString();
                ViewBag.EventoFecha = Request.Form["hdFechaEvento"].ToString();
                ViewBag.CONSECUTIVO = cADCONDTURNOCOMP.CONSECUTIVO;
                ViewBag.Editar = 1;
                if (ModelState.IsValid)
                {
                    cADCONDTURNOCOMP.MAQUINA_TRANSACCION = strNombre;
                    cADCONDTURNOCOMP.USUA_TRANSACCION = usuarioActual.USUARIO;
                    cADCONDTURNOCOMP.SITIO_GRABA = usuarioActual.SITIO_GRABACION;
                    contexto.Entry(cADCONDTURNOCOMP).State = EntityState.Modified;
                    contexto.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCONDTURNOCOMP>(cADCONDTURNOCOMP, "CADCONDTURNOCOMP", "SITIO_GRABA=" + cADCONDTURNOCOMP.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + cADCONDTURNOCOMP.CONSECUTIVO
                     , TipoAuditoria.Modificar);



                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADCONDTURNOCOMP.CONSECUTIVO.ToString());
                    return RedirectToAction("Inicio", "CondTurnComparendo", new { intSitio = cADCONDTURNOCOMP.SITIO_GRABA, lgTurno = cADCONDTURNOCOMP.CADCONDTURNTURNO, lgConducta = cADCONDTURNOCOMP.CADCONDTURNCOND, intEntidad = Request.Form["hdEntidadId"].ToString(), strMensaje = strMensaje });
                }
                return View(cADCONDTURNOCOMP);
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerMediosComparendos(int intSitio, long lgConsecutivo, long lgTurno, int intEntidad)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadCondMedio> elementos = new List<ExtCadCondMedio>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +

                    "<li> <button class='btn btn-default ' type='button'  data-popup='tooltip' onclick=\"_EDITAR\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +

                    "</i> Editar </button> </li><li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"_ELIMINAR\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li>" +
                    " </li></ul>";

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
                        strQuery = " WHERE (INTERNO.CADMEDISPCONSEC = " + parametros.ConsultaValor + " OR INTERNO.CEDU_EMPLEADO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.EMPLEADO,'" + parametros.ConsultaValor + "', 'i')  "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CADMEDISPCONSEC " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CADMEDISPCONSEC " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.PONDERACION " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consecutivo", lgConsecutivo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_medios_comparendos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton.Replace("_EDITAR", "AbrirEdicion('" + Url.Action("Editar", "MediosComparendo", new { intSitio = intSitio, lgCompConsec = lgConsecutivo, lgMedioConsec = fila["CADMEDISPCONSEC"].ToString(), strCeduEmpleado = fila["CEDU_EMPLEADO"].ToString(), lgTurno = lgTurno, intEntidad = intEntidad, strPatrulla = fila["DESC_PATRULLA"].ToString(), strDesc = fila["DESCRIPCION"].ToString(), strPersona = fila["EMPLEADO"].ToString() }) + "')");
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarMedio(" + intSitio + "," + lgConsecutivo + "," + fila["CADMEDISPCONSEC"].ToString() + ",'" + fila["CEDU_EMPLEADO"].ToString() + "'," + lgTurno + ",0,'CONSULTA','COMPARENDO')");
                    elementos.Add(new ExtCadCondMedio(fila, strBotonFinal, "COMPARENDOS"));
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
        public JsonResult EliminarComparendo(int intSitio, long lgConsecutivo)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                CADCONDTURNOCOMP comparendo;
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    resultado = Json(new { result = false, datos = strMensaje });
                }
                if (!usuarioActual.USUARIO.Equals("USR_CAD"))
                {
                    strMensaje = Resources.RecursoGeneral.MSN_MINUTA_NO_ELIMINAR;
                    resultado = Json(new { result = false, datos = strMensaje });
                }
                comparendo = (from registro in contexto.CADCONDTURNOCOMPs
                              where registro.SITIO_GRABA == intSitio
                              && registro.CONSECUTIVO == lgConsecutivo
                              select registro).FirstOrDefault();
                //varifica que el registro no tenga datos asociados
                if (comparendo != null)
                {
                    //Verifica que el comparendo no posea registros asociados
                    if (validar.ValidarAsociadosComparendo(comparendo))
                    {
                        contexto.CADCONDTURNOCOMPs.Remove(comparendo);
                        contexto.SaveChanges();
                        //Almacena la auditoría

                        CADCONDTURNOPER comparendo_copy = new CADCONDTURNOPER();
                        comparendo_copy = (CADCONDTURNOPER)auditoria.GenerarCopiaAuditoria(comparendo);


                        auditoria.RegistrarAuditoria<CADCONDTURNOPER>(comparendo_copy, "CADCONDTURNOCOMP", "LLAVE COMPUESTA: SITIO:" + comparendo.SITIO_GRABA.ToString() + " | CONSECUTIVO:" + comparendo.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                        resultado = Json(new { result = true, datos = strMensaje });
                    }
                    else
                    {
                        strMensaje = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Persona seleccionada");
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
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Persona seleccionada") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}