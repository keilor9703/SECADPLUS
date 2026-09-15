using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Models
{
    
    public class SecadPlusAutorizador : AuthorizeAttribute
    {
        #region Atributos

        private bool blAutorizar = false;
        private string strRutaValidar = "";

        #endregion Atributos

        /// <summary>
        /// </summary>
        /// <param name="strRuta">Es la ruta de acceso a una acción (/Controlador/Accion)</param>
        public SecadPlusAutorizador(string strRuta)
        {
            strRutaValidar = strRuta;
        }

        /// <summary>
        /// Sobrecarga del método de la clase padre
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                //Valida si la 
                if (httpContext.Session["USUARIO_SECAD"] == null) {

                }
                return VerificarPermiso();
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }
        public bool VerificarPermiso()
        {
            try
            {
                GestorGeneral gestorGeneral;
                string strUsuarioId = "";
                string strConexion = "";
                decimal dcIdElemento = 0;
                DataTable dtTabla;
                blAutorizar = false; //Por defecto se supone que el usuario no tiene permisos
                strUsuarioId = GestorBase.ObtenerIdUsuario();
                string strIp = GestorGeneral.ObtenerDireccionIp();
                //Se obtiene la conexion activa
                strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                //Consultamos el permiso especifico por procedimiento almacenado
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ruta", strRutaValidar, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario_id", strUsuarioId, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_permiso", Resources.RecursoGeneral.GestorSeguridad_Permiso, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dtTabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_obtener_permisos", strConexion, "Nonquery");

                // Imprimir el número de filas en dtTabla
                //System.Diagnostics.Debug.WriteLine($"Número de filas en dtTabla: {dtTabla.Rows.Count}");

                // Iterar sobre todas las filas y columnas del dtTabla
                //foreach (DataRow fila in dtTabla.Rows)
                //{
                //    foreach (DataColumn columna in dtTabla.Columns)
                //    {
                //        string valor = fila[columna].ToString();
                //        System.Diagnostics.Debug.WriteLine($"Columna: {columna.ColumnName}, Valor: {valor}");
                //    }
                //}

                foreach (DataRow fila in dtTabla.Rows)
                {
                    //System.Diagnostics.Debug.WriteLine($"Valor de ELEMENTO: {fila["IDELEMENTO"].ToString()}, Valor de USUARIO_ID: {fila["USUARIO_ID"].ToString()}");
                    if (!string.IsNullOrEmpty(fila["IDELEMENTO"].ToString()))
                        dcIdElemento = Convert.ToDecimal(fila["IDELEMENTO"].ToString());
                    if (!string.IsNullOrEmpty(fila["USUARIO_ID"].ToString()))
                    {
                        blAutorizar = true;
                    }
                    GestorBase.BREADCRUMB = fila["ELEMENTO_NOMBRE"].ToString();
                }

                // Si no se autoriza entonces se realiza registro del intento de acceso a la funcionalidad en la base de datos
                if (!blAutorizar)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.PERMISO_DENEGADO;
                    //Por complementar
                    RegistrarIngresoNoAutorizado(strUsuarioId, dcIdElemento, strIp, Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
                }
                return blAutorizar;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite ejecutar de manera asincrona el registro de ingresos no autorizados
        /// </summary>
        /// <param name="strOidId"></param>
        /// <param name="dcIdElemento"></param>
        /// <returns></returns>
        private async Task RegistrarIngresoNoAutorizado(string strOidId, decimal dcIdElemento, string strIp, string strConexion)
        {
            try
            {
                await Task.Run(() =>
                {
                    Registrar(strOidId, dcIdElemento, strIp, strConexion);
                });
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Ejecuta de manera asíncrona el registro de un ingreso no autorizado al sistema
        /// </summary>
        /// <param name="strOidId"></param>
        /// <param name="dcIdElemento"></param>
        /// <returns></returns>
        private async Task<bool> Registrar(string strOidId, decimal dcIdElemento, string strIp, string strConexion)
        {
            try
            {
                using (PermisosEntities db = new PermisosEntities(strConexion))
                {
                    CADACCESORESTRINGIDO registro = new CADACCESORESTRINGIDO();
                    registro.CADACCRES_CADELE_ID = dcIdElemento;
                    if (!string.IsNullOrEmpty(strOidId)) registro.CADACCRES_OID_ID = strOidId;
                    registro.CADACCRES_IP = strIp;
                    db.CADACCESORESTRINGIDOes.Add(registro);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}