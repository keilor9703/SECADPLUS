using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace SECADPLUS.Models
{
    public class Validador : GestorBase
    {
        #region Atributos

        private string strNumeros = "";

        public string Numeros
        {
            get { return strNumeros; }
            set { strNumeros = value; }
        }

        #endregion Atributos

        #region métodos

        //Constructor
        public Validador()
        {
            gestorGeneral = new GestorGeneral();
        }

        private decimal dcTurnoU = 0, dcTurnoA = 0, dcTurnoR = 0, dcTurnoTemp;
        private decimal dcNumeroTurnos = 0;

        public void Ejecutar()
        {
        }

        public bool ValidarUsuarioMinuta(CADUSUARIO usuario)
        {
            try
            {
                if (usuario.CONDICION.Equals("A") || usuario.CONDICION.Equals("J") || usuario.USUARIO.Equals("USR_CAD"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite verificar que el turno cumpla las codiciones necesarias para asociarle unidades
        /// </summary>
        /// <param name="turno"></param>
        /// <returns></returns>
        public bool ValidarTurno(CADTURNO turno)
        {
            try
            {
                bool blTurnoU = false, blTurnoA = false, blTurnoR = false;
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", turno.CADFUERZ_ID.ToString(), typeof(decimal)));
                dcNumeroTurnos = gestorGeneral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_cant_clase_turnos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                //Consulta la cantidad de turnos asociados
                //Válida el turno urbano
                blTurnoU = ValidarClaseTurno(turno, "01");
                //Válida el turno rural
                blTurnoR = ValidarClaseTurno(turno, "02");
                //Válida turno de apoyo
                blTurnoA = ValidarClaseTurno(turno, "03");

                if (dcNumeroTurnos > 0)
                {
                    return false;
                }

                if (blTurnoA)
                {
                    return true;
                }
                else
                {
                    if (dcNumeroTurnos <= 0)
                    {
                        strMensaje = Resources.RecursoGeneral.MSN_TURNO_APOYO_NO_EXISTE + " " + turno.HORA_INICIA.ToShortDateString();
                    }
                    return false;
                }
            }
            catch (OracleException oex)
            {
                throw GestionarExcepcion(oex, GetType().Name);
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        private bool ValidarClaseTurno(CADTURNO turno, string strClase)
        {
            try
            {
                string strFuncion = ".PK_CAD.f_turno";
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Clear();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", turno.HORA_INICIA.ToString(), typeof(DateTime)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", turno.CADFUERZ_ID.ToString(), typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strClase, typeof(string)));
                switch (strClase)
                {
                    case "03":
                        strFuncion = ".PK_CAD.F_TURNO_ACCESORIO";
                        break;
                }
                dcTurnoTemp = gestorGeneral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + strFuncion, Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                switch (strClase)
                {
                    case "01":
                        dcTurnoU = dcTurnoTemp;
                        dcNumeroTurnos--;
                        break;

                    case "02":
                        dcTurnoR = dcTurnoTemp;
                        dcNumeroTurnos--;
                        break;

                    default:
                        dcTurnoA = dcTurnoTemp;
                        break;
                }

                return true;
            }
            catch (OracleException oex)
            {
                if (oex.Number == 1403)
                {
                    switch (strClase)
                    {
                        case "01":
                            strMensaje += "No esta creado el Turno: Rural (" + strClase + ") para la fecha " + turno.HORA_INICIA.ToShortDateString();
                            break;

                        case "02":
                            strMensaje += "No esta creado el Turno: Urbano (" + strClase + ") para la fecha " + turno.HORA_INICIA.ToShortDateString();
                            break;

                        default:
                            strMensaje += "Debe Crear primero el turno de Apoyo del dia.. " + turno.HORA_INICIA.ToShortDateString();
                            break;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool ValidarEquipoPersona(string strCedula, long lgMedio)
        {
            try
            {
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    CADARMDISPONIBLE arma = (from registro in contexto.CADARMDISPONIBLES
                                             where registro.CADPOLI_CEDU_EMPLEADO == strCedula
                                             && registro.CADMEDISP_CONSECUTIVO == lgMedio
                                             select registro).FirstOrDefault();
                    CADVEHIDISPONIBLE vehiculo = (from registro in contexto.CADVEHIDISPONIBLES
                                                  where registro.POLI_CEDU_EMPLEADO == strCedula
                                                  && registro.CADMEDISP_CONSECUTIVO == lgMedio
                                                  select registro).FirstOrDefault();
                    CADRADIODISPONIBLE radio = (from registro in contexto.CADRADIODISPONIBLES
                                                where registro.CADPOLI_CEDU_EMPLEADO == strCedula
                                                && registro.CADMEDISP_CONSECUTIVO == lgMedio
                                                select registro).FirstOrDefault();
                    if (arma != null || vehiculo != null || radio != null)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool ValidaHoraTurno(CADTURNO turno)
        {
            try
            {
                if (turno.HORA_TERMINA < ObtenerTiempoServidor)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public decimal VerificarTurnoAsociaciones(long lgTurno)
        {
            try
            {
                decimal dcResultado = 0;
                string strFuncion = ".PK_CADPLUS.f_verificarturno";
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Clear();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_turno", lgTurno.ToString(), typeof(decimal)));
                dcResultado = gestorGeneral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + strFuncion, Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                return dcResultado;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool VerificarEliminarUsuario(CADUSUARIO usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.USUARIO_OID_ID))
                {
                    return false;
                }
                CADNETOIDROLE relaciones;
                //Verifica si el usuario tiene asociado roles
                using (PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities")))
                {
                    relaciones = (from registro in db.CADNETOIDROLES
                                  where registro.UserId == usuario.USUARIO_OID_ID
                                  select registro).FirstOrDefault();
                    if (relaciones != null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }


        /// <summary>
        ///Permite verificar la existencia en sesión del usuario de secad
        /// </summary>
        /// <returns></returns>
        public CADUSUARIO VerificarUsuario()
        {
            try
            {
                if (HttpContext.Current.Session["USUARIO_SECAD"] == null)
                {
                    var valor = ConsultarUsuarioSecad();                    
                }
                return (CADUSUARIO)HttpContext.Current.Session["USUARIO_SECAD"];
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite conocer si una conducta asociada a un turno posee datos asociados
        /// </summary>
        /// <param name="conductaAsociada">Conducta asociada a consultar</param>
        /// <returns>Retrna True si no existen datos asociados</returns>
        public bool ValidarAsociadosConducta(CADCONDTURNO conductaAsociada)
        {
            try
            {
                decimal bienes = 0, personas = 0, comparendos = 0;
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    bienes = (from registro in contexto.CADCONDTURNOBIENES
                              where registro.CADCONDTURNCOND == conductaAsociada.CADCONDCODIGO
                              && registro.CADCONDTURNSITGR == conductaAsociada.CADTURNSITIOGRABA
                              && registro.CADCONDTURNTURNO == conductaAsociada.CADTURNTURNO
                              select new { registro.CADCONDTURNCOND }).Count();

                    personas = (from registro in contexto.CADCONDTURNOPERS
                                where registro.CADCONDTURNCOND == conductaAsociada.CADCONDCODIGO
                                && registro.CADCONDTURNSITGR == conductaAsociada.CADTURNSITIOGRABA
                                && registro.CADCONDTURNTURNO == conductaAsociada.CADTURNTURNO
                                select new { registro.CADCONDTURNCOND }).Count();
                    comparendos = (from registro in contexto.CADCONDTURNOCOMPs
                                   where registro.CADCONDTURNCOND == conductaAsociada.CADCONDCODIGO
                                   && registro.CADCONDTURNSITGR == conductaAsociada.CADTURNSITIOGRABA
                                   && registro.CADCONDTURNTURNO == conductaAsociada.CADTURNTURNO
                                   select new { registro.CADCONDTURNCOND }).Count();
                    if (bienes > 0 && personas > 0 && comparendos > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite verificar los registros asociados a una persona relacionadao con una conducta de
        /// un turno de servicio
        /// </summary>
        /// <param name="persona">Persona involucrada en una conducta</param>
        /// <returns>
        /// Retorna False cuando existen registros asociados y True cuando no existen registros asociados
        /// </returns>
        public bool ValidarAsociadosPersona(CADCONDTURNOPER persona)
        {
            try
            {
                decimal medios = 0;
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    medios = (from registro in contexto.CADCONDTURNOPERSMEDISPs
                              where registro.CADPERCONSEC == persona.CONSECUTIVO
                              && registro.CADPERSITGR == persona.SITIO_GRABA
                              select new { registro.CADPERSITGR }).Count();
                    if (medios > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool ValidarAsociadosBien(CADCONDTURNOBIENE bien)
        {
            try
            {
                decimal medios = 0;
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    medios = (from registro in contexto.CADCONDTURNOBIENMEDISPs
                              where registro.CADBIENCONSEC == bien.CONSECUTIVO
                              && registro.CADBIENSITGR == bien.SITIO_GRABA
                              select new { registro.CADBIENSITGR }).Count();
                    if (medios > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool ValidarAsociadosComparendo(CADCONDTURNOCOMP comparendo)
        {
            try
            {
                decimal medios = 0;
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    medios = (from registro in contexto.CADCONDTURNOCOMPMEDISPs
                              where registro.CADCOMPCONSEC == comparendo.CONSECUTIVO
                              && registro.CADCOMPSITGR == comparendo.SITIO_GRABA
                              select new { registro.CADCOMPCONSEC }).Count();
                    if (medios > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite verificar que existan a lo menos 6 numeraciones celulares
        /// </summary>
        /// <returns></returns>
        public bool ValidarNumeracionCelular()
        {
            try
            {
                intTemp = 0;
                using (SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                {
                    var numeros = contexto.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "NUMERACION_CELULAR").Select(c => new { c.CODIGO }).ToList();
                    intTemp = numeros.Count;
                    if (intTemp < 6)
                    {
                        return false;
                    }
                    foreach (var numero in numeros)
                    {
                        strNumeros += "," + numero;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool VerificarConexionesPedidos(string strConexion, int intSitio, string strValor)
        {
            try
            {
                using (SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                {
                    var registroConexion = contexto.CADREFERENCIAS.Where(r => r.NOMBRE == strConexion && r.SITIO_GRABA == intSitio && r.CODIGO == strValor).Select(c => new { c.ABREVIATURA }).FirstOrDefault();

                    if (registroConexion == null)
                    {
                        if (strConexion.Equals("CONEXION_DIJIN"))
                        {
                            GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_CONEXION_DIJIN;
                        }
                        else
                        {
                            GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_RECEPCION_CONEXION_TELEPOL;
                        }
                        return false;
                    }
                    strMensaje = (registroConexion.ABREVIATURA.Equals("00") ? "NO" : "SI");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool VerificarEquipoRegistrado(string strNomreEquipo, int intSitio)
        {
            try
            {
                using (SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                {
                    CADEQUIPOS equipo = contexto.CADEQUIPOS.Where(c => c.IDENTIFICACION == strNomreEquipo && c.SITIO_GRABA==intSitio).FirstOrDefault();
                    if (equipo == null)
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EQUIPO_NO_REGISTRADO;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string ObtenerAcd(int intSitio, string strEquipo)
        {
            try
            {
                string strAcd = "";
                using (SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                {
                    var acd = (from registro in contexto.CADEQUIPOS
                               where registro.SITIO_GRABA == intSitio
                               && registro.IDENTIFICACION == strEquipo
                               select new { registro.ACD }).FirstOrDefault();
                    if (acd != null)
                    {
                        strAcd = acd.ACD;
                    }
                }
                return strAcd;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string IdentificarPorDns(string strIp)
        {
            try
            {
                return Dns.GetHostEntry(strIp).HostName;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                GestorBase.EX_CODIGO = "";
                return "";
            }
        }
        /// <summary>
        /// Permite identificar el nombre de la máquina del usuario final
        /// </summary>
        /// <returns></returns>
        public bool IdentificarCliente(HttpRequestBase Request, HttpSessionStateBase Session)
        {
            string strNombre = "";
            try
            {
                HttpCookie cookie;
                /* IPHostEntry host = new IPHostEntry();*/
                string strIp = Request.ServerVariables["REMOTE_ADDR"].ToString();
                Console.WriteLine("ip de la maquina" + strIp);
                int intInicio = 0;
                if (!string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    return true;
                }
                //verifica si se debe identificar la máquina de usuario a tráves del DNS
                if (ConfigurationManager.AppSettings["ActivarValidacionDNS"].ToString().ToUpper().Equals("SI"))
                {
                    //host = Dns.GetHostEntry(Request.ServerVariables["REMOTE_HOST"]);
                    /*strNombre = IdentificarPorDns(Request.ServerVariables["REMOTE_ADDR"]);*/
                    /* strNombre = "DESKTOP-H47N1DL";  */

                    IPHostEntry host = Dns.GetHostEntry(strIp);
                    strNombre = host.HostName.Split('.')[0];
                }
                //Verifica si existe una cookie con el nombre de máquina
                if (string.IsNullOrEmpty(strNombre))
                {
                    cookie = Request.Cookies["NOMBRE_MAQUINA"];
                    if (cookie != null)
                    {
                        //Asigna el nombre de maquina de la cookie en la variable de sesión
                        strNombre = cookie.Value;
                    }
                }
                
              
                //Verifica en la base de datos por nombre de máquina
                if (string.IsNullOrEmpty(strNombre))
                {
                    using (PermisosEntities contextoSeguridad = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities")))
                    {

                        strNombre = (from registro in contextoSeguridad.CADNETMAQUINAS
                                     where registro.EQUIPO_IP == strIp
                                     orderby registro.FECHA_REGISTRO descending
                                     select registro.EQUIPO_NOMBRE
                                     ).FirstOrDefault();
                    }
                }
                //Remueve el dominio del nombre
                intInicio = strNombre.IndexOf(".");
                if (intInicio > 0)
                {
                    strNombre = strNombre.Substring(0, intInicio);
                }
                if (string.IsNullOrEmpty(strNombre))
                {
                    Session["MAQUINA"] = "";
                }
                else
                {
                    Session["MAQUINA"] = strNombre.ToUpper();
                }

                return true;
            }
            catch (Exception exControlada)
            {
                GestorBase.GestionarExcepcion(exControlada, GetType().Name);
                GestorBase.EX_CODIGO = "";
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " DESDE VALDIADOR (Nombre identificado:" + strNombre + ")";
                return false;                
            }
        }
        #endregion métodos
    }
}