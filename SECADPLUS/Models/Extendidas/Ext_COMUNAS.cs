using SECADEntities.SecadLugaresGeograficos;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_COMUNAS : CADCOMUNA
    {
        public Ext_COMUNAS(DataRow fila, string strBoton = null)
        {
            Boton = strBoton;
            CODIGO = int.Parse(fila["CODIGO"].ToString());
            DESCRIPCION = (fila["DESCRIPCION"].ToString());
            //LUGE_CODIGO = int.Parse(fila["LUGE_CODIGO"].ToString() != string.Empty ? fila["LUGE_CODIGO"].ToString() : "0");
            VIGENTE = (fila["VIGENTE"].ToString());
            ORDEN = int.Parse(fila["ORDEN"].ToString() != string.Empty ? fila["ORDEN"].ToString() : "0");
        }

        public Ext_COMUNAS(DataRow fila, string strBoton, bool blEsMujer)
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