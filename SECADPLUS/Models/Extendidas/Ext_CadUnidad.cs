using SECADEntities.SECAD;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadUnidad : CADUNIDADE
    {
        public Ext_CadUnidad(DataRow fila, string strBoton)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            FUERZA = (string.IsNullOrEmpty(fila["FUERZA"].ToString()) ? 0 : Convert.ToInt32(fila["FUERZA"].ToString()));
            Boton = strBoton;
            SIGLA = (string.IsNullOrEmpty(fila["SIGLA"].ToString()) ? "" : (fila["SIGLA"].ToString()));
            DESCRIPCION = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
    }
}