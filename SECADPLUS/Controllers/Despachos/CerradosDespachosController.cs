using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SecadDatos.Areas.MinutaSivicc;

namespace SECADPLUS.Controllers
{
    public class CerradosDespachosController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: CerradosDespachos
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                /*RMR ADICION EN CERRADOS DESPACHO*/                
                var list = new DbMinuta().F_GetModalidad().ToList();
                list = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.ModalidadId = new SelectList(new DbMinuta().F_GetModalidad(), "ReferenciaId", "Descripcion");

                var listC = new DbMinuta().F_GetConductas().ToList();
                listC = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.ConductaId = new SelectList(new DbMinuta().F_GetConductas(), "ReferenciaId", "Descripcion");

                var listG = new DbMinuta().F_GetTipoArma().ToList();
                listG = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.TipoArmId = new SelectList(new DbMinuta().F_GetTipoArma(), "ReferenciaId", "Descripcion");

                var listT = new DbMinuta().F_GetTipoMoviliza().ToList();
                listT = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.TipoMovilizaId = new SelectList(new DbMinuta().F_GetTipoMoviliza(), "ReferenciaId", "Descripcion");                
                /*FIN RMR ADICION EN CERRADOS DESPACHO*/
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalIdentificacion = usr.IDENTIFICACION;
                ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;
                ViewBag.globalCanal = usr.CADCANA_CODIGO;
                ViewBag.globalCondicion = usr.CONDICION;
                ViewBag.globalUsuario = usr.USUARIO;

                var idFuerza = usr.CADCANA_FUERZA_ID;
                var idCanal = usr.CADCANA_CODIGO;

                //Revisar si es necesario canal asignado
                if (idCanal == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_CANAL_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }

                var maquina = db.CADCONEXIONs.Where(c => c.CADUSUA_USUARIO == usr.USUARIO && c.VIGENTE == "S").OrderByDescending(c => c.SESION_ID).Select(c => c.MAQUINA).First();

                //Revisar si es necesario maquina
                if (string.IsNullOrWhiteSpace(maquina))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.idFuerza = idFuerza;
                ViewBag.idCanal = idCanal;

                var fuerza = from e in db.CADFUERZAS
                             where e.ID == idFuerza
                             select new
                             {
                                 DESCRIPCION = e.DESCRIPCION,
                                 ABREVIATURA = e.ABREVIATURA
                             };

                ViewBag.FUERZA_DESCRIPCION = fuerza.Select(c => c.DESCRIPCION);
                ViewBag.FUERZA_ABREVIATURA = fuerza.Select(c => c.ABREVIATURA);

                var canales = from e in db.CADCANALES
                              where e.CADFUERZ_ID == idFuerza && e.CODIGO == idCanal
                              select new
                              {
                                  CLASE_TURNO = e.CLASE_TURNO,
                                  DESCRIPCION = e.DESCRIPCION
                              };

                if (canales != null)
                {
                    ViewBag.CANAL_DESCRIPCION = canales.Select(c => c.DESCRIPCION);
                    ViewBag.globalClaseTurno = canales.Select(c => c.CLASE_TURNO).FirstOrDefault();
                }
                else
                {
                    ViewBag.CLASE_TURNO = "99"; // Se asigna como administrador
                    ViewBag.globalClaseTurno = "99";
                }

