using System.Collections.Generic;
using System.Data;

namespace SECADPLUS.Models
{
    public class _TablasDespachos
    {
        public List<_PedidosPendientesCanalModel> PedidosPendientesCanal { get; set; }
        public List<_PedidosEfectivosRecepcionModel> PedidosEfectivosCanal { get; set; }
        public List<_ControlDespachosModel> ControlDespachosModel { get; set; }
        public List<_EncuestarEventosModel> EncuestarEventosModel { get; set; }
        public List<_PedidosLlamadaModel> PedidosLlamadaModel { get; set; }
        public List<_EventosModel> EventosModel { get; set; }
        public List<_EventosMediosModel> EventosMediosModel { get; set; }
        public List<_BarriosComunasModel> BarriosComunasModel { get; set; }
        public List<_MediosDisponiblesModel> MediosDisponiblesModel { get; set; }
        public List<_Eventos_EventosMediosModel> Eventos_EventosMediosModel { get; set; }
        public List<_AsociarLlamadaModel> AsociarLlamadaModel { get; set; }
        public List<_AnotacionesModel> AnotacionesModel { get; set; }
        public List<_EventoMediosRutaModel> EventoMediosRutaModel { get; set; }
        public List<_SitiosModel> SitiosModel { get; set; }
        public List<_PatrullasModel> PatrullasModel { get; set; }
        public List<_EventosMediosControlModel> EventosMediosControlModel { get; set; }
    }

    public class _PedidosPendientesCanalModel
    {
        public _PedidosPendientesCanalModel(string strDummy, string strNumeLlamada, string strBoton, string strHora, string strTiempo, string strNumeTelefono, string strEstado,
                                            string strDEstado, string strRecurso, string strEventos, string strRecursos, string strCodigo1, string strMpio, string strDireCaso,
                                            string strBarrio, string strPrioridad, string strComentario, string strReceptor, string strMinutosDespacho)
        {
            Dummy = strDummy;
            NUME_LLAMADA = strNumeLlamada;
            BOTON = strBoton;
            HORA = strHora;
            TIEMPO_DESPACHO = strTiempo;
            NUME_TELEFONO = strNumeTelefono;
            ESTADO_CANAL = strEstado;
            D_ESTADO_CANAL = strDEstado;
            DSP_RECURSO = strRecurso;
            CNT_EVENTOS = strEventos;
            CNT_RECURSOS = strRecursos;
            CODIGO1 = strCodigo1;
            MPIO = strMpio;
            DIRE_CASO = strDireCaso;
            BARRIO = strBarrio;
            PRIORIDAD = strPrioridad;
            COMENTARIO = strComentario;
            RECEPTOR = strReceptor;
            MINUTOS_DESPACHO = strMinutosDespacho;
        }

        public string Dummy { get; set; }
        public string NUME_LLAMADA { get; set; }
        public string BOTON { get; set; }
        public string HORA { get; set; }
        public string TIEMPO_DESPACHO { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string ESTADO_CANAL { get; set; }
        public string D_ESTADO_CANAL { get; set; }
        public string DSP_RECURSO { get; set; }
        public string CNT_EVENTOS { get; set; }
        public string CNT_RECURSOS { get; set; }
        public string CODIGO1 { get; set; }
        public string MPIO { get; set; }
        public string DIRE_CASO { get; set; }
        public string BARRIO { get; set; }
        public string PRIORIDAD { get; set; }
        public string COMENTARIO { get; set; }
        public string RECEPTOR { get; set; }
        public string MINUTOS_DESPACHO { get; set; }
    }

    public class _PedidosEfectivosRecepcionModel
    {
        public _PedidosEfectivosRecepcionModel(/*string strDummy, */string strNumeLlamada, string strHora, string strNumeTelefono, string strMpio,
                                               string strDireCaso, string strCodigo1, string strBarrio, string strComuna)
        {
            //Dummy = strDummy;
            NUME_LLAMADA = strNumeLlamada;
            HORA_CASO = strHora;
            NUME_TELEFONO = strNumeTelefono;
            MPIO = strMpio;
            DIRE_CASO = strDireCaso;
            CODIGO1 = strCodigo1;
            BARRIO = strBarrio;
            COMUNA = strComuna;
        }

