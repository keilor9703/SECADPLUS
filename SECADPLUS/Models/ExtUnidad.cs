using SECADEntities.SECAD;
using System;
using System.Data;

namespace SECADPLUS.Models
{
    public class ExtUnidad : CADUNIDADE
    {
        #region Propiedades

        public string ExtNombrePadre { get; set; }
        public string ExtLugar { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }

        #endregion Propiedades

        #region Métodos

        public ExtUnidad(DataRow dtDatos, string strBoton = null)
        {
            CODIGO = Convert.ToInt32(dtDatos["CODIGO"].ToString());
            SIGLA = (string.IsNullOrEmpty(dtDatos["SIGLA"].ToString())) ? "" : dtDatos["SIGLA"].ToString();
            FUERZA = Convert.ToByte(dtDatos["FUERZA"].ToString());
            DESCRIPCION = (string.IsNullOrEmpty(dtDatos["DESCRIPCION"].ToString())) ? "" : dtDatos["DESCRIPCION"].ToString();
            OPERATIVA = (string.IsNullOrEmpty(dtDatos["OPERATIVA"].ToString())) ? "" : dtDatos["OPERATIVA"].ToString();
            VIGENTE = (string.IsNullOrEmpty(dtDatos["OPERATIVA"].ToString())) ? "" : dtDatos["OPERATIVA"].ToString();
            ExtLugar = (string.IsNullOrEmpty(dtDatos["LUGAR"].ToString())) ? "" : dtDatos["LUGAR"].ToString();
            NIVEL = (string.IsNullOrEmpty(dtDatos["NIVEL"].ToString())) ? (byte)0 : Convert.ToByte(dtDatos["NIVEL"].ToString());
            ExtNombrePadre = (string.IsNullOrEmpty(dtDatos["UNIDADPADRE"].ToString())) ? "" : dtDatos["UNIDADPADRE"].ToString();
            if (!string.IsNullOrEmpty(strBoton))
            {
                Boton = strBoton;
            }
        }

        #endregion Métodos
    }
}