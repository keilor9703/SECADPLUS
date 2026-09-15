using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_OrdenCaptura
    {
        public Ext_OrdenCaptura(DataRow fila, string strBoton)
        {
            Dummy = "";
            NOMBRE = fila["NOMBRE"].ToString();
            PERSONA_REGISTRADURIA = fila["PERSONA_REGISTRADURIA"].ToString();
            NRO_ORDEN = fila["NRO_ORDEN"].ToString();
            NRO_FISCALIA = fila["NRO_FISCALIA"].ToString();
            FECHA = fila["FECHA"].ToString();
            CIUDAD = fila["CIUDAD"].ToString();
            DELITO = fila["DELITO"].ToString();
            IDENTIFICACION = fila["IDENTIFICACION"].ToString();
            TIPO = fila["TIPO"].ToString() != "" ? fila["TIPO"].ToString() == "CC" ? "Cedula" : fila["TIPO"].ToString() == "PA" ? "Pasaporte" :
                fila["TIPO"].ToString() == "CE" ? "Ced. Extranjeria" : fila["TIPO"].ToString() == "TI" ? "Tarjeta Id." : "" : "";
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string NOMBRE { get; set; }
        public string PERSONA_REGISTRADURIA { get; set; }
        public string NRO_ORDEN { get; set; }
        public string NRO_FISCALIA { get; set; }
        public string FECHA { get; set; }
        public string CIUDAD { get; set; }
        public string DELITO { get; set; }
        public string IDENTIFICACION { get; set; }
        public string TIPO { get; set; }
    }
}