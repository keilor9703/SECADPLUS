using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class _TablaMinutaMedios
    {
        public _TablaMinutaMedios(string strOpcion, DataRow fila, string strBoton)
        {
            switch (strOpcion)
            {
                case "VEHICULOS":
                    ID = (string.IsNullOrEmpty(fila["SERIE"].ToString()) ? "" : (fila["SERIE"].ToString()));
                    Cedula = (string.IsNullOrEmpty(fila["CEDULA"].ToString()) ? "" : (fila["CEDULA"].ToString()));
                    Observaciones = (string.IsNullOrEmpty(fila["DESC_UNIDAD"].ToString()) ? "" : (fila["DESC_UNIDAD"].ToString()));
                    Descripcion = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
                    break;

                case "ARMAS":
                    ID = (string.IsNullOrEmpty(fila["SERIE"].ToString()) ? "" : (fila["SERIE"].ToString()));
                    Cedula = (string.IsNullOrEmpty(fila["CEDULA"].ToString()) ? "" : (fila["CEDULA"].ToString()));
                    Observaciones = (string.IsNullOrEmpty(fila["DESC_UNIDAD"].ToString()) ? "" : (fila["DESC_UNIDAD"].ToString()));
                    Descripcion = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
                    break;

                case "VEHICULOSDISP":
                    ID = (string.IsNullOrEmpty(fila["SERIE"].ToString()) ? "" : (fila["SERIE"].ToString()));
                    Unidad = (string.IsNullOrEmpty(fila["DESC_UNIDAD"].ToString()) ? "" : (fila["DESC_UNIDAD"].ToString()));
                    Descripcion = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
                    break;

                case "ARMASDISP":
                    ID = (string.IsNullOrEmpty(fila["SERIE"].ToString()) ? "" : (fila["SERIE"].ToString()));
                    Unidad = (string.IsNullOrEmpty(fila["DESC_UNIDAD"].ToString()) ? "" : (fila["DESC_UNIDAD"].ToString()));
                    Descripcion = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
                    break;

                case "RADIOSDISP":
                    ID = (string.IsNullOrEmpty(fila["SERIE"].ToString()) ? "" : (fila["SERIE"].ToString()));
                    Unidad = (string.IsNullOrEmpty(fila["DESC_UNIDAD"].ToString()) ? "" : (fila["DESC_UNIDAD"].ToString()));
                    break;

                default:
                    ID = (string.IsNullOrEmpty(fila["SERIE"].ToString()) ? "" : (fila["SERIE"].ToString()));
                    Cedula = (string.IsNullOrEmpty(fila["CEDULA"].ToString()) ? "" : (fila["CEDULA"].ToString()));
                    Observaciones = (string.IsNullOrEmpty(fila["DESC_UNIDAD"].ToString()) ? "" : (fila["DESC_UNIDAD"].ToString()));
                    break;
            }
            Boton = strBoton;
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string ID { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public int Sitio { get; set; }
        public long Consecutivo { get; set; }
        public string Cedula { get; set; }
        public string Unidad { get; set; }
    }
}