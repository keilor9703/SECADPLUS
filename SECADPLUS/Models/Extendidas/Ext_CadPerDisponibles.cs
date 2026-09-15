using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadPerDisponibles
    {
        public Ext_CadPerDisponibles(DataRow fila, string strBoton)
        {
            Dummy = "";
            CADPOLI_CEDU_EMPLEADO = fila["CADPOLI_CEDU_EMPLEADO"].ToString();
            DES_POLICIA = fila["DES_POLICIA"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string CADPOLI_CEDU_EMPLEADO { get; set; }
        public string DES_POLICIA { get; set; }
    }
}