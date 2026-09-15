using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_MediosVehiculosEvento
    {
        public Ext_MediosVehiculosEvento(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;
            SITIO_GRABA = fila["SITIO_GRABA"].ToString();
            CONSECUTIVO = fila["CONSECUTIVO"].ToString();
            CADUNITUR_UNDEFUERZA = fila["CADUNITUR_UNDEFUERZA"].ToString();
            CADUNITUR_UNDECODIGO = fila["CADUNITUR_UNDECODIGO"].ToString();
            CADPATRU_CODIGO = fila["CADPATRU_CODIGO"].ToString();
            ESTADO = fila["ESTADO"].ToString();
            TIPO = fila["TIPO"].ToString();
            DESC_PATRULLA = fila["DESC_PATRULLA"].ToString();
            ESTADO_PATRULLA = fila["ESTADO_PATRULLA"].ToString();
            CUENTA = fila["CUENTA"].ToString();
            personal = fila["personal"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string SITIO_GRABA { get; set; }
        public string CONSECUTIVO { get; set; }
        public string CADUNITUR_UNDEFUERZA { get; set; }
        public string CADUNITUR_UNDECODIGO { get; set; }
        public string CADPATRU_CODIGO { get; set; }
        public string ESTADO { get; set; }
        public string TIPO { get; set; }
        public string DESC_PATRULLA { get; set; }
        public string ESTADO_PATRULLA { get; set; }
        public string CUENTA { get; set; }
        public string personal { get; set; }
    }
}