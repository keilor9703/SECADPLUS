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
    public class PerfilPermisoController : Controller
    {
        private PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: PerfilPermiso
        public async Task<ActionResult> Index(string strMensaje = null)
        {
            try { 
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                var cADNETPERFIL_ELE_PERMISO = db.CADNETPERFIL_ELE_PERMISO.Include(c => c.CADNETELEMENTO).Include(c => c.CADNETPERMISO).Include(c => c.CADNETPERFIL);
                return View(await cADNETPERFIL_ELE_PERMISO.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: PerfilPermiso/Details/5
        public async Task<ActionResult> Details(decimal id)
        {
            
            CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO = await db.CADNETPERFIL_ELE_PERMISO.FindAsync(id);
            if (cADNETPERFIL_ELE_PERMISO == null)
            {
                return HttpNotFound();
            }
            return View(cADNETPERFIL_ELE_PERMISO);
        }

        // GET: PerfilPermiso/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.PEP_ELEMENTO_ID = new SelectList(db.CADNETELEMENTOes.Select(e => new { e.CADELEMENTO_ID, e.ELEMENTO_NOMBRE }).OrderBy(e => e.ELEMENTO_NOMBRE), "CADELEMENTO_ID", "ELEMENTO_NOMBRE");
                ViewBag.PEP_PERMISO_ID = new SelectList(db.CADNETPERMISOes, "PERMISO_ID", "PERMISO_NOMBRE");
                ViewBag.PEP_PERFIL_ID = new SelectList(db.CADNETPERFILs.Select(p => new { p.PERFIL_ID, p.PERFIL_NOMBRE }).OrderBy(p => p.PERFIL_NOMBRE), "PERFIL_ID", "PERFIL_NOMBRE");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
        public JsonResult ObtenerElementos(decimal dcPerfilId) {
            try
            {
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                GestorGeneral objGestorGral;
                DataTable dttDatos;
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();                
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_perfil_id", dcPerfilId.ToString(), typeof(decimal)));                
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttDatos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_elementos_permiso", strConexion, "Nonquery");                
                //Se combiente en una lista
                var datos = (from DataRow dr in dttDatos.Rows
                                 select new
                                 {
                                     CODIGO = dr["CODIGO"],
                                     NOMBRE = dr["NOMBRE"]                                     
                                 }).ToList();
                var resultados = Json(new { result=true, datos = datos});
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
        // POST: PerfilPermiso/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PEP_PERFIL_ID,PEP_PERMISO_ID,PEP_ELEMENTO_ID,PEP_ESTADO")] CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    db.CADNETPERFIL_ELE_PERMISO.Add(cADNETPERFIL_ELE_PERMISO);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Seleccionado") });
                }

                ViewBag.PEP_ELEMENTO_ID = new SelectList(db.CADNETELEMENTOes.Select(e => new { e.CADELEMENTO_ID, e.ELEMENTO_NOMBRE }).OrderBy(e => e.ELEMENTO_NOMBRE), "CADELEMENTO_ID", "ELEMENTO_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_ELEMENTO_ID);
                ViewBag.PEP_PERMISO_ID = new SelectList(db.CADNETPERMISOes, "PERMISO_ID", "PERMISO_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_PERMISO_ID);
                ViewBag.PEP_PERFIL_ID = new SelectList(db.CADNETPERFILs.Select(p => new { p.PERFIL_ID, p.PERFIL_NOMBRE }).OrderBy(p => p.PERFIL_NOMBRE), "PERFIL_ID", "PERFIL_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_PERFIL_ID);
                return View(cADNETPERFIL_ELE_PERMISO);
            }
            catch (DbUpdateException dbEx)
            {
                GestorBase.GestionarExcepcion(dbEx, GetType().Name);
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Seleccionado");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        // GET: PerfilPermiso/Edit/5
        public ActionResult Edit(decimal perfil_id, decimal permiso_id, decimal elemento_id)
        {
            try
            {
                CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO = (from item in db.CADNETPERFIL_ELE_PERMISO
                                                                     where item.PEP_ELEMENTO_ID == elemento_id
                                                                     && item.PEP_PERFIL_ID == perfil_id
                                                                     && item.PEP_PERMISO_ID == permiso_id
                                                                     select item).FirstOrDefault();

                //await db.CADNETPERFIL_ELE_PERMISO.FindAsync(id);
                if (cADNETPERFIL_ELE_PERMISO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.PEP_ELEMENTO_ID = new SelectList(db.CADNETELEMENTOes.Select(e => new { e.CADELEMENTO_ID, e.ELEMENTO_NOMBRE }).OrderBy(e => e.ELEMENTO_NOMBRE), "CADELEMENTO_ID", "ELEMENTO_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_ELEMENTO_ID);
                ViewBag.PEP_PERMISO_ID = new SelectList(db.CADNETPERMISOes, "PERMISO_ID", "PERMISO_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_PERMISO_ID);
                ViewBag.PEP_PERFIL_ID = new SelectList(db.CADNETPERFILs.Select(p => new { p.PERFIL_ID, p.PERFIL_NOMBRE }).OrderBy(p => p.PERFIL_NOMBRE), "PERFIL_ID", "PERFIL_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_PERFIL_ID);
                return View(cADNETPERFIL_ELE_PERMISO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        // POST: PerfilPermiso/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PEP_PERFIL_ID,PEP_PERMISO_ID,PEP_ELEMENTO_ID,PEP_ESTADO,PEP_FECHA_CREACION")] CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADNETPERFIL_ELE_PERMISO).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    auditoria.RegistrarAuditoria<CADNETPERFIL_ELE_PERMISO>(cADNETPERFIL_ELE_PERMISO, "CADNETPERFIL_ELE_PERMISO", "PEP_PERFIL_ID=" + cADNETPERFIL_ELE_PERMISO.PEP_PERFIL_ID.ToString() , TipoAuditoria.Modificar);

                    
                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Seleccionado") });
                }
                ViewBag.PEP_ELEMENTO_ID = new SelectList(db.CADNETELEMENTOes.Select(e => new { e.CADELEMENTO_ID, e.ELEMENTO_NOMBRE }).OrderBy(e => e.ELEMENTO_NOMBRE), "CADELEMENTO_ID", "ELEMENTO_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_ELEMENTO_ID);
                ViewBag.PEP_PERMISO_ID = new SelectList(db.CADNETPERMISOes, "PERMISO_ID", "PERMISO_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_PERMISO_ID);
                ViewBag.PEP_PERFIL_ID = new SelectList(db.CADNETPERFILs.Select(p => new { p.PERFIL_ID, p.PERFIL_NOMBRE }).OrderBy(p => p.PERFIL_NOMBRE), "PERFIL_ID", "PERFIL_NOMBRE", cADNETPERFIL_ELE_PERMISO.PEP_PERFIL_ID);
                return View(cADNETPERFIL_ELE_PERMISO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Eliminar(decimal perfil_id, decimal permiso_id, decimal elemento_id)
        {
            try
            {
                CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO = (from item in db.CADNETPERFIL_ELE_PERMISO
                                                                     where item.PEP_ELEMENTO_ID == elemento_id
                                                                     && item.PEP_PERFIL_ID == perfil_id
                                                                     && item.PEP_PERMISO_ID == permiso_id
                                                                     select item).FirstOrDefault();
                //await db.CADNETPERFIL_ELE_PERMISO.FindAsync(id);
                if (cADNETPERFIL_ELE_PERMISO == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADNETPERFIL_ELE_PERMISO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult ObtenerRegistros()
        {
            try
            {
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtPermiso> elementos = new List<ExtPermiso>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                bool blFiltro = false, blControl = false;
                decimal dcElementoId = 0, dcPerfilId = 0, dcPermisoId = 0;
                DateTime dtTemporal;
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
                    blControl = DateTime.TryParse(parametros.ConsultaValor, out dtTemporal);
                    if (blControl)
                    {
                        strQuery = " WHERE ( TO_DATE(REGISTRO.PEP_FECHA_CREACION, 'dd/MM/YY') ='" + dtTemporal.ToShortDateString() + "' )";
                    }
                    else
                    {
                        strQuery = " WHERE (  REGEXP_LIKE(PERFIL.PERFIL_NOMBRE, '" + parametros.ConsultaValor + "','i')  "
                        + "OR REGEXP_LIKE(ELEMENTO.ELEMENTO_NOMBRE,'" + parametros.ConsultaValor + "','i') OR REGEXP_LIKE(PERMISO.PERMISO_NOMBRE,'" + parametros.ConsultaValor + "','i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY PERFIL.PERFIL_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY ELEMENTO.ELEMENTO_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY PERMISO.PERMISO_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY REGISTRO.PEP_FECHA_CREACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY REGISTRO.PEP_ESTADO " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY PERFIL.PERFIL_NOMBRE " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "PERMISOS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    dcElementoId = Convert.ToDecimal(fila["PEP_ELEMENTO_ID"].ToString());
                    dcPerfilId = Convert.ToDecimal(fila["PEP_PERFIL_ID"].ToString());
                    dcPermisoId = Convert.ToDecimal(fila["PEP_PERMISO_ID"].ToString());
                    elementos.Add(new ExtPermiso(fila, strBoton.Replace("_EDITAR", (Url.Action("Edit", new { perfil_id = dcPerfilId, permiso_id = dcPermisoId, elemento_id = dcElementoId })).ToString()).Replace("_ELIMINAR", (Url.Action("Eliminar", new { perfil_id = dcPerfilId, permiso_id = dcPermisoId, elemento_id = dcElementoId })).ToString())));
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

        // POST: PerfilPermiso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal perfil_id, decimal permiso_id, decimal elemento_id)
        {
            try
            {
                CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO = (from item in db.CADNETPERFIL_ELE_PERMISO
                                                                     where item.PEP_ELEMENTO_ID == elemento_id
                                                                     && item.PEP_PERFIL_ID == perfil_id
                                                                     && item.PEP_PERMISO_ID == permiso_id
                                                                     select item).FirstOrDefault();
                db.CADNETPERFIL_ELE_PERMISO.Remove(cADNETPERFIL_ELE_PERMISO);
                await db.SaveChangesAsync();


                CADNETPERFIL_ELE_PERMISO cADNETPERFIL_ELE_PERMISO_copy = new CADNETPERFIL_ELE_PERMISO();
                cADNETPERFIL_ELE_PERMISO_copy = (CADNETPERFIL_ELE_PERMISO)auditoria.GenerarCopiaAuditoria(cADNETPERFIL_ELE_PERMISO);

                auditoria.RegistrarAuditoria<CADNETPERFIL_ELE_PERMISO>(cADNETPERFIL_ELE_PERMISO_copy, "CADNETPERFIL_ELE_PERMISO", "PEP_PERFIL_ID=" + cADNETPERFIL_ELE_PERMISO.PEP_PERFIL_ID.ToString(), TipoAuditoria.Eliminar);

                return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                GestorBase.GestionarExcepcion(dbEx, GetType().Name);
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Seleccionado");
                return RedirectToAction("Index");
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