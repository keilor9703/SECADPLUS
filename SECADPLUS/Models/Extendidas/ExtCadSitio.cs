using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadSitio
    {
        public ExtCadSitio(DataRow fila, string strBoton)
        {
            Sitio = fila["SITIO"].ToString();
            Direccion = fila["DIRECCION"].ToString();
            Barrio = fila["BARRIO"].ToString();
            BarrCodigo = fila["BARR_CODIGO"].ToString();
            BarrioLugarCodigo = fila["BARR_LUGE_CODIGO"].ToString();
            Boton = strBoton;
        }

        public string Sitio { get; set; }
        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public string BarrCodigo { get; set; }
        public string BarrioLugarCodigo { get; set; }
        public string Dummy { get; set; }
        public string Boton {get;set;}
    }
}