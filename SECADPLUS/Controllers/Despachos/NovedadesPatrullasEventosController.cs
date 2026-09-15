using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class NovedadesPatrullasEventosController : Controller
    {
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities db = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();
        private GestorGeneral objGestorGral;

        [SecadPlusAutorizador("/NovedadesPatrullasEventos/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = "Novedades";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADNOVEDADESPATRULLAS/Create
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                ViewBag.TIPO_NOVEDAD = new SelectList((from t in contexto.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_NOVEDAD_PATRULLA" orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION");



            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: CADNOVEDADESPATRULLAS/Create Para protegerse de ataques de publicación excesiva,
        // habilite las propiedades específicas a las que desea enlazarse. Para obtener más
        // información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "CADMEDISP_SITIOGRABA,CADMEDISP_CONSECUTIVO,CONSECUTIVO,HORA_INICIA,HORA_TERMINA,TIPO_NOVEDAD,OBSERVACION,USUA_USUARIO,CADFUERZ_ID")] CADNOVEDADESPATRULLA cADNOVEDADESPATRULLAS)
        {
            try
            {
                if (cADNOVEDADESPATRULLAS.HORA_INICIA >= cADNOVEDADESPATRULLAS.HORA_TERMINA)
                {
                    ViewBag.TIPO_NOVEDAD = new SelectList((from t in contexto.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_NOVEDAD_PATRULLA" orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADNOVEDADESPATRULLAS.TIPO_NOVEDAD);
                    if (cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO != 0)
                        ViewBag.MEDIOS_DESC = ObtenerDescPatrulla(cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO);

                    ViewBag.MENSAJE_EXITOSO = "La hora inicial no puede ser mayor o igual a la hora en que termina.";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;

                    return View(cADNOVEDADESPATRULLAS);
                }

                if (ModelState.IsValid)
                {
                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                    CADMEDIOSDISPONIBLE cADMEDIOSDISPONIBLE = new CADMEDIOSDISPONIBLE();

                    var consecutivo = (from t in db.CADNOVEDADESPATRULLAS where t.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION && t.CADMEDISP_CONSECUTIVO == cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO orderby t.CONSECUTIVO descending select t.CONSECUTIVO).FirstOrDefault();
                    int intConsecutivo = consecutivo == 0 ? 1 : Convert.ToInt32(consecutivo) + 1;

                    cADNOVEDADESPATRULLAS.CADMEDISP_SITIOGRABA = usr.SITIO_GRABACION;
                    cADNOVEDADESPATRULLAS.CADFUERZ_ID = (int)usr.CADCANA_FUERZA_ID;
                    cADNOVEDADESPATRULLAS.USUA_USUARIO = usr.USUARIO;
                    cADNOVEDADESPATRULLAS.CONSECUTIVO = intConsecutivo;

                    db.CADNOVEDADESPATRULLAS.Add(cADNOVEDADESPATRULLAS);
                    await db.SaveChangesAsync();

                    if (cADNOVEDADESPATRULLAS.HORA_TERMINA == null)
                    {
                        cADMEDIOSDISPONIBLE = await db.CADMEDIOSDISPONIBLES.FindAsync(cADNOVEDADESPATRULLAS.CADMEDISP_SITIOGRABA, cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO);
                        cADMEDIOSDISPONIBLE.ESTADO = "05";
                        db.Entry(cADMEDIOSDISPONIBLE).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADNOVEDADESPATRULLAS.CONSECUTIVO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }
                else
                {
                    ViewBag.TIPO_NOVEDAD = new SelectList((from t in contexto.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_NOVEDAD_PATRULLA" orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADNOVEDADESPATRULLAS.TIPO_NOVEDAD);
                    if (cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO != 0)
                        ViewBag.MEDIOS_DESC = ObtenerDescPatrulla(cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO);
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View(cADNOVEDADESPATRULLAS);
        }

        // GET: CADNOVEDADESPATRULLAS/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? sitioGraba, Int64? cadMedisConsecutivo, int? consecutivo)
        {
            CADNOVEDADESPATRULLA cADNOVEDADESPATRULLAS = new CADNOVEDADESPATRULLA();
            try
            {
                if (sitioGraba == null && cadMedisConsecutivo == null && consecutivo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                cADNOVEDADESPATRULLAS = await db.CADNOVEDADESPATRULLAS.FindAsync(sitioGraba, cadMedisConsecutivo, consecutivo);

                if (cADNOVEDADESPATRULLAS == null)
                {
                    return HttpNotFound();
                }

                ViewBag.TIPO_NOVEDAD = new SelectList((from t in contexto.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_NOVEDAD_PATRULLA" && t.CODIGO == cADNOVEDADESPATRULLAS.TIPO_NOVEDAD select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.MEDIOS_DESC = ObtenerDescPatrulla(cadMedisConsecutivo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View(cADNOVEDADESPATRULLAS);
        }

        // POST: CADNOVEDADESPATRULLAS/Edit/5 Para protegerse de ataques de publicación excesiva,
        // habilite las propiedades específicas a las que desea enlazarse. Para obtener más
        // información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CADMEDISP_SITIOGRABA,CADMEDISP_CONSECUTIVO,CONSECUTIVO,HORA_INICIA,HORA_TERMINA,TIPO_NOVEDAD,OBSERVACION,USUA_USUARIO,CADFUERZ_ID")] CADNOVEDADESPATRULLA cADNOVEDADESPATRULLAS)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CADMEDIOSDISPONIBLE cADMEDIOSDISPONIBLE = new CADMEDIOSDISPONIBLE();

                    db.Entry(cADNOVEDADESPATRULLAS).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    auditoria.RegistrarAuditoria<CADNOVEDADESPATRULLA>(cADNOVEDADESPATRULLAS, "CADNOVEDADESPATRULLAS", "CADMEDISP_SITIOGRABA=" + cADNOVEDADESPATRULLAS.CADMEDISP_SITIOGRABA.ToString() + ",CADMEDISP_CONSECUTIVO=" + cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO + ",CONSECUTIVO=" + cADNOVEDADESPATRULLAS.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);



                    if (cADNOVEDADESPATRULLAS.HORA_TERMINA == null)
                    {
                        cADMEDIOSDISPONIBLE = await db.CADMEDIOSDISPONIBLES.FindAsync(cADNOVEDADESPATRULLAS.CADMEDISP_SITIOGRABA, cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO);
                        cADMEDIOSDISPONIBLE.ESTADO = "05";
                        db.Entry(cADMEDIOSDISPONIBLE).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        cADMEDIOSDISPONIBLE = await db.CADMEDIOSDISPONIBLES.FindAsync(cADNOVEDADESPATRULLAS.CADMEDISP_SITIOGRABA, cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO);
                        cADMEDIOSDISPONIBLE.ESTADO = "01";
                        db.Entry(cADMEDIOSDISPONIBLE).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }



                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADNOVEDADESPATRULLAS.CONSECUTIVO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View(cADNOVEDADESPATRULLAS);
        }

        // GET: CADNOVEDADESPATRULLAS/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADNOVEDADESPATRULLA cADNOVEDADESPATRULLAS = await db.CADNOVEDADESPATRULLAS.FindAsync(id);
            if (cADNOVEDADESPATRULLAS == null)
            {
                return HttpNotFound();
            }
            return View(cADNOVEDADESPATRULLAS);
        }

        // POST: CADNOVEDADESPATRULLAS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                CADNOVEDADESPATRULLA cADNOVEDADESPATRULLAS = await db.CADNOVEDADESPATRULLAS.FindAsync(id);
                db.CADNOVEDADESPATRULLAS.Remove(cADNOVEDADESPATRULLAS);
                await db.SaveChangesAsync();

                CADNOVEDADESPATRULLA cADNOVEDADESPATRULLAS_copy = new CADNOVEDADESPATRULLA();
                cADNOVEDADESPATRULLAS_copy = (CADNOVEDADESPATRULLA)auditoria.GenerarCopiaAuditoria(cADNOVEDADESPATRULLAS);

                auditoria.RegistrarAuditoria<CADNOVEDADESPATRULLA>(cADNOVEDADESPATRULLAS_copy, "CADNOVEDADESPATRULLAS", "CADMEDISP_SITIOGRABA=" + cADNOVEDADESPATRULLAS.CADMEDISP_SITIOGRABA.ToString() + ",CADMEDISP_CONSECUTIVO=" + cADNOVEDADESPATRULLAS.CADMEDISP_CONSECUTIVO + ",CONSECUTIVO=" + cADNOVEDADESPATRULLAS.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);



                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }


        }

        /// <summary>
        /// Metodo que carga novedades de las patrullas
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerObtenerNovedadesPatrulla()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;

                string strBoton = "<ul class='icons-list'> " +
                                        "<li class='dropdown'> " +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                                "<i class='icon-menu9'></i> " +
                                            "</button> " +
                                            "<ul class='dropdown-menu dropdown-menu-left'> " +
                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar' " +
                                                             // "onclick=\"window.location.href =
                                                             // '/NovedadesPatrullasEventos/Edit?sitioGraba=SITIO_GRABA&cadMedisConsecutivo=CADMEDIS_CONSECUTIVO&consecutivo=_CONSECUTIVO'\">
                                                             // " +
                                                             "onclick =\"location.href='_EDITAR'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar " +
                                                    "</button> " +
                                                "</li> " +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";

                List<_NovedadesPatrullasModel> listaNovedadPatrulla = new List<_NovedadesPatrullasModel>();
                gestor = new GestorAdministrador();
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                strQuery = "WHERE CADMEDISP_SITIOGRABA = " + usr.SITIO_GRABACION.ToString() + " AND CADFUERZ_ID = " + usr.CADCANA_FUERZA_ID.ToString();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " AND REGEXP_LIKE (DESC_TIPO_NOVEDAD, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (OBSERVACION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (MEDIO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (HORA_INICIA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (HORA_TERMINA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 3:
                        strOrderna += " ORDER BY DESC_TIPO_NOVEDAD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY HORA_INICIA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY HORA_TERMINA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY OBSERVACION " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaNovedadesPatrullaEvent", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    //strBotonDestino = strBoton.Replace("SITIO_GRABA", fila["CADMEDISP_SITIOGRABA"].ToString());
                    //strBotonDestino = strBotonDestino.Replace("CADMEDIS_CONSECUTIVO", fila["CADMEDISP_CONSECUTIVO"].ToString());
                    //strBotonDestino = strBotonDestino.Replace("_CONSECUTIVO", fila["CONSECUTIVO"].ToString());
                    //  "onclick=\"window.location.href = '/NovedadesPatrullasEventos/Edit?sitioGraba=SITIO_GRABA&cadMedisConsecutivo=CADMEDIS_CONSECUTIVO&consecutivo=_CONSECUTIVO'\"> " +

                    strBotonDestino = strBoton.Replace("_EDITAR", (Url.Action("Edit", new { sitioGraba = fila["CADMEDISP_SITIOGRABA"].ToString(), cadMedisConsecutivo = fila["CADMEDISP_CONSECUTIVO"].ToString(), consecutivo = fila["CONSECUTIVO"].ToString() })).ToString());

                    listaNovedadPatrulla.Add(new _NovedadesPatrullasModel("", strBotonDestino, fila["MEDIO"].ToString(), fila["DESC_TIPO_NOVEDAD"].ToString(), fila["HORA_INICIA"].ToString(), fila["HORA_TERMINA"].ToString(), fila["OBSERVACION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaNovedadPatrulla });
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
        /// Metodo que obtiene los medios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerMedios()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick=\"CargarMedio(CODIGO,DESCRIPCION)\" title='' data-placement ='bottom' data-original-title='Clic para seleccionar' >" +
                                 " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttMedios;
                List<_MediosModel> listaMedios = new List<_MediosModel>();
                string turnoA = Convert.ToString(ObtenerTurnoAccesorio());
                string turnoN = Convert.ToString(ObtenerTurnoNormal());
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (ESTADO_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (ESTADO, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (TIPO, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (CUENTA, '" + parametros.ConsultaValor + "', 'i') ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY REGISTRO.DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabacion", usr.SITIO_GRABACION.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoA", turnoA, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoN", turnoN, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttMedios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaMediosNovedadesPatrulla", strConexion, "Nonquery");

                foreach (DataRow fila in dttMedios.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CONSECUTIVO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESC_PATRULLA"].ToString() + "'");
                    listaMedios.Add(new _MediosModel("", strBotonSelect, fila["DESC_PATRULLA"].ToString(), fila["SITIO_GRABA"].ToString(), fila["CONSECUTIVO"].ToString(), fila["ESTADO_PATRULLA"].ToString(), fila["TIPO"].ToString(), fila["ESTADO"].ToString(), fila["CUENTA"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaMedios });
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
        /// Metodo que obtiene la fecha actual
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerSiguienteFecha()
        {
            try
            {
                DateTime? fecha = new DateTime();
                objGestorGral = new GestorGeneral();
                fecha = objGestorGral.ObtenerTiempoServidor;
                return Json(new { result = true, datos = fecha.ToString() });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = "Se presento un problema." });
            }
        }

        /// <summary>
        /// Metodo que obtiene un turno normal
        /// </summary>
        /// <returns></returns>
        public decimal ObtenerTurnoNormal()
        {
            decimal turnoN = 0;

            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                objGestorGral = new GestorGeneral();

                var claseTurno = (from t in contexto.CADCANALES join a in contexto.CADFUERZAS on t.CADFUERZ_ID equals a.ID where a.ID == usr.CADCANA_FUERZA_ID && t.CODIGO == usr.CADCANA_CODIGO select t.CLASE_TURNO).FirstOrDefault();

                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", objGestorGral.ObtenerTiempoServidor.ToString(), typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clasturno", claseTurno.ToString(), typeof(string)));

                turnoN = (decimal)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_turnoNormal", strConexion);
                return turnoN;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que obtiene turno accesorio
        /// </summary>
        /// <returns></returns>
        public decimal ObtenerTurnoAccesorio()
        {
            decimal turnoA = 0;

            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", objGestorGral.ObtenerTiempoServidor.ToString(), typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clasturno", "03", typeof(string)));

                turnoA = (decimal)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.F_TURNO_ACCESORIO", strConexion);
                return turnoA;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string ObtenerDescPatrulla(Int64? cadMedisConsecutivo)
        {
            string patrulla = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_cadMedisConsecutivo", cadMedisConsecutivo.ToString(), typeof(decimal)));

                patrulla = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_descPatrullaMedios", strConexion);
                return patrulla;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
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