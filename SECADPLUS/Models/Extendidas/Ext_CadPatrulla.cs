using SECADEntities.SECAD;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadPatrulla : CADPATRULLA
    {
        public Ext_CadPatrulla(DataRow fila, string strBoton)
        {
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            Patrulla = (string.IsNullOrEmpty(fila["PATRULLA"].ToString()) ? "" : (fila["PATRULLA"].ToString()));
            Movil = (string.IsNullOrEmpty(fila["MOVIL"].ToString()) ? "" : (fila["MOVIL"].ToString()));
            Estado = (string.IsNullOrEmpty(fila["ESTADO"].ToString()) ? "" : (fila["ESTADO"].ToString()));
            CADUNID_FUERZA = (string.IsNullOrEmpty(fila["FUERZA"].ToString()) ? 0 : Convert.ToInt32(fila["FUERZA"].ToString()));
            CADUNID_CODIGO = (string.IsNullOrEmpty(fila["UNIDAD"].ToString()) ? 0 : Convert.ToInt32(fila["UNIDAD"].ToString()));

            Boton = strBoton;
        }

        public Ext_CadPatrulla(DataRow fila, string strBoton, bool blConductas)
        {
            Consecutivo = fila["CONSECUTIVO"].ToString();
            Descripcion = fila["DESCRIPCION"].ToString();
            Patrulla = fila["PATRULLA"].ToString();
            SITIO_GRABA = (string.IsNullOrEmpty(fila["SITIO_GRABA"].ToString()) ? 0 : Convert.ToInt32(fila["SITIO_GRABA"].ToString()));
            Boton = strBoton;
        }

        public Ext_CadPatrulla(DataRow fila, bool blDijin)
        {
            Dummy = "";
            DESC_PATRULLA = fila["DESC_PATRULLA"].ToString();
            CADUNID_FUERZA = (string.IsNullOrEmpty(fila["CADUNID_FUERZA"].ToString()) ? 0 : Convert.ToInt32(fila["CADUNID_FUERZA"].ToString()));
            CADUNID_CODIGO = (string.IsNullOrEmpty(fila["CADUNID_CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CADUNID_CODIGO"].ToString()));
            CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            Canal = fila["CANAL"].ToString();
            Unidad = fila["UNIDAD"].ToString();
            MD_SITIO_GRABA = fila["MD_SITIO_GRABA"].ToString();
            MD_CONSECUTIVO = fila["MD_CONSECUTIVO"].ToString();
        }

        public string Patrulla { get; set; }
        public string Movil { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Consecutivo { get; set; }
        public string Canal { get; set; }
        public string Unidad { get; set; }
        public string MD_SITIO_GRABA { get; set; }
        public string MD_CONSECUTIVO { get; set; }
    }
}