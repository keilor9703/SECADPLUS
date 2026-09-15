using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadCondMedio : CADCONDTURNOPERSMEDISP
    {
        public ExtCadCondMedio(DataRow fila, string strBoton)
        {
            CADPERSITGR = (string.IsNullOrEmpty(fila["CADPERSITGR"].ToString()) ? 0 : Convert.ToInt32(fila["CADPERSITGR"].ToString()));
            CADMEDISPCONSEC = (string.IsNullOrEmpty(fila["CADMEDISPCONSEC"].ToString()) ? 0 : Convert.ToInt64(fila["CADMEDISPCONSEC"].ToString()));
            Patrulla = (string.IsNullOrEmpty(fila["DESC_PATRULLA"].ToString()) ? "" : (fila["DESC_PATRULLA"].ToString()));
            Unidad = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            Empleado = (string.IsNullOrEmpty(fila["EMPLEADO"].ToString()) ? "" : (fila["EMPLEADO"].ToString()));
            Ponderacion = (string.IsNullOrEmpty(fila["PONDERACION"].ToString()) ? "" : (fila["PONDERACION"].ToString()));
            CEDU_EMPLEADO = (string.IsNullOrEmpty(fila["CEDU_EMPLEADO"].ToString()) ? "" : (fila["CEDU_EMPLEADO"].ToString()));
            FechaConducta = (string.IsNullOrEmpty(fila["FECHA_CONDUCTA"].ToString()) ? "" : (fila["FECHA_CONDUCTA"].ToString()));
            Boton = strBoton;
        }

        public ExtCadCondMedio(DataRow fila, string strBoton, string strTipo)
        {
            switch (strTipo)
            {
                case "BIENES":
                    CADPERSITGR = (string.IsNullOrEmpty(fila["CADBIENSITGR"].ToString()) ? 0 : Convert.ToInt32(fila["CADBIENSITGR"].ToString()));
                    CADMEDISPCONSEC = (string.IsNullOrEmpty(fila["CADBIENCONSEC"].ToString()) ? 0 : Convert.ToInt64(fila["CADBIENCONSEC"].ToString()));
                    break;

                case "COMPARENDOS":
                    CADPERSITGR = (string.IsNullOrEmpty(fila["CADCOMPSITGR"].ToString()) ? 0 : Convert.ToInt32(fila["CADCOMPSITGR"].ToString()));
                    CADMEDISPCONSEC = (string.IsNullOrEmpty(fila["CADCOMPCONSEC"].ToString()) ? 0 : Convert.ToInt64(fila["CADCOMPCONSEC"].ToString()));
                    break;
            }
            Patrulla = (string.IsNullOrEmpty(fila["DESC_PATRULLA"].ToString()) ? "" : (fila["DESC_PATRULLA"].ToString()));
            Unidad = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            Empleado = (string.IsNullOrEmpty(fila["EMPLEADO"].ToString()) ? "" : (fila["EMPLEADO"].ToString()));
            Ponderacion = (string.IsNullOrEmpty(fila["PONDERACION"].ToString()) ? "" : (fila["PONDERACION"].ToString()));
            CEDU_EMPLEADO = (string.IsNullOrEmpty(fila["CEDU_EMPLEADO"].ToString()) ? "" : (fila["CEDU_EMPLEADO"].ToString()));
            FechaConducta = (string.IsNullOrEmpty(fila["FECHA_CONDUCTA"].ToString()) ? "" : (fila["FECHA_CONDUCTA"].ToString()));
            Boton = strBoton;
        }

        public string Patrulla { get; set; }
        public string Unidad { get; set; }
        public string Empleado { get; set; }
        public string Ponderacion { get; set; }
        public string FechaConducta { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}