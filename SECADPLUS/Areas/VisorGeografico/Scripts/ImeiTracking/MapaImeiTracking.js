var map;
$(document).ready(function () {
    $("#botonFiltros").click(function () {
        $("#panelFiltros").show();
    });
    $(".hide-link").click(function () {
        $("#panelFiltros").hide();
    });
    $("#botonCapas").click(function () {
        $("#panelCapas").show();
    });
    $(".hide-capas-link").click(function () {
        $("#panelCapas").hide();
    });
    $("#botonDireccion").click(function () {
        $("#panelDireccion").show();
    });
    $(".hide-direccion-link").click(function () {
        $("#panelDireccion").hide();
    });

    $(".selector-capa").click(function () {
        if ($(this).prop("checked")) {
            var layer = new esri.layers.ArcGISDynamicMapServiceLayer($(this).attr("data-url"));
            map.addLayer(layer);
            layer.setImageFormat("png32");
        } else {
            var urlCapa = $(this).attr("data-url");
            $.each(map.layerIds, function (indice, valor) {
                var capa = map.getLayer(valor);
                if (capa != null && capa != undefined && capa.url == urlCapa) {
                    map.removeLayer(capa);
                }
            });
        }
    });
});

require(["esri/map",
    "esri/dijit/BasemapGallery",
  //  "esri/layers/OpenStreetMapLayer",
    "esri/InfoTemplate",
    "esri/dijit/FeatureTable",
    "../../Mapas/js/geojsonlayer.js",
    "esri/renderers/SimpleRenderer",
    "esri/geometry/Point",
    "esri/symbols/PictureMarkerSymbol",
    "esri/SpatialReference",
    "esri/graphic",
    "esri/graphicsUtils",
    "esri/layers/GraphicsLayer",
    "dojo/_base/array",
    "esri/request",
    "dojo/on",
    "dojo/query",
    "dojo/dom",
    "dojo/dom-style",
    "dojo/domReady!"],
    function (Map, BasemapGallery, InfoTemplate, FeatureTable, GeoJsonLayer, SimpleRenderer, Point, PictureMarkerSymbol, SpatialReference, Graphic, graphicsUtils, GraphicsLayer, array, Request, on, query, dom, domStyle) {
        map = new Map("map", {
            basemap: "streets-night-vector",
            center: [-72, 4],
            zoom: 6
        });

       // openStreetMapLayer = new OpenStreetMapLayer();
      //  map.addLayer(openStreetMapLayer);

        map.infoWindow.domNode.className += " light";

        map.on("extent-change", function () {
            DeseleccionarPuntoPermanente();
        });
        function selectGeoJsonData(e) {
            var url;
            if (e.target.nodeName === "SELECT") {
                url = dom.byId("geoJsonUrl").value = e.target.options[e.target.selectedIndex].value;
            } else {
                url = dom.byId("geoJsonUrl").value;
            }
            addGeoJsonLayer(url);
        }

        var basemapGallery = new BasemapGallery({
            showArcGISBasemaps: true,
            map: map
        }, "basemapGallery");
        basemapGallery.startup();

        basemapGallery.on("error", function (msg) {
            console.log("Ha ocurrido un error cargando la Galeria de Mapas Base:  ", msg);
        });


        function removeAllLayers() {
            /*
            var i, lyr, ids = map.graphicsLayerIds;
            for (i = ids.length - 1; i > -1; i--) {
                lyr = map.getLayer(ids[i]);
                map.removeLayer(lyr);
            }
            */
            map.graphics.clear();
            map.infoWindow.hide();
        }

        function addGeoJsonLayer(url) {
            waitingDialog.hide();
            waitingDialog.show('Procesando...');
            var request = Request({
                url: url,
            });
            request.then(
                function (data) {
                    if (data.HayDatos) {
                        /// Puntos
                        var capaPuntos = new GeoJsonLayer({
                            data: data.Puntos
                        });

                        var puntos = new Array();
                        var marker_green = new PictureMarkerSymbol("../../img/policia_1.png", 43, 43);
                        var marker_yellow = new PictureMarkerSymbol("../../img/policia_2.png", 43, 43);
                        var marker_red = new PictureMarkerSymbol("../../img/policia_3.png", 43, 43);
                        var marker_gray = new PictureMarkerSymbol("../../img/policia_4.png", 43, 43);
                        var marker_alarma = new PictureMarkerSymbol("../../img/alarma.gif", 200, 80);

                        var wgs84 = new SpatialReference({ wkid: 4326 });
                        var geometry = new Point(wgs84);

                        array.forEach(data.Puntos.features, function (item) {
                            var point = new Point(Number(item.geometry.coordinates[0]), Number(item.geometry.coordinates[1]));

                            if (item.properties.DuracionT > 799) {
                                var pointGraphic = new Graphic(point, marker_gray);
                            } else {
                                switch (item.properties.DentroCuadrante) {
                                    case 1:
                                        var pointGraphic = new Graphic(point, marker_green);
                                        break;
                                    case 0:
                                        var pointGraphic = new Graphic(point, marker_red);
                                        break;
                                    default:
                                        var pointGraphic = new Graphic(point, marker_green);
                                        break;
                                }
                            }
                            var infoTemplate = new InfoTemplate();
                            infoTemplate.setTitle("Información de funcionario");
                            infoTemplate.setContent("<table><tr><td rowspan='5'><img src='https://sinac.policia.gov.co:8443/SinacPicture/picture.aspx?DocID=" + item.properties.IdentificacionEncry + "&Token=Mxl7995Julabdfjughyts1*_58$$" + "' style='border-radius:50%' width='120px' height='150px' align='center'></td><td><b>Funcionario: </b>" + item.properties.NombresApellidos + "</td></tr><tr><td ><b>Cédula: </b>" + item.properties.Identificacion + "</td></tr><tr><td ><b>IMEI: </b>" + item.properties.Imei + "</td></tr><tr><td><b style='color: #169201;'>Sesion PDA Activa:</b>" + item.properties.SesionActiva + "</td></tr><tr><td><b style='color: #169201;'>Tiempo Permanencia:</b>" + item.properties.Duracion + "</td></tr></table><br>"
                                + "<b style='color: #169201;'>Cuadrante: " + item.properties.Cuadrante + "</b><br>"
                                + "<b>Dentro del Cuadrante: </b>" + item.properties.DentroCuadranteText + "<hr><br>"
                                + "<b>Fecha: </b>" + item.properties.Fecha + "<br>"
                                + "<b>Velocidad: </b>" + item.properties.Velocidad + "<br>"
                                + "<b>Nivel de Bateria: </b>" + item.properties.NivelBateria + "<br>"
                                + "<b>Unidad:</b>" + item.properties.DependenciaDescripcion + "<br>"
                                + "<b>Celular Cuadrante: </b>" + item.properties.CelularCuadrante + "<br>"
                                + "<b>Serial del dispositivo: </b>" + item.properties.DispositivoSerie + "<br>"
                                + "<b>Marca/Modelo: </b>" + item.properties.DispositivoMarcaModelo + "<br>"
                                + "<b>Versión APK: </b>" + item.properties.VersionApk + "<br>"
                                + "<b>Direccion: </b><label id=\"lbDireccion\"><a href=\"javascript:ObtenerDireccion(" + item.properties.Latitud + "," + item.properties.Longitud + ")\">Buscar Dirección</a></label><br>"
                            );
                            pointGraphic.setInfoTemplate(infoTemplate);
                            map.graphics.add(pointGraphic);
                        });
                        capaPuntos.on("update-end", function (e) {
                            if (data.Puntos.features.length > 1) {
                                map.setExtent(e.target.extent.expand(1.2));
                            } else {
                                map.centerAndZoom(new Point(data.Puntos.features[0].geometry.coordinates[0], data.Puntos.features[0].geometry.coordinates[1]), 15);
                            }
                        });
                        map.addLayer(capaPuntos);
                        map.removeLayer(capaPuntos);

                        //Puntos Planes
                        var capaPuntosPlanes = new GeoJsonLayer({
                            data: data.PuntosPlanes
                        });
                        var puntosPlanes = new Array();
                        var Baliza = new PictureMarkerSymbol("../../img/Baliza.gif", 50, 50);
                        var Bancos = new PictureMarkerSymbol("../../img/Bancos.png", 40, 40);
                        var Balnearios = new PictureMarkerSymbol("../../img/Balnearios.png", 40, 40);
                        var Campanas = new PictureMarkerSymbol("../../img/Estandar.png", 40, 40);
                        var Comercio = new PictureMarkerSymbol("../../img/Hurto.png", 13, 13);
                        var Cierre = new PictureMarkerSymbol("../../img/Cierre.png", 40, 40);
                        var Colegios = new PictureMarkerSymbol("/Sivicc2.Web.Administracion/Content/img/Colegios.png", 40, 40);
                        var Fincas = new PictureMarkerSymbol("../../img/Estandar.png", 40, 40);
                        var Iglesias = new PictureMarkerSymbol("../../img/Iglesias.png", 40, 40);
                        var Paraderos = new PictureMarkerSymbol("../../img/Paraderos.png", 40, 40);
                        var Parqueaderos = new PictureMarkerSymbol("../../img/Parqueaderos.png", 40, 40);
                        var Parques = new PictureMarkerSymbol("../../img/Parques.png", 40, 40);
                        var Puentes = new PictureMarkerSymbol("../../img/Puentes.png", 40, 40);
                        var Control = new PictureMarkerSymbol("../../img/PuestoControl.png", 40, 40);
                        var Supermercados = new PictureMarkerSymbol("../../img/Marcado.png", 40, 40);
                        var Transporte = new PictureMarkerSymbol("../../img/Masivo.png", 40, 40);
                        var Cicla = new PictureMarkerSymbol("../../img/Cicla.png", 40, 40);
                        var VendedorAmbulante = new PictureMarkerSymbol("../../img/VendedorAmbulante.png", 40, 40);
                        var Playas = new PictureMarkerSymbol("../../img/Playas.png", 40, 40);
                        var Perifoneo = new PictureMarkerSymbol("../../img/Perifoneo.png", 40, 40);
                        var ComercioBici = new PictureMarkerSymbol("../../img/ComercioBici.png", 40, 40);
                        var Cementerios = new PictureMarkerSymbol("../../img/Cementerios.png", 40, 40);


                        var wgs84s = new SpatialReference({ wkid: 4326 });
                        var geometrys = new Point(wgs84s);
                        array.forEach(data.PuntosPlanes.features, function (item) {
                            var points = new Point(Number(item.properties.Longitud.replace(",", ".")), Number(item.properties.Latitud.replace(",", ".")));
                            var pointAttributess = { city: "Albuquerque", state: "New Mexico" };
                            //TODO: Aca se debe realizar el switch case para pintar marcadores según el grupo del delito.
                            switch (item.properties.Tipoplan) {
                                case "Baliza":
                                    var pointGraphics = new Graphic(points, Baliza, pointAttributess);
                                    break;
                                case "Bancos":
                                    var pointGraphics = new Graphic(points, Bancos, pointAttributess);
                                    break;
                                case "Balnearios":
                                    var pointGraphics = new Graphic(points, Balnearios, pointAttributess);
                                    break;
                                case "Campañas Prevención":
                                    var pointGraphics = new Graphic(points, Campanas, pointAttributess);
                                    break;
                                case "Centros Comerciales":
                                    var pointGraphics = new Graphic(points, Comercio, pointAttributess);
                                    break;
                                case "Cierre Establecimientos Públicos":
                                    var pointGraphics = new Graphic(points, Cierre, pointAttributess);
                                    break;
                                case "Colegios":
                                    var pointGraphics = new Graphic(points, Colegios, pointAttributess);
                                    break;
                                case "Fincas":
                                    var pointGraphics = new Graphic(points, Fincas, pointAttributess);
                                    break;
                                case "Iglesias":
                                    var pointGraphics = new Graphic(points, Iglesias, pointAttributess);
                                    break;
                                case "Paradero de Buses":
                                    var pointGraphics = new Graphic(points, Paraderos, pointAttributess);
                                    break;
                                case "Parqueaderos":
                                    var pointGraphics = new Graphic(points, Parqueaderos, pointAttributess);
                                    break;
                                case "Parques":
                                    var pointGraphics = new Graphic(points, Parques, pointAttributess);
                                    break;
                                case "Puentes Peatonales":
                                    var pointGraphics = new Graphic(points, Puentes, pointAttributess);
                                    break;
                                case "Puesto de Control":
                                    var pointGraphics = new Graphic(points, Control, pointAttributess);
                                    break;
                                case "Supermercados":
                                    var pointGraphics = new Graphic(points, Supermercados, pointAttributess);
                                    break;
                                case "Transporte Masivo":
                                    var pointGraphics = new Graphic(points, Transporte, pointAttributess);
                                    break;
                                case "Ciclo Rutas":
                                    var pointGraphics = new Graphic(points, Cicla, pointAttributess);
                                    break;
                                case "Ciclo Via":
                                    var pointGraphics = new Graphic(points, Cicla, pointAttributess);
                                    break;
                                case "Playas":
                                    var pointGraphics = new Graphic(points, Playas, pointAttributess);
                                    break;
                                case "Vendedores Ambulantes":
                                    var pointGraphics = new Graphic(points, VendedorAmbulante, pointAttributess);
                                    break;
                                case "Comercio de Bicicletas":
                                    var pointGraphics = new Graphic(points, ComercioBici, pointAttributess);
                                    break;
                                case "Perifonéo":
                                    var pointGraphics = new Graphic(points, Perifoneo, pointAttributess);
                                    break;
                                case "Control Cementerios":
                                    var pointGraphics = new Graphic(points, Cementerios, pointAttributess);
                                    break;
                                default:
                                    var pointGraphics = new Graphic(points, Fincas, pointAttributess);
                                    break;
                            }
                            map.graphics.add(pointGraphics);

                            var infoTemplates = new InfoTemplate();
                            infoTemplates.setTitle(item.properties.Tipoplan);
                            infoTemplates.setContent(
                                "<b style='color: #169201;'>Plan: </b>" + item.properties.Tipoplan + "<br>" +
                                "<b>Jefe Servicio:</b>" + item.properties.Jefe + "<br>" +
                                "<b>Celular:</b>" + item.properties.Celular + "<br>" +
                                "<b>Fecha Inicio:</b>" + item.properties.Inicio + "<br>" +
                                "<b>Fecha Modifica:</b>" + item.properties.Modifica + "<br>" +
                                "<b>Dispositivo:</b>" + item.properties.Dispositivo + "<br>");

                            pointGraphics.setInfoTemplate(infoTemplates);
                        });
                        ///Fin Planes

                        //Puntos Alarmas
                        var capaPuntosAlarmas = new GeoJsonLayer({
                            data: data.PuntosAlarmas
                        });

                        array.forEach(data.PuntosAlarmas.features, function (item) {
                            var point = new Point(Number(item.geometry.coordinates[0]), Number(item.geometry.coordinates[1]));

                            var pointGraphic = new Graphic(point, marker_alarma);


                            var infoTemplate = new InfoTemplate();
                            infoTemplate.setTitle("Esta es una Emergencia");
                            infoTemplate.setContent(
                                "<b style='color: #169201;'>Ciudadano: </b>" + item.properties.Ciudadano + "<br>" +
                                "<b>Direccion: </b>" + item.properties.Direccion + "<br>" +
                                "<b>Teléfonos: </b>" + item.properties.Celular + "-" + item.properties.Telefono + "<br>" +
                                "<b>Departamento: </b>" + item.properties.Dto + "<br>" +
                                "<b>Municipio: </b>" + item.properties.Municipio + "<br>" +
                                "<b>Correo: </b>" + item.properties.Correo + "<br>" +
                                "<b>Direccion GPS : </b><label id=\"lbDireccion\"><a href=\"javascript:ObtenerDireccion(" + item.properties.Latitud + "," + item.properties.Longitud + ")\">Buscar Dirección</a></label><br>" +
                                "<b>Codigo Postal: </b><label id=\"lblPostal\"></label><br>" +
                                "<b>Departamento: </b><label id=\"lblRegion\"></label><br>" +
                                "<b>Municipio: </b><label id=\"lblSubregion\"></label><br>" +
                                "<b>Ciudad: </b><label id=\"lblCity\"></label><br>" +
                                "<b>Barrio: </b><label id=\"lblBarrio\"></label><br>" +
                                "<b>Enviar a SECAD: </b><label style='color: #d50000;' id=\"lblCaso\"><a href=\"javascript:EnviarCasoSecad(" + item.properties.IdAlarma + ")\">Enviar a SECAD</a></label><br>" +
                                "<b>Apagar Alarma:</b><label style='color: #d50000;' id=\"lbAlarma\"><a href=\"javascript:ApagarAlarma(" + item.properties.IdGpsActual + ")\">Apagar</a></label><br>"
                            );
                            pointGraphic.setInfoTemplate(infoTemplate);
                            map.graphics.add(pointGraphic);
                        });
                        ///Fin alarmas
                    }
                    else {
                        MensajeInformacion("Consulta de ImeiTracking", "No hay datos retornados en la consulta");
                        domStyle.set(dom.byId("botonTabla"), "display", "none");
                    }
                    waitingDialog.hide();
                },
                function (error) {
                    MensajeInformacion("Consulta de ImeiTracking", "No hay datos retornados en la consulta");
                    waitingDialog.hide();
                }
            );
        }

        function addGeoJsonLayerUdp(url) {
            var request = Request({
                url: url,
            });
            request.then(
                function (data) {
                    if (data.HayDatos) {
                        /// Puntos
                        var capaPuntos = new GeoJsonLayer({
                            data: data.Puntos
                        });

                        var puntos = new Array();
                        var marker_green = new PictureMarkerSymbol("../../img/policia_1.png", 43, 43);
                        var marker_yellow = new PictureMarkerSymbol("../../img/policia_2.png", 43, 43);
                        var marker_red = new PictureMarkerSymbol("../../img/policia_3.png", 43, 43);
                        var marker_gray = new PictureMarkerSymbol("../../img/policia_4.png", 43, 43);
                        var marker_alarma = new PictureMarkerSymbol("../../img/alarma.gif", 200, 80);

                        var wgs84 = new SpatialReference({ wkid: 4326 });
                        var geometry = new Point(wgs84);

                        array.forEach(data.Puntos.features, function (item) {
                            var point = new Point(Number(item.geometry.coordinates[0]), Number(item.geometry.coordinates[1]));

                            if (item.properties.DuracionT > 799) {
                                var pointGraphic = new Graphic(point, marker_gray);
                            } else {
                                switch (item.properties.DentroCuadrante) {
                                    case 1:
                                        var pointGraphic = new Graphic(point, marker_green);
                                        break;
                                    case 0:
                                        var pointGraphic = new Graphic(point, marker_red);
                                        break;
                                    default:
                                        var pointGraphic = new Graphic(point, marker_green);
                                        break;
                                }
                            }
                            var infoTemplate = new InfoTemplate();
                            infoTemplate.setTitle("Información de funcionario");
                            infoTemplate.setContent("<table><tr><td rowspan='5'><img src='https://srvpqrs.policia.gov.co:9104/sivicc_rpt/picture.aspx?DocID=" + item.properties.IdentificacionEncry + "&Token=Mxl7995Julabdfjughyts1*_58$$" + "' style='border-radius:50%' width='120px' height='150px' align='center'></td><td><b>Funcionario: </b>" + item.properties.NombresApellidos + "</td></tr><tr><td ><b>Cédula: </b>" + item.properties.Identificacion + "</td></tr><tr><td ><b>IMEI: </b>" + item.properties.Imei + "</td></tr><tr><td><b style='color: #169201;'>Sesion PDA Activa:</b>" + item.properties.SesionActiva + "</td></tr><tr><td><b style='color: #169201;'>Tiempo Permanencia:</b>" + item.properties.Duracion + "</td></tr></table><br>"
                                + "<b style='color: #169201;'>Cuadrante: " + item.properties.Cuadrante + "</b><br>"
                                + "<b>Dentro del Cuadrante: </b>" + item.properties.DentroCuadranteText + "<hr><br>"
                                + "<b>Fecha: </b>" + item.properties.Fecha + "<br>"
                                + "<b>Velocidad: </b>" + item.properties.Velocidad + "<br>"
                                + "<b>Nivel de Bateria: </b>" + item.properties.NivelBateria + "<br>"
                                + "<b>Unidad:</b>" + item.properties.DependenciaDescripcion + "<br>"
                                + "<b>Celular Cuadrante: </b>" + item.properties.CelularCuadrante + "<br>"
                                + "<b>Serial del dispositivo: </b>" + item.properties.DispositivoSerie + "<br>"
                                + "<b>Marca/Modelo: </b>" + item.properties.DispositivoMarcaModelo + "<br>"
                                + "<b>Versión APK: </b>" + item.properties.VersionApk + "<br>"
                                + "<b>Direccion: </b><label id=\"lbDireccion\"><a href=\"javascript:ObtenerDireccion(" + item.properties.Latitud + "," + item.properties.Longitud + ")\">Buscar Dirección</a></label><br>"
                            );
                            pointGraphic.setInfoTemplate(infoTemplate);
                            map.graphics.add(pointGraphic);
                        });
                        capaPuntos.on("update-end", function (e) {
                            if (data.Puntos.features.length > 1) {
                                // map.setExtent(e.target.extent.expand(1.2));
                            } else {
                                map.centerAndZoom(new Point(data.Puntos.features[0].geometry.coordinates[0], data.Puntos.features[0].geometry.coordinates[1]), 15);
                            }
                        });
                        map.addLayer(capaPuntos);
                        map.removeLayer(capaPuntos);

                        //Puntos Planes
                        var capaPuntosPlanes = new GeoJsonLayer({
                            data: data.PuntosPlanes
                        });
                        var puntosPlanes = new Array();
                        var Baliza = new PictureMarkerSymbol("../../img/Baliza.gif", 50, 50);
                        var Bancos = new PictureMarkerSymbol("../../img/Bancos.png", 40, 40);
                        var Balnearios = new PictureMarkerSymbol("../../img/Balnearios.png", 40, 40);
                        var Campanas = new PictureMarkerSymbol("../../img/Estandar.png", 40, 40);
                        var Comercio = new PictureMarkerSymbol("../../img/Hurto.png", 13, 13);
                        var Cierre = new PictureMarkerSymbol("../../img/Cierre.png", 40, 40);
                        var Colegios = new PictureMarkerSymbol("/Sivicc2.Web.Administracion/Content/img/Colegios.png", 40, 40);
                        var Fincas = new PictureMarkerSymbol("../../img/Estandar.png", 40, 40);
                        var Iglesias = new PictureMarkerSymbol("../../img/Iglesias.png", 40, 40);
                        var Paraderos = new PictureMarkerSymbol("../../img/Paraderos.png", 40, 40);
                        var Parqueaderos = new PictureMarkerSymbol("../../img/Parqueaderos.png", 40, 40);
                        var Parques = new PictureMarkerSymbol("../../img/Parques.png", 40, 40);
                        var Puentes = new PictureMarkerSymbol("../../img/Puentes.png", 40, 40);
                        var Control = new PictureMarkerSymbol("../../img/PuestoControl.png", 40, 40);
                        var Supermercados = new PictureMarkerSymbol("../../img/Marcado.png", 40, 40);
                        var Transporte = new PictureMarkerSymbol("../../img/Masivo.png", 40, 40);
                        var Cicla = new PictureMarkerSymbol("../../img/Cicla.png", 40, 40);
                        var VendedorAmbulante = new PictureMarkerSymbol("../../img/VendedorAmbulante.png", 40, 40);
                        var Playas = new PictureMarkerSymbol("../../img/Playas.png", 40, 40);
                        var Perifoneo = new PictureMarkerSymbol("../../img/Perifoneo.png", 40, 40);
                        var ComercioBici = new PictureMarkerSymbol("../../img/ComercioBici.png", 40, 40);
                        var Cementerios = new PictureMarkerSymbol("../../img/Cementerios.png", 40, 40);

                        var wgs84s = new SpatialReference({ wkid: 4326 });
                        var geometrys = new Point(wgs84s);
                        array.forEach(data.PuntosPlanes.features, function (item) {
                            var points = new Point(Number(item.properties.Longitud.replace(",", ".")), Number(item.properties.Latitud.replace(",", ".")));
                            var pointAttributess = { city: "Albuquerque", state: "New Mexico" };
                            //TODO: Aca se debe realizar el switch case para pintar marcadores según el grupo del delito.
                            switch (item.properties.Tipoplan) {
                                case "Baliza":
                                    var pointGraphics = new Graphic(points, Baliza, pointAttributess);
                                    break;
                                case "Bancos":
                                    var pointGraphics = new Graphic(points, Bancos, pointAttributess);
                                    break;
                                case "Balnearios":
                                    var pointGraphics = new Graphic(points, Balnearios, pointAttributess);
                                    break;
                                case "Campañas Prevención":
                                    var pointGraphics = new Graphic(points, Campanas, pointAttributess);
                                    break;
                                case "Centros Comerciales":
                                    var pointGraphics = new Graphic(points, Comercio, pointAttributess);
                                    break;
                                case "Cierre Establecimientos Públicos":
                                    var pointGraphics = new Graphic(points, Cierre, pointAttributess);
                                    break;
                                case "Colegios":
                                    var pointGraphics = new Graphic(points, Colegios, pointAttributess);
                                    break;
                                case "Fincas":
                                    var pointGraphics = new Graphic(points, Fincas, pointAttributess);
                                    break;
                                case "Iglesias":
                                    var pointGraphics = new Graphic(points, Iglesias, pointAttributess);
                                    break;
                                case "Paradero de Buses":
                                    var pointGraphics = new Graphic(points, Paraderos, pointAttributess);
                                    break;
                                case "Parqueaderos":
                                    var pointGraphics = new Graphic(points, Parqueaderos, pointAttributess);
                                    break;
                                case "Parques":
                                    var pointGraphics = new Graphic(points, Parques, pointAttributess);
                                    break;
                                case "Puentes Peatonales":
                                    var pointGraphics = new Graphic(points, Puentes, pointAttributess);
                                    break;
                                case "Puesto de Control":
                                    var pointGraphics = new Graphic(points, Control, pointAttributess);
                                    break;
                                case "Supermercados":
                                    var pointGraphics = new Graphic(points, Supermercados, pointAttributess);
                                    break;
                                case "Transporte Masivo":
                                    var pointGraphics = new Graphic(points, Transporte, pointAttributess);
                                    break;
                                case "Ciclo Rutas":
                                    var pointGraphics = new Graphic(points, Cicla, pointAttributess);
                                    break;
                                case "Ciclo Via":
                                    var pointGraphics = new Graphic(points, Cicla, pointAttributess);
                                    break;
                                case "Playas":
                                    var pointGraphics = new Graphic(points, Playas, pointAttributess);
                                    break;
                                case "Vendedores Ambulantes":
                                    var pointGraphics = new Graphic(points, VendedorAmbulante, pointAttributess);
                                    break;
                                case "Comercio de Bicicletas":
                                    var pointGraphics = new Graphic(points, ComercioBici, pointAttributess);
                                    break;
                                case "Perifonéo":
                                    var pointGraphics = new Graphic(points, Perifoneo, pointAttributess);
                                    break;
                                case "Control Cementerios":
                                    var pointGraphics = new Graphic(points, Cementerios, pointAttributess);
                                    break;
                                default:
                                    var pointGraphics = new Graphic(points, Fincas, pointAttributess);
                                    break;
                            }
                            map.graphics.add(pointGraphics);

                            var infoTemplates = new InfoTemplate();
                            infoTemplates.setTitle(item.properties.Tipoplan);
                            infoTemplates.setContent(
                                "<b style='color: #169201;'>Plan: </b>" + item.properties.Tipoplan + "<br>" +
                                "<b>Jefe Servicio:</b>" + item.properties.Jefe + "<br>" +
                                "<b>Celular:</b>" + item.properties.Celular + "<br>" +
                                "<b>Fecha Inicio:</b>" + item.properties.Inicio + "<br>" +
                                "<b>Fecha Modifica:</b>" + item.properties.Modifica + "<br>" +
                                "<b>Dispositivo:</b>" + item.properties.Dispositivo + "<br>");

                            pointGraphics.setInfoTemplate(infoTemplates);
                        });
                        ///Fin Planes

                        //Puntos Alarmas
                        var capaPuntosAlarmas = new GeoJsonLayer({
                            data: data.PuntosAlarmas
                        });

                        array.forEach(data.PuntosAlarmas.features, function (item) {
                            var point = new Point(Number(item.geometry.coordinates[0]), Number(item.geometry.coordinates[1]));

                            var pointGraphic = new Graphic(point, marker_alarma);


                            var infoTemplate = new InfoTemplate();
                            infoTemplate.setTitle("Esta es una Emergencia");
                            infoTemplate.setContent(
                                "<b style='color: #169201;'>Ciudadano: </b>" + item.properties.Ciudadano + "<br>" +
                                "<b>Direccion: </b>" + item.properties.Direccion + "<br>" +
                                "<b>Teléfonos: </b>" + item.properties.Celular + "-" + item.properties.Telefono + "<br>" +
                                "<b>Departamento: </b>" + item.properties.Dto + "<br>" +
                                "<b>Municipio: </b>" + item.properties.Municipio + "<br>" +
                                "<b>Correo: </b>" + item.properties.Correo + "<br>" +
                                "<b>Direccion GPS : </b><label id=\"lbDireccion\"><a href=\"javascript:ObtenerDireccion(" + item.properties.Latitud + "," + item.properties.Longitud + ")\">Buscar Dirección</a></label><br>" +
                                "<b>Codigo Postal: </b><label id=\"lblPostal\"></label><br>" +
                                "<b>Departamento: </b><label id=\"lblRegion\"></label><br>" +
                                "<b>Municipio: </b><label id=\"lblSubregion\"></label><br>" +
                                "<b>Ciudad: </b><label id=\"lblCity\"></label><br>" +
                                "<b>Barrio: </b><label id=\"lblBarrio\"></label><br>" +
                                "<b>Enviar a SECAD: </b><label style='color: #d50000;' id=\"lblCaso\"><a href=\"javascript:EnviarCasoSecad(" + item.properties.IdAlarma + ")\">Enviar a SECAD</a></label><br>" +
                                "<b>Apagar Alarma:</b><label style='color: #d50000;' id=\"lbAlarma\"><a href=\"javascript:ApagarAlarma(" + item.properties.IdGpsActual + ")\">Apagar</a></label><br>"
                            );
                            pointGraphic.setInfoTemplate(infoTemplate);
                            map.graphics.add(pointGraphic);
                        });
                        ///Fin alarmas
                    }
                    else {
                        MensajeInformacion("Consulta de ImeiTracking", "No hay datos retornados en la consulta");
                        domStyle.set(dom.byId("botonTabla"), "display", "none");
                    }
                    waitingDialog.hide();
                },
                function (error) {
                    MensajeInformacion("Consulta de ImeiTracking", "No hay datos retornados en la consulta");
                    waitingDialog.hide();
                }
            );
        }

        function BuscarDatos() {
            removeAllLayers();
            //addGeoJsonLayer("/SECADPLUS/VisorGeografico/ImeiTracking/ObtenerImeiTracking/?" cambio rmr 06-01-2020
            addGeoJsonLayer("/VisorGeografico/ImeiTracking/ObtenerImeiTracking/?"
                + "&v_identificacion=" + encodeURI(dom.byId("Identificacion").value)
                + "&v_cuadrante=" + encodeURI(dom.byId("ddlCuadrante").value)
                + "&v_region=" + encodeURI(dom.byId("ddlRegion").value)
                + "&v_unidad=" + encodeURI(dom.byId("ddlUnidad").value)
                + "&v_dependencia=" + encodeURI(dom.byId("ddlDependencia").value)
            );
        }

        function BuscarDatosUdp() {
            removeAllLayers();
            //addGeoJsonLayerUdp("/SECADPLUS/VisorGeografico/ImeiTracking/ObtenerImeiTracking/?" cambio rmr 06-01-2020
            addGeoJsonLayerUdp("/VisorGeografico/ImeiTracking/ObtenerImeiTracking/?"
                + "&v_identificacion=" + encodeURI(dom.byId("Identificacion").value)
                + "&v_cuadrante=" + encodeURI(dom.byId("ddlCuadrante").value)
                + "&v_region=" + encodeURI(dom.byId("ddlRegion").value)
                + "&v_unidad=" + encodeURI(dom.byId("ddlUnidad").value)
                + "&v_dependencia=" + encodeURI(dom.byId("ddlDependencia").value)
            );
        }

        on(dom.byId("btnBuscar"), "click", BuscarDatos);
        var tid = setInterval(BuscarDatosUdp, 50000);
    }
);