        //public string Dummy { get; set; }
        public string NUME_LLAMADA { get; set; }

        public string HORA_CASO { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string MPIO { get; set; }
        public string DIRE_CASO { get; set; }
        public string CODIGO1 { get; set; }
        public string BARRIO { get; set; }
        public string COMUNA { get; set; }
    }

    public class _ControlDespachosModel
    {
        public _ControlDespachosModel(string strDummy, string strUsuario, string strNombre, string strLogin, string strEquipo,
                                      string strUbicacion, string strTurno, string strClaseTurno, string strGenerados,
                                      string strPendientes, string strCntMedios, string strFuerza, string strCanal,
                                      string strSitioGraba, string strMinPedidoPend)
        {
            Dummy = strDummy;
            USUARIO = strUsuario;
            NOMBRE = strNombre;
            LOGIN = strLogin;
            EQUIPO = strEquipo;
            UBICACION = strUbicacion;
            TURNO = strTurno;
            CLASE_TURNO = strClaseTurno;
            GENERADOS = strGenerados;
            PENDIENTES = strPendientes;
            CNT_MEDIOS = strCntMedios;
            FUERZA = strFuerza;
            CANAL = strCanal;
            SITIO_GRABA = strSitioGraba;
            MIN_PEDIDO_PEND = strMinPedidoPend;
        }

        public string Dummy { get; set; }
        public string USUARIO { get; set; }
        public string NOMBRE { get; set; }
        public string LOGIN { get; set; }
        public string EQUIPO { get; set; }
        public string UBICACION { get; set; }
        public string TURNO { get; set; }
        public string CLASE_TURNO { get; set; }
        public string GENERADOS { get; set; }
        public string PENDIENTES { get; set; }
        public string CNT_MEDIOS { get; set; }
        public string FUERZA { get; set; }
        public string CANAL { get; set; }
        public string SITIO_GRABA { get; set; }
        public string MIN_PEDIDO_PEND { get; set; }
    }

    public class _EncuestarEventosModel
    {
        public _EncuestarEventosModel(/*string strDummy,*/ string strBoton, string strNumeEvento, string strEstadoEvento, string strFechaCrea, string strUsuario,
                                      string strTurno, string strNumeLlamada)
        {
            //Dummy = strDummy;
            BOTON = strBoton;
            NUME_EVENTO = strNumeEvento;
            ESTADO_EVENTO = strEstadoEvento;
            FECH_CREA = strFechaCrea;
            CADUSUA_EVEN_GENERA = strUsuario;
            CADTURN_TURNO = strTurno;
            CADPEDCA_NUMELLAMADA = strNumeLlamada;
        }

        //public string Dummy { get; set; }
        public string BOTON { get; set; }

        public string NUME_EVENTO { get; set; }
        public string ESTADO_EVENTO { get; set; }
        public string FECH_CREA { get; set; }
        public string CADUSUA_EVEN_GENERA { get; set; }
        public string CADTURN_TURNO { get; set; }
        public string CADPEDCA_NUMELLAMADA { get; set; }
    }

