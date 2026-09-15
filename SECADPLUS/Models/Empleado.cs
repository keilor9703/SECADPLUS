using System;
using System.Data;

namespace SECADPLUS.Models
{
    public class Empleado
    {
        #region Propiedades

        public Empleado(DataRow datos)
        {
            Cedula = (string.IsNullOrEmpty(datos["CEDU_EMPLEADO"].ToString()) ? "" : datos["CEDU_EMPLEADO"].ToString());
            Nombre = (string.IsNullOrEmpty(datos["NOMB_EMPLEADO"].ToString()) ? "" : datos["NOMB_EMPLEADO"].ToString());
            Placa = (string.IsNullOrEmpty(datos["NUME_PLACA"].ToString()) ? 0 : Convert.ToDecimal(datos["NOMB_EMPLEADO"].ToString()));
            Estado = (string.IsNullOrEmpty(datos["ESTADO_EMPLEADO"].ToString()) ? "" : datos["ESTADO_EMPLEADO"].ToString());
            Fuerza = (string.IsNullOrEmpty(datos["FUERZA"].ToString()) ? "" : datos["FUERZA"].ToString());
            Grado = (string.IsNullOrEmpty(datos["GRADO"].ToString()) ? "" : datos["GRADO"].ToString());
        }

        public Empleado(DataRow fila, string strBoton)
        {
            Cedula = fila["CEDU_EMPLEADO"].ToString();
            Nombre = (string.IsNullOrEmpty(fila["EMPLEADO"].ToString()) ? "" : fila["EMPLEADO"].ToString());
            Boton = strBoton;
        }

        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public decimal Placa { get; set; }
        public string Estado { get; set; }
        public string Fuerza { get; set; }
        public string Grado { get; set; }
        public string Boton { get; set; }
        public string Dummy { get; set; }

        #endregion Propiedades
    }
}