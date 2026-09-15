using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class ElementosMaestrasController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        //private string strConexion = ConfigurationManager.ConnectionStrings["SECAD12C"].ConnectionString;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: Elementos
        [SecadPlusAutorizador("/ElementosMaestras/Index")]
        public ActionResult Index(int? id, string nom, string strMensaje, string strTipo)
        {
            try
            {
                var mensaje = "";
                var tipo = "";

                if (string.IsNullOrEmpty(strMensaje))
                    mensaje = string.Empty;
                else
                    mensaje = strMensaje;

                if (string.IsNullOrEmpty(strMensaje))
                    tipo = string.Empty;
                else
                    tipo = strTipo;

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;

                if (id == null)
                {
                    id = db.CADTIPOS_BIEN.OrderBy(c => c.DESCRIPCION).Select(c => c.CODIGO).First();
                    nom = db.CADTIPOS_BIEN.OrderBy(c => c.DESCRIPCION).Select(c => c.DESCRIPCION).First();
                }
                var cADTIPOS_BIEN = db.CADTIPOS_BIEN.OrderBy(c => c.DESCRIPCION);
                var cADMARCAS = db.CADMARCAS.Where(c => c.TIBI_CODIGO == id).OrderBy(c => c.DESCRIPCION);
                var cADCLASES_BIEN = db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == id).OrderBy(c => c.DESCRIPCION);
                var cADCARACTERISTICAS = db.CADCARACTERISTICAS.Where(c => c.TIBI_CODIGO == id).OrderBy(c => c.DESCRIPCION);

                var variosModelos = new Tuple<List<CADTIPOS_BIEN>, List<CADMARCA>, List<CADCLASES_BIEN>, List<CADCARACTERISTICA>>
                    (cADTIPOS_BIEN.ToList(), cADMARCAS.ToList(), cADCLASES_BIEN.ToList(), cADCARACTERISTICAS.ToList())
                { };

                ViewBag.TIBI_CODIGO = id;
                ViewBag.TIPO_BIEN = nom;
                ViewBag.TIPO_BIEN1 = nom;
                ViewBag.TIPO_BIEN2 = nom;
                ViewBag.TIPO_BIEN3 = nom;



                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetMarcas(int tibiCodigo)
        {
            try
            {
                var cADMARCAS = db.CADMARCAS.Where(c => c.TIBI_CODIGO == tibiCodigo).OrderBy(c => c.DESCRIPCION);
                return PartialView("_marcas", cADMARCAS.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetClasesBien(int tibiCodigo)
        {
            try
            {
                var cADCLASES_BIEN = db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == tibiCodigo).OrderBy(c => c.DESCRIPCION);
                return PartialView("_clasesBien", cADCLASES_BIEN.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetCaracteristicas(int tibiCodigo)
        {
            try
            {
                var cADCARACTERISTICAS = db.CADCARACTERISTICAS.Where(c => c.TIBI_CODIGO == tibiCodigo).OrderBy(c => c.DESCRIPCION);
                return PartialView("_caracteristicas", cADCARACTERISTICAS.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}