function ObtenerDireccion(Latitud, Longitud) {
    $("#lbDireccion").html("Buscando Dirección...");
    $.ajax({
        url: "https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/reverseGeocode?f=pjson&featureTypes=&location=" + Longitud + "," + Latitud,
        type: "POST",
        crossDomain: true,
        dataType: "json",
        success: function (response) {
            $("#lbDireccion").html(response.address.Address);
            $("#lblPostal").html(response.address.Postal);
            $("#lblRegion").html(response.address.Region);
            $("#lblSubregion").html(response.address.Subregion);
            $("#lblCity").html(response.address.City);
            $("#lblBarrio").html(response.address.Neighborhood);
        },
        error: function (xhr, status) {
            $("#lbDireccion").html("No se encontró una dirección");
        }
    });
}

function SeleccionarPunto(indice) {
    $("[id^='graphicsLayer'] text:eq(" + indice + ")").addClass("circulo-resaltado");
    var posicion = $("[id^='graphicsLayer'] text:eq(" + indice + ")").position();
    var alto = $("#indicadorMapa").height();
    var ancho = $("#indicadorMapa").width();

    $("#indicadorMapa").css({ top: posicion.top - alto, left: posicion.left - (ancho / 2) + 5 });
    $("#indicadorMapa").show();
}
function SeleccionarPuntoPermanente(elemento, indice) {
    $("[id^='graphicsLayer'] text:eq(" + indice + ")").click();
    var posicion = $("[id^='graphicsLayer'] text:eq(" + indice + ")").position();
    var alto = $("#indicadorMapaSeleccionado").height();
    var ancho = $("#indicadorMapaSeleccionado").width();

    $("#indicadorMapaSeleccionado").css({ top: posicion.top - alto, left: posicion.left - (ancho / 2) + 5 });
    $("#indicadorMapaSeleccionado").show();
    $(".puntoTablaSeleccionado").removeClass("puntoTablaSeleccionado");
    $(elemento).addClass("puntoTablaSeleccionado");
}

