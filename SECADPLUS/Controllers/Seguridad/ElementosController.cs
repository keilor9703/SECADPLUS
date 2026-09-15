using Oracle.ManagedDataAccess.Client;
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
    public class ElementosController : Controller
    {
        private PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();

        // GET: Elementos
        [SecadPlusAutorizador("/Elementos/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }
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
        public JsonResult ObtenerElementos()
        {
            try
            {
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtElemento> elementos = new List<ExtElemento>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdanamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                decimal dcElementoId = 0;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad two_btn dropdown-menu-left'> <li> <button class='btn btn-default ' type='button' data-toggle='modal' " +
                    "data-target='#modal_form_horizontal' data-popup='tooltip' onclick=\"location.href='_EDITAR'\" data-placement='bottom' data-original-title='Clic para editar'> <i class='glyphicon glyphicon-edit'>" +
                    "</i> Editar </button> </li><li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"EliminarRegistroGeneral('_ELIMINAR')\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li></ul> </li></ul>";
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
                        strQuery = "WHERE (ELEMENTO.ELEMENTO_POSICION = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(ELEMENTO.ELEMENTO_RUTA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(ELEMENTO.ELEMENTO_NOMBRE,'" + parametros.ConsultaValor + "', 'i')  "
                            + "OR REGEXP_LIKE(PADRE.ELEMENTO_NOMBRE,'" + parametros.ConsultaValor + "','i') OR REGEXP_LIKE(TIPO.TIPOELE_NOMBRE,'" + parametros.ConsultaValor + "','i') "
                            + "OR REGEXP_LIKE(ELEMENTO.ELEMENTO_FECHA_CREACION,'" + parametros.ConsultaValor + "','i' ) )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdanamiento += " ORDER BY PADRE.ELEMENTO_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_RUTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_POSICION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdanamiento += " ORDER BY TIPO.TIPOELE_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_ICONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_ESTADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_FECHA_CREACION " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdanamiento += " ORDER BY ELEMENTO.ELEMENTO_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "ELEMENTOS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdanamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    dcElementoId = Convert.ToDecimal(fila["CADELEMENTO_ID"].ToString());
                    elementos.Add(new ExtElemento(fila, strBoton.Replace("_EDITAR", (Url.Action("Edit", new { id = dcElementoId })).ToString()).Replace("_ELIMINAR", (Url.Action("Eliminar", new { id = dcElementoId })).ToString())));
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

        // GET: Elementos/Details/5
        public async Task<ActionResult> Details(decimal id)
        {
           
            CADNETELEMENTO cADNETELEMENTO = await db.CADNETELEMENTOes.FindAsync(id);
            if (cADNETELEMENTO == null)
            {
                return HttpNotFound();
            }
            return View(cADNETELEMENTO);
        }

        // GET: Elementos/Create

        public ActionResult Create()
        {
            try
            {
                SelectList lista = new SelectList(db.CADNETELEMENTOes, "CADELEMENTO_ID", "ELEMENTO_NOMBRE");
                ViewBag.Opcion = "Create";
                ViewBag.ELEMENTO_PADRE_ID = new SelectList(db.CADNETELEMENTOes, "CADELEMENTO_ID", "ELEMENTO_NOMBRE");
                ViewBag.ELEMENTO_TIPO_ID = new SelectList(db.CADNETTIPO_ELEMENTO, "CADTIPOELE_ID", "TIPOELE_NOMBRE");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Elementos/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CADELEMENTO_ID,ELEMENTO_TIPO_ID,ELEMENTO_NOMBRE,ELEMENTO_RUTA,ELEMENTO_ESTADO,ELEMENTO_POSICION,ELEMENTO_PADRE_ID, ELEMENTO_ICONO")] CADNETELEMENTO cADNETELEMENTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.CADNETELEMENTOes.Add(cADNETELEMENTO);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADNETELEMENTO.ELEMENTO_NOMBRE) });
                }

                ViewBag.ELEMENTO_PADRE_ID = new SelectList(db.CADNETELEMENTOes, "CADELEMENTO_ID", "ELEMENTO_NOMBRE", cADNETELEMENTO.ELEMENTO_PADRE_ID);
                ViewBag.ELEMENTO_TIPO_ID = new SelectList(db.CADNETTIPO_ELEMENTO, "CADTIPOELE_ID", "TIPOELE_NOMBRE", cADNETELEMENTO.ELEMENTO_TIPO_ID);
                return View(cADNETELEMENTO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Elementos/Edit/5
        public async Task<ActionResult> Edit(decimal id)
        {
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADNETELEMENTO cADNETELEMENTO = await db.CADNETELEMENTOes.FindAsync(id);
                if (cADNETELEMENTO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Opcion = "Edit";
                ViewBag.ELEMENTO_PADRE_ID = new SelectList(db.CADNETELEMENTOes, "CADELEMENTO_ID", "ELEMENTO_NOMBRE", cADNETELEMENTO.ELEMENTO_PADRE_ID);
                ViewBag.ELEMENTO_TIPO_ID = new SelectList(db.CADNETTIPO_ELEMENTO, "CADTIPOELE_ID", "TIPOELE_NOMBRE", cADNETELEMENTO.ELEMENTO_TIPO_ID);
                return View(cADNETELEMENTO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Elementos/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CADELEMENTO_ID,ELEMENTO_TIPO_ID,ELEMENTO_NOMBRE,ELEMENTO_RUTA,ELEMENTO_ESTADO,ELEMENTO_POSICION,ELEMENTO_PADRE_ID,ELEMENTO_ICONO, ELEMENTO_FECHA_CREACION")] CADNETELEMENTO cADNETELEMENTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADNETELEMENTO).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    //Almacena la auditoría
                    auditoria.RegistrarAuditoria<CADNETELEMENTO>(cADNETELEMENTO, "CADNETELEMENTO", cADNETELEMENTO.CADELEMENTO_ID.ToString(), TipoAuditoria.Modificar);
                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADNETELEMENTO.ELEMENTO_NOMBRE) });
                }
                ViewBag.ELEMENTO_PADRE_ID = new SelectList(db.CADNETELEMENTOes, "CADELEMENTO_ID", "ELEMENTO_NOMBRE", cADNETELEMENTO.ELEMENTO_PADRE_ID);
                ViewBag.ELEMENTO_TIPO_ID = new SelectList(db.CADNETTIPO_ELEMENTO, "CADTIPOELE_ID", "TIPOELE_NOMBRE", cADNETELEMENTO.ELEMENTO_TIPO_ID);
                return View(cADNETELEMENTO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Eliminar(decimal id)
        {
            CADNETELEMENTO cADNETELEMENTO = db.CADNETELEMENTOes.Find(id);
            if (cADNETELEMENTO == null)
            {
                return HttpNotFound();
            }
            return View("Delete", null, cADNETELEMENTO);
        }

        // POST: Elementos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal id, string strNombre)
        {
            try
            {
                CADNETELEMENTO cADNETELEMENTO = await db.CADNETELEMENTOes.FindAsync(id);
                db.CADNETELEMENTOes.Remove(cADNETELEMENTO);
                await db.SaveChangesAsync();
                //Almacena la auditoría

                CADNETELEMENTO cADNETELEMENTO_copy = new CADNETELEMENTO();
                cADNETELEMENTO_copy = (CADNETELEMENTO)auditoria.GenerarCopiaAuditoria(cADNETELEMENTO);

                auditoria.RegistrarAuditoria<CADNETELEMENTO>(cADNETELEMENTO_copy, "CADNETELEMENTO", "CADELEMENTO_ID=" + cADNETELEMENTO.CADELEMENTO_ID.ToString(), TipoAuditoria.Eliminar);
                return RedirectToAction("Index", "Elementos", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO.ToString() });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                return RedirectToAction("Index", "Elementos", new { strMensaje = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strNombre) });
            }
            catch (Exception ex)
            {
                ex.GetType();
                return RedirectToAction("Index");
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