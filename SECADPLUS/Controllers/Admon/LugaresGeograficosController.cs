using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadLugaresGeograficos;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class LugaresGeograficosController : Controller
    {
        private EntitiesComunasBarrios db = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private GestorGeneral objGestorGral;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private GestorAuditoria auditoria = new GestorAuditoria();


        /// <summary>
        /// Carga la lista de sitios cuando se hace click en ver del objeto padre
        /// </summary>
        /// <param name="idFather"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult FindByFather(string idFather = null, string strNombre = null, int? intNivel = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string idPadre = idFather != null ? idFather.ToString() : string.Empty;
                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugares;

                string strBoton = "<td> <ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'>";
                strBoton += "<i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default' type='button' onclick=\"BuscaPorPadre(@item.CODIGO, @item.NIVEL, '_boton" + strNombre + "', '@item.DESCRIPCION')\">";
                strBoton += "<i class='icon-eye'></i> Ver </button> </li><li> <li> <button class='btn btn-default' onclick=\"AbreEnNuevaVentana('urlEditar', @item.CODIGO, @item.NIVEL)\"> <i class='glyphicon glyphicon-edit'></i>Editar ";
                strBoton += "</button> </li><li> <button class='btn btn-default' onclick=\"AbreEnNuevaVentana('urlNuevo', @item.CODIGO, @item.NIVEL)\"><i class='icon-new'></i>Crear Lugar </button> </li><li> ";
                strBoton += "<button class='btn btn-default' type='button' onclick=\"EliminarLugarGeografico('urlBorrar', @item.LUGE, @item.NIVEL, '@item.DESCRIPCION')\"> ";
                strBoton += "<i class='icon - trash'></i> Borrar </button> </li></ul></li></ul> </td>";

                if (strNombre == "Veredas")
                {
                    strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default' type='button' onclick=\"BuscaPorPadre(@item.CODIGO, @item.NIVEL, '_boton" + strNombre + "', '@item.DESCRIPCION')\"><i class='icon-eye'></i> Ver </button></li><li><button class='btn btn-default' onclick=\"AbreEnNuevaVentana('urlEditar', @item.CODIGO, @item.NIVEL)\"><i class='glyphicon glyphicon-edit'></i>Editar </button></li><li><button class='btn btn-default' type='button' onclick=\"EliminarLugarGeografico('urlBorrar', @item.LUGE, @item.NIVEL, '@item.DESCRIPCION')\"><i class='icon-trash'></i> Borrar </button></li><li> <button class='btn btn-default' onclick=\"AbreEnNuevaVentana('urlNuevo', @item.CODIGO, @item.NIVEL)\"><i class='icon-new'></i>Crear Vereda </button> </li><li><button class='btn btn-default' onclick=\"AbreEnNuevaVentana('creaComuna', 'CODIGO','comunas')\"><i class='glyphicon glyphicon-edit'></i>Crear Comuna </button></li></ul></li></ul></td>";
                }
                else if (strNombre == "Comunas")
                {
                    strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn  btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default' type='button' onclick=\"BuscaComunas(@item.CODIGO, @item.NIVEL, '_boton" + strNombre + "', '@item.DESCRIPCION')\"><i class='icon-eye'></i> Ver </button></li><li><button class='btn btn-default' onclick=\"AbreEnNuevaVentana('urlEditar', @item.CODIGO, @item.NIVEL)\"><i class='glyphicon glyphicon-edit'></i>Editar </button></li><li><button class='btn btn-default' type='button' onclick=\"EliminarLugarGeografico('urlBorrar', @item.LUGE, @item.NIVEL, '@item.DESCRIPCION')\"><i class='icon-trash'></i> Borrar </button></li><li><button class='btn btn-default' onclick=\"AbreEnNuevaVentana('creaComuna', 'CODIGO','comunas')\"><i class='glyphicon glyphicon-edit'></i>Crear Comuna </button></li></ul></li></ul></td>";
                }

                List<Ext_LUGARES_GEOGRAFICOS> listaLugares = new List<Ext_LUGARES_GEOGRAFICOS>();
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
                            "%') OR upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%'))";
                    }
                    if (idPadre != string.Empty)
                    {
                        strQuery += " AND LUGE_CODIGO =" + idPadre;
                    }
                }
                else
                {
                    if (idPadre != string.Empty)
                    {
                        strQuery += " WHERE LUGE_CODIGO =" + idPadre;
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
                dttLugares = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listalugaresporpadre", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugares.Rows)
                {
                    //Control del páginado de registros
                    listaLugares.Add(new Ext_LUGARES_GEOGRAFICOS(fila, strBoton.Replace("@item.CODIGO",
                        fila["CODIGO"].ToString()).Replace("@item.NIVEL", (string.IsNullOrEmpty(fila["NIVEL"].ToString()) ? intNivel.ToString() : fila["NIVEL"].ToString())).Replace("@item.LUGE", fila["LUGE_CODIGO"].ToString()).Replace("_botonRefrescar",
                        "_btn" + strNombre).Replace("@item.DESCRIPCION", fila["DESCRIPCION"].ToString()).Replace("urlEditar",
                        Url.Action("Edit", "LugaresGeograficos", new { id = fila["CODIGO"].ToString() })).Replace("urlNuevo",
                        @Url.Action("CreateByFather", "LugaresGeograficos", new { idFather = fila["CODIGO"].ToString(), level = fila["NIVEL"].ToString() })).Replace("urlBorrar", @Url.Action("DeleteConfirmed", "LugaresGeograficos", new { id = fila["CODIGO"] }))
                        .Replace("creaComuna", @Url.Action("Create", "Comunas", new { idFather = fila["CODIGO"].ToString() }))));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaLugares });
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
        public JsonResult FindComunas(int? idFather, string strNombre = null, int? intNivel = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string idPadre = idFather != null ? idFather.ToString() : string.Empty;
                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugares;
                //string strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default ' type='button' onclick=\"location.href = 'urlModificaPersona'\"><i class='icon-pencil'></i> Editar</button></li><li><button class='btn btn-default ' type='button' onclick=\"EliminarElemento('urlBorraPersona' )\")><i class='icon-trash'></i> Borrar</button></li></ul></li></ul></td>";
                string strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default' type='button' onclick=\"BuscaBarrios(@item.CODIGO,@item.LUGE,'barrios', '_botonBarrios','_descripcionPADRE')\"><i class='icon-eye'></i> Ver </button></li><li><button class='btn btn-default' target='_blank' onclick=\"AbreEnNuevaVentana('urlEditar')\", 'LUGE','_descripcionPADRE')'><i class='glyphicon glyphicon-edit'></i>Editar </button></li><li><button class='btn btn-default' type='button' onclick=\"EliminarComuna('urlBorrar','@item.CODIGO', '@item.LUGE','barrioNombre')\"><i class='icon-trash'></i> Borrar </button></li><li><button class='btn btn-default' target='_blank' onclick=\"AbreEnNuevaVentana('urlCreaBarrios', 'CODIGO','barrioNombre')\"><i class='icon-versions'></i> Crear Barrio </button></li></ul></li></ul></td>";

                List<Ext_COMUNAS> listaLugares = new List<Ext_COMUNAS>();
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
                            "%') OR upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%'))";
                    }
                    if (idPadre != string.Empty)
                    {
                        strQuery += " AND LUGE_CODIGO =" + idPadre;
                    }
                }
                else
                {
                    if (idPadre != string.Empty)
                    {
                        strQuery += " WHERE LUGE_CODIGO =" + idPadre;
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
                dttLugares = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listacomunas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugares.Rows)
                {
                    //Control del páginado de registros

                    listaLugares.Add(new Ext_COMUNAS(fila, strBoton.Replace("@item.CODIGO", fila["CODIGO"].ToString()).Replace("@item.LUGE", fila["LUGE_CODIGO"].ToString()).Replace("@item.DESCRIPCION",
                        fila["DESCRIPCION"].ToString()).Replace("urlEditar", Url.Action("Edit", "Comunas",
                        new { id = fila["CODIGO"].ToString(), idPadre = fila["LUGE_CODIGO"].ToString() })).Replace("urlBorrar", @Url.Action("DeleteConfirmed", "Comunas"))
                        .Replace("urlCreaBarrios", @Url.Action("Create", "Barrios", new { idFather = fila["LUGE_CODIGO"].ToString(), idComuna = fila["CODIGO"].ToString() }))
                        .Replace("_descripcionPADRE", fila["DESCRIPCION"].ToString()).Replace("barrioNombre", fila["DESCRIPCION"].ToString())
                        ));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaLugares });
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
        public JsonResult FindBarrios(int? idCozoCodigo, int? idLugeCodigo)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugares;
                string strBoton = "<td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><button class='btn btn-default' target='_blank' onclick=\"AbreEnNuevaVentana('urlEditaBarrio', 'LUGE','barrioNombre')\"><i class='glyphicon glyphicon-edit'></i>Editar </button></li><li><button class='btn btn-default' type='button' onclick=\"EliminarBarrio('urlEliminarBarrio', 'CODIGO','LUGE','barrioNombre')\"><i class='icon-trash'></i> Borrar </button></li></ul></li></ul></td>";

                List<Ext_BARRIOS> listaLugares = new List<Ext_BARRIOS>();
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
                            "%') OR upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%'))";
                    }
                    if (idLugeCodigo.ToString() != string.Empty)
                    {
                        strQuery += " AND LUGE_CODIGO =" + idLugeCodigo.ToString() + " AND COZO_CODIGO ='" + idCozoCodigo.ToString();
                    }
                }
                else
                {
                    if (idLugeCodigo.ToString() != string.Empty)
                    {
                        strQuery += " WHERE LUGE_CODIGO =" + idLugeCodigo.ToString() + " AND COZO_CODIGO =" + idCozoCodigo.ToString();
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
                dttLugares = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listabarriosporpadre", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugares.Rows)
                {
                    //Control del páginado de registros
                    listaLugares.Add(new Ext_BARRIOS(fila, strBoton.Replace("urlEditaBarrio", @Url.Action("Edit", "Barrios", new { id = fila["CODIGO"].ToString(), idFather = fila["LUGE_CODIGO"].ToString() }))
                        .Replace("urlEliminarBarrio", @Url.Action("DeleteConfirmed", "Barrios", new { id = fila["CODIGO"].ToString(), idFather = fila["LUGE_CODIGO"].ToString() }))
                        .Replace("barrioNombre", fila["DESCRIPCION"].ToString())
                        .Replace("CODIGO", fila["CODIGO"].ToString())
                        .Replace("LUGE", fila["LUGE_CODIGO"].ToString())
                        ));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaLugares });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: CADLUGARES_GEOGRAFICOS
        [SecadPlusAutorizador("/LugaresGeograficos/Index")]
        public async Task<ActionResult> Index(string message = null)
        {
            try
            {
                if (message != null)
                {
                    if (message.StartsWith("No"))
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    else
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    ViewBag.MENSAJE_EXITOSO = message;
                }


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View(await db.CADLUGARES_GEOGRAFICOS.Where(c => c.NIVEL == 1).ToListAsync());
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        // GET: CADLUGARES_GEOGRAFICOS/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = await db.CADLUGARES_GEOGRAFICOS.FindAsync(id);
            if (cADLUGARES_GEOGRAFICOS == null)
            {
                return HttpNotFound();
            }
            return View(cADLUGARES_GEOGRAFICOS);
        }

        // GET: CADLUGARES_GEOGRAFICOS/Create
        [SecadPlusAutorizador("/LugaresGeograficos/Create")]
        public ActionResult Create(int iLevel)
        {
            //Lleno el drop downlist del luge_codigo
            ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.NIVEL == iLevel - 1), "CODIGO", "DESCRIPCION");
            return View();
        }

        // GET: CADLUGARES_GEOGRAFICOS/Create
        [SecadPlusAutorizador("/LugaresGeograficos/CreateByFather")]
        public ActionResult CreateByFather(int idFather, int? level)
        {
            Session["codFather"] = idFather;

            int codLevel = (int)((from t in db.CADLUGARES_GEOGRAFICOS where t.CODIGO == idFather select t.NIVEL).FirstOrDefault() != null ? (from t in db.CADLUGARES_GEOGRAFICOS where t.CODIGO == idFather select t.NIVEL).FirstOrDefault() + 1 : -1);

            if (level == 5) codLevel = 5;
            CADLUGARES_GEOGRAFICOS cADLUGARES = new CADLUGARES_GEOGRAFICOS();
            cADLUGARES.NIVEL = Convert.ToByte(codLevel);
            string tipoSeleccionado = string.Empty;
            //Lleno el drop downlist del luge_codigo
            switch (codLevel)
            {
                case 1:
                    tipoSeleccionado = "CO";
                    break;

                case 2:
                    tipoSeleccionado = "PA";
                    break;

                case 3:
                    tipoSeleccionado = "DE";
                    break;

                default:
                    tipoSeleccionado = "";
                    break;
            }
            ViewBag.TIPO = new SelectList(db.CADLUGARES_GEOGRAFICOS.GroupBy(t => t.TIPO).Select(g => g.FirstOrDefault()), "TIPO", "TIPO", tipoSeleccionado);
            ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");

            return View(cADLUGARES);
        }

        // POST: CADLUGARES_GEOGRAFICOS/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/LugaresGeograficos/CreateByFather")]
        public async Task<ActionResult> CreateByFather([Bind(Include = "CODIGO,DESCRIPCION,TIPO,CODIGO_DANE,CODIGO_REGISTRADURIA,LUGE_CODIGO,NIVEL,DPTO,VIGENTE,CODIGO_SIEDCO,CODIGO_SIATH,NIVEL_1,NIVEL_2,NIVEL_3,NIVEL_4,NIVEL_5,NIVEL_6")] CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS)
        {
            cADLUGARES_GEOGRAFICOS.VIGENTE = cADLUGARES_GEOGRAFICOS.VIGENTE == "true" ? "S" : "N";
            int idFather = int.Parse(Session["codFather"].ToString());
            ModelState.Clear();
            TryValidateModel(cADLUGARES_GEOGRAFICOS);
            ViewBag.TIPO = new SelectList(db.CADLUGARES_GEOGRAFICOS.GroupBy(t => t.TIPO).Select(g => g.FirstOrDefault()), "TIPO", "TIPO", cADLUGARES_GEOGRAFICOS.TIPO);
            ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADLUGARES_GEOGRAFICOS.LUGE_CODIGO), "CODIGO", "DESCRIPCION", cADLUGARES_GEOGRAFICOS.LUGE_CODIGO);
            ViewBag.VIGENTE = cADLUGARES_GEOGRAFICOS.VIGENTE == "S" ? true : false;

            if (ModelState.IsValid)
            {
                db.CADLUGARES_GEOGRAFICOS.Add(cADLUGARES_GEOGRAFICOS);
                try
                {
                    bool existe = (from t in db.CADLUGARES_GEOGRAFICOS where t.CODIGO == cADLUGARES_GEOGRAFICOS.CODIGO select t.CODIGO).Count() > 0;
                    if (existe)
                    {
                        ViewBag.MENSAJE_EXITOSO = string.Format("El registro ya existe para el código: {0}", cADLUGARES_GEOGRAFICOS.CODIGO);
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                        return View(cADLUGARES_GEOGRAFICOS);
                    }
                    await db.SaveChangesAsync();
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    cADLUGARES_GEOGRAFICOS = new CADLUGARES_GEOGRAFICOS();
                    ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");
                    return View(cADLUGARES_GEOGRAFICOS);
                }
                catch (Exception ex)
                {
                    var mensaje = ex.Message;
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }
            }
            ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");
            ViewBag.VIGENTE = true;
            return View(cADLUGARES_GEOGRAFICOS);
        }

        // POST: CADLUGARES_GEOGRAFICOS/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/LugaresGeograficos/Create")]
        public async Task<ActionResult> Create([Bind(Include = "CODIGO,DESCRIPCION,TIPO,CODIGO_DANE,CODIGO_REGISTRADURIA,LUGE_CODIGO,NIVEL,DPTO,VIGENTE,CODIGO_SIEDCO,CODIGO_SIATH,NIVEL_1,NIVEL_2,NIVEL_3,NIVEL_4,NIVEL_5,NIVEL_6")] CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.CADLUGARES_GEOGRAFICOS.Add(cADLUGARES_GEOGRAFICOS);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    var mensaje = ex.Message;
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }
            }
            ViewBag.TIPO = new SelectList(db.CADLUGARES_GEOGRAFICOS.GroupBy(t => t.TIPO).Select(g => g.FirstOrDefault()), "TIPO", "TIPO", cADLUGARES_GEOGRAFICOS.TIPO);
            ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS, "CODIGO", "DESCRIPCION", cADLUGARES_GEOGRAFICOS.LUGE_CODIGO);
            return View(cADLUGARES_GEOGRAFICOS);
        }

        // GET: CADLUGARES_GEOGRAFICOS/Edit/5
        [SecadPlusAutorizador("/LugaresGeograficos/Edit")]
        public async Task<ActionResult> Edit(int? id, int? level)
        {
            Session["codLevel"] = level;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = await db.CADLUGARES_GEOGRAFICOS.FindAsync(id);

            CADLUGARES_GEOGRAFICOS cAD_LUGE = db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADLUGARES_GEOGRAFICOS.LUGE_CODIGO).FirstOrDefault();
            if (cADLUGARES_GEOGRAFICOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.TIPO = new SelectList(db.CADLUGARES_GEOGRAFICOS.GroupBy(t => t.TIPO).Select(g => g.FirstOrDefault()), "TIPO", "TIPO", cADLUGARES_GEOGRAFICOS.TIPO);
            ViewBag.VIGENTE = cADLUGARES_GEOGRAFICOS.VIGENTE == "S" ? true : false;
            ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADLUGARES_GEOGRAFICOS.LUGE_CODIGO), "CODIGO", "DESCRIPCION", cADLUGARES_GEOGRAFICOS.LUGE_CODIGO);

            return View(cADLUGARES_GEOGRAFICOS);
        }

        // POST: CADLUGARES_GEOGRAFICOS/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/LugaresGeograficos/Edit")]
        public async Task<ActionResult> Edit([Bind(Include = "CODIGO,DESCRIPCION,TIPO,CODIGO_DANE,CODIGO_REGISTRADURIA,LUGE_CODIGO,NIVEL,DPTO,VIGENTE,CODIGO_SIEDCO,CODIGO_SIATH,NIVEL_1,NIVEL_2,NIVEL_3,NIVEL_4,NIVEL_5,NIVEL_6")] CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS)
        {
            try
            {
                cADLUGARES_GEOGRAFICOS.VIGENTE = cADLUGARES_GEOGRAFICOS.VIGENTE == "true" ? "S" : "N";
                ViewBag.TIPO = new SelectList(db.CADLUGARES_GEOGRAFICOS.GroupBy(t => t.TIPO).Select(g => g.FirstOrDefault()), "TIPO", "TIPO", cADLUGARES_GEOGRAFICOS.TIPO);

                ModelState.Clear();
                TryValidateModel(cADLUGARES_GEOGRAFICOS);
                ViewBag.VIGENTE = cADLUGARES_GEOGRAFICOS.VIGENTE == "S" ? true : false;
                if (ModelState.IsValid)
                {
                    db.Entry(cADLUGARES_GEOGRAFICOS).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    auditoria.RegistrarAuditoria<CADLUGARES_GEOGRAFICOS>(cADLUGARES_GEOGRAFICOS, "CADLUGARES_GEOGRAFICOS", "CODIGO=" + cADLUGARES_GEOGRAFICOS.CODIGO.ToString(), TipoAuditoria.Modificar);


                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADLUGARES_GEOGRAFICOS.LUGE_CODIGO), "CODIGO", "DESCRIPCION", cADLUGARES_GEOGRAFICOS.LUGE_CODIGO);

                    return View(cADLUGARES_GEOGRAFICOS);
                }
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADLUGARES_GEOGRAFICOS.LUGE_CODIGO), "CODIGO", "DESCRIPCION", cADLUGARES_GEOGRAFICOS.LUGE_CODIGO);
                return View(cADLUGARES_GEOGRAFICOS);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                return View(cADLUGARES_GEOGRAFICOS);
            }
        }

        // GET: CADLUGARES_GEOGRAFICOS/Delete/5
        [SecadPlusAutorizador("/LugaresGeograficos/Delete")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = await db.CADLUGARES_GEOGRAFICOS.FindAsync(id);
            if (cADLUGARES_GEOGRAFICOS == null)
            {
                return HttpNotFound();
            }
            return View(cADLUGARES_GEOGRAFICOS);
        }

        [Authorize]
        public async Task<ActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = await db.CADLUGARES_GEOGRAFICOS.FindAsync(id);
            if (cADLUGARES_GEOGRAFICOS == null)
            {
                return HttpNotFound();
            }
            return View("Delete", null, cADLUGARES_GEOGRAFICOS);
        }

        // POST: CADLUGARES_GEOGRAFICOS/Delete/5
        //[HttpPost, ActionName("DeleteConfirmed")]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public JsonResult DeleteConfirmed(int id)
        {
            try
            {
                string message = string.Empty;
                var lugaresHijos = (from t in db.CADLUGARES_GEOGRAFICOS.Where(t => t.LUGE_CODIGO == id) select t.CODIGO).ToList();
                var comunasHijas = (from t in db.CADCOMUNAS.Where(t => t.LUGE_CODIGO == id) select t.CODIGO).ToList();
                var barriosHijos = (from t in db.CADBARRIOS.Where(t => t.LUGE_CODIGO == id) select t.CODIGO).ToList();
                if (lugaresHijos.Count > 0)
                {
                    message = "No se puede borrar el registro mientras existan lugares dependientes de éste";
                    return Json(new { result = false, datos = message });
                }
                else if (comunasHijas.Count > 0)
                {
                    message = "No se puede borrar el registro mientras existan comunas dependientes de éste";
                    return Json(new { result = false, datos = message });
                }
                else if (barriosHijos.Count > 0)
                {
                    message = "No se puede borrar el registro mientras existan barrios dependientes de éste";
                    return Json(new { result = false, datos = message });
                }
                else
                {
                    CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = db.CADLUGARES_GEOGRAFICOS.Find(id);
                    if (cADLUGARES_GEOGRAFICOS != null)
                        db.CADLUGARES_GEOGRAFICOS.Remove(cADLUGARES_GEOGRAFICOS);
                    db.SaveChanges();



                    CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS_copy = new CADLUGARES_GEOGRAFICOS();
                    cADLUGARES_GEOGRAFICOS_copy = (CADLUGARES_GEOGRAFICOS)auditoria.GenerarCopiaAuditoria(cADLUGARES_GEOGRAFICOS);

                    auditoria.RegistrarAuditoria<CADLUGARES_GEOGRAFICOS>(cADLUGARES_GEOGRAFICOS_copy, "CADLUGARES_GEOGRAFICOS", "CODIGO=" + cADLUGARES_GEOGRAFICOS.CODIGO.ToString(), TipoAuditoria.Eliminar);



                    return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
                }
            }
            catch (DBConcurrencyException ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json("Error");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json("Error");
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