using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers.Minuta
{
    public class MediosComparendoController : Controller
    {
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private Validador validar = new Validador();
        private CADUSUARIO usuarioActual;
        private CADCONDTURNOCOMP comparendo;
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
                comparendo = (from registro in contexto.CADCONDTURNOCOMPs
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
                if (comparendo.FECHA_CONDUCTA != null)
                {
                    ViewBag.FechaConducta = ((DateTime)comparendo.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = comparendo.CONSECUTIVO;
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
        public ActionResult Crear([Bind(Include = "CADCOMPSITGR,CADCOMPCONSEC,CADMEDISPSITGR,CADMEDISPCONSEC,CEDU_EMPLEADO,FECHA_CONDUCTA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABA,TIPO_ACCION")] CADCONDTURNOCOMPMEDISP cADCONDTURNOCOMPMEDISP)
        {
            try
            {
                string strMensaje = "";
                var comparendo = (from registro in contexto.CADCONDTURNOCOMPs
                                  where registro.CONSECUTIVO == cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC
                                  && registro.SITIO_GRABA == cADCONDTURNOCOMPMEDISP.CADCOMPSITGR
                                  select new { registro.CADCONDTURNCOND, registro.CADCONDTURNTURNO, registro.FECHA_CONDUCTA, registro.CONSECUTIVO }).FirstOrDefault();
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                if (ModelState.IsValid)
                {
                    var registroExistente = (
                        from registro in contexto.CADCONDTURNOCOMPMEDISPs
                        where registro.CADCOMPCONSEC == cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC
                        && registro.CADCOMPSITGR == cADCONDTURNOCOMPMEDISP.CADCOMPSITGR
                        && registro.CADMEDISPCONSEC == cADCONDTURNOCOMPMEDISP.CADMEDISPCONSEC
                        && registro.CADMEDISPSITGR == cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR
                        && registro.CEDU_EMPLEADO == cADCONDTURNOCOMPMEDISP.CEDU_EMPLEADO
                        select registro
                        ).FirstOrDefault();
                    if (registroExistente != null)
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                        return RedirectToAction("Crear", "MediosComparendo", new { intSitio = cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR, lgTurno = comparendo.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC });
                    }
                    contexto.CADCONDTURNOCOMPMEDISPs.Add(cADCONDTURNOCOMPMEDISP);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Registro Nuevo");
                    return RedirectToAction("Crear", "MediosComparendo", new { intSitio = cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR, lgTurno = comparendo.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC, strMensaje = strMensaje });
                }
                ViewBag.CADPERSITGR = new SelectList(contexto.CADCONDTURNOPERS, "SITIO_GRABA", "MODOCONDUCTA", cADCONDTURNOCOMPMEDISP.CADCOMPSITGR);
                ViewBag.SitioId = cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR;
                ViewBag.Turno = comparendo.CADCONDTURNTURNO;
                ViewBag.EntidadId = Request.Form["hdEntidad"].ToString();
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOCOMPMEDISP.CADCOMPSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(Convert.ToInt32(Request.Form["hdEntidad"].ToString())).DESCRIPCION;
                ViewBag.EventoMedio = (Request.Form["hdEventoMedio"] == null) ? "0" : Request.Form["hdEventoMedio"];
                ViewBag.FechaConducta = "";
                if (comparendo != null)
                {
                    ViewBag.FechaConducta = ((DateTime)comparendo.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = comparendo.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = gestor.ObtenerTiempoServidor;
                ViewBag.Maquina = Session["MAQUINA"];
                return View("Crear", "MapaBase", cADCONDTURNOCOMPMEDISP);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [Authorize]
        public ActionResult Editar(long? lgEventoMedio = null, string strMensaje = null)
        {
            try
            {
                CADCONDTURNOCOMPMEDISP medio;
                long lgCompConsec = Convert.ToInt64(Request.Params["lgCompConsec"]);
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
                comparendo = (from registro in contexto.CADCONDTURNOCOMPs
                              where registro.CONSECUTIVO == lgCompConsec
                                 && registro.SITIO_GRABA == intSitio
                              select registro).FirstOrDefault();
                medio = (from registro in contexto.CADCONDTURNOCOMPMEDISPs
                         where registro.CADCOMPCONSEC == lgCompConsec
                         && registro.CADMEDISPSITGR == intSitio
                         && registro.CADMEDISPCONSEC == lgMedioConsec
                         && registro.CEDU_EMPLEADO == strCeduEmpleado
                         && registro.CADCOMPSITGR == intSitio
                         select registro).FirstOrDefault();
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.FechaConducta = "";
                if (comparendo.FECHA_CONDUCTA != null)
                {
                    ViewBag.FechaConducta = ((DateTime)comparendo.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = comparendo.CONSECUTIVO;
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
        public ActionResult Editar([Bind(Include = "CADCOMPSITGR,CADCOMPCONSEC,CADMEDISPSITGR,CADMEDISPCONSEC,CEDU_EMPLEADO,FECHA_CONDUCTA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABA,TIPO_ACCION")] CADCONDTURNOCOMPMEDISP cADCONDTURNOCOMPMEDISP)
        {
            try
            {
                string strMensaje = "";
                var comparendo = (from registro in contexto.CADCONDTURNOCOMPs
                                  where registro.CONSECUTIVO == cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC
                                  && registro.SITIO_GRABA == cADCONDTURNOCOMPMEDISP.CADCOMPSITGR
                                  select new { registro.CADCONDTURNCOND, registro.CADCONDTURNTURNO, registro.FECHA_CONDUCTA, registro.CONSECUTIVO }).FirstOrDefault();
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                if (ModelState.IsValid)
                {
                    contexto.Entry(cADCONDTURNOCOMPMEDISP).State = EntityState.Modified;
                    contexto.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCONDTURNOCOMPMEDISP>(cADCONDTURNOCOMPMEDISP, "CADCONDTURNOCOMPMEDISP", "CADCOMPSITGR=" + cADCONDTURNOCOMPMEDISP.CADCOMPSITGR.ToString() + ",CADCOMPCONSEC=" + cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC
                  + ",CADMEDISPSITGR=" + cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR + ",CADMEDISPCONSEC=" + cADCONDTURNOCOMPMEDISP.CADMEDISPCONSEC, TipoAuditoria.Modificar);



                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Registro Nuevo");
                    return RedirectToAction("Editar", "MediosComparendo", new { strCeduEmpleado = cADCONDTURNOCOMPMEDISP.CEDU_EMPLEADO, lgMedioConsec = cADCONDTURNOCOMPMEDISP.CADMEDISPCONSEC, intSitio = cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR, lgTurno = comparendo.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgCompConsec = cADCONDTURNOCOMPMEDISP.CADCOMPCONSEC, strMensaje = strMensaje, strPatrulla = Request.Form["txtPatrulla"], strDesc = Request.Form["txtDescripcion"], strPersona = Request.Params["txtEmpleado"] });
                }
                ViewBag.CADPERSITGR = new SelectList(contexto.CADCONDTURNOPERS, "SITIO_GRABA", "MODOCONDUCTA", cADCONDTURNOCOMPMEDISP.CADCOMPSITGR);
                ViewBag.SitioId = cADCONDTURNOCOMPMEDISP.CADMEDISPSITGR;
                ViewBag.Turno = comparendo.CADCONDTURNTURNO;
                ViewBag.EntidadId = Request.Form["hdEntidad"].ToString();
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOCOMPMEDISP.CADCOMPSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(Convert.ToInt32(Request.Form["hdEntidad"].ToString())).DESCRIPCION;
                ViewBag.EventoMedio = (Request.Form["hdEventoMedio"] == null) ? "0" : Request.Form["hdEventoMedio"];
                ViewBag.FechaConducta = "";
                if (comparendo != null)
                {
                    ViewBag.FechaConducta = ((DateTime)comparendo.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = comparendo.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = gestor.ObtenerTiempoServidor;
                ViewBag.Maquina = Session["MAQUINA"];
                ViewBag.Patrulla = Request.Form["txtPatrulla"];
                ViewBag.Desc = Request.Form["txtDescripcion"];
                ViewBag.Persona = Request.Params["txtEmpleado"];
                return View("Editar", "MapaBase", cADCONDTURNOCOMPMEDISP);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult EliminarMedio(int intSitio, long lgCompCon, long lgMedCon, string strCedula)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                string strMensaje = "";
                CADCONDTURNOCOMPMEDISP medio;
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
                medio = (from registro in contexto.CADCONDTURNOCOMPMEDISPs
                         where registro.CADMEDISPCONSEC == lgMedCon
                         && registro.CADMEDISPSITGR == intSitio
                         && registro.CADCOMPCONSEC == lgCompCon
                         && registro.CEDU_EMPLEADO == strCedula
                         && registro.CADCOMPSITGR == intSitio
                         select registro).FirstOrDefault();
                if (medio != null)
                {
                    contexto.CADCONDTURNOCOMPMEDISPs.Remove(medio);
                    contexto.SaveChanges();
                    //Almacena la auditoría

                    CADCONDTURNOCOMPMEDISP medio_copy = new CADCONDTURNOCOMPMEDISP();
                    medio_copy = (CADCONDTURNOCOMPMEDISP)auditoria.GenerarCopiaAuditoria(medio);

                    auditoria.RegistrarAuditoria<CADCONDTURNOCOMPMEDISP>(medio_copy, "CADCONDTURNOCOMPMEDISP", "CADCOMPSITGR=" + medio.CADCOMPSITGR.ToString() + ",CADCOMPCONSEC=" + medio.CADCOMPCONSEC
                  + ",CADMEDISPSITGR=" + medio.CADMEDISPSITGR + ",CADMEDISPCONSEC=" + medio.CADMEDISPCONSEC, TipoAuditoria.Modificar);



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