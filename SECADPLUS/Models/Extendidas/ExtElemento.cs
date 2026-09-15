using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtElemento : CADNETELEMENTO
    {
        public ExtElemento(DataRow fila, string strBoton)
        {
            CADELEMENTO_ID = (string.IsNullOrEmpty(fila["CADELEMENTO_ID"].ToString()) ? 0 : Convert.ToDecimal(fila["CADELEMENTO_ID"].ToString()));
            ELEMENTO_RUTA = (string.IsNullOrEmpty(fila["CADELEMENTO_ID"].ToString()) ? "" : (fila["ELEMENTO_RUTA"].ToString()));
            Estado = "";
            ELEMENTO_NOMBRE = (string.IsNullOrEmpty(fila["ELEMENTO_NOMBRE"].ToString()) ? "" : (fila["ELEMENTO_NOMBRE"].ToString()));
            ELEMENTO_ICONO = (string.IsNullOrEmpty(fila["ELEMENTO_ICONO"].ToString()) ? "" : (fila["ELEMENTO_ICONO"].ToString()));
            ELEMENTO_POSICION = (string.IsNullOrEmpty(fila["ELEMENTO_POSICION"].ToString()) ? Convert.ToByte(0) : Convert.ToByte(fila["ELEMENTO_POSICION"].ToString()));
            if (!string.IsNullOrEmpty(fila["ELEMENTO_FECHA_CREACION"].ToString())) FechaCreacion = Convert.ToDateTime(fila["ELEMENTO_FECHA_CREACION"]).ToShortDateString();
            Padre = (string.IsNullOrEmpty(fila["PADRE"].ToString()) ? "" : (fila["PADRE"].ToString()));
            Tipo = (string.IsNullOrEmpty(fila["TIPO"].ToString()) ? "" : (fila["TIPO"].ToString()));
            if (fila["ELEMENTO_ESTADO"].ToString().Equals("1"))
            {
                Estado = "<span class='label label-success'>Activo</span>";
            }
            else
            {
                Estado = "<span class='label label-danger'>Inactivo</span>";
            }
            Boton = strBoton;
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string FechaCreacion { get; set; }
        public string Estado { get; set; }
        public string Padre { get; set; }
        public string Tipo { get; set; }
    }
}