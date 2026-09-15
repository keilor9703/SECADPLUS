using System.Collections.Generic;
using System.Data;

namespace SECADPLUS.Models
{

    public class IncidentesModel
    {
        public IncidentesModel(DataRow fila)
        {
            TIPO = fila["TIPO"].ToString();
            PERSONAS = fila["PERS_CONSECUTIVO"].ToString();
            VEHICULOS = fila["VEHI_CONSECUTIVO"].ToString();
            SOLICITANTE = fila["PATRULLA_SOLICITA"].ToString();
            PATRULLA = fila["INTEGRANTES_PATRULLA"].ToString();
            MOTIVO = fila["MOTIVO"].ToString();            
            COMENTARIO = fila["COMENTARIO"].ToString();
            FECHA = fila["FECHA_GRABACION"].ToString();
            USUARIO = fila["USUA_GRABA"].ToString();

        }
        public string TIPO { get; set; }
        public string Dummy { get; set; }
        public string PERSONAS { get; set; }
        public string VEHICULOS { get; set; }
        public string SOLICITANTE { get; set; }
        public string PATRULLA { get; set; }
        public string MOTIVO { get; set; }
        public string COMENTARIO { get; set; }
        public string FECHA { get; set; }
        public string USUARIO { get; set; }
        
    }
    public class _TablasAdmon
    {
        public List<_CiudadesModel> Ciudades { get; set; }
        public List<_UnidadesModel> Unidades { get; set; }
        public List<_EquiposComputoModel> Equipos { get; set; }
        public List<_ReferenciasSECADModel> Lugares { get; set; }
        public List<_PreguntasModel> Preguntas { get; set; }
        public List<_CasosModel> Casos { get; set; }
        public List<_AtencionCasosModel> AtencionCasos { get; set; }
        public List<_ParqueaderosModel> Parqueaderos { get; set; }
        public List<_SitiosImportantesModel> SitiosImportantes { get; set; }
        public List<_AnotacionesGnrModel> AnotacionesGnr { get; set; }
        public List<_VerConexionesModel> VerConexiones { get; set; }
    }

    public class _CiudadesModel
    {
        public _CiudadesModel(string strDummy, string strBoton, string strCodigo, string strDescripcion, string strTipo, string strDpto)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
            TIPO = strTipo;
            DPTO = strDpto;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string TIPO { get; set; }
        public string DPTO { get; set; }
    }

    public class _UnidadesModel
    {
        public _UnidadesModel(string strDummy, string strBoton, string strCodigo, string strDescripcion)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
    }

    public class _EquiposComputoModel
    {
        public _EquiposComputoModel(string strDummy, string strBoton, string strSitio, string strIdentificacion, string strDescripcion,
                                    string strACD, string strTSNAME, string strUbicacion, string strSerie)
        {
            Dummy = strDummy;
            Boton = strBoton;
            SITIO_GRABA = strSitio;
            IDENTIFICACION = strIdentificacion;
            DESCRIPCION = strDescripcion;
            ACD = strACD;
            TNSNAMES60 = strTSNAME;
            UBICACION = strUbicacion;
            SERIE = strSerie;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string SITIO_GRABA { get; set; }
        public string IDENTIFICACION { get; set; }
        public string DESCRIPCION { get; set; }
        public string ACD { get; set; }
        public string TNSNAMES60 { get; set; }
        public string UBICACION { get; set; }
        public string SERIE { get; set; }
    }