    public class _PedidosLlamadaModel
    {
        public _PedidosLlamadaModel(string strSitioGraba, string strHoraCaso, string strNumeLlamada, string strUsuario, string strNumeTelefono, string strDispTelefonico,
                                    string strPropTelefono, string strDireTelefono, string strLugeTelefono, string strBarrTelefono, string strNombLlamante,
                                    string strDireLlamante, string strTipoInformacion, string strCodigoCooperante, string strPediNumeLlamada, string strComentario, string strCodiPedido,
                                    string strDescCodiPedido, string strCodiPedido2, string strDescCodiPedido2, string strDireCaso, string strDspBarrio, string strDspCiudad,
                                    string strLugeCodio, string strCodigoReal, string strDescCodigoReal)
        {
            SITIO_GRABA = strSitioGraba;
            HORA_CASO = strHoraCaso;
            NUME_LLAMADA = strNumeLlamada;
            CADUSUA_USUARIO = strUsuario;
            NUME_TELEFONO = strNumeTelefono;
            DISP_TELEFONICO = strDispTelefonico;
            PROP_TELEFONO = strPropTelefono;
            DIRE_TELEFONO = strDireTelefono;
            LUGE_TELEFONO = strLugeTelefono;
            BARR_TELEFONO = strBarrTelefono;
            NOMB_LLAMANTE = strNombLlamante;
            DIRE_LLAMANTE = strDireLlamante;
            TIPO_INFORMACION = strTipoInformacion;
            CODIGO_COOPERANTE = strCodigoCooperante;
            CADPEDI_NUME_LLAMADA = strPediNumeLlamada;
            COMENTARIO = strComentario;
            CODI_PEDIDO = strCodiPedido;
            DESC_CODI_PEDIDO = strDescCodiPedido;
            CODI_PEDIDO2 = strCodiPedido2;
            DESC_CODI_PEDIDO2 = strDescCodiPedido2;
            DIRE_CASO = strDireCaso;
            DSP_BARRIO = strDspBarrio;
            DSP_CIUDAD = strDspCiudad;
            LUGE_CODIGO = strLugeCodio;
            CODI_PEDIDO_REAL = strCodigoReal;
            DESC_CODI_PEDIDO_REAL = strDescCodigoReal;
        }

        public string SITIO_GRABA { get; set; }
        public string HORA_CASO { get; set; }
        public string NUME_LLAMADA { get; set; }
        public string CADUSUA_USUARIO { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string DISP_TELEFONICO { get; set; }
        public string PROP_TELEFONO { get; set; }
        public string DIRE_TELEFONO { get; set; }
        public string LUGE_TELEFONO { get; set; }
        public string BARR_TELEFONO { get; set; }
        public string NOMB_LLAMANTE { get; set; }
        public string DIRE_LLAMANTE { get; set; }
        public string TIPO_INFORMACION { get; set; }
        public string CODIGO_COOPERANTE { get; set; }
        public string CADPEDI_NUME_LLAMADA { get; set; }
        public string COMENTARIO { get; set; }
        public string CODI_PEDIDO { get; set; }
        public string DESC_CODI_PEDIDO { get; set; }
        public string CODI_PEDIDO2 { get; set; }
        public string DESC_CODI_PEDIDO2 { get; set; }
        public string DIRE_CASO { get; set; }
        public string DSP_BARRIO { get; set; }
        public string DSP_CIUDAD { get; set; }
        public string LUGE_CODIGO { get; set; }
        public string CODI_PEDIDO_REAL { get; set; }
        public string DESC_CODI_PEDIDO_REAL { get; set; }
    }

    //rmr 02-10-2020 para adicionar datos adicionales
    public class _PedidosDatosAdicModel
    {
        public _PedidosDatosAdicModel(string strSitioGraba, string strNumellamada, string strX_coord, string strY_coord, string strModalidadCaso, string strConductaEspecial,
                                    string strArmaMedio, string strTipoMovilizagresor)
        {
            CADPEDI_SITIOGRABA = strSitioGraba;
            CADPEDI_NUMELLAMADA = strNumellamada;
            X_COORD = strX_coord;
            Y_COORD = strY_coord;
            MODALIDADCASO = strModalidadCaso;
            CONDUCTAESPECIAL = strConductaEspecial;
            ARMAMEDIO = strArmaMedio;
            TIPOMOVILIZAGRESOR = strTipoMovilizagresor;            
        }

        public string CADPEDI_SITIOGRABA { get; set; }
        public string CADPEDI_NUMELLAMADA { get; set; }
        public string X_COORD { get; set; }
        public string Y_COORD { get; set; }
        public string MODALIDADCASO { get; set; }
        public string CONDUCTAESPECIAL { get; set; }
        public string ARMAMEDIO { get; set; }
        public string TIPOMOVILIZAGRESOR { get; set; }        
    }
    //fin rmr 02-10-2020

