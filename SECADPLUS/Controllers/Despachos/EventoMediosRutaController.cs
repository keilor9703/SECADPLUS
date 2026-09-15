using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class EventoMediosRutaController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: EventoMediosRuta
        [Authorize]
        public ActionResult Index(int idFuerza, int idSitio, int numeLlamada, long idConsecutivo, int numeEvento, string strMensaje, string strTipo)
        {
            try
            {
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

                //Cambiar estos datos por entrada de variables en Index
                //var idFuerza = 10610006;
                //var a = parametros[0];
                //var idSitio = 3947;
                //var numeLlamada = 6847915;

                ViewBag.idFuerza = idFuerza;
                ViewBag.idSitio = idSitio;
                ViewBag.numeLlamada = numeLlamada;
                ViewBag.idConsecutivo = idConsecutivo;
                ViewBag.numeEvento = numeEvento;

                GestorDespachos gestor = new GestorDespachos();
                ViewBag.DSP_MEDIOATENCION = gestor.ConsultarMedio(idSitio, idConsecutivo);

                //return View(dbDes.CADEVENTOMEDIOS_RUTAS.ToList());


                return View("Index", "MapaBase");

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerEventoMedios_Rutas(long idConsecutivo, int numeEvento)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " ";
                string strLimite = "", strOrderna = "";
                string strBoton = "<ul class='icons-list'>" +
                                        "<li class='dropdown'>" +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat" +
                                                                " dropdown-toggle' data-toggle='dropdown' aria-expanded='true'>" +
                                                "<i class='icon-menu9'></i>" +
                                            "</button>" +
                                            "<ul class='dropdown-menu two_btn'>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar'" +
                                                            "onclick=\"EditarRegistro('" + Url.Action("Edit", "EventoMediosRuta") + "', CONSECUTIVO_RUTA)\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar" +
                                                    "</button>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar'" +
                                                            "onclick=\"EliminarRegistro('" + Url.Action("Eliminar", "EventoMediosRuta") + "', CONSECUTIVO_RUTA)\"> " +
                                                        "<i class='icon-trash'></i> Eliminar" +
                                                    "</button>" +
                                                "</li>" +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";
                string strBotonDestino = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_EventoMediosRutaModel> listaRutas = new List<_EventoMediosRutaModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (CONSECUTIVO_RUTA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (ORIGEN, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (DESTINO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (HORA_INICIA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (HORA_LLEGA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (DSC_NOVEDAD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADMEDISP_CONSECUTIVO = " + idConsecutivo + " AND CADEVEN_NUME_EVENTO = " + numeEvento + " AND REGEXP_LIKE (OBSERVACIONES, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO_RUTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY ORIGEN " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY DESTINO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY HORA_INICIA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY HORA_LLEGA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY DSC_NOVEDAD " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY OBSERVACIONES " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_eventosmedios_rutas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CONSECUTIVO_RUTA", fila["CONSECUTIVO_RUTA"].ToString());

                    listaRutas.Add(new _EventoMediosRutaModel("", strBotonDestino, fila["CADMEDISP_CONSECUTIVO"].ToString(), fila["CONSECUTIVO_RUTA"].ToString(), fila["ORIGEN"].ToString(), fila["DESTINO"].ToString(),
                                                              fila["HORA_INICIA"].ToString(), fila["HORA_LLEGA"].ToString(), fila["DSC_NOVEDAD"].ToString(), fila["OBSERVACIONES"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRutas });
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
        public ActionResult Create(int idFuerza, int idSitio, int numeLlamada, long idConsecutivo, int numeEvento)
        {
            try
            {
                ViewBag.idFuerza = idFuerza;
                ViewBag.idSitio = idSitio;
                ViewBag.numeLlamada = numeLlamada;
                ViewBag.idConsecutivo = idConsecutivo;
                ViewBag.numeEvento = numeEvento;

                ViewBag.CADMEDISP_SITIOGRABA = idSitio;
                ViewBag.CADMEDISP_CONSECUTIVO = idConsecutivo;
                ViewBag.CADEVEN_SITIOGRABA = idSitio;
                ViewBag.CADEVEN_NUME_EVENTO = numeEvento;

                var CONSECUTIVO = dbDes.CADEVENTOMEDIOS_RUTAS.Max(c => c.CONSECUTIVO_RUTA);
                CONSECUTIVO += 1;

                ViewBag.CONSECUTIVO_RUTA = CONSECUTIVO;
                ViewBag.NOVEDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_NOVEDAD_PATRULLA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");


                return View("Create", "MapaBase");

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EventoMediosRuta/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CADMEDISP_SITIOGRABA,CADMEDISP_CONSECUTIVO,CADEVEN_SITIOGRABA,CADEVEN_NUME_EVENTO,CONSECUTIVO_RUTA,ORIGEN,DESTINO,HORA_INICIA,HORA_LLEGA,NOVEDAD,OBSERVACIONES")] CADEVENTOMEDIOS_RUTAS cADEVENTOMEDIOS_RUTAS)
        {
            try
            {
                var idFuerza = Request["idFuerza"];
                var idSitio = Request["idSitio"];
                var numeLlamada = Request["numeLlamada"];
                var idConsecutivo = Request["idConsecutivo"];
                var numeEvento = Request["numeEvento"];

                if (ModelState.IsValid)
                {
                    dbDes.CADEVENTOMEDIOS_RUTAS.Add(cADEVENTOMEDIOS_RUTAS);
                    dbDes.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADEVENTOMEDIOS_RUTAS>(cADEVENTOMEDIOS_RUTAS, "CADEVENTOMEDIOS_RUTAS", cADEVENTOMEDIOS_RUTAS.CONSECUTIVO_RUTA.ToString(), TipoAuditoria.Crear);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADEVENTOMEDIOS_RUTAS.CONSECUTIVO_RUTA.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idFuerza = idFuerza, idSitio = idSitio, numeLlamada = numeLlamada, idConsecutivo = idConsecutivo, numeEvento = numeEvento, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.idFuerza = idFuerza;
                ViewBag.idSitio = idSitio;
                ViewBag.numeLlamada = numeLlamada;
                ViewBag.idConsecutivo = idConsecutivo;
                ViewBag.numeEvento = numeEvento;

                ViewBag.CADMEDISP_SITIOGRABA = idSitio;
                ViewBag.CADMEDISP_CONSECUTIVO = idConsecutivo;
                ViewBag.CADEVEN_SITIOGRABA = idSitio;
                ViewBag.CADEVEN_NUME_EVENTO = numeEvento;

                var CONSECUTIVO = dbDes.CADEVENTOMEDIOS_RUTAS.Max(c => c.CONSECUTIVO_RUTA);
                CONSECUTIVO += 1;

                ViewBag.CONSECUTIVO_RUTA = CONSECUTIVO;

                ViewBag.NOVEDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_NOVEDAD_PATRULLA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADEVENTOMEDIOS_RUTAS.NOVEDAD);

                return View(cADEVENTOMEDIOS_RUTAS);
            }
            catch (DbUpdateException ex)
            {
                var m = ex.Message;
                var mensaje = "No se puede crear, revise datos.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: EventoMediosRuta/Edit/5
        [Authorize]
        public ActionResult Edit(int? id, int idFuerza, int idSitio, int numeLlamada, long idConsecutivo, int numeEvento)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.idFuerza = idFuerza;
                ViewBag.idSitio = idSitio;
                ViewBag.numeLlamada = numeLlamada;
                ViewBag.idConsecutivo = idConsecutivo;
                ViewBag.numeEvento = numeEvento;

                CADEVENTOMEDIOS_RUTAS cADEVENTOMEDIOS_RUTAS = dbDes.CADEVENTOMEDIOS_RUTAS.Find(idSitio, idConsecutivo, idSitio, numeEvento, id);
                if (cADEVENTOMEDIOS_RUTAS == null)
                {
                    return HttpNotFound();
                }

                ViewBag.NOVEDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_NOVEDAD_PATRULLA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADEVENTOMEDIOS_RUTAS.NOVEDAD);


                return View("Edit", "MapaBase", cADEVENTOMEDIOS_RUTAS);

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EventoMediosRuta/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CADMEDISP_SITIOGRABA,CADMEDISP_CONSECUTIVO,CADEVEN_SITIOGRABA,CADEVEN_NUME_EVENTO,CONSECUTIVO_RUTA,ORIGEN,DESTINO,HORA_INICIA,HORA_LLEGA,NOVEDAD,OBSERVACIONES")] CADEVENTOMEDIOS_RUTAS cADEVENTOMEDIOS_RUTAS)
        {
            try
            {
                var idFuerza = Request["idFuerza"];
                var idSitio = Request["idSitio"];
                var numeLlamada = Request["numeLlamada"];
                var idConsecutivo = Request["idConsecutivo"];
                var numeEvento = Request["numeEvento"];

                if (ModelState.IsValid)
                {
                    dbDes.Entry(cADEVENTOMEDIOS_RUTAS).State = EntityState.Modified;
                    dbDes.SaveChanges();

                    auditoria.RegistrarAuditoria<CADEVENTOMEDIOS_RUTAS>(cADEVENTOMEDIOS_RUTAS, "CADEVENTOMEDIOS_RUTAS", cADEVENTOMEDIOS_RUTAS.CONSECUTIVO_RUTA.ToString(), TipoAuditoria.Modificar);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADEVENTOMEDIOS_RUTAS.CONSECUTIVO_RUTA.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idFuerza = idFuerza, idSitio = idSitio, numeLlamada = numeLlamada, idConsecutivo = idConsecutivo, numeEvento = numeEvento, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.idFuerza = idFuerza;
                ViewBag.idSitio = idSitio;
                ViewBag.numeLlamada = numeLlamada;
                ViewBag.idConsecutivo = idConsecutivo;
                ViewBag.numeEvento = numeEvento;

                ViewBag.CADMEDISP_SITIOGRABA = idSitio;
                ViewBag.CADMEDISP_CONSECUTIVO = idConsecutivo;
                ViewBag.CADEVEN_SITIOGRABA = idSitio;
                ViewBag.CADEVEN_NUME_EVENTO = numeEvento;

                ViewBag.NOVEDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_NOVEDAD_PATRULLA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADEVENTOMEDIOS_RUTAS.NOVEDAD);
                return View(cADEVENTOMEDIOS_RUTAS);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: EventoMediosRuta/Delete/5
        [Authorize]
        public ActionResult Eliminar(int? id, int idFuerza, int idSitio, int numeLlamada, long idConsecutivo, int numeEvento)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADEVENTOMEDIOS_RUTAS cADEVENTOMEDIOS_RUTAS = dbDes.CADEVENTOMEDIOS_RUTAS.Find(idSitio, idConsecutivo, idSitio, numeEvento, id);
                if (cADEVENTOMEDIOS_RUTAS == null)
                {
                    return HttpNotFound();
                }

                ViewBag.idFuerza = idFuerza;
                ViewBag.idSitio = idSitio;
                ViewBag.numeLlamada = numeLlamada;
                ViewBag.idConsecutivo = idConsecutivo;
                ViewBag.numeEvento = numeEvento;

                return View("Delete", null, cADEVENTOMEDIOS_RUTAS);

                //return View(cADEVENTOMEDIOS_RUTAS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EventoMediosRuta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, int idFuerza, int idSitio, int numeLlamada, long idConsecutivo, int numeEvento)
        {
            try
            {
                CADEVENTOMEDIOS_RUTAS cADEVENTOMEDIOS_RUTAS = dbDes.CADEVENTOMEDIOS_RUTAS.Find(idSitio, idConsecutivo, idSitio, numeEvento, id);
                dbDes.CADEVENTOMEDIOS_RUTAS.Remove(cADEVENTOMEDIOS_RUTAS);
                dbDes.SaveChanges();


                CADEVENTOMEDIOS_RUTAS cADEVENTOMEDIOS_RUTAS_copy = new CADEVENTOMEDIOS_RUTAS();
                cADEVENTOMEDIOS_RUTAS_copy = (CADEVENTOMEDIOS_RUTAS)auditoria.GenerarCopiaAuditoria(cADEVENTOMEDIOS_RUTAS);

                auditoria.RegistrarAuditoria<CADEVENTOMEDIOS_RUTAS>(cADEVENTOMEDIOS_RUTAS_copy, "CADEVENTOMEDIOS_RUTAS", cADEVENTOMEDIOS_RUTAS.CONSECUTIVO_RUTA.ToString(), TipoAuditoria.Eliminar);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO.Replace("NUMERO", cADEVENTOMEDIOS_RUTAS.CONSECUTIVO_RUTA.ToString());
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { idFuerza = idFuerza, idSitio = idSitio, numeLlamada = numeLlamada, idConsecutivo = idConsecutivo, numeEvento = numeEvento, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idFuerza = idFuerza, idSitio = idSitio, numeLlamada = numeLlamada, idConsecutivo = idConsecutivo, numeEvento = numeEvento, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}