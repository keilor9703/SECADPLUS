using SECADEntities.SECAD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadSitioGrabacion : CADSITIOS_GRABACION
    {
        public ExtCadSitioGrabacion(DataRow fila, string strBoton) {
            CONSECUTIVO = (string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString()) ? 0 : Convert.ToInt32(fila["CONSECUTIVO"].ToString()));
            DESCRIPCION = fila["DESCRIPCION"].ToString();
            CADUNDE_CODIGO = (string.IsNullOrEmpty(fila["CADUNDE_CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CONSECUTIVO"].ToString()));
            Unidad = fila["DESCRIPCION1"].ToString();
            Departamento = fila["DEPARTAMENTO"].ToString();
            FechaInstalacion = fila["FECHA_INSTALACION"].ToString();
            Ciudad = fila["LUGE_CIUDAD"].ToString();
            Fuerza = fila["CADUNDE_FUERZA"].ToString();
            Boton = strBoton;
            OBSERVACIONES = fila["OBSERVACIONES"].ToString();
        }
        public string Departamento { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Ciudad { get; set; }
        public string Fuerza { get; set; }
        public string Unidad { get; set; }
        public string FechaInstalacion { get; set; }
        public string CodigoRegional { get; set; }
        public string Regional { get; set; }

    }

}
