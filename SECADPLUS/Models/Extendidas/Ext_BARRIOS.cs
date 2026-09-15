using SECADEntities.SecadLugaresGeograficos;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_BARRIOS : CADBARRIO
    {
        public Ext_BARRIOS(DataRow fila, string strBoton = null)
        {
            Boton = strBoton;
            CODIGO = int.Parse(fila["CODIGO"].ToString());
            DESCRIPCION = (fila["DESCRIPCION"].ToString());
            LUGE_CODIGO = int.Parse(fila["LUGE_CODIGO"].ToString() != string.Empty ? fila["LUGE_CODIGO"].ToString() : "0");
            COZO_CODIGO = int.Parse(fila["COZO_CODIGO"].ToString() != string.Empty ? fila["COZO_CODIGO"].ToString() : "0");
            COZO_LUGE_CODIGO = int.Parse(fila["COZO_LUGE_CODIGO"].ToString() != string.Empty ? fila["COZO_LUGE_CODIGO"].ToString() : "0");
        }

        public Ext_BARRIOS(DataRow fila, string strBoton, bool blEsMujer)
        {
            Boton = strBoton;
            CODIGO = int.Parse(fila["CODIGO"].ToString());
            DESCRIPCION = (fila["DESCRIPCION"].ToString());
            LUGE_CODIGO = int.Parse(fila["LUGE_CODIGO"].ToString() != string.Empty ? fila["LUGE_CODIGO"].ToString() : "0");
        }


  
        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}