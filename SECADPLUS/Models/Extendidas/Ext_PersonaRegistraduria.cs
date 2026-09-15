using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_PersonaRegistraduria
    {
        public Ext_PersonaRegistraduria(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;
            TIPO_IDENTIFICACION = fila["TIPO_IDENTIFICACION"].ToString();
            IDENTIFICACION = fila["IDENTIFICACION"].ToString();
            NOMBRES = fila["NOMBRES"].ToString();
            SEXO = fila["SEXO"].ToString() != "" ? fila["SEXO"].ToString() == "MA" ? "MASCULINO" : "FEMENINO" : "";
            ESTADO_PERSONA = fila["ESTADO_PERSONA"].ToString();
            FECHA_NACIMIENTO = fila["FECHA_NACIMIENTO"].ToString();
            FECHA_EXPEDICION = fila["FECHA_EXPEDICION"].ToString();
            CIUDAD = fila["CIUDAD"].ToString();
            DEPARTAMENTO = fila["DEPARTAMENTO"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string NOMBRES { get; set; }
        public string SEXO { get; set; }
        public string ESTADO_PERSONA { get; set; }
        public string FECHA_NACIMIENTO { get; set; }
        public string FECHA_EXPEDICION { get; set; }
        public string CIUDAD { get; set; }
        public string DEPARTAMENTO { get; set; }
        public string TIPO_IDENTIFICACION { get; set; }
        public string IDENTIFICACION { get; set; }
    }
}