function DeseleccionarPunto(indice) {
    $("[id^='graphicsLayer'] text:eq(" + indice + ")").removeClass("circulo-resaltado");
    $("#indicadorMapa").hide();
}
function DeseleccionarPuntoPermanente() {
    $("#indicadorMapaSeleccionado").hide();
    $(".puntoTablaSeleccionado").removeClass("puntoTablaSeleccionado");
}


function MostrarDetallesTabla(indice) {
    $(".tabla-datos-ocultos").hide();
    $("#tabla_datos_" + indice).show();
}

function VerHistorialSimCard(Imei) {
    AbrirFormularioModal("VisorGeografico", "ImeiTracking", "VerHistorialSimCard", { Imei: Imei }, "normal");
}
function CentrarDireccion() {
    $("#lbBusquedaDireccion").html("Buscando Dirección...");
    var ciudad = $("#buscarCiudad").val();
    var direccion = $("#buscarDireccion").val();
    $.ajax({
        url: "https://gis.policia.gov.co:6443/arcgis/rest/services/STREETMAP/Geocode_Ponal/GeocodeServer/geocodeAddresses?addresses={%22records%22:[{%22attributes%22:{%22OBJECTID%22:1,%22Address%22:%22" + encodeURI(direccion) + "%22,%22City%22:%22" + encodeURI(ciudad) + "%22}}]}&sourceCountry=CO&f=pjson",
        type: "POST",
        crossDomain: true,
        dataType: "json",
        success: function (response) {
            if (isNaN(response.locations[0].location.x)) {
                $("#lbBusquedaDireccion").html("No se encontró una dirección");
            } else {
                EstablecerUbicacion(response.locations[0].location.x, response.locations[0].location.y);
                $(".hide-direccion-link").click();
            }
        },
        error: function (xhr, status) {
            alert("error");
        }
    });
}

