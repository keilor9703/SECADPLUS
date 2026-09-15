using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
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
    public class ReferenciasSitioGrabacionController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        //private string strConexion = ConfigurationManager.ConnectionStrings["SECAD12C"].ConnectionString;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: ReferenciasSitioGrabacion
        [SecadPlusAutorizador("/ReferenciasSitioGrabacion/Index")]
        public ActionResult Index(int? id, string nom, string strMensaje, string strTipo)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

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

                //Agregar filtro de sitios de grabación
                //var cADREFERENCIAS = db.CADREFERENCIAS.Include(c => c.CADSITIOS_GRABACION).Where(c => c.SITIO_GRABA == id);
                ViewBag.idSitio = id;
                ViewBag.DESCRIPCION = nom;
                //var cADREFERENCIAS = db.CADREFERENCIAS.Include(c => c.CADSITIOS_GRABACION);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ReferenciasSitioGrabacion/Create
        [SecadPlusAutorizador("/ReferenciasSitioGrabacion/Create")]
        public ActionResult Create(int? id, string nom)
        {
            try
            {
                //ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.SITIO_GRABA = id;
                ViewBag.NOM_GRABA = nom;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ReferenciasSitioGrabacion/Create To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, string nom, [Bind(Include = "SITIO_GRABA,NOMBRE,CODIGO,DESCRIPCION,ABREVIATURA")] CADREFERENCIA cADREFERENCIA)
        {
            try
            {
                var existe = db.CADREFERENCIAS.Find(cADREFERENCIA.SITIO_GRABA, cADREFERENCIA.NOMBRE, cADREFERENCIA.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("NOMBRE", "Este Nombre y Código esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {
                    db.CADREFERENCIAS.Add(cADREFERENCIA);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADREFERENCIA>(cADREFERENCIA, "CADREFERENCIAS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADREFERENCIA.SITIO_GRABA + " | NOMBRE:" + cADREFERENCIA.NOMBRE + " | CODIGO:" + cADREFERENCIA.CODIGO, TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADREFERENCIA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { id = id, nom = nom });
                }

                //ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION", cADREFERENCIA.SITIO_GRABA);
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.SITIO_GRABA = id;
                ViewBag.NOM_GRABA = nom;
                return View(cADREFERENCIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ReferenciasSitioGrabacion/Edit/5
        [SecadPlusAutorizador("/ReferenciasSitioGrabacion/Edit")]
        public ActionResult Edit(int codSitio, string nomSitio, string nom, string cod)
        {
            try
            {
                if (cod == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADREFERENCIA cADREFERENCIA = db.CADREFERENCIAS.Find(codSitio, nom, cod);
                if (cADREFERENCIA == null)
                {
                    return HttpNotFound();
                }

                //ViewBag.nomSitio = nomSitio;
                ViewBag.NOMSITIO_GRABA = nomSitio;
                ViewBag.antiguoNombre = cADREFERENCIA.NOMBRE;
                ViewBag.antiguoCodigo = cADREFERENCIA.CODIGO;

                return View(cADREFERENCIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ReferenciasSitioGrabacion/Edit/5 To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SITIO_GRABA,NOMBRE,CODIGO,DESCRIPCION,ABREVIATURA")] CADREFERENCIA cADREFERENCIA)
        {
            try
            {
                var antiguoNombre = Request["antiguoNombre"].ToString();
                var antiguoCodigo = Request["antiguoCodigo"].ToString();
                var nomSitio = Request["NOMSITIO_GRABA"].ToString();

                if (antiguoNombre != cADREFERENCIA.NOMBRE || antiguoCodigo != cADREFERENCIA.CODIGO)
                {
                    var existe = db.CADREFERENCIAS.Find(cADREFERENCIA.SITIO_GRABA, cADREFERENCIA.NOMBRE, cADREFERENCIA.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("NOMBRE", "Este Nombre y Código esta siendo utilizado en otro registro.");
                }

                if (ModelState.IsValid)
                {
                    if (antiguoNombre == cADREFERENCIA.NOMBRE && antiguoCodigo == cADREFERENCIA.CODIGO)
                    {
                        db.Entry(cADREFERENCIA).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADREFERENCIA>(cADREFERENCIA, "CADREFERENCIAS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADREFERENCIA.SITIO_GRABA + " | NOMBRE:" + cADREFERENCIA.NOMBRE + " | CODIGO:" + cADREFERENCIA.CODIGO, TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADREFERENCIA cADREFERENCIA1 = db.CADREFERENCIAS.Find(cADREFERENCIA.SITIO_GRABA, antiguoNombre, antiguoCodigo);
                        db.CADREFERENCIAS.Remove(cADREFERENCIA1);
                        db.SaveChanges();

                        //cADREFERENCIA.NOMBRE = nuevoNombre;
                        //cADREFERENCIA.CODIGO = nuevoCodigo;

                        db.CADREFERENCIAS.Add(cADREFERENCIA);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADREFERENCIA>(cADREFERENCIA, "CADREFERENCIAS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADREFERENCIA.SITIO_GRABA + " | NOMBRE:" + cADREFERENCIA.NOMBRE + " | CODIGO:" + cADREFERENCIA.CODIGO, TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADREFERENCIA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = cADREFERENCIA.SITIO_GRABA, nom = nomSitio, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { id = cADREFERENCIA.SITIO_GRABA, nom = nomSitio });
                }

                ViewBag.NOMSITIO_GRABA = nomSitio;
                ViewBag.antiguoNombre = antiguoNombre;
                ViewBag.antiguoCodigo = antiguoCodigo;

                return View(cADREFERENCIA);
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

        [SecadPlusAutorizador("/ReferenciasSitioGrabacion/Eliminar")]
        public ActionResult Eliminar(int codSitio, string nomSitio, string nom, string cod)
        {
            try
            {
                if (cod == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADREFERENCIA cADREFERENCIA = db.CADREFERENCIAS.Find(codSitio, nom, cod);
                if (cADREFERENCIA == null)
                {
                    return HttpNotFound();
                }
                ViewBag.id = codSitio;
                ViewBag.nomSitio = nomSitio;
                return View("Delete", null, cADREFERENCIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ReferenciasSitioGrabacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int codSitio, string nomSitio, string nom, string cod)
        {
            try
            {
                CADREFERENCIA cADREFERENCIA = db.CADREFERENCIAS.Find(codSitio, nom, cod);
                db.CADREFERENCIAS.Remove(cADREFERENCIA);
                db.SaveChanges();



                CADREFERENCIA cADREFERENCIA_copy = new CADREFERENCIA();
                cADREFERENCIA_copy = (CADREFERENCIA)auditoria.GenerarCopiaAuditoria(cADREFERENCIA);

                auditoria.RegistrarAuditoria<CADREFERENCIA>(cADREFERENCIA_copy, "CADREFERENCIAS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADREFERENCIA.SITIO_GRABA + " | NOMBRE:" + cADREFERENCIA.NOMBRE + " | CODIGO:" + cADREFERENCIA.CODIGO, TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado la referencia: " + cADREFERENCIA.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { id = codSitio, nom = nomSitio, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", new { id = codSitio, nom = nomSitio });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { id = codSitio, nom = nomSitio, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult ObteneReferencias(string strSitioGraba, string descripcionSitio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_ReferenciasSitioGraba> listaReferencias = new List<Ext_ReferenciasSitioGraba>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                string strBotonDestino = "";
                string strBoton = "<ul class='icons-list'> " +
                            "<li class='dropdown'> " +
                                "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                    "<i class='icon-menu9'></i> " +
                                "</button> " +
                                "<ul class='dropdown-menu dropdown-menu-left'> " +
                                    "<li> " +
                                        "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                "onclick =\"location.href='_EDITAR'\"> " +
                                            "<i class='icon-trash'></i> Editar" +
                                        "</button>" +
                                    "</li>" +
                                     "<li> " +
                                        "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                "onclick=\"EliminarRegistroGeneral('" + Url.Action("Eliminar", "ReferenciasSitioGrabacion") + "?_ELIMINAR')\"> " +
                                            "<i class='icon-trash'></i> Eliminar" +
                                        "</button>" +
                                    "</li>" +

                                "</ul>" +
                            "</li>" +
                        "</ul>";

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (NOMBRE, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(ABREVIATURA, '" + parametros.ConsultaValor + "', 'i') ";

                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY NOMBRE";
                        break;

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

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", strSitioGraba, typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaReferenciaSitioGraba", strConexion, "Nonquery");

                foreach (DataRow fila in dttPerRelacionada.Rows)
                {
                    // strBotonDestino = strBoton.Replace("SITIO_GRABA", fila["SITIO_GRABA"].ToString()).Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());
                    strBotonDestino = strBoton.Replace("_EDITAR", (Url.Action("Edit", new { codSitio = fila["SITIO_GRABA"].ToString(), nomSitio = descripcionSitio, nom = fila["NOMBRE"].ToString(), cod = fila["CODIGO"].ToString() })).ToString()).
                        Replace("_ELIMINAR", "codSitio=" + fila["SITIO_GRABA"].ToString() + "&nomSitio=" + descripcionSitio + "&nom=" + fila["NOMBRE"].ToString() + "&cod=" + fila["CODIGO"].ToString());

                    listaReferencias.Add(new Ext_ReferenciasSitioGraba(fila, strBotonDestino));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaReferencias });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
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