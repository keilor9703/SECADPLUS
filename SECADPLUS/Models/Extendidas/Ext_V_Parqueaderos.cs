using SECADEntities.SecadLugaresGeograficos;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_V_Parqueaderos : V_PARQUEADEROS
    {
        public Ext_V_Parqueaderos(DataRow fila, string strBoton = null)
        {
            Boton = strBoton;
            CODIGO = fila["CODIGO"].ToString();
            DESCRIPCION = fila["DESCRIPCION"].ToString();
            NOMBRECIUDAD = fila["NOMBRECIUDAD"].ToString();
            NOMBREBARRIO = fila["NOMBREBARRIO"].ToString();
            DIRECCION = fila["DIRECCION"].ToString();
            TELEFONO = fila["TELEFONO"].ToString();
            HORARIO = fila["HORARIO"].ToString();
            NOMBRESITIOGRABACION = fila["NOMBRESITIOGRABACION"].ToString();
            SITIO_GRABA = Convert.ToInt32(fila["SITIO_GRABA"].ToString());
        }

        public Ext_V_Parqueaderos(DataRow fila, string strBoton, bool blConducta)
        {
            Boton = strBoton;
            CODIGO = fila["CODIGO"].ToString();
            DESCRIPCION = fila["DESCRIPCION"].ToString();
            SITIO_GRABA = Convert.ToInt32(fila["SITIO_GRABA"].ToString());
        }

        public object Boton { get; private set; }
        public string Dummy { get; set; }
    }
}