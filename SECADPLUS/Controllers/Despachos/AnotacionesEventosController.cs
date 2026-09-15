using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;

using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class AnotacionesEventosController : Controller
    {
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorGeneral objGestorGral;

        // GET: AnotacionesEventos
        [SecadPlusAutorizador("/AnotacionesEventos/AnotacionesEventos")]
        public ActionResult AnotacionesEventos(string strSitioGraba, string strNumeroEvento)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                //strNumeroEvento = "216861";
                //strSitioGraba = "3947";
                if (strSitioGraba == null || strNumeroEvento == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = "No se encuentra un evento para anotaciones.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.SitioGraba = strSitioGraba;
                ViewBag.NumeroEvento = strNumeroEvento;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View("AnotacionesEventos", "MapaBase");
            // return View();
        }

        /// <summary>
        /// Metodo que guarda una nueva anotacion
        /// </summary>
        /// <param name="strAnotacion"></param>
        /// <param name="strNumeroEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult GuardarAnotacionesEventos(string strAnotacion, string strNumeroEvento)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                int intConsecutivos = ObtenerConsecutivo(strNumeroEvento);

                CADANOTACIONE cADANOTACIONES = new CADANOTACIONE();

                cADANOTACIONES.ANOTACION = strAnotacion;
                cADANOTACIONES.CADEVEN_NUMEEVENTO = Convert.ToInt64(strNumeroEvento);
                cADANOTACIONES.CADEVEN_SITIOGRABA = usr.SITIO_GRABACION;
                cADANOTACIONES.CADUSUA_GRABA = usr.USUARIO;
                cADANOTACIONES.CONSECUTIVO = intConsecutivos;
                cADANOTACIONES.FECHA_GRABACION = DateTime.Now;
                cADANOTACIONES.TIPO = 3;

                contexto.CADANOTACIONES.Add(cADANOTACIONES);
                contexto.SaveChangesAsync();

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Con el consecutivo" + cADANOTACIONES.CONSECUTIVO);
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                var resultado = Json(new { result = true, mensajes = mensaje, tipos = tipo });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que genera la tabla de anotaciones
        /// </summary>
        /// <param name="strCadEvenNumeroEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerAnotacionesEventos(string strCadEvenNumeroEvento, string strSitioGraba)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttAnotaciones;

                List<_AnotacionesEventosModel> listaAnotacionesGnr = new List<_AnotacionesEventosModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                // strQuery = "WHERE (CADEVEN_NUMEEVENTO = " + strCadEvenNumeroEvento + " AND
                // CADEVEN_SITIOGRABA = " + strSitioGraba + ")";

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (CADUSUA_GRABA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (FECHA_GRABACION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (ANOTACION, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CONSECUTIVO DESC";
                        break;

                    case 1:
                        strOrderna += " ORDER BY ANOTACION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY CADUSUA_GRABA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeroEvento", strCadEvenNumeroEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", strSitioGraba, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttAnotaciones = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaAnotacionesEventos", strConexion, "Nonquery");

                foreach (DataRow fila in dttAnotaciones.Rows)
                {
                    //strBotonDestino = strBoton.Replace("SITIO_GRABA", fila["SITIO_GRABA"].ToString());

                    //strBotonDestino = strBotonDestino.Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());

                    listaAnotacionesGnr.Add(new _AnotacionesEventosModel("", "", fila["CADUSUA_GRABA"].ToString(), fila["CONSECUTIVO"].ToString(), fila["FECHA_GRABACION"].ToString(), fila["ANOTACION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaAnotacionesGnr });
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
        /// Metodo que genera conexiones remotas
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerConexionesRemotas()
        {
            try
            {
                var varRemotos = (from t in contexto.CADREMOTOS select new { t.SITIOGRABA, t.DESCSITIO, t.DBLINK, t.FUERZA, t.CANAL });

                var resultado = Json(new { result = true, listaRemotos = varRemotos });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que envia un caso a remoto
        /// </summary>
        /// <param name="strCadenaParametros"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult EnviarCasoRemoto(string strCadenaParametros)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                DataTable ddtMensaje = null;
                string[] datos = strCadenaParametros.Split(';');

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeroevento", datos[1], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_dbLink", datos[0], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaRemoto", datos[4], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerzaRemoto", datos[2], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canalRemoto", datos[3], typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_usuario", usr.USUARIO.ToString(), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));

                ddtMensaje = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_enviarCasoRemoto", strConexion, "Nonquery");

                var mensaje = "Proceso exitoso " + ddtMensaje.Rows[0].ItemArray[0];
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                var resultado = Json(new { result = true, mensajes = mensaje, tipos = tipo });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que consulta consecutivo para una nueva anotacion
        /// </summary>
        /// <param name="strNumEvento"></param>
        /// <returns></returns>
        public int ObtenerConsecutivo(string strNumEvento)
        {
            int consecutivo = 0;

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_numeroEvento", strNumEvento, typeof(decimal)));
                consecutivo = (int)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consecutivosAnotacionesEvent", strConexion);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }

            return consecutivo;
        }
    }
}