                string strFuerza = Convert.ToString(idFuerza);
                var tiempoRefresco = db.CADREFERENCIAS.Where(c => c.NOMBRE == "TIEMPO_REFRESCO_EVENTOS" && c.CODIGO == strFuerza).Select(c => c.ABREVIATURA).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoRefresco))
                {
                    tiempoRefresco = "15000"; // 15 segundos por defecto

                    ViewBag.MENSAJE_EXITOSO = "Revise en cadreferenciassecad el valor para TIEMPO_PANT_SELECCION.. Se establecen por defecto 15 segundos..";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }

                ViewBag.tiempoRefresco = int.Parse(tiempoRefresco);
                ViewBag.FECHA_GRABACION = DateTime.Now;
                ViewBag.USUA_USUARIO = usr.USUARIO;
                ViewBag.CONSECUTIVO = 0;

                //Eliminar globalnumeEvento cuando sea necesario.
                ViewBag.globalnumeEvento = 0;

                ViewBag.CADCLCI_CODIGO = new SelectList(dbDes.CADCLASIFICACION_CIERRE.Where(c => c.ID_FUERZA == idFuerza).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");

                ViewBag.NOVEDAD = new SelectList(new[] { new { value = "S" , text = "TIENE NOVEDAD" },
                                                         new { value = "N" , text = "SIN NOVEDAD" },
                                                         new { value = "D" , text = "DESASIGNACION"} }, "value", "text");

                var tiempoLlega = db.CADREFERENCIAS.Where(c => c.NOMBRE == "MINUTOS_LLEGA" && c.CODIGO == "01").Select(c => c.DESCRIPCION).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoLlega))
                    ViewBag.tiempoLlega = 5; // 5 Minutos por defecto
                else
                    ViewBag.tiempoLlega = tiempoLlega;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerCerradas(int idFuerza, int idCanal)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strCondicion = " WHERE CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' ";
                string strQuery = " ";
                string strLimite = "";
                string strBoton = "<button class='btn bg-teal btn-mixed' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Crear Evento' " +
                                         " onclick='CargarEventos(NUME_LLAMADA)'>" +
                                        "<i class='icon-power2'></i>" +
                                  "</button>";
                string strBotonDestino = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                GestorDespachos gestorDespachos = new GestorDespachos();
                DataTable dttLugar;
                List<_PedidosPendientesCanalModel> listaRecibidas = new List<_PedidosPendientesCanalModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (NUME_LLAMADA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (HORA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (NUME_TELEFONO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (D_ESTADO_CANAL, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (DSP_RECURSO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (CNT_EVENTOS, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (CNT_RECURSOS, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (CODIGO1, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (MPIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (DIRE_CASO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (BARRIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO = 'C' AND REGEXP_LIKE (PRIORIDAD, '" + parametros.ConsultaValor + "', 'i')) ";
                }

                strLimite = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_condiciones", strCondicion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_pedidoscerrados_canal", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("NUME_LLAMADA", fila["NUME_LLAMADA"].ToString());

                    listaRecibidas.Add(new _PedidosPendientesCanalModel("", fila["NUME_LLAMADA"].ToString(), strBotonDestino, fila["HORA"].ToString(), "", fila["NUME_TELEFONO"].ToString(),
                                                                        fila["ESTADO_CANAL"].ToString(), fila["D_ESTADO_CANAL"].ToString(), fila["DSP_RECURSO"].ToString(), fila["CNT_EVENTOS"].ToString(),
                                                                        fila["CNT_RECURSOS"].ToString(), fila["CODIGO1"].ToString(), fila["MPIO"].ToString(),
                                                                        fila["DIRE_CASO"].ToString(), fila["BARRIO"].ToString(), fila["PRIORIDAD"].ToString(), fila["COMENTARIO"].ToString(), fila["RECEPTOR"].ToString(), ""));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRecibidas });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarEvento(string[] parametros)
        {
            try
            {
                var idSitio = int.Parse(parametros[0]);
                var numeLlamada = decimal.Parse(parametros[1]);
                var idFuerza = int.Parse(parametros[2]);
                var idCanal = int.Parse(parametros[3]);

                //GestorDespachos gestor = new GestorDespachos();
                //var numeEvento = gestor.ConsultarNumeroEvento(int.Parse(parametros[0]), parametros[7]);

                var numeEvento = dbDes.CADEVENTOS
                    .Where(c => c.SITIO_GRABA == idSitio
                             && c.CADPEDCA_NUMELLAMADA == numeLlamada
                             && c.CADPEDCA_FUERZA_ID == idFuerza
                             && c.CADPEDCA_CANAL == idCanal)
                    .Select(c => c.NUME_EVENTO).FirstOrDefault();

                return Json(new { result = true, datos = numeEvento });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public ActionResult VerComentariosAdicionales(int idSitio, decimal numeLlamada)
        {
            try
            {
                var cADPEDIDOS_INFORMACION = dbDes.CADPEDIDOS_INFORMACION.Where(c => c.CADPEDI_SITIOGRABA == idSitio && c.CADPEDI_NUMELLAMADA == numeLlamada);
                return PartialView("_ComentariosAdicionales", cADPEDIDOS_INFORMACION.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}