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
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class EncuestarController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: Encuestar
        [SecadPlusAutorizador("/Encuestar/Index")]
        public ActionResult Index()
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
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            string strAccion = ControllerContext.RouteData.Values["action"].ToString();
            string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
            ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            ViewBag.USUARIO_CREACION = usr.USUARIO;
            ViewBag.USUARIO = usr.USUARIO;

            ViewBag.FECHA_CREACION = DateTime.Now;

            ViewBag.NOVEDAD_ENCUESTA = new SelectList(dbDes.CLNOVEDADES_ENCUESTA.Where(c => c.TIPO == "EN").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
            ViewBag.NOVEDAD_ATENCION = new SelectList(dbDes.CLNOVEDADES_ENCUESTA.Where(c => c.TIPO == "NA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");

            return View();
        }

        [Authorize]
        public JsonResult ConsultarTurno(int idSitio, int idTurno)
        {
            try
            {
                var turno = db.CADREFERENCIASSECADs.SqlQuery("Select b.* " +
                                                             "From CADREFERENCIASSECAD b " +
                                                             "Inner Join CADTURNOS a " +
                                                             "On b.CODIGO = a.TIPO_TURNO " +
                                                             "Where a.sitio_graba = " + idSitio + " " +
                                                             "And a.turno = " + idTurno + " " +
                                                             "And b.nombre = 'TURNO_VIGILANCIA'").Select(c => c.DESCRIPCION).FirstOrDefault();

                var resultados = Json(new { result = true, datos = turno });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerEventos(int idSitio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + idSitio + " AND ESTADO_EVENTO = 'C' ";
                string strLimite = "";
                string strBoton = "<button class='btn bg-teal btn-mixed' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para ver' " +
                                         " onclick='CargarCamposPedido(NUME_EVENTO, CADPEDCA_NUMELLAMADA)'>" +
                                        "<i class='icon-eye'></i>" +
                                  "</button>";
                string strBotonDestino = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_EncuestarEventosModel> listaEventos = new List<_EncuestarEventosModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (SITIO_GRABA = " + idSitio + " AND ESTADO_EVENTO = 'C' AND REGEXP_LIKE (NUME_EVENTO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND ESTADO_EVENTO = 'C' AND REGEXP_LIKE (ESTADO_EVENTO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND ESTADO_EVENTO = 'C' AND REGEXP_LIKE (FECH_CREA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND ESTADO_EVENTO = 'C' AND REGEXP_LIKE (CADUSUA_EVEN_GENERA, '" + parametros.ConsultaValor + "', 'i')) ";
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_encuestar_eventos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("NUME_EVENTO", fila["NUME_EVENTO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CADPEDCA_NUMELLAMADA", fila["CADPEDCA_NUMELLAMADA"].ToString());

                    listaEventos.Add(new _EncuestarEventosModel(strBotonDestino, fila["NUME_EVENTO"].ToString(), fila["ESTADO_EVENTO"].ToString(), fila["FECH_CREA"].ToString(),
                                                                fila["CADUSUA_EVEN_GENERA"].ToString(), fila["CADTURN_TURNO"].ToString(), fila["CADPEDCA_NUMELLAMADA"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEventos });
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
        public JsonResult VerPedidoEvento(int idSitio, decimal numeLlamada)
        {
            try
            {
                string strQuery = " WHERE t.sitio_graba = " + idSitio + " AND t.nume_llamada = " + numeLlamada + " ";
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_PedidosLlamadaModel> listaPedidosLlamada = new List<_PedidosLlamadaModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();

                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_pedidos_llamada", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaPedidosLlamada.Add(new _PedidosLlamadaModel(fila["SITIO_GRABA"].ToString(), fila["HORA_CASO"].ToString(), fila["NUME_LLAMADA"].ToString(), fila["CADUSUA_USUARIO"].ToString(),
                                                                     fila["NUME_TELEFONO"].ToString(), fila["DISP_TELEFONICO"].ToString(), fila["PROP_TELEFONO"].ToString(),
                                                                     fila["DIRE_TELEFONO"].ToString(), fila["LUGE_TELEFONO"].ToString(), fila["BARR_TELEFONO"].ToString(),
                                                                     fila["NOMB_LLAMANTE"].ToString(), fila["DIRE_LLAMANTE"].ToString(), fila["TIPO_INFORMACION"].ToString(),
                                                                     fila["CODIGO_COOPERANTE"].ToString(), fila["CADPEDI_NUME_LLAMADA"].ToString(), fila["COMENTARIO"].ToString(),
                                                                     fila["CODI_PEDIDO"].ToString(), fila["DESC_CODI_PEDIDO"].ToString(), fila["CODI_PEDIDO2"].ToString(),
                                                                     fila["DESC_CODI_PEDIDO2"].ToString(), fila["DIRE_CASO"].ToString(), fila["DSP_BARRIO"].ToString(),
                                                                     fila["DSP_CIUDAD"].ToString(), fila["LUGE_CODIGO"].ToString(), fila["CODI_PEDIDO_REAL"].ToString(), fila["DESC_CODI_PEDIDO_REAL"].ToString()));
                }

                //rmr 02-10-2020

                string strQuery2 = " WHERE CD.CADPEDI_SITIOGRABA = " + idSitio + " AND CD.CADPEDI_NUMELLAMADA = " + numeLlamada + " ";
                GestorAdministrador gestor2;
                DataTable dttLugar2;
                List<_PedidosDatosAdicModel> listaPedidosLlamada2 = new List<_PedidosDatosAdicModel>();
                GestorGeneral objGestorGral2 = new GestorGeneral();
                gestor2 = new GestorAdministrador();

                objGestorGral2.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery2, typeof(string)));
                objGestorGral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar2 = objGestorGral2.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADINTEGMINUTASECADSIVICC.P_LISTA_PEDIDOS_DATOS_ADIC", strConexion, "Nonquery");

                foreach (DataRow fila2 in dttLugar2.Rows)
                {
                    listaPedidosLlamada2.Add(new _PedidosDatosAdicModel(fila2["CADPEDI_SITIOGRABA"].ToString(), fila2["CADPEDI_NUMELLAMADA"].ToString(), fila2["X_COORD"].ToString(), fila2["Y_COORD"].ToString(),
                                                                     fila2["MODALIDADCASO"].ToString(), fila2["CONDUCTAESPECIAL"].ToString(), fila2["ARMAMEDIO"].ToString(),
                                                                     fila2["TIPOMOVILIZAGRESOR"].ToString()));
                }

                // fin rmr 02-10-2020
                //var resultados = Json(new { result = true, lista = listaPedidosLlamada }); antes cambio
                var resultados = Json(new { result = true, lista = listaPedidosLlamada, lista2 = listaPedidosLlamada2 });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        //[Authorize]
        //public JsonResult VerEventoMedios(int idSitio, int numeEvento)
        //{
        //    try
        //    {
        //        DatatableForm parametros = new DatatableForm(Request.Form);
        //        string strQuery = " WHERE cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " ";
        //        string strLimite = "";
        //        //string strBoton = "<button class='btn bg-teal btn-mixed' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para ver' " +
        //        //                         " onclick='LlenarCamposPedido(CADPEDCA_NUMELLAMADA)'>" +
        //        //                        "<i class='icon-eye'></i>" +
        //        //                  "</button>";
        //        //string strBotonDestino = "";
        //        bool blFiltro = false;
        //        GestorAdministrador gestor;
        //        DataTable dttLugar;
        //        List<_EventosMediosModel> listaEventoMedios = new List<_EventosMediosModel>();
        //        GestorGeneral objGestorGral = new GestorGeneral();
        //        gestor = new GestorAdministrador();
        //        objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
        //        //Verifica si se debe realizar búsqueda de registros
        //        if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
        //        {
        //            blFiltro = true;
        //            strQuery += " AND (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (DSP_DESC_UNIDAD, '" + parametros.ConsultaValor + "', 'i')) " +
        //                       " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (DSP_MEDIODISPO, '" + parametros.ConsultaValor + "', 'i')) " +
        //                       " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (TIPO_RESPONSABLE, '" + parametros.ConsultaValor + "', 'i')) " +
        //                       " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_ASIGNA, '" + parametros.ConsultaValor + "', 'i')) " +
        //                       " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_LLEGA, '" + parametros.ConsultaValor + "', 'i')) " +
        //                       " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_TERMINA, '" + parametros.ConsultaValor + "', 'i')) " +
        //                       " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (NOVEDAD, '" + parametros.ConsultaValor + "', 'i')) ";

        // } strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " +
        // parametros.Final; objGestorGral.ParamsAvanzados.Add(new Tuple<string, string,
        // Type>("p_complemento", strQuery, typeof(string))); objGestorGral.ParamsAvanzados.Add(new
        // Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
        // objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "",
        // typeof(OracleDbType))); dttLugar =
        // objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario +
        // ".PK_CADPLUS.p_lista_eventos_medios", strConexion, "Nonquery");

        // foreach (DataRow fila in dttLugar.Rows) { listaEventoMedios.Add(new
        // _EventosMediosModel(fila["DSP_DESC_UNIDAD"].ToString(), fila["DSP_MEDIODISPO"].ToString(),
        // fila["TIPO_RESPONSABLE"].ToString(), fila["HORA_ASIGNA"].ToString(),
        // fila["HORA_LLEGA"].ToString(), fila["HORA_TERMINA"].ToString(), fila["NOVEDAD"].ToString()));

        // parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
        // parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString()); }
        // if (!blFiltro) { parametros.TotalRegistroFiltrados = parametros.TotalRegistros; }

        //        var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEventoMedios });
        //        resultados.MaxJsonLength = int.MaxValue;
        //        return resultados;
        //    }
        //    catch (Exception ex)
        //    {
        //        GestorBase.GestionarExcepcion(ex, GetType().Name);
        //        return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
        //    }
        //}

        public ActionResult VerEventoMedios(int idSitio, int numeEvento)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var eventosMedios = gestor.ConsultarEventoMedios(idSitio, numeEvento);

                List<_EventosMediosModel> listaEventoMedios = new List<_EventosMediosModel>();

                foreach (DataRow fila in eventosMedios.Rows)
                {
                    listaEventoMedios.Add(new _EventosMediosModel(fila["DSP_DESC_UNIDAD"].ToString(), fila["DSP_MEDIODISPO"].ToString(), fila["TIPO_RESPONSABLE"].ToString(), fila["HORA_ASIGNA"].ToString(),
                                                                  fila["HORA_LLEGA"].ToString(), fila["HORA_TERMINA"].ToString(), fila["NOVEDAD"].ToString()));
                }

                return PartialView("_EventoMedios", listaEventoMedios);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult VerEncuesta(int idSitio, decimal numeLlamada)
        {
            try
            {
                JsonResult resultados = new JsonResult();

                CLENCUESTA_CALIDAD_LLAMADA cLENCUESTA_CALIDAD_LLAMADA = dbDes.CLENCUESTA_CALIDAD_LLAMADA.Find(idSitio, numeLlamada);
                if (cLENCUESTA_CALIDAD_LLAMADA == null)
                {
                    return Json(new { result = true, exite = false });
                }

                List<string> datosEncuenta = new List<string>();
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.ACEPTA_ENCUESTA == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.ACEPTA_ENCUESTA.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.NOVEDAD_ENCUESTA == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.NOVEDAD_ENCUESTA.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.NOMBRE_ENCUESTADO == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.NOMBRE_ENCUESTADO.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.NOVEDAD_ATENCION == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.NOVEDAD_ATENCION.ToString());

                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.CALIF_OPORTUNIDAD == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.CALIF_OPORTUNIDAD.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.CALIF_AMABILIDAD == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.CALIF_AMABILIDAD.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.CALIF_EFECTIVIDAD == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.CALIF_EFECTIVIDAD.ToString());

                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.OBSERV_OPORTUNIDAD == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.OBSERV_OPORTUNIDAD.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.OBSERV_AMABILIDAD == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.OBSERV_AMABILIDAD.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.OBSERV_EFECTIVIDAD == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.OBSERV_EFECTIVIDAD.ToString());

                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.OBSERVACION == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.OBSERVACION.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.EMAIL_PERSONA == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.EMAIL_PERSONA.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.ESTADO_ENCUESTA == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.ESTADO_ENCUESTA.ToString());

                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.USUARIO_CREACION == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.USUARIO_CREACION.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.FECHA_CREACION == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.FECHA_CREACION.ToString());

                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.USUARIO_MODIFICACION == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.USUARIO_MODIFICACION.ToString());
                datosEncuenta.Add(cLENCUESTA_CALIDAD_LLAMADA.FECHA_MODIFICACION == null ? "" : cLENCUESTA_CALIDAD_LLAMADA.FECHA_MODIFICACION.ToString());

                resultados = Json(new { result = true, exite = true, encuesta = datosEncuenta });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
                // return Json(new { result = true, exite = true, encuesta =
                // cLENCUESTA_CALIDAD_LLAMADA });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // POST: Encuestar/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CADSITIO_GRABACION,CADNUME_PEDIDO,NUME_TELEFONO,LUGE_CASO,FECHA_CASO,CODIGO_CASO,RESUMEN_CASO,LLAMANTE,ACEPTA_ENCUESTA,NOVEDAD_ENCUESTA,NOMBRE_ENCUESTADO,CALIF_OPORTUNIDAD,OBSERV_OPORTUNIDAD,CALIF_AMABILIDAD,OBSERV_AMABILIDAD,CALIF_EFECTIVIDAD,OBSERV_EFECTIVIDAD,OBSERVACION,EMAIL_PERSONA,USUARIO_CREACION,FECHA_CREACION,USUARIO_MODIFICACION,FECHA_MODIFICACION,NOVEDAD_ATENCION,ESTADO_ENCUESTA")] CLENCUESTA_CALIDAD_LLAMADA cLENCUESTA_CALIDAD_LLAMADA)
        {
            if (ModelState.IsValid)
            {
                dbDes.CLENCUESTA_CALIDAD_LLAMADA.Add(cLENCUESTA_CALIDAD_LLAMADA);
                dbDes.SaveChanges();

                //auditoria.RegistrarAuditoria<CLENCUESTA_CALIDAD_LLAMADA>(cLENCUESTA_CALIDAD_LLAMADA, "CLENCUESTA_CALIDAD_LLAMADA", cLENCUESTA_CALIDAD_LLAMADA.CADNUME_PEDIDO.ToString(), TipoAuditoria.Crear);

                return RedirectToAction("Index");
            }

            return View(cLENCUESTA_CALIDAD_LLAMADA);
        }

        [Authorize]
        public JsonResult CreacionEncuesta([Bind(Include = "CADSITIO_GRABACION,CADNUME_PEDIDO,NUME_TELEFONO,LUGE_CASO,FECHA_CASO,CODIGO_CASO,RESUMEN_CASO,LLAMANTE,ACEPTA_ENCUESTA,NOVEDAD_ENCUESTA,NOMBRE_ENCUESTADO,CALIF_OPORTUNIDAD,OBSERV_OPORTUNIDAD,CALIF_AMABILIDAD,OBSERV_AMABILIDAD,CALIF_EFECTIVIDAD,OBSERV_EFECTIVIDAD,OBSERVACION,EMAIL_PERSONA,USUARIO_CREACION,FECHA_CREACION,USUARIO_MODIFICACION,FECHA_MODIFICACION,NOVEDAD_ATENCION,ESTADO_ENCUESTA")] CLENCUESTA_CALIDAD_LLAMADA cLENCUESTA_CALIDAD_LLAMADA)
        {
            try
            {
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    GestorGeneral gestor = new GestorGeneral();

                    int intExiste = dbDes.CLENCUESTA_CALIDAD_LLAMADA.Where(t => t.CADSITIO_GRABACION == cLENCUESTA_CALIDAD_LLAMADA.CADSITIO_GRABACION && t.CADNUME_PEDIDO == cLENCUESTA_CALIDAD_LLAMADA.CADNUME_PEDIDO).Count();
                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                    if (intExiste > 0)
                    {
                        cLENCUESTA_CALIDAD_LLAMADA.FECHA_MODIFICACION = (DateTime)gestor.ObtenerTiempoServidor;
                        cLENCUESTA_CALIDAD_LLAMADA.USUARIO_MODIFICACION = usr.USUARIO;
                        cLENCUESTA_CALIDAD_LLAMADA.USUARIO_CREACION = usr.USUARIO;


                        dbDes.Entry(cLENCUESTA_CALIDAD_LLAMADA).State = EntityState.Modified;
                        dbDes.SaveChanges();


                        auditoria.RegistrarAuditoria<CLENCUESTA_CALIDAD_LLAMADA>(cLENCUESTA_CALIDAD_LLAMADA, "CLENCUESTA_CALIDAD_LLAMADA", "CADSITIO_GRABACION=" + cLENCUESTA_CALIDAD_LLAMADA.CADSITIO_GRABACION.ToString() + ",CADNUME_PEDIDO=" + cLENCUESTA_CALIDAD_LLAMADA.CADNUME_PEDIDO
                        , TipoAuditoria.Modificar);


                        return Json(new { result = true, datos = new { MENSAJE = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cLENCUESTA_CALIDAD_LLAMADA.CADNUME_PEDIDO.ToString()), FECHA_CREACION = cLENCUESTA_CALIDAD_LLAMADA.FECHA_CREACION.ToString("dd/MM/yyyy HH:mm:ss") } });
                    }
                    else
                    {
                        cLENCUESTA_CALIDAD_LLAMADA.FECHA_CREACION = (DateTime)gestor.ObtenerTiempoServidor;
                        cLENCUESTA_CALIDAD_LLAMADA.USUARIO_CREACION = usr.USUARIO;

                        dbDes.CLENCUESTA_CALIDAD_LLAMADA.Add(cLENCUESTA_CALIDAD_LLAMADA);
                        dbDes.SaveChanges();

                        return Json(new { result = true, datos = new { MENSAJE = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cLENCUESTA_CALIDAD_LLAMADA.CADNUME_PEDIDO.ToString()), FECHA_CREACION = cLENCUESTA_CALIDAD_LLAMADA.FECHA_CREACION.ToString("dd/MM/yyyy HH:mm:ss") } });
                    }

                    //auditoria.RegistrarAuditoria<CLENCUESTA_CALIDAD_LLAMADA>(cLENCUESTA_CALIDAD_LLAMADA, "CLENCUESTA_CALIDAD_LLAMADA", cLENCUESTA_CALIDAD_LLAMADA.CADNUME_PEDIDO.ToString(), TipoAuditoria.Crear);
                }

                return Json(new { result = false, datos = "NO FUNCIONO" });

                //return View(cADMENSAJE);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, encuesta ya existe.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}