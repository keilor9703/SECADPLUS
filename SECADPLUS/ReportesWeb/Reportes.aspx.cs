using Microsoft.Reporting.WebForms;
using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace SECADPLUS.Temporal
{
    public partial class Page : System.Web.UI.Page
    {
        private string strFuerza, strNivel, strCodigo, strReporte, strVigente, strOperativa, strRangoFecha, strUsuarioOid, strIp, strId;
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));

        private int intOpcion = 0;
        private GestorGeneral gestor;
        private DataTable tabla;
        private DataTable tabla2;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Verifica que existan variables
                strReporte = string.IsNullOrEmpty(Request["REPORTE"]) ? "0" : Request["REPORTE"].ToString();
                strNivel = string.IsNullOrEmpty(Request["NIVEL"]) ? "0" : Request["NIVEL"].ToString();
                strFuerza = string.IsNullOrEmpty(Request["FUERZA"]) ? "0" : Request["FUERZA"].ToString();
                strCodigo = string.IsNullOrEmpty(Request["CODIGO"]) ? "0" : Request["CODIGO"].ToString();
                strVigente = string.IsNullOrEmpty(Request["VIGENTE"]) ? "" : Request["VIGENTE"].ToString();
                strOperativa = string.IsNullOrEmpty(Request["OPERATIVA"]) ? "" : Request["OPERATIVA"].ToString();
                intOpcion = string.IsNullOrEmpty(Request["OPCION"]) ? 0 : Convert.ToInt32(Request["OPCION"].ToString());
                strRangoFecha = string.IsNullOrEmpty(Request["RF"]) ? "" : Request["RF"].ToString();
                strUsuarioOid = string.IsNullOrEmpty(Request["US"]) ? "" : Request["US"].ToString();
                strId = string.IsNullOrEmpty(Request["ID"]) ? "0" : Request["ID"].ToString();
                strIp = string.IsNullOrEmpty(Request["IP"]) ? "" : Request["IP"].ToString();
                GenerateReport();
            }
        }

        /// <summary>
        /// Administra los métodos que generan cada uno de los reportes
        /// </summary>
        public void GenerateReport()
        {
            switch (strReporte)
            {
                case "UNIDADES":
                    GenerarReporteUnidades();
                    break;

                case "MEDIOS":
                    GenerarReporteMedios();
                    break;

                case "EXCEPCIONES":
                    GenerarReporteExcepciones();
                    break;

                case "CONDUCTASPONDERACION":
                    GenerarReporteConductasPonderacion();
                    break;

                case "PLANILLATURNO":
                    GenerarReportePlanillaTurno();
                    break;

                case "RESUMENPLANILLA":
                    GenerarReporteResumenPlanilla();
                    break;

                case "RESUMENSITIOGRABA":
                    GenerarReportesResumenLllamdasSitioGraba();
                    break;

                case "SEGUIMIENTOCASOS":
                    GenerarReportesSeguimientoCasos();
                    break;

                case "ANTECEDENTESUSUARIO":
                    GenerarReporteAntecedentesPorUsuario();
                    break;

                case "PLANILLACONTROLTIEMPOS":
                    GenerarReportePlanillaControlTiempos();
                    break;

                case "LLAMADASBARRIOSYCODIGO":
                    GenerarReporteBarriosYCodigo();
                    break;

                case "PROMEDIOATENCIONDIAS":
                    GenerarReportePromedioAtencionDias();
                    break;

                case "LLAMADASDIAHORA":
                    GenerarReporteLlamadasDiaHora();
                    break;

                case "LLAMADASTELEFONO":
                    GenerarReporteLlamadasTelefono();
                    break;

                case "LLAMADASUSUARIO":
                    GenerarReporteLlamadasUsuario();
                    break;

                case "EVENTOSUSUARIO":
                    GenerarReporteEventosUsuario();
                    break;

                case "CASOSPATRULLAS":
                    GenerarReporteCantidadCasosPatrulla();
                    break;

                case "EVENTOSENTIDADCANAL":
                    GenerarReporteEventosEntidadCanal();
                    break;

                case "EVALUACIONUNIDAD":
                    GenerarReporteEvaluacionUnidad();
                    break;

                case "EVALUACIONPERSONAL":
                    GenerarReporteEvaluacionPersonal();
                    break;

                case "RESUMENEVALUACION":
                    GenerarReporteResumenEvaluacion();
                    break;

                case "DATAMART":
                    GenerarReporteDataMartLlamadasyEventos();
                    break;

                case "CASOSUNIDADTIEMPO":
                    GenerarReporteCasosUnidadTiempo();
                    break;

                case "PLANILLALLAMADASPERIODOS":
                    GenerarReportePlanillaPeriodos();
                    break;

                case "RESUMENLLAMADASPERIODOS":
                    GenerarReporteResumenLlamadasPeriodos();
                    break;

                case "REPORTEAUDITORIA":
                    GenerarReporteAuditoria();
                    break;

                case "REVISTAS":
                    GenerarReporteRevistas();
                    break;

                case "RESTADISTICOENCUESTAS":
                    GenerarReporteEstadisticoEncuestas();
                    break;

                case "LISTADOUSUARIOS":
                    GenerarReporteListadoUsuarios();
                    break;

                case "ACTAUSUARIO":
                    GenerarReporteActaUsuario();
                    break;

                case "PARTEGEONUMERICO":
                    GenerarReporteParteGeonumerico();
                    break;

                case "MINUTASERVICIOATENCION":
                    GenerarReporteMinutaServicio();
                    break;

                case "ANOTACIONESGENERALES":
                    GenerarReporteAnotacionesGenerales();
                    break;

            }
        }



        /// <summary>
        /// Genera el reporte de anotaciones generales
        /// </summary>
        private void GenerarReporteAnotacionesGenerales()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                string strEntidad = Request.Form["Entidad"] == "" ? "0" : Request.Form["Entidad"];
                string strConsecutivo = Request.Form["Consecutivo"] == "" ? "0" : Request.Form["Consecutivo"];
                string strUsuario = Request.Form["Usuario"] == "" ? "0" : Request.Form["Usuario"];
                string strFecha1 = Request.Form["FechaInicio"];
                string strFecha2 = Request.Form["FechaFinal"];
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strElaboro = Request.Form["ElaboradoPor"];

                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteAnotacionesGrls.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", strEntidad.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", strFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", strFecha2.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consecutivo", strConsecutivo, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", strUsuario, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteAnotacionesGrls", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));

                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("AnotacionesGenerales", tabla));

                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("Elaboro");

                string fechaDB = ObtenerFechaDB();
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = fechaDB;
                string strPie = ObtenerTelefonosDireccionTelematica();
                tabla.Rows[0]["Titulo"] = "Sistema de Seguimiento y Control de Atención a Casos SECAD123 \r\n"
                    + "Anotaciones Generales";
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de minuta del servicio
        /// </summary>
        private void GenerarReporteMinutaServicio()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                int iFuerza = int.Parse(Request.Form["Fuerza"]);
                string strTurno = Request.Form["Turno"] == string.Empty ? "0" : Request.Form["Turno"];
                string strCondicion = Request.Form["Condicion"];
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strElaboro = Request.Form["ElaboradoPor"];

                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                CADFUERZA objFuerza = (from t in db.CADFUERZAS where t.ID == iFuerza select t).FirstOrDefault();

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteMinutaServicio.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", iFuerza.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", strTurno, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", objFuerza.CADUNDE_CODIGO.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", objFuerza.CADUNDE_FUERZA.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteMinutaServicio", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("MinutaServicio", tabla));

                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("Elaboro");

                string fechaDB = ObtenerFechaDB();
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = fechaDB;
                string strPie = ObtenerTelefonosDireccionTelematica();
                tabla.Rows[0]["Titulo"] = "Sistema de Seguimiento y Control de Atención a Casos SECAD123 \r\n"
                    + "Minuta Servicio - Por Turno y Entidad";
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de parte geonumérico
        /// </summary>
        private void GenerarReporteParteGeonumerico()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strFecha1 = Request.Form["FechaInicio"];
                string strFecha2 = Request.Form["FechaFinal"];
                int iEntidad = int.Parse(Request.Form["Fuerza"]);
                string strCondicion = Request.Form["Condicion"];
                //string strResponsable = Request.Form["Responsable"];
                //string strCargoResponsable = Request.Form["CargoResponsable"];
                ReportParameter parametro;
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                CADFUERZA objFuerza = (from t in db.CADFUERZAS where t.ID == iEntidad select t).FirstOrDefault();

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteGeonumerico.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", strFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", strFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", iEntidad.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerunirep", objFuerza.CADUNDE_FUERZA.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unirep", objFuerza.CADUNDE_CODIGO.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteGeonumerico", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Geonumerico", tabla));

                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Condicion");
                tabla.Columns.Add("FECHADB");
                string fechaDB = ObtenerFechaDB();

                tabla.Rows[0]["FECHADB"] = fechaDB;
                tabla.Rows[0]["Condicion"] = strCondicion;
                tabla.Rows[0]["Titulo"] = "PERSONAL EN SERVICIO DEL " + strFecha1 + " AL " + strFecha2;
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                Reporte.LocalReport.Refresh();
                parametro = new ReportParameter("CONDI", strCondicion);
                Reporte.LocalReport.SetParameters(parametro);
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte acta de usuario
        /// </summary>
        private void GenerarReporteActaUsuario()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strCedula = Request.Form["Cedula"];
                string strEntidad = Request.Form["Fuerza"];
                string strCondicion = Request.Form["Condicion"];
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];

                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteActaUsuario.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", strEntidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_condicion", strCondicion, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actaUsuario", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ActaUsuario", tabla));

                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("CF_UNIDAD");
                tabla.Columns.Add("Pie");
                string strCF_UNIDAD = (string)(from t in db.CADFUERZAS where t.CADUNDE_FUERZA == usr.CADUNDE_FUERZA && t.CADUNDE_CODIGO == usr.CADUNDE_CODIGO select t.DESCRIPCION).FirstOrDefault();
                tabla.Rows[0]["CF_UNIDAD"] = strCF_UNIDAD;
                string fechaDB = ObtenerFechaDB();
                string strCiudadUsuario = (from t in db.CADLUGARES_GEOGRAFICOS where t.CODIGO == usr.LUGE_CODIGO_CAD select t.DESCRIPCION).FirstOrDefault();
                string strCiudadFecha = strCiudadUsuario + " " + fechaDB.Split(' ')[0];
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = fechaDB;
                tabla.Columns.Add("CiudadFecha");
                tabla.Rows[0]["CiudadFecha"] = strCiudadFecha;
                string strPie = ObtenerTelefonosDireccionTelematica();
                tabla.Rows[0]["Pie"] = strPie;
                tabla.Rows[0]["Titulo"] = "Entrega de usuarios";
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Obtiene la dirección de telemática de la base de datos
        /// </summary>
        /// <returns></returns>
        private string ObtenerTelefonosDireccionTelematica()
        {
            try
            {
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();

                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                DataTable tablaFecha = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_pieTelefonosDireccion", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                return tablaFecha.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return "Error en fecha de base de datos";
            }
        }

        /// <summary>
        /// Genera el reporte de listado de usuarios
        /// </summary>
        private void GenerarReporteListadoUsuarios()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                string strEntidad = Request.Form["Entidad"];
                string strCondicion = Request.Form["Condicion"];
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];

                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteListadoUsuarios.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", strEntidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_condicion", strCondicion, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listadoUsuarios", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ListadoUsuarios", tabla));

                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("FECHADB");

                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                string p_entidad = (string)(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TITULO_REPORTES" && t.CODIGO == "01" select t.DESCRIPCION).FirstOrDefault();
                string p_entidad1 = (string)(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TITULO_REPORTES" && t.CODIGO == "02" select t.DESCRIPCION).FirstOrDefault();
                tabla.Rows[0]["Titulo"] = p_entidad1 + " \r\n " + p_entidad + "\r\n LISTADO DE USUARIOS";
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte estadístico de encuestas
        /// </summary>
        private void GenerarReporteEstadisticoEncuestas()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                string strFecha1 = Request.Form["FechaInicio"];
                string strFecha2 = Request.Form["FechaFinal"];
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteEncuestaLlamadas.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", strFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", strFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_reporteEncuestas", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("EstadisticoEncuestas", tabla));

                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("CargoElaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "Reporte Encuestas Realizadas del "
                    + strFecha1 + " al " + strFecha2;
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                tabla.Rows[0]["CargoElaboro"] = strCargoElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Trae la fecha actual de la base de datos
        /// </summary>
        /// <returns></returns>
        private string ObtenerFechaDB()
        {
            try
            {
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();

                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                DataTable tablaFecha = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_horaBaseDatos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                return tablaFecha.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return "Error en fecha de base de datos";
            }
        }

        /// <summary>
        /// Genera el reporte de auditoría
        /// </summary>
        private void GenerarReporteAuditoria()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string strFecha1 = Request.Form["FechaInicio"];
                string strFecha2 = Request.Form["FechaFinal"];
                string strDireccionIP = Request.Form["DireccionIP"];
                string strTipoEvento = Request.Form["TipoEvento"];
                string strUsuario = Request.Form["Usuario"];
                string strTabla = Request.Form["Tabla"];
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strElaboro = Request.Form["Elaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/TablasAuditoria.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", strFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", strFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_direccionIP", strDireccionIP == string.Empty ? "::1" : strDireccionIP, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoEvento", strTipoEvento, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", strUsuario == string.Empty ? "T" : strUsuario, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tabla", strTabla, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteAuditoria", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("RegistrosAuditoria", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NombreUsuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "Reporte Tablas de Auditoría del "
                    + strFecha1 + " al " + strFecha2;
                tabla.Rows[0]["NombreUsuario"] = usr.USUARIO;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de datamart
        /// </summary>
        private void GenerarReporteDataMartLlamadasyEventos()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                double dTiempoLimite = double.Parse(Request.Form["TXT_TIEMPO"].Replace(".", ","));
                //string strResponsable = Request.Form["Responsable"];
                //string strCargoResponsable = Request.Form["CargoResponsable"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteGraficoLlamadasEventos.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", usr.SITIO_GRABACION.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_graficoLlamadasEventos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("EventosGraficoLlamadasEventos", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "Resumén Llamadas y Eventos \r\n Período del "
                    + dFecha1 + " AL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de revistas
        /// </summary>
        private void GenerarReporteRevistas()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string strSitioGraba = Request.Form["SITIOS_GRABA"];
                string strEntidad = Request.Form["ENTIDADES"];
                string strConsecutivo = Request.Form["CONSECUTIVO"];
                string strFirma = Request.Form["FIRMA"];
                string strCargoFirma = Request.Form["CARGO_FIRMA"];
                string strElaboro = Request.Form["txt_elaboro"];

                strConsecutivo = strConsecutivo == "" ? "0" : strConsecutivo;
                ReportParameter parametro;
                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/Revistas.rdlc");
                Reporte.LocalReport.EnableExternalImages = true;

                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", strSitioGraba, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_entidad", strEntidad, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consecutivo", strConsecutivo, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));

                DataTable tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteRevistas", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));

                if (tabla.Rows.Count > 0)
                {
                    tabla.Columns.Add("FIRMA");
                    tabla.Columns.Add("CARGO_FIRMA");
                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("ELABORO");

                    tabla.Rows[0]["FIRMA"] = strFirma;
                    tabla.Rows[0]["CARGO_FIRMA"] = strCargoFirma;

                    tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                    tabla.Rows[0]["ELABORO"] = strElaboro;

                    string imagePath = new Uri(Server.MapPath(Resources.RecursoGeneral.RutaLogoReporte.ToString())).AbsoluteUri;
                    parametro = new ReportParameter("ImagePath", imagePath);
                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Revistas", tabla));
                    Reporte.LocalReport.SetParameters(parametro);
                    Reporte.LocalReport.Refresh();
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }

                Reporte.LocalReport.Refresh();

                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte del resumen de evaluación
        /// </summary>
        private void GenerarReporteResumenEvaluacion()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteUltimaEvaluacion.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_reporte_ultima_evaluacion", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ResumenUltimaEvaluacion", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "RESUMÉN DE EVALUACIÓN DEL SERVICIO POR UNIDAD Y PERSONAS ";
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de evaluación personal
        /// </summary>
        private void GenerarReporteEvaluacionPersonal()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                int iUnidad = int.Parse(Request.Form["Unidad"]);
                string strDetalle = Request.Form["Detalle"];
                string strCedula = Request.Form["Cedula"];
                string strVerTurno = Request.Form["VerTurno"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteCondPersonalPeriodos.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", iUnidad.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", strVerTurno.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_reporte_cond_personal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CondPersonalPeriodos", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("Detalle_Personas");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "FORMATO EVALUACION DE SERVICIO POR PERSONAS Y PERIODOS DE TIEMPO \r\n DEL "
                    + dFecha1 + " AL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Detalle_Personas"] = strDetalle;
                Reporte.LocalReport.Refresh();
                bool generaResumen = EnviaDatosTablaResumen(tabla);

                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    if (!generaResumen)
                    {
                        GestorBase.MSN_ALERTA_COMUN = "Resumén de Evaluación no fue creado";
                    }
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Envia datos de la tabla de resumen
        /// </summary>
        /// <param name="tabla"></param>
        /// <returns></returns>
        private bool EnviaDatosTablaResumen(DataTable tabla)
        {
            try
            {
                string strDescNiv6, strNivel6, strNombreEmpl, strDescCondPers, strUsuario, strTitulo;
                int iSitio = 0, iCsCntPers = 0, iCsPondCondPers = 0;
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    strDescNiv6 = tabla.Rows[i]["DESC_NIV6"].ToString();
                    strNivel6 = tabla.Rows[i]["NIVEL_6"].ToString();
                    strNombreEmpl = tabla.Rows[i]["NOMBRE_EMPL"].ToString();
                    strDescCondPers = string.IsNullOrEmpty(tabla.Rows[i]["DESC_COND_PERS"].ToString()) ? " " : tabla.Rows[i]["DESC_COND_PERS"].ToString();
                    strUsuario = tabla.Rows[i]["Nombre_Usuario"].ToString();
                    strTitulo = tabla.Rows[0]["Titulo"].ToString();
                    iSitio = int.Parse(tabla.Rows[i]["SITIO_GRABA"].ToString());
                    iCsCntPers = string.IsNullOrEmpty(tabla.Rows[i]["CANTIDAD_PERS"].ToString()) ? 0 : int.Parse(tabla.Rows[i]["CANTIDAD_PERS"].ToString());
                    iCsPondCondPers = string.IsNullOrEmpty(tabla.Rows[i]["POND_PERS"].ToString()) ? 0 : int.Parse(tabla.Rows[i]["POND_PERS"].ToString());

                    gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", iSitio.ToString(), typeof(int)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_desc_niv6", strDescNiv6, typeof(string)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nivel_6", strNivel6, typeof(string)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nombre_empl", strNombreEmpl, typeof(string)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_desc_cond_pers", strDescCondPers, typeof(string)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cs_cnt_pers", iCsCntPers.ToString(), typeof(int)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cs_pond_cond_pers", iCsPondCondPers.ToString(), typeof(int)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", strUsuario, typeof(string)));
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_titulo", strTitulo, typeof(string)));
                    gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_envia_res_ev_personal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "bool"));
                }
                return true;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return false;
            }
        }

        /// <summary>
        /// Genera el reporte de evaluación de unidad
        /// </summary>
        private void GenerarReporteEvaluacionUnidad()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                int iUnidad = int.Parse(Request.Form["Unidad"]);
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteCondUnidadPeriodos.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", iUnidad.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));

                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporte_unidad_periodos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CondUnidadPeriodos", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "FORMATO EVALUACION DE SERVICIO POR UNIDAD Y PERIODOS DE TIEMPO \r\n DEL "
                    + dFecha1 + " AL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de eventos por entidad y canal
        /// </summary>
        private void GenerarReporteEventosEntidadCanal()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                int iSitio = int.Parse(Request.Form["Sitio"]);
                int iEntidadFuerza = int.Parse(Request.Form["EntidadFuerza"]);
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteEventosEntidadCanal.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", iSitio.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", iEntidadFuerza.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteEventosEntidadCanal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("EventosEntidadCanal", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "Resumen de casos conocidos por Entidades y Canales \r\n Período del "
                    + dFecha1 + " Al " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de cantidad de casos por patrulla
        /// </summary>
        private void GenerarReporteCantidadCasosPatrulla()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                int iSitio = int.Parse(Request.Form["Sitio"]);
                int iUnidad = int.Parse(Request.Form["Unidad"]);
                int iIntegrantesMedios = int.Parse(Request.Form["IntegrantesMedios"]);
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteCantidadCasosPatrullas.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", iSitio.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_datos_integ", iIntegrantesMedios.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", iUnidad.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporte_casos_patrullas", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CantidadCasosPatrullas", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Rows[0]["Titulo"] = "Cantidad de casos conocidos por Unidades \r\n Período del "
                    + dFecha1 + " AL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de eventos por usuario
        /// </summary>
        private void GenerarReporteEventosUsuario()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                string strCalidad = Request.Form["Calidad"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteEventosUsuarioDiaHora.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteEventosPorUsuario", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PlanillaEventosUsuario", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("Cargo_Elaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "PLANILLA CANTIDAD DE EVENTOS GENERADOS POR DÍA, HORA POR USUARIO \r\n DESDE EL "
                    + dFecha1 + " HASTA EL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                tabla.Rows[0]["Cargo_Elaboro"] = strCargoElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por usuario que recibe
        /// </summary>
        private void GenerarReporteLlamadasUsuario()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                string strCalidad = Request.Form["Calidad"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteLlamadasUsuarioDiaHora.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                AdicionaLogo(Reporte);
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteLlamadasPorUsuario", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PlanillaLlamadasUsuario", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("Cargo_Elaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "PLANILLA CANTIDAD DE LLAMADAS RECIBIDAS POR DÍA Y HORA POR USUARIO \r\n DESDE EL "
                    + dFecha1 + " HASTA EL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                tabla.Rows[0]["Cargo_Elaboro"] = strCargoElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por número de teléfono
        /// </summary>
        private void GenerarReporteLlamadasTelefono()
        {
            try
            {
                string strTitulo = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                int iSitio = int.Parse(Request.Form["Sitio"]);
                int iCantidad = int.Parse(Request.Form["CantidadLimite"]);
                string strAbreviatura = Request.Form["Abreviatura"];
                string strResponsable = Request.Form["Firma"];
                string strCargoResponsable = Request.Form["CargoFirma"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteLlamadasPorTelefono.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", iSitio.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cantidad", iCantidad.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_abreviatura", strAbreviatura, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteLlamadasTelefono", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("LlamadasPorTelefono", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Cargo_Responsable");
                switch (strAbreviatura)
                {
                    case "01":
                        strTitulo = "RELACION DE TELEFONOS CON LLAMADAS EFECTIVAS" + "\r\n" + "DEL " + dFecha1
                                      + " HASTA EL " + dFecha2;
                        break;

                    case "02":
                        strTitulo = "RELACION DE TELEFONOS CON LLAMADAS NO EFECTIVAS" + "\r\n" + "DEL " + dFecha1
                     + " HASTA EL " + dFecha2;
                        break;

                    case "03":
                        strTitulo = "RELACION DE TELEFONOS CON LLAMADAS" + "\r\n" + "DEL " + dFecha1
                                                + " HASTA EL " + dFecha2;
                        break;

                    default:
                        break;
                }
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = strTitulo;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por día y hora
        /// </summary>
        private void GenerarReporteLlamadasDiaHora()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                string strCalidad = Request.Form["Calidad"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ReporteLlamadasDiaHora.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteLlamadasHoraDia", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("LlamadasDiaHora", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("Cargo_Elaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = "PLANILLA CANTIDAD DE LLAMADAS POR DIA Y HORA \r\n DESDE EL " + dFecha1 + " HASTA EL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                tabla.Rows[0]["Cargo_Elaboro"] = strCargoElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte del promedio de atención por días
        /// </summary>
        private void GenerarReportePromedioAtencionDias()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                int iCondicion = int.Parse(Request.Form["Detalle"].ToString());
                string strPTitulo = string.Empty;
                strPTitulo = "LLAMADAS ENVIADAS A DESPACHO PERIODO DEL " + dFecha1
                       + " HASTA EL " + dFecha2;
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/PromedioAtencionDias.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reportePromedioAtencionDia", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PromedioAtencionDias", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("Nombre_Usuario");
                tabla.Columns.Add("Fuerza_Usuario");
                tabla.Columns.Add("Condicion");
                tabla.Columns.Add("CuentaTotal");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = strPTitulo;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Condicion"] = iCondicion;
                tabla.Rows[0]["CuentaTotal"] = tabla.Rows.Count;

                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por código y barrio
        /// </summary>
        private void GenerarReporteBarriosYCodigo()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                string strTipoCodigo = Request.Form["PorTipoCodigo"].ToString();
                int iNivelInformacion = int.Parse(Request.Form["NivelInformacion"].ToString());
                string strPTitulo = string.Empty;
                strPTitulo = "LLAMADAS POR CIUDAD Y CODIGO DE LLAMADA  DESDE EL " + dFecha1
                       + " HASTA EL " + dFecha2;
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/LlamadasPorBarrioYCodigo.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo_codigo", strTipoCodigo.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteBarriosYCodigo", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("LlamadasBarriosYCodigo", tabla));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("Nombre_Usuario");
                tabla.Columns.Add("Fuerza_Usuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("Tipo_Promedio");
                tabla.Columns.Add("Nivel_Informacion");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = strPTitulo;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Nivel_Informacion"] = iNivelInformacion;

                var codisPedido = (from t in tabla.AsEnumerable() select new { idPedidorow = t.Field<String>("CODI_PEDIDO") }).Distinct();
                foreach (var item in codisPedido)
                {
                    tabla.Columns.Add(item.ToString());
                    tabla.Rows[0][item.ToString()] = item.ToString();
                }

                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de planilla de control de tiempos
        /// </summary>
        private void GenerarReportePlanillaControlTiempos()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ReportParameter parametro = new ReportParameter();
                string strSitio = Request.Form["Sitio"];
                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                decimal dUnidad = decimal.Parse(Request.Form["Unidad"]);
                string strResponsable = Request.Form["Responsable"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                int iTipoPromedio = int.Parse(Request.Form["TipoPromedio"].ToString());
                int iCondicionRegistro = int.Parse(Request.Form["CondicionRegistro"].ToString());
                int iIntegrantesMedios = int.Parse(Request.Form["IntegrantesMedios"].ToString());
                int iNivelInformacion = int.Parse(Request.Form["NivelInformacion"].ToString());
                string strPTitulo = string.Empty;
                int iPTiempoMin = 0, iPTiempoMax = 0, iPTiempo1 = 0, iPTiempo2 = 0, iPTiempo3 = 0, iPTiempo4 = 0, iPTiempo1F = 0, iPTiempo2F = 0, iPTiempo3F = 0, iPTiempo4F = 0;
                iPTiempoMin = int.Parse(Request.Form["txtMinutosMinimo"].ToString());
                iPTiempoMax = int.Parse(Request.Form["txtMinutosMaximo"].ToString());

                switch (iTipoPromedio)
                {
                    case 1:
                        strPTitulo = "Tiempo de llamada contra tiempo de llegada";
                        iPTiempo1 = iPTiempoMin * 60;
                        iPTiempo1F = iPTiempoMax * 60;
                        break;

                    case 2:
                        strPTitulo = "Tiempo de Asignación contra tiempo de llegada";
                        iPTiempo2 = iPTiempoMin * 60;
                        iPTiempo2F = iPTiempoMax * 60;
                        break;

                    case 3:
                        strPTitulo = "Tiempo de llamada contra tiempo de termino";
                        iPTiempo3 = iPTiempoMin * 60;
                        iPTiempo3F = iPTiempoMax * 60;
                        break;

                    case 4:
                        strPTitulo = "Tiempo de asignación contra tiempo de termino";
                        iPTiempo4 = iPTiempoMin * 60;
                        iPTiempo4F = iPTiempoMax * 60;
                        break;

                    default:
                        break;
                }
                switch (iCondicionRegistro)
                {
                    case 1:
                        strPTitulo += "\r\n Período del: " + dFecha1
                            + " AL " + dFecha2;
                        break;

                    case 2:
                        strPTitulo += "\r\n Tiempo entre: " + iPTiempoMin + " Minutos \r\n Período del: "
                            + dFecha1
                            + " AL " + dFecha2;
                        break;

                    case 3:
                        strPTitulo += "\r\n Tiempo mayor a: " + iPTiempoMin + " Minutos \r\n Período del: "
                            + dFecha1
                            + " AL " + dFecha2;
                        break;

                    case 4:
                        strPTitulo += "\r\n Tiempo menor o igual a: " + iPTiempoMin + " Minutos \r\n Período del: "
                            + dFecha1
                            + " AL " + dFecha2;
                        break;

                    default:
                        break;
                }

                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/PlanillaControlTiempos.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_datos_integ", iIntegrantesMedios.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo_promedio", iTipoPromedio.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_condicion_reg", iCondicionRegistro.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo1", iPTiempo1.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo2", iPTiempo2.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo3", iPTiempo3.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo4", iPTiempo4.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo1f", iPTiempo1F.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo2f", iPTiempo2F.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo3f", iPTiempo3F.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tiempo4f", iPTiempo4F.ToString(), typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", dUnidad.ToString(), typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_reporteControlTiempos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PlanillaControlTiempos", tabla));
                Reporte.LocalReport.SetParameters(new ReportParameter("TipoPromedio", iTipoPromedio.ToString()));
                Reporte.LocalReport.SetParameters(new ReportParameter("NivelInformacion", iNivelInformacion.ToString()));
                tabla.Columns.Add("Titulo");
                tabla.Columns.Add("Nombre_Usuario");
                tabla.Columns.Add("Fuerza_Usuario");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("Tipo_Promedio");
                tabla.Columns.Add("Nivel_Informacion");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Titulo"] = strPTitulo;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                tabla.Rows[0]["Tipo_Promedio"] = iTipoPromedio;
                tabla.Rows[0]["Nivel_Informacion"] = iNivelInformacion;

                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de antecedentes por usuario
        /// </summary>
        private void GenerarReporteAntecedentesPorUsuario()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                //string strRangoFechas = Request.Form["RangoFecha"];

                string txtFechaInicial = Request.Form["txtFechaInicial"];
                string txtFechaFinal = Request.Form["txtFechaFinal"];

                string strPatrulla = Request.Form["PATRULLAS"];
                string strFirma = Request.Form["FIRMA"];
                string strCargoFirma = Request.Form["txt_cargoFirma"];
                string strUsuarioConsulta = Request.Form["txt_usuario"];
                string strTipoReporte = Request.Form["selTipoReporte"];

                txtFechaInicial = txtFechaInicial == "" ? DateTime.Now.ToString() : txtFechaInicial;
                txtFechaFinal = txtFechaFinal == "" ? DateTime.Now.ToString() : txtFechaFinal;

                var valoresPatrulla = strPatrulla.Split('-');
                int intSalida = 0;
                string strFuerzaPatrulla = int.TryParse(valoresPatrulla[0], out intSalida) ? valoresPatrulla[0] : "0";
                string strUndeCodigoPatrulla = valoresPatrulla.Length > 1 ? int.TryParse(valoresPatrulla[1], out intSalida) ? valoresPatrulla[1] : "0" : "0";
                string strCodPatrulla = valoresPatrulla.Length > 2 ? int.TryParse(valoresPatrulla[2], out intSalida) ? valoresPatrulla[2] : "0" : "0";

                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/AntecedentesPorUsuario.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                Reporte.LocalReport.EnableExternalImages = true;

                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fechaInicial", txtFechaInicial, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fechaFinal", txtFechaFinal, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerzaPatrulla", strFuerzaPatrulla, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_undeCodigoPatrulla", strUndeCodigoPatrulla, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codPatrulla", strCodPatrulla, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuarioConsulta", strUsuarioConsulta, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_esPersona", strTipoReporte, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));

                DataTable tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteAntecedentes", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));

                if (tabla.Rows.Count > 0)
                {
                    tabla.Columns.Add("FIRMA");
                    tabla.Columns.Add("CARGO_FIRMA");
                    tabla.Columns.Add("FECHA_INICIAL");
                    tabla.Columns.Add("FECHA_FINAL");
                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("TIPO_REPORTE");
                    tabla.Columns.Add("FECHADB");
                    tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                    tabla.Rows[0]["FIRMA"] = strFirma;
                    tabla.Rows[0]["CARGO_FIRMA"] = strCargoFirma;
                    tabla.Rows[0]["FECHA_INICIAL"] = txtFechaInicial;
                    tabla.Rows[0]["FECHA_FINAL"] = txtFechaFinal;
                    tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                    tabla.Rows[0]["TIPO_REPORTE"] = strTipoReporte;

                    ReportParameter parametro;
                    string imagePath = new Uri(Server.MapPath(Resources.RecursoGeneral.RutaLogoReporte.ToString())).AbsoluteUri;
                    parametro = new ReportParameter("ImagePath", imagePath);
                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("AntecedentesPorUsuario", tabla));
                    Reporte.LocalReport.SetParameters(parametro);
                    Reporte.LocalReport.Refresh();
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }

                Reporte.LocalReport.Refresh();

                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de seguimiento casos
        /// </summary>
        private void GenerarReportesSeguimientoCasos()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string txtFechaInicial = Request.Form["txtFechaInicial"];
                string txtFechaFinal = Request.Form["txtFechaFinal"];

                string strSitioGraba = Request.Form["SITIO_GRABA"];
                string strFirma = Request.Form["FIRMA"];
                string strCargoFirma = Request.Form["txt_cargoFirma"];
                string strNroLlamada = Request.Form["nroLlamada"];
                string strTipoBusqueda = Request.Form["tipoBusqueda"];
                string strCodigoCaso = Request.Form["CODIGO_CASO"] == "" ? "0" : Request.Form["CODIGO_CASO"];
                string strCalidad = Request.Form["CALIDAD"];
                string strTipoPedido = Request.Form["TIPO_PEDIDO"];
                string strElaboro = Request.Form["txt_elaboro"];
                string strFuerza = usr.CADCANA_FUERZA_ID.ToString();
                int intEntidad = (int)usr.CADCANA_FUERZA_ID;
                string strEntidad = "";
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strEntidad = (from t in db.CADFUERZAS where t.ID == intEntidad select t.DESCRIPCION).FirstOrDefault();
                txtFechaInicial = txtFechaInicial == "" ? DateTime.Now.ToString() : txtFechaInicial;
                txtFechaFinal = txtFechaFinal == "" ? DateTime.Now.ToString() : txtFechaFinal;

                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/SeguimientoCasos.rdlc");
                Reporte.LocalReport.EnableExternalImages = true;
                ReportParameter parametro;

                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", strSitioGraba, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nroLlamada", strNroLlamada, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fechaInicial", txtFechaInicial, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fechaFinal", txtFechaFinal, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoBusqueda", strTipoBusqueda, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigoCaso", strCodigoCaso, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoPedido", strTipoPedido, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerza, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));

                DataTable tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteSeguimientoCasos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));

                if (tabla.Rows.Count > 0)
                {
                    tabla.Columns.Add("FIRMA");
                    tabla.Columns.Add("CARGO_FIRMA");
                    tabla.Columns.Add("FOTO_CONVERTIDA", typeof(byte[]));

                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("ELABORO");
                    tabla.Columns.Add("FECHADB");
                    tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();

                    tabla.Rows[0]["FIRMA"] = strFirma;
                    tabla.Rows[0]["CARGO_FIRMA"] = strCargoFirma;

                    tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                    tabla.Rows[0]["ELABORO"] = strElaboro;

                    int intContador = 0;

                    foreach (DataRow item in tabla.Rows)
                    {
                        intContador += 1;
                        if (item["FOTO"].ToString() != "")
                        {
                            Int64 intNumeEvento = Convert.ToInt64(item["FT_NUME_EVENTO"].ToString());
                            int intConsecutivo = Convert.ToInt32(item["FT_CONSEC"].ToString());
                            byte[] data = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == intNumeEvento && t.CONSECUTIVO == intConsecutivo select t.FOTO).FirstOrDefault();

                            tabla.Rows[intContador - 1]["FOTO_CONVERTIDA"] = data;
                        }
                    }

                    string imagePath = new Uri(Server.MapPath(Resources.RecursoGeneral.RutaLogoReporte.ToString())).AbsoluteUri;
                    parametro = new ReportParameter("ImagePath", imagePath);
                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("SeguimientoCasos", tabla));
                    Reporte.LocalReport.SetParameters(parametro);
                    Reporte.LocalReport.SetParameters(new ReportParameter("ENTIDAD", strEntidad));
                    Reporte.LocalReport.Refresh();
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }

                Reporte.LocalReport.Refresh();

                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas sitios de grabacion
        /// </summary>
        private void GenerarReportesResumenLllamdasSitioGraba()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string txtFechaInicial = Request.Form["txtFechaInicial"];
                string txtFechaFinal = Request.Form["txtFechaFinal"];
                string sitioGraba = Request.Form["SITIO_GRABA"];
                string firma = Request.Form["FIRMA"];
                string cargoFirma = Request.Form["txt_cargoFirma"];

                txtFechaInicial = txtFechaInicial == "" ? DateTime.Now.ToString() : txtFechaInicial;
                txtFechaFinal = txtFechaFinal == "" ? DateTime.Now.ToString() : txtFechaFinal;
                ReportParameter parametro;
                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ResumenLlamadasSitioGraba.rdlc");
                Reporte.LocalReport.EnableExternalImages = true;
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", sitioGraba, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fechaInicial", txtFechaInicial, typeof(DateTime)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fechaFinal", txtFechaFinal, typeof(DateTime)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteResumenLlamadasSitio", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));

                if (tabla.Rows.Count > 0)
                {
                    tabla.Columns.Add("FIRMA");
                    tabla.Columns.Add("CARGO_FIRMA");
                    tabla.Columns.Add("FECHA_INICIAL");
                    tabla.Columns.Add("FECHA_FINAL");
                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("FECHADB");
                    tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                    tabla.Rows[0]["FIRMA"] = firma;
                    tabla.Rows[0]["CARGO_FIRMA"] = cargoFirma;
                    tabla.Rows[0]["FECHA_INICIAL"] = txtFechaInicial;
                    tabla.Rows[0]["FECHA_FINAL"] = txtFechaFinal;
                    tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;

                    string imagePath = new Uri(Server.MapPath(Resources.RecursoGeneral.RutaLogoReporte.ToString())).AbsoluteUri;
                    parametro = new ReportParameter("ImagePath", imagePath);
                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ResumenSitioGraba", tabla));

                    Reporte.LocalReport.SetParameters(parametro);
                    Reporte.LocalReport.Refresh();
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }

                Reporte.LocalReport.Refresh();

                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera el reporte de casos por unidad por tiempo
        /// </summary>
        private void GenerarReporteCasosUnidadTiempo()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                string strSitio = Request.Form["Sitio"];
                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                decimal dUnidad = decimal.Parse(Request.Form["Unidad"]);
                string strResponsable = Request.Form["Firma"];
                string strCargoResponsable = Request.Form["CargoFirma"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/CasosUnidadTiempos.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", strSitio, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", dUnidad.ToString(), typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporte_casos_unidad_tiempos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                if (tabla.Rows.Count > 0)
                {
                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CasosUnidadTiempos", tabla));

                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("Fecha_Inicio");
                    tabla.Columns.Add("Fecha_Final");
                    tabla.Columns.Add("Fuerza_Usuario");
                    tabla.Columns.Add("Responsable");
                    tabla.Columns.Add("Elaboro");
                    tabla.Columns.Add("Cargo_Responsable");
                    tabla.Columns.Add("Cargo_Elaboro");
                    tabla.Columns.Add("FECHADB");
                    tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                    tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                    tabla.Rows[0]["Fecha_Inicio"] = dFecha1;
                    tabla.Rows[0]["Fecha_Final"] = dFecha2;
                    tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                    tabla.Rows[0]["Responsable"] = strResponsable;
                    tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera Reporte de Resumén Planilla
        /// </summary>
        private void GenerarReporteResumenPlanilla()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var selectedTurno = int.Parse(Request.Form["Turno"].ToString());
                var turno = ObtenerTurnos().Where(t => t.TURNO == selectedTurno && t.SITIO_GRABA == usr.SITIO_GRABACION).FirstOrDefault();
                DateTime dFecha1 = turno.HORA_INICIA;
                DateTime dFecha2 = turno.HORA_TERMINA;
                string strCalidad = Request.Form["Calidad"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                gestor = new GestorGeneral();
                GestorGeneral gestor1 = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ResumenPlanilla.rdlc");
                gestor1.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor1.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1.ToString("dd/MM/yyyy HH:mm:ss"), typeof(string)));
                gestor1.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2.ToString("dd/MM/yyyy HH:mm:ss"), typeof(string)));
                gestor1.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor1.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor1.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteResumenPlanillaQ1", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ResumenPlanillaQ1", tabla));
                tabla2 = gestor1.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteResumenPlanillaQ3Q2", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                if (tabla.Rows.Count > 0)
                {
                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ResumenPlanillaQ3Q2", tabla2));


                    tabla.Columns.Add("TURNO");
                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("FUERZA_USUARIO");
                    tabla.Columns.Add("Responsable");
                    tabla.Columns.Add("Elaboro");
                    tabla.Columns.Add("Cargo_Responsable");
                    tabla.Columns.Add("Cargo_Elaboro");
                    tabla.Columns.Add("FECHADB");
                    tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                    tabla.Rows[0]["Turno"] = turno.DESCRIPCION;
                    tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                    tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                    tabla.Rows[0]["Responsable"] = strResponsable;
                    tabla.Rows[0]["Elaboro"] = strElaboro;
                    tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                    tabla.Rows[0]["Cargo_Elaboro"] = strCargoElaboro;
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera Reporte de Planilla de Turnos
        /// </summary>
        private void GenerarReportePlanillaTurno()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var selectedTurno = int.Parse(Request.Form["Turno"].ToString());
                var turno = ObtenerTurnos().Where(t => t.TURNO == selectedTurno && t.SITIO_GRABA == usr.SITIO_GRABACION).FirstOrDefault();

                DateTime dFecha1 = turno.HORA_INICIA;
                DateTime dFecha2 = turno.HORA_TERMINA;
                string strMarcar = Request.Form["txtMarcarMayorQue"];
                string strCalidad = Request.Form["Calidad"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/PlanillaTurno.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1.ToString("dd/MM/yyyy HH:mm:ss"), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2.ToString("dd/MM/yyyy HH:mm:ss"), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_marcar", strMarcar, typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reportePlanillaTurnos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                if (tabla.Rows.Count > 0)
                {

                    Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PlanillaTurno", tabla));
                    tabla.Columns.Add("TURNO");
                    tabla.Columns.Add("NOMBRE_USUARIO");
                    tabla.Columns.Add("FUERZA_USUARIO");
                    tabla.Columns.Add("Responsable");
                    tabla.Columns.Add("Elaboro");
                    tabla.Columns.Add("CargoResponsable");
                    tabla.Columns.Add("CargoElaboro");
                    tabla.Columns.Add("FECHADB");
                    tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                    tabla.Rows[0]["TURNO"] = turno.DESCRIPCION;
                    tabla.Rows[0]["NOMBRE_USUARIO"] = usr.USUARIO;
                    tabla.Rows[0]["FUERZA_USUARIO"] = strFuerza;
                    tabla.Rows[0]["Responsable"] = strResponsable;
                    tabla.Rows[0]["Elaboro"] = strElaboro;
                    tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                    tabla.Rows[0]["CargoElaboro"] = strCargoElaboro;
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS_REPORTE;
                }
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Genera Reporte de Conductas de Ponderación
        /// </summary>
        private void GenerarReporteConductasPonderacion()
        {
            try
            {
                string strUsr = string.Empty;
                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/PonderacionConductas.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    throw new Exception("Usuario fuera de sesión, posible intento de acceder por URL");
                }
                else
                {
                    strUsr = usr.USUARIO;
                    SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                    strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                }
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reportePonderacionConductas", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                AdicionaLogo(Reporte);
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PonderacionConductas", tabla));
                tabla.Columns.Add("Usuario");
                tabla.Columns.Add("Fuerza");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["Usuario"] = strUsr;
                tabla.Rows[0]["Fuerza"] = strFuerza;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                tabla = new DataTable();
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Medios", tabla));
                Reporte.LocalReport.Refresh();
                lbMensaje.Text = Resources.RecursoGeneral.MSN_NO_DATOS;
                panelContenedor.Visible = true;
            }
        }

        /// <summary>
        /// Permite generar el reporte de excepciones
        /// </summary>
        private void GenerarReporteExcepciones()
        {
            try
            {

                string strConsultaFecha;
                var valores = strRangoFecha.Split('-');

                strConsultaFecha = " WHERE ( TO_DATE(EX.EX_FECHA, 'dd/MM/YY') BETWEEN TO_DATE('" + valores[0] + "', 'dd/MM/YY') AND TO_DATE('" + valores[1] + "', 'dd/MM/YY') )";
                panelContenedor.Visible = false;
                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/Excepciones.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_rango_fecha", strConsultaFecha, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_id", strId, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_equipo_ip", strIp, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario_id", strUsuarioOid, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporte_excepciones", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Excepciones", tabla));
                Reporte.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite generar el reporte de Unidades
        /// </summary>
        private void GenerarReporteUnidades()
        {
            try
            {
                panelContenedor.Visible = false;
                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/Unidades.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("PFUERZA", strFuerza, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("PCODIGO", strCodigo, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("PNIVEL", strNivel, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generar_unidades", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Unidades", tabla));
                Reporte.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite generar el reporte de medios de atención asociados a unidades
        /// </summary>
        private void GenerarReporteMedios()
        {
            try
            {
                string strMensaje = "", strOpcion = "TODOS";
                switch (intOpcion)
                {
                    case 1:
                        strOpcion = "INDICATIVOS";
                        break;

                    case 2:
                        strOpcion = "ARMAS";
                        break;

                    case 3:
                        strOpcion = "VEHICULOS";
                        break;
                }
                gestor = new GestorGeneral();
                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/Medios.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerza, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", strCodigo, typeof(decimal)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_operativa", strOperativa, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_vigente", strVigente, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_opcion", strOpcion, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporte_medios_atencion", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Medios", tabla));
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                tabla = new DataTable();
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Medios", tabla));
                Reporte.LocalReport.Refresh();
                lbMensaje.Text = Resources.RecursoGeneral.MSN_NO_DATOS;
                panelContenedor.Visible = true;
            }
        }

        /// <summary>
        /// Busca turnos en la base de datos para usarlos en un dropdownlist
        /// </summary>
        /// <returns></returns>
        private List<dynamic> ObtenerTurnos()
        {
            CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_FUERZA", usr.CADCANA_FUERZA_ID.ToString(), typeof(string)));
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttTurnos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_cargaTurnos", strConexion, "Nonquery");
            var arrTurnos = (from t in dttTurnos.AsEnumerable() select new { TURNO = t["TURNO"], DESCRIPCION = t["DESCRIPCION"], HORA_INICIA = t["HORA_INICIA"], HORA_TERMINA = t["HORA_TERMINA"], SITIO_GRABA = t["SITIO_GRABA"] });
            return arrTurnos.ToList<dynamic>();
        }

        /// <summary>
        /// Genera Reporte de Planilla Llamadas por períodos de tiempo
        /// </summary>
        private void GenerarReportePlanillaPeriodos()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                string strCalidad = Request.Form["Calidad"];
                string strMarcar = Request.Form["txtMarcarMayorQue"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/PlanillaLlamadasPeriodo.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_marcar", strMarcar, typeof(int)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                AdicionaLogo(Reporte);

                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_reportePlanillaTurnos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));

                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("PlanillaTurno", tabla));
                tabla.Columns.Add("TURNO");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("CargoResponsable");
                tabla.Columns.Add("CargoElaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["TURNO"] = "PLANILLA DE LLAMADAS PERIODO DE TIEMPO \r\n DESDE EL " + dFecha1 + " HASTA EL " + dFecha2;
                tabla.Rows[0]["NOMBRE_USUARIO"] = usr.USUARIO;
                tabla.Rows[0]["FUERZA_USUARIO"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                tabla.Rows[0]["CargoResponsable"] = strCargoResponsable;
                tabla.Rows[0]["CargoElaboro"] = strCargoElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Adiciona el logotipo de SecadPlus a los reportes
        /// </summary>
        /// <param name="reporte"></param>
        private void AdicionaLogo(ReportViewer reporte)
        {
            ReportParameter[] reportParameter = new ReportParameter[1];
            reportParameter[0] = new ReportParameter("LogoSECADPlus", new Uri(Server.MapPath(Resources.RecursoGeneral.RutaLogoReporte.ToString())).AbsoluteUri);
            Reporte.LocalReport.EnableExternalImages = true;
            Reporte.LocalReport.SetParameters(reportParameter);
        }

        /// <summary>
        /// Genera Reporte de Planilla Resumén Llamadas por períodos de tiempo
        /// </summary>
        private void GenerarReporteResumenLlamadasPeriodos()
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string dFecha1 = Request.Form["FechaInicio"];
                string dFecha2 = Request.Form["FechaFinal"];
                string strCalidad = Request.Form["Calidad"];
                string strResponsable = Request.Form["Responsable"];
                string strElaboro = Request.Form["Elaboro"];
                string strCargoResponsable = Request.Form["CargoResponsable"];
                string strCargoElaboro = Request.Form["CargoElaboro"];
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                strFuerza = db.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;

                gestor = new GestorGeneral();

                Reporte.Reset();
                Reporte.LocalReport.ReportPath = Server.MapPath("~/Reportes/ResumenPlanillaLlamadasPeriodo.rdlc");
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", dFecha1, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", dFecha2, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", strCalidad, typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteResumenLlamadas", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                tabla2 = gestor.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_reporteResumenPlanillaQ1", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString, "nonquery"));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ResumenPlanillaQ1", tabla2));
                Reporte.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("ResumenLlamadasPeriodo", tabla));
                AdicionaLogo(Reporte);
                tabla.Columns.Add("TURNO");
                tabla.Columns.Add("NOMBRE_USUARIO");
                tabla.Columns.Add("FUERZA_USUARIO");
                tabla.Columns.Add("Responsable");
                tabla.Columns.Add("Elaboro");
                tabla.Columns.Add("Cargo_Responsable");
                tabla.Columns.Add("Cargo_Elaboro");
                tabla.Columns.Add("FECHADB");
                tabla.Rows[0]["FECHADB"] = ObtenerFechaDB();
                tabla.Rows[0]["TURNO"] = "RESUMÉN PLANILLA DE LLAMADAS PERIODO \r\n DESDE EL " + dFecha1 + " HASTA EL " + dFecha2;
                tabla.Rows[0]["Nombre_Usuario"] = usr.USUARIO;
                tabla.Rows[0]["Fuerza_Usuario"] = strFuerza;
                tabla.Rows[0]["Responsable"] = strResponsable;
                tabla.Rows[0]["Elaboro"] = strElaboro;
                tabla.Rows[0]["Cargo_Responsable"] = strCargoResponsable;
                tabla.Rows[0]["Cargo_Elaboro"] = strCargoElaboro;
                Reporte.LocalReport.Refresh();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    var strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    lbMensaje.Text = strMensaje;
                    panelContenedor.Visible = true;
                }
                else
                {
                    this.panelContenedor.Visible = false;
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Convierte objeto a datatable
        /// </summary>
        /// <param name="o">Objeto a convertir</param>
        /// <returns></returns>
        public static DataTable ObjectToDataTable(object o)
        {
            DataTable dt = new DataTable("OutputData");

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);

            o.GetType().GetProperties().ToList().ForEach(f =>
            {
                try
                {
                    f.GetValue(o, null);
                    dt.Columns.Add(f.Name, f.PropertyType);
                    dt.Rows[0][f.Name] = f.GetValue(o, null);
                }
                catch { }
            });
            return dt;
        }
    }
}