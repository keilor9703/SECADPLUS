using SECADEntities.SECAD;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_VCanalesBarrios : V_BARRIOS_CANALES
    {
        public Ext_VCanalesBarrios(DataRow fila, string strBoton = null)
        {
            DESCRIPCIONCIUDAD = fila["DESCRIPCIONCIUDAD"].ToString();
            DESCRIPCIONBARRIO = fila["DESCRIPCIONBARRIO"].ToString();
            CODIGOBARRIO = int.Parse(fila["CODIGOBARRIO"].ToString());
            ASIGNADO = fila["ASIGNADO"] != null ? decimal.Parse(fila["ASIGNADO"].ToString()) : 0;
            CODIGOLUGE = fila["CODIGOLUGE"] != null ? int.Parse(fila["CODIGOLUGE"].ToString()) : 0;
            CODIGOCANAL = (!string.IsNullOrEmpty(fila["CODIGOCANAL"].ToString())) ? int.Parse(fila["CODIGOCANAL"].ToString()) : 0;
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}