using SECADEntities.SECAD;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_Infracciones : CADINFRACCIONES_TRANSITO
    {
        public Ext_Infracciones(DataRow fila, string strBoton = null)
        {
            Dummy = "";
            Boton = strBoton;
            CODIGO = fila["CODIGO"].ToString();
            DESCRIPCION = (fila["DESCRIPCION"].ToString());
            INMOVILIZACION = fila["INMOVILIZACION"].ToString();
            NRO_SALARIOS = int.Parse(fila["NRO_SALARIOS"].ToString() != string.Empty ? fila["NRO_SALARIOS"].ToString() : "0");
            VIGENTE = fila["VIGENTE"].ToString();
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}