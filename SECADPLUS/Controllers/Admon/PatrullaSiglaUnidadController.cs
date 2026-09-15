using SECADPLUS.Models;
using System;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class PatrullaSiglaUnidadController : Controller
    {
        // GET: PatrullaSiglaUnidad
        private GestorGeneral gestor;

        [SecadPlusAutorizador("/PatrullaSiglaUnidad/Consultar/")]
        public ActionResult Consultar()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult ConsultarPatrulla(int intSitio, string strPatrulla)
        {
            try
            {
                string strResultado = "";
                gestor = new GestorGeneral();
                strResultado = gestor.ConsultarPatrullas(intSitio, strPatrulla);
                return Json(new { result = true, datos = strResultado });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ConsultarVehiculo(string strSigla)
        {
            try
            {
                string strResultado = "";
                gestor = new GestorGeneral();
                strResultado = gestor.ConsultarVehiculo(strSigla);
                return Json(new { result = true, datos = strResultado });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ConsultarUnidadRastreo(string strSigla)
        {
            try
            {
                string strResultado = "";
                gestor = new GestorGeneral();
                strResultado = gestor.ConsultarUnidadRastreo(strSigla);
                return Json(new { result = true, datos = strResultado });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}