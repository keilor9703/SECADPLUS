using SECADEntities.SecadEntities;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadCondTurno : CADCONDTURNO
    {
        public ExtCadCondTurno(DataRow fila, string strBoton)
        {
            CADTURNSITIOGRABA = int.Parse(fila["CADTURNSITIOGRABA"].ToString() != string.Empty ? fila["CADTURNSITIOGRABA"].ToString() : "0");
            CADCONDCODIGO = int.Parse(fila["CADCONDCODIGO"].ToString() != string.Empty ? fila["CADCONDCODIGO"].ToString() : "0");
            CADTURNTURNO = long.Parse(fila["CADTURNTURNO"].ToString() != string.Empty ? fila["CADTURNTURNO"].ToString() : "0");
            Tipo = fila["TIPO"].ToString() != string.Empty ? fila["TIPO"].ToString() : "";
            Conducta = fila["DESCRIPCION"].ToString() != string.Empty ? fila["DESCRIPCION"].ToString() : "";
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Tipo { get; set; }
        public string Conducta { get; set; }
    }
}