    //rmr 08-10-2020 para adicionar datos adicionales
    public class _PedidosDatoseventos
    {
        public _PedidosDatoseventos(string strSitioGraba, string strNumeEvento, string strCodiFinal, string strClciCodigo)
        {
            SITIO_GRABA = strSitioGraba;
            NUME_EVENTO = strNumeEvento;
            CADCODI_FINAL = strCodiFinal;
            CADCLCI_CODIGO = strClciCodigo;            
        }
        public string SITIO_GRABA { get; set; }
        public string NUME_EVENTO { get; set; }
        public string CADCODI_FINAL { get; set; }
        public string CADCLCI_CODIGO { get; set; }        
    }
    //fin rmr 08-10-2020

    public class _EventosModel
    {
        public _EventosModel(string strBoton, string strNumeEvento, string strEstadoEvento, string strFechaCrea, string strUsuario,
                                      string strTurno, string strNumeLlamada, string strClciCodigo)
        {
            BOTON = strBoton;
            NUME_EVENTO = strNumeEvento;
            ESTADO_EVENTO = strEstadoEvento;
            FECH_CREA = strFechaCrea;
            CADUSUA_EVEN_GENERA = strUsuario;
            CADTURN_TURNO = strTurno;
            CADPEDCA_NUMELLAMADA = strNumeLlamada;
            CADCLCI_CODIGO = strClciCodigo;
        }

        public string Dummy { get; set; }
        public string BOTON { get; set; }

        public string NUME_EVENTO { get; set; }
        public string ESTADO_EVENTO { get; set; }
        public string FECH_CREA { get; set; }
        public string CADUSUA_EVEN_GENERA { get; set; }
        public string CADTURN_TURNO { get; set; }
        public string CADPEDCA_NUMELLAMADA { get; set; }
        public string CADCLCI_CODIGO { get; set; }
    }

    public class _EventosMediosModel
    {
        public _EventosMediosModel(string strDescUnidad, string strMedioDisp, string strTipoResponsable, string strHoraAsigna, string strHoraLlega,
                                   string strHoraTermina, string strNovedad)
        {
            DSP_DESC_UNIDAD = strDescUnidad;
            DSP_MEDIODISPO = strMedioDisp;
            TIPO_RESPONSABLE = strTipoResponsable;
            HORA_ASIGNA = strHoraAsigna;
            HORA_LLEGA = strHoraLlega;
            HORA_TERMINA = strHoraTermina;
            NOVEDAD = strNovedad;
        }

        public string DSP_DESC_UNIDAD { get; set; }
        public string DSP_MEDIODISPO { get; set; }
        public string TIPO_RESPONSABLE { get; set; }
        public string HORA_ASIGNA { get; set; }
        public string HORA_LLEGA { get; set; }
        public string HORA_TERMINA { get; set; }
        public string NOVEDAD { get; set; }
    }

    public class _BarriosComunasModel
    {
        public _BarriosComunasModel(string strBoton, string strCodigo, string strDescripcion, string strCiudad)
        {
            BOTON = strBoton;
            CODIGO = strCodigo;
            DESCRIPCION = strDescripcion;
            CIUDAD_BARRIO = strCiudad;
        }

        public string BOTON { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string CIUDAD_BARRIO { get; set; }
    }

    public class _MediosDisponiblesModel
    {
        public _MediosDisponiblesModel(string strUnidad, string strPatrulla, string strRadio, string strCuenta,
                                       string strBoton, string stridFuerza, string stridUnidad, string stridPatrulla,
                                       string strConsecutivo)
        {
            DESC_UNIDAD = strUnidad;
            DESC_PATRULLA = strPatrulla;
            ID_RADIO = strRadio;
            CUENTA = strCuenta;
            BOTON = strBoton;
            CADUNITUR_UNDEFUERZA = stridFuerza;
            CADUNITUR_UNDECODIGO = stridUnidad;
            CADPATRU_CODIGO = stridPatrulla;
            CONSECUTIVO = strConsecutivo;
        }