function ApagarAlarma(v_id) {
    $.ajax({
        url: "/VisorGeografico/ImeiTracking/ApagarAlarma/?" + "v_id=" + v_id,
        type: "GET",
        dataType: "json",
        success: function (response) {
            $("#lbAlarma").html("La alarma ha sido desactivada, en breve desaparecerá del mapa");
            MensajeInformacion("Señor(a) Funcionario(a:)", "La alarma ha sido desactivada, en breve desaparecerá del mapa");
        },
        error: function (xhr, status) {
            $("#lbAlarma").html("No es posible desactivar la alarma");
            MensajeInformacion("Señor(a) Funcionario(a:)", "No es posible desactivar la alarma");
        }
    });
}

function EnviarCasoSecad(v_id) {
    $.ajax({
        url: '/VisorGeografico/ImeiTracking/EnviarCasoSecad/',
        data: { "v_id": v_id, "v_direccion": $("#lbDireccion").text(), "postal": $("#lblPostal").text(), "p_dto": $("#lblRegion").text(), "p_municipio": $("#lblSubregion").text(), "p_ciudad": $("#lblCity").text(), "P_Barrio": $("#lblBarrio").text() },
        type: "GET",
        dataType: "json",
        success: function (response) {
            MensajeInformacion("El caso ha sido enviado al SECAD, ahora proceda a desactivar la alarma, Caso No: " + response.message);
            $("#lblCaso").html("El caso ha sido enviado al SECAD, ahora proceda a desactivar la alarma, Caso No: " + response.message);
        },
        error: function (xhr, status) {
            $("#lblCaso").html("No es posible enviar el Caso al SECAD");
            MensajeInformacion("Señor(a) Funcionario(a:)", "No es posible enviar el Caso al SECAD");
        }
    });
}

function EstablecerUbicacion(longitud, latitud) {
    var point = new esri.geometry.Point({
        latitude: latitud,
        longitude: longitud
    });
    map.centerAndZoom(point, 15);
}