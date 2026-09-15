using SECADEntities.SECAD;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadCanales : CADCANALE
    {
        public Ext_CadCanales(DataRow fila, string strBoton)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            DESCRIPCION = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            CADFUERZ_ID = (string.IsNullOrEmpty(fila["CADFUERZ_ID"].ToString()) ? 0 : Convert.ToInt32(fila["CADFUERZ_ID"].ToString()));
            Boton = strBoton;
        }

        public Ext_CadCanales(DataRow fila, string strBoton, bool blPedido)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            DESCRIPCION = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            CADFUERZ_ID = (string.IsNullOrEmpty(fila["CADFUERZ_ID"].ToString()) ? 0 : Convert.ToInt32(fila["CADFUERZ_ID"].ToString()));
            Boton = strBoton;

            DISPONIBLE = (string.IsNullOrEmpty(fila["DISPONIBLE"].ToString()) ? "" : fila["DISPONIBLE"].ToString() == "S" ? "Si" : "No");
        }

        public Ext_CadCanales(DataRow fila, string strBoton, string strOpcion)
        {
            DESCRIPCION = fila["CANA_ATENCION"].ToString();
            Canal = fila["CANAL"].ToString();
            Entidad = fila["ENT"].ToString();
            Fuerza = fila["FUERZA"].ToString();
            if (strOpcion.Equals("ASIGNADOS"))
            {
                Estado = fila["ESTADO"].ToString();
                Evento = fila["EVENTO"].ToString();
            }
            Boton = strBoton;
        }

        public Ext_CadCanales(DataRow fila, string strBoton, int dummy)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            DESCRIPCION = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            CADFUERZ_ID = (string.IsNullOrEmpty(fila["CADFUERZ_ID"].ToString()) ? 0 : Convert.ToInt32(fila["CADFUERZ_ID"].ToString()));
            CLASE_TURNO = (string.IsNullOrEmpty(fila["CLASE_TURNO"].ToString()) ? "" : (fila["CLASE_TURNO"].ToString()));
            ABREVIATURA = (string.IsNullOrEmpty(fila["ABREVIATURA"].ToString()) ? "" : (fila["ABREVIATURA"].ToString()));
            TIPO_CANAL = (string.IsNullOrEmpty(fila["TIPO_CANAL"].ToString()) ? "" : (fila["TIPO_CANAL"].ToString()));
            VIGENTE = (string.IsNullOrEmpty(fila["VIGENTE"].ToString()) ? "" : (fila["VIGENTE"].ToString()));
            FechaDesde = (string.IsNullOrEmpty(fila["DESDE"].ToString()) ? "" : (fila["DESDE"].ToString()));
            FechaHasta = (string.IsNullOrEmpty(fila["HASTA"].ToString()) ? "" : (fila["HASTA"].ToString()));
            DISPONIBLE = (string.IsNullOrEmpty(fila["DISPONIBLE"].ToString()) ? "" : (fila["DISPONIBLE"].ToString()));
            OBSERVACION = (string.IsNullOrEmpty(fila["OBSERVACION"].ToString()) ? "" : (fila["OBSERVACION"].ToString()));
            TIPO_ATENCION = (string.IsNullOrEmpty(fila["TIPO_ATENCION"].ToString()) ? "" : (fila["TIPO_ATENCION"].ToString()));
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Canal { get; set; }
        public string Fuerza { get; set; }
        public string Entidad { get; set; }
        public string Estado { get; set; }
        public string Evento { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
    }
}