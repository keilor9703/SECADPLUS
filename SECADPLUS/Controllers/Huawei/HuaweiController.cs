using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClProcesosIntegracion;

namespace SECADPLUS.Controllers.Huawei
{
    public class HuaweiController : Controller
    {

        // GET: Huawei
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
         public JsonResult IncluirDatosGrabacionICP(string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10)
        {
            ClWsProcesosIntegracion IngresoDatos = new ClWsProcesosIntegracion();
            ClGrabacionesICP.ClCadPedidosDatosICPOut vsalida = new ClGrabacionesICP.ClCadPedidosDatosICPOut();
            ClGrabacionesICP.ClCadPedidosDatosICP datosGrabacion = new ClGrabacionesICP.ClCadPedidosDatosICP();
              
            datosGrabacion.cadpedi_sitiograba = p1;
            datosGrabacion.cadpedi_numellamada = p2;
            datosGrabacion.acd = p3;
            datosGrabacion.taskid = p4;
            datosGrabacion.caller = p5;
            datosGrabacion.recordid = p6;
            datosGrabacion.called = p7;
            datosGrabacion.callid = p8;
            datosGrabacion.usersecad = p9;
            datosGrabacion.cadequipo = p10;

            System.Exception ExcTmp = new Exception("FUNCIONO!!!!");
            try {
                vsalida = IngresoDatos.MInsDatosGrabacion(datosGrabacion);
            }
            catch(System.Exception Exc)
            {
                ExcTmp = Exc;
            }
            
            return Json(new {CodigoSalida = vsalida.codSalida, Descripcion = vsalida.desSalida, Mensaje = Newtonsoft.Json.JsonConvert.SerializeObject(datosGrabacion), Error = ExcTmp.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ConsutarPersMediosDispRadios(string p1sitio, string p2consMedio)
        {
            ClWsProcesosIntegracion IngresoDatos = new ClWsProcesosIntegracion();
            ClPersMediosDispRadios.ClPersMediosDispRadiosOut vsalida = new ClPersMediosDispRadios.ClPersMediosDispRadiosOut();
            ClPersMediosDispRadios.ClPersMediosDispRadiosIn datosConsulta = new ClPersMediosDispRadios.ClPersMediosDispRadiosIn();

            datosConsulta.sitioGr = p1sitio;
            datosConsulta.consMedio = p2consMedio;

            System.Exception ExcTmp = new Exception("FUNCIONO ConsutarPersMediosDispRadios!!!!");
            try
            {
                vsalida = IngresoDatos.MConsPersMediosDispRadios(datosConsulta);
            }
            catch (System.Exception Exc)
            {
                ExcTmp = Exc;
            }

            return Json(new { CodigoSalida = vsalida.codSalida, Descripcion = vsalida.desSalida, DatosSecad = vsalida.datosSecad, Mensaje = Newtonsoft.Json.JsonConvert.SerializeObject(datosConsulta), Error = ExcTmp.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ConsultarDatosGrabacion(string p1Sitio, string p2Numellamada, string p3FechaIni, string p4FechaFin, string p5Acd)
        {
            ClWsProcesosIntegracion IngresoDatos = new ClWsProcesosIntegracion();
            ClGrabacionesICP.ClDatosGrabacionOut vsalida = new ClGrabacionesICP.ClDatosGrabacionOut();
            ClGrabacionesICP.ClDatosGrabacionIn datosConsulta = new ClGrabacionesICP.ClDatosGrabacionIn();

            datosConsulta.sitioGr = p1Sitio;
            datosConsulta.Llamada = p2Numellamada;
            datosConsulta.fechaInicial = p3FechaIni;
            datosConsulta.fechaFinal = p4FechaFin;
            datosConsulta.acd = p5Acd;



            System.Exception ExcTmp = new Exception("FUNCIONO ConsultarDatosGrabacion!!!!");
            try
            {
                vsalida = IngresoDatos.MConsDatosGrabacion(datosConsulta);
            }
            catch (System.Exception Exc)
            {
                ExcTmp = Exc;
            }

            return Json(new { CodigoSalida = vsalida.codSalida, Descripcion = vsalida.descSalida, DatosSecad = vsalida.datosSecad, Mensaje = Newtonsoft.Json.JsonConvert.SerializeObject(datosConsulta), Error = ExcTmp.Message }, JsonRequestBehavior.AllowGet);

        }

    }
}