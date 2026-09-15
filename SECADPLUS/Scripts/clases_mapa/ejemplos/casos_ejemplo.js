function obtenerCasosEjemplo(){
    var casos = Array();
    casos.push(obtenerCasoEjemplo(1));//carga caso de ejemplo 1
    casos.push(obtenerCasoEjemplo(2));//carga caso de ejemplo 2
    casos.push(obtenerCasoEjemplo(3));//carga caso de ejemplo 3
    casos.push(obtenerCasoEjemplo(4));//carga caso de ejemplo 4
    casos.push(obtenerCasoEjemplo(1));//carga caso de ejemplo 1
    casos.push(obtenerCasoEjemplo(2));//carga caso de ejemplo 2
    casos.push(obtenerCasoEjemplo(3));//carga caso de ejemplo 3
    casos.push(obtenerCasoEjemplo(4));//carga caso de ejemplo 4
    casos.push(obtenerCasoEjemplo(1));//carga caso de ejemplo 1
    casos.push(obtenerCasoEjemplo(2));//carga caso de ejemplo 2
    casos.push(obtenerCasoEjemplo(3));//carga caso de ejemplo 3
    casos.push(obtenerCasoEjemplo(4));//carga caso de ejemplo 4
    return casos;
}

function obtenerCasoEjemplo(caso_id){
    var caso = Array();
    if(caso_id == 1){
        caso["id"] = caso_id
        caso["nombre"] = "Caso 1 (Figura con círculo)";
        caso["descripcion"] = "Hay un gato en el techo del Museo";
        caso["hora"] = "Abril 14 14:53";
        caso["color_icono"] = "orange";
        caso["color"] = "#a84e1d";//hexadecimal
    }
    else if(caso_id == 2){
        caso["id"] = caso_id;
        caso["nombre"] = "Caso 2 (Figura con semi arco)";
        caso["descripcion"] = "Hay un incendio en la fábrica";
        caso["hora"] = "13:15";
        caso["color_icono"] = "red";
        caso["color"] = "#a81d31";//hexadecimal
    }
    else if(caso_id == 3){
        caso["id"] = caso_id;
        caso["nombre"] = "Caso 3 (Figura con polígono)";
        caso["descripcion"] = "Hay un incendio en la fábrica";
        caso["hora"] = "13:15";
        caso["color_icono"] = "green";
        caso["color"] = "#1da894";//hexadecimal
    }
    else if(caso_id == 4){
        caso["id"] = caso_id;
        caso["nombre"] = "Caso 4 (Figura con ovalo)";
        caso["descripcion"] = "Hay un incendio en la fábrica";
        caso["hora"] = "13:15";
        caso["color_icono"] = "red";
        caso["color"] = "#a81d31";//hexadecimal
    }
    return caso;
}

function obtenerFigurasCaso(caso){
    if(caso["id"]==1){
        caso["circulos"] = obteneCoordenadasCirculosEjemplo();
    }
    else if(caso["id"]==2){
        caso["semi_arcos"] = obteneCoordenadasSemiArcosEjemplo();
    }
    else if(caso["id"]==3){
        caso["poligonos"] = obteneCoordenadasPoligonosEjemplo();
    }
    else if(caso["id"]==4){
        caso["ovalos"] = obteneCoordenadasOvalosEjemplo();
    }
    return caso;
}

function obteneCoordenadasCirculosEjemplo(){
    var circulos = Array();
    
    var circulo_uno = Array();
    circulo_uno["nombre"] = "Círculo de prueba";
    circulo_uno["info"] = "Información detallada del caso";
    circulo_uno["latitud"] = 5.8;
    circulo_uno["longitud"] = -76.5;
    circulo_uno["radio"] = 10000;
    circulos.push(circulo_uno);
    
    return circulos;
}

function obteneCoordenadasSemiArcosEjemplo(){
    var semi_arcos = Array();
    //Donut ONE
    var semi_arco_uno = Array();
    semi_arco_uno["nombre"] = "Semidona de prueba";
    semi_arco_uno["info"] = "Información detallada del caso";
    semi_arco_uno["latitud"] = 5.4;
    semi_arco_uno["longitud"] = -76.5;
    semi_arco_uno["in_radius"] = 5000;//distancia en metros
    semi_arco_uno["out_radius"] = 10000;//distancia en metros
    semi_arco_uno["start_angle"] = 45;
    semi_arco_uno["stop_angle"] = 90;
    semi_arcos.push(semi_arco_uno);
    
    return semi_arcos;
}

function obteneCoordenadasPoligonosEjemplo(){
    var poligonos = Array();
    
    //POLIGON ONE
    var poligono_uno = Array();
    poligono_uno["nombre"] = "Polígono de prueba";
    poligono_uno["info"] = "Información detallada del caso";
    poligono_uno["points"] = [
        [-76.0, 5.5],
        [-75.5, 5.5],
        [-75.3, 5.4],
        [-75.75, 5.3],
        [-76.2, 5.4],
        [-76.0, 5.5]
    ];
    poligonos.push(poligono_uno);
    
    return poligonos;
}

function obteneCoordenadasOvalosEjemplo(){
    var ovalos = Array();
    //ELLIPSE ONE
    var ovalo_uno = Array();
    ovalo_uno["nombre"] = "Ovalo de prueba";
    ovalo_uno["info"] = "Información detallada del caso";
    ovalo_uno["latitud"] = 5.8;
    ovalo_uno["longitud"] = -75.5;
    ovalo_uno["radius_minor"] = 5000;//en metros
    ovalo_uno["radius_major"] = 10000;//en metros
    ovalo_uno["angle"] = 290;//desde el eje x en el sentido inverso a las manesillas del reloj
    ovalos.push(ovalo_uno);
    
    return ovalos;
}