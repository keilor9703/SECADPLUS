using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class TutorialesController : Controller
    {
        // GET: Tutoriales
        [Authorize]
        public ActionResult Index(string strRutaVideo = null)
        {

            ViewBag.RutaVideo = strRutaVideo;
            return View();
        }
    }
}