        public string DESC_UNIDAD { get; set; }
        public string DESC_PATRULLA { get; set; }
        public string ID_RADIO { get; set; }
        public string CUENTA { get; set; }
        public string BOTON { get; set; }
        public string CADUNITUR_UNDEFUERZA { get; set; }
        public string CADUNITUR_UNDECODIGO { get; set; }
        public string CADPATRU_CODIGO { get; set; }
        public string CONSECUTIVO { get; set; }
    }

    //rmr adicion para patrullas xcrime 16-01-2021
    public class _MediosXcrimeDisponiblesModel
    {
        public _MediosXcrimeDisponiblesModel(string strUnidad, string strPatrulla, string strRadio, string strCuenta,
                                       string strBoton, string stridFuerza, string stridUnidad, string stridPatrulla,
                                       string strConsecutivo, string strEta)
        {
            DESC_UNIDAD = strUnidad;
            DESC_PATRULLA = strPatrulla;
            ID_RADIO = strRadio;
            CUENTA = strCuenta;
            BOTON = strBoton;
            CADUNITUR_UNDEFUERZA = stridFuerza;
            CADUNITUR_UNDECODIGO = stridUnidad;
            CADPATRU_CODIGO = stridPatrulla;
            CONSECUTIVO = strConsecutivo;
            ETA = strEta;
        }

        public string DESC_UNIDAD { get; set; }
        public string DESC_PATRULLA { get; set; }
        public string ID_RADIO { get; set; }
        public string CUENTA { get; set; }
        public string BOTON { get; set; }
        public string CADUNITUR_UNDEFUERZA { get; set; }
        public string CADUNITUR_UNDECODIGO { get; set; }
        public string CADPATRU_CODIGO { get; set; }
        public string CONSECUTIVO { get; set; }
        public string ETA { get; set; }
    }
    //fin rmr adicion para patrullas xcrime 16-01-2021

    public class _Eventos_EventosMediosModel
    {
        public _Eventos_EventosMediosModel(DataRow fila, string strDummy, string strBoton)
        {
            Dummy = strDummy;
            BOTON = strBoton;
            DSP_DESC_UNIDAD = fila["DSP_DESC_UNIDAD"].ToString();
            DSP_MEDIODISPO = fila["DSP_MEDIODISPO"].ToString();
            TIPO_RESPONSABLE = fila["TIPO_RESPONSABLE"].ToString();
            CRESPONSABLE = fila["CRESPONSABLE"].ToString();
            HORA_ASIGNA = fila["HORA_ASIGNA"].ToString();
            HORA_RUTA = fila["HORA_RUTA"].ToString();
            HORA_LLEGA = fila["HORA_LLEGA"].ToString();
            HORA_TERMINA = fila["HORA_TERMINA"].ToString();
            NOVEDAD = fila["NOVEDAD"].ToString();
            CNOVEDAD = fila["CNOVEDAD"].ToString();
            CADMEDISP_CONSECUTIVO = fila["CADMEDISP_CONSECUTIVO"].ToString();
            HORA_BCONTROL = fila["HORA_BCONTROL"].ToString();
            HORA_REFERENCIA = fila["HORA_SREFERENCIA"].ToString();
            HORA_REGULADO = fila["HORA_PREGULADO"].ToString();
            HORA_IPS = fila["HORA_LLIPS"].ToString();
            HORA_RECIBIDO = fila["HORA_PRECIBIDO"].ToString();
        }

        public string Dummy { get; set; }
        public string BOTON { get; set; }
        public string DSP_DESC_UNIDAD { get; set; }
        public string DSP_MEDIODISPO { get; set; }
        public string TIPO_RESPONSABLE { get; set; }
        public string CRESPONSABLE { get; set; }
        public string HORA_ASIGNA { get; set; }
        public string HORA_RUTA { get; set; }
        public string HORA_LLEGA { get; set; }
        public string HORA_TERMINA { get; set; }
        public string HORA_BCONTROL { get; set; }
        public string HORA_REFERENCIA { get; set; }
        public string HORA_REGULADO { get; set; }
        public string HORA_IPS { get; set; }
        public string HORA_RECIBIDO { get; set; }

