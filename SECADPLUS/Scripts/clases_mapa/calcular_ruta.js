var use_google = false;//define si usa servicios de google (true) o servicios de arcgis(false) para calculo de rutas
var origen = Array();
var destino = Array();

$(document).ready(function() {
    $( "#calcular_ruta_button" ).click(function() {
        origen.direccion = $('#direccion_punto_origen').val();
        origen.ciudad = $('#ciudad_punto_origen').val();
        destino.direccion = $('#direccion_punto_destino').val();
        destino.ciudad = $('#ciudad_punto_destino').val();
        if(origen.direccion === ""){
            alert("Debe ingresar dirección de origen");
        }
        else if(origen.ciudad === ""){
            alert("Debe ingresar ciudad de origen");
        }
        else if(destino.direccion === ""){
            alert("Debe ingresar dirección de destino");
        }
        else if(destino.ciudad === ""){
            alert("Debe ingresar ciudad de destino");
        }
        else {
            iniciarCalculoRuta();
        }
    });
});

function iniciarCalculoRuta(){
    console.log("iniciarCalculoRuta");
    eliminarGraficasUltimaRuta();
    if(use_google===true){
        //Google specs start
        //var directionsService = new google.maps.DirectionsService();
        //var google_request = {
        //    origin: origen.direccion+","+origen.ciudad+", Colombia",
        //    destination: destino.direccion+","+destino.ciudad+", Colombia",
        //    travelMode: google.maps.TravelMode.TRANSIT
        //};
        //ActivarLoader("mapa_policia");
        //directionsService.route(google_request, function(result, status) {
        //    if (status == 'OK') {
        //        console.log("obtiene respuesta de rutas de google");
        //        var puntos_ruta_response = result["routes"][0]["legs"][0]["steps"];
        //        dibujarRutaOptima(puntos_ruta_response);
        //    }
        //    else{
        //        console.log("error obteniendo respuesta de rutas de google");
        //        console.log(result);
        //    }
        //    InactivarLoader("mapa_policia");
        //});
    }
    else{
        //PRIMERO OBTIENE LATITUD Y LONGITUD DE LOS PUNTOS
        ActivarLoader("mapa_policia");
        obtenerGeoreferenciacionDirecta('origen');
    }
}

function typeOf (obj) {
  return {}.toString.call(obj).split(' ')[1].slice(0, -1).toLowerCase();
}

function obtenerGeoreferenciacionDirecta(ref) {
    console.log("obtenerGeoreferenciacionDirecta para "+ref);
    var punto = "";
    if(ref == "origen"){
        punto = origen;
    }
    else if(ref == "destino"){
        punto = destino;
    }
    console.log("evaluando geocodificacion directa para:" + punto.direccion + "," + punto.ciudad);
    var request = {
        "address":punto.direccion,
        "city":punto.ciudad,
        "f":"pjson"
    };
    $.ajax({
        url: 'https://gis.policia.gov.co:6443/arcgis/rest/services/STREETMAP/Geocode_Ponal/GeocodeServer/findAddressCandidates?',
        type: 'GET',
        data: request ,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (response) {
            var response_json = JSON.parse(response);
            var georeferencia_valida = false;
            if(typeOf(response_json) == "object"){
                var candidates = response_json.candidates;
                if(typeOf(candidates) == "array"){
                    var candidate = candidates[0];
                    if(typeOf(candidates) == "array"){
                        var candidate = candidates[0];
                        if(typeOf(candidate) == "object"){
                            var location = candidate.location;
                            if (typeOf(location) == "object") {
                                if(ref=="origen"){
                                    origen.latitud = location.y;
                                    origen.longitud = location.x;
                                    console.log("Obtiene punto de origen");
                                    obtenerGeoreferenciacionDirecta("destino");
                                }
                                else if(ref=="destino"){
                                    console.log("Obtiene punto de destino");
                                    destino.latitud = location.y;
                                    destino.longitud = location.x;
                                    //destino.latitud = 4.621877;
                                    //destino.longitud = -74.085608;
                                }
                                georeferencia_valida = true;
                            }
                        }
                    }
                }
            }
            if(georeferencia_valida===false){
                alert("La dirección de "+ref+" ingresada no fue encontrada.");
                InactivarLoader("mapa_policia");
            }
            if(georeferencia_valida===true && ref=="destino"){
                solicitarRutaServicioArcgis();
            }
        },
        error: function () {
            alert("No fue posible localizar el "+ref+". Por favor intente con otra palabra");
            InactivarLoader("mapa_policia");
        }
    });
}

