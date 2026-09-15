using Microsoft.Owin.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SECADPLUS.Models
{
    public class DbAdministracion : IDbAdministracion
    {

        private readonly DbConfiguration _iConfiguration;
 
        private readonly string _strConexionTelepol;
        private readonly ILogger _logger;


        public DataTable F_GetImagenes(int Consecutivo)
        {
            DataTable objRetorno = new DataTable();

            OracleConnection Conexion = null;
            OracleCommand objCommand = null;
            OracleDataAdapter adaptador = null;

            try
            {
                // Inicialización de objetos
                Conexion = new OracleConnection("strConexionTelepol");
                objCommand = new OracleCommand();
                adaptador = new OracleDataAdapter();

                // Configuración del comando
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.Text;
                objCommand.CommandText = "SELECT CONSECUTIVO, CONTENT_TYPE, FILENAME, FOTO FROM IMAGENES WHERE CONSECUTIVO = :consecutivo";

                // Abrir la conexión
                Conexion.Open();

                // Configuración de los parámetros
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter(":consecutivo", Consecutivo)).Direction = ParameterDirection.Input;

                // Llenar el DataTable con los resultados
                if (Conexion.State == ConnectionState.Open)
                {
                    adaptador.SelectCommand = objCommand;
                    adaptador.Fill(objRetorno);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                _logger.WriteError("Error Ejecutando la consulta de imagenes(): [ID error:]", ex);
            }
            finally
            {
                // Liberar recursos en el bloque finally
                if (Conexion != null)
                {
                    if (Conexion.State == ConnectionState.Open)
                    {
                        Conexion.Close();
                    }
                    Conexion.Dispose();
                }
                if (objCommand != null)
                {
                    objCommand.Dispose();
                }
                if (adaptador != null)
                {
                    adaptador.Dispose();
                }
            }

            return objRetorno;
        }


        public async Task<DtoResultado<List<DtoSlider>>> F_GetSilerSuperior()
        {
            DataTable resultado = new DataTable();
            List<DtoSlider> retorno = new List<DtoSlider>();
            DtoResultado<List<DtoSlider>> resp = new DtoResultado<List<DtoSlider>>();

            OracleConnection Conexion = null;
            OracleCommand objCommand = null;

            try
            {
                Conexion = new OracleConnection("strConexionTelepol");
                objCommand = new OracleCommand();

                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CTR_ADMINISTRACION.F_GetSlider";
                objCommand.BindByName = true;
                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    // Ejecutar el comando y llenar el DataTable
                    using (var reader = objCommand.ExecuteReader())
                    {
                        resultado.Load(reader);
                    }

                    // Convertir DataTable a lista de DTOs
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoSlider>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetMenu";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                if (Conexion != null)
                {
                    if (Conexion.State == ConnectionState.Open)
                    {
                        Conexion.Close();
                    }
                    Conexion.Dispose();
                }
                if (objCommand != null)
                {
                    objCommand.Dispose();
                }

                _logger.WriteError("Creacion de log");
                _logger.WriteWarning("Error Ejecutando PK_CTR_ADMINISTRACION.F_GetSlider " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";
            }
            finally
            {
                if (Conexion != null)
                {
                    if (Conexion.State == ConnectionState.Open)
                    {
                        Conexion.Close();
                    }
                    Conexion.Dispose();
                }
                if (objCommand != null)
                {
                    objCommand.Dispose();
                }
                if (resultado != null)
                {
                    resultado.Dispose();
                }
            }

            // Retornar el resultado envuelto en un Task
            return await Task.FromResult(resp);
        }



    }
}