        public string NOVEDAD { get; set; }
        public string CNOVEDAD { get; set; }
        public string CADMEDISP_CONSECUTIVO { get; set; }
    }

    public class _AsociarLlamadaModel
    {
        public _AsociarLlamadaModel(string strDummy, string strBoton, string strNumeLlamada, string strHoraCaso,
                                    string strNumeTelefono, string strCaliPedido, string strCiudad, string strNombLlamante,
                                    string strDireCaso, string strCodiPedido, string strEstado, string strSitioGraba)
        {
            Dummy = strDummy;
            Boton = strBoton;
            NUME_LLAMADA = strNumeLlamada;
            HORA_CASO = strHoraCaso;
            NUME_TELEFONO = strNumeTelefono;
            CALI_PEDIDO = strCaliPedido;
            CIUDAD = strCiudad;
            NOMB_LLAMANTE = strNombLlamante;
            DIRE_CASO = strDireCaso;
            CODI_PEDIDO = strCodiPedido;
            ESTADO = strEstado;
            SITIO_GRABA = strSitioGraba;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string NUME_LLAMADA { get; set; }
        public string HORA_CASO { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string CALI_PEDIDO { get; set; }
        public string CIUDAD { get; set; }
        public string NOMB_LLAMANTE { get; set; }
        public string DIRE_CASO { get; set; }
        public string CODI_PEDIDO { get; set; }
        public string ESTADO { get; set; }
        public string SITIO_GRABA { get; set; }
    }

    public class _AnotacionesModel
    {
        public _AnotacionesModel(string strAnotacion, string strFuerza, string strCanal, string strUsuario, string strFecha,
                                 string strConsecutivo, string strDummy)
        {
            Dummy = strDummy;
            ANOTACION = strAnotacion;
            DSP_FUERZA = strFuerza;
            DSP_CANAL = strCanal;
            CADUSUA_GRABA = strUsuario;
            FECHA_GRABACION = strFecha;
            CONSECUTIVO = strConsecutivo;
        }

        public string ANOTACION { get; set; }
        public string DSP_FUERZA { get; set; }
        public string DSP_CANAL { get; set; }
        public string CADUSUA_GRABA { get; set; }
        public string FECHA_GRABACION { get; set; }
        public string CONSECUTIVO { get; set; }
        public string Dummy { get; set; }

    }

    public class _EventoMediosRutaModel
    {
        public _EventoMediosRutaModel(string strDummy, string strBoton, /*string strSitioMedio,*/ string strConsecutivoMedio, /*string strSitioEvento, string strNumeEvento,*/
                                      string strConsecutivoRuta, string strOrigen, string strDestino, string strHoraInicial, string strHoraLlega, /*string strNovedad,*/
                                      string strDesNovedad, string strObservaciones)
        {
            Dummy = strDummy;
            Boton = strBoton;
            //CADMEDISP_SITIOGRABA = strSitioMedio;
            CADMEDISP_CONSECUTIVO = strConsecutivoMedio;
            //CADEVEN_SITIOGRABA = strSitioEvento;
            //CADEVEN_NUME_EVENTO = strNumeEvento;
            CONSECUTIVO_RUTA = strConsecutivoRuta;
            ORIGEN = strOrigen;
            DESTINO = strDestino;
            HORA_INICIA = strHoraInicial;
            HORA_LLEGA = strHoraLlega;
            //NOVEDAD = strNovedad;
            DSC_NOVEDAD = strDesNovedad;
            OBSERVACIONES = strObservaciones;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }

        //public string CADMEDISP_SITIOGRABA { get; set; }
        public string CADMEDISP_CONSECUTIVO { get; set; }

        //public string CADEVEN_SITIOGRABA { get; set; }
        //public string CADEVEN_NUME_EVENTO { get; set; }
        public string CONSECUTIVO_RUTA { get; set; }

        public string ORIGEN { get; set; }
        public string DESTINO { get; set; }
        public string HORA_INICIA { get; set; }
        public string HORA_LLEGA { get; set; }

        //public string NOVEDAD { get; set; }
        public string DSC_NOVEDAD { get; set; }

        public string OBSERVACIONES { get; set; }
    }

    public class _RevistasModel
    {
        public _RevistasModel(string strDummy, string strBoton, string strConsecutivo, string strSitioGraba, string strFecha,
                              string strSitio, string strPatrulla, string strObservaciones)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CONSECUTIVO = strConsecutivo;
            DSP_SITIO_GRABA = strSitioGraba;
            FECHA = strFecha;
            DSP_CADSITI_CODIGO = strSitio;
            DSP_CADPATR_CODIGO = strPatrulla;
            OBSERVACIONES = strObservaciones;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CONSECUTIVO { get; set; }
        public string DSP_SITIO_GRABA { get; set; }
        public string FECHA { get; set; }
        public string DSP_CADSITI_CODIGO { get; set; }
        public string DSP_CADPATR_CODIGO { get; set; }
        public string OBSERVACIONES { get; set; }
    }

    public class _SitiosModel
    {
        public _SitiosModel(string strDummy, string strBoton, string strConsecutivo, string strDescripcion)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CONSECUTIVO = strConsecutivo;
            DESCRIPCION = strDescripcion;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CONSECUTIVO { get; set; }
        public string DESCRIPCION { get; set; }
    }

    public class _PatrullasModel
    {
        public _PatrullasModel(string strDummy, string strBoton, string strUnidadF, string strUnidadC,
                               string strCodigo, string strPatrulla)
        {
            Dummy = strDummy;
            Boton = strBoton;
            CADUNID_FUERZA = strUnidadF;
            CADUNID_CODIGO = strUnidadC;
            CODIGO = strCodigo;
            DESC_PATRULLA = strPatrulla;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string CADUNID_FUERZA { get; set; }
        public string CADUNID_CODIGO { get; set; }
        public string CODIGO { get; set; }
        public string DESC_PATRULLA { get; set; }
    }

    public class _EventosMediosControlModel
    {
        public _EventosMediosControlModel(string strDummy, string strPatrulla, string strBoton, string strCodiPedido, string strCodiPedio2,
                                          string strHoraAsigna, string strHoraRuta, string strHoraLlega, string strDireCaso, string strMinutos,
                                          string strConsecutivo, string strNumeEvento)
        {
            Dummy = strDummy;
            DESC_PATRULLA = strPatrulla;
            Boton = strBoton;
            CODI_PEDIDO = strCodiPedido;
            CODI_PEDIDO2 = strCodiPedio2;
            HORA_ASIGNA = strHoraAsigna;
            HORA_RUTA = strHoraRuta;
            HORA_LLEGA = strHoraLlega;
            DIRE_CASO = strDireCaso;
            MINUTOS = strMinutos;
            CADMEDISP_CONSECUTIVO = strConsecutivo;
            EVENTO_NUMEEVENTO = strNumeEvento;
        }

        public string Dummy { get; set; }
        public string DESC_PATRULLA { get; set; }
        public string Boton { get; set; }
        public string CODI_PEDIDO { get; set; }
        public string CODI_PEDIDO2 { get; set; }
        public string HORA_ASIGNA { get; set; }
        public string HORA_RUTA { get; set; }
        public string HORA_LLEGA { get; set; }
        public string DIRE_CASO { get; set; }
        public string MINUTOS { get; set; }
        public string CADMEDISP_CONSECUTIVO { get; set; }
        public string EVENTO_NUMEEVENTO { get; set; }
    }
    public class _AnotacionPatrulla
    {
        
        public string Anotacion { get; set; }
        public string Boton { get; set; }
    }
}