    public class _ReferenciasSECADModel
    {
        public _ReferenciasSECADModel(string strDummy, string strBoton, string strNombre, string strCodigo, string strDescripcion, string strAbreviatura)
        {
            Dummy = strDummy;
            Boton = strBoton;
            NOMBRE = strNombre;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
            ABREVIATURA = strAbreviatura;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string NOMBRE { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string ABREVIATURA { get; set; }
    }

    public class _PreguntasModel
    {
        public _PreguntasModel(string strDummy, string strBoton, string intCodigo, string strDescripcion)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CODIGO = intCodigo;
            DESCRIPCION = strDescripcion;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
    }

    public class _CasosModel
    {
        public _CasosModel(string strDummy, string strBoton, string strCodigo, string strDescripcion, string strPrioridad, string strImportancia, string strTipoUso)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
            PRIORIDAD = strPrioridad;
            IMPORTANCIA = strImportancia;
            TIPO_USO = strTipoUso;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string PRIORIDAD { get; set; }
        public string IMPORTANCIA { get; set; }
        public string TIPO_USO { get; set; }
    }

    public class _AtencionCasosModel
    {
        public _AtencionCasosModel(string strDummy, string strBoton, string strCodigo, string strDescripcion)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
    }

    public class _ParqueaderosModel
    {
        public _ParqueaderosModel(string strDummy, string strCodigo, string strDescripcion, string strCiudad, string strBarrio,
                                  string strDireccion, string strTelefono, string strHorario, string strUsuario, string strFecha)
        {
            Dummy = strDummy;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
            NOMBRECIUDAD = strCiudad;
            NOMBREBARRIO = strBarrio;
            DIRECCION = strDireccion;
            TELEFONO = strTelefono;
            HORARIO = strHorario;
            USUA_TRANSACCION = strUsuario;
            FECHA_TRANSACCION = strFecha;
        }

        public string Dummy { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string NOMBRECIUDAD { get; set; }
        public string NOMBREBARRIO { get; set; }
        public string DIRECCION { get; set; }
        public string TELEFONO { get; set; }
        public string HORARIO { get; set; }
        public string USUA_TRANSACCION { get; set; }
        public string FECHA_TRANSACCION { get; set; }
    }
}

public class _AnotacionesGnrModel
{
    public _AnotacionesGnrModel(string strDummy, string strBoton, string strUsuario, string strConsecutivo, string strSitioGraba, string strFechaGrabacion, string strAnotacion, string strMaquina)
    {
        Dummy = strDummy;
        Boton = strBoton;
        USUARIO = strUsuario;
        CONSECUTIVO = strConsecutivo;
        SITIO_GRABA = strSitioGraba;
        FECHA_GRABACION = strFechaGrabacion;
        ANOTACION = strAnotacion;
        MAQUINA = strMaquina;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string USUARIO { get; set; }
    public string CONSECUTIVO { get; set; }
    public string SITIO_GRABA { get; set; }
    public string FECHA_GRABACION { get; set; }
    public string ANOTACION { get; set; }
    public string MAQUINA { get; set; }
}

public class _AnotacionesEventosModel
{
    public _AnotacionesEventosModel(string strDummy, string strBoton, string strUsuario, string strConsecutivo, string strFechaGrabacion, string strAnotacion)
    {
        Dummy = strDummy;
        Boton = strBoton;
        CADUSUA_GRABA = strUsuario;
        CONSECUTIVO = strConsecutivo;
        FECHA_GRABACION = strFechaGrabacion;
        ANOTACION = strAnotacion;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string CADUSUA_GRABA { get; set; }
    public string CONSECUTIVO { get; set; }
    public string FECHA_GRABACION { get; set; }
    public string ANOTACION { get; set; }
}

public class _VerConexionesModel
{
    public _VerConexionesModel(string strDummy, string strBoton, string strCondicion, string strGrado, string strNombre, string strFecha, string strFechaHasta,
        string strSesionId, string strMaquina, string strUsuario, string strfuerza, string strCanal)
    {
        Dummy = strDummy;
        Boton = strBoton;
        DESC_CONDICION = strCondicion;
        GRAD_ALFABETICO = strGrado;
        NOMB_EMPLEADO = strNombre;
        FECHA_INICIO = strFecha;
        FECHA_HASTA = strFechaHasta;
        SESION_ID = strSesionId;
        MAQUINA = strMaquina;
        CADUSUA_USUARIO = strUsuario;
        CADCANA_FUERZA_ID = strfuerza;
        CADCANA_CODIGO = strCanal;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string DESC_CONDICION { get; set; }
    public string GRAD_ALFABETICO { get; set; }
    public string NOMB_EMPLEADO { get; set; }
    public string FECHA_INICIO { get; set; }
    public string FECHA_HASTA { get; set; }
    public string SESION_ID { get; set; }
    public string MAQUINA { get; set; }
    public string CADUSUA_USUARIO { get; set; }
    public string CADCANA_FUERZA_ID { get; set; }
    public string CADCANA_CODIGO { get; set; }
}

public class _SitiosImportantesModel
{
    public _SitiosImportantesModel(string strDummy, string strConsecutivo, string strClaseSitio, string strDescripcion, string strCiudad,
                                    string strBarrio, string strDireccion, string strLatitud, string strLongitud, string strTelefono, string strComentario)
    {
        Dummy = strDummy;
        CONSECUTIVO = strConsecutivo;
        CLASESITIODESCRIPCION = strClaseSitio;
        DESCRIPCION = strDescripcion;
        NOMBRECIUDAD = strCiudad;
        NOMBREBARRIO = strBarrio;
        DIRECCION = strDireccion;
        LATITUD = strLatitud;
        LONGITUD = strLongitud;
        TELEFONO = strTelefono;
        COMENTARIO = strComentario;
    }

    public string Dummy { get; set; }
    public string CONSECUTIVO { get; set; }
    public string CLASESITIODESCRIPCION { get; set; }
    public string DESCRIPCION { get; set; }
    public string NOMBRECIUDAD { get; set; }
    public string NOMBREBARRIO { get; set; }
    public string DIRECCION { get; set; }
    public string LATITUD { get; set; }
    public string LONGITUD { get; set; }
    public string TELEFONO { get; set; }
    public string COMENTARIO { get; set; }
}

public class _DirectorioModel
{
    public _DirectorioModel(string strDummy, string strBoton, string strTelefono, string strPropietario, string strDireccion, string strBarrio, string nit, string strLugeCodigo, string strCadLuge, string strBarrrioCodigo, string strBarrioAsignado)
    {
        Dummy = strDummy;
        Boton = strBoton;
        NUME_TELEFONO = strTelefono;
        NOMB_PROPIETARIO = strPropietario;
        NIT = nit;
        DIRE_PROPIETA = strDireccion;
        BARRIO = strBarrio;
        CADLUGE_CODIGO = strLugeCodigo;
        CADLUGE = strCadLuge;
        BARR_CODIGO = strBarrrioCodigo;
        BARRIO_ASIGNADO = strBarrioAsignado;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string NUME_TELEFONO { get; set; }
    public string NOMB_PROPIETARIO { get; set; }
    public string DIRE_PROPIETA { get; set; }
    public string BARRIO { get; set; }
    public string NIT { get; set; }
    public string CADLUGE_CODIGO { get; set; }
    public string CADLUGE { get; set; }
    public string BARR_CODIGO { get; set; }
    public string BARRIO_ASIGNADO { get; set; }
}

public class _BarriosModel
{
    public _BarriosModel(string strDummy, string strBoton, string strCodigo, string strDescripcion)
    {
        Dummy = strDummy;
        Boton = strBoton;
        CODIGOBARRIO = strCodigo;
        DESCRIPCIONBARRIO = strDescripcion;
    }
    public _BarriosModel(string strDummy, string strBoton, string strCodigo, string strDescripcion, string strComuna)
    {
        Dummy = strDummy;
        Boton = strBoton;
        CODIGOBARRIO = strCodigo;
        DESCRIPCIONBARRIO = strDescripcion;
        COMUNA = strComuna;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string CODIGOBARRIO { get; set; }
    public string DESCRIPCIONBARRIO { get; set; }
    public string COMUNA { get; set; }
}

public class _FotosEventosModel
{
    public _FotosEventosModel(string strDummy, string strBoton, string strConsecutivo, string strNumeroEvento, string strObservacion, string strFoto)
    {
        Dummy = strDummy;
        Boton = strBoton;
        CADEVEN_NUMEEVENTO = strNumeroEvento;
        OBSERVACION = strObservacion;
        FOTO = strFoto;
        CONSECUTIVO = strConsecutivo;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string CADEVEN_NUMEEVENTO { get; set; }
    public string CONSECUTIVO { get; set; }
    public string OBSERVACION { get; set; }
    public string FOTO { get; set; }
}

public class _NovedadesPatrullasModel
{
    public _NovedadesPatrullasModel(string strDummy, string strBoton, string strMedio, string strTipoNovedad, string strHoraInicial, string strHoraTermina, string strObservaciones)
    {
        Dummy = strDummy;
        Boton = strBoton;
        MEDIO = strMedio;
        DESC_TIPO_NOVEDAD = strTipoNovedad;
        HORA_INICIA = strHoraInicial;
        HORA_TERMINA = strHoraTermina;
        OBSERVACION = strObservaciones;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string MEDIO { get; set; }
    public string DESC_TIPO_NOVEDAD { get; set; }
    public string HORA_INICIA { get; set; }
    public string HORA_TERMINA { get; set; }
    public string OBSERVACION { get; set; }
}

public class _MediosModel
{
    public _MediosModel(string strDummy, string strBoton, string strDescPatrulla, string strSitioGraqba, string strConsecutivo, string strEstadoPatrulla, string strTipo, string strEstado, string strCuenta)
    {
        Dummy = strDummy;
        Boton = strBoton;
        DESC_PATRULLA = strDescPatrulla;
        SITIO_GRABA = strSitioGraqba;
        CONSECUTIVO = strConsecutivo;
        ESTADO_PATRULLA = strEstadoPatrulla;
        TIPO = strTipo;
        ESTADO = strEstado;
        CUENTA = strCuenta;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string DESC_PATRULLA { get; set; }
    public string SITIO_GRABA { get; set; }
    public string CONSECUTIVO { get; set; }
    public string ESTADO_PATRULLA { get; set; }
    public string TIPO { get; set; }
    public string ESTADO { get; set; }
    public string CUENTA { get; set; }
}

public class _BienesRelacionadosModel
{
    public _BienesRelacionadosModel(string strDummy, string strBoton, string strConsecutivo, string strUsuario,
        string strFechaTransaccion, string strPlaca, string strChasis, string strMotor, string strSerie, string strTipoBien, string strClase, string strColor, string strMarca, string strLinea, string strObservaciones)
    {
        Dummy = strDummy;
        Boton = strBoton;
        CONSECUTIVO = strConsecutivo;
        USUA_TRANSACCION = strUsuario;
        FECHA_TRANSACCION = strFechaTransaccion;
        PLACA = strPlaca;
        CHASIS = strChasis;
        MOTOR = strMotor;
        SERIE = strSerie;
        TIPO_VEHICULO = strTipoBien;
        CLASE_BIEN = strClase;
        COLOR = strColor;
        MARCA = strMarca;
        LINEA = strLinea;
        OBSERVACIONES = strObservaciones;
    }

    public string Dummy { get; set; }
    public string Boton { get; set; }
    public string CONSECUTIVO { get; set; }
    public string USUA_TRANSACCION { get; set; }
    public string FECHA_TRANSACCION { get; set; }
    public string PLACA { get; set; }
    public string CHASIS { get; set; }
    public string MOTOR { get; set; }
    public string SERIE { get; set; }
    public string TIPO_VEHICULO { get; set; }
    public string CLASE_BIEN { get; set; }
    public string COLOR { get; set; }
    public string MARCA { get; set; }
    public string LINEA { get; set; }
    public string OBSERVACIONES { get; set; }
}