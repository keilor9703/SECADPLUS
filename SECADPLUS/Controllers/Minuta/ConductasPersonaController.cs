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
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ConductasPersonaController : Controller
    {
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private CADTURNO turno;
        private CADUSUARIO usuarioActual;
        private GestorAuditoria auditoria = new GestorAuditoria();
        private Validador validador = new Validador();
        private string strMensaje = "";

        [Authorize]
        public ActionResult Inicio(int intSitio, long lgTurno, long lgConducta, int intEntidad, long? lgEvento = null, long? lgEventoMedio = null, string strFecha = null, string strMensaje = null)
        {
            try
            {
                if (!VerificarUsuario())
                {
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
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.TIPO_ID = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_DOCUMENTO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.PARTCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_PARTICIPACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.GENERO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_SEXO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.RELACONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "RELACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.EntidadId = intEntidad;

                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.EventoFecha = strFecha;
                ViewBag.InfraccionDesc = "";
                ViewBag.CondicionDesc = "";


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { intSitio = intSitio, lgTurno = lgTurno, lgConducta = lgConducta, intEntidad = intEntidad , lgEvento = lgEvento , lgEventoMedio = lgEventoMedio });
                ViewBag.NombreUbicacion = "Turnos-Personas";
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
        public JsonResult ObtenerConductaPersonas(int intSitio, long lgTurno, long lgConducta, int intEntidad, string strConsulta = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadCondPersonas> elementos = new List<ExtCadCondPersonas>();
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
                        strQuery = " WHERE (INTERNO.IDENTIFICACION = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.NOMBRES, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.APELLIDOS,'" + parametros.ConsultaValor + "', 'i') ";
                        strQuery += " OR REGEXP_LIKE(INTERNO.TIPO, '" + parametros.ConsultaValor + "', 'i')  " + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.APELLIDOS " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.NOMBRES " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.GENERO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_conducta", lgConducta.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_conductasturno", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_MEDIOS", "ConsultarMedios(" + fila["SITIO_GRABA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + "," + fila["CADCONDTURNCOND"].ToString() + ",'" + fila["MODOID"].ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_EDITAR", (Url.Action("Editar", new { intSitio = fila["SITIO_GRABA"].ToString(), lgConsecutivo = fila["CONSECUTIVO"].ToString(), lgTurno = lgTurno, lgConducta = fila["CADCONDTURNCOND"].ToString(), intEntidad = intEntidad, strInfraccion = fila["INFRACCIONDES"].ToString(), strCondicion = fila["CONDUCTADESC"].ToString() })).ToString());
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarPersona(" + fila["SITIO_GRABA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + ",'" + fila["IDENTIFICACION"].ToString() + "')");
                    elementos.Add(new ExtCadCondPersonas(fila, strBotonFinal));
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
        public ActionResult Editar(int intSitio, long lgConsecutivo, long lgTurno, long lgConducta, int intEntidad, string strInfraccion, string strCondicion, long? lgEvento = null, long? lgEventoMedio = null, string strFecha = null)
        {
            try
            {
                CADCONDTURNOPER persona = (from registro in contexto.CADCONDTURNOPERS
                                           where registro.CONSECUTIVO == lgConsecutivo
                                           && registro.SITIO_GRABA == intSitio
                                           select registro).FirstOrDefault();
                if (persona == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    return RedirectToAction("Inicio", "ConductasPersona", new { intSitio = intSitio, lgTurno = lgTurno, lgConducta = lgConducta, intEntidad = intEntidad });
                }
                if (!VerificarUsuario())
                {
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
                ViewBag.Infraccion = "";
                if (!string.IsNullOrEmpty(persona.INFRACCION))
                {
                    ViewBag.Infraccion = (db.CADINFRACCIONES_TRANSITO.Find(persona.INFRACCION).DESCRIPCION);
                }
                ViewBag.ConductaDesc = "";
                if (persona.CONDCODIGO != null)
                {
                    ViewBag.ConductaDesc = (db.CADCONDUCTAS.Find(persona.CONDCODIGO).DESCRIPCION);
                }

                ViewBag.TIPO_ID = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_DOCUMENTO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", persona.TIPO_ID);
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", persona.MODOCONDUCTA);
                ViewBag.PARTCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_PARTICIPACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", persona.PARTCONDUCTA);
                ViewBag.GENERO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_SEXO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", persona.GENERO);
                ViewBag.RELACONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "RELACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", persona.RELACONDUCTA);
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.SitioId = persona.CADCONDTURNSITGR;
                ViewBag.Turno = persona.CADCONDTURNTURNO;
                ViewBag.Coducta = persona.CADCONDTURNCOND;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(persona.CADCONDTURNSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEvento;
                ViewBag.EventoFecha = strFecha;
                ViewBag.InfraccionDesc = strInfraccion;
                ViewBag.CondicionDesc = strCondicion;
                return View("Inicio", persona);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCONDTURNSITGR,CADCONDTURNTURNO,CADCONDTURNCOND,MODOCONDUCTA,RELACONDUCTA,PARTCONDUCTA,TIPO_ID,IDENTIFICACION,APELLIDOS,NOMBRES,GENERO,INFRACCION,CONDCODIGO,CADEVENSITIOGRABA,CADEVENNUMEEVENTO,OBSERVACIONES,FECHA_CONDUCTA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION")] CADCONDTURNOPER cADCONDTURNOPER)
        {
            try
            {
                string strMensaje = "";
                ViewBag.TIPO_ID = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_DOCUMENTO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.TIPO_ID);
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.MODOCONDUCTA);
                ViewBag.PARTCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_PARTICIPACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.PARTCONDUCTA);
                ViewBag.GENERO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_SEXO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.GENERO);
                ViewBag.RELACONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "RELACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.RELACONDUCTA);
                ViewBag.Infraccion = "";
                ViewBag.ConductaDesc = "";
                if (cADCONDTURNOPER.INFRACCION != null)
                {
                    ViewBag.Infraccion = (db.CADINFRACCIONES_TRANSITO.Find(cADCONDTURNOPER.INFRACCION).DESCRIPCION);
                }
                if (cADCONDTURNOPER.INFRACCION != null)
                {
                    ViewBag.ConductaDesc = (db.CADCONDUCTAS.Find(cADCONDTURNOPER.CONDCODIGO).DESCRIPCION);
                }

                ViewBag.SitioId = cADCONDTURNOPER.CADCONDTURNSITGR;
                ViewBag.Turno = cADCONDTURNOPER.CADCONDTURNTURNO;
                ViewBag.Coducta = cADCONDTURNOPER.CADCONDTURNCOND;
                ViewBag.EntidadId = Request.Form["hdEntidadId"];
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOPER.CADCONDTURNSITGR).DESCRIPCION;
                ViewBag.Entidad = Request.Form["hdEntidad"].ToString();
                ViewBag.Evento = Request.Form["hdEvento"].ToString();
                ViewBag.EventoMedio = Request.Form["hdEventoMedio"].ToString();
                ViewBag.EventoFecha = Request.Form["hdFechaEvento"].ToString();
                if (!VerificarUsuario())
                {
                    return RedirectToAction("Index", "Home");
                }
                if (ModelState.IsValid)
                {
                    contexto.Entry(cADCONDTURNOPER).State = EntityState.Modified;
                    contexto.SaveChanges();
                    //Almacena la auditoría
                    auditoria.RegistrarAuditoria<CADCONDTURNOPER>(cADCONDTURNOPER, "CADCONDTURNOPER", "LLAVE COMPUESTA: SITIO:" + cADCONDTURNOPER.SITIO_GRABA.ToString() + " | CONSECUTIVO:" + cADCONDTURNOPER.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADCONDTURNOPER.CONSECUTIVO.ToString());
                    return RedirectToAction("Inicio", "ConductasPersona", new { intSitio = cADCONDTURNOPER.CADCONDTURNSITGR, lgTurno = cADCONDTURNOPER.CADCONDTURNTURNO, lgConducta = cADCONDTURNOPER.CADCONDTURNCOND, intEntidad = Request.Form["hdEntidadId"], strMensaje = strMensaje });
                }

                return View("Inicio", cADCONDTURNOPER);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Crear(int intSitio, long lgTurno, long lgConducta, int intEntidad)
        {
            try
            {
                if (!VerificarUsuario())
                {
                    return RedirectToAction("Index", "Home");
                }
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss"); ;
                }
                ViewBag.TIPO_ID = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_DOCUMENTO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.PARTCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_PARTICIPACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.GENERO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_SEXO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.RELACONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "RELACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCONDTURNSITGR,CADCONDTURNTURNO,CADCONDTURNCOND,MODOCONDUCTA,RELACONDUCTA,PARTCONDUCTA,TIPO_ID,IDENTIFICACION,APELLIDOS,NOMBRES,GENERO,INFRACCION,CONDCODIGO,CADEVENSITIOGRABA,CADEVENNUMEEVENTO,OBSERVACIONES,FECHA_CONDUCTA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION")] CADCONDTURNOPER cADCONDTURNOPER)
        {
            try
            {
                string strNombre = "", strMensaje = "";
                if (!VerificarUsuario())
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.TIPO_ID = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_DOCUMENTO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.TIPO_ID);
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.MODOCONDUCTA);
                ViewBag.PARTCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_PARTICIPACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.PARTCONDUCTA);
                ViewBag.GENERO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_SEXO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.GENERO);
                ViewBag.RELACONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "RELACION_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOPER.RELACONDUCTA);

                ViewBag.SitioId = cADCONDTURNOPER.CADCONDTURNSITGR;
                ViewBag.Turno = cADCONDTURNOPER.CADCONDTURNTURNO;
                ViewBag.Conducta = cADCONDTURNOPER.CADCONDTURNCOND;
                ViewBag.EntidadId = Request.Form["hdEntidadId"];
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOPER.CADCONDTURNSITGR).DESCRIPCION;
                ViewBag.Entidad = Request.Form["hdEntidad"].ToString();
                ViewBag.Evento = Request.Form["hdEvento"].ToString();
                ViewBag.EventoMedio = Request.Form["hdEventoMedio"].ToString();
                ViewBag.EventoFecha = Request.Form["hdFechaEvento"].ToString();
                ViewBag.InfraccionDesc = "";
                ViewBag.CondicionDesc = "";

                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == cADCONDTURNOPER.CADCONDTURNSITGR
                         && registros.TURNO == cADCONDTURNOPER.CADCONDTURNTURNO
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss");
                }

                if (ModelState.IsValid)
                {
                    //Verifica si la máquina ha sido identificada.
                    if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                    strNombre = Session["MAQUINA"].ToString();
                    if (!string.IsNullOrEmpty(strNombre))
                    {
                        cADCONDTURNOPER.MAQUINA_TRANSACCION = strNombre;
                    }
                    cADCONDTURNOPER.USUA_TRANSACCION = usuarioActual.USUARIO;
                    cADCONDTURNOPER.SITIO_GRABA = usuarioActual.SITIO_GRABACION;

                    contexto.CADCONDTURNOPERS.Add(cADCONDTURNOPER);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADCONDTURNOPER.CONSECUTIVO.ToString());
                    return RedirectToAction("Inicio", "ConductasPersona", new { intSitio = cADCONDTURNOPER.CADCONDTURNSITGR, lgTurno = cADCONDTURNOPER.CADCONDTURNTURNO, lgConducta = cADCONDTURNOPER.CADCONDTURNCOND, intEntidad = Request.Form["hdEntidadId"], lgEvento = Request.Form["hdEvento"], lgEventoMedio = Request.Form["hdEventoMedio"], strFecha = Request.Form["hdFechaEvento"], strMensaje = strMensaje });
                }

                return View("Inicio", cADCONDTURNOPER);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite obtener el listado de conductas disponibles para una persona
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConductas()
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
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_conductas", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
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
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerInfracciones()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Infraccion> elementos = new List<Infraccion>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarInfraccion('_CODIGO','_DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
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
                        strQuery = " WHERE (INTERNO.CODIGO = '" + parametros.ConsultaValor + "') ";
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
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_infracciones", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_DESCRIPCION", fila["DESCRIPCION"].ToString());
                    elementos.Add(new Infraccion(fila, strBotonFinal));
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
        [HttpPost]
        public JsonResult ObtenerEvento(int intSitio, long lgEvento, long lgMedio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<EventoConducta> elementos = new List<EventoConducta>();
                GestorGeneral objGestorGral;
                string strBotonFinal = "";
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarEvento(_SITIO,_EVENTO)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_event_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_event_medio", lgMedio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_event_numero", lgEvento.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_eventoconducta", strConexion, "Nonquery");
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_SITIO", fila["cadeven_sitiograba"].ToString()).Replace("_EVENTO", fila["cadeven_nume_evento"].ToString());
                    elementos.Add(new EventoConducta(fila, strBotonFinal));
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

        /// <summary>
        ///Permite verificar la existencia en sesión del usuario de secad
        /// </summary>
        /// <returns></returns>
        private bool VerificarUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return false;
                }
                usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerMediosPersona(int intSitio, long lgConsecutivo, long lgConducta, long lgTurno, int intEntidad, string strModo)
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
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_conducta", lgConducta.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_modo", strModo, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_medios_persona", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton.Replace("_EDITAR", "AbrirEdicion('" + Url.Action("Editar", "MediosPersona", new { intSitio = intSitio, lgPerConsec = lgConsecutivo, lgMedioConsec = fila["CADMEDISPCONSEC"].ToString(), strCeduEmpleado = fila["CEDU_EMPLEADO"].ToString(), lgTurno = lgTurno, intEntidad = intEntidad, strPatrulla = fila["DESC_PATRULLA"].ToString(), strDesc = fila["DESCRIPCION"].ToString(), strPersona = fila["EMPLEADO"].ToString() }) + "')");
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarMedio(" + intSitio + "," + lgConsecutivo + "," + fila["CADMEDISPCONSEC"].ToString() + ",'" + fila["CEDU_EMPLEADO"].ToString() + "'," + lgTurno + "," + lgConducta + ",'" + strModo + "','PERSONA')");

                    elementos.Add(new ExtCadCondMedio(fila, strBotonFinal));
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
        public JsonResult EliminarPersona(int intSitio, long lgConsecutivo)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validador.VerificarUsuario();
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
                CADCONDTURNOPER persona = (from registro in contexto.CADCONDTURNOPERS
                                           where registro.SITIO_GRABA == intSitio
                                           && registro.CONSECUTIVO == lgConsecutivo
                                           select registro).FirstOrDefault();
                //varifica que el registro no tenga datos asociados
                if (persona != null)
                {
                    if (validador.ValidarAsociadosPersona(persona))
                    {
                        contexto.CADCONDTURNOPERS.Remove(persona);
                        contexto.SaveChanges();
                        //Almacena la auditoría


                        CADCONDTURNOPER persona_copy = new CADCONDTURNOPER();
                        persona_copy = (CADCONDTURNOPER)auditoria.GenerarCopiaAuditoria(persona);

                        auditoria.RegistrarAuditoria<CADCONDTURNOPER>(persona_copy, "CADCONDTURNOPER", "LLAVE COMPUESTA: SITIO:" + persona.SITIO_GRABA.ToString() + " | CONSECUTIVO:" + persona.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);
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