using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadPerMedios : CADPERDISPONIBLE
    {
        public Ext_CadPerMedios(DataRow fila, string strBoton)
        {
            Boton = strBoton;
            Cargo = (string.IsNullOrEmpty(fila["CARGO"].ToString()) ? "" : (fila["CARGO"].ToString()));
            CODI_CARGO = (string.IsNullOrEmpty(fila["CODI_CARGO"].ToString()) ? Convert.ToInt16("0") : Convert.ToInt16(fila["CODI_CARGO"].ToString()));
            Empleado = (string.IsNullOrEmpty(fila["EMPLEADO"].ToString()) ? "" : (fila["EMPLEADO"].ToString()));
            CADMEDISP_SITIOGRABA = (string.IsNullOrEmpty(fila["CADMEDISP_SITIOGRABA"].ToString()) ? 0 : Convert.ToInt32(fila["CADMEDISP_SITIOGRABA"].ToString()));
            CADMEDISP_CONSECUTIVO = (string.IsNullOrEmpty(fila["CADMEDISP_CONSECUTIVO"].ToString()) ? 0 : Convert.ToInt64(fila["CADMEDISP_CONSECUTIVO"].ToString()));
            CADPOLI_CEDU_EMPLEADO = (string.IsNullOrEmpty(fila["CADPOLI_CEDU_EMPLEADO"].ToString()) ? "" : (fila["CADPOLI_CEDU_EMPLEADO"].ToString()));
            ORDENAMIENTO = (string.IsNullOrEmpty(fila["ORDENAMIENTO"].ToString()) ? Convert.ToByte("0") : Convert.ToByte(fila["ORDENAMIENTO"].ToString()));
            OBSERVACION = (string.IsNullOrEmpty(fila["OBSERVACION"].ToString()) ? "" : (fila["OBSERVACION"].ToString()));
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string Cargo { get; set; }
        public string Empleado { get; set; }
    }
}