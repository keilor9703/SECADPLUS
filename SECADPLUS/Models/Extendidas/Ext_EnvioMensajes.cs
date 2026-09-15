using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_EnvioMensajes
    {


        public Ext_EnvioMensajes(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;
            SITIO_GRABA = fila["SITIO_GRABA"].ToString();
            CONSECUTIVO = fila["CONSECUTIVO"].ToString();
            CADUSUA_INICIA = fila["CADUSUA_INICIA"].ToString();
            DESP_SITIO_GRABA = fila["DESP_SITIO_GRABA"].ToString();

            FECHA = fila["FECHA"].ToString();
            ESTADO = fila["ESTADO"].ToString() ==  "S" ? "Sin Enviar" : "Enviado";
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string SITIO_GRABA { get; set; }
        public string CONSECUTIVO { get; set; }
        public string CADUSUA_INICIA { get; set; }
        public string FECHA { get; set; }
        public string TIPO_ASOCIADO { get; set; }
        public string NUME_ASOCIADO { get; set; }
        public string ESTADO { get; set; }
        public string DESP_SITIO_GRABA { get; set; }





    }
}