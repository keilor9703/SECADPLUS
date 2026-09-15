using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadCondComparendo : CADCONDTURNOCOMP
    {
        public ExtCadCondComparendo(DataRow fila, string strBoton)
        {
            CODI_INFRACCION = fila["CODIGO"].ToString();
            FechaConducta = (fila["FECHA_CONDUCTA"].ToString());
            Infraccion = fila["DESCRIPCION"].ToString();
            CADEVENNUMEEVENTO = (string.IsNullOrEmpty(fila["CADEVENNUMEEVENTO"].ToString()) ? 0 : Convert.ToInt64(fila["CADEVENNUMEEVENTO"].ToString()));
            CANTIDAD = (string.IsNullOrEmpty(fila["CANTIDAD"].ToString()) ? (short)0 : Convert.ToInt16(fila["CANTIDAD"].ToString()));
            OBSERVACIONES = fila["OBSERVACIONES"].ToString();
            Boton = strBoton;
        }

        public string FechaConducta { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Infraccion { get; set; }
    }
}