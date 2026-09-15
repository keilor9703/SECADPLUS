using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_SocialFichaTipos
    {


        public Ext_SocialFichaTipos(DataRow fila, string strBoton, bool blTs)
        {
            Dummy = "";
            Boton = strBoton;
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();
            NRO_FICHA = fila["NRO_FICHA"].ToString();

            CADUSUARIO = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION = fila["FECHA_TRANSACCION"].ToString();
            if (blTs)
            {

                TIPO_SERVICIO = fila["TIPO_SERVICIO"].ToString();
                DESP_TIPO_SERVICIO = fila["DESP_TIPO_SERVICIO"].ToString();
            }
            else
            {
                TIPO_REMISION = fila["TIPO_REMISION"].ToString();
                DESP_TIPO_REMISION = fila["DESP_TIPO_REMISION"].ToString();
            }

          


        }



   

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }
        public string NRO_FICHA { get; set; }
        public string TIPO_REMISION { get; set; }
        public string CADUSUARIO { get; set; }
        public string FECHA_TRANSACCION { get; set; }
        public string DESP_TIPO_REMISION { get; set; }
        public string TIPO_SERVICIO { get; set; }
        public string DESP_TIPO_SERVICIO { get; set; }






    }
}