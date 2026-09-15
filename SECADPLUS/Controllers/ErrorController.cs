using SECADPLUS.Models;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ShowError(int error)
        {
            switch (error)
            {
                case 404:
                    return View("Error404", "LoginBase");

                default:
                    return View("Error404", "LoginBase");
            }
        }
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult ApplicationError()
        {
            ViewBag.ERROR_MENSAJE = "";
            if (!string.IsNullOrEmpty(GestorBase.EX_CODIGO))
            {

                ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                GestorBase.EX_CODIGO = "";
                GestorBase.EX_CODIGO_INTERNO = "";
            }
            return View("Error500", "MapaBase");
        }
    }
}