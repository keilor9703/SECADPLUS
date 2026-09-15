using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class LlamadasGralSalaController : Controller
    {
        private DesRepMinEntities db = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: LlamadasGralSala
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerPedidosEfectivosRecibidos(int idSitio)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + idSitio + " ";
                string strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<PedidosEfectivosRecibidos> listaPedidos = new List<PedidosEfectivosRecibidos>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (NUME_LLAMADA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (NUME_TELEFONO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (HORA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (CODIGO1, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + "AND REGEXP_LIKE (MPIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (DIRE_CASO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (BARRIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (COMENTARIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (TIPO_PEDIDO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (CALI_PEDIDO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (ESTADO_LLAMADA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (TIPO_GENERO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND REGEXP_LIKE (RECEPTOR, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY NUME_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY NUME_TELEFONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY HORA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY CODIGO1 " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY MPIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY DIRE_CASO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY BARRIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY COMENTARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrderna += " ORDER BY TIPO_PEDIDO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 10:
                        strOrderna += " ORDER BY CALI_PEDIDO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 11:
                        strOrderna += " ORDER BY ESTADO_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 12:
                        strOrderna += " ORDER BY TIPO_GENERO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 13:
                        strOrderna += " ORDER BY RECEPTOR " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_pedidos_recibidos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaPedidos.Add(new PedidosEfectivosRecibidos(fila));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPedidos });
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