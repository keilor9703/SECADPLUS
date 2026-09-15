var alerta = "<div class='alert alert-warning alert-styled-left alert-arrow-left alert-bordered ' ><button type='button' class='close' data-dismiss='alert'><span>×</span><span class='sr-only'>Close</span></button><span class='text-semibold' > MENSAJE</span> </div>";
var alertaSuccess = "<div class='alert alert-success alert-styled-left alert-arrow-left alert-bordered ' ><button type='button' class='close' data-dismiss='alert'><span>×</span><span class='sr-only'>Close</span></button><span class='text-semibold' > MENSAJE</span> </div>";
var botonRefrescar = "<button type='button' class='btn btn-success btn-xs' onclick='CargaNivelX(_CODIGOPADRE, _NIVELPADRE, \"_NOMBREPADRE\", \"_DESCRIPCIONNIVEL\")' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para recargar'><b><i class='icon-reload-alt position-left'></i></b> </button>";
var botonBase = "<a href='URLRUTA'  target='_blank' class='btn btn-success btn-xs' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para crear nuevas unidades'><b><i class='glyphicon glyphicon-plus-sign position-left'></i></b>Crear unidades</a>";
var modeloFila = "<tr ><td></td><td><ul class='icons-list decad_menu'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><a href='URLEDITAR' target='_blank' class='btn btn-default '   data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para editar' ><i class='glyphicon glyphicon-edit'></i> Editar</a></li><li><button class='btn btn-default ' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para borrar' onclick='EliminarUnidad(ADDCODIGO, BORRANIVEL, ELCOD, \"NOMBRE\")'><i class='glyphicon glyphicon-trash'></i> Borrar</button></li><li><button type='button' class='btn btn-slate btn-labeled' onclick='ImprimirNivel(FUERZA,IMPCODIGO,IMPNIVEL)'>Imprimir</button></li></ul></li></ul></td><td onclick='CargaNivelX(INSCODIGO, NIVEL, \"NOMBRE\",\"_DESCRIPCIONNIVEL\")'>SIGLA</td><td>DESCRIPCION</td><td>OPERATIVA</td><td>VIGENTE</td><td>CODIGOX</td><td>LUGAR</td><td>_NIVEL</td><td>SIEDCO</td><td>SIATH</td><td>PADRE</td><td>ORDEN</td></tr>";
var modeloFilaComplemento = "<tr ><td></td><td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9 '></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><a href='URLMEDIOS' target='_blank' class='btn btn-default '   data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para gestionar medios' ><i class='glyphicon glyphicon-edit'></i> Medios</a></li><li><button type='button' class='btn btn-slate btn-labeled' onclick='AbrirModalImprimir(FUERZA,IMPCODIGO)'>Imprimir</button></li></ul></li></ul></td><td onclick='CargaNivelX(INSCODIGO, NIVEL, \"NOMBRE\",\"_DESCRIPCIONNIVEL\")'>SIGLA</td><td>DESCRIPCION</td><td>OPERATIVA</td><td>VIGENTE</td><td>CODIGOX</td><td>LUGAR</td><td>_NIVEL</td><td>SIEDCO</td><td>SIATH</td><td>PADRE</td><td>ORDEN</td></tr>";
var Variable = [];
var blnCargado = false;
var botonMedios = "<li><a href='URLMEDIOS' target='_blank' class='btn btn-default '   data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para gestionar medios' ><i class='glyphicon glyphicon-edit'></i> Medios</a></li>";
var urlImprimir = "../ReportesWeb/Reportes.aspx?REPORTE=UNIDADES&NIVEL=_UN&FUERZA=UF&CODIGO=UC";
var urlImprimirMedios = "../ReportesWeb/Reportes.aspx?REPORTE=MEDIOS&FUERZA=_UF&CODIGO=_UC&VIGENTE=_VG&OPERATIVA=_O&OPCION=_SE";
var botonEditar1 = "<a href='URLEDITAR' target='_blank' class='btn btn-default '   data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para editar' ><i class='glyphicon glyphicon-edit'></i> Editar</a>";
var botonEliminar1 = "<button class='btn btn-default ' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para borrar' onclick='EliminarUnidad(ADDCODIGO, BORRANIVEL, ELCOD, \"NOMBRE\")'><i class='glyphicon glyphicon-trash'></i> Borrar</button>";
var botonImprimir1 = " <button type='button' class='btn bg-slate btn-sm' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para imprimir documento' onclick='ImprimirNivel(FUERZA,IMPCODIGO,IMPNIVEL)'><b><i class='glyphicon glyphicon-print'></i></b> Imprimir</button>";
var botonImprimirMedio = " <button type='button' class='btn bg-slate btn-sm' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para imprimir documento' onclick='AbrirModalImprimir(FUERZA,IMPCODIGO)'><b><i class='glyphicon glyphicon-print'></i></b> Imprimir</button>";
var vigente = "S";
var operativa = "S";
for (tabla = 0 ; tabla <= 10; tabla++) {
    Variable[tabla] = 0;
}
function inicializarDatos() {
    for (tabla = 2 ; tabla <= 3; tabla++)
        AplicarPluginTabla('TBLNIVEL' + tabla);
}
function Seleccionar() {
    console.log("seleccionado!");
}
inicializarDatos();
//limpio las plantillas hijas
$(document).ready(function () {
    $("a[data-toggle=\"tab\"]").on("shown.bs.tab", function (e) {
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust();
    });
    $('input[name="cbxVigente"]').on('switchChange.bootstrapSwitch', function (event, state) {
        console.log(state);
        if (state) {
            vigente = "S";
        } else {
            vigente = "N";
        }
    });
    $('input[name="cbxOperativa"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            operativa = "S";
        } else {
            operativa = "N";
        }
    });
    GetFuerzaOrden();
    if (blnCargado == false) {
        blnCargado = true;
    }
    $("#MTDOFUERZA").change(function () {
        console.log(controlUnidades);
        var urlUnidadesConsulta = urlAction1;
        if (!controlUnidades) {
            urlUnidadesConsulta = urlNivelUnoActivos;
        }
        //$("#NIVELUNO").empty();
        limpiarTablas(2);
        for (tabla = 0 ; tabla <= 10; tabla++)
            Variable[tabla] = 0;
        var intIdPadre = { intCodigoPadre: $("#MTDOFUERZA").val() }
        $.ajax({
            type: "POST",
            url: urlUnidadesConsulta,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: intIdPadre,
            success: function (dataNivel1) {
                $("#NIVELUNO").empty();
                $("#NIVELUNO").append($('<option></option>').val(-1).html(""));
                $.each(dataNivel1, function (i, Nivel1) {
                    $("#NIVELUNO").append($('<option></option>').val(Nivel1.CODIGO).html(Nivel1.CODIGO + "-" + Nivel1.DESCRIPCION));
                });
                //Cargar botones del nivel 1
                GenerarBotonCreacion($("#MTDOFUERZA").val(), 0, 1);
            },
            error: function (ex) {
                alert('Fallo al cargar el nivel1.' + ex);
            }
        });
    })
    $("#NIVELUNO").change(function () {
        //        ActivarLoader("panelContenedor2");
        $("#contenedorEditar1").empty();
        $("#contenedorBorrar1").empty();
        $("#contenedorImprimir1").empty();
        var intIdPadre = $("#NIVELUNO").val();
        var descripcion = $("#NIVELUNO").val();
        var fuerza = $("#MTDOFUERZA").val();
        var nombreUnidad = $("#NIVELUNO").text();
        CargaNivelX(intIdPadre, "2", descripcion, nombreUnidad);
        if (controlUnidades) {
            //Editar
            $("#contenedorEditar1").html(botonEditar1.replace("URLEDITAR", urlEditar + "?intCodigo=" + intIdPadre + "&&btFuerza=" + fuerza));
            //borrar
            $("#contenedorBorrar1").html(botonEliminar1.replace().replace(/\NOMBRE/g, intIdPadre + "-" + descripcion).replace("ADDCODIGO", intIdPadre).replace("BORRANIVEL", parseInt(2)).replace("ELCOD", 0));
        } else {
            //Medios
            $("#contenedorEditar1").html(botonMedios.replace("URLMEDIOS", (urlVerPatrullas + "?intUnidadFuerza=" + fuerza + "&intUnidadCodigo=" + intIdPadre)));
        }
        //Imprimir
        if (controlUnidades) {
            $("#contenedorImprimir1").html(botonImprimir1.replace("FUERZA", fuerza).replace("IMPCODIGO", intIdPadre).replace("IMPNIVEL", 1));
        } else {
            $("#contenedorImprimir1").html(botonImprimirMedio.replace("FUERZA", fuerza).replace("IMPCODIGO", intIdPadre).replace("IMPNIVEL", 1));
        }
    })
});
//Permite crear de anera dinamica los botones de creación acorde a cada nivel seleccionado
function GenerarBotonCreacion(idFuerza, idPadre, Nivel, Nombre, Descripcion) {
    console.log("Genera");
    $("#spannivel" + Nivel).html(" " + Descripcion);
    if (controlUnidades) {
        var botonFinal = botonBase.replace("URLRUTA", (urlCreacion + "?strFuerza=" + idFuerza + "&intCodigo=" + idPadre + "&intNivel=" + Nivel));
    }
    var botonRFinal = botonRefrescar.replace("_CODIGOPADRE", idPadre).replace("_NIVELPADRE", Nivel).replace("_NOMBREPADRE", Nombre).replace("_DESCRIPCIONNIVEL", Descripcion);
    $("#contenedorCargarNivel" + Nivel).empty();
    $("#contenedorCargarNivel" + Nivel).html(botonRFinal);
    $("#contenedorNivel" + Nivel).empty();
    $("#contenedorNivel" + Nivel).html(botonFinal);
}
function GetFuerzaOrden() {
    var Data = ""
    var opcion = "<option value=\"VALOR\">NOMBRE</option>";
    var opciones = "";
    $("#MTDOFUERZA").empty();
    $("#fuerzaImprimir").empty();
    $.ajax({
        type: "POST",
        url: urlFuerzas,
        content: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultados) {
            if (resultados.result) {
                for (i = resultados.datos.length - 1; i >= 0; i--) {
                    opciones += opcion.replace("VALOR", resultados.datos[i].FUERZA).replace("NOMBRE", resultados.datos[i].DESCRIPCION);
                    //$("#fuerzaImprimir").append(OptionValue.replace("VALOR", dataFuerza[i].FUERZA).replace("NOMBRE", dataFuerza[i].DESCRIPCION));
                    //$("#MTDOFUERZA").append(OptionValue.replace("VALOR", dataFuerza[i].FUERZA).replace("NOMBRE", dataFuerza[i].DESCRIPCION));
                }
                $("#fuerzaImprimir").html(opciones);
                $("#MTDOFUERZA").html(opciones);
            } else {
                GestionarAlerta("warning", resultados.datos);
                location.href = resultados.urlInicio;
            }
        },
        error: function (ex) {
            GestionarAlerta("Warning", 'Fallo al cargar el nivel1.' + ex);
        }
    });
}
///Permite generar el reporte de un registro especifico
function ImprimirNivel(fuerza, codigo, nivel) {
    var url = urlImprimir.replace("_UN", nivel).replace("UF", fuerza).replace("UC", codigo);
    //console.log(url);
    window.open(url, '_blank');
}
function AbrirModalImprimir(fuerza, codigo) {
    $("#fuerzaImprimir").val(fuerza);
    $("#fuerzaImprimir").trigger("change");
    $("#txtUnidad").val(codigo);
    $("#param_medios").modal("show");
}
///Permite generar el reporte de un registro especifico
function ImprimirMedio() {
    var fuerza = $("#fuerzaImprimir").val();
    var codigo = $("#txtUnidad").val();
    var unidad = $("#txtUnidad").val();;
    var opcion = $("#listaOpcion").val();

    var url = urlImprimirMedios.replace("_VG", vigente).replace("_O", operativa).replace("_UF", fuerza).replace("_UC", codigo).replace("_SE", opcion);
    console.log(url);
    window.open(url, '_blank');
}
function ObtenerFila(control, datos,padre, urlEditar, urlMedios) {
    var urlCuadrantes = urlVerCuadrantes.replace(/amp;/g, "") + "?strUnidad="+datos.DESCRIPCION+"&strCodigo="+datos.CODIGO;
    if (control) {
        return "<tr ><td></td><td><ul class='icons-list decad_menu'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9'></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><a href='" + urlEditar + "' target='_blank' class='btn btn-default '   data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para editar' ><i class='glyphicon glyphicon-edit'></i> Editar</a></li><li><button class='btn btn-default ' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para borrar' onclick='EliminarUnidad(" + datos.CODIGO + ", " + datos.NIVEL + ", " + datos.padre + ", \" - " + datos.CODIGO + "\")'><i class='glyphicon glyphicon-trash'></i> Borrar</button></li><li><button type='button' class='btn btn-default' onclick='ImprimirNivel(" + $("#MTDOFUERZA").val() + "," + datos.CODIGO + "," + datos.NIVEL + ")'>Imprimir</button></li><li><a type='button' class='btn btn-default' href='" + urlCuadrantes + "' target='_blank'><i class='icon icon-eye'></i>Ver Cuadrantes</a></li></ul></li></ul></td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.SIGLA + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.DESCRIPCION + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>OPERATIVA</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>VIGENTE</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.CODIGO + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.LUGAR + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.NIVEL + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.SIEDCO + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.SIATH + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.PADRE + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.ORDEN + "</td></tr>";
    } else {
        return "<tr ><td></td><td><ul class='icons-list'><li class='dropdown'><button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'><i class='icon-menu9 '></i></button><ul class='dropdown-menu secad dropdown-menu-left'><li><a href='" + urlMedios + "' target='_blank' class='btn btn-default '   data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para gestionar medios' ><i class='glyphicon glyphicon-edit'></i> Medios</a></li><li><button type='button' class='btn btn-slate btn-labeled' onclick='AbrirModalImprimir(" + $("#MTDOFUERZA").val() + "," + datos.CODIGO + ")'>Imprimir</button></li></ul></li></ul></td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \" - " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.SIGLA + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.DESCRIPCION + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>OPERATIVA</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>VIGENTE</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.CODIGO + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.LUGAR + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.NIVEL + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.SIEDCO + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>SIATH</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.PADRE + "</td><td onclick='CargaNivelX(" + datos.CODIGO + ", " + (parseInt(datos.NIVEL) + 1) + ", \"- " + datos.CODIGO + "\", \"" + datos.DESCRIPCION + "\")'>" + datos.ORDEN + "</td></tr>";
    }
}
//Actualiza las tablas de un nivel especifico
function CargaTabla(dataNivelx, nivel, IdPadre) {
    if (controlUnidades) {
        fila = modeloFila;
    } else {
        fila = modeloFilaComplemento;
    }
    if ($.fn.DataTable.isDataTable("#TBLNIVEL" + nivel)) {
        $("#TBLNIVEL" + nivel).DataTable().destroy();
    }
    var tblId = "#TBLNIVEL" + nivel + " tbody";
    $.when($.each(dataNivelx, function (i, item) {
        filaModif = ObtenerFila(controlUnidades, dataNivelx[i], IdPadre, (urlEditar + "?intCodigo=" + dataNivelx[i].CODIGO + "&&btFuerza=" + dataNivelx[i].FUERZA), (urlVerPatrullas + "?intUnidadFuerza=" + dataNivelx[i].FUERZA + "&intUnidadCodigo=" + dataNivelx[i].CODIGO));
       
        if (dataNivelx[i].OPERATIVA === "S") {
            filaModif = filaModif.replace("OPERATIVA", "<span class='label label-success'>SI</span>");
        } else {
            filaModif = filaModif.replace("OPERATIVA", "<span class='label label-danger'>NO</span>");
        }
        if (dataNivelx[i].VIGENTE === "S") {
            filaModif = filaModif.replace("VIGENTE", "<span class='label label-success'>SI</span>");
        } else {
            filaModif = filaModif.replace("VIGENTE", "<span class='label label-danger'>NO</span>");
        }

        $(tblId).append(filaModif).show();
    })).then(function () { InactivarLoader("panelContenedor" + nivel); });

    AplicarPluginTabla("TBLNIVEL" + nivel);
}
//Limpia las tablas que se requieran
function limpiarTablas(idNivel) {
    var rowCount = 0
    for (nivel = parseInt(idNivel) ; nivel <= 10; nivel++) {
        rowCount = $('#TBLNIVEL' + nivel).length;
        if (rowCount > 0) {
            if ($.fn.DataTable.isDataTable('#TBLNIVEL' + nivel)) {
                $('#TBLNIVEL' + nivel).DataTable().clear().draw();
            }
            $('#TBLNIVEL' + nivel + " tbody").empty();
            $('#TBLNIVEL' + nivel).DataTable().clear().draw();
            //AplicarPluginTabla('TBLNIVEL' + nivel);
        }
        $("#contenedorCargarNivel" + nivel).empty();
        $("#contenedorNivel" + nivel).empty();
        $("#spannivel" + nivel).empty();
    }
}
//Consulta por AJAX la información de un nivel especifíco
function CargaNivelX(idPadre, idNivel, Nombre, descripcion) {
    $("#ALERTA").empty();
    var urlConsulta = urlUnidades;
    if (!controlUnidades) {
        urlConsulta = urlUnidadesActivos;
    }
    //Evalúa cual pestaña debe habilitar
    switch (idNivel) {
        case 4:
            $("#niv45").trigger("click");
            break;
        case 6:
            $("#niv67").trigger("click");
            break;
        case 8:
            $("#niv89").trigger("click");
            break;
    }
    ActivarLoader("panelContenedor" + idNivel);
    //-1 indica que debo capturar el padre del nivel 1, es decir la fuerza
    idPadre = (idPadre == -1) ? $("#MTDOFUERZA").val() : idPadre;
    var intIdPadre = { intCodigoPadre: idPadre, intNivel: idNivel, intFuerza: $("#MTDOFUERZA").val() }
    Variable[idNivel] = idPadre;
    $.ajax({
        type: "POST",
        url: urlConsulta,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: intIdPadre,
        success: function (dataNivelx) {
            //Limpiar Niveles
            limpiarTablas(idNivel);
            GenerarBotonCreacion($("#MTDOFUERZA").val(), idPadre, idNivel, Nombre, descripcion);
            for (tabla = parseInt(idNivel) + 1 ; tabla <= 10; tabla++)
                Variable[tabla] = 0;
            if (dataNivelx)
                if (dataNivelx[0].NIVEL != null) {
                    CargaTabla(dataNivelx, idNivel, idPadre)
                }
        },
        error: function (ex) {
            //Limpiar Niveles
            limpiarTablas(idNivel);
            GenerarBotonCreacion($("#MTDOFUERZA").val(), idPadre, idNivel, Nombre, descripcion);
            $("#ALERTA").empty();
            $("#ALERTA").html(alerta.replace("MENSAJE", "No se encontraron registros."));
            InactivarLoader("panelContenedor" + idNivel);
        }
    });
}
//Permite realizar la eliminación de un registro especifíco
function EliminarUnidad(IdCodigo, IdNivel, IdPadre, nombre) {
    var fuerzaId = $("#MTDOFUERZA").val();

    var datos = { fuerza: fuerzaId, codigo: IdCodigo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Unidad " + nombre,
        type: "warning",
        html: true,
        showCancelButton: true,
        confirmButtonColor: "#EF5350",
        cancelButtonColor: "#EF5385",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true,
        closeOnCancel: true
    },
    function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                type: "POST",
                url: urlEliminar,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: datos,
                beforeSend: function () {
                    $(".loader_decad").removeClass("hidden");
                    $(".loader_back").removeClass("hidden");
                },
                error: function (msn) {
                    GestionarAlerta("warning", msn);
                }
            }).done(function (resultado) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                if (resultado.result) {
                    GestionarAlerta("success", resultado.datos);
                    //recarga el nivel actual
                    CargaNivelX(IdPadre, IdNivel, nombre,"");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
    //});
}
function ActivarModoConsulta() {
    controlUnidades = false;
    glCreado = true;
}