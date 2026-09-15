using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ReferenciasSECADController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: ReferenciasSECAD
        [SecadPlusAutorizador("/ReferenciasSECAD/Index")]
        public ActionResult Index(string strMensaje, string strTipo)
        {
            try
            {
                var mensaje = "";
                var tipo = "";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (string.IsNullOrEmpty(strMensaje))
                {
                    mensaje = string.Empty;
                    tipo = string.Empty;
                }
                else
                {
                    mensaje = strMensaje;
                    tipo = strTipo;
                }

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;

                //return View(db.CADREFERENCIASSECADs.OrderBy(c => c.NOMBRE).ThenBy(c => c.CODIGO).ToList());
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetReferenciasSECAD()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<ul class='icons-list'>" +
                                        "<li class='dropdown'>" +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'>" +
                                                "<i class='icon-menu9'></i>" +
                                            "</button>" +
                                            "<ul class='dropdown-menu dropdown-menu-left'>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar'" +
                                                            "onclick=\"window.location.href = '../ReferenciasSECAD/Edit?nom=NOMBRE&id=CODIGO'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar" +
                                                    "</button>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar'" +
                                                            "onclick=\"EliminarRegistroGeneral('../ReferenciasSECAD/Eliminar?nom=NOMBRE&id=CODIGO')\"> " +
                                                        "<i class='icon-trash'></i> Eliminar" +
                                                    "</button>" +
                                                "</li>" +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";
                string strBotonDestino = "";
                List<_ReferenciasSECADModel> listaReferenciasSECAD = new List<_ReferenciasSECADModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    //blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    //if (blControl)
                    //{
                    strQuery = " WHERE REGEXP_LIKE (NOMBRE, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (ABREVIATURA, '" + parametros.ConsultaValor + "', 'i') ";
                    //}
                    //else
                    //{
                    //strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i')";
                    //}
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY NOMBRE " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY ABREVIATURA " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_referencias_SECAD", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("NOMBRE", fila["NOMBRE"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CODIGO", fila["CODIGO"].ToString());

                    listaReferenciasSECAD.Add(new _ReferenciasSECADModel("", strBotonDestino, fila["NOMBRE"].ToString(), fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString(), fila["ABREVIATURA"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaReferenciasSECAD });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: ReferenciasSECAD/Create
        [SecadPlusAutorizador("/ReferenciasSECAD/Create")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ReferenciasSECAD/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOMBRE,CODIGO,DESCRIPCION,ABREVIATURA")] CADREFERENCIASSECAD cADREFERENCIASSECAD)
        {
            try
            {
                var existe = db.CADREFERENCIASSECADs.Find(cADREFERENCIASSECAD.NOMBRE, cADREFERENCIASSECAD.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("NOMBRE", "Este Nombre y Código esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {
                    db.CADREFERENCIASSECADs.Add(cADREFERENCIASSECAD);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADREFERENCIASSECAD>(cADREFERENCIASSECAD, "CADREFERENCIASSECAD", "LLAVE COMPUESTA: NOMBRE:" + cADREFERENCIASSECAD.NOMBRE + " | CODIGO:" + cADREFERENCIASSECAD.CODIGO, TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADREFERENCIASSECAD.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                return View(cADREFERENCIASSECAD);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ReferenciasSECAD/Edit/5
        [SecadPlusAutorizador("/ReferenciasSECAD/Edit")]
        public ActionResult Edit(string nom, string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADREFERENCIASSECAD cADREFERENCIASSECAD = db.CADREFERENCIASSECADs.Find(nom, id);
                if (cADREFERENCIASSECAD == null)
                {
                    return HttpNotFound();
                }

                ViewBag.antiguoNombre = cADREFERENCIASSECAD.NOMBRE;
                ViewBag.antiguoCodigo = cADREFERENCIASSECAD.CODIGO;
                return View(cADREFERENCIASSECAD);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ReferenciasSECAD/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NOMBRE,CODIGO,DESCRIPCION,ABREVIATURA")] CADREFERENCIASSECAD cADREFERENCIASSECAD)
        {
            try
            {
                var antiguoNombre = Request["antiguoNombre"];
                var antiguoCodigo = Request["antiguoCodigo"];

                if (antiguoNombre != cADREFERENCIASSECAD.NOMBRE || antiguoCodigo != cADREFERENCIASSECAD.CODIGO)
                {
                    var existe = db.CADREFERENCIASSECADs.Find(cADREFERENCIASSECAD.NOMBRE, cADREFERENCIASSECAD.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("NOMBRE", "Este Nombre y Código esta siendo utilizado en otro registro.");
                }

                if (ModelState.IsValid)
                {
                    if (antiguoNombre == cADREFERENCIASSECAD.NOMBRE && antiguoCodigo == cADREFERENCIASSECAD.CODIGO)
                    {
                        db.Entry(cADREFERENCIASSECAD).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADREFERENCIASSECAD>(cADREFERENCIASSECAD, "CADREFERENCIASSECAD", "LLAVE COMPUESTA: NOMBRE:" + cADREFERENCIASSECAD.NOMBRE + " | CODIGO:" + cADREFERENCIASSECAD.CODIGO, TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADREFERENCIASSECAD cADREFERENCIASSECAD1 = db.CADREFERENCIASSECADs.Find(antiguoNombre, antiguoCodigo);
                        db.CADREFERENCIASSECADs.Remove(cADREFERENCIASSECAD1);
                        db.SaveChanges();

                        //cADREFERENCIASSECAD.NOMBRE = nuevoNombre;
                        //cADREFERENCIASSECAD.CODIGO = nuevoCodigo;

                        db.CADREFERENCIASSECADs.Add(cADREFERENCIASSECAD);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADREFERENCIASSECAD>(cADREFERENCIASSECAD, "CADREFERENCIASSECAD", "LLAVE COMPUESTA: NOMBRE:" + cADREFERENCIASSECAD.NOMBRE + " | CODIGO:" + cADREFERENCIASSECAD.CODIGO, TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADREFERENCIASSECAD.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.antiguoNombre = antiguoNombre;
                ViewBag.antiguoCodigo = antiguoCodigo;

                return View(cADREFERENCIASSECAD);
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

        [SecadPlusAutorizador("/ReferenciasSECAD/Eliminar")]
        public ActionResult Eliminar(string nom, string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADREFERENCIASSECAD cADREFERENCIASSECAD = db.CADREFERENCIASSECADs.Find(nom, id);
                if (cADREFERENCIASSECAD == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADREFERENCIASSECAD);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ReferenciasSECAD/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string nom, string id)
        {
            try
            {
                CADREFERENCIASSECAD cADREFERENCIASSECAD = db.CADREFERENCIASSECADs.Find(nom, id);
                db.CADREFERENCIASSECADs.Remove(cADREFERENCIASSECAD);
                db.SaveChanges();

                CADREFERENCIASSECAD cADREFERENCIASSECAD_copy = new CADREFERENCIASSECAD();
                cADREFERENCIASSECAD_copy = (CADREFERENCIASSECAD)auditoria.GenerarCopiaAuditoria(cADREFERENCIASSECAD);

                auditoria.RegistrarAuditoria<CADREFERENCIASSECAD>(cADREFERENCIASSECAD_copy, "CADREFERENCIASSECAD", "LLAVE COMPUESTA: NOMBRE:" + cADREFERENCIASSECAD.NOMBRE + " | CODIGO:" + cADREFERENCIASSECAD.CODIGO, TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado la referencia SECAD: " + cADREFERENCIASSECAD.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
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