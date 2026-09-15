using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadUnidadEvento
    {
        public Ext_CadUnidadEvento(DataRow fila, string strBoton)
        {
            Boton = strBoton;
            FUERZA = fila["FUERZA"].ToString();
            COD_UNIDAD = fila["COD_UNIDAD"].ToString();
            DESC_UNIDAD = fila["DESC_UNIDAD"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string FUERZA { get; set; }
        public string COD_UNIDAD { get; set; }
        public string DESC_UNIDAD { get; set; }
    }
}