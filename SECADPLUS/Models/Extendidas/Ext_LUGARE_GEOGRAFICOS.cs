using SECADEntities.SecadLugaresGeograficos;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_LUGARES_GEOGRAFICOS : CADLUGARES_GEOGRAFICOS
    {
        public Ext_LUGARES_GEOGRAFICOS(DataRow fila, string strBoton = null)
        {
            Boton = strBoton;
            CODIGO = int.Parse(fila["CODIGO"].ToString());
            DESCRIPCION = (fila["DESCRIPCION"].ToString());
            TIPO = (fila["TIPO"].ToString());
            LUGE_CODIGO = int.Parse(fila["LUGE_CODIGO"].ToString() != string.Empty ? fila["LUGE_CODIGO"].ToString() : "0");
            CODIGO_REGISTRADURIA = (fila["CODIGO_REGISTRADURIA"].ToString());
            CODIGO_DANE = (fila["CODIGO_DANE"].ToString());
            DPTO = (fila["DPTO"].ToString());
            NIVEL = byte.Parse(fila["NIVEL"].ToString() != string.Empty ? fila["NIVEL"].ToString() : "0");
            CODIGO_SIATH = int.Parse(fila["CODIGO_SIATH"].ToString() != string.Empty ? fila["CODIGO_SIATH"].ToString() : "0");
            CODIGO_SIEDCO = int.Parse(fila["CODIGO_SIEDCO"].ToString() != string.Empty ? fila["CODIGO_SIEDCO"].ToString() : "0");
            VIGENTE = (fila["VIGENTE"].ToString());
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}