using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_FichaSocialPersona
    {


        public Ext_FichaSocialPersona(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;
           
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();
            NRO_FICHA = fila["NRO_FICHA"].ToString();
            CONSECUTIVO = fila["CONSECUTIVO"].ToString();
            TIPO_IDENTIFICACION = fila["TIPO_IDENTIFICACION"].ToString() != "" ? fila["TIPO_IDENTIFICACION"].ToString() == "TI" ? "Tarjeta De Identidad" : fila["TIPO_IDENTIFICACION"].ToString() == "SD" ? "Sin Documentos" : fila["TIPO_IDENTIFICACION"].ToString() == "RC" ? "Registro Civil"
                                 : fila["TIPO_IDENTIFICACION"].ToString() == "PA" ? "Pasaporte" : fila["TIPO_IDENTIFICACION"].ToString() == "NU" ? "Numero Unico De Identificacion" : fila["TIPO_IDENTIFICACION"].ToString() == "NI" ? "Numero De Identificacion Tributaria"
                                 : fila["TIPO_IDENTIFICACION"].ToString() == "00" ? "Indocumentado" : fila["TIPO_IDENTIFICACION"].ToString() == "CE" ? "Cedula Extranjeria" : fila["TIPO_IDENTIFICACION"].ToString() == "CC" ? "Cedula Ciudadania" : "" : "";

            CODIGO_TIPO_IDENTIFICACION = fila["TIPO_IDENTIFICACION"].ToString();
            IDENTIFICACION = fila["IDENTIFICACION"].ToString();
            NOMBRES = fila["NOMBRES"].ToString();
            APELLIDOS = fila["APELLIDOS"].ToString();
            FECHA_NACE = fila["FECHA_NACE"].ToString();
            EDAD_PERSONA = fila["EDAD_PERSONA"].ToString();



            GENERO = fila["CODI_GENERO"].ToString() != "" ? fila["CODI_GENERO"].ToString() == "NR" ? "No Reporta" : fila["CODI_GENERO"].ToString() == "MA" ? "Masculino" : fila["CODI_GENERO"].ToString() == "FE" ? "Femenino" : "" : "";
            CODI_GENERO = fila["CODI_GENERO"].ToString();

            RAZA = fila["CODI_RAZA"].ToString() != "" ? fila["CODI_RAZA"].ToString() == "RG" ? "Rom O Gitana" : fila["CODI_RAZA"].ToString() == "RZ" ? "Raizal" : fila["CODI_RAZA"].ToString() == "PL" ? "Palenquero" : fila["CODI_RAZA"].ToString() == "MZ" ? "Mestizo"
                   : fila["CODI_RAZA"].ToString() == "IN" ? "Indigena" : fila["CODI_RAZA"].ToString() == "BL" ? "Blanca" : fila["CODI_RAZA"].ToString() == "AF" ? "Afro" : "" : "";
            CODI_RAZA = fila["CODI_RAZA"].ToString();

            OBSERVACIONES = fila["OBSERVACIONES"].ToString();
            CADUSUARIO = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION = fila["FECHA_TRANSACCION"].ToString();



            COND_DISCAPACIDAD = fila["COND_DISCAPACIDAD"].ToString() == "S" ? "SI" : "NO";
            COND_LGTBI = fila["COND_LGTBI"].ToString().ToString() == "S" ? "SI" : "NO";
            COND_DESPLAZADO = fila["COND_DESPLAZADO"].ToString().ToString() == "S" ? "SI" : "NO";



        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string CODIGO_TIPO_IDENTIFICACION { get; set; }
        public string GENERO { get; set; }
        public string RAZA { get; set; }    
        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }      
        public string OBSERVACIONES { get; set; }
        public string CADUSUARIO { get; set; }
        public string FECHA_TRANSACCION { get; set; }
        public string NRO_FICHA { get; set; }
        public string TIPO_IDENTIFICACION { get; set; }
        public string IDENTIFICACION { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public string FECHA_NACE { get; set; }
        public string CODI_GENERO { get; set; }
        public string CODI_RAZA { get; set; }
        public string CONSECUTIVO { get; set; }
        public string EDAD_PERSONA { get; set; }



        public string COND_DISCAPACIDAD { get; set; }
        public string COND_LGTBI { get; set; }
        public string COND_DESPLAZADO { get; set; }




    }
}