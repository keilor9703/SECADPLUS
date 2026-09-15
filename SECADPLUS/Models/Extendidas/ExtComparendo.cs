using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtComparendo
    {
        public ExtComparendo(DataRow fila, string strBoton)
        {
            Boton = strBoton;
            Cedula = fila["CADPOLI_CEDULA"].ToString();
            Policia = fila["NOMB_EMPLEADO"].ToString();
            Comparendo = fila["CNP_ORDEN_COMPARENDO"].ToString();
            Fecha = fila["FECHA_GRABACION"].ToString();
            Validado = fila["VALIDADO_CNP"].ToString();
            Apellidos = fila["CADPOLI_CEDULA"].ToString();
            Nombres = fila["CADPOLI_CEDULA"].ToString();
            Evento = fila["EVENTO"].ToString();
        }
        public ExtComparendo(DataRow fila, string strBoton, bool blOpcion)
        {
            Boton = strBoton;
            IdComportamiento = fila["CNP_COMPORTAMIENTO_ID"].ToString();

            Comparendo = fila["CNP_ORDEN_COMPARENDO"].ToString();
            Fecha = fila["FECHA"].ToString();
            Comportamiento = fila["DESCRIPCION"].ToString();
            Apellidos = fila["APELLIDOS_PERSONA"].ToString();
            Nombres = fila["NOMBRE_PERSONA"].ToString();
        }
        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Cedula { get; set; }
        public string Policia { get; set; }
        public string Comparendo { get; set; }
        public string Fecha { get; set; }
        public string Validado { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
        public string Comportamiento { get; set; }
        public string IdComportamiento { get; set; }
        public string Evento { get; set; }
    }
}
