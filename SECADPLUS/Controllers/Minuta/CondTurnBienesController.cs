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
    public class CondTurnBienesController : Controller
    {
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private CADUSUARIO usuarioActual;
        private Validador validar = new Validador();
        private CADTURNO turno;
        private GestorAuditoria auditoria = new GestorAuditoria();


        [Authorize]
        public ActionResult Inicio(int intSitio, long lgTurno, long lgConducta, int intEntidad, long? lgEvento = null, long? lgEventoMedio = null, string strFecha = null, string strMensaje = null)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
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
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.EntidadId = intEntidad;
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss"); ;
                }
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { c.CODIGO, c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.EventoFecha = strFecha;






                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new
                {
                    intSitio = intSitio,
                    lgTurno = lgTurno,
                    lgConducta = lgConducta,
                    intEntidad = intEntidad,
                    lgEvento = lgEvento,
                    lgEventoMedio = lgEventoMedio

                });
                ViewBag.NombreUbicacion = "Turnos-Bienes";
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
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == intSitio
                         && registros.TURNO == lgTurno
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss"); ;
                }
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { c.CODIGO, c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
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

        [Authorize]
        public ActionResult Editar(int intSitio, long lgTurno, int intEntidad, long lgConducta, long lgConsecutivo, long? lgEvento = null, long? lgEventoMedio = null, string strFecha = null)
        {
            try
            {
                CADCONDTURNOBIENE bien = (from registro in contexto.CADCONDTURNOBIENES
                                          where registro.SITIO_GRABA == intSitio
                                          && registro.CONSECUTIVO == lgConsecutivo
                                          select registro).FirstOrDefault();
                if (bien == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    return RedirectToAction("Inicio", "CondTurnBienes", new { intSitio = intSitio, lgTurno = lgTurno, lgConducta = lgConducta, intEntidad = intEntidad });
                }
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
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
                ViewBag.Infraccion = "";
                if (!string.IsNullOrEmpty(bien.CADINFR_CODIGO))
                {
                    ViewBag.Infraccion = (db.CADINFRACCIONES_TRANSITO.Find(bien.CADINFR_CODIGO).DESCRIPCION);
                }
                ViewBag.Parqueadero = null;
                if (!string.IsNullOrEmpty(bien.CADPARQ_CODIGO))
                {
                    ViewBag.Parqueadero = ((from registro in db.CADPARQUEADEROS
                                            where registro.SITIO_GRABA == bien.CADPARQ_SITGR
                                            && registro.CODIGO == bien.CADPARQ_CODIGO
                                            select new { registro.DESCRIPCION }).FirstOrDefault());
                }

                // ViewBag.Parqueadero = (db.CADPARQUEADEROS.Find(c=>c.).DESCRIPCION);
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { c.CODIGO, c.DESCRIPCION }), "CODIGO", "DESCRIPCION", bien.CADCLBI_TIBI_CODIGO);
                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Select(c => new { c.CODIGO, c.DESCRIPCION }), "CODIGO", "DESCRIPCION", bien.CADCLBI_CODIGO);
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                if (bien.COLO_CODIGO != null)
                {
                    ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", bien.COLO_CODIGO);
                }
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                if (bien.COLO_CODIGO != null)
                {
                    ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", bien.MODOCONDUCTA);
                }

                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.Conducta = lgConducta;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.Evento = (lgEvento == null) ? 0 : lgEvento;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEvento;
                ViewBag.EventoFecha = strFecha;
                return View("Inicio", bien);
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
        public ActionResult Crear([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCONDTURNSITGR,CADCONDTURNTURNO,CADCONDTURNCOND,MODOCONDUCTA,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,MARCA_CODIGO,LINEA_CODIGO,COLO_CODIGO,PLACA,CHASIS,MOTOR,SERIE,CANTIDAD,AVALUO,CADPARQ_SITGR,CADPARQ_CODIGO,CADINFR_CODIGO,CADEVENSITIOGRABA,CADEVENNUMEEVENTO,OBSERVACIONES,FECHA_CONDUCTA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION,CADUNID_MEDIDA")] CADCONDTURNOBIENE cADCONDTURNOBIENE)
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
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { c.CODIGO, c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCONDTURNOBIENE.CADCLBI_TIBI_CODIGO);
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOBIENE.COLO_CODIGO);
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOBIENE.MODOCONDUCTA);
                ViewBag.SitioId = cADCONDTURNOBIENE.SITIO_GRABA;
                ViewBag.Turno = cADCONDTURNOBIENE.CADCONDTURNTURNO;
                ViewBag.Conducta = cADCONDTURNOBIENE.CADCONDTURNCOND;
                ViewBag.Entidad = Request.Form["hdEntidad"].ToString();
                ViewBag.EntidadId = Request.Form["hdEntidadId"].ToString();
                turno = (from registros in contexto.CADTURNOS
                         where registros.SITIO_GRABA == cADCONDTURNOBIENE.SITIO_GRABA
                         && registros.TURNO == cADCONDTURNOBIENE.CADCONDTURNTURNO
                         select registros).FirstOrDefault();
                ViewBag.FechaTurno = "";
                if (turno != null)
                {
                    ViewBag.FechaTurno = turno.HORA_INICIA.ToString("dd/MM/yyyy HH:mm:ss"); ;
                }
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOBIENE.SITIO_GRABA).DESCRIPCION;

                ViewBag.Evento = Request.Form["hdEvento"].ToString();
                ViewBag.EventoMedio = Request.Form["hdEventoMedio"].ToString();
                ViewBag.EventoFecha = Request.Form["hdFechaEvento"].ToString();
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
                        cADCONDTURNOBIENE.MAQUINA_TRANSACCION = strNombre;
                    }
                    cADCONDTURNOBIENE.USUA_TRANSACCION = usuarioActual.USUARIO;
                    cADCONDTURNOBIENE.SITIO_GRABA = usuarioActual.SITIO_GRABACION;
                    contexto.CADCONDTURNOBIENES.Add(cADCONDTURNOBIENE);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADCONDTURNOBIENE.CONSECUTIVO.ToString());
                    return RedirectToAction("Inicio", "CondTurnBienes", new { intSitio = cADCONDTURNOBIENE.SITIO_GRABA, lgTurno = cADCONDTURNOBIENE.CADCONDTURNTURNO, lgConducta = cADCONDTURNOBIENE.CADCONDTURNCOND, intEntidad = Request.Form["hdEntidadId"].ToString(), lgEvento = Request.Form["hdEvento"], lgEventoMedio = Request.Form["hdEventoMedio"], strFecha = Request.Form["hdFechaEvento"], strMensaje = strMensaje });
                }
                return View("Inicio", cADCONDTURNOBIENE);
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
                throw;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCONDTURNSITGR,CADCONDTURNTURNO,CADCONDTURNCOND,MODOCONDUCTA,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,MARCA_CODIGO,LINEA_CODIGO,COLO_CODIGO,PLACA,CHASIS,MOTOR,SERIE,CANTIDAD,AVALUO,CADPARQ_SITGR,CADPARQ_CODIGO,CADINFR_CODIGO,CADEVENSITIOGRABA,CADEVENNUMEEVENTO,OBSERVACIONES,FECHA_CONDUCTA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION,CADUNID_MEDIDA")] CADCONDTURNOBIENE cADCONDTURNOBIENE)
        {
            try
            {
                string strNombre = "", strMensaje = "";
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
                strNombre = Session["MAQUINA"].ToString();
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { c.CODIGO, c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCONDTURNOBIENE.CADCLBI_TIBI_CODIGO);
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOBIENE.COLO_CODIGO);
                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDTURNOBIENE.MODOCONDUCTA);
                ViewBag.SitioId = cADCONDTURNOBIENE.SITIO_GRABA;
                ViewBag.Turno = cADCONDTURNOBIENE.CADCONDTURNTURNO;
                ViewBag.Conducta = cADCONDTURNOBIENE.CADCONDTURNCOND;
                ViewBag.Entidad = Request.Form["hdEntidad"].ToString();
                ViewBag.Evento = Request.Form["hdEvento"].ToString();
                ViewBag.EventoMedio = Request.Form["hdEventoMedio"].ToString();
                ViewBag.EventoFecha = Request.Form["hdFechaEvento"].ToString();
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(strNombre))
                    {
                        cADCONDTURNOBIENE.MAQUINA_TRANSACCION = strNombre;
                    }
                    cADCONDTURNOBIENE.USUA_TRANSACCION = usuarioActual.USUARIO;
                    cADCONDTURNOBIENE.SITIO_GRABA = usuarioActual.SITIO_GRABACION;
                    contexto.Entry(cADCONDTURNOBIENE).State = EntityState.Modified;
                    contexto.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCONDTURNOBIENE>(cADCONDTURNOBIENE, "CADCONDTURNOBIENE", "SITIO_GRABA=" + cADCONDTURNOBIENE.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + cADCONDTURNOBIENE.CONSECUTIVO
                         , TipoAuditoria.Eliminar);

                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADCONDTURNOBIENE.CONSECUTIVO.ToString());
                    return RedirectToAction("Inicio", "CondTurnBienes", new { intSitio = cADCONDTURNOBIENE.SITIO_GRABA, lgTurno = cADCONDTURNOBIENE.CADCONDTURNTURNO, lgConducta = cADCONDTURNOBIENE.CADCONDTURNCOND, intEntidad = Request.Form["hdEntidadId"].ToString(), lgEvento = Request.Form["hdEvento"], lgEventoMedio = Request.Form["hdEventoMedio"], strFecha = Request.Form["hdFechaEvento"], strMensaje = strMensaje });
                }
                return View(cADCONDTURNOBIENE);
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
                throw;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConductaBienes(int intSitio, long lgTurno, long lgConducta, int intEntidad)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadCondBiene> elementos = new List<ExtCadCondBiene>();
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
                        strQuery = " WHERE (INTERNO.CONSECUTIVO = " + parametros.ConsultaValor + " ) ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.TIPO, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.CLASE,'" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.PARQUEADERO,'" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.MARCA,'" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.LINEA,'" + parametros.ConsultaValor + "', 'i') "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.TIPOCODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
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
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_conductas_bienes", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_MEDIOS", "ConsultarMediosBienes(" + fila["SITIO_GRABA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + "," + fila["CADCONDTURNCOND"].ToString() + ",'" + fila["MODOID"].ToString() + "')");
                    strBotonFinal = strBotonFinal.Replace("_EDITAR", Url.Action("Editar", "CondTurnBienes", new { intSitio = fila["SITIO_GRABA"].ToString(), lgTurno = lgTurno, intEntidad = intEntidad, lgConducta = lgConducta, lgConsecutivo = fila["CONSECUTIVO"].ToString() }));
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarBien(" + fila["SITIO_GRABA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + ",'" + fila["TIPO"].ToString() + "')");
                    elementos.Add(new ExtCadCondBiene(fila, strBotonFinal));
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
        public JsonResult EliminarBien(int intSitio, long lgBienCon)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                string strMensaje = "";
                CADCONDTURNOBIENE bien;
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
                bien = (from registro in contexto.CADCONDTURNOBIENES
                        where registro.CONSECUTIVO == lgBienCon
                        && registro.SITIO_GRABA == intSitio
                        select registro).FirstOrDefault();
                if (bien != null)
                {
                    if (validar.ValidarAsociadosBien(bien))
                    {
                        contexto.CADCONDTURNOBIENES.Remove(bien);
                        contexto.SaveChanges();
                        //Almacena la auditoría

                        CADCONDTURNOPER bien_copy = new CADCONDTURNOPER();
                        bien_copy = (CADCONDTURNOPER)auditoria.GenerarCopiaAuditoria(bien);

                        auditoria.RegistrarAuditoria<CADCONDTURNOPER>(bien_copy, "CADCONDTURNOPER", "LLAVE COMPUESTA: SITIO:" + bien.SITIO_GRABA.ToString() + " | CONSECUTIVO:" + bien.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);
                        strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                        resultado = Json(new { result = true, datos = strMensaje });
                    }
                    else
                    {
                        strMensaje = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Bien seleccionada");
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

        [Authorize]
        [HttpPost]
        public JsonResult ListaDatos(string strOpcion, int intValor)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                switch (strOpcion)
                {
                    case "CLASES":
                        var clases = db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == intValor).Select(c => new { c.CODIGO, c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION).ToList();
                        resultados = Json(new { result = true, datos = clases });
                        break;

                    case "MARCAS":
                        var marcas = db.CADMARCAS.Where(c => c.TIBI_CODIGO == intValor).Select(c => new { c.CODIGO, c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION).ToList();
                        resultados = Json(new { result = true, datos = marcas });
                        break;

                    case "LINEAS":
                        var lineas = db.CADLINEAS_MARCA.Where(c => c.MARC_CODIGO == intValor).Select(c => new { c.CODIGO, c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION).ToList();
                        resultados = Json(new { result = true, datos = lineas });
                        break;
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
        public JsonResult ObtenerParqueaderos(int intSitio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_V_Parqueaderos> elementos = new List<Ext_V_Parqueaderos>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
               "onclick = \"SeleccionarParqueaderos(_CONSEC,'_DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
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
                        strQuery = " WHERE (INTERNO.CODIGO = '" + parametros.ConsultaValor + "' ) ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
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
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_condparqueaderos", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CONSEC", fila["CODIGO"].ToString()).Replace("_DESCRIPCION", fila["DESCRIPCION"].ToString());
                    elementos.Add(new Ext_V_Parqueaderos(fila, strBotonFinal, true));
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
        public JsonResult ObtenerMediosBienes(int intSitio, long lgConsecutivo, long lgConducta, long lgTurno, int intEntidad, string strModo)
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
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_medios_bienes", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton.Replace("_EDITAR", "AbrirEdicion('" + Url.Action("Editar", "MediosBienes", new { intSitio = intSitio, lgBienConsec = lgConsecutivo, lgMedioConsec = fila["CADMEDISPCONSEC"].ToString(), strCeduEmpleado = fila["CEDU_EMPLEADO"].ToString(), lgTurno = lgTurno, intEntidad = intEntidad, strPatrulla = fila["DESC_PATRULLA"].ToString(), strDesc = fila["DESCRIPCION"].ToString(), strPersona = fila["EMPLEADO"].ToString() }) + "')");
                    strBotonFinal = strBotonFinal.Replace("_ELIMINAR", "EliminarMedio(" + intSitio + "," + lgConsecutivo + "," + fila["CADMEDISPCONSEC"].ToString() + ",'" + fila["CEDU_EMPLEADO"].ToString() + "'," + lgTurno + "," + lgConducta + ",'" + strModo + "','BIEN')");
                    elementos.Add(new ExtCadCondMedio(fila, strBotonFinal, "BIENES"));
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
    }
}