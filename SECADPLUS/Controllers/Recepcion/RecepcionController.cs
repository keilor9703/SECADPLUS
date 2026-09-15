using Oracle.ManagedDataAccess.Client;
using SecadDatos.Areas.Recepcion;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;


namespace SECADPLUS.Controllers.Recepcion
{
    public class RecepcionController : Controller
    {
        static readonly HttpClient client11 = new HttpClient();
        private GestorRecepcion gestorRecepcion = new GestorRecepcion();
        private CADUSUARIO usuarioActual;
        private GestorGeneral objGestorGral;
        private Validador validar = new Validador();
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private SecadEntities contextoSecad = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities contextoRecepcion = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();

        // GET: Recepcion
        [SecadPlusAutorizador("/Recepcion/Recepcion")]
        public ActionResult Recepcion(string strMensaje = null)
        {
            try
            {
                DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                RecepcionModel modelo = new RecepcionModel();
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
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

                modelo.Conexiones = "CONEXIONES: ";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Valida la numeracion celular
                if (!validar.ValidarNumeracionCelular())
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_NUMERACION;
                    return RedirectToAction("Index", "Home");
                }
                modelo.Numeracion = validar.Numeros;
                //Valida las conexiones de Dijin y Telepol
                if (!validar.VerificarConexionesPedidos("CONEXION_DIJIN", usuarioActual.SITIO_GRABACION, "01"))
                {
                    return RedirectToAction("Index", "Home");
                }
                modelo.Conexiones += " DIJIN: " + validar.ObtenerMensaje();
                modelo.ConexionDijin = validar.ObtenerMensaje();
                //valida las conexion del mapa
                if (!validar.VerificarConexionesPedidos("CONEXION_MAPA_RECEPCION", usuarioActual.SITIO_GRABACION, "01"))
                {
                    //return RedirectToAction("Index", "Home");
                    //modelo.ConexionMapa = "00";
                }
                modelo.ConexionMapa = validar.ObtenerMensaje();
                //Valida las conexiones de Dijin y Telepol
                if (!validar.VerificarConexionesPedidos("CONEXION_TELEPOL", usuarioActual.SITIO_GRABACION, "01"))
                {
                    return RedirectToAction("Index", "Home");
                }
                if (usuarioActual.LUGE_CODIGO_CAD == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_SIN_CIUDAD;
                    return RedirectToAction("Index", "Home");
                }
                modelo.Conexiones += " TELEPOL: " + validar.ObtenerMensaje();
                modelo.ConexionTelepol = validar.ObtenerMensaje();
                //Obtiene ACD
                modelo.ACD = validar.ObtenerAcd(usuarioActual.SITIO_GRABACION, Session["MAQUINA"].ToString());
                if (string.IsNullOrEmpty(modelo.ACD))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_ACD;
                    return RedirectToAction("Index", "Home");
                }
                var sitio = contextoSecad.CADSITIOS_GRABACION.Find(usuarioActual.SITIO_GRABACION);
                modelo.LugarSitio = null;
                if (sitio != null)
                    modelo.LugarSitio = sitio.LUGE_CIUDAD;
                modelo.Consecutivo = "";
                modelo.FechaHora = fechaActual.ToString("dd/MM/yyyy HH:mm:ss");
                modelo.FECH_CASO = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 0, 0, 0);
                modelo.Usuario = usuarioActual.USUARIO;
                modelo.Identificacion = usuarioActual.IDENTIFICACION;
                modelo.LugeCad = usuarioActual.LUGE_CODIGO_CAD;
                modelo.Medios = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "DISPOSITIVO_COMUNICACION").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_PEDIDO = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "TIPO_PEDIDO").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CALI_PEDIDO = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALI_PEDIDO").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                modelo.Coordenadas = new SelectList(contextoRecepcion.CADPUNTOS_CARDINALES.Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                modelo.LugeDpto = usuarioActual.LUGE_CODIGO_DPTO;
                if (usuarioActual.CADCANA_FUERZA_ID == null)
                {
                    modelo.Fuerza = 0;
                }
                else
                {
                    modelo.Fuerza = usuarioActual.CADCANA_FUERZA_ID;
                }

                modelo.Canal = usuarioActual.CADCANA_CODIGO;
                modelo.SITIO_GRABA = usuarioActual.SITIO_GRABACION;

                if (usuarioActual.LUGE_CODIGO_CAD != null)
                {
                    var ciudad = (from registro in contextoSecad.CADLUGARES_GEOGRAFICOS
                                  where registro.CODIGO == usuarioActual.LUGE_CODIGO_CAD
                                  select registro).FirstOrDefault();
                    modelo.lgCiudaCodigo = usuarioActual.LUGE_CODIGO_CAD;

                    //modelo.CodDane = CodigoDane

                    if (ciudad != null)
                    {
                        modelo.Ciudad = ciudad.DESCRIPCION;
                        var Dane = ciudad.CODIGO_DANE;
                        
                        // Verificar si la longitud de CODIGO_DANE es mayor o igual a 5
                        if (!string.IsNullOrEmpty(Dane) && Dane.Length >= 5)
                        {
                            modelo.CodDane = Dane.Substring(0, 5);
                        }
                        else
                        {
                            // Manejar el caso donde la longitud es menor a 5, podrías asignar un valor por defecto o manejar el error
                            modelo.CodDane = Dane; // O podrías manejarlo de otra manera
                        }
                    }
                }

                var fecha = gestorRecepcion.ObtenerTiempoServidor;

                return View("Recepcion", "BaseRecepcion", modelo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarDatosLlamada(string strConexionDijin, string strTelefono, string strNumeracion, int intLugeCad, string strUsuario)
        {
            try
            {
                Dictionary<string, string> datosLlamada = new Dictionary<string, string>();
                DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                JsonResult resultados = new JsonResult();
                string strDir = "";
                string strCiudad = "";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nro_entrante", strTelefono, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeracion", strNumeracion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ciudadcad", intLugeCad.ToString(), typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_propietario", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_direccion", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_barrio", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_desciudabonado", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_codciudad", "0", typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_codbarrio", "0", typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_codciudbarrio", "0", typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_latitud", "0", typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_longitud", "0", typeof(decimal)));
                if (strConexionDijin.Equals("SI"))
                {
                    objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.P_DATOS_TELEFONO", strConexion, "MULTIPLE", 5);
                }
                else
                {
                    objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.P_DATOS_TELEFONO_SIN", strConexion, "MULTIPLE");
                }

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosLlamada.Add(parameter.Item1, parameter.Item2.ToString());
                    //consulta el nombre de la ciudad si existe
                    if (parameter.Item1.Equals("p_codciudad"))
                    {
                        var registro = contextoSecad.CADLUGARES_GEOGRAFICOS.Find(Convert.ToInt32(parameter.Item2.ToString()));
                        if (registro != null)
                        {
                            strCiudad = registro.DESCRIPCION;
                        }
                        datosLlamada.Add("Ciudad", strCiudad);
                    }
                    if (parameter.Item1.Equals("p_direccion"))
                    {
                        strDir = parameter.Item2.ToString();
                    }
                }
                datosLlamada.Add("FECHA_HORA", fechaActual.ToString("dd/MM/yyyy HH:mm:ss"));

                resultados = Json(new { result = true, datos = datosLlamada });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        //rmr 20-11-2020
        [Authorize]
        [HttpPost]
        public JsonResult F_GetDatosXCrimeCr(string strpabonado, string strpnumerosecad)
        {
            //var result = new DbRecepcion().F_GetDatosXCrimeCr("3115918342", "21/11/2020 10:43:11", 48954149);
            DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
            var result = new DbRecepcion().F_GetDatosXCrimeCr(strpabonado, Convert.ToString(fechaActual), Convert.ToInt64(strpnumerosecad));
            if (result.Count() > 0)
            {
                return Json(new { success = true, message = "Consulta Exitosa", dataM = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "El peso del documento supera las 25 MB ", dataM = result }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [HttpPost]
        public JsonResult F_GetDatosXCrimeCrDatos(int SitioGrabacion, int Idreg)
        {
            var resultCodig = new DbRecepcion().F_GetDatosXCrimeCodigos(SitioGrabacion, Idreg);

            if (resultCodig.Count() > 0)
            {

                return Json(new { success = true, data = resultCodig }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, data = resultCodig }, JsonRequestBehavior.AllowGet);
            }
        }
        //fin rmr 20-11-2020
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerNumeroConsecutivo(string strLlamada)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                if (string.IsNullOrEmpty(strLlamada))
                {
                    strLlamada = gestorRecepcion.ObtenerConsecutivoLlamada().ToString();
                }
                resultados = Json(new { result = true, llamada = strLlamada });
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
        public JsonResult ConsultarUbicacion(string strTelefono, string strSitio, string strLlamada, string strUsuario)
        {
            try
            {
                Dictionary<string, string> datosLlamada = new Dictionary<string, string>();
                JsonResult resultados = new JsonResult();
                objGestorGral = new GestorGeneral();
                if (string.IsNullOrEmpty(strLlamada))
                {
                    //strLlamada = gestorRecepcion.ObtenerConsecutivoLlamada().ToString();
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validar.IdentificarCliente(Request, Session))
                    {
                        resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")" });
                        return resultados;
                    }

                }
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                //Complementa la dirección
                objGestorGral.ParamsAvanzados.Clear();
                objGestorGral.ParamsSalida.Clear();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PSITIO", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PLLAMADA", strLlamada, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PUSUARIO", strUsuario, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PESTADO", "S", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PEQUIPO", Session["MAQUINA"].ToString(), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PNUMERO_ABONADO", strTelefono, typeof(decimal)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCORDX", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCORDY", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PTIPOSHAPE", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PRADIO", null, typeof(decimal)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PFECHAGMLC", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("POPERADOR", null, typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADVISOR.P_GETXYGMLCF", strConexion, "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosLlamada.Add(parameter.Item1, parameter.Item2.ToString());
                }
                datosLlamada.Add("CONSECUTIVO", strLlamada);
                resultados = Json(new { result = true, datos = datosLlamada });
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
        public JsonResult ConsultarMedioCom(string strNumero, string strNumeracion)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                resultados = Json(new { result = true, datos = gestorRecepcion.ObtenerMedioCom(strNumero, strNumeracion) });
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
        public JsonResult ConsultarCiudades(decimal dcDptoId, string strCiudadId = null)
       {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadLugares> elementos = new List<Ext_CadLugares>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "", strNombre = "", strCodigo = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarLugar('_CODIGO' ,'_NOMBRE', 'CIUDAD', '_CODIDANE')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
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
                        strQuery = " WHERE (INTERNO.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.CIUDADPADRE, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.TIPO, '" + parametros.ConsultaValor + "', 'i'))";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CIUDADPADRE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                if (!string.IsNullOrEmpty(strCiudadId))
                {
                    strQuery = " WHERE  (REGEXP_LIKE(INTERNO.CIUDADPADRE, '" + strCiudadId + "', 'i'))";
                    strLimites = "";
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_dpto", dcDptoId.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_SIVICC_MINUTAS.p_consultarciudades", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    string conversionNumDane;

                    if (fila["CODIGO_DANE"].ToString().Length < 5)
                        conversionNumDane = fila["CODIGO_DANE"].ToString();
                    else
                        conversionNumDane = fila["CODIGO_DANE"].ToString().Substring(0, 5);

                    elementos.Add(new Ext_CadLugares(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_NOMBRE", fila["CIUDADPADRE"].ToString()).Replace("_CODIDANE", conversionNumDane), true));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    if (!string.IsNullOrEmpty(strCiudadId))
                    {
                        strCodigo = fila["CODIGO"].ToString();
                        strNombre = fila["CIUDADPADRE"].ToString();
                    }
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                if (!string.IsNullOrEmpty(strCiudadId))
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
        public JsonResult ConsultarCasos(string strCodigo = null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCasosCanal> elementos = new List<ExtCasosCanal>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "", strNombre = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarCaso('_CODIGO' ,'_NOMBRE')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CODIGO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(strCodigo))
                {
                    strQuery = " WHERE (INTERNO.CODIGO = '" + strCodigo + "') ";
                    strLimites = "";
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_casos_recepcion", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new ExtCasosCanal(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_NOMBRE", fila["DESCRIPCION"].ToString())));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    if (!string.IsNullOrEmpty(strCodigo))
                    {
                        strNombre = fila["DESCRIPCION"].ToString();
                    }
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                if (!string.IsNullOrEmpty(strCodigo))
                {
                    resultados = Json(new { result = true, datos = strNombre });
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
        public JsonResult CargarTodosLosBarrios(string lugeCodigo)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick = \"CargarBarrio(CODIGO,DESCRIPCION, '_COM')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                 " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttBarrios;
                List<_BarriosModel> listaBarrios = new List<_BarriosModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                string strBotonSelect = "";
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
                        strQuery = " WHERE (REGEXP_LIKE(DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY DESCRIPCION ASC ";

                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                    case 4:
                        strOrdenamiento += " ORDER BY desc_comuna " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ciudad", lugeCodigo, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttBarrios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_barrios_recepcion", strConexion, "Nonquery");

                foreach (DataRow fila in dttBarrios.Rows)
                {
                    strBotonSelect = strBoton.Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'").Replace("_COM", fila["desc_comuna"].ToString());

                    listaBarrios.Add(new _BarriosModel("", strBotonSelect, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString(), fila["desc_comuna"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaBarrios });
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
        public JsonResult ConsultarBarrioNombre(string lugeCodigo, string strNombre)
        {
            try
            {

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick = \"CargarBarrio(CODIGO,DESCRIPCION, '_COM')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                 " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = "";
                GestorAdministrador gestor;
                DataTable dttBarrios;
                JsonResult resultados = new JsonResult();
                List<_BarriosModel> listaBarrios = new List<_BarriosModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                string strBotonSelect = "";
                //Verifica si se debe realizar búsqueda de registros
                strQuery = " WHERE (TRIM(UPPER(DESCRIPCION)) = TRIM(upper('" + strNombre + "')) ) AND ROWNUM <= 3 ";
                strOrdenamiento += " ORDER BY DESCRIPCION ASC ";

                strLimites = "";
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ciudad", lugeCodigo, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttBarrios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_barrios_recepcion", strConexion, "Nonquery");
                if (dttBarrios.Rows.Count > 1)
                {
                    resultados = Json(new { result = true, cantidad = 2 });
                }
                else if (dttBarrios.Rows.Count == 0)
                {
                    resultados = Json(new { result = true, cantidad = 0 });
                }
                else
                {
                    foreach (DataRow fila in dttBarrios.Rows)
                    {
                        resultados = Json(new
                        {
                            result = true,
                            cantidad = 1,
                            datos = new
                            {
                                codigo = fila["CODIGO"].ToString(),
                                descripcion = fila["DESCRIPCION"].ToString(),
                                comuna = fila["desc_comuna"].ToString()
                            }
                        });
                    }
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
        public JsonResult ConsultarPreguntas(string strCaso)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                string strPreguntas = "";
                strPreguntas = gestorRecepcion.ObtenerCasoPreguntas(strCaso);
                resultados = Json(new { result = true, datos = strPreguntas });
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
        public JsonResult ConsultarCanales(string strOpcion, string strNumeroLlamada = null, string strSitio = null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadCanales> elementos = new List<Ext_CadCanales>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "", strProcedimiento = ".PK_CADPLUS.p_canales_atencion", strAccion = "CREAR";
                bool blFiltro = false;

                string strBoton = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para asignar' onclick=\"_SELECCIONAR\">"
                + "<i class='icon-arrow-up13'></i></button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    strQuery = " WHERE (REGEXP_LIKE(INTERNO.CANA_ATENCION, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.ENT, '" + parametros.ConsultaValor + "', 'i'))";
                }

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.ENT " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.CANA_ATENCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CANA_ATENCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                if (strOpcion.Equals("PONAL"))
                {
                    strProcedimiento = ".PK_CADPLUS.p_canales_ponal";
                }
                else if (strOpcion.Equals("ASIGNADOS"))
                {
                    strProcedimiento = ".PK_CADPLUS.p_canales_asignados";
                    strBoton = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para asignar' onclick=\"_SELECCIONAR\">"
                    + "<i class='icon-arrow-down132'></i></button>";
                    strAccion = "REMOVER";
                }
                else if (strOpcion.Equals("CONSULTA"))
                {
                    strProcedimiento = ".PK_CADPLUS.p_canales_asignados";
                    strBoton = "";
                    strAccion = "REMOVER";
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nume_llamada", strNumeroLlamada, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));

                // ==============================================================================================
                // INICIO BLOQUE DE IMPRESIÓN EN CONSOLA (PARAMETROS DE ENTRADA)
                // ==============================================================================================
                System.Diagnostics.Debug.WriteLine("\n[DEBUG SP] === EJECUTANDO PROCEDIMIENTO ALMACENADO ===");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP] Nombre SP:      {Resources.RecursoGeneral.BaseDats_Usuario + strProcedimiento}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP] p_nume_llamada: {strNumeroLlamada}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP] p_sitio_graba:  {strSitio}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP] p_ordenamiento: {strOrdenamiento}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP] p_complemento:  {strQuery}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP] p_limites:      {strLimites}");
                System.Diagnostics.Debug.WriteLine("[DEBUG SP] ===================================================\n");
                // ==============================================================================================

                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + strProcedimiento, strConexion, "Nonquery");

                // ==============================================================================================
                // INICIO BLOQUE DE IMPRESIÓN EN CONSOLA (RESULTADOS DE SALIDA)
                // ==============================================================================================
                int cantidadFilas = dttLugar != null ? dttLugar.Rows.Count : 0;
                System.Diagnostics.Debug.WriteLine($"\n[DEBUG SP RESULTADOS] === RESPUESTA DEL PROCEDIMIENTO ===");
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP RESULTADOS] Cantidad de registros devueltos: {cantidadFilas}");

                if (dttLugar != null && cantidadFilas > 0)
                {
                    // 1. Obtener e imprimir los nombres de las columnas
                    System.Collections.Generic.List<string> nombresColumnas = new System.Collections.Generic.List<string>();
                    foreach (System.Data.DataColumn col in dttLugar.Columns)
                    {
                        nombresColumnas.Add(col.ColumnName);
                    }
                    System.Diagnostics.Debug.WriteLine($"[DEBUG SP RESULTADOS] COLUMNAS: {string.Join(" | ", nombresColumnas)}");
                    System.Diagnostics.Debug.WriteLine("[DEBUG SP RESULTADOS] ---------------------------------------------------");

                    // 2. Iterar e imprimir cada fila de datos
                    int indexFila = 1;
                    foreach (System.Data.DataRow filaDebug in dttLugar.Rows)
                    {
                        System.Collections.Generic.List<string> valoresFila = new System.Collections.Generic.List<string>();
                        foreach (var item in filaDebug.ItemArray)
                        {
                            valoresFila.Add(item?.ToString() ?? "NULL");
                        }
                        System.Diagnostics.Debug.WriteLine($"[DEBUG SP RESULTADOS] Fila {indexFila}: {string.Join(" | ", valoresFila)}");
                        indexFila++;
                    }
                }
                System.Diagnostics.Debug.WriteLine("[DEBUG SP RESULTADOS] ===================================================\n");
                // ==============================================================================================

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new Ext_CadCanales(fila, strBoton.Replace("_SELECCIONAR", "GestionarCanal(" + fila["FUERZA"].ToString() + "," + fila["CANAL"].ToString() + ", '" + strAccion + "')"), strOpcion));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }

                //Verifica si se ha aplicado algun filtro a la consulta
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
                System.Diagnostics.Debug.WriteLine($"[DEBUG SP ERROR] Ocurrió una excepción: {ex.Message}");
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }



        [Authorize]
        [HttpPost]
        public JsonResult CrearPedido([Bind(Include = "SITIO_GRABA,NUME_LLAMADA,HORA_CASO,DISP_TELEFONICO,CELDA_MARCACION,NUME_TELEFONO,PROP_TELEFONO,DIRE_TELEFONO,LUGE_TELEFONO,BARR_TELEFONO,TIPO_PEDIDO,CALI_PEDIDO,TIPO_INFORMACION,CODIGO_COOPERANTE,NOMB_LLAMANTE,DIRE_LLAMANTE,LUGE_CODIGO,DIRE_CASO,PUCA_CODIGO,CODI_BARRIO,LUGE_BARRIO,LATITUD_CASO,LONGITUD_CASO,COMENTARIO,CODI_PEDIDO,CODI_PEDIDO2,CODI_PEDIDO_REAL,IMPORTANCIA,PRIORIDAD,ESTADO,ENVIAR,CADUSUA_USUARIO,CADPOLI_CEDU_EMPLEADO,CADPEDI_SITIO_GRABA,CADPEDI_NUME_LLAMADA,FECHA_GRABACION,FECH_CASO,TOTAL_CANALES,TIPO_GENERO")] CADPEDIDO cADPEDIDO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .SelectMany(x => x.Value.Errors.Select(e => new
                        {
                            Campo = x.Key,
                            Error = e.ErrorMessage
                        }))
                        .ToList();

                    return Json(new
                    {
                        result = false,
                        datos = "El modelo no es válido",
                        errores = errores
                    });
                }

                cADPEDIDO.FECHA_GRABACION =
                    (DateTime)gestorRecepcion.ObtenerTiempoServidor;

                cADPEDIDO.DIRE_CASO =
                    cADPEDIDO.DIRE_CASO?.Trim();

                if (cADPEDIDO.CODI_BARRIO == null)
                {
                    cADPEDIDO.LUGE_BARRIO = null;
                }

                contextoRecepcion.Database.Log = sql =>
                        System.Diagnostics.Debug.WriteLine(sql);

                contextoRecepcion.CADPEDIDOS.Add(cADPEDIDO);

                contextoRecepcion.SaveChanges();

                // ============================================
                // VERIFICAR QUE REALMENTE QUEDÓ EN LA BD
                // ============================================

                var pedidoGuardado = contextoRecepcion.CADPEDIDOS
                    .AsNoTracking()
                    .FirstOrDefault(x =>
                        x.SITIO_GRABA == cADPEDIDO.SITIO_GRABA &&
                        x.NUME_LLAMADA == cADPEDIDO.NUME_LLAMADA);

                if (pedidoGuardado == null)
                {
                    return Json(new
                    {
                        result = false,
                        datos = "SaveChanges() terminó correctamente, pero el registro no fue encontrado nuevamente en la BD.",
                        sitio = cADPEDIDO.SITIO_GRABA,
                        llamada = cADPEDIDO.NUME_LLAMADA
                    });
                }

                // ============================================
                // ÉXITO
                // ============================================

                return Json(new
                {
                    result = true,
                    datos = "Completado",
                    fechaGraba = cADPEDIDO.FECHA_GRABACION
                        .ToString("dd/MM/yyyy HH:mm:ss"),
                    sitio = cADPEDIDO.SITIO_GRABA,
                    llamada = cADPEDIDO.NUME_LLAMADA
                });
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

                GestorBase.GestionarExcepcion(
                    exE,
                    GetType().Name);

                return Json(new
                {
                    result = false,
                    datos = Resources.RecursoGeneral.EX_MENSAJE
                        .Replace("ERROR_CODE", GestorBase.EX_CODIGO)
                });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(
                    ex,
                    GetType().Name);

                return Json(new
                {
                    result = false,
                    datos = Resources.RecursoGeneral.EX_MENSAJE
                        .Replace("ERROR_CODE", GestorBase.EX_CODIGO)
                });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ActualizarPedido([Bind(Include = "SITIO_GRABA,NUME_LLAMADA,HORA_CASO,DISP_TELEFONICO,CELDA_MARCACION,NUME_TELEFONO,PROP_TELEFONO,DIRE_TELEFONO,LUGE_TELEFONO,BARR_TELEFONO,TIPO_PEDIDO,CALI_PEDIDO,TIPO_INFORMACION,CODIGO_COOPERANTE,NOMB_LLAMANTE,DIRE_LLAMANTE,LUGE_CODIGO,DIRE_CASO,PUCA_CODIGO,CODI_BARRIO,LUGE_BARRIO,LATITUD_CASO,LONGITUD_CASO,COMENTARIO,CODI_PEDIDO,CODI_PEDIDO2,CODI_PEDIDO_REAL,IMPORTANCIA,PRIORIDAD,ESTADO,ENVIAR,CADUSUA_USUARIO,CADPOLI_CEDU_EMPLEADO,CADPEDI_SITIO_GRABA,CADPEDI_NUME_LLAMADA,FECHA_GRABACION,FECH_CASO,TOTAL_CANALES,TIPO_GENERO")] CADPEDIDO cADPEDIDO)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                if (ModelState.IsValid)
                {
                    cADPEDIDO.DIRE_CASO = cADPEDIDO.DIRE_CASO.Trim().TrimEnd().TrimStart();
                    //verifica si el barrioes nulo
                    if (cADPEDIDO.CODI_BARRIO == null)
                    {
                        cADPEDIDO.LUGE_BARRIO = null;
                    }
                    contextoRecepcion.Entry(cADPEDIDO).State = EntityState.Modified;
                    contextoRecepcion.SaveChanges();
                    auditoria.RegistrarAuditoria<CADPEDIDO>(cADPEDIDO, "CADPEDIDO", "SITIO_GRABA=" + cADPEDIDO.SITIO_GRABA.ToString() + ",NUME_LLAMADA=" + cADPEDIDO.NUME_LLAMADA
                        , TipoAuditoria.Modificar);


                }
                resultados = Json(new { result = true, datos = "Completado" });
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
        public JsonResult AsignarCanal([Bind(Include = "CADPEDI_SITIOGRABA,CADPEDI_NUMELLAMADA,CADCANA_FUERZ_ID,CADCANA_CODIGO,CALI_PEDIDO,COMENTARIO,ESTADO,ENVIAR,CADEVEN_SITIO_GRABA,CADEVEN_NUME_EVENTO,FECHA_GRABACION,FECHA_MODIFICACION,USUA_MODIFICA")] CADPEDIDOS_CANALES cADPEDIDOS_CANALES)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                if (ModelState.IsValid)
                {
                    cADPEDIDOS_CANALES.COMENTARIO = null;
                    cADPEDIDOS_CANALES.ESTADO = "S";
                    cADPEDIDOS_CANALES.FECHA_GRABACION = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                    contextoRecepcion.CADPEDIDOS_CANALES.Add(cADPEDIDOS_CANALES);

                    contextoRecepcion.SaveChanges();
                }
                resultados = Json(new { result = true, datos = "Completado" });
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
        public JsonResult RemoverCanal(int intSitio, long lgNumero, long lgEntidad, long lgCanal)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                CADPEDIDOS_CANALES canalAsociado = (from registro in contextoRecepcion.CADPEDIDOS_CANALES
                                                    where registro.CADPEDI_SITIOGRABA == intSitio
                                                    && registro.CADPEDI_NUMELLAMADA == lgNumero
                                                    && registro.CADCANA_FUERZ_ID == lgEntidad
                                                    && registro.CADCANA_CODIGO == lgCanal
                                                    select registro).FirstOrDefault();
                if (canalAsociado != null)
                {
                    contextoRecepcion.CADPEDIDOS_CANALES.Remove(canalAsociado);
                    contextoRecepcion.SaveChanges();

                    CADPEDIDOS_CANALES canalAsociado_copy = new CADPEDIDOS_CANALES();
                    canalAsociado_copy = (CADPEDIDOS_CANALES)auditoria.GenerarCopiaAuditoria(canalAsociado);

                    auditoria.RegistrarAuditoria<CADPEDIDOS_CANALES>(canalAsociado_copy, "CADPEDIDOS_CANALES", "CADPEDI_SITIOGRABA=" + canalAsociado.CADPEDI_SITIOGRABA.ToString() + ",CADPEDI_NUMELLAMADA=" + canalAsociado.CADPEDI_NUMELLAMADA
                         + ",CADCANA_FUERZ_ID=" + canalAsociado.CADCANA_FUERZ_ID + ",CADCANA_CODIGO=" + canalAsociado.CADCANA_CODIGO, TipoAuditoria.Eliminar);

                }
                resultados = Json(new { result = true, datos = "Completado" });
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

        //[Authorize]
        [HttpPost]
        //[HttpGet]
        [AllowAnonymous]
        public JsonResult EnviarPedido(string strSitio, string strNumeroLlamada, string strTelefono, string strPedido)
        {
            try
            {
                //CADPEDIDO obj_wsdl;
                //CADPEDIDO pedido11 = null;
                RecepcionModel modelo11 = new RecepcionModel();
                JsonResult resultados = new JsonResult();
                bool blValor = false;
                bool blValorws = false;
                string strMensaje = "";
                string wsld = "";
                decimal idFuerzaEntidad = 0;
                string resp_ref_abreviada = "";

                blValor = gestorRecepcion.EnviarLlamada(strSitio, strNumeroLlamada, strTelefono, strPedido, ref idFuerzaEntidad, ref resp_ref_abreviada);
                if (idFuerzaEntidad == 64570007 && resp_ref_abreviada == "01")
                {
                    wsld = wsdlnew(strSitio, "DBL_MEPER", strNumeroLlamada, 0, idFuerzaEntidad, 1);
                    blValorws = gestorRecepcion.InsertaHistoricoWs(strSitio, "DBL_MEPER", strNumeroLlamada, Convert.ToString(idFuerzaEntidad), wsld);
                }
                if (!GestorBase.MSN_ALERTA_COMUN.Equals(""))
                {
                    strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    resultados = Json(new { result = false, datos = strMensaje, datowsdl = wsld });
                }
                else
                {
                    resultados = Json(new { result = true, datos = strMensaje, datowsdl = wsld });
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

        [HttpGet]
        [AllowAnonymous]
        public string wsdlnew(string strSitio, string usuario, string strNumeroLlamada, decimal NumeroEvento, decimal FuerzaRetorno, decimal canal)
        {
            /*
            PARAMETROS
            string urlget = "https://catalogoservicioweb.policia.gov.co/WsIntegracionSecadMeper/api/Secad/ValEvento?pSitioGraba=" + strSitio + "&pUsuario=" + usuario + "&pNumLlamada=" + strNumeroLlamada + "&pNumeroEvento=" + NumeroEvento + "&pFuerza=" + FuerzaRetorno + "&pCanal=" + canal;
            DIRECTO
            string urlget = "https://catalogoservicioweb.policia.gov.co/WsIntegracionSecadMeper/api/Secad/ValEvento?pSitioGraba=26&pUsuario=DBL_MEPER&pNumLlamada=16813622&pNumeroEvento=0&pFuerza=64570007&pCanal=1";
            */
            //string urlget = "https://catalogoservicioweb.policia.gov.co/WsIntegracionSecadMeper/api/Secad/ValEvento?pSitioGraba=26&pUsuario=DBL_MEPER&pNumLlamada=16813622&pNumeroEvento=0&pFuerza=64570007&pCanal=1";
            string urlget = "https://catalogoservicioweb.policia.gov.co/WsIntegracionSecadMeper/api/Secad/ValEvento?pSitioGraba=" + strSitio + "&pUsuario=" + usuario + "&pNumLlamada=" + strNumeroLlamada + "&pNumeroEvento=" + NumeroEvento + "&pFuerza=" + FuerzaRetorno + "&pCanal=" + canal;
            string ResponseString = "";
            HttpWebResponse response = null;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var request = (HttpWebRequest)WebRequest.Create(urlget);
                request.Accept = "application/json"; //"application/xml";"application/json"
                response = (HttpWebResponse)request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        ResponseString = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)ex.Response;
                    ResponseString = "Some error occured: " + response.StatusCode.ToString();
                }
                else
                {
                    ResponseString = "Some error occured: " + ex.Status.ToString();
                }
            }

            return " Respuesta Servicio SEM = " + ResponseString;
        }



        /// <summary>
        /// consultar ELS a RapidSOS para obtener la ubicación del celular
        /// </summary>
        /// <param name="strNumeroTelefono"></param>
        /// <returns></returns>

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ObtenerUbicacionCelular(string strNumeroTelefono)
        {
            JsonResult resultados = new JsonResult();
            string strLatitud = "";
            string strLongitud = "";
            string strMensaje = "";
            bool blValor = false;

            try
            {
                //string urlget = "https://api-sandbox.rapidsos.com/v1/rem/trigger/hook/antioquia/trigger?caller_id="
                //                 + strNumeroTelefono + "&case_id=abc123";


                // 1. Obtener la URL base y armar la ruta
                string urlBase = ConfigurationManager.AppSettings["RapidSOS_ApiUrl"];
                string urlget = $"{urlBase}?caller_id={strNumeroTelefono}&case_id=abc123";


                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var request = (HttpWebRequest)WebRequest.Create(urlget);
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentLength = 0; // no hay body, solo query params

                // Credenciales - Basic Auth
                //string strId = "tGhPlwUpEWB4vGPwHzJ4p4ALarD6PHeD";
                //string strSecret = "96aGGLNuOEGq8YVA";
                //string strCredenciales = Convert.ToBase64String(Encoding.ASCII.GetBytes(strId + ":" + strSecret));
                //request.Headers.Add("Authorization", "Basic " + strCredenciales);


                // 2. Obtener credenciales del Web.config
                string strId = ConfigurationManager.AppSettings["RapidSOS_Id"];
                string strSecret = ConfigurationManager.AppSettings["RapidSOS_Secret"];
                string strCredenciales = Convert.ToBase64String(Encoding.ASCII.GetBytes(strId + ":" + strSecret));
                request.Headers.Add("Authorization", "Basic " + strCredenciales);


                string responseString = "";
                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        responseString = reader.ReadToEnd();
                    }
                }
                catch (WebException exWeb)
                {
                    if (exWeb.Status == WebExceptionStatus.ProtocolError)
                    {
                        var errResponse = (HttpWebResponse)exWeb.Response;
                        if (errResponse.StatusCode == HttpStatusCode.NotFound)
                            strMensaje = "El dispositivo no reportó ubicación o no es compatible.";
                        else
                            strMensaje = "Error del servicio de ubicación: " + errResponse.StatusCode;
                    }
                    else
                    {
                        strMensaje = "Error de conexión con el servicio de ubicación: " + exWeb.Status;
                    }
                }

                if (!string.IsNullOrEmpty(responseString))
                {
                    dynamic jsonResp = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);

                    if (jsonResp?.results != null && jsonResp.results.Count > 0)
                    {
                        var primerResultado = jsonResp.results[0];
                        strLatitud = primerResultado?.latitude != null ? primerResultado.latitude.ToString() : "";
                        strLongitud = primerResultado?.longitude != null ? primerResultado.longitude.ToString() : "";
                    }

                    if (!string.IsNullOrEmpty(strLatitud) && !string.IsNullOrEmpty(strLongitud))
                        blValor = true;
                    else if (string.IsNullOrEmpty(strMensaje))
                        strMensaje = "El servicio no devolvió coordenadas para este número.";
                }

                resultados = Json(new { result = blValor, latitud = strLatitud, longitud = strLongitud, datos = strMensaje });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, latitud = "", longitud = "", datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }


        [Authorize]
        [HttpPost]
        public JsonResult ValidarPedidoExistente(string strFechaCaso, string strCodigoCaso, string strDireCaso, string strLugeCodigo)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                if (gestorRecepcion.PedidoRepetido(strFechaCaso, strCodigoCaso, strDireCaso, strLugeCodigo))
                {
                    resultados = Json(new { result = true, datos = new { SITIO = gestorRecepcion.Sitio, LLAMADA = gestorRecepcion.NumeroLlamada } });
                }
                else
                {
                    resultados = Json(new { result = false, datos = "NUEVO" });
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
        public JsonResult ConsultarLlamada(int intSitio, string strACD)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                long lgNumero = 0;
                lgNumero = gestorRecepcion.ConsultarLlamadaEntrante(intSitio, strACD);
                resultados = Json(new { result = true, datos = new { NUMERO = lgNumero } });
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
        public JsonResult ConsultarComentarios(string strSitio, string strNumeroLlamada)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtPedidoComentario> elementos = new List<ExtPedidoComentario>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                bool blFiltro = false;

                string strBoton = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para asignar' onclick=\"_SELECCIONAR\">"
                + "<i class='icon-pencil'></i></button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    strQuery = " WHERE (REGEXP_LIKE(INTERNO.ANOTACION, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.USUA_USUARIO, '" + parametros.ConsultaValor + "', 'i'))";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.ANOTACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.USUA_USUARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nume_llamada", strNumeroLlamada, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));

                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_pedido_comentarios", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new ExtPedidoComentario(fila, strBoton.Replace("_SELECCIONAR", "EditarComentario(" + strSitio + "," + strNumeroLlamada + "," + fila["CONSECUTIVO"].ToString() + ",'" + fila["ANOTACION"].ToString() + "')")));
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

        [Authorize]
        [HttpPost]
        public JsonResult GuardarComentario([Bind(Include = "CADPEDI_SITIOGRABA,CADPEDI_NUMELLAMADA,CONSECUTIVO,CODI_PREGUNTA,RTA_PREGUNTA,ANOTACION,FECHA_GRABACION,USUA_USUARIO")] CADPEDIDOS_INFORMACION cADPEDIDOS_INFORMACION)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                int intConsecutivo = 1;
                //Verifica si el registro existe
                decimal registro = (from comentario in contextoRecepcion.CADPEDIDOS_INFORMACION
                                    where comentario.CADPEDI_SITIOGRABA == cADPEDIDOS_INFORMACION.CADPEDI_SITIOGRABA
                                    && comentario.CADPEDI_NUMELLAMADA == cADPEDIDOS_INFORMACION.CADPEDI_NUMELLAMADA
                                    && comentario.CONSECUTIVO == cADPEDIDOS_INFORMACION.CONSECUTIVO
                                    select comentario).Count();
                if (registro > 0)
                {
                    if (ModelState.IsValid)
                    {
                        //Actualiza el registro
                        //cADPEDIDOS_INFORMACION.FECHA_GRABACION = ((DateTime)gestorRecepcion.ObtenerTiempoServidor());
                        contextoRecepcion.Entry(cADPEDIDOS_INFORMACION).State = EntityState.Modified;
                        contextoRecepcion.SaveChanges();
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var valor = contextoRecepcion.CADPEDIDOS_INFORMACION.Where(c => c.CADPEDI_SITIOGRABA == cADPEDIDOS_INFORMACION.CADPEDI_SITIOGRABA && c.CADPEDI_NUMELLAMADA == cADPEDIDOS_INFORMACION.CADPEDI_NUMELLAMADA).Select(c => new { c.CONSECUTIVO }).OrderByDescending(c => c.CONSECUTIVO).FirstOrDefault();
                        if (valor != null)
                        {
                            intConsecutivo = valor.CONSECUTIVO + 1;
                        }
                        cADPEDIDOS_INFORMACION.CONSECUTIVO = intConsecutivo;
                        cADPEDIDOS_INFORMACION.FECHA_GRABACION = ((DateTime)gestorRecepcion.ObtenerTiempoServidor);
                        contextoRecepcion.CADPEDIDOS_INFORMACION.Add(cADPEDIDOS_INFORMACION);
                        contextoRecepcion.SaveChanges();
                    }
                }

                resultados = Json(new { result = true, datos = "Completado" });
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

        [SecadPlusAutorizador("/Recepcion/ConsultaPedidos")]
        public ActionResult ConsultaPedidos(string strOpcion = null)
        {
            try
            {
                ViewBag.Servicios = "CONSULTA";
                if (!string.IsNullOrEmpty(strOpcion))
                {
                    ViewBag.Servicios = strOpcion;
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { strOpcion = "REVISION" });
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Sitio = usuarioActual.SITIO_GRABACION;
                ViewBag.Usuario = usuarioActual.USUARIO;
                return View("ConsultarPedidos", "BaseRecepcion");

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarPedidos(string strSitio, string strUsuario, string strOpcion = null, string strCasoPedido = null, string strAbonado = null, string strFecha = null, string strDesc = null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Pedidos> elementos = new List<Pedidos>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                DateTime dtControl;
                bool blFiltro = false, blControl = false, blDtControl = false; ;
                string strBoton = "<a class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para ver el pedido' href='_RUTA' target='_blank'>"
                + "<i class='icon icon-eye'></i></a>";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(strCasoPedido))
                {
                    strQuery += " AND PEDIDO.CODI_PEDIDO ='" + strCasoPedido + "' ";
                    blFiltro = true;
                }
                if (!string.IsNullOrEmpty(strAbonado))
                {
                    strQuery += " AND PEDIDO.NUME_TELEFONO ='" + strAbonado + "' ";
                    blFiltro = true;
                }
                if (!string.IsNullOrEmpty(strFecha))
                {
                    strQuery += " AND PEDIDO.HORA_CASO BETWEEN TO_DATE('" + strFecha + " 00:00:00" + "', 'DD/MM/YYYY hh24:mi:ss') AND TO_DATE('" + strFecha + " 23:59:59" + "', 'DD/MM/YYYY hh24:mi:ss') ";
                    blFiltro = true;
                }
                if (!string.IsNullOrEmpty(strDesc))
                {
                    strQuery += " AND  REGEXP_LIKE(PEDIDO.COMENTARIO, '" + strDesc + "', 'i') ";
                    blFiltro = true;
                }
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    //valida si debe filtrar por fecha
                    blDtControl = DateTime.TryParse(parametros.ConsultaValor, out dtControl);
                    if (blDtControl)
                    {

                        strQuery += " AND (PEDIDO.HORA_CASO =  to_date('" + parametros.ConsultaValor + "','DD/MM/YYYY HH24:MI:SS')) ";
                    }
                    else if (blControl)
                    {
                        strQuery += " AND (PEDIDO.NUME_LLAMADA = " + parametros.ConsultaValor + " OR PEDIDO.NUME_TELEFONO= " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery += " AND UPPER(CADUSUA_USUARIO) ='" + parametros.ConsultaValor.ToUpper() + "' ";
                    }

                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY PEDIDO.NUME_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY PEDIDO.NUME_TELEFONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY PEDIDO.CODI_PEDIDO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 5:
                        strOrdenamiento += " ORDER BY PEDIDO.CADPEDI_NUME_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY PEDIDO.SITIO_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY PEDIDO.PROP_TELEFONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdenamiento += " ORDER BY PEDIDO.HORA_CASO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrdenamiento += " ORDER BY PEDIDO.CADUSUA_USUARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY PEDIDO.HORA_CASO DESC  ";
                        break;
                }
                //strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                strLimites = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", strUsuario, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_casos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);

                foreach (DataRow fila in dttLugar.Rows)
                {
                    if (strOpcion.Equals("REVISION"))
                    {
                        elementos.Add(new Pedidos(fila, strBoton.Replace("_RUTA", Url.Action("Consulta", "Recepcion", new { intSitio = fila["SITIO_GRABA"].ToString(), lgLlamada = fila["NUME_LLAMADA"].ToString(), strOpcion = strOpcion }))));
                    }
                    else
                    {
                        elementos.Add(new Pedidos(fila, strBoton.Replace("_RUTA", Url.Action("Consulta", "Recepcion", new { intSitio = fila["SITIO_GRABA"].ToString(), lgLlamada = fila["NUME_LLAMADA"].ToString() }))));
                    }
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString()); ;
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

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarAsociadas(string strSitio, string strHoraCaso, string strLlamada)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_LlamadaAsociada> elementos = new List<Ext_LlamadaAsociada>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarAsociada(NUMERO , SITIO)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
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
                        strQuery = " WHERE (INTERNO.NUME_LLAMADA = " + parametros.ConsultaValor + " OR INTERNO.CALI_PEDIDO= " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.CIUDAD, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.NOMB_LLAMANTE, '" + parametros.ConsultaValor + "', 'i'))";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.NUME_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.HORA_CASO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.NUME_TELEFONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.NUME_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_llamada", strLlamada, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_caso", strHoraCaso, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_asociadas", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new Ext_LlamadaAsociada(fila, strBoton.Replace("NUMERO", fila["NUME_LLAMADA"].ToString()).Replace("SITIO", fila["SITIO_GRABA"].ToString())));
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

        [SecadPlusAutorizador("/Recepcion/Consulta")]
        public ActionResult Consulta(int intSitio, long lgLlamada, string strOpcion = null)
        {
            try
            {
                DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                RecepcionModel modelo = new RecepcionModel();
                CADPEDIDO pedido = null;
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

                modelo.Conexiones = "CONEXIONES: ";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Valida la numeracion celular
                if (!validar.ValidarNumeracionCelular())
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_NUMERACION;
                    return RedirectToAction("Index", "Home");
                }
                modelo.Numeracion = validar.Numeros;
                //Valida las conexiones de Dijin y Telepol
                if (!validar.VerificarConexionesPedidos("CONEXION_DIJIN", usuarioActual.SITIO_GRABACION, "01"))
                {
                    return RedirectToAction("Index", "Home");
                }
                modelo.Conexiones += " DIJIN: " + validar.ObtenerMensaje();
                modelo.ConexionDijin = validar.ObtenerMensaje();
                //Valida las conexiones de Dijin y Telepol
                if (!validar.VerificarConexionesPedidos("CONEXION_TELEPOL", usuarioActual.SITIO_GRABACION, "01"))
                {
                    return RedirectToAction("Index", "Home");
                }
                modelo.Conexiones += " TELEPOL: " + validar.ObtenerMensaje();
                //Obtiene ACD
                modelo.ACD = validar.ObtenerAcd(usuarioActual.SITIO_GRABACION, Session["MAQUINA"].ToString());
                if (string.IsNullOrEmpty(modelo.ACD))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_ACD;
                    return RedirectToAction("Index", "Home");
                }
                var sitio = contextoSecad.CADSITIOS_GRABACION.Find(usuarioActual.SITIO_GRABACION);
                //Consulta el pedido
                pedido = (from registro in contextoRecepcion.CADPEDIDOS
                          where registro.SITIO_GRABA == intSitio
                          && registro.NUME_LLAMADA == lgLlamada
                          select registro).FirstOrDefault();
                if (pedido == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    return RedirectToAction("Index", "Home");
                }
                modelo.LugarSitio = null;
                if (sitio != null)
                    modelo.LugarSitio = sitio.LUGE_CIUDAD;
                modelo.Consecutivo = "";
                modelo.FechaHora = pedido.HORA_CASO.ToString("dd/MM/yyyy HH:mm:ss");
                modelo.FECH_CASO = new DateTime(pedido.HORA_CASO.Year, pedido.HORA_CASO.Month, pedido.HORA_CASO.Day, 0, 0, 0);
                modelo.Usuario = pedido.CADUSUA_USUARIO;
                modelo.Identificacion = usuarioActual.IDENTIFICACION;
                modelo.LugeCad = usuarioActual.LUGE_CODIGO_CAD;
                modelo.Medios = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "DISPOSITIVO_COMUNICACION").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.DISP_TELEFONICO);
                ViewBag.TIPO_PEDIDO = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "TIPO_PEDIDO").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.TIPO_PEDIDO);
                ViewBag.CALI_PEDIDO = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALI_PEDIDO").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.CALI_PEDIDO);
                modelo.Coordenadas = new SelectList(contextoRecepcion.CADPUNTOS_CARDINALES.Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.PUCA_CODIGO);
                modelo.CasoDesc = (from registro in contextoSecad.CADCASOS
                                   where registro.CODIGO == pedido.CODI_PEDIDO
                                   select new { registro.DESCRIPCION }).FirstOrDefault().DESCRIPCION;
                modelo.Caso2Desc = (from registro in contextoSecad.CADCASOS
                                    where registro.CODIGO == pedido.CODI_PEDIDO2
                                    select registro.DESCRIPCION).FirstOrDefault();
                modelo.CasoRealDesc = (from registro in contextoSecad.CADCASOS
                                       where registro.CODIGO == pedido.CODI_PEDIDO_REAL
                                       select registro.DESCRIPCION).FirstOrDefault();

                modelo.LugeDpto = usuarioActual.LUGE_CODIGO_DPTO;
                modelo.Fuerza = usuarioActual.CADCANA_FUERZA_ID;
                modelo.Canal = usuarioActual.CADCANA_CODIGO;
                modelo.SITIO_GRABA = usuarioActual.SITIO_GRABACION;
                modelo.NUME_LLAMADA = pedido.NUME_LLAMADA;
                modelo.NUME_TELEFONO = pedido.NUME_TELEFONO;
                modelo.NOMB_LLAMANTE = pedido.NOMB_LLAMANTE;
                modelo.PROP_TELEFONO = pedido.PROP_TELEFONO;
                modelo.BARR_TELEFONO = pedido.BARR_TELEFONO;
                modelo.DIRE_TELEFONO = pedido.DIRE_TELEFONO;
                modelo.CODI_PEDIDO = pedido.CODI_PEDIDO;
                modelo.CODI_PEDIDO2 = pedido.CODI_PEDIDO2;
                modelo.COMENTARIO = pedido.COMENTARIO;
                modelo.DIRE_CASO = pedido.DIRE_CASO;
                modelo.LONGITUD_CASO = pedido.LONGITUD_CASO;
                modelo.LATITUD_CASO = pedido.LATITUD_CASO;
                modelo.PRIORIDAD = pedido.PRIORIDAD;
                modelo.IMPORTANCIA = pedido.IMPORTANCIA;
                modelo.CODI_BARRIO = pedido.CODI_BARRIO;
                modelo.CADPEDI_NUME_LLAMADA = pedido.CADPEDI_NUME_LLAMADA;
                modelo.CADPEDI_SITIO_GRABA = pedido.CADPEDI_SITIO_GRABA;
                modelo.TOTAL_CANALES = pedido.TOTAL_CANALES;
                modelo.TIPO_GENERO = pedido.TIPO_GENERO;
                modelo.ESTADO = pedido.ESTADO;
                modelo.DIRE_LLAMANTE = pedido.DIRE_LLAMANTE;
                modelo.LUGE_CODIGO = pedido.LUGE_CODIGO;
                //Genera ciudad
                modelo.LUGE_BARRIO = pedido.LUGE_BARRIO;
                var ciudad = (from registro in contextoSecad.CADLUGARES_GEOGRAFICOS
                              where registro.CODIGO == pedido.LUGE_BARRIO
                              select new { registro.DESCRIPCION }).FirstOrDefault();
                if (ciudad != null)
                {
                    modelo.Ciudad = ciudad.DESCRIPCION;
                }
                var barrio = (from registro in contextoSecad.CADBARRIOS
                              where registro.CODIGO == pedido.CODI_BARRIO
                              && registro.LUGE_CODIGO == pedido.LUGE_BARRIO
                              select new { registro.DESCRIPCION }).FirstOrDefault();
                if (barrio != null)
                {
                    modelo.Barrio = barrio.DESCRIPCION;
                }
                modelo.Modo = "CONSULTA";
                if (!string.IsNullOrEmpty(strOpcion) && strOpcion.Equals("REVISION"))
                {
                    modelo.Modo = strOpcion;
                }
                return View("Consulta", "BaseRecepcion", modelo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Recepcion/ConsultaAsociada")]
        public ActionResult ConsultaAsociada()
        {
            try
            {
                int intSitio = Convert.ToInt32(Request.Form["CADPEDI_SITIO_GRABA"].ToString());
                long lgLlamada = Convert.ToInt64(Request.Form["CADPEDI_NUME_LLAMADA"].ToString());
                DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                RecepcionModel modelo = new RecepcionModel();
                CADPEDIDO pedido = null;
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

                modelo.Conexiones = "CONEXIONES: ";
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Valida la numeracion celular
                if (!validar.ValidarNumeracionCelular())
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_NUMERACION;
                    return RedirectToAction("Index", "Home");
                }
                modelo.Numeracion = validar.Numeros;
                //Valida las conexiones de Dijin y Telepol
                if (!validar.VerificarConexionesPedidos("CONEXION_DIJIN", usuarioActual.SITIO_GRABACION, "01"))
                {
                    return RedirectToAction("Index", "Home");
                }
                modelo.Conexiones += " DIJIN: " + validar.ObtenerMensaje();
                modelo.ConexionDijin = validar.ObtenerMensaje();
                //Valida las conexiones de Dijin y Telepol
                if (!validar.VerificarConexionesPedidos("CONEXION_TELEPOL", usuarioActual.SITIO_GRABACION, "01"))
                {
                    return RedirectToAction("Index", "Home");
                }
                modelo.Conexiones += " TELEPOL: " + validar.ObtenerMensaje();
                //Obtiene ACD
                modelo.ACD = validar.ObtenerAcd(usuarioActual.SITIO_GRABACION, Session["MAQUINA"].ToString());
                if (string.IsNullOrEmpty(modelo.ACD))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_ACD;
                    return RedirectToAction("Index", "Home");
                }
                var sitio = contextoSecad.CADSITIOS_GRABACION.Find(usuarioActual.SITIO_GRABACION);
                //Consulta el pedido
                pedido = (from registro in contextoRecepcion.CADPEDIDOS
                          where registro.SITIO_GRABA == intSitio
                          && registro.NUME_LLAMADA == lgLlamada
                          select registro).FirstOrDefault();
                if (pedido == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    return RedirectToAction("Index", "Home");
                }
                modelo.LugarSitio = null;
                if (sitio != null)
                    modelo.LugarSitio = sitio.LUGE_CIUDAD;
                modelo.SitioOriginal = Request.Form["SITIO_GRABA"];
                modelo.LlamadaOriginal = Request.Form["NUME_LLAMADA"];
                modelo.Consecutivo = "";
                modelo.FechaHora = pedido.HORA_CASO.ToString("dd/MM/yyyy HH:mm:ss");
                modelo.FECH_CASO = new DateTime(pedido.HORA_CASO.Year, pedido.HORA_CASO.Month, pedido.HORA_CASO.Day, 0, 0, 0);
                modelo.Usuario = usuarioActual.USUARIO;
                modelo.Identificacion = usuarioActual.IDENTIFICACION;
                modelo.LugeCad = usuarioActual.LUGE_CODIGO_CAD;
                modelo.Medios = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "DISPOSITIVO_COMUNICACION").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.DISP_TELEFONICO);
                ViewBag.TIPO_PEDIDO = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "TIPO_PEDIDO").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.TIPO_PEDIDO);
                ViewBag.CALI_PEDIDO = new SelectList(contextoSecad.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALI_PEDIDO").Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.CALI_PEDIDO);
                modelo.Coordenadas = new SelectList(contextoRecepcion.CADPUNTOS_CARDINALES.Select(c => new { c.DESCRIPCION, c.CODIGO }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", pedido.PUCA_CODIGO);
                modelo.CasoDesc = (from registro in contextoSecad.CADCASOS
                                   where registro.CODIGO == pedido.CODI_PEDIDO
                                   select new { registro.DESCRIPCION }).FirstOrDefault().DESCRIPCION;
                modelo.Caso2Desc = (from registro in contextoSecad.CADCASOS
                                    where registro.CODIGO == pedido.CODI_PEDIDO2
                                    select registro.DESCRIPCION).FirstOrDefault();
                modelo.CasoRealDesc = (from registro in contextoSecad.CADCASOS
                                       where registro.CODIGO == pedido.CODI_PEDIDO_REAL
                                       select registro.DESCRIPCION).FirstOrDefault();

                modelo.LugeDpto = usuarioActual.LUGE_CODIGO_DPTO;
                modelo.Fuerza = usuarioActual.CADCANA_FUERZA_ID;
                modelo.Canal = usuarioActual.CADCANA_CODIGO;
                modelo.SITIO_GRABA = usuarioActual.SITIO_GRABACION;
                modelo.NUME_LLAMADA = pedido.NUME_LLAMADA;
                modelo.NUME_TELEFONO = pedido.NUME_TELEFONO;
                modelo.NOMB_LLAMANTE = pedido.NOMB_LLAMANTE;
                modelo.PROP_TELEFONO = pedido.PROP_TELEFONO;
                modelo.BARR_TELEFONO = pedido.BARR_TELEFONO;
                modelo.DIRE_TELEFONO = pedido.DIRE_TELEFONO;
                modelo.CODI_PEDIDO = pedido.CODI_PEDIDO;
                modelo.CODI_PEDIDO2 = pedido.CODI_PEDIDO2;
                modelo.COMENTARIO = pedido.COMENTARIO;
                modelo.DIRE_CASO = pedido.DIRE_CASO;
                modelo.LONGITUD_CASO = pedido.LONGITUD_CASO;
                modelo.LATITUD_CASO = pedido.LATITUD_CASO;
                modelo.PRIORIDAD = pedido.PRIORIDAD;
                modelo.IMPORTANCIA = pedido.IMPORTANCIA;
                modelo.CODI_BARRIO = pedido.CODI_BARRIO;
                modelo.CADPEDI_NUME_LLAMADA = pedido.CADPEDI_NUME_LLAMADA;
                modelo.CADPEDI_SITIO_GRABA = pedido.CADPEDI_SITIO_GRABA;
                modelo.TOTAL_CANALES = pedido.TOTAL_CANALES;
                modelo.TIPO_GENERO = pedido.TIPO_GENERO;
                //Genera ciudad
                modelo.LUGE_BARRIO = pedido.LUGE_BARRIO;
                var ciudad = (from registro in contextoSecad.CADLUGARES_GEOGRAFICOS
                              where registro.CODIGO == pedido.LUGE_BARRIO
                              select new { registro.DESCRIPCION }).FirstOrDefault();
                if (ciudad != null)
                {
                    modelo.Ciudad = ciudad.DESCRIPCION;
                }
                var barrio = (from registro in contextoSecad.CADBARRIOS
                              where registro.CODIGO == pedido.CODI_BARRIO
                              && registro.LUGE_CODIGO == pedido.LUGE_BARRIO
                              select new { registro.DESCRIPCION }).FirstOrDefault();
                if (barrio != null)
                {
                    modelo.Barrio = barrio.DESCRIPCION;
                }

                var fecha = pedido.HORA_CASO;

                return View(modelo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADPEDIDOS_DOCUMENTOS/Create
        public ActionResult CrearDocumento(int intSitio, long lgNumeLlamada, string strUsuario)
        {
            try
            {
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                ViewBag.FechaHora = fechaActual.ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.Sitio = intSitio;
                ViewBag.Llamada = lgNumeLlamada;
                ViewBag.Usuario = strUsuario;

                return View("CrearDocumento", "MapaBase", null);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADPEDIDOS_DOCUMENTOS/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearDocumento([Bind(Include = "CADPEDI_SITIOGRABA,CADPEDI_NUMELLAMADA,CONSECUTIVO,COMENTARIO,DOCUMENTO,FECHA_GRABACION,FECHA_MODIFICACION,USUA_MODIFICA,RUTA,NOMBRE_ARCHIVO,ARCHIVO")] CADPEDIDOS_DOCUMENTOS cADPEDIDOS_DOCUMENTOS)
        {
            try
            {
                int documentos = 0;
                DateTime fechaActual = (DateTime)gestorRecepcion.ObtenerTiempoServidor;
                short srConsecutivo = 0;
                ViewBag.FechaHora = fechaActual.ToString("dd/MM/yyyy HH:mm:ss");
                objGestorGral = new GestorGeneral();
                if (ModelState.IsValid)
                {
                    //string strArchivo = Request.Files[0].ToString();
                    foreach (string strArchivo in Request.Files)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Request.Files[strArchivo].InputStream.CopyTo(ms);

                            byte[] data = null;

                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                if (ms.Length > 0)
                                {
                                    Bitmap mifoto = new Bitmap(ms);
                                    string rutaArchivo = Redimensionar(mifoto, "tmp");

                                    data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                    System.IO.File.Delete(Server.MapPath("~/" + rutaArchivo));
                                }
                                // cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO = txtNumeroEvento;
                                if (data != null)
                                {
                                    cADPEDIDOS_DOCUMENTOS.ARCHIVO = data;
                                }
                                else
                                {
                                    cADPEDIDOS_DOCUMENTOS.ARCHIVO = new byte[0];
                                }
                            }
                        }
                    }
                    //verifica si el documento ya existe
                    documentos = (from registro in contextoRecepcion.CADPEDIDOS_DOCUMENTOS
                                  where registro.CADPEDI_SITIOGRABA == cADPEDIDOS_DOCUMENTOS.CADPEDI_SITIOGRABA
                                  && registro.CONSECUTIVO == cADPEDIDOS_DOCUMENTOS.CONSECUTIVO
                                  && registro.CADPEDI_NUMELLAMADA == cADPEDIDOS_DOCUMENTOS.CADPEDI_NUMELLAMADA
                                  select new { registro.CONSECUTIVO }).Count();
                    if (documentos > 0)
                    {
                        //acutaliza el documento
                        cADPEDIDOS_DOCUMENTOS.FECHA_MODIFICACION = fechaActual;
                        contextoRecepcion.Entry(cADPEDIDOS_DOCUMENTOS).State = EntityState.Modified;
                    }
                    else
                    {
                        //Crea un nuevo documento
                        srConsecutivo = objGestorGral.ObtenerDocumentoConsecutivo(cADPEDIDOS_DOCUMENTOS.CADPEDI_SITIOGRABA.ToString(), cADPEDIDOS_DOCUMENTOS.CADPEDI_NUMELLAMADA.ToString());
                        if (srConsecutivo == 0)
                        {
                            GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.EX_MSN_CONSECUTIVO_DOCUMENTO;
                            return RedirectToAction("CrearDocumento", new { intSitio = cADPEDIDOS_DOCUMENTOS.CADPEDI_SITIOGRABA, strNumeLlamada = cADPEDIDOS_DOCUMENTOS.CADPEDI_NUMELLAMADA, strUsuario = cADPEDIDOS_DOCUMENTOS.USUA_MODIFICA });
                        }
                        cADPEDIDOS_DOCUMENTOS.CONSECUTIVO = srConsecutivo;

                        contextoRecepcion.CADPEDIDOS_DOCUMENTOS.Add(cADPEDIDOS_DOCUMENTOS);
                    }

                    contextoRecepcion.SaveChanges();
                    return RedirectToAction("CrearDocumento", "Recepcion", new { intSitio = cADPEDIDOS_DOCUMENTOS.CADPEDI_SITIOGRABA, lgNumeLlamada = cADPEDIDOS_DOCUMENTOS.CADPEDI_NUMELLAMADA, strUsuario = cADPEDIDOS_DOCUMENTOS.USUA_MODIFICA });
                }
                ViewBag.Sitio = cADPEDIDOS_DOCUMENTOS.CADPEDI_SITIOGRABA;
                ViewBag.Llamada = cADPEDIDOS_DOCUMENTOS.CADPEDI_NUMELLAMADA;
                ViewBag.Usuario = cADPEDIDOS_DOCUMENTOS.USUA_MODIFICA;
                return View("CrearDocumento", "MapaBase", cADPEDIDOS_DOCUMENTOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        private string Redimensionar(Image Imagen_Original, string nombre)
        {
            try
            {
                //RUTA DEL DIRECTORIO TEMPORAL
                String DirTemp = "~/Images/PedidosDocumentos/" + nombre + ".jpg";
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
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
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
                throw GestorBase.GestionarExcepcion(ex, "RecepcionController");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarDocumento(int intSitio, long lgNumeroLlamada, short srConsecutivo)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                CADPEDIDOS_DOCUMENTOS documento = (from registro in contextoRecepcion.CADPEDIDOS_DOCUMENTOS
                                                   where registro.CADPEDI_SITIOGRABA == intSitio
                                                   && registro.CONSECUTIVO == srConsecutivo
                                                   && registro.CADPEDI_NUMELLAMADA == lgNumeroLlamada
                                                   select registro).FirstOrDefault();
                string strRuta = Url.Content("~/images/Avatar.png");
                if (documento != null)
                {
                    if (documento.ARCHIVO != null)
                    {
                        strRuta = Url.Content("~" + Bytes_A_Imagen(documento.ARCHIVO, srConsecutivo.ToString()));
                    }
                }
                resultados = Json(new { result = true, datos = new { RUTA = documento.RUTA, FECHA_GRABACION = documento.FECHA_GRABACION.ToString("dd/MM/yyyy HH:mm:ss"), NOMBRE_ARCHIVO = documento.NOMBRE_ARCHIVO }, imagen = strRuta });
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
        public JsonResult ObtenerDocumentos(string strSitio, string strLlamada)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadPedidosDocs> elementos = new List<Ext_CadPedidosDocs>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                int intId = 0;
                byte[] data = null;
                string strQuery = "", strLimites = "", strOrdenamiento = "", strBotonFinal = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"EditarDocumento('_CONSECUTIVO' ,'_NOMBRE')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para Editar' >" +
                " Editar </button>";
                string strBotonImagen = " <a href='#' class='secadImg'><img src='URL_IMAGEN' alt ='Foto miniatura' WIDTH='44' HEIGHT='27' />" +
                                     "<span>" +
                                         "<img src='URL_IMAGEN' alt = 'Foto ampliada' />" +
                                     "</span>" +
                                     "</a> ";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CONSECUTIVO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.NOMBRE_ARCHIVO, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.RUTA, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_MODIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.USUA_MODIFICA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.RUTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.NOMBRE_ARCHIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero_llamada", strLlamada, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_documentos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    intId = Convert.ToInt32(fila["CONSECUTIVO"].ToString());
                    if (fila["ARCHIVO"].ToString() != "")
                    {
                        data = (byte[])fila["ARCHIVO"];
                        if (data != null && data.Length > 0)
                        {
                            strBotonFinal = strBotonImagen.Replace("URL_IMAGEN", Url.Content("~" + Bytes_A_Imagen(data, intId.ToString())));
                        }
                        else
                            strBotonFinal = "Sin Imagen";
                    }
                    else
                        strBotonFinal = "Sin Imagen";
                    elementos.Add(new Ext_CadPedidosDocs(fila, strBoton.Replace("_CONSECUTIVO", fila["CONSECUTIVO"].ToString()), strBotonFinal));
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
        public JsonResult ConsultarResumen(string strSitio, string strLlamada, string strSitioAsoc, string strLlamadaAsoc)
        {
            try
            {
                //Consulta el turno seleccionado
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strResumen = "";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_comentario", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitiogr", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_llamadaini", strLlamada, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioasoc", strSitioAsoc, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_llamadaasoc", strLlamadaAsoc, typeof(decimal)));
                strResumen = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_resumencaso", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));

                resultados = Json(new { result = true, datos = strResumen });

                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        //FUNCIONES PARA CONVERTIR DE BYTES A IMAGEN
        private string Bytes_A_Imagen(byte[] ImgBytes, string strConsecutivo)
        {
            Bitmap imagen = null;
            try
            {
                Byte[] bytes = (Byte[])(ImgBytes);
                MemoryStream ms = new MemoryStream(bytes);
                imagen = new Bitmap(ms);
                string rutaFinal = Redimensionar(imagen, strConsecutivo);
                imagen.Dispose();
                return rutaFinal;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "RecepcionController");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult VerificarDireccion(string strSitio, string strLlamada)
        {
            try
            {
                Dictionary<string, string> datosLlamada = new Dictionary<string, string>();
                JsonResult resultados = new JsonResult();
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PSITIO_GRABA", strSitio, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PNUME_LLAMADA", strLlamada, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PDIRE_CASO", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PVALUECOORDX", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PVALUECOORDY", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PVALUENOMBARRIO", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PVALUENOMCOMUNA", "0", typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCODBARRIO", "0", typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PCODCOMUNA", "0", typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PESTADO", "0", typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("PSALIDA", "0", typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.P_GETPEDIDOSTEMP", strConexion, "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosLlamada.Add(parameter.Item1, parameter.Item2.ToString());
                }
                resultados = Json(new { result = true, datos = datosLlamada });
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
        public JsonResult ConsultarSitios(int intSitio, long lgLugeCodigo)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadSitio> elementos = new List<ExtCadSitio>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;
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
                        strQuery = " AND (A.BARR_CODIGO = " + parametros.ConsultaValor + " OR  A.BARR_LUGE_CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " AND (REGEXP_LIKE(A.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(A.DIRECCION, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(B.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i'))";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY A.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY A.DIRECCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY B.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 5:
                        strOrdenamiento += " ORDER BY A.BARR_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 6:
                        strOrdenamiento += " ORDER BY A.BARR_LUGE_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY A.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", intSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", lgLugeCodigo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_sitios", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new ExtCadSitio(fila, "<button class='btn btn-xs btn-default' id='btnDireccion' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarDireccion('" + fila["DIRECCION"].ToString() + "')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>"));
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

        [HttpGet]
        public ActionResult ObtenerCordenadas(string _nroCuadrante, string _codDane, string _codEstacion, string _barrio, string _cuadrante, string _latitud, string _longitud, string _direccion, string _nomMunicipio)
        {
            RecepcionModel modelo = new RecepcionModel();
            Session["Cuadrante"] = _nroCuadrante;
            Session["CuadranteID"] = _cuadrante;
            Session["Latitud"] = _latitud;
            Session["Longitud"] = _longitud;
            Session["Barrio"] = _barrio;
            Session["DireccionMapa"] = _direccion;
            Session["Municipio"] = _nomMunicipio;

            return Json(new { success = true, message = "Datos Recibidos" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Ubicacion(string _idCriminalidad, string _idFuente)
        {

            ViewBag.CRIMINALIDAD_ID = _idCriminalidad;
            ViewBag.FUENTE = _idFuente;

            return View();
        }

    }
}