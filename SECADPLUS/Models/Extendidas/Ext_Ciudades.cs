using SECADEntities.SecadLugaresGeograficos;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_Ciudades : CADLUGARES_GEOGRAFICOS
    {
        public Ext_Ciudades(DataRow fila, string strBoton = null)
        {
            Boton = strBoton;
            CODIGO = int.Parse(fila["CODIGO"].ToString());
            DESCRIPCION = (fila["DESCRIPCION"].ToString());
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}