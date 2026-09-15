using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
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
    public class PreguntasController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: Preguntas
        [SecadPlusAutorizador("/Preguntas/Index")]
        public ActionResult Index(string strMensaje, string strTipo)
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
                // return View(db.CADPREGUNTAS.ToList());


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetPreguntas()
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
                                                            "onclick=\"window.location.href = '../Preguntas/Edit?id=CODIGO'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar" +
                                                    "</button>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar'" +
                                                            "onclick=\"EliminarRegistroGeneral('../Preguntas/Eliminar?id=CODIGO')\"> " +
                                                        "<i class='icon-trash'></i> Eliminar" +
                                                    "</button>" +
                                                "</li>" +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";
                List<_PreguntasModel> listaPreguntas = new List<_PreguntasModel>();
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
                    strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') ";
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
                        strOrderna += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_preguntas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());

                    listaPreguntas.Add(new _PreguntasModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPreguntas });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: Preguntas/Create
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

        // POST: Preguntas/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,DESCRIPCION")] CADPREGUNTA cADPREGUNTA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mensaje = "";
                    var tipo = "";
                    var existe = db.CADPREGUNTAS.Where(p => p.DESCRIPCION.ToUpper() == cADPREGUNTA.DESCRIPCION.ToUpper()).Count();

                    if (existe > 0)
                    {
                        mensaje = "EL registro (" + cADPREGUNTA.DESCRIPCION.ToString() + ") ya existe.";
                        tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                    else
                    {
                        Int16 CODIGO = db.CADPREGUNTAS.Max(c => c.CODIGO);
                        CODIGO += 1;
                        cADPREGUNTA.CODIGO = CODIGO;
                        db.CADPREGUNTAS.Add(cADPREGUNTA);
                        db.SaveChanges();

                        mensaje = "Ha sido creado el registro: " + cADPREGUNTA.DESCRIPCION.ToString();
                        tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    }


                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                return View(cADPREGUNTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Preguntas/Edit/5
        public ActionResult Edit(short? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPREGUNTA cADPREGUNTA = db.CADPREGUNTAS.Find(id);
                if (cADPREGUNTA == null)
                {
                    return HttpNotFound();
                }
                return View(cADPREGUNTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Preguntas/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODIGO,DESCRIPCION")] CADPREGUNTA cADPREGUNTA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADPREGUNTA).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADPREGUNTA>(cADPREGUNTA, "CADPREGUNTAS", cADPREGUNTA.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editada la pregunta: " + cADPREGUNTA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }
                return View(cADPREGUNTA);
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

        public ActionResult Eliminar(short? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPREGUNTA cADPREGUNTA = db.CADPREGUNTAS.Find(id);
                if (cADPREGUNTA == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADPREGUNTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Preguntas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            try
            {
                CADPREGUNTA cADPREGUNTA = db.CADPREGUNTAS.Find(id);
                db.CADPREGUNTAS.Remove(cADPREGUNTA);
                db.SaveChanges();



                CADPREGUNTA cADPREGUNTA_copy = new CADPREGUNTA();
                cADPREGUNTA_copy = (CADPREGUNTA)auditoria.GenerarCopiaAuditoria(cADPREGUNTA);


                auditoria.RegistrarAuditoria<CADPREGUNTA>(cADPREGUNTA_copy, "CADPREGUNTAS", cADPREGUNTA.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminada la pregunta: " + cADPREGUNTA.DESCRIPCION.ToString();
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