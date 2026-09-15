using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_PerRelacionesMujer
    {
        public Ext_PerRelacionesMujer(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;
            PERSATEN_SITIOGRABACION = fila["PERSATEN_SITIOGRABACION"].ToString();
            PERSATEN_CONSECUTIVO = fila["PERSATEN_CONSECUTIVO"].ToString();
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();
            TIPO_INTERVINIENTE = fila["TIPO_INTERVINIENTE"].ToString();
            MEDIO_CONOCIMIENTO = fila["MEDIO_CONOCIMIENTO"].ToString();
            CALIDAD_REPORTANTE = fila["CALIDAD_REPORTANTE"].ToString();
            EDAD_PERSONA = fila["EDAD_PERSONA"].ToString();
            COND_LGTBI = fila["COND_LGTBI"].ToString();
            COND_DISCAPACIDAD = fila["COND_DISCAPACIDAD"].ToString();
            TIPO_DISCAPACIDAD = fila["TIPO_DISCAPACIDAD"].ToString();
            COND_DESPLAZADO = fila["COND_DESPLAZADO"].ToString();
            TIPO_VICT_CONFLICTO = fila["TIPO_VICT_CONFLICTO"].ToString();
            TELEFONO1 = fila["TELEFONO1"].ToString();
            TELEFONO2 = fila["TELEFONO2"].ToString();
            MPIO_RESIDENCIA = fila["MPIO_RESIDENCIA"].ToString();
            COZO_LUGECODIGO_RESIDENCIA = fila["COZO_LUGECODIGO_RESIDENCIA"].ToString();
            COZO_CODIGO_RESIDENCIA = fila["COZO_CODIGO_RESIDENCIA"].ToString();
            BARRIO_CODIGO_RESIDENCIA = fila["BARRIO_CODIGO_RESIDENCIA"].ToString();
            BARRIO_LUGECODIGO_RESIDENCIA = fila["BARRIO_LUGECODIGO_RESIDENCIA"].ToString();
            DIRECCION_RESIDENCIA = fila["DIRECCION_RESIDENCIA"].ToString();
            NIVEL_EDUCATIVO = fila["NIVEL_EDUCATIVO"].ToString();
            ACTIVIDAD_ECONOMICA = fila["ACTIVIDAD_ECONOMICA"].ToString();
            PARTICIP_ORGANIZACION = fila["PARTICIP_ORGANIZACION"].ToString();
            NOMBRE_ORGANIZACION = fila["NOMBRE_ORGANIZACION"].ToString();
            REGIMEN_SALUD = fila["REGIMEN_SALUD"].ToString();
            NOMBRE_IPS = fila["NOMBRE_IPS"].ToString();
            TIPO_RELACION_AGRESOR = fila["TIPO_RELACION_AGRESOR"].ToString();
            RELACION_FAMILIAR_AGRESOR = fila["RELACION_FAMILIAR_AGRESOR"].ToString();
            RELACION_PARTICULAR_AGRESOR = fila["RELACION_PARTICULAR_AGRESOR"].ToString();
            GRUPO_AGRESOR = fila["GRUPO_AGRESOR"].ToString();
            DESCRIPCION_GRUPO = fila["DESCRIPCION_GRUPO"].ToString();
            MECANISMO_ARMA = fila["MECANISMO_ARMA"].ToString();
            LUGAR_OCURRENCIA = fila["LUGAR_OCURRENCIA"].ToString();
            NOMBRE_SITIO = fila["NOMBRE_SITIO"].ToString();

            MPIO_HECHOS = fila["MPIO_HECHOS"].ToString();
            COZO_LUGECODIGO_HECHOS = fila["COZO_LUGECODIGO_HECHOS"].ToString();
            COZO_CODIGO_HECHOS = fila["COZO_CODIGO_HECHOS"].ToString();
            BARRIO_CODIGO_HECHOS = fila["BARRIO_CODIGO_HECHOS"].ToString();
            BARRIO_LUGECODIGO_HECHOS = fila["BARRIO_LUGECODIGO_HECHOS"].ToString();
            DIRECCION_HECHOS = fila["DIRECCION_HECHOS"].ToString();

            TIPO_ESPECTATIVA = fila["TIPO_ESPECTATIVA"].ToString();
            OBSERVACIONES = fila["OBSERVACIONES"].ToString();
            CADUSUARIO = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION = fila["FECHA_TRANSACCION"].ToString();
            NRO_FICHA = fila["NRO_FICHA"].ToString();

            TIPO_IDENTIFICACION = fila["TIPO_IDENTIFICACION"].ToString() != "" ? fila["TIPO_IDENTIFICACION"].ToString() == "TI" ? "Tarjeta De Identidad" : fila["TIPO_IDENTIFICACION"].ToString() == "SD" ? "Sin Documentos" : fila["TIPO_IDENTIFICACION"].ToString() == "RC" ? "Registro Civil"
                                  : fila["TIPO_IDENTIFICACION"].ToString() == "PA" ? "Pasaporte" : fila["TIPO_IDENTIFICACION"].ToString() == "NU" ? "Numero Unico De Identificacion" : fila["TIPO_IDENTIFICACION"].ToString() == "NI" ? "Numero De Identificacion Tributaria"
                                  : fila["TIPO_IDENTIFICACION"].ToString() == "00" ? "Indocumentado" : fila["TIPO_IDENTIFICACION"].ToString() == "CE" ? "Cedula Extranjeria" : fila["TIPO_IDENTIFICACION"].ToString() == "CC" ? "Cedula Ciudadania" : "" : "";

            CODIGO_TIPO_IDENTIFICACION = fila["TIPO_IDENTIFICACION"].ToString();
            IDENTIFICACION = fila["IDENTIFICACION"].ToString();
            NOMBRES = fila["NOMBRES"].ToString();
            APELLIDOS = fila["APELLIDOS"].ToString();
            FECHA_NACE = fila["FECHA_NACE"].ToString();

            GENERO = fila["CODI_GENERO"].ToString() != "" ? fila["CODI_GENERO"].ToString() == "NR" ? "No Reporta" : fila["CODI_GENERO"].ToString() == "MA" ? "Masculino" : fila["CODI_GENERO"].ToString() == "FE" ? "Femenino" : "" : "";
            CODI_GENERO = fila["CODI_GENERO"].ToString();

            RAZA = fila["CODI_RAZA"].ToString() != "" ? fila["CODI_RAZA"].ToString() == "RG" ? "Rom O Gitana" : fila["CODI_RAZA"].ToString() == "RZ" ? "Raizal" : fila["CODI_RAZA"].ToString() == "PL" ? "Palenquero" : fila["CODI_RAZA"].ToString() == "MZ" ? "Mestizo"
                   : fila["CODI_RAZA"].ToString() == "IN" ? "Indigena" : fila["CODI_RAZA"].ToString() == "BL" ? "Blanca" : fila["CODI_RAZA"].ToString() == "AF" ? "Afro" : "" : "";
            CODI_RAZA = fila["CODI_RAZA"].ToString();

            DSP_CIUDAD_RESIDENCIA = fila["DSP_CIUDAD_RESIDENCIA"].ToString() != "Ciudad No registrada" ? fila["DSP_CIUDAD_RESIDENCIA"].ToString() : "";
            dsp_comuna_RESIDENCIA = fila["dsp_comuna_RESIDENCIA"].ToString() != "Comuna no registrado" ? fila["dsp_comuna_RESIDENCIA"].ToString() : "";
            dsp_barrio_RESIDENCIA = fila["dsp_barrio_RESIDENCIA"].ToString() != "Barrio No registrado" ? fila["dsp_barrio_RESIDENCIA"].ToString() : "";

            DSP_MPIOHECOS = fila["DSP_MPIOHECOS"].ToString();
            DSP_BARRIOHECHOS = fila["DSP_BARRIOHECHOS"].ToString();
            DSP_COMUNAHECHOS = fila["DSP_COMUNAHECHOS"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string CODIGO_TIPO_IDENTIFICACION { get; set; }
        public string GENERO { get; set; }
        public string RAZA { get; set; }

        public string DSP_CIUDAD_RESIDENCIA { get; set; }
        public string dsp_comuna_RESIDENCIA { get; set; }
        public string dsp_barrio_RESIDENCIA { get; set; }

        public string DSP_MPIOHECOS { get; set; }
        public string DSP_BARRIOHECHOS { get; set; }
        public string DSP_COMUNAHECHOS { get; set; }

        public string PERSATEN_SITIOGRABACION { get; set; }
        public string PERSATEN_CONSECUTIVO { get; set; }
        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }
        public string TIPO_INTERVINIENTE { get; set; }
        public string MEDIO_CONOCIMIENTO { get; set; }
        public string CALIDAD_REPORTANTE { get; set; }
        public string EDAD_PERSONA { get; set; }
        public string COND_LGTBI { get; set; }
        public string COND_DISCAPACIDAD { get; set; }
        public string TIPO_DISCAPACIDAD { get; set; }
        public string COND_DESPLAZADO { get; set; }
        public string TIPO_VICT_CONFLICTO { get; set; }
        public string TELEFONO1 { get; set; }
        public string TELEFONO2 { get; set; }
        public string MPIO_RESIDENCIA { get; set; }
        public string COZO_LUGECODIGO_RESIDENCIA { get; set; }
        public string COZO_CODIGO_RESIDENCIA { get; set; }
        public string BARRIO_CODIGO_RESIDENCIA { get; set; }
        public string BARRIO_LUGECODIGO_RESIDENCIA { get; set; }
        public string DIRECCION_RESIDENCIA { get; set; }
        public string NIVEL_EDUCATIVO { get; set; }
        public string ACTIVIDAD_ECONOMICA { get; set; }
        public string PARTICIP_ORGANIZACION { get; set; }
        public string NOMBRE_ORGANIZACION { get; set; }
        public string REGIMEN_SALUD { get; set; }
        public string NOMBRE_IPS { get; set; }
        public string TIPO_RELACION_AGRESOR { get; set; }
        public string RELACION_FAMILIAR_AGRESOR { get; set; }
        public string RELACION_PARTICULAR_AGRESOR { get; set; }
        public string GRUPO_AGRESOR { get; set; }
        public string DESCRIPCION_GRUPO { get; set; }
        public string MECANISMO_ARMA { get; set; }
        public string LUGAR_OCURRENCIA { get; set; }
        public string NOMBRE_SITIO { get; set; }
        public string MPIO_HECHOS { get; set; }
        public string COZO_LUGECODIGO_HECHOS { get; set; }
        public string COZO_CODIGO_HECHOS { get; set; }
        public string BARRIO_CODIGO_HECHOS { get; set; }
        public string BARRIO_LUGECODIGO_HECHOS { get; set; }
        public string DIRECCION_HECHOS { get; set; }
        public string TIPO_ESPECTATIVA { get; set; }
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
    }
}