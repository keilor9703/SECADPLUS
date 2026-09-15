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
    public class CasosMaestrasController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: CasosMaestras
        [SecadPlusAutorizador("/CasosMaestras/Index")]
        public ActionResult Index(string id, string nom, string strMensaje, string strTipo)
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

                if (string.IsNullOrEmpty(id))
                {
                    id = "";
                    nom = "";
                    //id = db.CADCASOS.OrderBy(c => c.CODIGO).Select(c => c.CODIGO).First().ToString(); //.Where(c => c.CODIGO == "935E")
                    //nom = db.CADCASOS.OrderBy(c => c.CODIGO).Select(c => c.DESCRIPCION).First().ToString(); //.Where(c => c.CODIGO == "935E")
                }

                //var cADCASOS = db.CADCASOS.SqlQuery("Select CODIGO, DESCRIPCION, IMPORTANCIA, " +
                //                                    "PK_CAD.f_referenciassecad('PRIORIDAD', PRIORIDAD) As PRIORIDAD, PK_CAD.f_referenciassecad('TIPO_USO_CODIGO', TIPO_USO) As TIPO_USO " +
                //                                    "From CADCASOS Order by CODIGO");

                var cADCASOS = db.CADCASOS.Where(c => c.CODIGO == "");

                var cADPREGUNTASCASOS = db.CADPREGUNTASCASOS.Include(c => c.CADCASO).Include(c => c.CADPREGUNTA).Where(c => c.CADCASO_CODIGO == id).OrderBy(c => c.ORDEN);

                var variosModelos = new Tuple<List<CADCASO>, List<CADPREGUNTASCASO>>
                    (cADCASOS.ToList(), cADPREGUNTASCASOS.ToList())
                { };

                ViewBag.COD_CASO = id;
                ViewBag.NOM_CASO = nom;



                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View(variosModelos);
                //return View(listado);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetCasos()
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
                                                            "onclick=\"window.location.href = '" + Url.Action("Edit", "CasosMaestras") + "?id=CODIGO'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar " +
                                                    "</button> " +
                                                "</li> " +
                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                            "onclick=\"EliminarRegistroGeneral('" + Url.Action("Eliminar", "CasosMaestras") + "?id=CODIGO')\"> " +
                                                        "<i class='icon-trash'></i> Eliminar" +
                                                    "</button>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para ver asociados'" +
                                                            "onclick=\"VerPreguntas('CODIGO', 'DESCRIPCION')\">" +
                                                        "<i class='icon-eye'></i> Ver asociados" +
                                                    "</button>" +
                                                "</li>" +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";
                List<_CasosModel> listaCasos = new List<_CasosModel>();
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
                               " OR REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (PRIORIDAD, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (IMPORTANCIA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (TIPO_USO, '" + parametros.ConsultaValor + "', 'i') ";
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

                    case 4:
                        strOrderna += " ORDER BY PRIORIDAD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY IMPORTANCIA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 6:
                        strOrderna += " ORDER BY TIPO_USO " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_casos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaCasos.Add(new _CasosModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString(), fila["PRIORIDAD"].ToString(), fila["IMPORTANCIA"].ToString(), fila["TIPO_USO"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCasos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        public ActionResult GetPreguntas(string id)
        {
            try
            {
                var cADPREGUNTASCASOS = db.CADPREGUNTASCASOS.Include(c => c.CADCASO).Include(c => c.CADPREGUNTA).Where(c => c.CADCASO_CODIGO == id).OrderBy(c => c.ORDEN);
                return PartialView("_preguntas", cADPREGUNTASCASOS.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult EditarVigente(string idCaso, int idPreg, bool checkVigente)
        {
            try
            {
                CADPREGUNTASCASO cADPREGUNTASCASO = db.CADPREGUNTASCASOS.Find(idCaso, idPreg);

                if (cADPREGUNTASCASO == null)
                {
                    return Json(false);
                }

                if (checkVigente)
                    cADPREGUNTASCASO.VIGENTE = "S";
                else
                    cADPREGUNTASCASO.VIGENTE = "N";

                db.Entry(cADPREGUNTASCASO).State = EntityState.Modified;
                db.SaveChanges();

                return Json(true);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        // GET: CasosMaestras/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.PRIORIDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "PRIORIDAD").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_USO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_USO_CODIGO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CasosMaestras/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,DESCRIPCION,PRIORIDAD,IMPORTANCIA,TIPO_USO")] CADCASO cADCASO)
        {
            try
            {
                var existe = db.CADCASOS.Find(cADCASO.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {
                    db.CADCASOS.Add(cADCASO);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCASO>(cADCASO, "CADCASOS", cADCASO.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADCASO.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.PRIORIDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "PRIORIDAD").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_USO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_USO_CODIGO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");

                return View(cADCASO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CasosMaestras/CreateP
        public ActionResult CreateP(string id, string nom)
        {
            try
            {
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.COD_CASO = id;
                ViewBag.NOM_CASO = nom;

                //var preguntas = db.CADPREGUNTAS.OrderBy(m => m.DESCRIPCION);

                var preguntas = db.CADPREGUNTAS.SqlQuery("SELECT CODIGO, DESCRIPCION FROM USR_CAD.CADPREGUNTAS " +
                                                         "WHERE CODIGO NOT IN " +
                                                         "(SELECT CADPREG_CODIGO FROM USR_CAD.CADPREGUNTASCASOS " +
                                                         "WHERE CADCASO_CODIGO = '" + id + "')");

                ViewBag.CADPREG_CODIGO = new SelectList(preguntas, "CODIGO", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CasosMaestras/CreateP To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateP(string id, string nom, [Bind(Include = "CADCASO_CODIGO,CADPREG_CODIGO,ORDEN,VIGENTE")] CADPREGUNTASCASO cADPREGUNTASCASO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cADPREGUNTASCASO.CADCASO_CODIGO = id;

                    if (cADPREGUNTASCASO.VIGENTE == "true")
                        cADPREGUNTASCASO.VIGENTE = "S";
                    else
                        cADPREGUNTASCASO.VIGENTE = "N";

                    db.CADPREGUNTASCASOS.Add(cADPREGUNTASCASO);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADPREGUNTASCASO>(cADPREGUNTASCASO, "CADPREGUNTASCASOS", cADPREGUNTASCASO.CADCASO_CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADPREGUNTASCASO.CADPREG_CODIGO.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { id = id, nom = nom });
                }

                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.COD_CASO = id;
                ViewBag.NOM_CASO = nom;

                var preguntas = db.CADPREGUNTAS.SqlQuery("SELECT CODIGO, DESCRIPCION FROM USR_CAD.CADPREGUNTAS " +
                                                         "WHERE CODIGO NOT IN " +
                                                         "(SELECT CADPREG_CODIGO FROM USR_CAD.CADPREGUNTASCASOS " +
                                                         "WHERE CADCASO_CODIGO = '" + id + "')");

                ViewBag.CADPREG_CODIGO = new SelectList(preguntas, "CODIGO", "DESCRIPCION");
                ViewBag.VIGENTE = cADPREGUNTASCASO.VIGENTE;
                return View(cADPREGUNTASCASO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CasosMaestras/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCASO cADCASO = db.CADCASOS.Find(id);
                if (cADCASO == null)
                {
                    return HttpNotFound();
                }

                ViewBag.antiguoCodigo = cADCASO.CODIGO;
                ViewBag.PRIORIDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "PRIORIDAD").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCASO.PRIORIDAD);
                ViewBag.TIPO_USO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_USO_CODIGO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCASO.TIPO_USO);

                return View(cADCASO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CasosMaestras/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODIGO,DESCRIPCION,PRIORIDAD,IMPORTANCIA,TIPO_USO")] CADCASO cADCASO)
        {
            try
            {
                var antiguoCodigo = Request["antiguoCodigo"];

                if (antiguoCodigo != cADCASO.CODIGO)
                {
                    var existe = db.CADCASOS.Find(cADCASO.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");
                }

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADCASO.CODIGO)
                    {
                        db.Entry(cADCASO).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCASO>(cADCASO, "CADCASOS", cADCASO.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADCASO cADCASO1 = db.CADCASOS.Find(antiguoCodigo);

                        db.CADCASOS.Remove(cADCASO1);
                        db.SaveChanges();

                        //cADCASO.CODIGO = antiguoCodigo;

                        db.CADCASOS.Add(cADCASO);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCASO>(cADCASO, "CADCASOS", cADCASO.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    var mensaje = "Ha sido editado el registro: " + cADCASO.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.antiguoCodigo = antiguoCodigo;

                ViewBag.PRIORIDAD = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "PRIORIDAD").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_USO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_USO_CODIGO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");

                return View(cADCASO);
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

        // GET: CasosMaestras/EditP/5
        public ActionResult EditP(string idCaso, int? idPreg, string nomCaso)
        {
            try
            {
                if (idPreg == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPREGUNTASCASO cADPREGUNTASCASO = db.CADPREGUNTASCASOS.Find(idCaso, idPreg);
                if (cADPREGUNTASCASO == null)
                {
                    return HttpNotFound();
                }

                var preguntas = db.CADPREGUNTAS.SqlQuery("SELECT CODIGO, DESCRIPCION FROM USR_CAD.CADPREGUNTAS " +
                                                         "WHERE CODIGO = " + idPreg + " OR CODIGO  NOT IN " +
                                                         "(SELECT CADPREG_CODIGO FROM USR_CAD.CADPREGUNTASCASOS " +
                                                         "WHERE CADCASO_CODIGO = '" + idCaso + "')");

                //ViewBag.nuevoCodigo = new SelectList(preguntas, "CODIGO", "DESCRIPCION", cADPREGUNTASCASO.CADPREG_CODIGO);
                ViewBag.antiguoCodigo = cADPREGUNTASCASO.CADPREG_CODIGO;

                ViewBag.CADCASO_CODIGO = idCaso;
                ViewBag.CADPREG_CODIGO = new SelectList(preguntas, "CODIGO", "DESCRIPCION", cADPREGUNTASCASO.CADPREG_CODIGO);
                ViewBag.idCaso = idCaso;
                ViewBag.nomCaso = nomCaso;
                ViewBag.NOM_CASO = nomCaso;
                ViewBag.VIGENTE = cADPREGUNTASCASO.VIGENTE == "S" ? true : false;
                return View(cADPREGUNTASCASO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CasosMaestras/EditP/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditP(string idCaso, string nomCaso, [Bind(Include = "CADCASO_CODIGO,CADPREG_CODIGO,ORDEN,VIGENTE")] CADPREGUNTASCASO cADPREGUNTASCASO)
        {
            try
            {
                var antiguoCodigo = int.Parse(Request["antiguoCodigo"]);

                if (cADPREGUNTASCASO.VIGENTE == "true")
                    cADPREGUNTASCASO.VIGENTE = "S";
                else
                    cADPREGUNTASCASO.VIGENTE = "N";

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADPREGUNTASCASO.CADPREG_CODIGO)
                    {
                        db.Entry(cADPREGUNTASCASO).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADPREGUNTASCASO>(cADPREGUNTASCASO, "CADPREGUNTASCASOS", cADPREGUNTASCASO.CADCASO_CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADPREGUNTASCASO cADPREGUNTASCASO1 = db.CADPREGUNTASCASOS.Find(cADPREGUNTASCASO.CADCASO_CODIGO, antiguoCodigo);
                        db.CADPREGUNTASCASOS.Remove(cADPREGUNTASCASO1);
                        db.SaveChanges();

                        //cADPREGUNTASCASO.CADPREG_CODIGO = nuevoCodigo;

                        db.CADPREGUNTASCASOS.Add(cADPREGUNTASCASO);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADPREGUNTASCASO>(cADPREGUNTASCASO, "CADPREGUNTASCASOS", cADPREGUNTASCASO.CADCASO_CODIGO.ToString(), TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADPREGUNTASCASO.CADPREG_CODIGO.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = idCaso, nom = nomCaso, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { id = idCaso, nom = nomCaso });
                }
                var preguntas = db.CADPREGUNTAS.SqlQuery("SELECT CODIGO, DESCRIPCION FROM USR_CAD.CADPREGUNTAS " +
                                                         "WHERE CODIGO = " + cADPREGUNTASCASO.CADPREG_CODIGO + " OR CODIGO  NOT IN " +
                                                         "(SELECT CADPREG_CODIGO FROM USR_CAD.CADPREGUNTASCASOS " +
                                                         "WHERE CADCASO_CODIGO = '" + idCaso + "')");

                //ViewBag.nuevoCodigo = new SelectList(preguntas, "CODIGO", "DESCRIPCION", cADPREGUNTASCASO.CADPREG_CODIGO);

                ViewBag.antiguoCodigo = antiguoCodigo;

                ViewBag.CADCASO_CODIGO = idCaso;
                ViewBag.CADPREG_CODIGO = new SelectList(preguntas, "CODIGO", "DESCRIPCION", cADPREGUNTASCASO.CADPREG_CODIGO);
                ViewBag.idCaso = idCaso;
                ViewBag.nomCaso = nomCaso;
                ViewBag.NOM_CASO = nomCaso;
                ViewBag.VIGENTE = cADPREGUNTASCASO.VIGENTE;

                return View(cADPREGUNTASCASO);
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

        public ActionResult Eliminar(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCASO cADCASO = db.CADCASOS.Find(id);
                if (cADCASO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.PRIORIDAD = db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "PRIORIDAD" && m.CODIGO == cADCASO.PRIORIDAD).Select(m => m.DESCRIPCION).First();
                ViewBag.TIPO_USO = db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_USO_CODIGO" && m.CODIGO == cADCASO.TIPO_USO).Select(m => m.DESCRIPCION).First();

                return View("Delete", null, cADCASO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CasosMaestras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                CADCASO cADCASO = db.CADCASOS.Find(id);
                db.CADCASOS.Remove(cADCASO);
                db.SaveChanges();


                CADCASO cADCASO_copy = new CADCASO();
                cADCASO_copy = (CADCASO)auditoria.GenerarCopiaAuditoria(cADCASO);


                auditoria.RegistrarAuditoria<CADCASO>(cADCASO_copy, "CADCASOS", cADCASO.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el caso: " + cADCASO.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EliminarP(string idCaso, int? idPreg, string nomCaso)
        {
            try
            {
                if (idPreg == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPREGUNTASCASO cADPREGUNTASCASO = db.CADPREGUNTASCASOS.Find(idCaso, idPreg);
                if (cADPREGUNTASCASO == null)
                {
                    return HttpNotFound();
                }

                ViewBag.CADCASO_CODIGO = idCaso;
                ViewBag.CADPREG_CODIGO = idPreg;
                ViewBag.NOM_CASO = nomCaso;
                ViewBag.idCaso = idCaso;
                ViewBag.nomCaso = nomCaso;

                return View("DeleteP", null, cADPREGUNTASCASO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CasosMaestras/DeleteP/5
        [HttpPost, ActionName("DeleteP")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedP(string idCaso, int? idPreg, string nomCaso)
        {
            try
            {
                CADPREGUNTASCASO cADPREGUNTASCASO = db.CADPREGUNTASCASOS.Find(idCaso, idPreg);
                db.CADPREGUNTASCASOS.Remove(cADPREGUNTASCASO);
                db.SaveChanges();


                CADPREGUNTASCASO cADPREGUNTASCASO_copy = new CADPREGUNTASCASO();
                cADPREGUNTASCASO_copy = (CADPREGUNTASCASO)auditoria.GenerarCopiaAuditoria(cADPREGUNTASCASO);

                auditoria.RegistrarAuditoria<CADPREGUNTASCASO>(cADPREGUNTASCASO_copy, "CADPREGUNTASCASOS", cADPREGUNTASCASO.CADCASO_CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el caso: " + cADPREGUNTASCASO.CADPREG_CODIGO.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { id = idCaso, nom = nomCaso, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
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