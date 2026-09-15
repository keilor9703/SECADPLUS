using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_ReferenciasSitioGraba
    {

        public Ext_ReferenciasSitioGraba(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;
            SITIO_GRABA = fila["SITIO_GRABA"].ToString();
            NOMBRE = fila["NOMBRE"].ToString();
            CODIGO = fila["CODIGO"].ToString();
            DESCRIPCION = fila["DESCRIPCION"].ToString();
            ABREVIATURA = fila["ABREVIATURA"].ToString();

        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string SITIO_GRABA { get; set; }
        public string NOMBRE { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string ABREVIATURA { get; set; }

    }
}