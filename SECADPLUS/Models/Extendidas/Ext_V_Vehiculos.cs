using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_V_Vehiculos
    {
        public Ext_V_Vehiculos(DataRow fila)
        {
            Dummy = "";
            PLACA = fila["PLACA"].ToString();
            MOTOR = fila["MOTOR"].ToString();
            CHASIS = fila["CHASIS"].ToString();

            SERIE = fila["SERIE"].ToString();
            MODELO = fila["MODELO"].ToString();
            MARCA = fila["MARCA"].ToString();
            LINEA = fila["LINEA"].ToString();
            COLOR = fila["COLOR"].ToString();
            TIPO = fila["TIPO"].ToString();
            ESTADO = fila["ESTADO"].ToString();
            AUTORIDAD = fila["AUTORIDAD"].ToString();
        }
        public Ext_V_Vehiculos(DataRow fila, string strBoton)
        {
            Boton = strBoton;
            CLASE = fila["CLASE"].ToString();
            CONSECUTIVO = fila["CONSECUTIVO"].ToString();
            MARCA = fila["MARCA"].ToString();
            LINEA = fila["LINEA"].ToString();
            COLOR = fila["COLOR"].ToString();
            PLACA = fila["PLACA"].ToString();
            CHASIS = fila["CHASIS"].ToString();
            MOTOR = fila["MOTOR"].ToString();
            SERIE = fila["SERIE"].ToString();
            OBSERVACIONES = fila["OBSERVACIONES"].ToString();
            USUA_TRANSACCION = fila["USUA_TRANSACCION"].ToString();
            FECHA_TRANSACCION = fila["FECHA_TRANSACCION"].ToString();
        }
        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string PLACA { get; set; }
        public string MOTOR { get; set; }
        public string CHASIS { get; set; }
        public string SERIE { get; set; }
        public string MODELO { get; set; }
        public string MARCA { get; set; }
        public string LINEA { get; set; }
        public string COLOR { get; set; }
        public string TIPO { get; set; }
        public string ESTADO { get; set; }
        public string AUTORIDAD { get; set; }
        public string CLASE { get; set; }
        public string CONSECUTIVO { get; set; }
        public string OBSERVACIONES { get; set; }
        public string USUA_TRANSACCION { get; set; }
        public string FECHA_TRANSACCION { get; set; }
    }
}