using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using SecadDatos.Areas.PlanesPreventivos;
using SecadDatos.Context;
using SecadModelos.Areas.PlanesPrev;
using SecadModelos.Helpers;
using SecadModelos.Visor;
using SECADPLUS.Areas.VisorGeografico.Models;
using SECADPLUS.Areas.VisorGeografico.Models.ModelSIVICCGPS;



namespace SECADPLUS.Areas.VisorGeografico.Controllers
{
    public class VisorXCrimeController : Controller
    {
        // GET: VisorGeografico/VisorXCrime
        public ActionResult Index()
        {
            return View();
        }
    }
}