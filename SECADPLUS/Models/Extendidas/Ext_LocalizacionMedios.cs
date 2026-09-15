using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_LocalizacionMedios
    {
        public Ext_LocalizacionMedios(DataRow fila, string strBoton)
        {
            Dummy = "";
            DESC_PATRULLA = fila["DESC_PATRULLA"].ToString();
            FECHA = fila["FECHA"].ToString();
            DIRE_UBICACION = fila["DIRE_UBICACION"].ToString();
            MUNICIPIO = fila["MUNICIPIO"].ToString();
            X_COORD_UBICA = fila["X_COORD_UBICA"].ToString();
            Y_COORD_UBICA = fila["Y_COORD_UBICA"].ToString();
            VIGENTE = (string.IsNullOrEmpty(fila["VIGENTE"].ToString()) ? "" : fila["VIGENTE"].ToString() == "S" ? "Si" : "No");
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string DESC_PATRULLA { get; set; }
        public string FECHA { get; set; }
        public string DIRE_UBICACION { get; set; }
        public string MUNICIPIO { get; set; }
        public string X_COORD_UBICA { get; set; }
        public string Y_COORD_UBICA { get; set; }
        public string VIGENTE { get; set; }
    }
}