using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers.Minuta
{
    public class MediosBienesController : Controller
    {
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private Validador validar = new Validador();
        private CADUSUARIO usuarioActual;
        private CADCONDTURNOBIENE bien;
        private GestorGeneral gestor = new GestorGeneral();
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: MediosPersona/Create
        [Authorize]
        public ActionResult Crear(int intSitio, long lgTurno, int intEntidad, long lgConsecutivo, long? lgEventoMedio = null, string strMensaje = null)
        {
            try
            {
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("InicioEmbebido", "Home");
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
                bien = (from registro in contexto.CADCONDTURNOBIENES
                        where registro.CONSECUTIVO == lgConsecutivo
                           && registro.SITIO_GRABA == intSitio
                        select registro).FirstOrDefault();
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.FechaConducta = "";
                if (bien.FECHA_CONDUCTA != null)
                {
                    ViewBag.FechaConducta = ((DateTime)bien.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = bien.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.Maquina = Session["MAQUINA"];
                return View("Crear", "MapaBase");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        // POST: MediosPersona/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "CADBIENSITGR,CADBIENCONSEC,CADMEDISPSITGR,CADMEDISPCONSEC,CEDU_EMPLEADO,FECHA_CONDUCTA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABA,TIPO_ACCION")] CADCONDTURNOBIENMEDISP cADCONDTURNOBIENMEDISP)
        {
            try
            {
                string strMensaje = "";
                var bien = (from registro in contexto.CADCONDTURNOBIENES
                            where registro.CONSECUTIVO == cADCONDTURNOBIENMEDISP.CADBIENCONSEC
                            && registro.SITIO_GRABA == cADCONDTURNOBIENMEDISP.CADBIENSITGR
                            select new { registro.CADCONDTURNCOND, registro.CADCONDTURNTURNO, registro.FECHA_CONDUCTA, registro.CONSECUTIVO }).FirstOrDefault();
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                if (ModelState.IsValid)
                {
                    var registroExistente = (
                        from registro in contexto.CADCONDTURNOBIENMEDISPs
                        where registro.CADBIENCONSEC == cADCONDTURNOBIENMEDISP.CADBIENCONSEC
                        && registro.CADBIENSITGR == cADCONDTURNOBIENMEDISP.CADBIENSITGR
                        && registro.CADMEDISPCONSEC == cADCONDTURNOBIENMEDISP.CADMEDISPCONSEC
                        && registro.CADMEDISPSITGR == cADCONDTURNOBIENMEDISP.CADMEDISPSITGR
                        && registro.CEDU_EMPLEADO == cADCONDTURNOBIENMEDISP.CEDU_EMPLEADO
                        select registro
                        ).FirstOrDefault();

                    if (registroExistente != null)
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                        return RedirectToAction("Crear", "MediosBienes", new { intSitio = cADCONDTURNOBIENMEDISP.CADMEDISPSITGR, lgTurno = bien.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOBIENMEDISP.CADBIENCONSEC });
                    }
                    contexto.CADCONDTURNOBIENMEDISPs.Add(cADCONDTURNOBIENMEDISP);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Registro Nuevo");
                    return RedirectToAction("Crear", "MediosBienes", new { intSitio = cADCONDTURNOBIENMEDISP.CADMEDISPSITGR, lgTurno = bien.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOBIENMEDISP.CADBIENCONSEC, strMensaje = strMensaje });
                }

                ViewBag.CADPERSITGR = new SelectList(contexto.CADCONDTURNOPERS, "SITIO_GRABA", "MODOCONDUCTA", cADCONDTURNOBIENMEDISP.CADBIENSITGR);
                ViewBag.SitioId = cADCONDTURNOBIENMEDISP.CADMEDISPSITGR;
                ViewBag.Turno = bien.CADCONDTURNTURNO;
                ViewBag.EntidadId = Request.Form["hdEntidad"].ToString();
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOBIENMEDISP.CADBIENSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(Convert.ToInt32(Request.Form["hdEntidad"].ToString())).DESCRIPCION;
                ViewBag.EventoMedio = (Request.Form["hdEventoMedio"] == null) ? "0" : Request.Form["hdEventoMedio"];
                ViewBag.FechaConducta = "";
                if (bien != null)
                {
                    ViewBag.FechaConducta = ((DateTime)bien.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = bien.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.Maquina = Session["MAQUINA"];
                return View("Editar", "MapaBase", cADCONDTURNOBIENMEDISP);
            }
            catch (DbEntityValidationException exE)
            {
                string strComplemento = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var name = ve.PropertyName;
                        var mensaje = ve.ErrorMessage;

                        strComplemento += mensaje;
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name, strComplemento);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
            catch (Exception ex)
            {
                string strComplemento = "";
                strComplemento = ex.Message + " --fecha:" + ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd / MM / yyyy HH: mm: ss");
                GestorBase.GestionarExcepcion(ex, GetType().Name, strComplemento);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [Authorize]
        public ActionResult Editar(long? lgEventoMedio = null, string strMensaje = null)
        {
            try
            {
                CADCONDTURNOBIENMEDISP medio;
                long lgBienConsec = Convert.ToInt64(Request.Params["lgBienConsec"]);
                int intSitio = Convert.ToInt32(Request.Params["intSitio"]);
                long lgMedioConsec = Convert.ToInt64(Request.Params["lgMedioConsec"]);
                string strCeduEmpleado = Request.Params["strCeduEmpleado"];
                long lgTurno = Convert.ToInt64(Request.Params["lgTurno"]);
                int intEntidad = Convert.ToInt32(Request.Params["intEntidad"]);
                string strPatrulla = Request.Params["strPatrulla"], strDesc = Request.Params["strDesc"], strPersona = Request.Params["strPersona"];
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("InicioEmbebido", "Home");
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
                bien = (from registro in contexto.CADCONDTURNOBIENES
                        where registro.CONSECUTIVO == lgBienConsec
                           && registro.SITIO_GRABA == intSitio
                        select registro).FirstOrDefault();
                medio = (from registro in contexto.CADCONDTURNOBIENMEDISPs
                         where registro.CADBIENCONSEC == lgBienConsec
                         && registro.CADMEDISPSITGR == intSitio
                         && registro.CADMEDISPCONSEC == lgMedioConsec
                         && registro.CEDU_EMPLEADO == strCeduEmpleado
                         && registro.CADBIENSITGR == intSitio
                         select registro).FirstOrDefault();
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.FechaConducta = "";
                if (bien.FECHA_CONDUCTA != null)
                {
                    ViewBag.FechaConducta = ((DateTime)bien.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = bien.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.Maquina = Session["MAQUINA"];
                ViewBag.Patrulla = strPatrulla;
                ViewBag.Desc = strDesc;
                ViewBag.Persona = strPersona;
                return View("Editar", "MapaBase", medio);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "CADBIENSITGR,CADBIENCONSEC,CADMEDISPSITGR,CADMEDISPCONSEC,CEDU_EMPLEADO,FECHA_CONDUCTA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABA,TIPO_ACCION")] CADCONDTURNOBIENMEDISP cADCONDTURNOBIENMEDISP)
        {
            try
            {
                string strMensaje = "";
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                var bien = (from registro in contexto.CADCONDTURNOBIENES
                            where registro.CONSECUTIVO == cADCONDTURNOBIENMEDISP.CADBIENCONSEC
                            && registro.SITIO_GRABA == cADCONDTURNOBIENMEDISP.CADBIENSITGR
                            select new { registro.CADCONDTURNCOND, registro.CADCONDTURNTURNO, registro.FECHA_CONDUCTA, registro.CONSECUTIVO }).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    contexto.Entry(cADCONDTURNOBIENMEDISP).State = EntityState.Modified;
                    contexto.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCONDTURNOBIENMEDISP>(cADCONDTURNOBIENMEDISP, "CADCONDTURNOBIENMEDISP", "CADBIENSITGR=" + cADCONDTURNOBIENMEDISP.CADBIENSITGR.ToString() + ",CADBIENCONSEC=" + cADCONDTURNOBIENMEDISP.CADBIENCONSEC
                   + ",CADMEDISPSITGR=" + cADCONDTURNOBIENMEDISP.CADMEDISPSITGR + ",CADMEDISPCONSEC=" + cADCONDTURNOBIENMEDISP.CADMEDISPCONSEC, TipoAuditoria.Modificar);




                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Registro Seleccionado");
                    return RedirectToAction("Editar", "MediosBienes", new { strCeduEmpleado = cADCONDTURNOBIENMEDISP.CEDU_EMPLEADO, lgMedioConsec = cADCONDTURNOBIENMEDISP.CADMEDISPCONSEC, intSitio = cADCONDTURNOBIENMEDISP.CADMEDISPSITGR, lgTurno = bien.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgBienConsec = cADCONDTURNOBIENMEDISP.CADBIENCONSEC, strMensaje = strMensaje, strPatrulla = Request.Form["txtPatrulla"], strDesc = Request.Form["txtDescripcion"], strPersona = Request.Params["txtEmpleado"] });
                }

                ViewBag.CADPERSITGR = new SelectList(contexto.CADCONDTURNOPERS, "SITIO_GRABA", "MODOCONDUCTA", cADCONDTURNOBIENMEDISP.CADBIENSITGR);
                ViewBag.SitioId = cADCONDTURNOBIENMEDISP.CADMEDISPSITGR;
                ViewBag.Turno = bien.CADCONDTURNTURNO;
                ViewBag.EntidadId = Request.Form["hdEntidad"].ToString();
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOBIENMEDISP.CADBIENSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(Convert.ToInt32(Request.Form["hdEntidad"].ToString())).DESCRIPCION;
                ViewBag.EventoMedio = (Request.Form["hdEventoMedio"] == null) ? "0" : Request.Form["hdEventoMedio"];
                ViewBag.FechaConducta = "";
                if (bien != null)
                {
                    ViewBag.FechaConducta = ((DateTime)bien.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = bien.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.Maquina = Session["MAQUINA"];
                ViewBag.Patrulla = Request.Form["txtPatrulla"];
                ViewBag.Desc = Request.Form["txtDescripcion"];
                ViewBag.Persona = Request.Params["txtEmpleado"];
                return View("Editar", "MapaBase", cADCONDTURNOBIENMEDISP);
            }
            catch (DbEntityValidationException exE)
            {
                string strComplemento = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var name = ve.PropertyName;
                        var mensaje = ve.ErrorMessage;

                        strComplemento += mensaje;
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name, strComplemento);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
            catch (Exception ex)
            {
                string strComplemento = "";
                strComplemento = ex.Message + " --fecha:" + ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd / MM / yyyy HH: mm: ss");
                GestorBase.GestionarExcepcion(ex, GetType().Name, strComplemento);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult EliminarMedio(int intSitio, long lgBienCon, long lgMedCon, string strCedula)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                string strMensaje = "";
                CADCONDTURNOBIENMEDISP medio;
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
                medio = (from registro in contexto.CADCONDTURNOBIENMEDISPs
                         where registro.CADMEDISPCONSEC == lgMedCon
                         && registro.CADMEDISPSITGR == intSitio
                         && registro.CADBIENCONSEC == lgBienCon
                         && registro.CEDU_EMPLEADO == strCedula
                         && registro.CADBIENSITGR == intSitio
                         select registro).FirstOrDefault();
                if (medio != null)
                {
                    contexto.CADCONDTURNOBIENMEDISPs.Remove(medio);
                    contexto.SaveChanges();
                    //Almacena la auditoría


                    CADCONDTURNOBIENMEDISP medio_copy = new CADCONDTURNOBIENMEDISP();
                    medio_copy = (CADCONDTURNOBIENMEDISP)auditoria.GenerarCopiaAuditoria(medio);

                    auditoria.RegistrarAuditoria<CADCONDTURNOBIENMEDISP>(medio_copy, "CADCONDTURNOBIENMEDISP", "CADBIENSITGR=" + medio.CADBIENSITGR.ToString() + ",CADBIENCONSEC=" + medio.CADBIENCONSEC
                    + ",CADMEDISPSITGR=" + medio.CADMEDISPSITGR + ",CADMEDISPCONSEC=" + medio.CADMEDISPCONSEC, TipoAuditoria.Eliminar);


                    //  auditoria.RegistrarAuditoria<CADCONDTURNOPER>(persona, "CADCONDTURNOPER", "LLAVE COMPUESTA: SITIO:" + persona.SITIO_GRABA.ToString() + " | CONSECUTIVO:" + persona.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                    resultado = Json(new { result = true, datos = strMensaje });
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