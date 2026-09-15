using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadCondBiene : CADCONDTURNOBIENE
    {
        public ExtCadCondBiene(DataRow fila, string strBoton)
        {
            CONSECUTIVO = (string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString()) ? 0 : Convert.ToInt64(fila["CONSECUTIVO"].ToString()));
            SITIO_GRABA = (string.IsNullOrEmpty(fila["SITIO_GRABA"].ToString()) ? 0 : Convert.ToInt32(fila["SITIO_GRABA"].ToString()));
            CADCLBI_TIBI_CODIGO = (string.IsNullOrEmpty(fila["TIPOCODIGO"].ToString()) ? (short)0 : Convert.ToInt16(fila["TIPOCODIGO"].ToString()));
            Tipo = fila["TIPO"].ToString();
            CADCLBI_CODIGO = (string.IsNullOrEmpty(fila["CLASECODIGO"].ToString()) ? (short)0 : Convert.ToInt16(fila["CLASECODIGO"].ToString()));
            Clase = fila["CLASE"].ToString();
            MARCA_CODIGO = (string.IsNullOrEmpty(fila["MARCACODIGO"].ToString()) ? (short)0 : Convert.ToInt16(fila["MARCACODIGO"].ToString()));
            Marca = fila["MARCA"].ToString();
            LINEA_CODIGO = (string.IsNullOrEmpty(fila["LINEACODIGO"].ToString()) ? (short)0 : Convert.ToInt16(fila["LINEACODIGO"].ToString()));
            Linea = fila["LINEA"].ToString();
            Fecha = fila["FECHA_CONDUCTA"].ToString();
            Modo = fila["MODO"].ToString();
            CANTIDAD = (string.IsNullOrEmpty(fila["CANTIDAD"].ToString()) ? (long)0 : Convert.ToInt64(fila["CANTIDAD"].ToString()));
            AVALUO = (string.IsNullOrEmpty(fila["AVALUO"].ToString()) ? (long)0 : Convert.ToInt64(fila["AVALUO"].ToString()));
            PLACA = fila["PLACA"].ToString();
            CHASIS = fila["CHASIS"].ToString();
            MOTOR = fila["MOTOR"].ToString();
            SERIE = fila["SERIE"].ToString();
            Parqueadero = fila["PARQUEADERO"].ToString();
            COLO_CODIGO = (string.IsNullOrEmpty(fila["COLORCODIGO"].ToString()) ? (short)0 : Convert.ToInt16(fila["COLORCODIGO"].ToString()));
            Color = fila["COLOR"].ToString();
            CADINFR_CODIGO = fila["INFRACODIGO"].ToString();
            Infraccion = fila["INFRACCION"].ToString();
            CADEVENNUMEEVENTO = (string.IsNullOrEmpty(fila["CADEVENNUMEEVENTO"].ToString()) ? (long)0 : Convert.ToInt64(fila["CADEVENNUMEEVENTO"].ToString()));
            OBSERVACIONES = fila["OBSERVACIONES"].ToString();
            CADPARQ_CODIGO = fila["CADPARQ_CODIGO"].ToString();
            Boton = strBoton;
        }

        public string Tipo { get; set; }
        public string Clase { get; set; }
        public string Marca { get; set; }
        public string Linea { get; set; }
        public string Fecha { get; set; }
        public string Modo { get; set; }
        public string Parqueadero { get; set; }
        public string Color { get; set; }
        public string Infraccion { get; set; }
        public string Boton { get; set; }
        public string Dummy { get; set; }
    }
}