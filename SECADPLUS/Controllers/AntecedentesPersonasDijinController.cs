using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class AntecedentesPersonasDijinController : Controller
    {
        public string vctrinpec = "";

        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorGeneral objGestorGral;
        private Validador validar = new Validador();
        CADUSUARIO usr;
        // GET: CADBIENES
        [SecadPlusAutorizador("/AntecedentesPersonasDijin/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;

                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();
                if (usr == null)
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
                if (usr.CADCANA_CODIGO == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_CANAL_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            return View("Index", "BaseRecepcion");
        }

        /// <summary>
        /// Metodo que obtiene patrullas
        /// </summary>
        /// <param name="strCnTodos"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerPatrullas(string strCnTodos = null, string strConsulta = null)
        {
            try
            {
                if (strCnTodos == null)
                    strCnTodos = "cp";

                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttEntidades;
                List<Ext_CadPatrulla> listaPatrullas = new List<Ext_CadPatrulla>();
                GestorGeneral objGestorGral = new GestorGeneral();
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    validar.IdentificarCliente(Request, Session);
                }
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (strConsulta != null && strConsulta != "" && string.IsNullOrEmpty(parametros.ConsultaValor))
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (DESC_PATRULLA, '" + strConsulta + "', 'i') ";
                }

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY DESC_PATRULLA ASC ";
                        break;

                    case 1:
                        strOrderna += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;


                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", usr.CADCANA_CODIGO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_todos", strCnTodos, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaPatrullasDijin", strConexion, "Nonquery");

                foreach (DataRow fila in dttEntidades.Rows)
                {
                    listaPatrullas.Add(new Ext_CadPatrulla(fila, true));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPatrullas });
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
        /// Metodo que obtiene policias disponibles
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerPerDisponibles(string stMedioDispSitioGraba = null, string stpMedioDispConsecutivo = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttEntidades;
                List<Ext_CadPerDisponibles> listaPerDisponibles = new List<Ext_CadPerDisponibles>();
                GestorGeneral objGestorGral = new GestorGeneral();
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();

                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    validar.IdentificarCliente(Request, Session);
                }
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (CADPOLI_CEDU_EMPLEADO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DES_POLICIA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY DES_POLICIA ASC ";
                        break;

                    case 1:
                        strOrderna += " ORDER BY CADPOLI_CEDU_EMPLEADO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY DES_POLICIA " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                if (stMedioDispSitioGraba != null && stpMedioDispConsecutivo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medioDispSitioGraba", stMedioDispSitioGraba, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medioDispConsecutivo", stpMedioDispConsecutivo, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaPerdisponibles", strConexion, "Nonquery");

                    foreach (DataRow fila in dttEntidades.Rows)
                    {
                        listaPerDisponibles.Add(new Ext_CadPerDisponibles(fila, ""));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                        parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    }
                }
                else
                {
                    parametros.TotalRegistros = 0;
                    parametros.TotalRegistroFiltrados = 0;
                }

                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPerDisponibles });
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
        /// Metodo que trae un registro de registraduria
        /// </summary>
        /// <param name="strDatosConsulta"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult FilaNuevaBusquedaRegistraduria(string strDatosConsulta = "")
        {
            try
            {
                string[] datos = strDatosConsulta.Split(';');

                string strBoton = "<ul class='icons-list'> " +
                                        "<li class='dropdown'> " +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                                "<i class='icon-menu9'></i> " +
                                            "</button> " +
                                            "<ul class='dropdown-menu dropdown-menu-left'> " +
                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar' " +
                                                              "onclick = \"CargarAntecedentesDatosReg(_TIPO_IDENTIFICACION,_IDENTIFICACION)\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Ver Antecedentes " +
                                                    "</button> " +
                                                "</li> " +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";

                GestorAdministrador gestor;
                DataTable dttEntidades;
                List<Ext_PersonaRegistraduria> listaPerRegistraduria = new List<Ext_PersonaRegistraduria>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_anterior", datos[0], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_operador", datos[1], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_esConsulta", datos[2], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_Identificacion", datos[3], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nombres", datos[4], typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_RegistraduriaPerUnoAuno", strConexion, "Nonquery");

                foreach (DataRow fila in dttEntidades.Rows)
                {
                    strBotonDestino = strBoton.Replace("_TIPO_IDENTIFICACION", "'" + fila["TIPO_IDENTIFICACION"].ToString() + "'").Replace("_IDENTIFICACION", "'" + fila["IDENTIFICACION"].ToString() + "'");

                    listaPerRegistraduria.Add(new Ext_PersonaRegistraduria(fila, strBotonDestino));
                }

                var reseultado = Json(new { result = true, filas = listaPerRegistraduria });
                return reseultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que obtiene ordenes de captura
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strIdentificacion"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerOrdenCaptura(string strTipoIdentificacion = null, string strIdentificacion = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                int intContadorRegistros = 0;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttEntidades;
                List<Ext_OrdenCaptura> listaOrdenesCap = new List<Ext_OrdenCaptura>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length == 0 && strTipoIdentificacion != null && strIdentificacion != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoIdentificacion", strTipoIdentificacion, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_Identificacion", strIdentificacion, typeof(string)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_OrdenCapturaPerUnoAuno", strConexion, "Nonquery");

                    foreach (DataRow fila in dttEntidades.Rows)
                    {
                        intContadorRegistros += 1;
                        listaOrdenesCap.Add(new Ext_OrdenCaptura(fila, ""));
                    }
                }
                else
                {
                    parametros.TotalRegistros = 0;
                    parametros.TotalRegistroFiltrados = 0;
                    blFiltro = true;
                }

                parametros.TotalRegistros = intContadorRegistros;
                parametros.TotalRegistroFiltrados = intContadorRegistros;

                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaOrdenesCap });
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
        /// Obtiene y verifica antecedentes
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strNumeroIdentificacion"></param>
        /// <param name="strDesPatrulla"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerDatosAntecedentes(string strTipoIdentificacion = null, string strNumeroIdentificacion = null, string strDatosPatrulla = null)
        {
            try
            {
                String[] datosPatrulla = strDatosPatrulla.Split(';');
                GestorAuditoria auditor = new GestorAuditoria();
                Dictionary<string, string> datosAntecedentes = new Dictionary<string, string>();
                Dictionary<string, string> datosAntecedentesAuditoria = new Dictionary<string, string>();
                List<Tuple<string, string, Type>> parametros = new List<Tuple<string, string, Type>>();
                string strEsdijin = "";
                //string mensajePonal = "";
                //string strObservacionesInpec = "";
                //string strOrdenCaptura = "";
                string mensajePonal = ConsultarAntecedentes(strTipoIdentificacion, strNumeroIdentificacion);
                string strObservacionesInpec = ConsultarInpec(strTipoIdentificacion, strNumeroIdentificacion, datosPatrulla[0]);
                string strOrdenCaptura = VerificarOrdenesCaptura(strTipoIdentificacion, strNumeroIdentificacion);
                datosAntecedentes = ConsultarDatosPersona(strTipoIdentificacion, strNumeroIdentificacion);
                string strMaquina = "";
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();

                if (strOrdenCaptura == "VERIFIQUE A ESTA PERSONA LE FIGURA ORDEN DE CAPTURA")
                    strEsdijin = "V";
                else
                    strEsdijin = "F";
                validar.IdentificarCliente(Request, Session);
                strMaquina = Session["MAQUINA"].ToString();
                parametros.Add(new Tuple<string, string, Type>("p_sitio_graba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));
                parametros.Add(new Tuple<string, string, Type>("p_tipo_id", strTipoIdentificacion, typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_cedula", strNumeroIdentificacion, typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_consulta_siedco", "N", typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_consulta_dijin", strEsdijin, typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_consulta_inpec", vctrinpec, typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_es_policial", mensajePonal == "" ? "F" : "V", typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_fuerza", ObtenerDescFuerza(usr.CADCANA_FUERZA_ID.ToString()), typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_canal", ObtenerCanal(usr.CADCANA_FUERZA_ID.ToString(), usr.CADCANA_CODIGO.ToString()), typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("P_unde_fuerza", (datosPatrulla[1] == "" ? "0" : datosPatrulla[1]), typeof(decimal)));
                parametros.Add(new Tuple<string, string, Type>("P_unde_codigo", (datosPatrulla[2] == "" ? "0" : datosPatrulla[2]), typeof(decimal)));
                parametros.Add(new Tuple<string, string, Type>("P_codi_patrulla", (datosPatrulla[3] == "" ? "0" : datosPatrulla[3]), typeof(decimal)));
                parametros.Add(new Tuple<string, string, Type>("p_patrulla", datosPatrulla[0], typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("P_policial", datosPatrulla[4], typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_usuario", usr.USUARIO, typeof(string)));
                parametros.Add(new Tuple<string, string, Type>("p_maquina", strMaquina, typeof(string)));
                auditor.RegistrarAuditoriaAntecedente(parametros);
                var resultados = Json(new { result = true, mensajeP = mensajePonal, datosPersona = datosAntecedentes, observacionesInpec = strObservacionesInpec, mensajeOrdenCap = strOrdenCaptura });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NO_DATOS });
                //return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// consulta datos antecedentes
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strNumeroIdentificacion"></param>
        /// <returns></returns>
        public string ConsultarAntecedentes(string strTipoIdentificacion, string strNumeroIdentificacion)
        {
            try
            {
                string mensaje = "";
                Dictionary<string, string> datosAntecedentes = new Dictionary<string, string>();
                JsonResult resultados = new JsonResult();

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoid", strTipoIdentificacion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_identificacion", strNumeroIdentificacion, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_nombre", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_unidad", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_estado_empleado", null, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadantecedentes.p_perponal", strConexion, "MULTIPLE");

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosAntecedentes.Add(parameter.Item1, parameter.Item2.ToString());
                }

                if (datosAntecedentes.Count > 0 && datosAntecedentes["p_nombre"] != "null")
                {
                    mensaje = " Nombre: " + datosAntecedentes["p_nombre"].ToString() + "<br/> Unidad : " + datosAntecedentes["p_unidad"].ToString() +
                                            " <br /> Estado empleado: " + datosAntecedentes["p_estado_empleado"].ToString();
                }

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// consulta datos persona
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strNumeroIdentificacion"></param>
        /// <returns></returns>
        public Dictionary<string, string> ConsultarDatosPersona(string strTipoIdentificacion, string strNumeroIdentificacion)
        {
            try
            {

                Dictionary<string, string> datosAntecedentes = new Dictionary<string, string>();
                JsonResult resultados = new JsonResult();

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoid", strTipoIdentificacion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_identificacion", strNumeroIdentificacion, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_nombre", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_genero", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_estadoCedula", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_fechaNacimiento", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_fechaExp", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_ciudad", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_departamento", null, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_datosPersonasConDijin", strConexion, "MULTIPLE");

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosAntecedentes.Add(parameter.Item1, parameter.Item2.ToString());
                }

                return datosAntecedentes;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// consulta datos inpec
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strNumeroIdentificacion"></param>
        /// <param name="strDesPatrulla"></param>
        /// <returns></returns>
        public string ConsultarInpec(string strTipoIdentificacion, string strNumeroIdentificacion, string strDesPatrulla)
        {
            try
            {
                string observacionesInpec = "";
                Dictionary<string, string> datosAntecedentes = new Dictionary<string, string>();
                JsonResult resultados = new JsonResult();
                validar.IdentificarCliente(Request, Session);
                string strMaquina = Session["MAQUINA"].ToString();
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo_identidad", strTipoIdentificacion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero_documento", strNumeroIdentificacion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", usr.USUARIO.ToString(), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_maquina", strMaquina, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_desc_patrulla", strDesPatrulla, typeof(string)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_control", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_observacion", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_nu", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_apellido1", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_apellido2", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_nombres", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_establecimiento", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_tipo_domiciliaria", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_autoridad", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_delitos", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_situacion_juridica", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_proceso", null, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadantecedentes.p_antecedente_inpec", strConexion, "MULTIPLE");

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosAntecedentes.Add(parameter.Item1, parameter.Item2.ToString());
                }

                if (datosAntecedentes["p_control"] != "null")
                {
                    int outInt = 0;
                    if (int.TryParse(datosAntecedentes["p_control"], out outInt))
                    {
                        if (outInt > 2)
                            vctrinpec = "V";
                        else
                            vctrinpec = "F";
                    }
                }

                if (datosAntecedentes["p_observacion"] != "null")
                    observacionesInpec = datosAntecedentes["p_observacion"];

                return observacionesInpec;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que verifica ordenes de captura
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strNumeroIdentificacion"></param>
        /// <returns></returns>
        public string VerificarOrdenesCaptura(string strTipoIdentificacion, string strNumeroIdentificacion)
        {
            string mensaje = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo", strTipoIdentificacion.TrimStart().TrimEnd(), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strNumeroIdentificacion.TrimStart().TrimEnd(), typeof(string)));

                mensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadantecedentes.F_CONSULTA_ORDEN", strConexion);

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que ontiene la descripcion de la fuerza
        /// </summary>
        /// <param name="strFuerzaID"></param>
        /// <returns></returns>
        public string ObtenerDescFuerza(string strFuerzaID)
        {
            string mensaje = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerzaID, typeof(decimal)));

                mensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cad.f_desc_fuerza", strConexion);

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que obtiene descripcion canal
        /// </summary>
        /// <param name="strFuerzaID"></param>
        /// <param name="strCanalCodigo"></param>
        /// <returns></returns>
        public string ObtenerCanal(string strFuerzaID, string strCanalCodigo)
        {
            string mensaje = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerzaID, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", strCanalCodigo, typeof(decimal)));

                mensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cad.f_desc_canal", strConexion);

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public void GuardarAuditoria(Dictionary<string, string> datos)
        {
            try
            {
                Dictionary<string, string> datosAntecedentes = new Dictionary<string, string>();

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", datos["SitioGraba"], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo_id", datos["TipoIdentificacion"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", datos["NumeroIdentificacion"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consulta_siedco", datos["ConsultaSiedco"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consulta_dijin", datos["ConsultaDijin"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consulta_inpec", datos["ConsultaInpec"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_es_policial", datos["EsPolicia"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", datos["fuerza"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", datos["Canal"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_unde_fuerza", datos["undefuerza"], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_unde_codigo", datos["undecodigo"], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_codi_patrulla", datos["cadpatru_codigo"], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_patrulla", datos["dsp_patrulla"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_policial", datos["dsp_policial"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", datos["usuario"], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_maquina", datos["maquina"], typeof(string)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_consecutivo", null, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_mensaje", null, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadantecedentes.p_cadcon_personas", strConexion, "MULTIPLE");

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosAntecedentes.Add(parameter.Item1, parameter.Item2.ToString());
                }
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
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