using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class UnidadesController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorGeneral objGestorGral;
        private GestorAuditoria auditoria = new GestorAuditoria();
        private string _strMensaje = "";
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: CADUNIDADEs
        [SecadPlusAutorizador("/Unidades/Index")]
        public ActionResult Index()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.UNIDADES = "SI";
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Unidades/Consulta")]
        public ActionResult Consulta()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                return View("Index");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADUNIDADEs/Create
        [Authorize]
        public ActionResult Create(string strFuerza, int intCodigo, int intNivel, string strMensaje = null)
        {
            try
            {
                ViewBag.MENSAJE_EXITOSO = "";
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA" && m.CODIGO == strFuerza).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.UNIDADPADRE = new SelectList(db.CADUNIDADES.Where(m => m.CODIGO == intCodigo).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CODIGOPADRE = intCodigo;
                ViewBag.NIVEL = intNivel;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADUNIDADEs/Create Para protegerse de ataques de publicación excesiva, habilite las
        // propiedades específicas a las que desea enlazarse. Para obtener más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "FUERZA,SIGLA,DESCRIPCION,OPERATIVA,VIGENTE,LUGE_CODIGO,NIVEL,CONS_SIEDCO,CONS_SIATH,CADUNDE_FUERZA,CADUNDE_CODIGO,ORDENAMIENTO,ABREVIATURA_UNIDAD,CADUNDE_CODIGO")] CADUNIDADE cADUNIDADE)
        {
            string strFuerza = "", strMensaje = "";
            try
            {
                GestorGeneral gestor = new GestorGeneral();
                int intConsecutivo = gestor.ObtenerMaximoUnidad(cADUNIDADE.CADUNDE_CODIGO.ToString());
                if (intConsecutivo <= 0)
                {
                }
                cADUNIDADE.CODIGO = intConsecutivo;
                if (ModelState.IsValid)
                {
                    //Modifica los valores de llave forarea recursiva para el escenario sin unidad padre
                    if (cADUNIDADE.CADUNDE_CODIGO == 0)
                    {
                        cADUNIDADE.CADUNDE_CODIGO = null;
                        cADUNIDADE.CADUNDE_FUERZA = null;
                    }
                    db.CADUNIDADES.Add(cADUNIDADE);
                    db.SaveChanges();
                    strFuerza = cADUNIDADE.FUERZA.ToString();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADUNIDADE.CODIGO.ToString());
                    if (cADUNIDADE.CADUNDE_CODIGO == null) return RedirectToAction("Create", new { strFuerza = strFuerza, intCodigo = 0, intNivel = cADUNIDADE.NIVEL, strMensaje = strMensaje });
                    return RedirectToAction("Create", new { strFuerza = strFuerza, intCodigo = cADUNIDADE.CADUNDE_CODIGO, intNivel = cADUNIDADE.NIVEL, strMensaje = strMensaje });
                }
                strFuerza = cADUNIDADE.FUERZA.ToString();
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA" && m.CODIGO == strFuerza).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CODIGOPADRE = cADUNIDADE.CADUNDE_CODIGO;
                ViewBag.UNIDADPADRE = new SelectList(db.CADUNIDADES.Where(m => m.CODIGO == cADUNIDADE.CADUNDE_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.NIVEL = cADUNIDADE.NIVEL;
                return View(cADUNIDADE);
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADUNIDADEs/Edit/5
        [Authorize]
        public ActionResult Edit(int intCodigo, byte btFuerza, string strMensaje = null)
        {
            try
            {
                string strFuerza;
                ViewBag.MENSAJE_EXITOSO = "";
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }

                CADUNIDADE cADUNIDADE = db.CADUNIDADES.Where(c => c.CODIGO == intCodigo && c.FUERZA == btFuerza).FirstOrDefault();
                if (cADUNIDADE == null)
                {
                    return HttpNotFound();
                }
                strFuerza = cADUNIDADE.FUERZA.ToString();
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA" && m.CODIGO == strFuerza).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.UNIDADPADRE = db.CADUNIDADES.Where(m => m.CODIGO == cADUNIDADE.CADUNDE_CODIGO).OrderBy(m => m.DESCRIPCION).FirstOrDefault();
                ViewBag.NIVEL = cADUNIDADE.NIVEL;

                return View(cADUNIDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADUNIDADEs/Edit/5 Para protegerse de ataques de publicación excesiva, habilite las
        // propiedades específicas a las que desea enlazarse. Para obtener más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "FUERZA,CODIGO,SIGLA,DESCRIPCION,OPERATIVA,VIGENTE,LUGE_CODIGO,NIVEL,CONS_SIEDCO,CONS_SIATH,CADUNDE_FUERZA,CADUNDE_CODIGO,ORDENAMIENTO,ABREVIATURA_UNIDAD")] CADUNIDADE cADUNIDADE)
        {
            try
            {
                string strFuerza;
                if (ModelState.IsValid)
                {
                    db.Entry(cADUNIDADE).State = EntityState.Modified;
                    db.SaveChanges();


                    auditoria.RegistrarAuditoria<CADUNIDADE>(cADUNIDADE, "CADUNIDADE", "PERSATEN_SITIOGRABACION=" + cADUNIDADE.FUERZA.ToString() + ",CODIGO=" + cADUNIDADE.CODIGO
                    , TipoAuditoria.Modificar);

                    _strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADUNIDADE.CODIGO.ToString());
                    return RedirectToAction("Edit", new { intCodigo = cADUNIDADE.CODIGO, btFuerza = cADUNIDADE.FUERZA, strMensaje = _strMensaje });
                }
                strFuerza = cADUNIDADE.FUERZA.ToString();
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA" && m.CODIGO == strFuerza).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.UNIDADPADRE = db.CADUNIDADES.Where(m => m.CODIGO == cADUNIDADE.CADUNDE_CODIGO).OrderBy(m => m.DESCRIPCION).FirstOrDefault();
                ViewBag.NIVEL = cADUNIDADE.NIVEL;
                return View(cADUNIDADE);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                return View(cADUNIDADE);
            }
        }

        /// <summary>
        /// Captura el listado de las fuerzas
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult GetFuerza()
        {
            try
            {
                objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));

                var dttFuerza = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_fuerza", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                var LstFuerzas = (from DataRow dr in dttFuerza.Rows
                                  select new
                                  {
                                      FUERZA = dr["FUERZA"],
                                      DESCRIPCION = dr["DESCRIPCION"],
                                  }).ToList();

                return Json(new { result = true, datos = LstFuerzas });
                //return Json((LstFuerzas), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Captura el listado del Lugar
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerLugaresAjaxPro()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                int intControl = 0;
                string strQuery = "";
                double dblControl = 0;
                bool blControl = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"cargarValor(_CODIGO, 'LUGAR')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                List<_LugaresModel> listaLugares = new List<_LugaresModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE DATOS.CODIGO = " + parametros.ConsultaValor + " ";
                    }
                    else
                    {
                        strQuery = " WHERE DATOS.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strQuery += " ORDER BY DATOS.CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strQuery += " ORDER BY DATOS.DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");
                parametros.TotalRegistroFiltrados = dttLugar.Rows.Count;
                foreach (DataRow fila in dttLugar.Rows)
                {
                    //Control del páginado de registros
                    if (intControl >= parametros.Inicio && intControl <= parametros.Final)
                    {
                        listaLugares.Add(new _LugaresModel("", strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()), fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    }
                    intControl++;
                }
                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaLugares });
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
        public ActionResult ObtenerUnidadesAjax(string strFuerza)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                int intControl = 0;
                string strQuery = "";
                double dblControl = 0;
                bool blControl = false;
                List<ExtUnidad> datos = new List<ExtUnidad>();
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"cargarUnidad(_CODIGO, 'DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " AND (NIV.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " AND (NIV.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' OR PADRE.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' OR NIV.SIGLA = '" + parametros.ConsultaValor + "') ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strQuery += " ORDER BY NIV.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strQuery += " ORDER BY NIV.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strQuery += " ORDER BY PADRE.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strQuery += " ORDER BY NIV.SIGLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strQuery += " ORDER BY NIV.OPERATIVA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strQuery += " ORDER BY NIV.VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strQuery += " ORDER BY LUGAR.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_FUERZA", strFuerza, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.P_OBTENER_UNIDADES", strConexion, "Nonquery");

                parametros.TotalRegistroFiltrados = dttLugar.Rows.Count;
                foreach (DataRow fila in dttLugar.Rows)
                {
                    //Control del páginado de registros
                    if (intControl >= parametros.Inicio && intControl <= parametros.Final)
                    {
                        datos.Add(new ExtUnidad(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", fila["DESCRIPCION"].ToString())));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    }
                    intControl++;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = datos });
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
        public JsonResult ObtenerLugaresLista()
        {
            try
            {
                objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                List<SelectListItem> lista = new List<SelectListItem>();

                foreach (DataRow fila in dttLugar.Rows)
                {
                    lista.Add(new SelectListItem()
                    {
                        Text = fila["CODIGO"].ToString() + " - " + fila["DESCRIPCION"].ToString(),
                        Value = fila["CODIGO"].ToString()
                    });
                }
                SelectList item = new SelectList(lista, "Value", "Text");
                return Json(lista);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Captura el listado del Lugar
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerCodigosSIATH()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                int intControl = 0;
                string strQuery = "";
                double dblControl = 0;
                bool blControl = false;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"cargarValor(_CODIGO, 'SIATH')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                _TablasModel datos = new _TablasModel();
                datos.CodigosSIATH = new List<_SiathModel>();
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (DATOS.SIATH = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (DATOS.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' ) ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strQuery += " ORDER BY DATOS.SIATH " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strQuery += " ORDER BY DATOS.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strQuery += " ORDER BY DATOS.VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "SIATH", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");
                parametros.TotalRegistroFiltrados = dttLugar.Rows.Count;
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    //Control del páginado de registros
                    if (intControl >= parametros.Inicio && intControl <= parametros.Final)
                    {
                        datos.CodigosSIATH.Add(new _SiathModel(fila, strBoton.Replace("_CODIGO", fila["SIATH"].ToString())));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    }
                    intControl++;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = datos.CodigosSIATH });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Captura el nivel uno con respecto a la fuerza escojida
        /// </summary>
        /// <param name="intCodigoPadre">Tipo enetero y es el código de la fuerza</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult GetNivelUno(string intCodigoPadre)
        {
            try
            {
                objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo_fuerza", intCodigoPadre, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "COMPLETO", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttNivel1 = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_unidades_principal", strConexion, "Nonquery");
                if (dttNivel1.Rows.Count > 0)
                {
                }
                //Se combiente en una lista
                var LstNivel1 = (from DataRow dr in dttNivel1.Rows
                                 select new
                                 {
                                     CODIGO = dr["CODIGO"],
                                     SIGLA = dr["SIGLA"],
                                     DESCRIPCION = dr["DESCRIPCION"],
                                     OPERATIVA = dr["OPERATIVA"],
                                     LUGAR = dr["LUGAR"],
                                     NIVEL = dr["NIVEL"],
                                     SIEDCO = dr["SIEDCO"],
                                     SIATH = dr["SIATH"],
                                     PADRE = dr["PADRE"],
                                     ORDENAMIENTO = dr["ORDENAMIENTO"]
                                 }).ToList();

                return Json((LstNivel1), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult GetNivelUnoActivos(string intCodigoPadre)
        {
            try
            {
                objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo_fuerza", intCodigoPadre, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "ACTIVOS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttNivel1 = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_unidades_principal", strConexion, "Nonquery");
                if (dttNivel1.Rows.Count > 0)
                {
                }
                //Se combiente en una lista
                var LstNivel1 = (from DataRow dr in dttNivel1.Rows
                                 select new
                                 {
                                     CODIGO = dr["CODIGO"],
                                     SIGLA = dr["SIGLA"],
                                     DESCRIPCION = dr["DESCRIPCION"],
                                     OPERATIVA = dr["OPERATIVA"],
                                     LUGAR = dr["LUGAR"],
                                     NIVEL = dr["NIVEL"],
                                     SIEDCO = dr["SIEDCO"],
                                     SIATH = dr["SIATH"],
                                     PADRE = dr["PADRE"],
                                     ORDENAMIENTO = dr["ORDENAMIENTO"]
                                 }).ToList();

                return Json((LstNivel1), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Captura el nivel siguiente con respecto al item seleccionado
        /// </summary>
        /// <param name="intCodigoPadre">Código del item seleccionado</param>
        /// <param name="intNivel">Nivel siguiente para mostrar</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerNiveles(string intCodigoPadre, int intNivel, int intFuerza)
        {
            try
            {
                objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo_padre", intCodigoPadre, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "COMPLETO", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nivel", intNivel.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", intFuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));

                var dttNivel1 = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_nivel", strConexion, "Nonquery");
                //Se combiente en una lista
                var LstNivel1 = (from DataRow dr in dttNivel1.Rows
                                 select new
                                 {
                                     CODIGO = dr["CODIGO"],
                                     SIGLA = dr["SIGLA"],
                                     DESCRIPCION = dr["DESCRIPCION"],
                                     OPERATIVA = dr["OPERATIVA"],
                                     VIGENTE = dr["VIGENTE"],
                                     LUGAR = dr["LUGAR"],
                                     NIVEL = dr["NIVEL"],
                                     SIEDCO = dr["SIEDCO"],
                                     SIATH = dr["SIATH"],
                                     PADRE = dr["PADRE"],
                                     ORDENAMIENTO = dr["ORDENAMIENTO"],
                                     ABREVIATURA = dr["ABREVIATURA"],
                                     FUERZA = dr["FUERZA"]
                                 }).ToList();
                if (LstNivel1.Count > 0)
                    return Json((LstNivel1), JsonRequestBehavior.AllowGet);
                else
                    return Json((null), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ObtenerNivelActivos(string intCodigoPadre, int intNivel, int intFuerza)
        {
            try
            {
                objGestorGral = new GestorGeneral();
                GestorAdministrador gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo_padre", intCodigoPadre, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", "ACTIVOS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nivel", intNivel.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", intFuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));

                var dttNivel1 = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_nivel", strConexion, "Nonquery");
                //Se combiente en una lista
                var LstNivel1 = (from DataRow dr in dttNivel1.Rows
                                 select new
                                 {
                                     CODIGO = dr["CODIGO"],
                                     SIGLA = dr["SIGLA"],
                                     DESCRIPCION = dr["DESCRIPCION"],
                                     OPERATIVA = dr["OPERATIVA"],
                                     VIGENTE = dr["VIGENTE"],
                                     LUGAR = dr["LUGAR"],
                                     NIVEL = dr["NIVEL"],
                                     SIEDCO = dr["SIEDCO"],
                                     SIATH = dr["SIATH"],
                                     PADRE = dr["PADRE"],
                                     ORDENAMIENTO = dr["ORDENAMIENTO"],
                                     ABREVIATURA = dr["ABREVIATURA"],
                                     FUERZA = dr["FUERZA"]
                                 }).ToList();
                if (LstNivel1.Count > 0)
                    return Json((LstNivel1), JsonRequestBehavior.AllowGet);
                else
                    return Json((null), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult Eliminar(int fuerza, int codigo)
        {
            try
            {
                //Eliminar la información del código enviado
                CADUNIDADE cADUNIDADDEL = db.CADUNIDADES.Where(x => x.FUERZA == fuerza && x.CODIGO == codigo).ToList().FirstOrDefault();
                db.CADUNIDADES.Remove(cADUNIDADDEL);
                db.SaveChanges();


                CADUNIDADE cADUNIDADDEL_copy = new CADUNIDADE();
                cADUNIDADDEL_copy = (CADUNIDADE)auditoria.GenerarCopiaAuditoria(cADUNIDADDEL);

                auditoria.RegistrarAuditoria<CADUNIDADE>(cADUNIDADDEL_copy, "CADUNIDADE", "PERSATEN_SITIOGRABACION=" + cADUNIDADDEL.FUERZA.ToString() + ",CODIGO=" + cADUNIDADDEL.CODIGO
                , TipoAuditoria.Eliminar);



                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Registro seleccionado") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public ActionResult VerCuadrantes(string strUnidad, string strCodigo)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Unidad = strUnidad;
                ViewBag.UnidadCodigo = strCodigo;
                return View("Cuadrantes");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarCuadrantes(string strCodigo)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<VistaCuadrantesCivic> elementos = new List<VistaCuadrantesCivic>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                DateTime dtControl;
                bool blFiltro = false, blControl = false, blDtControl = false;
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 2)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    //valida si debe filtrar por fecha
                    blDtControl = DateTime.TryParse(parametros.ConsultaValor, out dtControl);
                    if (blDtControl)
                    {

                        strQuery = " WHERE (INTERNO.FECHA_DESDE =  to_date('" + parametros.ConsultaValor + "','DD/MM/YYYY HH24:MI:SS') OR INTERNO.FECHA_HASTA =  to_date('" + parametros.ConsultaValor + "','DD/MM/YYYY HH24:MI:SS')) ";
                    }
                    else if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.SITIO_GRABA = " + parametros.ConsultaValor + " OR INTERNO.CONSECUTIVO= " + parametros.ConsultaValor + " OR INTERNO.CADUNID_CODIGO= " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.NRO_CUADRANTE, '" + parametros.ConsultaValor + "', 'i') OR INTERNO.NIVEL_PELIGROSIDAD = '" + parametros.ConsultaValor + "')";
                    }

                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrdenamiento += " ORDER BY INTERNO.SITIO_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CADUNID_FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.CADUNID_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.NRO_CUADRANTE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.CAPA_CUADRANTE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.AMBITO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 8:
                        strOrdenamiento += " ORDER BY INTERNO.VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 9:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_DESDE " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 10:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_HASTA " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 11:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 12:
                        strOrdenamiento += " ORDER BY INTERNO.USUA_TRANSACCION " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 13:
                        strOrdenamiento += " ORDER BY INTERNO.NIVEL_PELIGROSIDAD " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 14:
                        strOrdenamiento += " ORDER BY INTERNO.CENTROIDEX " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 15:
                        strOrdenamiento += " ORDER BY INTERNO.CENTROIDEY " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 16:
                        strOrdenamiento += " ORDER BY INTERNO.CAD1CUADSITGR_DIJIN " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 17:
                        strOrdenamiento += " ORDER BY INTERNO.CAD1CUADCONS_DIJIN " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 18:
                        strOrdenamiento += " ORDER BY INTERNO.CAD1CUADSITGR_DIPOL " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 19:
                        strOrdenamiento += " ORDER BY INTERNO.CAD1CUADCONS_DIPOL " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 20:
                        strOrdenamiento += " ORDER BY INTERNO.SIGLA_DPTO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 21:
                        strOrdenamiento += " ORDER BY INTERNO.MOTIVO_CIERRE " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.NRO_CUADRANTE DESC  ";
                        break;
                }

                strLimites = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", strCodigo, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_cuadrantes", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);

                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new VistaCuadrantesCivic(fila));

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