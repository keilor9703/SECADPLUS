using Oracle.ManagedDataAccess.Client;
using SecadDatos.Areas.RNMC;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADEntities.SecadLugaresGeograficos;
using SecadModelos.Areas.RNMC;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Controllers.CNP
{
    public class CNPController : Controller
    {
        private DesRepMinEntities contextoRecepcion = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities contextoSecad = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private EntitiesComunasBarrios contextoCommunas = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private CADUSUARIO usuarioActual;
        private GestorGeneral objGestorGral;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private Validador validar = new Validador();
        private string strMensaje = "";
        // GET: CNP
        public ActionResult Inicio(int intSitio, decimal dcNumEvento)
        {
            try
            {
                ViewBag.ArticuloId = new SelectList(new DbRnmc().F_GetArticulos().OrderBy(a => a.Id_Comportamiento).ToList(), "ID_COMPORTAMIENTO", "Descripcion");
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validar.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                    strMensaje = "";
                }                
                ViewBag.USUARIO = usuarioActual.USUARIO;
                ViewBag.MAQUINA = Session["MAQUINA"].ToString();
                ViewBag.Evento = dcNumEvento;
                ViewBag.Sitio = intSitio;
                return View("Inicio", "BaseRecepcion");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }


        [Authorize]
        [HttpPost]
        public JsonResult TraerNumerales(int V_Articulo)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            foreach (var item in new DbRnmc().F_GetNumeral_Id(V_Articulo).OrderBy(n => n.Id_Comportamiento))
            {
                lista.Add(new SelectListItem
                {
                    Text = item.Descripcion.ToString(),
                    Value = item.Id_Comportamiento.ToString()
                });
            }
            lista.Add(new SelectListItem { Text = "Numeral no seleccionado", Value = "0" });

            return Json(lista.OrderBy(x => x.Value));
        }

        [Authorize]
        [HttpPost]
        public JsonResult TraerLiterales(int V_Numeral)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            foreach (var item in new DbRnmc().F_GetNumeral_Id(V_Numeral).OrderBy(n => n.Id_Comportamiento))
            {
                lista.Add(new SelectListItem
                {
                    Text = item.Descripcion.ToString(),
                    Value = item.Id_Comportamiento.ToString()
                });
            }
            lista.Add(new SelectListItem { Text = "Literal no seleccionado", Value = "0" });

            return Json(lista.OrderBy(x => x.Value));
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarPersona(string strCedula)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                string strNombre = "";
                var resultado = (from registro in contextoCommunas.CADPOLICIAS
                                 where registro.CEDU_EMPLEADO == strCedula
                                 select new { registro.GRAD_ALFABETICO, registro.NOMB_EMPLEADO }).FirstOrDefault();
                if (resultado != null)
                {
                    strNombre = resultado.GRAD_ALFABETICO + ". " + resultado.NOMB_EMPLEADO;
                    resultados = Json(new { result = true, datos = strNombre });
                }
                else
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.CNP_CEDULA_NO_ENCONTRADA });
                }
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
        public JsonResult ValidarComparendo(string strCedula, string strComparendo)
        {
            try
            {
                //Consulta el turno seleccionado
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strMensaje = "";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("v_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_comparendo", strComparendo, typeof(string)));
                strMensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_validar_comparendo", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                resultados = Json(new { result = true, datos = strMensaje });
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
        public JsonResult ConsultarComportamientos(string strNumero = null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtComportamiento> elementos = new List<ExtComportamiento>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "", strNombre = "", strCodigo = "", strComplemento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;
                if (!string.IsNullOrEmpty(strNumero))
                {
                    strComplemento = "WHERE ID_COMPORTAMIENTO=" + strNumero + " ";
                }
                string strBoton = "";
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
                        strQuery = " WHERE (INTERNO.ID_COMPORTAMIENTO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.descripcion, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.dominio, '" + parametros.ConsultaValor + "', 'i'))";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.ID_COMPORTAMIENTO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.dominio " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.descripcion " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.ID_COMPORTAMIENTO " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                if (!string.IsNullOrEmpty(strNumero))
                {
                    strLimites = "";
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consulta_numero", strComplemento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_comportamientos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                    "onclick = \"SeleccionarComportamiento( " + fila["ID_COMPORTAMIENTO"].ToString() + ",'" + fila["descripcion"].ToString() + "')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                    " Seleccionar </button>";
                    elementos.Add(new ExtComportamiento(fila, strBoton));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    if (!string.IsNullOrEmpty(strNumero))
                    {
                        strCodigo = fila["ID_COMPORTAMIENTO"].ToString();
                        strNombre = fila["descripcion"].ToString();
                    }
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                if (!string.IsNullOrEmpty(strNumero))
                {
                    resultados = Json(new { result = true, datos = new { CODIGO = strCodigo, NOMBRE = strNombre } });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarRegistraduria(string strTipo, string strDocumento)
        {
            try
            {
                //Consulta el turno seleccionado
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strMensaje = "";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = GestorBase.MSN_ALERTA_COMUN });
                }
                //Valida las conexiones de Dijin
                if (!validar.VerificarConexionesPedidos("CONEXION_DIJIN", usuarioActual.SITIO_GRABACION, "01"))
                {
                    resultados = Json(new { result = false, datos = "CONEXION DIJIN CERRADA.." });
                }
                else
                {
                    objGestorGral = new GestorGeneral();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_tipoIdentificacion", strTipo, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_identificacion", strDocumento, typeof(string)));
                    strMensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_registraduria_cnp", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                    resultados = Json(new { result = true, datos = strMensaje });
                }

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
        public JsonResult ConsultarCantComparendos(string strComparendo, string strTipo, string strDocumento)
        {
            try
            {
                //Consulta el turno seleccionado
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strMensaje = "";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = GestorBase.MSN_ALERTA_COMUN });
                }
                //Valida las conexiones de Dijin
                if (!validar.VerificarConexionesPedidos("CONEXION_DIJIN", usuarioActual.SITIO_GRABACION, "01"))
                {
                    resultados = Json(new { result = false, datos = "CONEXION DIJIN CERRADA.." });
                }
                else
                {
                    objGestorGral = new GestorGeneral();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_orden_comp", strComparendo, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoid", strTipo, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_id", strDocumento, typeof(string)));
                    strMensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_nro_cnp_pers", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                    resultados = Json(new { result = true, datos = strMensaje });
                }

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
        [ValidateAntiForgeryToken]
        public JsonResult Crear([Bind(Include = "CNP_ORDEN_COMPARENDO,CADEVEN_SITIOGRABA,CADEVEN_NUMEEVENTO,CNP_COMPORTAMIENTO_ID,TIPO_IDPERSONA,IDPERSONA,NOMBRE_PERSONA,APELLIDOS_PERSONA,CADPOLI_CEDULA,USUA_GRABA,FECHA_GRABACION,MAQUINA_GRABACION,VALIDADO_CNP,PERSONA_REGISTRADURIA")] CADEVENTOS_CNP_COMP cADEVENTOS_CNP_COMP)
        {
            try
            {
                objGestorGral = new GestorGeneral();
                JsonResult resultados = new JsonResult();
                DateTime fechaActual;
                string strErrores = "";
                if (ModelState.IsValid)
                {
                    //Verifica que no exista otro comparendo ya creado
                    var existencia = (from registro in contextoRecepcion.CADEVENTOS_CNP_COMP
                                      where registro.CNP_ORDEN_COMPARENDO == cADEVENTOS_CNP_COMP.CNP_ORDEN_COMPARENDO
                                      select new { registro.CNP_ORDEN_COMPARENDO }).FirstOrDefault();
                    if (existencia != null)
                    {
                        resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO });

                    }
                    else
                    {
                        fechaActual = (DateTime)objGestorGral.ObtenerTiempoServidor;
                        cADEVENTOS_CNP_COMP.FECHA_GRABACION = fechaActual;
                        contextoRecepcion.CADEVENTOS_CNP_COMP.Add(cADEVENTOS_CNP_COMP);
                        contextoRecepcion.SaveChanges();
                        resultados = Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADEVENTOS_CNP_COMP.CNP_ORDEN_COMPARENDO) });
                    }
                }
                else
                {
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            strErrores += "\n" + error.ErrorMessage;
                        }
                    }
                    resultados = Json(new { result = false, datos = strErrores });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var name = ve.PropertyName;
                        var mensaje = ve.ErrorMessage;
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }

        }
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarComparendos(string dcNumEvento)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtComparendo> elementos = new List<ExtComparendo>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "";
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
                        strQuery = " WHERE (INTERNO.CADPOLI_CEDULA = '" + parametros.ConsultaValor + "' OR INTERNO.CNP_ORDEN_COMPARENDO = '" + parametros.ConsultaValor + "' OR INTERNO.EVENTO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.NOMB_EMPLEADO, '" + parametros.ConsultaValor + "', 'i'))";
                    }
                }
                if (!string.IsNullOrEmpty(dcNumEvento))
                {
                    blFiltro = true;
                    string filtroEvento = " AND";
                    if (strQuery.Equals(""))
                    {
                        filtroEvento = " WHERE";
                    }
                    filtroEvento += " INTERNO.EVENTO = " + dcNumEvento;
                    strQuery += filtroEvento;
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CADPOLI_CEDULA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.NOMB_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.CNP_ORDEN_COMPARENDO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;


                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_comparendos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                    "onclick = \"SeleccionarRegistro( " + fila["CADPOLI_CEDULA"].ToString() + ",'" + fila["NOMB_EMPLEADO"].ToString() + "','" + fila["CNP_ORDEN_COMPARENDO"].ToString() +
                    "','" + fila["VALIDADO_CNP"].ToString() + "'," + fila["CNP_COMPORTAMIENTO_ID"].ToString() + ",'" + fila["COMPORTAMIENTO"].ToString() + "','"
                    + fila["TIPO_IDPERSONA"].ToString() + "'," + fila["IDPERSONA"].ToString() + ",'" + fila["NOMBRE_PERSONA"].ToString() +
                    "','" + fila["APELLIDOS_PERSONA"].ToString() + "','" + fila["USUA_GRABA"].ToString() + "','"
                    + fila["FECHA_GRABACION"].ToString() + "','" + fila["MAQUINA_GRABACION"].ToString() + "','" + fila["PERSONA_REGISTRADURIA"].ToString() + "')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                    " Seleccionar </button>";
                    elementos.Add(new ExtComparendo(fila, strBoton));
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
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarAsociados(string strComparendo, string strTipo, string strCedula)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtComparendo> elementos = new List<ExtComparendo>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "";
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
                        strQuery = " WHERE (INTERNO.CNP_COMPORTAMIENTO_ID = " + parametros.ConsultaValor + " OR INTERNO.CNP_ORDEN_COMPARENDO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') OR  REGEXP_LIKE(INTERNO.APELLIDOS_PERSONA, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.NOMBRE_PERSONA, '" + parametros.ConsultaValor + "', 'i'))";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrdenamiento += " ORDER BY INTERNO.CNP_ORDEN_COMPARENDO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CNP_COMPORTAMIENTO_ID " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.APELLIDOS_PERSONA " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.NOMBRE_PERSONA " + parametros.TipoOrdenamiento + " ";
                        break;
                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CNP_ORDEN_COMPARENDO " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;


                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nro_comparendo", strComparendo, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo", strTipo, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_comp_asociados", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {

                    elementos.Add(new ExtComparendo(fila, strBoton, true));
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
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}