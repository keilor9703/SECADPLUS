using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_FichaSocial
    {
        public Ext_FichaSocial(DataRow fila, string strBoton)
        {
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();
            NRO_FICHA = fila["NRO_FICHA"].ToString();
            CODI_EVENTO = fila["CODI_EVENTO"].ToString();
            FICHA_REMITIDA = fila["FICHA_REMITIDA"].ToString();
            OBSERVACION1 = fila["OBSERVACION1"].ToString();
            MPIO_ATENCION = fila["MPIO_ATENCION"].ToString();
            BARRIO_CODIGO = fila["BARRIO_CODIGO"].ToString();
            BARRIO_LUGECODIGO = fila["BARRIO_LUGECODIGO"].ToString();    
            DIRECCION = fila["DIRECCION"].ToString();
            TELEFONO = fila["TELEFONO"].ToString();
            CADUSUARIO = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION = fila["FECHA_TRANSACCION"].ToString();
            OBSERVACION2 = fila["OBSERVACION2"].ToString();
            DESP_MPIO_ATENCION = fila["DESP_MPIO_ATENCION"].ToString();
            DESP_BARRIO_ATENCION = fila["DESP_BARRIO_ATENCION"].ToString();
            Boton = strBoton;
        }




        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }
        public string NRO_FICHA { get; set; }
        public string CODI_EVENTO { get; set; }
        public string FICHA_REMITIDA { get; set; }
        public string OBSERVACION1 { get; set; }
        public string MPIO_ATENCION { get; set; }
        public string BARRIO_CODIGO { get; set; }
        public string BARRIO_LUGECODIGO { get; set; }
        public string DIRECCION { get; set; }
        public string TELEFONO { get; set; }
        public string CADUSUARIO { get; set; }
        public string FECHA_TRANSACCION { get; set; }
        public string OBSERVACION2 { get; set; }
        public string DESP_MPIO_ATENCION { get; set; }
        public string DESP_BARRIO_ATENCION { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }

    }
}