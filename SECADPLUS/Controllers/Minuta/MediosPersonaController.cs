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

namespace SECADPLUS.Controllers.Minuta
{
    public class MediosPersonaController : Controller
    {
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private Validador validar = new Validador();
        private CADUSUARIO usuarioActual;
        private CADCONDTURNOPER persona;
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
                    return RedirectToAction("Index", "Home");
                }
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
                persona = (from registro in contexto.CADCONDTURNOPERS
                           where registro.CONSECUTIVO == lgConsecutivo
                           && registro.SITIO_GRABA == intSitio
                           select registro).FirstOrDefault();
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.EventoMedio = (lgEventoMedio == null) ? 0 : lgEventoMedio;
                ViewBag.FechaConducta = persona.FECHA_CONDUCTA.ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.PersonaConsecutivo = persona.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = ((DateTime)gestor.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.Maquina = Session["MAQUINA"];
                return View("Crear", "MapaBase");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MediosPersona/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "CADPERSITGR,CADPERCONSEC,CADMEDISPSITGR,CADMEDISPCONSEC,CEDU_EMPLEADO,FECHA_CONDUCTA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABA,TIPO_ACCION")] CADCONDTURNOPERSMEDISP cADCONDTURNOPERSMEDISP)
        {
            try
            {
                string strMensaje = "";
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                var persona = (from registro in contexto.CADCONDTURNOPERS
                               where registro.CONSECUTIVO == cADCONDTURNOPERSMEDISP.CADPERCONSEC
                               && registro.SITIO_GRABA == cADCONDTURNOPERSMEDISP.CADPERSITGR
                               select registro).FirstOrDefault();
                cADCONDTURNOPERSMEDISP.FECHA_GRABACION = (DateTime)gestor.ObtenerTiempoServidor;
                if (ModelState.IsValid)
                {
                    var registroExistente = (
                        from registro in contexto.CADCONDTURNOPERSMEDISPs
                        where registro.CADPERCONSEC == cADCONDTURNOPERSMEDISP.CADPERCONSEC
                        && registro.CADPERSITGR == cADCONDTURNOPERSMEDISP.CADPERSITGR
                        && registro.CADMEDISPCONSEC == cADCONDTURNOPERSMEDISP.CADMEDISPCONSEC
                        && registro.CADMEDISPSITGR == cADCONDTURNOPERSMEDISP.CADMEDISPSITGR
                        && registro.CEDU_EMPLEADO == cADCONDTURNOPERSMEDISP.CEDU_EMPLEADO
                        select registro
                        ).FirstOrDefault();

                    if (registroExistente != null)
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                        return RedirectToAction("Crear", "MediosPersona", new { intSitio = cADCONDTURNOPERSMEDISP.CADMEDISPSITGR, lgTurno = persona.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOPERSMEDISP.CADPERCONSEC });
                    }
                    contexto.CADCONDTURNOPERSMEDISPs.Add(cADCONDTURNOPERSMEDISP);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Registro Nuevo");
                    return RedirectToAction("Crear", "MediosPersona", new { intSitio = cADCONDTURNOPERSMEDISP.CADMEDISPSITGR, lgTurno = persona.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOPERSMEDISP.CADPERCONSEC, strMensaje = strMensaje });
                }

                ViewBag.CADPERSITGR = new SelectList(contexto.CADCONDTURNOPERS, "SITIO_GRABA", "MODOCONDUCTA", cADCONDTURNOPERSMEDISP.CADPERSITGR);
                ViewBag.SitioId = cADCONDTURNOPERSMEDISP.CADMEDISPSITGR;
                ViewBag.Turno = persona.CADCONDTURNTURNO;
                ViewBag.EntidadId = Request.Form["hdEntidad"].ToString();
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOPERSMEDISP.CADPERSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(Convert.ToInt32(Request.Form["hdEntidad"].ToString())).DESCRIPCION;
                ViewBag.EventoMedio = (Request.Form["hdEventoMedio"] == null) ? "0" : Request.Form["hdEventoMedio"];
                ViewBag.FechaConducta = "";
                if (persona != null)
                {
                    ViewBag.FechaConducta = ((DateTime)persona.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = persona.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = gestor.ObtenerTiempoServidor;
                ViewBag.Maquina = Session["MAQUINA"];
                return View("Crear", "MapaBase", cADCONDTURNOPERSMEDISP);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [Authorize]
        public ActionResult Editar(string strMensaje = null)
        {
            try
            {
                CADCONDTURNOPERSMEDISP medio;

                long lgPerConsec = Convert.ToInt64(Request.Params["lgPerConsec"]);
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
                persona = (from registro in contexto.CADCONDTURNOPERS
                           where registro.CONSECUTIVO == lgPerConsec
                           && registro.SITIO_GRABA == intSitio
                           select registro).FirstOrDefault();
                medio = (from registro in contexto.CADCONDTURNOPERSMEDISPs
                         where registro.CADMEDISPCONSEC == lgMedioConsec
                         && registro.CADMEDISPSITGR == intSitio
                         && registro.CADPERCONSEC == lgPerConsec
                         && registro.CEDU_EMPLEADO == strCeduEmpleado
                         && registro.CADPERSITGR == intSitio
                         select registro).FirstOrDefault();
                ViewBag.SitioId = intSitio;
                ViewBag.Turno = lgTurno;
                ViewBag.EntidadId = intEntidad;
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(intSitio).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(intEntidad).DESCRIPCION;
                ViewBag.EventoMedio = lgMedioConsec;
                ViewBag.FechaConducta = persona.FECHA_CONDUCTA.ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.PersonaConsecutivo = persona.CONSECUTIVO;
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
        public ActionResult Editar([Bind(Include = "CADPERSITGR,CADPERCONSEC,CADMEDISPSITGR,CADMEDISPCONSEC,CEDU_EMPLEADO,FECHA_CONDUCTA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABA,TIPO_ACCION")] CADCONDTURNOPERSMEDISP cADCONDTURNOPERSMEDISP)
        {
            try
            {
                string strMensaje = "";
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("InicioEmbebido", "Home");
                }
                var persona = (from registro in contexto.CADCONDTURNOPERS
                               where registro.CONSECUTIVO == cADCONDTURNOPERSMEDISP.CADPERCONSEC
                               && registro.SITIO_GRABA == cADCONDTURNOPERSMEDISP.CADPERSITGR
                               select registro).FirstOrDefault();
                cADCONDTURNOPERSMEDISP.FECHA_GRABACION = (DateTime)gestor.ObtenerTiempoServidor;
                if (ModelState.IsValid)
                {
                    contexto.Entry(cADCONDTURNOPERSMEDISP).State = EntityState.Modified;
                    contexto.SaveChanges();


                    auditoria.RegistrarAuditoria<CADCONDTURNOPERSMEDISP>(cADCONDTURNOPERSMEDISP, "CADCONDTURNOPERSMEDISP", "CADPERSITGR=" + cADCONDTURNOPERSMEDISP.CADPERSITGR.ToString() + ",CADPERCONSEC=" + cADCONDTURNOPERSMEDISP.CADPERCONSEC
                         + ",CADMEDISPSITGR=" + cADCONDTURNOPERSMEDISP.CADMEDISPSITGR + ",CADMEDISPCONSEC=" + cADCONDTURNOPERSMEDISP.CADMEDISPCONSEC, TipoAuditoria.Modificar);




                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Registro seleccionado-");

                    ///return RedirectToAction("Editar", "MediosPersona", new { intSitio = cADCONDTURNOPERSMEDISP.CADMEDISPSITGR, lgTurno = persona.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOPERSMEDISP.CADPERCONSEC, strMensaje = strMensaje , strPatrulla = Request.Params["strPatrulla"], strDesc = Request.Params["strDesc"], strPersona = Request.Params["strPersona"];
                    return RedirectToAction("Editar", "MediosPersona", new { strCeduEmpleado = cADCONDTURNOPERSMEDISP.CEDU_EMPLEADO, lgPerConsec = cADCONDTURNOPERSMEDISP.CADPERCONSEC, lgMedioConsec = cADCONDTURNOPERSMEDISP.CADMEDISPCONSEC, intSitio = cADCONDTURNOPERSMEDISP.CADMEDISPSITGR, lgTurno = persona.CADCONDTURNTURNO, intEntidad = Request.Form["hdEntidad"].ToString(), lgConsecutivo = cADCONDTURNOPERSMEDISP.CADPERCONSEC, strMensaje = strMensaje, strPatrulla = Request.Form["txtPatrulla"], strDesc = Request.Form["txtDescripcion"], strPersona = Request.Params["txtEmpleado"] });
                }

                ViewBag.CADPERSITGR = new SelectList(contexto.CADCONDTURNOPERS, "SITIO_GRABA", "MODOCONDUCTA", cADCONDTURNOPERSMEDISP.CADPERSITGR);
                ViewBag.SitioId = cADCONDTURNOPERSMEDISP.CADMEDISPSITGR;
                ViewBag.Turno = persona.CADCONDTURNTURNO;
                ViewBag.EntidadId = Request.Form["hdEntidad"].ToString();
                ViewBag.Sitio = db.CADSITIOS_GRABACION.Find(cADCONDTURNOPERSMEDISP.CADPERSITGR).DESCRIPCION;
                ViewBag.Entidad = db.CADFUERZAS.Find(Convert.ToInt32(Request.Form["hdEntidad"].ToString())).DESCRIPCION;
                ViewBag.EventoMedio = (Request.Form["hdEventoMedio"] == null) ? "0" : Request.Form["hdEventoMedio"];
                ViewBag.FechaConducta = "";
                if (persona != null)
                {
                    ViewBag.FechaConducta = ((DateTime)persona.FECHA_CONDUCTA).ToString("dd/MM/yyyy HH:mm:ss");
                }
                ViewBag.PersonaConsecutivo = persona.CONSECUTIVO;
                ViewBag.Usuario = usuarioActual.USUARIO;
                ViewBag.FechaRegistro = gestor.ObtenerTiempoServidor;
                ViewBag.Maquina = Session["MAQUINA"];
                ViewBag.Patrulla = Request.Form["txtPatrulla"];
                ViewBag.Desc = Request.Form["txtDescripcion"];
                ViewBag.Persona = Request.Params["txtEmpleado"];
                return View("Editar", "MapaBase", cADCONDTURNOPERSMEDISP);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult EliminarMedio(int intSitio, long lgPerCon, long lgMedCon, string strCedula)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                string strMensaje = "";
                CADCONDTURNOPERSMEDISP medio;
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
                medio = (from registro in contexto.CADCONDTURNOPERSMEDISPs
                         where registro.CADMEDISPCONSEC == lgMedCon
                         && registro.CADMEDISPSITGR == intSitio
                         && registro.CADPERCONSEC == lgPerCon
                         && registro.CEDU_EMPLEADO == strCedula
                         && registro.CADPERSITGR == intSitio
                         select registro).FirstOrDefault();
                if (medio != null)
                {
                    contexto.CADCONDTURNOPERSMEDISPs.Remove(medio);
                    contexto.SaveChanges();
                    //Almacena la auditoría


                    CADCONDTURNOPERSMEDISP medio_copy = new CADCONDTURNOPERSMEDISP();
                    medio_copy = (CADCONDTURNOPERSMEDISP)auditoria.GenerarCopiaAuditoria(medio);

                    auditoria.RegistrarAuditoria<CADCONDTURNOPERSMEDISP>(medio_copy, "CADCONDTURNOPERSMEDISP", "CADPERSITGR=" + medio.CADPERSITGR.ToString() + ",CADPERCONSEC=" + medio.CADPERCONSEC
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

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerMediosPersona(int intSitio, long lgTurno, long lgEventoMedio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadPatrulla> elementos = new List<Ext_CadPatrulla>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
               "onclick = \"SeleccionarPatrulla(_CONSEC,'_PATRULLA','_DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
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
                        strQuery = " WHERE (INTERNO.CADMEDISPCONSEC = '" + parametros.ConsultaValor + "' ) ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.PATRULLA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.DESCRIPCION,'" + parametros.ConsultaValor + "', 'i')  "
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
                        strOrdenamiento += " ORDER BY INTERNO.PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_medio", lgEventoMedio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_medioscondpersona", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CONSEC", fila["CONSECUTIVO"].ToString()).Replace("_PATRULLA", fila["PATRULLA"].ToString()).Replace("_DESCRIPCION", fila["DESCRIPCION"].ToString());
                    elementos.Add(new Ext_CadPatrulla(fila, strBotonFinal, true));
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
        public JsonResult ObtenerEmpleados(int intSitio, long lgConsecutivo, long lgTurno)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Empleado> elementos = new List<Empleado>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
               "onclick = \"SeleccionarEmpleado(_CEDULA,'_EMPLEADO')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
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
                        strQuery = " WHERE (INTERNO.CEDU_EMPLEADO = " + parametros.ConsultaValor + " ) ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.EMPLEADO, '" + parametros.ConsultaValor + "', 'i') "
                            + ")";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_consec", lgConsecutivo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", lgTurno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dtDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_personas_condturnper", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strBotonFinal = strBoton;
                    strBotonFinal = strBotonFinal.Replace("_CEDULA", fila["CEDU_EMPLEADO"].ToString()).Replace("_EMPLEADO", fila["EMPLEADO"].ToString());
                    elementos.Add(new Empleado(fila, strBotonFinal));
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