using SecadDatos.Areas.MinutaSivicc;
using SECADEntities.SecadEntities;
using SecadModelos.Areas.MinutaSivicc;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Controllers.Sivicc
{
    public class SiviccController : Controller
    {
        // GET: Sivicc
        private CADUSUARIO usuarioActual;
        private Validador validador = new Validador();

        // GET: Sivicc
        public ActionResult Minutas()
        {

            usuarioActual = validador.VerificarUsuario();
            if (usuarioActual == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Verifica si la máquina ha sido identificada.
            if (Session["MAQUINA"] == null)
            {
                Session["MAQUINA"] = "";
                if (!validador.IdentificarCliente(Request, Session))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                    return RedirectToAction("Index", "Home");
                }
            }


            var ListarEstacion = new DbMinuta().F_GetUnidades().ToList();
            ListarEstacion.Add(new MdEstaciones { Id = 0, Descripcion = "[Seleccione Estación]" });
            ListarEstacion = ListarEstacion.OrderBy(x => x.Descripcion).ToList();
            ViewBag.ddlUnidad = new SelectList(ListarEstacion, "Id", "Descripcion");


            return View();
        }

        //Metodo Json que trae funcionarios en las minutas
        public JsonResult F_GetMinuta(int UnidadId)

        {

            var retorno = new DbMinuta().F_GetMinuta(UnidadId); /// .OrderByDescending(x => Convert.ToDateTime(x.Fecha));

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult F_GetMinutaNew(int UnidadId)
        {

            string vFechaInicial = Convert.ToString(Session["vstrFechaInicia"]);
            string vfechaFinal = Convert.ToString(Session["vstrFechaFin"]);

            var retorno = new DbMinuta().F_GetMinutaNew(UnidadId, vFechaInicial, vfechaFinal); /// .OrderByDescending(x => Convert.ToDateTime(x.Fecha));

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        //Metodo Json que trae Armamento
        public JsonResult F_GetMedios(int UnidadId)
        {
            var retorno = new DbMinuta().F_GetMinuta(UnidadId); /// .OrderByDescending(x => Convert.ToDateTime(x.Fecha));

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        //Metodo Json que trae la lista de armamento asignado
        public JsonResult F_GetArmasEmpl(int P_MinEmplId)
        {
            var retorno = new DbMinuta().F_GetArmasEmpl(P_MinEmplId);

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        //Metodo Json que trae la lista de elementos de Comunicaciones Asignados
        public JsonResult F_GetComunicaEmpl(int P_MinEmplId)
        {
            var retorno = new DbMinuta().F_GetComunicaEmpl(P_MinEmplId);

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }
        //Metodo Json que trae la lista de vehiculos Asignados
        public JsonResult F_GetVehiEmpl(int P_MinEmplId)
        {
            var retorno = new DbMinuta().F_GetVehiEmpl(P_MinEmplId);

            if (retorno.Count() > 0)
            {

                return Json(new { data = retorno }, JsonRequestBehavior.AllowGet);

                //return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}