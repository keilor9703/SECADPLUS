using SECADEntities.SECAD;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadLugares : CADLUGARES_GEOGRAFICOS
    {
        public Ext_CadLugares(DataRow fila, string strBoton)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            DESCRIPCION = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            TIPO = (string.IsNullOrEmpty(fila["TIPO"].ToString()) ? "" : (fila["TIPO"].ToString()));
            Boton = strBoton;
        }

        public Ext_CadLugares(DataRow fila, string strBoton, bool blRecepcion)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            DESCRIPCION = (string.IsNullOrEmpty(fila["CIUDADPADRE"].ToString()) ? "" : (fila["CIUDADPADRE"].ToString()));
            TIPO = (string.IsNullOrEmpty(fila["TIPO"].ToString()) ? "" : (fila["TIPO"].ToString()));
            Boton = strBoton;
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
    }
}