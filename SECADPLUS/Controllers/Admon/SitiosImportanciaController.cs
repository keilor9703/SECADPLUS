using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadLugaresGeograficos;
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

namespace SECADPLUS.Controllers
{
    public class SitiosImportanciaController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private EntitiesComunasBarrios db2 = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private GestorGeneral objGestorGral;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        /// <summary>
        /// Carga página principal
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public ActionResult Index(string message = null)
        {
            if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
            {
                ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                GestorBase.MSN_ALERTA_COMUN = "";
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
            }
            if (message != null)
            {
                ViewBag.MENSAJE_EXITOSO = message;
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
            }
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            string strAccion = ControllerContext.RouteData.Values["action"].ToString();
            string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
            ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Parqueaderos

        /// <summary>
        /// Carga la vista para crear parqueaderos
        /// </summary>
        /// <param name="idFather"></param>
        /// <returns></returns>
        [SecadPlusAutorizador("/SitiosImportancia/CreaParqueadero")]
        public ActionResult CreaParqueadero(int idFather, int intPadre, string strNombre)
        {
            try
            {
                //Lleno el drop downlist del luge_codigo
                List<SelectListItem> sitios = new List<SelectListItem>();
                sitios.Add(new SelectListItem() { Text = strNombre, Value = idFather.ToString() });
                ViewBag.SITIO_GRABA = new SelectList(sitios, "Value", "Text");
                ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION");

                ViewBag.CODI_BARRIO = new SelectList((from t in db2.CADBARRIOS where t.LUGE_CODIGO == -1 select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Post de la vista para crear parqueaderos
        /// </summary>
        /// <param name="cADPARQUEADERO">datos</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/SitiosImportancia/CreaParqueadero")]
        public async Task<ActionResult> CreaParqueadero(int idFather, int intPadre, [Bind(Include = "CODIGO,DESCRIPCION,LUGE_CODIGO,DIRECCION,CODI_BARRIO,LUGE_BARRIO,TELEFONO,HORARIO,USUA_TRANSACCION,FECHA_TRANSACCION,LUGE_CODIGO,SITIO_GRABA")] CADPARQUEADERO cADPARQUEADERO)
        {
            try
            {
                ViewBag.CODI_BARRIO = new SelectList((from t in db2.CADBARRIOS where t.LUGE_CODIGO == cADPARQUEADERO.LUGE_CODIGO select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADPARQUEADERO.CODI_BARRIO);
                if (ModelState.IsValid)
                {
                    CADPARQUEADERO cADPARQUEADEROExistente = db.CADPARQUEADEROS.Where(t => t.CODIGO == cADPARQUEADERO.CODIGO && t.SITIO_GRABA == cADPARQUEADERO.SITIO_GRABA).FirstOrDefault();
                    if (cADPARQUEADEROExistente != null)
                    {
                        ViewBag.MENSAJE_EXITOSO = "Ya existe un parqueadero con este código para el sitio de grabación";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                    else
                    {
                        db.CADPARQUEADEROS.Add(cADPARQUEADERO);
                        await db.SaveChangesAsync();
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADPARQUEADERO.CODIGO.ToString());
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    }
                }
                //var sitio = db.V_FORM_SITIOS_GRABACION.Where(m => m.CONSECUTIVO == idFather);
                ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION", cADPARQUEADERO.LUGE_CODIGO);

                ViewBag.SITIO_GRABA = new SelectList((from t in db.V_FORM_SITIOS_GRABACION where t.CONSECUTIVO == cADPARQUEADERO.SITIO_GRABA orderby t.DESCRIPCION select new { t.CONSECUTIVO, t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                return View(cADPARQUEADERO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Crea vista para borrar parqueaderos
        /// </summary>
        /// <param name="id">id parqueadero</param>
        /// <param name="idSitioGrabacion">id sitio grabación</param>
        /// <returns></returns>
        // GET: SitiosGrabacion/Delete/5
        [Authorize]
        public ActionResult BorrarParqueadero(string id, int? idSitioGrabacion)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPARQUEADERO cADPARQUEADERO = db.CADPARQUEADEROS.Where(t => t.CODIGO == id && t.SITIO_GRABA == idSitioGrabacion).FirstOrDefault();
                if (cADPARQUEADERO == null)
                {
                    return HttpNotFound();
                }
                return View("BorrarParqueadero", null, cADPARQUEADERO);
            }
            catch (Exception e)
            {
                return View("Error", e);
            }
        }

        /// <summary>
        /// carga página para confirmar borrado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idSitioGrabacion"></param>
        /// <returns></returns>
        [HttpPost, ActionName("BorrarParqueaderoConfirmed")]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/SitiosImportancia/BorraParqueadero")]
        public ActionResult BorrarParqueaderoConfirmed(string id, int? idSitioGrabacion)
        {
            try
            {
                CADPARQUEADERO cADPARQUEADERO = db.CADPARQUEADEROS.Where(t => t.CODIGO == id && t.SITIO_GRABA == idSitioGrabacion).FirstOrDefault();
                db.CADPARQUEADEROS.Remove(cADPARQUEADERO);
                db.SaveChanges();
                return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                return RedirectToAction("Index", "SitiosImportancia", new { message = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", id.ToString()) });
            }
            catch (Exception e)
            {
                return View("Error", e);
            }
        }

        /// <summary>
        /// Crea la vista para editar parqueaderos
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idSitioGrabacion"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditarParqueadero(string id, int? idSitioGrabacion, int? intPadre)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPARQUEADERO cADPARQUEADERO = db.CADPARQUEADEROS.Where(t => t.CODIGO == id && t.SITIO_GRABA == idSitioGrabacion).FirstOrDefault();

                if (cADPARQUEADERO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION", cADPARQUEADERO.LUGE_CODIGO);
                ViewBag.SITIO_GRABA = new SelectList((from m in db.V_FORM_SITIOS_GRABACION where m.CONSECUTIVO == cADPARQUEADERO.SITIO_GRABA orderby m.DESCRIPCION select new { m.CONSECUTIVO, m.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION", cADPARQUEADERO.SITIO_GRABA);
                ViewBag.CODI_BARRIO = new SelectList((from b in db2.CADBARRIOS where b.LUGE_CODIGO == cADPARQUEADERO.LUGE_CODIGO select new { b.CODIGO, b.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADPARQUEADERO.CODI_BARRIO);

                return View(cADPARQUEADERO);
            }
            catch (Exception e)
            {
                return View("Error", e);
            }
        }

        /// <summary>
        /// Carga vista para editar parqueadero
        /// </summary>
        /// <param name="idSitioGrabacion"></param>
        /// <param name="intPadre"></param>
        /// <param name="cADPARQUEADERO"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/SitiosImportancia/EditarParqueadero")]
        public ActionResult EditarParqueadero(int idSitioGrabacion, int? intPadre, [Bind(Include = "CODIGO,DESCRIPCION,LUGE_CODIGO,DIRECCION,CODI_BARRIO,LUGE_BARRIO,TELEFONO,HORARIO,USUA_TRANSACCION,FECHA_TRANSACCION,LUGE_CODIGO,SITIO_GRABA")] CADPARQUEADERO cADPARQUEADERO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADPARQUEADERO).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADPARQUEADERO.CODIGO) });
                }
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
            }

            var sitio = db.V_FORM_SITIOS_GRABACION.Where(m => m.CONSECUTIVO == idSitioGrabacion);
            ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION", cADPARQUEADERO.LUGE_CODIGO);
            ViewBag.SITIO_GRABA = new SelectList((from m in db.V_FORM_SITIOS_GRABACION where m.CONSECUTIVO == cADPARQUEADERO.SITIO_GRABA orderby m.DESCRIPCION select new { m.CONSECUTIVO, m.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION", cADPARQUEADERO.SITIO_GRABA);
            ViewBag.CODI_BARRIO = new SelectList((from b in db2.CADBARRIOS where b.LUGE_CODIGO == cADPARQUEADERO.LUGE_CODIGO select new { b.CODIGO, b.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADPARQUEADERO.CODI_BARRIO);

            return View(cADPARQUEADERO);
        }

        /// <summary>
        /// Muestra los parqueaderos para un sitio de grabación
        /// </summary>
        /// <param name="idConsecutivo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult VerParqueaderos(string idConsecutivo = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPersonas;
                string strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default' type='button' onclick=\"urlModificaParqueadero\"><i class='glyphicon glyphicon-edit'></i> Editar Parqueadero </button></li><li><button class='btn btn-default' type='button' onclick=\"EliminarElemento('urlEliminaParqueadero')\"><i class='icon-trash'></i> Borrar Parqueadero </button></li></ul></li></ul></td>";
                List<Ext_V_Parqueaderos> listaPersonas = new List<Ext_V_Parqueaderos>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor +
                            "%') OR upper(NOMBREBARRIO) LIKE upper('%" + parametros.ConsultaValor +
                            "%') OR upper(NOMBRECIUDAD)LIKE upper('%" + parametros.ConsultaValor + "%') )";
                    }
                    if (idConsecutivo != null && idConsecutivo != "-1")
                    {
                        strQuery += " AND SITIO_GRABA ='" + idConsecutivo + "'";
                    }
                }
                else
                {
                    if (idConsecutivo != null && idConsecutivo != "-1")
                    {
                        strQuery += " WHERE SITIO_GRABA ='" + idConsecutivo + "'";
                    }
                }

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttPersonas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaParqueaderos", strConexion, "Nonquery");

                foreach (DataRow fila in dttPersonas.Rows)
                {
                    //Control del páginado de registros
                    listaPersonas.Add(new Ext_V_Parqueaderos(fila, strBoton.Replace("@item.CODIGO",
                        fila["CODIGO"].ToString()).Replace("@item.IDSITIOGRABACION", fila["SITIO_GRABA"].ToString()).Replace("urlModificaParqueadero", "location.href='" +
                        Url.Action("EditarParqueadero", "SitiosImportancia", new { id = fila["CODIGO"].ToString(), idSitioGrabacion = fila["SITIO_GRABA"].ToString(), intPadre = fila["PADRE_CODIGO"].ToString() }).ToString() +
                        "'").Replace("urlBorraParqueadero",
                            Url.Action("BorrarParqueaderoConfirmed", "SitiosImportancia", new { id = fila["CODIGO"].ToString(), idSitioGrabacion = fila["SITIO_GRABA"].ToString() }).ToString())
                            .Replace("urlEliminaParqueadero", @Url.Action("BorrarParqueadero", "SitiosImportancia", new { id = fila["CODIGO"].ToString(), idSitioGrabacion = fila["SITIO_GRABA"].ToString() }))
                            ));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPersonas });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        #endregion Parqueaderos

        #region SitiosImportancia

        /// <summary>
        /// Crea la vista parcial para ver sitios de importancia
        /// </summary>
        /// <param name="idConsecutivo"></param>
        /// <returns></returns>
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public ActionResult VerSitiosImportantes(int idConsecutivo)
        {
            try
            {
                var cADSITIOS = db2.V_SITIOS_IMPORTANTES.Where(t => t.SITIO_GRABA == idConsecutivo).ToList();
                ViewBag.sitio = db.V_FORM_SITIOS_GRABACION.Where(t => t.CONSECUTIVO == idConsecutivo).FirstOrDefault();

                return PartialView(cADSITIOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// carga vista para crear sitio de importancia
        /// </summary>
        /// <param name="idFather">id lugar geográfico</param>
        /// <param name="intPadre">id sitio de grabación</param>
        /// <param name="strNombre">nombre sitio de grabación</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public ActionResult CreaSitio(int idFather, int intPadre, string strNombre)
        {
            List<SelectListItem> sitios = new List<SelectListItem>();
            sitios.Add(new SelectListItem() { Text = strNombre, Value = idFather.ToString() });

            ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION");

            ViewBag.SITIO_GRABA = new SelectList(sitios, "Value", "Text");

            ViewBag.CLSI_CODIGO = new SelectList(db.CADCLASES_SITIO, "CODIGO", "DESCRIPCION");
            ViewBag.BARRIOS = new SelectList((from t in db2.CADBARRIOS where t.LUGE_CODIGO == -1 select new { t.CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION");

            return View();
        }

        /// <summary>
        /// Guarda los datos del nuevo sitio de importancia
        /// </summary>
        /// <param name="idFather">id sitio de grabación</param>
        /// <param name="intPadre">id lugar geográfico</param>
        /// <param name="cADSITIO">datos</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public async Task<ActionResult> CreaSitio(int idFather, int intPadre, [Bind(Include = "SITIO_GRABA,CONSECUTIVO,CLSI_CODIGO,DESCRIPCION,DIRECCION,TELEFONO,BARR_CODIGO,BARR_LUGE_CODIGO,LATITUD,LONGITUD,COMENTARIO,LUGE_CODIGO")] CADSITIO cADSITIO)
        {
            try
            {

                cADSITIO.CONSECUTIVO = db.CADSITIOS.Max(t => t.CONSECUTIVO) + 1;
                ModelState.Clear();
                TryValidateModel(cADSITIO);

                if (ModelState.IsValid)
                {
                    CADSITIO cADSITIOExiste = db.CADSITIOS.Find(cADSITIO.SITIO_GRABA, cADSITIO.CONSECUTIVO);
                    if (cADSITIOExiste == null)
                    {

                        db.CADSITIOS.Add(cADSITIO);

                        await db.SaveChangesAsync();
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                        return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADSITIO.CONSECUTIVO.ToString()) });
                    }
                    else
                    {
                        ViewBag.MENSAJE_EXITOSO = "Ya existe un registro con este número para el sitio de grabación";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    }
                }
                var sitio = db.V_FORM_SITIOS_GRABACION.Where(m => m.CONSECUTIVO == idFather);
                ViewBag.SITIO_GRABA = new SelectList(sitio, "CONSECUTIVO", "DESCRIPCION", idFather);

                ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION", cADSITIO.LUGE_CODIGO);

                ViewBag.CLSI_CODIGO = new SelectList(db.CADCLASES_SITIO, "CODIGO", "DESCRIPCION", cADSITIO.CLSI_CODIGO);
                ViewBag.BARRIOS = new SelectList((from t in db2.CADBARRIOS where t.LUGE_CODIGO == cADSITIO.LUGE_CODIGO select new { CODIGO = t.CODIGO + "-" + t.LUGE_CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADSITIO.BARR_CODIGO + "-" + cADSITIO.BARR_LUGE_CODIGO);

                return View(cADSITIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga vista con listado de sitios de importancia
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="idSitioGrabacion"></param>
        /// <param name="intPadre"></param>
        /// <returns></returns>
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public async Task<ActionResult> EditarSitio(int? id, int? idSitioGrabacion, int intPadre)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADSITIO cADSITIO = await db.CADSITIOS.Where(t => t.CONSECUTIVO == id && t.SITIO_GRABA == idSitioGrabacion).FirstOrDefaultAsync();
            if (cADSITIO == null)
            {
                return HttpNotFound();
            }
            ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION", cADSITIO.LUGE_CODIGO);

            ViewBag.SITIO_GRABA = new SelectList((from m in db.V_FORM_SITIOS_GRABACION where m.CONSECUTIVO == idSitioGrabacion orderby m.DESCRIPCION select new { m.CONSECUTIVO, m.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
            ViewBag.CLSI_CODIGO = new SelectList(db.CADCLASES_SITIO, "CODIGO", "DESCRIPCION", cADSITIO.CLSI_CODIGO);
            ViewBag.BARRIOS = new SelectList((from b in db2.CADBARRIOS where b.LUGE_CODIGO == cADSITIO.LUGE_CODIGO select new { b.CODIGO, b.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADSITIO.BARR_CODIGO);
            return View(cADSITIO);
        }

        // POST: CADSITIOs/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public async Task<ActionResult> EditarSitio(int idSitioGrabacion, int intPadre, [Bind(Include = "SITIO_GRABA,CONSECUTIVO,CLSI_CODIGO,DESCRIPCION,DIRECCION,TELEFONO,BARR_CODIGO,BARR_LUGE_CODIGO,LATITUD,LONGITUD,COMENTARIO,LUGE_CODIGO")] CADSITIO cADSITIO)
        {
            if (cADSITIO.CONSECUTIVO == 0) cADSITIO.CONSECUTIVO = db.CADSITIOS.Max(t => t.CONSECUTIVO) + 1;

            ViewBag.LUGE_CODIGO = new SelectList((from t in db.CADLUGARES_GEOGRAFICOS where t.LUGE_CODIGO == intPadre orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray(), "CODIGO", "DESCRIPCION", cADSITIO.LUGE_CODIGO);

            ViewBag.SITIO_GRABA = new SelectList(db.V_FORM_SITIOS_GRABACION.Where(m => m.CONSECUTIVO == cADSITIO.SITIO_GRABA).OrderBy(m => m.DESCRIPCION), "CONSECUTIVO", "DESCRIPCION");
            ViewBag.CLSI_CODIGO = new SelectList(db.CADCLASES_SITIO, "CODIGO", "DESCRIPCION", cADSITIO.CLSI_CODIGO);
            //ViewBag.BARRIOS = new SelectList(db2.CADBARRIOS.Where(t => t.LUGE_CODIGO == -1), "CODIGO", "DESCRIPCION", cADSITIO.BARR_CODIGO);
            ViewBag.BARRIOS = new SelectList((from t in db2.CADBARRIOS where t.LUGE_CODIGO == cADSITIO.LUGE_CODIGO select new { CODIGO = t.CODIGO + "-" + t.LUGE_CODIGO, t.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADSITIO.BARR_CODIGO + "-" + cADSITIO.BARR_LUGE_CODIGO);
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(cADSITIO).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADSITIO.CONSECUTIVO.ToString()) });
                }
                catch (Exception ex)
                {
                    var mensaje = ex.Message;
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }
            }

            return View(cADSITIO);
        }

        /// <summary>
        /// Borra sitio importante
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idSitioGrabacion"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EliminarSitio(int id, int? idSitioGrabacion)
        {
            try
            {


                CADSITIO cADSitio = db.CADSITIOS.Where(t => t.CONSECUTIVO == id && t.SITIO_GRABA == idSitioGrabacion).FirstOrDefault();
                if (cADSitio == null)
                {
                    return HttpNotFound();
                }
                return View("BorrarSitio", null, cADSitio);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: SitiosGrabacion/Delete/5
        [HttpPost, ActionName("BorrarSitio")]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/SitiosImportancia/Index")]
        public ActionResult BorrarSitioConfirmed(int? id, int? idSitioGrabacion)
        {
            try
            {
                CADSITIO cADSitio = db.CADSITIOS.Where(t => t.CONSECUTIVO == id && t.SITIO_GRABA == idSitioGrabacion).FirstOrDefault();
                db.CADSITIOS.Remove(cADSitio);
                db.SaveChanges();
                return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", id.ToString());
                return RedirectToAction("Index", "SitiosImportancia");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion SitiosImportancia

        /// <summary>
        /// Busca los barrios con el id de la ciudad
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult BuscaBarrios(int id)
        {
            ViewBag.BARRIOS = new SelectList(db2.CADBARRIOS.Where(t => t.LUGE_CODIGO == id).OrderBy(t => t.DESCRIPCION), "CODIGO", "DESCRIPCION");
            var barrios = (from t in db2.CADBARRIOS where t.LUGE_CODIGO == id select new { descripcion = t.DESCRIPCION, codigo = t.CODIGO + "-" + t.LUGE_CODIGO }).ToList();

            return Json(barrios);
        }

        [Authorize]
        public JsonResult BuscaLugares(string sitio)
        {
            var lugares = (from t in db.CADLUGARES_GEOGRAFICOS where t.NIVEL > 1 && t.DESCRIPCION.Contains(sitio) orderby t.DESCRIPCION select new { t.CODIGO, t.DESCRIPCION }).ToArray();

            return Json(lugares);
        }
    }
}