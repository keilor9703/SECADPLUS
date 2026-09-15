using Newtonsoft.Json;
using SecadDatos.Areas.PlanesPreventivos;
using SecadDatos.Context;
using SecadModelos.Areas.PlanesPrev;
using SecadModelos.Helpers;
using SecadModelos.Visor;
using SECADPLUS.Areas.VisorGeografico.Models;
using SECADPLUS.Areas.VisorGeografico.Models.ModelSIVICCGPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Areas.VisorGeografico.Controllers
{
    public class ImeiTrackingController : Controller
    {

        // GET: VisorGeografico/ImeiTracking
        private DbContexSIVICC Db = new DbContexSIVICC();
        public ActionResult MapaImeiTracking()
        {
            ViewBag.ListaCapas = new DbSivicc().F_GetCapaDinamica().ToList();

            ViewBag.ddlRegion = new SelectList(new DbSivicc().F_GetUnidades().Where(x => x.TiunCodigo == 308 && x.Regi=="REGI1").OrderBy(x => x.SiglaFisica), "SiglaFisica", "Dependencia");

            return View("MapaImeiTracking", "~/Areas/VisorGeografico/Views/Shared/_LayoutMapas.cshtml");
        }
        [HttpPost]
        //Listar los Dominios
        public List<MdDominios> ListarDominio(int DominioId)
        {
            var retorno = new DbSivicc().f_ListarDominio(DominioId);
            return retorno;
        }
        //Listar los equipos cuadrantes en tiempo real
        public ActionResult ObtenerImeiTracking(long? v_identificacion, int v_cuadrante, string v_region, string v_unidad, int v_dependencia)
        {

            if (v_identificacion == null)
            {
                v_identificacion = 0;
            }

            if (v_cuadrante == 0)
            {
                v_cuadrante = 0;
            }

            if (v_dependencia == 0)
            {
                v_dependencia = 0;
            }


            var UbicacionGPS = ListarUbicacionGPS(v_identificacion, v_cuadrante, v_region, v_unidad, v_dependencia);
            var Alarmas = new DbSivicc().F_GetListadoAlarmas().ToList();

            List<MdPlanesPreventivos> ListPlanes = new List<MdPlanesPreventivos>();
            ListPlanes = new DbPlanesPrev().F_GetPlanesGPS(v_dependencia).ToList();

            var recorrido = ListarUbicacionGPS(v_identificacion, v_cuadrante, v_region, v_unidad, v_dependencia)
                .Select(x => new
                {
                    x.UbicacionGpsId,
                    x.Latitud,
                    x.Longitud,
                    x.Velocidad,
                    x.NivelBateria,
                    Fecha = x.Fecha,
                    x.CodigoCuadrante,
                    x.Identificacion,
                    x.NombresApellidos,
                    x.Imei,
                    x.NumeroCelularCuadrante,
                    x.SerialSim,
                    x.SesionActiva,
                    x.TipoLectura,
                    x.VersionApk,
                    x.DireccionIP,
                    x.FechaInstalacion,
                    x.DependenciaDescripcion,
                    x.DependenciaSigla,
                    x.DependenciaSiglaPapa,
                    x.DispositivoSerie,
                    x.DispositivoMarcaModelo,
                    x.EstaDentroCuadrante,
                    x.Duracion,
                    x.Minutos
                })
            .ToList()
            ;
            
            try
            {
                if (recorrido == null || recorrido.Count == 0)
                {
                    var retorno = new
                    {
                        HayDatos = false
                    };
                    return Content(JsonConvert.SerializeObject(retorno), "application/json");
                }
                else
                {
                    var coleccionPuntos = new GeoJSON.Net.Feature.FeatureCollection();
                    var coleccionPuntosAlarmas = new GeoJSON.Net.Feature.FeatureCollection();
                    var coleccionPuntosPlanes = new GeoJSON.Net.Feature.FeatureCollection();

                    //Puntos para las alarmas
                    foreach (var puntoAlarma in Alarmas)
                    {
                        var propiedadesAlarma = new Dictionary<string, object>();
                        propiedadesAlarma.Add("Latitud", puntoAlarma.Latitud);
                        propiedadesAlarma.Add("Longitud", puntoAlarma.Longitud);
                        propiedadesAlarma.Add("Ciudadano", puntoAlarma.Ciudadano);
                        propiedadesAlarma.Add("Identificacion", puntoAlarma.Identificacion);
                        propiedadesAlarma.Add("Direccion", puntoAlarma.Direccion);
                        propiedadesAlarma.Add("Telefono", puntoAlarma.TelefonoFijo);
                        propiedadesAlarma.Add("Celular", puntoAlarma.Celular);
                        propiedadesAlarma.Add("Correo", puntoAlarma.Correo);
                        propiedadesAlarma.Add("Segmento", puntoAlarma.Segmento);
                        propiedadesAlarma.Add("Dto", puntoAlarma.Dto);
                        propiedadesAlarma.Add("IdGpsActual", puntoAlarma.IdGpsActual);
                        propiedadesAlarma.Add("IdAlarma", puntoAlarma.IdAlarma);
                        propiedadesAlarma.Add("Municipio", puntoAlarma.Municipio);
                        propiedadesAlarma.Add("IdentificacionEncryp", ClsEncriptar.Encriptar(Convert.ToString(puntoAlarma.Identificacion)));

                        coleccionPuntosAlarmas.Features.Add(new GeoJSON.Net.Feature.Feature(
                           new GeoJSON.Net.Geometry.Point(new GeoJSON.Net.Geometry.Position(Convert.ToDouble(puntoAlarma.Latitud), Convert.ToDouble(puntoAlarma.Longitud))),
                           propiedadesAlarma, puntoAlarma.IdGpsActual.ToString()));
                    }


                    //Planes
                    foreach (var puntoPlanes in ListPlanes)
                    {
                        var propiedades = new Dictionary<string, object>();
                        propiedades.Add("Tipoplan", puntoPlanes.Tipoplan);
                        propiedades.Add("Inicio", puntoPlanes.Inicio);
                        propiedades.Add("Modifica", puntoPlanes.Modifica);
                        propiedades.Add("Dispositivo", puntoPlanes.Dispositivo);
                        propiedades.Add("Latitud", puntoPlanes.Latitud);
                        propiedades.Add("Longitud", puntoPlanes.Longitud);
                        propiedades.Add("Jefe", puntoPlanes.Jefe);
                        propiedades.Add("Celular", puntoPlanes.Celular);

                        coleccionPuntosPlanes.Features.Add(new GeoJSON.Net.Feature.Feature(
                            new GeoJSON.Net.Geometry.Point(new GeoJSON.Net.Geometry.Position(puntoPlanes.Latitud, puntoPlanes.Longitud)),
                            propiedades, puntoPlanes.IdPLan.ToString()));
                    }

                    //Poblar Cuadrantes
                    foreach (var punto in recorrido)
                    {
                        var propiedades = new Dictionary<string, object>();
                        propiedades.Add("UbicacionGpsId", punto.UbicacionGpsId);
                        propiedades.Add("Imei", punto.Imei);
                        propiedades.Add("Identificacion", punto.Identificacion);
                        propiedades.Add("NombresApellidos", punto.NombresApellidos);
                        propiedades.Add("Fecha", punto.Fecha);
                        propiedades.Add("Velocidad", punto.Velocidad + " kM/h");
                        propiedades.Add("NivelBateria", punto.NivelBateria + "%");
                        propiedades.Add("Cuadrante", punto.CodigoCuadrante);
                        propiedades.Add("CelularCuadrante", punto.NumeroCelularCuadrante);
                        propiedades.Add("SerialSim", punto.SerialSim);
                        propiedades.Add("SesionActiva", Convert.ToBoolean(punto.SesionActiva) ? "Si" : "No");
                        propiedades.Add("TipoLectura", punto.TipoLectura.ToUpper());
                        propiedades.Add("Latitud", punto.Latitud);
                        propiedades.Add("Longitud", punto.Longitud);
                        propiedades.Add("VersionApk", punto.VersionApk);
                        propiedades.Add("DireccionIP", punto.DireccionIP);
                        propiedades.Add("FechaInstalacion", punto.FechaInstalacion?.ToString("dd/MM/yyyy"));
                        propiedades.Add("DependenciaDescripcion", punto.DependenciaDescripcion);
                        propiedades.Add("DependenciaSigla", punto.DependenciaSigla);
                        propiedades.Add("DependenciaSiglaPapa", punto.DependenciaSiglaPapa);
                        propiedades.Add("DentroCuadrante", punto.EstaDentroCuadrante);
                        propiedades.Add("DentroCuadranteText", Convert.ToBoolean(punto.EstaDentroCuadrante) ? "Si" : "No");
                        propiedades.Add("DispositivoSerie", punto.DispositivoSerie);
                        propiedades.Add("DispositivoMarcaModelo", punto.DispositivoMarcaModelo);
                        propiedades.Add("Duracion", punto.Minutos);
                        propiedades.Add("DuracionT", punto.Duracion);
                        propiedades.Add("IdentificacionEncry", ClsEncriptar.Encriptar(Convert.ToString(punto.Identificacion)));

                        coleccionPuntos.Features.Add(new GeoJSON.Net.Feature.Feature(
                            new GeoJSON.Net.Geometry.Point(new GeoJSON.Net.Geometry.Position(punto.Latitud, punto.Longitud)),

                            propiedades, punto.UbicacionGpsId.ToString()));
                        var posicion = new GeoJSON.Net.Geometry.Position(punto.Latitud, punto.Longitud);
                    }
                    var coleccion = new
                    {
                        Puntos = coleccionPuntos,
                        PuntosAlarmas = coleccionPuntosAlarmas,
                        PuntosPlanes = coleccionPuntosPlanes,
                        HayDatos = true
                    };
                    return Content(JsonConvert.SerializeObject(coleccion), "application/json");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<V_CONSULTA_ACTUAL_GPS> ListarUbicacionGPS(long? v_identificacion, int v_cuadrante, string v_region, string v_unidad, int v_dependencia)
        {

            var ListaPuntos = Db.UbicacionGPSActual.AsNoTracking();

         //   var ListaPuntos = new DbSivicc().F_GetUbicacionPda().ToList();


            var DosHoras = DateTime.Now.AddHours(-0.5);

        
            if (v_identificacion == null)
            {
                v_identificacion = 0;
            }

            if (v_identificacion > 0) //Consulta Cedula
            {
                return ListaPuntos.Where(x => x.Identificacion == v_identificacion && x.Fecha >= DosHoras);
            }
            else if (v_cuadrante > 0)
            {
                return ListaPuntos.Where(x => x.CuadranteId == v_cuadrante && x.DependenciaId == v_dependencia);
            }
            else if (v_identificacion == 0 && v_cuadrante == 0 && v_region != null && v_unidad == null && v_dependencia == 0) //Consulta Región
            {
                return ListaPuntos.Where(x => x.DescRegion == v_region && x.Fecha >= DosHoras);
            }
            else if (v_identificacion == 0 && v_cuadrante == 0 && v_region != null && v_unidad != null && v_dependencia == 0) //Consulta Unidad
            {
                return ListaPuntos.Where(x => x.DependenciaSiglaPapa == v_unidad && x.Fecha >= DosHoras);
            }
            else if (v_identificacion == 0 && v_cuadrante == 0 && v_region != null && v_unidad != null && v_dependencia != 0) //Consulta Dependencia
            {
                return ListaPuntos.Where(x => x.DependenciaId == v_dependencia && x.Fecha >= DosHoras); ;
            }
            else
            {
                return ListaPuntos.Where(x => x.Identificacion == 0);
            }

        }

        public ActionResult ApagarAlarma(int v_id)
        {
            DbSivicc db_GpsDay = new DbSivicc();

            db_GpsDay.DesacAlamarmas(v_id, Convert.ToString(Session["USER"]));
            return Json(new { success = true, message = "Alarma Desactivada" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnviarCasoSecad(int v_id, string v_direccion, string postal, string p_dto, string p_municipio, string p_ciudad, string P_Barrio)

        {
            DbSivicc db = new DbSivicc();
            int CodigoSecad = 0;

            if (v_direccion == "Buscar Dirección")
            {
                v_direccion = "NO REPORTADA";
            }

            try
            {
                if (postal != null)
                {
                    CodigoSecad = db.F_CodigoSecad(Convert.ToInt32(postal));
                }
                else
                {
                    CodigoSecad = 1228;
                }
            }
            catch (Exception)
            {

                CodigoSecad = 1228;
            }

            var Casos = db.CosultaAlarmas(v_id).ToList();

            MdPedidos obj = new MdPedidos();
            try
            {
                foreach (var Item in Casos)
                {
                    obj.Hora_caso = Convert.ToDateTime(Item.FECHA_CREACION);
                    obj.Disp_telefonico = "02";
                    obj.Celda_marcacion = null;
                    obj.Nume_telefono = Convert.ToInt64(Item.CELULAR);
                    obj.Prop_telefono = Item.CIUDADANO;
                    obj.Dire_telefono = Item.DIRECCION_CIUDADANO;
                    obj.Luge_telefono = Convert.ToInt32(Item.ID_MUNICIPIO);
                    obj.Barr_telefono = "0";
                    obj.Tipo_pedido = "40";
                    obj.Cali_pedido = "01";
                    obj.Tipo_informacion = "RC";
                    obj.Codigo_cooperante = Convert.ToInt32(Item.ID_RPC_VINCULADO);
                    obj.Nomb_llamante = Item.CIUDADANO;
                    obj.Dire_llamante = Item.DIRECCION_CIUDADANO;
                    obj.Luge_codigo = CodigoSecad; //Convert.ToDecimal(Item.ID_LUGAR);
                    obj.Dire_caso = v_direccion + "-" + p_municipio;
                    obj.Puca_codigo = 1;
                    obj.Codi_barrio = null;
                    obj.Luge_barrio = null;
                    obj.Latitud_caso = Convert.ToString(Item.LATITUD);
                    obj.Longitud_caso = Convert.ToString(Item.LONGITUD);
                    obj.Comentario = "Alarma activada por el Ciudadano";
                    obj.Codi_pedido = "947";
                    obj.Importancia = "01";
                    obj.Prioridad = "01";
                    obj.Estado = "P";
                    obj.Enviar = "S";
                    obj.Cadusua_usuario = Convert.ToString(Session["USER"]);
                    obj.Cadpoli_cedu_empleado = "0";
                    obj.Cadpedi_sitio_graba = null;
                    obj.Cadpedi_nume_llamada = null;
                    obj.Total_canales = "N";
                    obj.Tipo_genero = "CG";
                }

                var num_llamada = db.EnviarCasoSecad(obj);

                if (num_llamada != "0")
                {
                    db.UdpNumeroCasoSecad(v_id, Convert.ToInt64(num_llamada), p_dto, p_municipio, p_ciudad, P_Barrio);
                    return Json(new { success = true, message = num_llamada }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "0" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(new { success = false, message = "El caso no puede ser enviado, revise si ya existe en SECAD" }, JsonRequestBehavior.AllowGet);
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}