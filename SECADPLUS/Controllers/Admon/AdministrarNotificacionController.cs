using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class AdministrarNotificacionController : Controller
    {
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();
        private string strConexion;

        // GET: AdministrarNotificacion
        //[SecadPlusAutorizador("/AdministrarNotificacion/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                NotificacionesViewModel model = new NotificacionesViewModel();

                strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                GestorAdministrador gestor;
                DataTable dttNotificaciones;

                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttNotificaciones = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_estadoNotificaciones", strConexion, "Nonquery");

                foreach (DataRow fila in dttNotificaciones.Rows)
                {
                    if (fila["NOMBRE"].ToString() == "EMERGENTE")
                    {
                        model.NotificacionEmergente = Convert.ToBoolean(fila["PARAMETRO"].ToString());
                    }

                    if (fila["NOMBRE"].ToString() == "NO_EMERGENTE")
                    {
                        model.NotificacionNoEmergente = Convert.ToBoolean(fila["PARAMETRO"].ToString());
                    }
                }

                //model.NotificacionEmergente = Convert.ToBoolean(ConfigurationManager.AppSettings["NotificacionEmergente"]);
                //model.NotificacionNoEmergente = Convert.ToBoolean(ConfigurationManager.AppSettings["NotificacionNoEmergente"]);

                return View(model);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: AdministrarNotificacion/Index To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NotificacionesViewModel model)
        {
            try
            {
                CADVISORPARAMETRO cADVISORPARAMETRO_EMERGENTE = dbDes.CADVISORPARAMETROS.Where(t => t.TIPO == "NOTIFICACIONES" && t.NOMBRE == "EMERGENTE").FirstOrDefault();
                CADVISORPARAMETRO cADVISORPARAMETRO_NOEMERGENTE = dbDes.CADVISORPARAMETROS.Where(t => t.TIPO == "NOTIFICACIONES" && t.NOMBRE == "NO_EMERGENTE").FirstOrDefault();
                cADVISORPARAMETRO_EMERGENTE.PARAMETRO = model.NotificacionEmergente.ToString();
                cADVISORPARAMETRO_NOEMERGENTE.PARAMETRO = model.NotificacionNoEmergente.ToString();

                dbDes.Entry(cADVISORPARAMETRO_EMERGENTE).State = EntityState.Modified;
                dbDes.SaveChanges();

                auditoria.RegistrarAuditoria<CADVISORPARAMETRO>(cADVISORPARAMETRO_EMERGENTE, "CADVISORPARAMETROS", cADVISORPARAMETRO_EMERGENTE.CODIGO.ToString(), TipoAuditoria.Modificar);

                dbDes.Entry(cADVISORPARAMETRO_NOEMERGENTE).State = EntityState.Modified;
                dbDes.SaveChanges();

                auditoria.RegistrarAuditoria<CADVISORPARAMETRO>(cADVISORPARAMETRO_NOEMERGENTE, "CADVISORPARAMETROS", cADVISORPARAMETRO_NOEMERGENTE.CODIGO.ToString(), TipoAuditoria.Modificar);

                //Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

                ////Establese el valor de la llave de configuracion en el web config para las notificaciones emergentes
                //config.AppSettings.Settings["NotificacionEmergente"].Value = model.NotificacionEmergente.ToString();
                //config.Save(ConfigurationSaveMode.Modified);

                ////Establese el valor de la llave de configuracion en el web config para las notificaciones no emergentes
                //config.AppSettings.Settings["NotificacionNoEmergente"].Value = model.NotificacionNoEmergente.ToString();
                //config.Save(ConfigurationSaveMode.Modified);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Notificación ");
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}