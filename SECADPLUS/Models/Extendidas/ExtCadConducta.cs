using SECADEntities.SECAD;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadConducta : CADCONDUCTA
    {
        public ExtCadConducta(DataRow fila, string strBoton)
        {
            CODIGO = int.Parse(fila["CODIGO"].ToString() != string.Empty ? fila["CODIGO"].ToString() : "0");
            DESCRIPCION = fila["DESCRIPCION"].ToString() != string.Empty ? fila["DESCRIPCION"].ToString() : "";
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}