using SECADEntities.SecadLugaresGeograficos;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadPolicia : CADPOLICIA
    {
        public Ext_CadPolicia(DataRow fila, string strBoton)
        {
            NOMB_EMPLEADO = (string.IsNullOrEmpty(fila["NOMB_EMPLEADO"].ToString()) ? "" : (fila["NOMB_EMPLEADO"].ToString()));
            CEDU_EMPLEADO = (string.IsNullOrEmpty(fila["CEDU_EMPLEADO"].ToString()) ? "" : (fila["CEDU_EMPLEADO"].ToString()));
            NUME_PLACA = (string.IsNullOrEmpty(fila["NUME_PLACA"].ToString()) ? 0 : Convert.ToInt32(fila["NUME_PLACA"].ToString()));
            CADUNID_FUERZA = (string.IsNullOrEmpty(fila["CADUNID_FUERZA"].ToString()) ? Convert.ToByte("0") : Convert.ToByte(fila["CADUNID_FUERZA"].ToString()));
            CADUNID_CODIGO = (string.IsNullOrEmpty(fila["CADUNID_CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CADUNID_CODIGO"].ToString()));
            GRAD_ALFABETICO = (string.IsNullOrEmpty(fila["GRAD_ALFABETICO"].ToString()) ? "" : (fila["GRAD_ALFABETICO"].ToString()));
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}