function solicitarRutaServicioArcgis(){
    console.log("solicitarRutaServicioArcgis");
    //LLAMA A SERVICIO DE ARCGIS DE RUTAS
        var stops = '\n\
        { "features"  : [\n\
              {\n\
                "geometry" : {"x" : '+origen.longitud+', "y" : '+origen.latitud+'},\n\
                "attributes" : {"Name" : "From", "RouteName" : "Ruta A"}\n\
              },\n\
              {\n\
                "geometry" : {"x" : '+destino.longitud+', "y" : '+destino.latitud+'},\n\
                "attributes" : {"Name" : "To", "RouteName" : "Ruta A"}\n\
              }\n\
          ]\n\
        }\n\
        ';
        console.log("stops:");
        console.log(stops);
        var request = {
            "stops":stops,
            "barriers":'',
            "polylineBarriers":'',
            "polygonBarriers":'',
            "outSR":4326,
            "ignoreInvalidLocations":true,
            "accumulateAttributeNames":'',
            "impedanceAttributeName":'TravelTime',
            "restrictionAttributeNames": '',
            "attributeParameterValues": '',
            "restrictUTurns": '',
            "useHierarchy":true,
            "returnDirections":true,
            "returnRoutes":true,
            "returnStops":false,
            "returnBarriers":false,
            "returnPolylineBarriers":false,
            "returnPolygonBarriers":false,
            "directionsLanguage": '',
            "directionsStyleName":'',
            "outputLines": '',
            "findBestSequence":false,
            "preserveFirstStop":true,
            "preserveLastStop":true,
            "useTimeWindows":false,
            "startTime": '',
            "outputGeometryPrecision": '',
            "outputGeometryPrecisionUnits": '',
            "directionsTimeAttributeName": 'TravelTime',
            "directionsLengthUnits": '',
            "f":'pjson'
        };
        $.ajax({
            url: 'https://gis.policia.gov.co:6443/arcgis/rest/services/STREETMAP/RoutingPonal/NAServer/Route/solve?',
            type: 'GET',
            data: request ,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (response) {
                //console.log("Se obtiene respuesta de servicio de rutas");
                //console.log(response);
                var obtiene_ruta = false;
                var json_response = JSON.parse(response);
                if(typeOf(json_response)=="object"){
                    var routes = json_response.routes;
                    if(typeOf(routes)=="object"){
                        var features_array = routes.features;
                        if(typeOf(features_array)=="array"){
                            var features = features_array[0];
                            if(typeOf(features)=="object"){
                                var geometry = features.geometry;
                                if (typeOf(geometry) == "object") {
                                    var paths = geometry.paths;
                                    if (typeof (paths) == "array" || typeof (paths) == "object") {
                                        if (paths.length > 0) {
                                            var puntos_ruta_response = geometry.paths[0];
                                            dibujarRutaOptima(puntos_ruta_response);
                                            obtiene_ruta = true;
                                        }
                                        else {
                                            alert("La ruta de origen es la misma ruta de destino");
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                }
                if(obtiene_ruta === false){
                    alert("La ruta requerida no generó resultados, por favor intente con otras palabras para especificar el origen y el destino")
                }
                InactivarLoader("mapa_policia");
            },
            error: function () {
                alert("La ruta requerida no generó resultados, por favor intente con otras palabras para especificar el origen y el destino")
                InactivarLoader("mapa_policia");
            }
        });
}

function dibujarRutaOptima(puntos_ruta_response) {
    console.log("dibujarRutaOptima");
    console.log(puntos_ruta_response);
    var string_origen = origen.direccion+", "+origen.ciudad;
    var string_destino = destino.direccion+", "+destino.ciudad;
    var ruta = Array();//Array de arrays. Cada elemento tiene los �ndices "latitude" y "longitude"
    var xmin = 0;
    var ymin = 0;
    var xmax = 0;
    var ymax = 0;
    
    //punto de origen
    var datos_punto_origen = Array();
    datos_punto_origen.name = "Punto de origen";
    datos_punto_origen.info = string_origen;
    if(use_google){//ESTRUCTURA USANDO SERVICIO DE GOOGLE
        datos_punto_origen.latitud = puntos_ruta_response[0].start_point.lat();
        datos_punto_origen.longitud = puntos_ruta_response[0].start_point.lng();
    }
    else{//ESTRUCTURA USANDO SERVICIO DE ARCGIS
        var punto_inicial = puntos_ruta_response[0];
        datos_punto_origen.latitud = punto_inicial[1];
        datos_punto_origen.longitud = punto_inicial[0];
    }
    dibujarPunto(datos_punto_origen, true);
    //punto de destino
    var datos_punto_destino = Array();
    datos_punto_destino.name = "Punto de destino";
    datos_punto_destino.info = string_destino;
    if(use_google){//ESTRUCTURA USANDO SERVICIO DE GOOGLE
        datos_punto_destino.latitud = puntos_ruta_response[puntos_ruta_response.length-1].start_point.lat();
        datos_punto_destino.longitud = puntos_ruta_response[puntos_ruta_response.length-1].start_point.lng();
    }
    else{//ESTRUCTURA USANDO SERVICIO DE ARCGIS
        var punto_final = puntos_ruta_response[puntos_ruta_response.length-1];
        datos_punto_destino.latitud = punto_final[1];
        datos_punto_destino.longitud = punto_final[0];
    }
    dibujarPunto(datos_punto_destino, true);

    $.each(puntos_ruta_response, function(index, punto_ruta_response) {
        var lat = 0;
        var lng = 0;
        if(use_google){//ESTRUCTURA USANDO SERVICIO DE GOOGLE
            lat = punto_ruta_response.start_point.lat();
            lng = punto_ruta_response.start_point.lng();
        }
        else{//ESTRUCTURA USANDO SERVICIO DE ARCGIS
            lat = punto_ruta_response[1];
            lng = punto_ruta_response[0];
        }
        var lng_lat = [lng, lat];
        ruta.push(lng_lat);
        //set xmin
        if(xmin === 0){
            xmin = lng;
        }
        else if(xmin > lng){
            xmin = lng;
        }
        //set xmax
        if(xmax === 0){
            xmax = lng;
        }
        else if(xmax < lng){
            xmax = lng;
        } //set ymin
        if(ymin === 0){
            ymin = lat;
        }
        else if(ymin > lat){
            ymin = lat;
        }
        //set ymax
        if(ymax === 0){
            ymax = lat;
        }
        else if(ymax < lat){
            ymax = lat;
        }
    });
    var datos_polilinea = Array();
    datos_polilinea.name = "Ruta";
    datos_polilinea.info = string_origen+" => "+string_destino; 
    datos_polilinea.points = ruta;
    dibujarPolilinea(datos_polilinea, true);

    var coordenadas = Array();
    coordenadas.xmin = xmin;
    coordenadas.ymin = ymin;
    coordenadas.xmax = xmax;
    coordenadas.ymax = ymax;
    centrarMapaEnFiguras(coordenadas);
}