using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadCondPersonas : CADCONDTURNOPER
    {
        public ExtCadCondPersonas(DataRow fila, string strBoton)
        {
            CONSECUTIVO = (string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString()) ? 0 : Convert.ToInt64(fila["CONSECUTIVO"].ToString()));
            SITIO_GRABA = (string.IsNullOrEmpty(fila["SITIO_GRABA"].ToString()) ? 0 : Convert.ToInt32(fila["SITIO_GRABA"].ToString()));
            IDENTIFICACION = (string.IsNullOrEmpty(fila["IDENTIFICACION"].ToString()) ? 0 : Convert.ToInt64(fila["IDENTIFICACION"].ToString()));
            NOMBRES = (string.IsNullOrEmpty(fila["NOMBRES"].ToString()) ? "" : (fila["NOMBRES"].ToString()));
            APELLIDOS = (string.IsNullOrEmpty(fila["APELLIDOS"].ToString()) ? "" : (fila["APELLIDOS"].ToString()));
            Tipo = (string.IsNullOrEmpty(fila["TIPO"].ToString()) ? "" : (fila["TIPO"].ToString()));
            GENERO = (string.IsNullOrEmpty(fila["GENERO"].ToString()) ? "" : (fila["GENERO"].ToString()));
            Infracciones = (string.IsNullOrEmpty(fila["INFRACCIONDES"].ToString()) ? "" : (fila["INFRACCIONDES"].ToString()));
            INFRACCION = (string.IsNullOrEmpty(fila["INFRACCIONCOD"].ToString()) ? "" : (fila["INFRACCIONCOD"].ToString()));
            CONDCODIGO = (string.IsNullOrEmpty(fila["CONDCODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CONDCODIGO"].ToString()));
            Conducta = (string.IsNullOrEmpty(fila["CONDUCTADESC"].ToString()) ? "" : (fila["CONDUCTADESC"].ToString()));

            FechaConducta = (string.IsNullOrEmpty(fila["FECHA_CONDUCTA"].ToString()) ? "" : (fila["FECHA_CONDUCTA"].ToString()));
            CADEVENNUMEEVENTO = (string.IsNullOrEmpty(fila["EVENTO"].ToString()) ? 0 : Convert.ToInt64(fila["EVENTO"].ToString()));
            Modo = (string.IsNullOrEmpty(fila["MODO"].ToString()) ? "" : (fila["MODO"].ToString()));
            Participacion = (string.IsNullOrEmpty(fila["PARTICIPACION"].ToString()) ? "" : (fila["PARTICIPACION"].ToString()));
            Relacion = (string.IsNullOrEmpty(fila["RELACION"].ToString()) ? "" : (fila["RELACION"].ToString()));
            OBSERVACIONES = (string.IsNullOrEmpty(fila["OBSERVACIONES"].ToString()) ? "" : (fila["OBSERVACIONES"].ToString()));
            CADCONDTURNCOND = (string.IsNullOrEmpty(fila["CADCONDTURNCOND"].ToString()) ? 0 : Convert.ToInt64(fila["CADCONDTURNCOND"].ToString()));
            MODOCONDUCTA = (string.IsNullOrEmpty(fila["MODOID"].ToString()) ? "" : (fila["MODOID"].ToString()));
            PARTCONDUCTA = (string.IsNullOrEmpty(fila["PARTCONDUCTA"].ToString()) ? "" : (fila["PARTCONDUCTA"].ToString()));
            RELACONDUCTA = (string.IsNullOrEmpty(fila["RELACONDUCTA"].ToString()) ? "" : (fila["RELACONDUCTA"].ToString()));
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Tipo { get; set; }
        public string Genero { get; set; }
        public string Infracciones { get; set; }
        public string InfraccionesId { get; set; }
        public string Conducta { get; set; }
        public string Modo { get; set; }
        public string Participacion { get; set; }
        public string Relacion { get; set; }
        public string FechaConducta { get; set; }
    }
}