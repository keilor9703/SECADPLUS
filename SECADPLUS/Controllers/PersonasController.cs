using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADEntities.SecadLugaresGeograficos;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class PersonasController : Controller
    {
        private EntitiesComunasBarrios db = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private SecadEntities db2 = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorGeneral objGestorGral;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: Personas
        [SecadPlusAutorizador("/Personas/Index")]
        public ActionResult Index(int? pagina, string message = null)
        {
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
            var grados = (from t in db.CADGRADOS where t.DESCRIPCION != "PENDIENTE POR DEFINIR...." orderby t.DESCRIPCION select new { t.ALFABETICO, t.DESCRIPCION }).ToList();
            grados.Add(new { ALFABETICO = "-1", DESCRIPCION = "Seleccione una opción" });
            ViewBag.GRAD_ALFABETICO = new SelectList(grados, "ALFABETICO", "DESCRIPCION", -1);

            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarPersonas(string gradoAlfabetico = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPersonas;
                string strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-xs border-success text-success-600 btn-flat btn-tabla dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default  btn-tabla' type='button' onclick=\"location.href = 'urlModificaPersona'\"><i class='icon-pencil'></i> Editar</button></li><li><button class='btn btn-default  btn-tabla' type='button' onclick=\"EliminarElemento('urlBorraPersona' )\")><i class='icon-trash'></i> Borrar</button></li></ul></li></ul></td>";
                List<Ext_VPERSONAS> listaPersonas = new List<Ext_VPERSONAS>();
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
                        strQuery = " WHERE (PERSONA.CEDU_EMPLEADO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (upper(NOMB_EMPLEADO) LIKE upper('%" + parametros.ConsultaValor +
                            "%') OR upper(DESCRIPCIONTIPOFUERZA) LIKE upper('%" + parametros.ConsultaValor + "%'))";
                    }
                    if (gradoAlfabetico != null && gradoAlfabetico != "-1")
                    {
                        strQuery += " AND GRAD_ALFABETICO ='" + gradoAlfabetico + "'";
                    }
                }
                else
                {
                    if (gradoAlfabetico != null && gradoAlfabetico != "-1")
                    {
                        strQuery += " WHERE GRAD_ALFABETICO ='" + gradoAlfabetico + "'";
                    }
                }

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY PERSONA.CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY PERSONA.NOMB_EMPLEADO " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttPersonas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaPersonas", strConexion, "Nonquery");

                foreach (DataRow fila in dttPersonas.Rows)
                {
                    //Control del páginado de registros
                    listaPersonas.Add(new Ext_VPERSONAS(fila, strBoton.Replace("@item.CEDU_EMPLEADO",
                        fila["CEDU_EMPLEADO"].ToString()).Replace("urlModificaPersona",
                        Url.Action("Edit", "Personas", new { id = fila["CEDU_EMPLEADO"].ToString() }).ToString()).Replace("urlBorraPersona",
                            Url.Action("Eliminar", "Personas", new { id = fila["CEDU_EMPLEADO"].ToString() }).ToString())));

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

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADPOLICIA cADPOLICIAS = await db.CADPOLICIAS.FindAsync(id);
            if (cADPOLICIAS == null)
            {
                return HttpNotFound();
            }
            return View(cADPOLICIAS);
        }

        // GET: Personas/Create
        [SecadPlusAutorizador("/Personas/Create")]
        public ActionResult Create()
        {
            if (Session["USUARIO_SECAD"] == null)
            {
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                return RedirectToAction("Index", "Home");
            }
            CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];

            ViewBag.TIPO_SEXO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.GRUP_SANGUINEO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "GRUPO_SANGUINEO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.ESTA_CIVIL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ESTACIVIL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.CODI_ESPECIALIDAD = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "ESPECIALIDAD" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.ESTA_LABORAL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_LABORAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.NUME_SECCION = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "SECCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.CADUNID_FUERZA = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
            ViewBag.CADUNID_CODIGO = string.Empty;
            ViewBag._UNIDAD = string.Empty;
            ViewBag.CADFUERZ_ID = new SelectList(ObtenerFuerzasPorUsuario(usuarioActual), "ID", "DESCRIPCION");
            ViewBag.GRAD_ALFABETICO = new SelectList(db.CADGRADOS.OrderBy(t => t.DESCRIPCION), "ALFABETICO", "DESCRIPCION");

            return View();
        }

        private IEnumerable ObtenerFuerzasPorUsuario(CADUSUARIO usuarioActual)
        {
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canalFuerzaId", usuarioActual.CADCANA_FUERZA_ID.ToString(), typeof(string)));
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_condicion", usuarioActual.CONDICION, typeof(string)));
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttFuerzas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_entidadesPorUsuarioActivo", strConexion, "Nonquery");
            var arrFuerzas = (from t in dttFuerzas.AsEnumerable() select new { ID = t["CODIGO"], DESCRIPCION = t["DESCRIPCION"] }).ToArray();
            return arrFuerzas;
        }

        // POST: Personas/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SecadPlusAutorizador("/Personas/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CEDU_EMPLEADO,NOMB_EMPLEADO,NUME_PLACA,DIRE_RESIDENCIA,TELE_RESIDENCIA,FECH_NACIMIENTO,TIPO_SEXO,GRUP_SANGUINEO,ESTA_CIVIL,CODI_ESPECIALIDAD,ESTA_LABORAL,NUME_SECCION,CADUNID_FUERZA,CADUNID_CODIGO,GRAD_ALFABETICO,CADFUERZ_ID")] CADPOLICIA cADPOLICIA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if ((from t in db.CADPOLICIAS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t).Count() > 0)
                    {
                        ViewBag.MENSAJE_EXITOSO = "El registro ya existe para este número de cédula";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    }
                    else
                    {
                        db.CADPOLICIAS.Add(cADPOLICIA);

                        foreach (string upload in Request.Files)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Request.Files[upload].InputStream.CopyTo(ms);
                                if (ms.Length > 0)
                                {
                                    Bitmap mifoto = new Bitmap(ms);
                                    string rutaArchivo = Redimensionar(mifoto, "tmp");

                                    using (MemoryStream ms2 = new MemoryStream())
                                    {
                                        byte[] data;
                                        data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                        if (data.Length > 0 && data.Length < 3500)
                                        {
                                            System.IO.File.Delete(Server.MapPath("~/" + rutaArchivo));
                                            CADPOLICIAS_FOTOS cADPOLICIA_FOTO = new CADPOLICIAS_FOTOS();
                                            cADPOLICIA_FOTO.CEDU_EMPLEADO = cADPOLICIA.CEDU_EMPLEADO;
                                            cADPOLICIA_FOTO.FOTO = data;
                                            if ((from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t.FOTO).ToArray().Count() > 0)
                                                db.Entry(cADPOLICIA_FOTO).State = EntityState.Modified;
                                            else
                                                db.CADPOLICIAS_FOTOS.Add(cADPOLICIA_FOTO);
                                        }
                                        else
                                        {
                                            ViewBag.MENSAJE_EXITOSO = "Registro fue guardado, el tamaño de la foto NO permite ser guardada";
                                            ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    ViewBag.MENSAJE_EXITOSO = "Registro fue guardado, la foto NO fue seleccionada regrese y use editar para modificarla";
                                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                }
                            }
                        }
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADPOLICIA.CEDU_EMPLEADO) });
                    }
                }
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.TIPO_SEXO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                ViewBag.GRUP_SANGUINEO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "GRUPO_SANGUINEO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                ViewBag.ESTA_CIVIL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ESTACIVIL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                ViewBag.CODI_ESPECIALIDAD = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "ESPECIALIDAD" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                ViewBag.ESTA_LABORAL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_LABORAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                ViewBag.NUME_SECCION = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "SECCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                ViewBag.CADUNID_FUERZA = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion");
                if (cADPOLICIA.CADUNID_CODIGO != null)
                    ViewBag._UNIDAD = db2.CADUNIDADES.Where(t => t.CODIGO == cADPOLICIA.CADUNID_CODIGO).FirstOrDefault().DESCRIPCION;
                else ViewBag._UNIDAD = string.Empty;
                ViewBag.CADFUERZ_ID = new SelectList(ObtenerFuerzasPorUsuario(usuarioActual), "ID", "DESCRIPCION", cADPOLICIA.CADFUERZ_ID);
                //ViewBag.CADFUERZ_ID = new SelectList(db.CADFUERZAS.Where(t => t.ID == usuarioActual.CADCANA_FUERZA_ID), "ID", "DESCRIPCION", usuarioActual.CADCANA_FUERZA_ID);
                ViewBag.GRAD_ALFABETICO = new SelectList(db.CADGRADOS, "ALFABETICO", "DESCRIPCION", cADPOLICIA.GRAD_ALFABETICO);
                byte[] data1 = (from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t.FOTO).FirstOrDefault();
                if (data1 != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data1, cADPOLICIA.CEDU_EMPLEADO));
                }
                return View(cADPOLICIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Personas/Edit/5
        [SecadPlusAutorizador("/Personas/Edit")]
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPOLICIA cADPOLICIA = await db.CADPOLICIAS.FindAsync(id);
                if (cADPOLICIA == null)
                {
                    return HttpNotFound();
                }
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.TIPO_SEXO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.TIPO_SEXO);
                ViewBag.GRUP_SANGUINEO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "GRUPO_SANGUINEO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.GRUP_SANGUINEO);
                ViewBag.ESTA_CIVIL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ESTACIVIL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.ESTA_CIVIL);
                ViewBag.CODI_ESPECIALIDAD = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "ESPECIALIDAD" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.CODI_ESPECIALIDAD);
                ViewBag.ESTA_LABORAL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_LABORAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.ESTA_LABORAL);
                ViewBag.NUME_SECCION = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "SECCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.NUME_SECCION);
                ViewBag.CADUNID_FUERZA = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.CADUNID_FUERZA);
                if (cADPOLICIA.CADUNID_CODIGO != null)
                    ViewBag._UNIDAD = db2.CADUNIDADES.Where(t => t.CODIGO == cADPOLICIA.CADUNID_CODIGO).FirstOrDefault().DESCRIPCION;
                else ViewBag._UNIDAD = string.Empty;
                ViewBag.CADFUERZ_ID = new SelectList(ObtenerFuerzasPorUsuario(usuarioActual), "ID", "DESCRIPCION", cADPOLICIA.CADFUERZ_ID);
                ViewBag.GRAD_ALFABETICO = new SelectList(db.CADGRADOS, "ALFABETICO", "DESCRIPCION", cADPOLICIA.GRAD_ALFABETICO);

                CADPOLICIAS_FOTOS cADFOTO = db.CADPOLICIAS_FOTOS.Find(cADPOLICIA.CEDU_EMPLEADO);
                byte[] data = (from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t.FOTO).FirstOrDefault();
                if (data != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data, cADPOLICIA.CEDU_EMPLEADO));
                }
                else
                {
                    ViewBag.foto = Url.Content("~/images/Avatar.png");
                }
                return View(cADPOLICIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Personas/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/Personas/Edit")]
        public async Task<ActionResult> Edit([Bind(Include = "CEDU_EMPLEADO,NOMB_EMPLEADO,NUME_PLACA,DIRE_RESIDENCIA,TELE_RESIDENCIA,FECH_NACIMIENTO,TIPO_SEXO,GRUP_SANGUINEO,ESTA_CIVIL,CODI_ESPECIALIDAD,ESTA_LABORAL,NUME_SECCION,CADUNID_FUERZA,CADUNID_CODIGO,GRAD_ALFABETICO,CADFUERZ_ID")] CADPOLICIA cADPOLICIA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADPOLICIA).State = EntityState.Modified;

                    auditoria.RegistrarAuditoria<CADPOLICIA>(cADPOLICIA, "CADPOLICIA", "CEDU_EMPLEADO=" + cADPOLICIA.CEDU_EMPLEADO.ToString(), TipoAuditoria.Modificar);



                    ViewBag.MENSAJE_EXITOSO = "Registro fue guardado, la foto NO fue seleccionada";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    //HttpPostedFile filePosted = Request.Files["uploader"];
                    foreach (string upload in Request.Files)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Request.Files[upload].InputStream.CopyTo(ms);
                            if (ms.Length > 0)
                            {
                                Bitmap mifoto = new Bitmap(ms);
                                string rutaArchivo = Redimensionar(mifoto, "tmp");

                                using (MemoryStream ms2 = new MemoryStream())
                                {
                                    byte[] data;
                                    data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                    if (data.Length > 0 && data.Length < 3500)
                                    {
                                        System.IO.File.Delete(Server.MapPath("~/" + rutaArchivo));
                                        CADPOLICIAS_FOTOS cADPOLICIA_FOTO = new CADPOLICIAS_FOTOS();
                                        cADPOLICIA_FOTO.CEDU_EMPLEADO = cADPOLICIA.CEDU_EMPLEADO;
                                        cADPOLICIA_FOTO.FOTO = data;
                                        if ((from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t.FOTO).ToArray().Count() > 0)
                                        {
                                            db.Entry(cADPOLICIA_FOTO).State = EntityState.Modified;

                                            ViewBag.MENSAJE_EXITOSO = "Registro fue guardado, cambiando la fotografía";
                                            ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                        }
                                        else
                                        {
                                            db.CADPOLICIAS_FOTOS.Add(cADPOLICIA_FOTO);
                                            ViewBag.MENSAJE_EXITOSO = "Registro fue guardado,adicionando la fotografía";
                                            ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.MENSAJE_EXITOSO = "Registro fue guardado, el tamaño de la foto NO permite ser guardada";
                                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.MENSAJE_EXITOSO = "Registro fue guardado, la foto NO fue seleccionada";
                                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                            }
                        }
                    }
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", new { message = ViewBag.MENSAJE_EXITOSO });
                }
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.TIPO_SEXO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.TIPO_SEXO);
                ViewBag.GRUP_SANGUINEO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "GRUPO_SANGUINEO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.GRUP_SANGUINEO);
                ViewBag.ESTA_CIVIL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ESTACIVIL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.ESTA_CIVIL);
                ViewBag.CODI_ESPECIALIDAD = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "ESPECIALIDAD" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.CODI_ESPECIALIDAD);
                ViewBag.ESTA_LABORAL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_LABORAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.ESTA_LABORAL);
                ViewBag.NUME_SECCION = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "SECCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.NUME_SECCION);
                ViewBag.CADUNID_FUERZA = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.CADUNID_FUERZA);
                if (cADPOLICIA.CADUNID_CODIGO != null)
                    ViewBag._UNIDAD = db2.CADUNIDADES.Where(t => t.CODIGO == cADPOLICIA.CADUNID_CODIGO).FirstOrDefault().DESCRIPCION;
                else ViewBag._UNIDAD = string.Empty;
                ViewBag.CADFUERZ_ID = new SelectList(ObtenerFuerzasPorUsuario(usuarioActual), "ID", "DESCRIPCION", cADPOLICIA.CADFUERZ_ID);
                ViewBag.GRAD_ALFABETICO = new SelectList(db.CADGRADOS, "ALFABETICO", "DESCRIPCION", cADPOLICIA.GRAD_ALFABETICO);
                byte[] data1 = (from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t.FOTO).FirstOrDefault();
                if (data1 != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data1, cADPOLICIA.CEDU_EMPLEADO));
                }
                else
                {
                    ViewBag.foto = Url.Content("~/images/Avatar.png");
                }
                return View(cADPOLICIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Personas/Delete/5
        [Authorize]
        public ActionResult Eliminar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADPOLICIA cADPOLICIAS = db.CADPOLICIAS.Find(id);
            if (cADPOLICIAS == null)
            {
                return HttpNotFound();
            }
            return View("Delete", null, cADPOLICIAS);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [SecadPlusAutorizador("/Personas/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                CADPOLICIA cADPOLICIAS = await db.CADPOLICIAS.FindAsync(id);
                CADPOLICIAS_FOTOS cADFOTO = db.CADPOLICIAS_FOTOS.Where(t => t.CEDU_EMPLEADO == cADPOLICIAS.CEDU_EMPLEADO).FirstOrDefault();
                if (cADFOTO != null)
                    db.CADPOLICIAS_FOTOS.Remove(cADFOTO);
                db.CADPOLICIAS.Remove(cADPOLICIAS);
                await db.SaveChangesAsync();


                CADPOLICIA cADPOLICIAS_copy = new CADPOLICIA();
                cADPOLICIAS_copy = (CADPOLICIA)auditoria.GenerarCopiaAuditoria(cADPOLICIAS);

                auditoria.RegistrarAuditoria<CADPOLICIA>(cADPOLICIAS_copy, "CADPOLICIA", "CEDU_EMPLEADO=" + cADPOLICIAS.CEDU_EMPLEADO.ToString(), TipoAuditoria.Eliminar);



                return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DBConcurrencyException ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                ViewBag.MENSAJE_EXITOSO = "No fue borrado el registro";
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                return RedirectToAction("Index", new { message = "No fue borrado el registro" });
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

        [Authorize]
        public JsonResult BuscaUnidadesPorFuerza(int idFuerza)
        {
            return Json(from t in db2.CADUNIDADES where t.FUERZA == idFuerza select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION });
        }

        //FUNCION PARA CONVERTIR DE BYTES A IMAGEN
        public string Bytes_A_Imagen(byte[] ImgBytes, string cedula)
        {
            Bitmap imagen = null;
            try
            {
                Byte[] bytes = (Byte[])(ImgBytes);
                MemoryStream ms = new MemoryStream(bytes);
                imagen = new Bitmap(ms);
                string rutaFinal = Redimensionar(imagen, cedula);
                imagen.Dispose();
                return rutaFinal;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        public string Redimensionar(Image Imagen_Original, string nombre)
        {
            try
            {
                //RUTA DEL DIRECTORIO TEMPORAL
                //String DirTemp = Path.GetTempPath() + @"~\Images\" + nombre + ".jpg";
                String DirTemp = "~/Images/Personas/" + nombre + ".jpg";
                //IMAGEN ORIGINAL A REDIMENSIONAR
                Bitmap imagen = new Bitmap(Imagen_Original);
                //CREAMOS UN MAPA DE BIT CON LAS DIMENSIONES QUE QUEREMOS PARA LA NUEVA IMAGEN
                Bitmap nuevaImagen = new Bitmap(150, 200);
                //CREAMOS UN NUEVO GRAFICO
                Graphics gr = Graphics.FromImage(nuevaImagen);
                //DIBUJAMOS LA NUEVA IMAGEN
                gr.DrawImage(imagen, 0, 0, 150, 200);
                //LIBERAMOS RECURSOS
                gr.Dispose();

                //GUARDAMOS LA NUEVA IMAGEN ESPECIFICAMOS LA RUTA Y EL FORMATO
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter;
                System.Drawing.Imaging.Encoder myEncoder;
                ImageCodecInfo myImageCodecInfo;
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameter = new EncoderParameter(myEncoder, 25L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                nuevaImagen.Save(Server.MapPath(DirTemp), myImageCodecInfo, myEncoderParameters);
                //LIBERAMOS RECURSOS
                nuevaImagen.Dispose();
                imagen.Dispose();
                return DirTemp.Replace("~", "");
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            try
            {
                int j;
                ImageCodecInfo[] encoders;
                encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType == mimeType)
                        return encoders[j];
                }
                return null;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        [Authorize]
        public JsonResult GetUnidades(int id)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "", strFiltro = " WHERE FUERZA = " + id;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorUnidad(CODIGO,'DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_UnidadesModel> listaUnidades = new List<_UnidadesModel>();
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
                    strQuery = " AND (FUERZA = " + id + " AND REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (FUERZA = " + id + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) ";
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
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_filtro", strFiltro, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_unidades", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaUnidades.Add(new _UnidadesModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaUnidades });
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
        public JsonResult GetUnidadesByFuerza(int? id)
        {
            try
            {
                StringBuilder opciones = new StringBuilder();

                var unidades = db2.CADUNIDADES.Where(m => m.FUERZA == id).Select(m => new { CODIGO = m.CODIGO, DESCRIPCION = m.DESCRIPCION }).OrderBy(m => m.DESCRIPCION).ToList();

                if (unidades.Count == 0)
                {
                    unidades.Insert(0, new { CODIGO = 0, DESCRIPCION = " " });
                }

                for (int i = 0; i < unidades.Count(); i++)
                {
                    opciones.AppendFormat("<option value = '{0}'  > {1} </option>", unidades[i].CODIGO, unidades[i].DESCRIPCION);
                }

                return Json(opciones.ToString());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Botón traer datos ponal
        /// </summary>
        /// <param name="strCedula"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult TraeDatosPonal(string strCedula)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADPOLICIA cADPOLICIA = new CADPOLICIA();
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                GestorGeneral objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttPersona = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actualiza_persona_por_cedula", strConexion, "Nonquery");

                if (dttPersona.Rows.Count > 0)
                {
                    cADPOLICIA.CEDU_EMPLEADO = string.IsNullOrEmpty(dttPersona.Rows[0]["CEDU_EMPLEADO"].ToString()) ? "" : dttPersona.Rows[0]["CEDU_EMPLEADO"].ToString();
                    cADPOLICIA.NOMB_EMPLEADO = string.IsNullOrEmpty(dttPersona.Rows[0]["NOMB_EMPLEADO"].ToString()) ? "" : dttPersona.Rows[0]["NOMB_EMPLEADO"].ToString();
                    cADPOLICIA.NUME_PLACA = int.Parse(string.IsNullOrEmpty(dttPersona.Rows[0]["NUME_PLACA"].ToString()) ? 0.ToString() : dttPersona.Rows[0]["NUME_PLACA"].ToString());
                    cADPOLICIA.DIRE_RESIDENCIA = string.IsNullOrEmpty(dttPersona.Rows[0]["DIRE_RESIDENCIA"].ToString()) ? "" : dttPersona.Rows[0]["DIRE_RESIDENCIA"].ToString();
                    cADPOLICIA.TELE_RESIDENCIA = string.IsNullOrEmpty(dttPersona.Rows[0]["TELE_RESIDENCIA"].ToString()) ? "" : dttPersona.Rows[0]["TELE_RESIDENCIA"].ToString();
                    cADPOLICIA.FECH_NACIMIENTO = DateTime.Parse(string.IsNullOrEmpty(dttPersona.Rows[0]["FECH_NACIMIENTO"].ToString()) ? null : dttPersona.Rows[0]["FECH_NACIMIENTO"].ToString());
                    cADPOLICIA.TIPO_SEXO = string.IsNullOrEmpty(dttPersona.Rows[0]["TIPO_SEXO"].ToString()) ? "" : dttPersona.Rows[0]["TIPO_SEXO"].ToString();
                    cADPOLICIA.GRUP_SANGUINEO = string.IsNullOrEmpty(dttPersona.Rows[0]["GRUP_SANGUINEO"].ToString()) ? "" : dttPersona.Rows[0]["GRUP_SANGUINEO"].ToString();
                    cADPOLICIA.ESTA_CIVIL = string.IsNullOrEmpty(dttPersona.Rows[0]["ESTA_CIVIL"].ToString()) ? "" : dttPersona.Rows[0]["ESTA_CIVIL"].ToString();
                    cADPOLICIA.CODI_ESPECIALIDAD = string.IsNullOrEmpty(dttPersona.Rows[0]["CODI_ESPECIALIDAD"].ToString()) ? "" : dttPersona.Rows[0]["CODI_ESPECIALIDAD"].ToString();
                    cADPOLICIA.ESTA_LABORAL = string.IsNullOrEmpty(dttPersona.Rows[0]["ESTA_LABORAL"].ToString()) ? "" : dttPersona.Rows[0]["ESTA_LABORAL"].ToString();
                    cADPOLICIA.GRAD_ALFABETICO = string.IsNullOrEmpty(dttPersona.Rows[0]["GRAD_ALFABETICO"].ToString()) ? "" : cADPOLICIA.GRAD_ALFABETICO;
                    cADPOLICIA.CADUNID_FUERZA = byte.Parse(string.IsNullOrEmpty(dttPersona.Rows[0]["CADUNID_FUERZA"].ToString()) ? 0.ToString() : dttPersona.Rows[0]["CADUNID_FUERZA"].ToString());
                }
                else
                {
                    cADPOLICIA = (from t in db.CADPOLICIAS where t.CEDU_EMPLEADO == strCedula select t).FirstOrDefault();
                }
                ViewBag.TIPO_SEXO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.TIPO_SEXO);
                ViewBag.GRUP_SANGUINEO = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "GRUPO_SANGUINEO" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.GRUP_SANGUINEO);
                ViewBag.ESTA_CIVIL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ESTACIVIL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.ESTA_CIVIL);
                ViewBag.CODI_ESPECIALIDAD = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "ESPECIALIDAD" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.CODI_ESPECIALIDAD);
                ViewBag.ESTA_LABORAL = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_LABORAL" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.ESTA_LABORAL);
                ViewBag.NUME_SECCION = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "SECCION" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.NUME_SECCION);
                ViewBag.CADUNID_FUERZA = new SelectList((from t in db2.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }).ToArray(), "codigo", "descripcion", cADPOLICIA.CADUNID_FUERZA);
                if (cADPOLICIA.CADUNID_CODIGO != null)
                    ViewBag._UNIDAD = db2.CADUNIDADES.Where(t => t.CODIGO == cADPOLICIA.CADUNID_CODIGO).FirstOrDefault().DESCRIPCION;
                else ViewBag._UNIDAD = string.Empty;
                ViewBag.CADUNID_CODIGO = string.Empty;
                ViewBag.CADFUERZ_ID = new SelectList(ObtenerFuerzasPorUsuario(usuarioActual), "ID", "DESCRIPCION", cADPOLICIA.CADFUERZ_ID);
                ViewBag.GRAD_ALFABETICO = new SelectList(db.CADGRADOS, "ALFABETICO", "DESCRIPCION", cADPOLICIA.GRAD_ALFABETICO);
                CADPOLICIAS_FOTOS cADFOTO = db.CADPOLICIAS_FOTOS.Find(cADPOLICIA.CEDU_EMPLEADO);
                byte[] data = (from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == cADPOLICIA.CEDU_EMPLEADO select t.FOTO).FirstOrDefault();
                if (data != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data, cADPOLICIA.CEDU_EMPLEADO));
                }
                else
                {
                    ViewBag.foto = Url.Content("~/images/Avatar.png");
                }
                return View("Create", cADPOLICIA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}