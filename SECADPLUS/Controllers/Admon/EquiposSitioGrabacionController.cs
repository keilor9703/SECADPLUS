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
    public class EquiposSitioGrabacionController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: EquiposSitioGrabacion
        [SecadPlusAutorizador("/EquiposSitioGrabacion/Index")]
        public ActionResult Index(int? id, string nom, string strMensaje, string strTipo)
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

                var cADEQUIPOS = db.CADEQUIPOS.Include(c => c.CADSITIOS_GRABACION).Where(c => c.SITIO_GRABA == id);
                var cADCONEXIONs = db.CADCONEXIONs.Include(c => c.CADSITIOS_GRABACION).Include(c => c.CADEQUIPOS).Where(c => c.SITIO_GRABA == 0);

                var variosModelos = new Tuple<List<CADEQUIPOS>, List<CADCONEXION>>
                    (cADEQUIPOS.ToList(), cADCONEXIONs.ToList())
                { };

                ViewBag.idSitio = id;
                ViewBag.DESCRIPCION = nom;
                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetEquiposComputo(int id, string nom)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strFiltro = "WHERE SITIO_GRABA = " + id;

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
                                                            "onclick=\"window.location.href = '" + Url.Action("Edit", "EquiposSitioGrabacion") + "?codSitio=" + id + "&nomSitio=" + nom + "&nom=DESCRIPCION&id=IDENTIFICACION" + "'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar " +
                                                    "</button> " +
                                                "</li> " +
                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                            "onclick=\"EliminarRegistroGeneral('" + Url.Action("Eliminar", "EquiposSitioGrabacion") + "?codSitio=" + id + "&nomSitio=" + nom + "&nom=DESCRIPCION&id=IDENTIFICACION" + "')\"> " +
                                                        "<i class='icon-trash'></i> Eliminar " +
                                                    "</button> " +
                                                "</li> " +
                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para ver conexiones' " +
                                                            "onclick=\"VerConexiones(" + id + ", 'IDENTIFICACION')\"> " +
                                                        "<i class='icon-eye'></i> Ver conexiones " +
                                                    "</button> " +
                                                "</li> " +
                                            "</ul> " +
                                        "</li> " +
                                    "</ul> ";

                string strBotonDestino = "";
                List<_EquiposComputoModel> listaEquipos = new List<_EquiposComputoModel>();
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
                    strQuery = " AND (SITIO_GRABA = " + id + " AND REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (IDENTIFICACION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (ACD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (TNSNAMES60, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (UBICACION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (SERIE, '" + parametros.ConsultaValor + "', 'i')) ";
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
                        strQuery += " ORDER BY SITIO_GRABA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strQuery += " ORDER BY IDENTIFICACION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strQuery += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strQuery += " ORDER BY ACD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 6:
                        strQuery += " ORDER BY TNSNAMES60 " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 7:
                        strQuery += " ORDER BY UBICACION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 8:
                        strQuery += " ORDER BY SERIE " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_filtro", strFiltro, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_equipos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());
                    strBotonDestino = strBotonDestino.Replace("IDENTIFICACION", fila["IDENTIFICACION"].ToString());

                    listaEquipos.Add(new _EquiposComputoModel("", strBotonDestino, fila["SITIO_GRABA"].ToString(), fila["IDENTIFICACION"].ToString(), fila["DESCRIPCION"].ToString(), fila["ACD"].ToString(), fila["TNSNAMES60"].ToString(), fila["UBICACION"].ToString(), fila["SERIE"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                }
                if (blFiltro)
                {
                    parametros.TotalRegistroFiltrados = dttLugar.Rows.Count;
                }
                else
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEquipos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        public ActionResult GetConexionesEquipos(int codSitio, string id)
        {
            try
            {
                var cADCONEXIONs = db.CADCONEXIONs.Include(c => c.CADSITIOS_GRABACION).Include(c => c.CADEQUIPOS).Where(c => c.SITIO_GRABA == codSitio && c.MAQUINA == id).OrderByDescending(c => c.CONSECUTIVO);
                return PartialView("_resultadoConexiones", cADCONEXIONs.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult EditaConexionVigente(int codSitio, int id, bool checkVigente)
        {
            try
            {
                CADCONEXION cADCONEXION = db.CADCONEXIONs.Find(codSitio, id);

                if (cADCONEXION == null)
                {
                    return Json(false);
                }

                if (checkVigente)
                    cADCONEXION.VIGENTE = "S";
                else
                    cADCONEXION.VIGENTE = "N";

                db.Entry(cADCONEXION).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { result = true, datos = "Se actualizo vigente: " + checkVigente + ", el equipo: " + id });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: EquiposSitioGrabacion/Create
        [SecadPlusAutorizador("/EquiposSitioGrabacion/Create")]
        public ActionResult Create(int? id, string nom)
        {
            try
            {
                var acd = db.CADREFERENCIAS.SqlQuery("SELECT * FROM USR_CAD.CADREFERENCIAS " +
                                                     "WHERE SITIO_GRABA = " + id + " " +

                                                     "AND NOMBRE = 'ARCHIVO_ACD' " +
                                                     "AND CODIGO NOT IN (SELECT NVL(ACD,'-1') FROM USR_CAD.CADEQUIPOS " +
                                                     "WHERE SITIO_GRABA = " + id + ") ORDER BY DESCRIPCION").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION });
                ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION");

                //ViewBag.ACD = new SelectList(db.CADREFERENCIAS.Where(m => m.SITIO_GRABA == id && m.NOMBRE == "ARCHIVO_ACD").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

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

        // POST: EquiposSitioGrabacion/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, string nom, [Bind(Include = "SITIO_GRABA,IDENTIFICACION,DESCRIPCION,ACD,TNSNAMES60,TNSNAMES9I,UBICACION,SERIE")] CADEQUIPOS cADEQUIPOS)
        {
            try
            {
                CADEQUIPOS existe;
                existe = db.CADEQUIPOS.Find(cADEQUIPOS.SITIO_GRABA, cADEQUIPOS.IDENTIFICACION);

                if (existe != null)
                    ModelState.AddModelError("IDENTIFICACION", "Esta Identificación esta siendo utilizado en otro registro.");

                existe = db.CADEQUIPOS.SingleOrDefault(c => c.SITIO_GRABA == cADEQUIPOS.SITIO_GRABA && c.ACD == cADEQUIPOS.ACD);

                if (existe != null)
                    ModelState.AddModelError("ACD", "Esta referencia ACD esta siendo utilizado en otro registro.");

                existe = db.CADEQUIPOS.SingleOrDefault(c => c.SERIE == cADEQUIPOS.SERIE);

                if (existe != null)
                    ModelState.AddModelError("SERIE", "Esta Serie esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {
                    cADEQUIPOS.IDENTIFICACION = cADEQUIPOS.IDENTIFICACION.Trim().TrimStart().TrimEnd().ToUpper();
                    db.CADEQUIPOS.Add(cADEQUIPOS);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADEQUIPOS>(cADEQUIPOS, "CADEQUIPOS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADEQUIPOS.SITIO_GRABA + " | IDENTIFICACION:" + cADEQUIPOS.IDENTIFICACION, TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADEQUIPOS.IDENTIFICACION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { id = id, nom = nom });
                }

                //ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION", cADEQUIPOS.SITIO_GRABA);

                var acd = db.CADREFERENCIAS.SqlQuery("SELECT * FROM USR_CAD.CADREFERENCIAS " +
                                     "WHERE SITIO_GRABA = " + id + " " +
                                     "AND NOMBRE = 'ARCHIVO_ACD' " +
                                     "AND CODIGO NOT IN (SELECT NVL(ACD,'-1') FROM USR_CAD.CADEQUIPOS " +
                                     "WHERE SITIO_GRABA = " + id + ") ORDER BY DESCRIPCION").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION });
                ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION");
                if (!string.IsNullOrEmpty(cADEQUIPOS.ACD))
                {
                    ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION", cADEQUIPOS.ACD);
                }
                //ViewBag.ACD = new SelectList(db.CADREFERENCIAS.Where(m => m.SITIO_GRABA == id && m.NOMBRE == "ARCHIVO_ACD").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.SITIO_GRABA = id;
                ViewBag.NOM_GRABA = nom;
                return View(cADEQUIPOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: EquiposSitioGrabacion/Edit/5
        [SecadPlusAutorizador("/EquiposSitioGrabacion/Edit")]
        public ActionResult Edit(int codSitio, string nomSitio, string nom, string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADEQUIPOS cADEQUIPOS = db.CADEQUIPOS.Find(codSitio, id);
                if (cADEQUIPOS == null)
                {
                    return HttpNotFound();
                }

                var acd = db.CADREFERENCIAS.SqlQuery("SELECT * FROM USR_CAD.CADREFERENCIAS " +
                                     "WHERE SITIO_GRABA = " + codSitio + " " +
                                     "AND NOMBRE = 'ARCHIVO_ACD' " +
                                     "AND CODIGO NOT IN (SELECT NVL(ACD,'-1') FROM USR_CAD.CADEQUIPOS " +
                                     "WHERE SITIO_GRABA = " + codSitio + ") ORDER BY DESCRIPCION").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION });

                ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION");
                if (!string.IsNullOrEmpty(cADEQUIPOS.ACD))
                {
                    acd = db.CADREFERENCIAS.SqlQuery("SELECT * FROM USR_CAD.CADREFERENCIAS " +
                                                     "WHERE (CODIGO = '" + cADEQUIPOS.ACD + "' AND SITIO_GRABA = " + codSitio + " AND NOMBRE = 'ARCHIVO_ACD') " +
                                                     "OR (SITIO_GRABA = " + codSitio + " AND NOMBRE = 'ARCHIVO_ACD' " +
                                                     "AND CODIGO NOT IN (SELECT NVL(ACD,'-1') FROM USR_CAD.CADEQUIPOS " +
                                                     "WHERE SITIO_GRABA = " + codSitio + ")) ORDER BY DESCRIPCION").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION });
                    ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION", cADEQUIPOS.ACD);
                }

                //ViewBag.ACD = new SelectList(db.CADREFERENCIAS.Where(m => m.SITIO_GRABA == codSitio && m.NOMBRE == "ARCHIVO_ACD").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADEQUIPOS.ACD);
                ViewBag.SITIO_GRABA = codSitio;
                ViewBag.NOM_GRABA = nomSitio;
                ViewBag.antiguoIdentificacion = cADEQUIPOS.IDENTIFICACION;
                return View(cADEQUIPOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EquiposSitioGrabacion/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SITIO_GRABA,IDENTIFICACION,DESCRIPCION,ACD,TNSNAMES60,TNSNAMES9I,UBICACION,SERIE")] CADEQUIPOS cADEQUIPOS)
        {
            try
            {
                var antiguoIdentificacion = Request["antiguoIdentificacion"].ToString();
                var nomSitio = Request["NOM_GRABA"].ToString();

                CADEQUIPOS existe;

                if (antiguoIdentificacion != cADEQUIPOS.IDENTIFICACION)
                {
                    existe = db.CADEQUIPOS.Find(cADEQUIPOS.SITIO_GRABA, cADEQUIPOS.IDENTIFICACION);

                    if (existe != null)
                        ModelState.AddModelError("IDENTIFICACION", "Esta Identificación esta siendo utilizado en otro registro.");
                }

                existe = db.CADEQUIPOS.SingleOrDefault(c => c.SITIO_GRABA == cADEQUIPOS.SITIO_GRABA && c.IDENTIFICACION != antiguoIdentificacion && c.ACD == cADEQUIPOS.ACD);

                if (existe != null)
                    ModelState.AddModelError("ACD", "Esta referencia ACD esta siendo utilizado en otro registro.");

                existe = db.CADEQUIPOS.SingleOrDefault(c => c.SERIE == cADEQUIPOS.SERIE && c.IDENTIFICACION != antiguoIdentificacion);

                if (existe != null)
                    ModelState.AddModelError("SERIE", "Esta Serie esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {

                    if (antiguoIdentificacion == cADEQUIPOS.IDENTIFICACION)
                    {
                        cADEQUIPOS.IDENTIFICACION = cADEQUIPOS.IDENTIFICACION.Trim().TrimStart().TrimEnd().ToUpper();
                        db.Entry(cADEQUIPOS).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADEQUIPOS>(cADEQUIPOS, "CADEQUIPOS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADEQUIPOS.SITIO_GRABA + " | IDENTIFICACION:" + cADEQUIPOS.IDENTIFICACION, TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADEQUIPOS cADEQUIPOS1 = db.CADEQUIPOS.Find(cADEQUIPOS.SITIO_GRABA, antiguoIdentificacion);
                        db.CADEQUIPOS.Remove(cADEQUIPOS1);
                        db.SaveChanges();
                        cADEQUIPOS.IDENTIFICACION = cADEQUIPOS.IDENTIFICACION.Trim().TrimStart().TrimEnd().ToUpper();
                        //cADEQUIPOS.IDENTIFICACION = antiguoIdentificacion;                        
                        db.CADEQUIPOS.Add(cADEQUIPOS);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADEQUIPOS>(cADEQUIPOS, "CADEQUIPOS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADEQUIPOS.SITIO_GRABA + " | IDENTIFICACION:" + cADEQUIPOS.IDENTIFICACION, TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADEQUIPOS.IDENTIFICACION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = cADEQUIPOS.SITIO_GRABA, nom = nomSitio, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { id = cADEQUIPOS.SITIO_GRABA, nom = nomSitio });
                }
                var acd = db.CADREFERENCIAS.SqlQuery("SELECT * FROM USR_CAD.CADREFERENCIAS " +
                                                     "WHERE (CODIGO = '" + cADEQUIPOS.ACD + "' AND SITIO_GRABA = " + cADEQUIPOS.SITIO_GRABA + " AND NOMBRE = 'ARCHIVO_ACD') " +
                                                     "OR (SITIO_GRABA = " + cADEQUIPOS.SITIO_GRABA + " AND NOMBRE = 'ARCHIVO_ACD' " +
                                                     "AND CODIGO NOT IN (SELECT NVL(ACD,'-1') FROM USR_CAD.CADEQUIPOS " +
                                                     "WHERE SITIO_GRABA = " + cADEQUIPOS.SITIO_GRABA + ")) ORDER BY DESCRIPCION").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION });
                ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION");
                if (!string.IsNullOrEmpty(cADEQUIPOS.ACD))
                {
                    ViewBag.ACD = new SelectList(acd, "CODIGO", "DESCRIPCION", cADEQUIPOS.ACD);
                }

                //ViewBag.ACD = new SelectList(db.CADREFERENCIAS.Where(m => m.SITIO_GRABA == cADEQUIPOS.SITIO_GRABA && m.NOMBRE == "ARCHIVO_ACD").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADEQUIPOS.ACD);
                ViewBag.NOM_GRABA = nomSitio;
                ViewBag.antiguoIdentificacion = antiguoIdentificacion;

                return View(cADEQUIPOS);
            }
            catch (DbUpdateException bdEx)
            {
                var mensaje = bdEx.InnerException.Message.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                GestorBase.GestionarExcepcion(bdEx, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/EquiposSitioGrabacion/Eliminar")]
        public ActionResult Eliminar(int codSitio, string nomSitio, string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADEQUIPOS cADEQUIPOS = db.CADEQUIPOS.Find(codSitio, id);
                if (cADEQUIPOS == null)
                {
                    return HttpNotFound();
                }
                ViewBag.nomSitio = nomSitio;
                return View("Delete", null, cADEQUIPOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EquiposSitioGrabacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int codSitio, string nomSitio, string nom, string id)
        {
            try
            {
                CADEQUIPOS cADEQUIPOS = db.CADEQUIPOS.Find(codSitio, id);
                db.CADEQUIPOS.Remove(cADEQUIPOS);
                db.SaveChanges();


                CADEQUIPOS cADEQUIPOS_copy = new CADEQUIPOS();
                cADEQUIPOS_copy = (CADEQUIPOS)auditoria.GenerarCopiaAuditoria(cADEQUIPOS);

                auditoria.RegistrarAuditoria<CADEQUIPOS>(cADEQUIPOS_copy, "CADEQUIPOS", "LLAVE COMPUESTA: SITIO_GRABA:" + cADEQUIPOS.SITIO_GRABA + " | IDENTIFICACION:" + cADEQUIPOS.IDENTIFICACION, TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el equipo de computo: " + cADEQUIPOS.IDENTIFICACION.ToString();
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