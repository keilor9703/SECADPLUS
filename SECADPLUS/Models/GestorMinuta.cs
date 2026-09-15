using SECADEntities.SecadEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECADPLUS.Models
{
    public class GestorMinuta : GestorBase
    {
        #region métodos

        /// <summary>
        /// Permite consultar el consecutivo para la creación de un registro de turno
        /// </summary>
        /// <returns></returns>
        public long ObtenerTurnoConsecutivo()
        {
            try
            {
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                return gestorGeneral.EjecutarFuncion<long>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_obtener_turno_consecutivo", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public long ObtenerMedioDisponibleConsecutivo()
        {
            try
            {
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                return gestorGeneral.EjecutarFuncion<long>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_mediosdisp_consecutivo", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public CADUNITURNO ConsultarUniTurno(string[] strParametros)
        {
            try
            {
                int intSitio = 0, intFuerza = 0, intCodigo = 0;
                long lgTurno = 0;
                CADUNITURNO uniTurno;
                intSitio = Convert.ToInt32(strParametros[0]);
                lgTurno = Convert.ToInt64(strParametros[1]);
                intFuerza = Convert.ToInt32(strParametros[2]);
                intCodigo = Convert.ToInt32(strParametros[3]);
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    uniTurno = (from registro in contexto.CADUNITURNOS
                                where registro.CADTURN_SITIOGRABA == intSitio
                                && registro.CADTURN_TURNO == lgTurno
                                && registro.CADUNID_FUERZA == intFuerza
                                && registro.CADUNIP_CODIGO == intCodigo
                                select registro).FirstOrDefault();
                }
                return uniTurno;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite consultar un turno especifíco
        /// </summary>
        /// <param name="intSitio"></param>
        /// <param name="lgTurno"></param>
        /// <returns></returns>
        public CADTURNO ConsultarTurno(int intSitio, long lgTurno)
        {
            try
            {
                CADTURNO turno;
                using (DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                {
                    turno = (from registro in contexto.CADTURNOS
                             where registro.TURNO == lgTurno
                             && registro.SITIO_GRABA == intSitio
                             select registro).FirstOrDefault();
                }
                return turno;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        #endregion métodos
    }
}