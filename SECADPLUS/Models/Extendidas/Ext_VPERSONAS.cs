using SECADEntities.SecadLugaresGeograficos;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_VPERSONAS : V_PERSONAS
    {
        public string fecha_Nacimiento { get; set; }

        public Ext_VPERSONAS(DataRow fila, string strBoton = null)
        {
            CEDU_EMPLEADO = (fila["CEDU_EMPLEADO"].ToString());
            NOMB_EMPLEADO = (fila["NOMB_EMPLEADO"].ToString());
            NUME_PLACA = int.Parse(fila["NUME_PLACA"].ToString() != string.Empty ? fila["NUME_PLACA"].ToString() : "0");
            DIRE_RESIDENCIA = (fila["DIRE_RESIDENCIA"].ToString());
            TELE_RESIDENCIA = (fila["TELE_RESIDENCIA"].ToString());

            if (!string.IsNullOrEmpty(fila["FECH_NACIMIENTO"].ToString()))
                fecha_Nacimiento = Convert.ToDateTime(fila["FECH_NACIMIENTO"].ToString()).ToShortDateString();
            DESCRIPCIONSEXO = (fila["DESCRIPCIONSEXO"].ToString());
            DESCRIPCIONGRUPOSANGUINEO = (fila["DESCRIPCIONGRUPOSANGUINEO"].ToString());
            DESCRIPCIONESTADOCIVIL = (fila["DESCRIPCIONESTADOCIVIL"].ToString());
            DESCRIPCIONESPECIALIDAD = (fila["DESCRIPCIONESPECIALIDAD"].ToString());
            DESCRIPCIONTIPOLABORAL = (fila["DESCRIPCIONTIPOLABORAL"].ToString());
            DESCRIPCIONSECCION = (fila["DESCRIPCIONSECCION"].ToString());
            DESCRIPCIONTIPOFUERZA = (fila["DESCRIPCIONTIPOFUERZA"].ToString());
            DESCRIPCIONUNIDADES = (fila["DESCRIPCIONUNIDADES"].ToString());
            DESCRIPCIONGRADO = (fila["DESCRIPCIONGRADO"].ToString());
            DESCRIPCIONFUERZA = (fila["DESCRIPCIONFUERZA"].ToString());
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}