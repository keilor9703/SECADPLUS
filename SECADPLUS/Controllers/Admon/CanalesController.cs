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
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class CanalesController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorGeneral objGestorGral;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private EntitiesCasos db3 = new EntitiesCasos(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ModelCasos", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: Canales
        [SecadPlusAutorizador("/Canales/Index")]
        public ActionResult Index(int fuerzaId, string message = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!string.IsNullOrEmpty(message))
                {
                    ViewBag.MENSAJE_EXITOSO = message;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }

                //var cADCANALES = db.CADCANALES.Where(t => t.CADFUERZ_ID == fuerzaId).Include(c => c.CADFUERZA);
                ViewBag.idSitio = db.CADFUERZAS.Find(fuerzaId).SITIO_GRABA;
                ViewBag.idEntidad = fuerzaId;
                ViewBag.nombreEntidad = db.CADFUERZAS.Find(fuerzaId).DESCRIPCION;


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////



                //return View(await cADCANALES.ToListAsync());
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Canales/Create
        [SecadPlusAutorizador("/Canales/Create")]
        public ActionResult Create(int fuerzaId)
        {
            try
            {
                ViewBag.CADFUERZ_ID = new SelectList(db.CADFUERZAS.Where(t => t.ID == fuerzaId), "ID", "DESCRIPCION", fuerzaId);
                ViewBag.FUERZADESC = db.CADFUERZAS.Where(t => t.ID == fuerzaId).FirstOrDefault().DESCRIPCION;
                ViewBag.CLASE_TURNO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CLASE_TURNO" && (t.CODIGO == "01" || t.CODIGO == "02") orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion");
                ViewBag.TIPO_CANAL = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_CANAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion");
                ViewBag.TIPO_ATENCION = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ATENCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion");
                ViewBag.idFuerza = fuerzaId;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Canales/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/Canales/Create")]
        public async Task<ActionResult> Create(int fuerzaId, [Bind(Include = "CADFUERZ_ID,CODIGO,DESCRIPCION,ABREVIATURA,VIGENTE,DESDE,HASTA,OBSERVACION,DISPONIBLE,TIPO_CANAL,CLASE_TURNO,FECH_TRANSACCION,TIPO_ATENCION,AUTORIZACION_MEDIOS,AUTORIZACION_ANOTACIONES")] CADCANALE cADCANALE)
        {
            try
            {
                ViewBag.idFuerza = cADCANALE.CADFUERZ_ID;
                cADCANALE.VIGENTE = cADCANALE.VIGENTE == "true" ? "S" : "N";
                cADCANALE.DISPONIBLE = cADCANALE.DISPONIBLE == "true" ? "S" : "N";
                ModelState.Clear();
                TryValidateModel(cADCANALE);
                if ((from t in db.CADCANALES where t.CADFUERZ_ID == t.CADFUERZ_ID && t.CODIGO == cADCANALE.CODIGO select t).Count() > 0)
                {
                    ViewBag.MENSAJE_EXITOSO = "El registro ya existe para esta fuerza";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        db.CADCANALES.Add(cADCANALE);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { fuerzaId = cADCANALE.CADFUERZ_ID, message = Resources.RecursoGeneral.MSN_REGISTRO_CREACION });
                    }
                }
                ViewBag.VIGENTE = cADCANALE.VIGENTE == "S" ? true : false;
                ViewBag.DISPONIBLE = cADCANALE.DISPONIBLE == "S" ? true : false;
                ViewBag.CLASE_TURNO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CLASE_TURNO" && (t.CODIGO == "01" || t.CODIGO == "02") orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim().Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.CLASE_TURNO);
                ViewBag.TIPO_CANAL = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_CANAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.TIPO_CANAL);
                ViewBag.TIPO_ATENCION = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ATENCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.TIPO_ATENCION);

                ViewBag.CADFUERZ_ID = new SelectList(db.CADFUERZAS.Where(t => t.ID == fuerzaId), "ID", "DESCRIPCION", cADCANALE.CADFUERZ_ID);
                ViewBag.FUERZADESC = db.CADFUERZAS.Where(t => t.ID == cADCANALE.CADFUERZ_ID).FirstOrDefault().DESCRIPCION;
                return View(cADCANALE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Canales/Edit/5
        [SecadPlusAutorizador("/Canales/Edit")]
        public async Task<ActionResult> Edit(int fuerzaId, int? codigo)
        {
            try
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //CADCANALE cADCANALE = db.CADCANALES.Where(t => t.CADFUERZ_ID == fuerzaId && t.CODIGO == codigo).FirstOrDefault();
                CADCANALE cADCANALE = await db.CADCANALES.FindAsync(fuerzaId, codigo);
                if (cADCANALE == null)
                {
                    return HttpNotFound();
                }
                ViewBag.VIGENTE = cADCANALE.VIGENTE == "S" ? true : false;
                ViewBag.DISPONIBLE = cADCANALE.DISPONIBLE == "S" ? true : false;
                ViewBag.CLASE_TURNO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CLASE_TURNO" && (t.CODIGO == "01" || t.CODIGO == "02") orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim().Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.CLASE_TURNO);
                ViewBag.TIPO_CANAL = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_CANAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.TIPO_CANAL);
                ViewBag.TIPO_ATENCION = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ATENCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.TIPO_ATENCION);
                ViewBag.CADFUERZ_ID = new SelectList(db.CADFUERZAS.Where(t => t.ID == cADCANALE.CADFUERZ_ID), "ID", "DESCRIPCION");
                ViewBag.FUERZADESC = db.CADFUERZAS.Where(t => t.ID == cADCANALE.CADFUERZ_ID).FirstOrDefault().DESCRIPCION;
                return View(cADCANALE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Canales/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CADFUERZ_ID,CODIGO,DESCRIPCION,ABREVIATURA,VIGENTE,DESDE,HASTA,OBSERVACION,DISPONIBLE,TIPO_CANAL,CLASE_TURNO,FECH_TRANSACCION,TIPO_ATENCION,AUTORIZACION_MEDIOS,AUTORIZACION_ANOTACIONES")] CADCANALE cADCANALE)
        {
            try
            {
                cADCANALE.VIGENTE = cADCANALE.VIGENTE == "true" ? "S" : "N";
                cADCANALE.DISPONIBLE = cADCANALE.DISPONIBLE == "true" ? "S" : "N";
                ModelState.Clear();
                TryValidateModel(cADCANALE);
                if (ModelState.IsValid)
                {
                    db.Entry(cADCANALE).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    auditoria.RegistrarAuditoria<CADCANALE>(cADCANALE, "CADCANALE", "CADFUERZ_ID=" + cADCANALE.CADFUERZ_ID.ToString() + ",CODIGO=" + cADCANALE.CODIGO, TipoAuditoria.Modificar);



                    return RedirectToAction("Index", new { fuerzaId = cADCANALE.CADFUERZ_ID, message = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO });
                }
                ViewBag.CLASE_TURNO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CLASE_TURNO" && (t.CODIGO == "01" || t.CODIGO == "02") orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim().Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.CLASE_TURNO);
                ViewBag.TIPO_CANAL = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_CANAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.TIPO_CANAL);
                ViewBag.TIPO_ATENCION = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ATENCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION.Trim() }).ToArray(), "codigo", "descripcion", cADCANALE.TIPO_ATENCION);
                ViewBag.CADFUERZ_ID = new SelectList(db.CADFUERZAS, "ID", "DESCRIPCION", cADCANALE.CADFUERZ_ID);
                return View(cADCANALE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Canales/Delete/5
        [Authorize]
        public ActionResult Eliminar(int codigo, int fuerzaId)
        {
            try
            {
                CADCANALE cADCANALE = db.CADCANALES.Find(fuerzaId, codigo);
                if (cADCANALE == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADCANALE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Canales/Delete/5

        [Authorize]
        [HttpPost, ActionName("ConfirmaBorrado")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmaBorrado(decimal codigo, decimal fuerzaId)
        {
            try
            {
                CADCANALE cADCANALE = await db.CADCANALES.FindAsync(fuerzaId, codigo);
                db.CADCANALES.Remove(cADCANALE);
                await db.SaveChangesAsync();



                CADCANALE cADCANALE_copy = new CADCANALE();
                cADCANALE_copy = (CADCANALE)auditoria.GenerarCopiaAuditoria(cADCANALE);

                auditoria.RegistrarAuditoria<CADCANALE>(cADCANALE_copy, "CADCANALE", "CADFUERZ_ID=" + cADCANALE.CADFUERZ_ID.ToString() + ",CODIGO=" + cADCANALE.CODIGO, TipoAuditoria.Eliminar);




                return RedirectToAction("Index", new { fuerzaId = cADCANALE.CADFUERZ_ID, message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var valor = dbEx.Message;
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "El registro seleccionado");
                return RedirectToAction("Index", new { fuerzaId = fuerzaId });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Busca Barrios Asignados y por Asignar según el código de la ciudad
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="fuerzaId"></param>
        /// <param name="lugeCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> AsignarBarrios(decimal codigo, decimal fuerzaId, int? lugeCodigo)
        {
            try
            {
                ViewBag.CODIGOCANAL = codigo;
                ViewBag.CODIGOFUERZA = fuerzaId;
                ViewBag.CANAL = db.CADCANALES.Find(fuerzaId, codigo).DESCRIPCION;
                ViewBag.FUERZA = db.CADFUERZAS.Find(fuerzaId).DESCRIPCION;
                ViewBag.BARR_LUGE_CODIGO = new SelectList(new List<SelectListItem>(), "CODIGO", "DESCRIPCION");
                var cADBARRIOS = (from t in db.V_BARRIOS_CANALES where t.CODIGOCANAL == codigo && t.CODIGOFUERZA == fuerzaId && t.ASIGNADO == 1 select t);
                return View("AsignarBarrios", await cADBARRIOS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Busca barrios
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult BuscaBarrios(int id)
        {
            try
            {
                var barrios = (from t in db.CADBARRIOS
                               where t.LUGE_CODIGO == id
                               orderby t.DESCRIPCION
                               select new { descripcion = t.DESCRIPCION, codigo = t.CODIGO }).ToList();
                var cADBARRIOS = db.V_BARRIOS_CANALES;
                return Json(barrios);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Busca ciudades para seleccionar
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult CargaCiudades()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick = \"CargarCiudad(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                 " Seleccionar </button>";
                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca Ciudades sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                int? intLugeCodigo = usr.LUGE_CODIGO_DPTO;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttCiudades;
                List<Ext_Ciudades> listaCanalesBarrios = new List<Ext_Ciudades>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE (upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%') )";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                if (strQuery == string.Empty)
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                else
                    strLimites = " AND DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", intLugeCodigo.ToString(), typeof(Int32)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaCiudadesAsignarBarrios", strConexion, "Nonquery");

                foreach (DataRow fila in dttCiudades.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                    listaCanalesBarrios.Add(new Ext_Ciudades(fila, strBotonSelect));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCanalesBarrios });
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
        public JsonResult CargarTodosLosBarrios(string lugeCodigo = null, string canal = null, string fuerzaId = null)
        {
            try
            {
                if (lugeCodigo.Contains("Seleccione")) lugeCodigo = "-1";
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttBarrios;
                List<Ext_VCanalesBarrios> listaCanalesBarrios = new List<Ext_VCanalesBarrios>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                //strQuery = " WHERE (upper(DESCRIPCIONBARRIO) LIKE upper('%" + parametros.ConsultaValor + "%') ) ";

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE (upper(REGISTRO.DESCRIPCIONBARRIO) LIKE upper('%" + parametros.ConsultaValor + "%') ) ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY REGISTRO.DESCRIPCIONBARRIO " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo_luge", lugeCodigo, typeof(Int32)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", fuerzaId, typeof(Int32)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", canal, typeof(Int32)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttBarrios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_barrios", strConexion, "Nonquery");

                foreach (DataRow fila in dttBarrios.Rows)
                {
                    string boton = string.Format("<input name='ASIGNADO' id='{2}' onchange='BarrioCambia({2},{4})' class='checbox' type='checkbox' {3}>",
                       fila["DESCRIPCIONCIUDAD"]
                       , fila["DESCRIPCIONBARRIO"], fila["CODIGOBARRIO"], Convert.ToInt16(fila["ASIGNADO"]) == 1 ? "checked" : string.Empty
                       , fila["CODIGOLUGE"]);
                    listaCanalesBarrios.Add(new Ext_VCanalesBarrios(fila, boton));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCanalesBarrios });
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
        public JsonResult GetBarriosCanales(Estado[] estadosBarriosCanales, int canalCodigo, int fuerzaID)
        {
            try
            {
                if (estadosBarriosCanales != null)
                {
                    int intDuplicados = 0;
                    int intRechazadosPorError = 0;
                    int intNuevos = 0;
                    int intBorrados = 0;
                    foreach (var item in estadosBarriosCanales)
                    {
                        CADCANALES_BARRIOS cADCANALES_BARRIOS = db3.CADCANALES_BARRIOS.Find(item.id, item.LugeCodigo, fuerzaID, canalCodigo);
                        if (cADCANALES_BARRIOS != null)
                        {
                            if (item.estado == false)
                            {
                                db3.CADCANALES_BARRIOS.Remove(cADCANALES_BARRIOS);
                                try
                                {
                                    db3.SaveChanges();
                                    intBorrados++;
                                }
                                catch (Exception)
                                {
                                    intRechazadosPorError++;
                                }
                            }
                            else if (item.estado == true)
                            {
                                intDuplicados++;
                            }
                        }
                        else
                        {
                            cADCANALES_BARRIOS = new CADCANALES_BARRIOS();
                            cADCANALES_BARRIOS.BARR_CODIGO = item.id;
                            cADCANALES_BARRIOS.BARR_LUGE_CODIGO = item.LugeCodigo;
                            cADCANALES_BARRIOS.CADCANA_CODIGO = canalCodigo;
                            cADCANALES_BARRIOS.CADCANA_FUERZA = fuerzaID;
                            db3.CADCANALES_BARRIOS.Add(cADCANALES_BARRIOS);
                            db3.SaveChanges();
                            intNuevos++;
                        }
                    }
                    return Json(new { result = true, intDuplicados, intRechazadosPorError, intNuevos, intBorrados });
                }
                else
                {
                    return Json(new { result = true, datos = "Sin Cambios" });
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarCanales(string fuerzaId)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                //List<ExtComparendo> elementos = new List<ExtComparendo>();
                List<Ext_CadCanales> elementos = new List<Ext_CadCanales>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "";
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
                        strQuery = " WHERE (INTERNO.DESCRIPCION = '" + parametros.ConsultaValor + "' OR INTERNO.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i'))";
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

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.CLASE_TURNO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.ABREVIATURA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO_CANAL " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_DESDE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_HASTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 10:
                        strOrdenamiento += " ORDER BY INTERNO.DISPONIBLE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 11:
                        strOrdenamiento += " ORDER BY INTERNO.OBSERVACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 12:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO_ATENCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza_id", fuerzaId, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_canales", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    fila["VIGENTE"] = fila["VIGENTE"].ToString().Equals("S") ? "<span class='label label-success'>Sí</span>" : "<span class='label label-danger'>No</span>";
                    fila["DISPONIBLE"] = fila["DISPONIBLE"].ToString().Equals("S") ? "<span class='label label-success'>Sí</span>" : "<span class='label label-danger'>No</span>";
                    strBoton = "<ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle'" +
                                " data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button>" +
                                "<ul class='dropdown-menu secad dropdown-menu-left'>" +
                                "<li><button class='btn btn-default' type='button' onclick=\"location.href='" + @Url.Action("Edit", "Canales", new { fuerzaId = fila["CADFUERZ_ID"].ToString(), codigo = fila["CODIGO"].ToString() }) + "'\">" +
                                "<i class='icon-pen'></i> Editar" +
                                "</button></li><li>" +
                                "<button class='btn btn-default' type='button' onclick=\"EliminarElemento('" + @Url.Action("Eliminar", "Canales", new { codigo = fila["CODIGO"].ToString(), fuerzaId = fila["CADFUERZ_ID"].ToString() }) + "')\">" +
                                "<i class='icon-trash'></i> Borrar" +
                                "</button></li><li>" +
                                "<button class='btn btn-default' type='button' onclick=\"location.href='" + @Url.Action("Index", "AtencionCasos", new { idFuerza = fila["CADFUERZ_ID"].ToString(), idCanal = fila["CODIGO"].ToString() }) + "'\">" +
                                "<i class='icon-link'></i> Atención en casos" +
                                "</button></li><li>" +
                                "<button class='btn btn-default' type='button' onclick=\"location.href='" + @Url.Action("AsignarBarrios", "Canales", new { codigo = fila["CODIGO"].ToString(), fuerzaId = fila["CADFUERZ_ID"].ToString() }) + "'\">" +
                                "<i class='icon-link'></i> Asignar Barrios" +
                                "</button></li></ul></li></ul>";
                    elementos.Add(new Ext_CadCanales(fila, strBoton, 0));
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

        public class Estado
        {
            public int id { get; set; }
            public bool estado { get; set; }
            public int LugeCodigo { get; set; }
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