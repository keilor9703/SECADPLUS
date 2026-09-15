using SecadDatos.Context;
using SECADPLUS.Areas.VisorGeografico.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Areas.VisorGeografico.Controllers
{
    public class UnidadesDependenciaController : Controller
    {
        private ContextRehu db = new ContextRehu();
        //private ContextSivicc dbSivicc = new ContextSivicc();

        public JsonResult GetUnidad(string id)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            if (id != null)
            {
                foreach (var item in db.VM_UNIDADES_PONAL.Where(X => X.VIGENTE == "SI" && X.REGI == "REGI1" && (X.TIUN_CODIGO == 4 || X.TIUN_CODIGO == 5 ) && X.SIGLA_FISICA == "METUN").OrderBy(x => x.ORDENAMIENTO))
                {
                    lista.Add(new SelectListItem
                    {
                        Text = item.SIGLA_FISICA.ToString() + " - " + item.DESCRIPCION_DEPENDENCIA.ToString(),
                        Value = item.SIGLA_FISICA.ToString()
                    });
                }
            }
            else
            {
                foreach (var item in db.VM_UNIDADES_PONAL.Where(X => X.VIGENTE == "SI" && X.REGI == id && (X.TIUN_CODIGO == 4 || X.TIUN_CODIGO == 5)).OrderBy(x => x.ORDENAMIENTO))
                {
                    lista.Add(new SelectListItem { Text = "No encontrado", Value = "0" });
                }
            }

            lista.Add(new SelectListItem { Text = "Unidad no seleccionada", Value = "0" });
            db.Dispose();
            return Json(lista.OrderBy(x => x.Value), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependencia(string id)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            if (id != null)
            {
                foreach (var item in db.VM_UNIDADES_PONAL.Where(X => X.VIGENTE == "SI" && X.SIGLA_DEPENDE == id && (X.TIUN_CODIGO == 7)).OrderBy(x => x.ORDENAMIENTO))
                {
                    lista.Add(new SelectListItem
                    {
                        Text = item.DESCRIPCION_DEPENDENCIA.ToString(),
                        Value = item.CONSECUTIVO.ToString()
                    });
                }
            }
            else
            {
                foreach (var item in db.VM_UNIDADES_PONAL.Where(X => X.VIGENTE == "SI" && X.SIGLA_DEPENDE == id && (X.TIUN_CODIGO == 308 || X.TIUN_CODIGO == 4 || X.TIUN_CODIGO == 5 || X.TIUN_CODIGO == 6 || X.TIUN_CODIGO == 123 || X.TIUN_CODIGO == 164 || X.TIUN_CODIGO == 7 || X.TIUN_CODIGO == 14 || X.TIUN_CODIGO == 384)).OrderBy(x => x.ORDENAMIENTO))
                {
                    lista.Add(new SelectListItem { Text = "No encontrado", Value = "0" });
                }
            }

            lista.Add(new SelectListItem { Text = "Dependencia no seleccionada", Value = "0" });
            db.Dispose();
            return Json(lista.OrderBy(x => x.Value), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCuadrante(int? id)
        {


            var ListaCuadrantes = new DbSivicc().F_GetCuadrantes().Where(x => x.UndeConsec == id);


            List<SelectListItem> lista = new List<SelectListItem>();
            if (id != null)
            {
                foreach (var item in ListaCuadrantes)
                {
                    lista.Add(new SelectListItem
                    {
                        Text = item.CodigoCuadrante.ToString(),
                        Value = item.CuadranteId.ToString()
                    });
                }
            }
            else
            {
                foreach (var item in ListaCuadrantes)
                {
                    lista.Add(new SelectListItem { Text = "No encontrado", Value = "0" });
                }
            }
            lista.Add(new SelectListItem { Text = "Cuadrante no seleccionado", Value = "0" });
            //dbSivicc.Dispose();
            return Json(lista.OrderBy(x => x.Value), JsonRequestBehavior.AllowGet);
        }
    }
}