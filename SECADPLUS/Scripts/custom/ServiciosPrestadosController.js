var glConsecutivoMedio = 0;
var glHtmlOpcion = "<option value='_VALUE'>_DATO</option>";
var glClase = 0, glMarca = 0, glLinea = 0;
var glEditar = false;
$(function () {
    $(".lista").change(function () {
        CargarListas($(this).prop("id"));
    });
    $("#txtInfraccion").dblclick(function () {
        AbrirInfracciones()
    });
    $("#fecha_conducta").dblclick(function () {
        if (glEventFecha === "") {
            $('#fecha_conducta').data("DateTimePicker").date(glFechaTurno);
        } else {
            $('#fecha_conducta').data("DateTimePicker").date(glEventFecha);
        }
    });
    $("#txtConductas").dblclick(function () { AbrirConductas() });

    $("#txtEvento").dblclick(function () { AbrirEventos() });
    $(".chosen-select").chosen({ allow_single_deselect: true, width: "100%" });
    //Creación de medios
    $("#txtConsec").dblclick(function () {
        if (glEditar) {
            return;
        }
        AbrirMedios()
    });
    $("#txtCedula").dblclick(function () {
        if (glEditar) {
            return;
        }
        AbrirEmpleados()
    });
    $("#FechaConducta").dblclick(function () { CargarFechaConducta() });
    $("#txtParqueaderos").dblclick(function () {
        AbrirParqueaderos()
    });
});

function ConsultarPersonas() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaPersonas")) {
        $("#tablaPersonas").DataTable().destroy();
        $("#tablaPersonas tbody").empty();
    }
    var urlConductasPersonas = urlObtenerPersonas.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "IDENTIFICACION", "orderable": true },
                { "data": "Tipo", "orderable": true },
                { "data": "APELLIDOS", "orderable": true },
                { "data": "NOMBRES", "orderable": true },
                { "data": "GENERO", "orderable": true },
                { "data": "Infracciones", "orderable": true },
                { "data": "Conducta", "orderable": true },
                { "data": "Modo", "orderable": true },
                { "data": "CADEVENNUMEEVENTO", "orderable": true },
                { "data": "FechaConducta", "orderable": true },
                { "data": "Relacion", "orderable": true },
                { "data": "Participacion", "orderable": true },
                { "data": "OBSERVACIONES", "orderable": true }
    ];
    AplicarPluginTablaAjax("tablaPersonas", urlConductasPersonas, parametrosAjax);
}
function AbrirConductas() {
    ConsultarConductas();
    $("#ModalConductas").modal("toggle");
}
function AbrirInfracciones() {
    ConsultarInfracciones();
    $("#ModalInfracciones").modal("toggle");
}
function AbrirEventos() {
    ConsultarEvento();
    $("#ModalEvento").modal("toggle");
}

function ConsultarInfracciones() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblInfracciones")) {
        $("#tblInfracciones").DataTable().destroy();
        $("#tblInfracciones tbody").empty();
    }
    var urlFinal = urlConsultarInfracciones.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Codigo", "orderable": true },
                { "data": "Descripcion", "orderable": true },
    ];
    AplicarPluginTablaAjax("tblInfracciones", urlFinal, parametrosAjax);
}
function ConsultarConductas() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblConductas")) {
        $("#tblConductas").DataTable().destroy();
        $("#tblConductas tbody").empty();
    }
    var urlFinal = urlConsultarConductas.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true },
    ];
    AplicarPluginTablaAjax("tblConductas", urlFinal, parametrosAjax);
}
function SeleccionarConducta(codigo, nombre) {
    $("#txtConductas").val(codigo);
    $("#txtCondDesc").val(nombre);
    $("#ModalConductas").modal("toggle");
}

function SeleccionarInfraccion(codigo, nombre) {
    $("#txtInfraccion").val(codigo);
    $("#txtInfDesc").val(nombre);
    $("#hdInfDesc").val(nombre);
    $("#ModalInfracciones").modal("toggle");
}
function SeleccionarEvento(sitio, evento) {
    $("#hdSitio").val(sitio);
    $("#txtEvento").val(evento);
    $("#ModalEvento").modal("toggle");
}

function ConsultarEvento() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblEvento")) {
        $("#tblEvento").DataTable().destroy();
        $("#tblEvento tbody").empty();
    }
    var params = { intSitio: glEventoSitio, lgEvento: glEvento, lgMedio: glEventoMedio };
    var urlFinal = urlConsultarEvento.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Evento", "orderable": false },
                { "data": "Llamada", "orderable": false },
                { "data": "HoraCaso", "orderable": false },
                { "data": "DireccionCaso", "orderable": false },
                { "data": "Ciudad", "orderable": false },
                { "data": "CodigoPedido", "orderable": false },
                { "data": "Sitio", "orderable": false }
    ];
    AplicarPluginTablaAjaxLocal("tblEvento", urlFinal, parametrosAjax, params);
}
function AplicarDateTimePlugin() {
    $(".FechaHora").datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        locale: 'es',
        //minDate : new Date(),
        showClear: true
    });
}

function ConsultarMedios(sitio, consecutivo, conducta, modo) {
    var urlCrear = urlAgregarMedio;
    urlCrear = urlCrear.replace("_CONSEC", consecutivo).replace(/amp;/g, "");
    $("#frameMedio").attr("src", urlCrear);
    $("#contenedorFormMedio").removeClass("hidden");
    $("#btnAgregarPersonaCond").attr("href", urlCrear);
    $("#btnAgregarPersonaCond").removeClass("hidden");
    $("#refrescarPersonas").removeClass("hidden");
    $("#refrescarPersonas").attr("onclick","ConsultarMedios("+sitio+","+consecutivo+","+conducta+",'"+modo+"')");

    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaMedios")) {
        $("#tablaMedios").DataTable().destroy();
        $("#tablaMedios tbody").empty();
    }
    var params = { intSitio: sitio, lgConsecutivo: consecutivo, lgConducta: conducta, lgTurno: glTurno, intEntidad: glEntidad, strModo: modo };
    var urlFinal = urlObtenerMediosPersona.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADMEDISPCONSEC", "orderable": true },
                { "data": "Patrulla", "orderable": true },
                { "data": "Unidad", "orderable": true },
                { "data": "CEDU_EMPLEADO", "orderable": true },
                { "data": "Empleado", "orderable": true },
                { "data": "FechaConducta", "orderable": false },
                { "data": "Ponderacion", "orderable": false }
    ];
    AplicarPluginTablaAjaxLocal("tablaMedios", urlFinal, parametrosAjax, params);
}
function ConsultarMediosBienes(sitio, consecutivo, conducta, modo) {
    var urlCrear = urlAgregarMedio;
    urlCrear = urlCrear.replace("_CONSEC", consecutivo).replace(/amp;/g, "");
    $("#frameMedio").attr("src", urlCrear);
    $("#contenedorFormMedio").removeClass("hidden");
    $("#btnAgregarBienCond").attr("href", urlCrear);
    $("#btnAgregarBienCond").removeClass("hidden");
    $("#refrescarPersonas").removeClass("hidden");
    $("#refrescarPersonas").attr("onclick", "ConsultarMediosBienes(" + sitio + "," + consecutivo + "," + conducta + ",'" + modo + "')");
    // Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaMedios")) {
        $("#tablaMedios").DataTable().destroy();
        $("#tablaMedios tbody").empty();
    }
    var params = { intSitio: sitio, lgConsecutivo: consecutivo, lgConducta: conducta, lgTurno: glTurno, intEntidad: glEntidad, strModo: modo };
    var urlFinal = urlObtenerMediosBienes.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADMEDISPCONSEC", "orderable": true },
                { "data": "Patrulla", "orderable": true },
                { "data": "Unidad", "orderable": true },
                { "data": "CEDU_EMPLEADO", "orderable": true },
                { "data": "Empleado", "orderable": true },
                { "data": "FechaConducta", "orderable": false },
                { "data": "Ponderacion", "orderable": false }
    ];
    AplicarPluginTablaAjaxLocal("tablaMedios", urlFinal, parametrosAjax, params);
}
function AbrirMedios() {
    glConsecutivoMedio = 0;
    ConsultarMediosCond();
    $("#ModalMedios").modal("toggle");
}

function ConsultarMediosCond() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblMedios")) {
        $("#tblMedios").DataTable().destroy();
        $("#tblMedios tbody").empty();
    }
    var params = { intSitio: glSitio, lgTurno: glTurno, lgEventoMedio: glEventoMedio };
    var urlFinal = urlConsultarMedios.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Consecutivo", "orderable": true },
                { "data": "SITIO_GRABA", "orderable": true },
                { "data": "Patrulla", "orderable": true },
                { "data": "Descripcion", "orderable": true }
    ];
    AplicarPluginTablaAjaxLocal("tblMedios", urlFinal, parametrosAjax, params);
}
function SeleccionarPatrulla(consecutivo, patrulla, descripcion) {
    $("#txtConsec").val(consecutivo);
    $("#txtPatrulla").val(patrulla);
    $("#txtDescripcion").val(descripcion);
    $("#ModalMedios").modal("toggle");
    glConsecutivoMedio = consecutivo;
}
function AbrirEmpleados() {
    ConsultarEmpleados();
    $("#ModalPersonas").modal("toggle");
}
function ConsultarEmpleados() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblEmpleados")) {
        $("#tblEmpleados").DataTable().destroy();
        $("#tblEmpleados tbody").empty();
    }
    var params = { intSitio: glSitio, lgConsecutivo: glConsecutivoMedio, lgTurno: glTurno };
    var urlFinal = urlConsultarEmpleados.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Cedula", "orderable": true },
                { "data": "Nombre", "orderable": true }
    ];
    AplicarPluginTablaAjaxLocal("tblEmpleados", urlFinal, parametrosAjax, params);
}
function SeleccionarEmpleado(cedula, empleado) {
    $("#txtCedula").val(cedula);
    $("#txtEmpleado").val(empleado);
    $("#ModalPersonas").modal("toggle");
}
function CargarFechaConducta() {
    console.log(glFechaConducta);
    if (glFechaConducta === "" && glFechaConducta === null) {
        return;
    }
    $("#FechaConducta").val(glFechaConducta);
}
function ConsultarBienes() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaBienes")) {
        $("#tablaBienes").DataTable().destroy();
        $("#tablaBienes tbody").empty();
    }
    var urlFinal = urlObtenerBienes.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADCLBI_TIBI_CODIGO", "orderable": true },
                { "data": "Tipo", "orderable": true },
                { "data": "CADCLBI_CODIGO", "orderable": true },
                { "data": "Clase", "orderable": true },
                { "data": "MARCA_CODIGO", "orderable": true },
                { "data": "Marca", "orderable": true },
                { "data": "LINEA_CODIGO", "orderable": true },
                { "data": "Linea", "orderable": true },
                { "data": "COLO_CODIGO", "orderable": true },
                { "data": "Color", "orderable": true },
                { "data": "Fecha", "orderable": true },
                { "data": "Modo", "orderable": true },
                { "data": "CANTIDAD", "orderable": true },
                { "data": "AVALUO", "orderable": true },
                { "data": "PLACA", "orderable": true },
                { "data": "CHASIS", "orderable": true },
                { "data": "MOTOR", "orderable": true },
                { "data": "SERIE", "orderable": true },
                { "data": "CADPARQ_CODIGO", "orderable": true },
                { "data": "Parqueadero", "orderable": true },
                { "data": "CADINFR_CODIGO", "orderable": true },
                { "data": "Infraccion", "orderable": true },
                { "data": "CADEVENNUMEEVENTO", "orderable": true },
                { "data": "OBSERVACIONES", "orderable": true, "visible": true }

    ];
    AplicarPluginTablaAjax("tablaBienes", urlFinal, parametrosAjax);
}
//Carga las listas de clases, marcas y lineas
function CargarListas(identificador) {
    var opcion = "";
    var destino = "";
    var valor = $("#" + identificador).val();
    var urlFinal = urlConsultarListas.replace(/amp;/g, "");

    switch (identificador) {
        case "ddlTipo":
            opcion = "CLASES"
            destino = "ddlClase";
            //Limpia listas
            $("#" + destino).empty();
            $("#ddlMarca").empty();
            $("#ddlLinea").empty();
            break;
        case "ddlClase":
            opcion = "MARCAS"
            destino = "ddlMarca";
            $("#" + destino).empty();
            $("#ddlLinea").empty();
            break;
        case "ddlMarca":
            opcion = "LINEAS"
            destino = "ddlLinea";
            $("#" + destino).empty();
            break;
        case "ddlLinea":
            return;
            break;
    }
    if (valor === "") {
        $(".chosen-select").trigger("chosen:updated");
        return;
    }
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strOpcion: opcion, intValor: valor },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#" + destino).append($('<option>', {
                    value: "",
                    text: ""
                }));
                $.each(data.datos, function (index, valor) {
                    $("#" + destino).append($('<option>', {
                        value: valor.CODIGO,
                        text: valor.DESCRIPCION
                    }));
                });
                $("#" + destino).trigger("change");
            } else {
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function () {
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        //Actualiza todos los elementos chosen
        //evalua si debe aplicar un valor
        switch (identificador) {
            case "ddlTipo":
                destino = "ddlClase";
                if (glClase === 0) break;
                $("#ddlClase").val(glClase);
                CargarListas("ddlClase");
                break;
            case "ddlClase":
                if (glMarca === 0) break;

                $("#ddlMarca").val(glMarca);
                CargarListas("ddlMarca");
                break;
            case "ddlMarca":
                if (glLinea === 0) break;
                $("#ddlLinea").val(glLinea);
                CargarListas("ddlLinea");
                break;
            case "ddlLinea":
                return;
                break;
        }
        $(".chosen-select").trigger("chosen:updated");
    });
}
function AbrirParqueaderos() {
    ConsultarParqueaderos();
    $("#ModalParqueaderos").modal("toggle");
}
function ConsultarParqueaderos() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblParqueaderos")) {
        $("#tblParqueaderos").DataTable().destroy();
        $("#tblParqueaderos tbody").empty();
    }
    var urlFinal = urlConsultarParqueaderos.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true },
    ];
    AplicarPluginTablaAjax("tblParqueaderos", urlFinal, parametrosAjax);
}

function SeleccionarParqueaderos(codigo, descripcion) {
    $("#txtParqueaderos").val(codigo);
    $("#txtParDesc").val(descripcion);
    $("#ModalParqueaderos").modal("toggle");
}
function ConsultarComparendos() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaComparendos")) {
        $("#tablaComparendos").DataTable().destroy();
        $("#tablaComparendos tbody").empty();
    }
    var urlFinal = urlObtenerComparendos.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CODI_INFRACCION", "orderable": true },
                { "data": "Infraccion", "orderable": true },
                { "data": "CADEVENNUMEEVENTO", "orderable": true },
                { "data": "FechaConducta", "orderable": true },
                { "data": "CANTIDAD", "orderable": true },
                { "data": "OBSERVACIONES", "orderable": true }
    ];
    AplicarPluginTablaAjax("tablaComparendos", urlFinal, parametrosAjax);
}
function ConsultarMediosComparendos(sitio, consecutivo) {
    var urlCrear = urlAgregarMedio;
    urlCrear = urlCrear.replace("_CONSEC", consecutivo).replace(/amp;/g, "");
    $("#frameMedio").attr("src", urlCrear);
    $("#contenedorFormMedio").removeClass("hidden");
    
    $("#refrescarPersonas").removeClass("hidden");
    $("#refrescarPersonas").attr("onclick", "ConsultarMediosComparendos(" + sitio + "," + consecutivo + ")");
    // Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaMedios")) {
        $("#tablaMedios").DataTable().destroy();
        $("#tablaMedios tbody").empty();
    }
    var params = { intSitio: sitio, lgConsecutivo: consecutivo, lgTurno: glTurno, intEntidad: glEntidad };
    var urlFinal = urlObtenerMediosBienes.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADMEDISPCONSEC", "orderable": true },
                { "data": "Patrulla", "orderable": true },
                { "data": "Unidad", "orderable": true },
                { "data": "CEDU_EMPLEADO", "orderable": true },
                { "data": "Empleado", "orderable": true },
                { "data": "FechaConducta", "orderable": false },
                { "data": "Ponderacion", "orderable": false }
    ];
    AplicarPluginTablaAjaxLocal("tablaMedios", urlFinal, parametrosAjax, params);
}
function EliminarPersona(sitio, consecutivo, cedula) {
    var datos = { intSitio: sitio, lgConsecutivo: consecutivo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Número de indentificación: " + cedula,
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
                url: urlEliminarPersona,
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
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
                //Actualiza los registros
                ConsultarPersonas();
            });
        }
    }
    );
}

function CargarListasEdicion(clase, marca, linea) {
    glClase = clase;
    glMarca = marca;
    glLinea = linea;
    $("#ddlTipo").trigger("chosen:update");
    $("#ddlTipo").trigger("change");
    CargarListas("ddlTipo");
}

function AbrirEdicion(url) {
    $("#frameMedio").attr("src", url);
    $("#contenedorFormMedio").removeClass("hidden");    
}
function EliminarMedio(sitio, padreCon, medCon, cedula, turno, conducta, modo, opcion) {
    var datos = { intSitio: sitio, lgPerCon: padreCon, lgMedCon: medCon, strCedula: cedula };
    var urlFinal = "";
    switch (opcion) {
        case "BIEN":
            datos = { intSitio: sitio, lgBienCon: padreCon, lgMedCon: medCon, strCedula: cedula };
            urlFinal = urlEliminarMedioBien.replace(/amp;/g, "");
            break;
        case "COMPARENDO":
            datos = { intSitio: sitio, lgCompCon: padreCon, lgMedCon: medCon, strCedula: cedula };
            urlFinal = urlEliminarMedioComparendo.replace(/amp;/g, "");
            break;
        default:
            urlFinal = urlEliminarMedio.replace(/amp;/g, "");
            break;
    }
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Número de indentificación: " + cedula,
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
                url: urlFinal,
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
                    switch (opcion) {
                        case "PERSONA":
                            ConsultarMedios(sitio, padreCon, conducta, modo);
                            break;
                        case "BIEN":
                            ConsultarMediosBienes(sitio, padreCon, conducta, modo);
                            break;
                        case "COMPARENDO":
                            ConsultarMediosComparendos(sitio, padreCon);
                            break;
                    }
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}
function EliminarBien(sitio, consecutivo, tipo) {
    var datos = { intSitio: sitio, lgBienCon: consecutivo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Tipo Bien: " + tipo,
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
                url: urlEliminarBien,
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
                    //Actualiza los registros
                    ConsultarBienes();
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

function EliminarComparendo(sitio, consecutivo, infraccion) {
    var datos = { intSitio: sitio, lgConsecutivo: consecutivo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Infracción: " + infraccion,
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
                url: urlEliminarComparendo,
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
                    //Actualiza los registros
                    ConsultarComparendos();
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

function CambiarRutaForm(opcion, consecutivo) {
    switch(opcion){
        case "EDITARPERSONA":
            $("#formPersonas").attr("action", urlEditarPersona);
            $("#consecutivoPersona").val(consecutivo);
            break;
        case "EDITARBIEN":
            $("#formBienes").attr("action", urlEditarBien);
            $("#consecutivoBien").val(consecutivo);
            break;
        case "EDITARCOMPARENDO":
            $("#formComparendo").attr("action", urlEditarComparendo);
            $("#consecutivoComparendo").val(consecutivo);
            break;
    }
}


function AbrirEditarNormal(url) {
    var urlFinal = url.replace(/amp;/g, "");
    console.log(glEvento);
    if (glEvento>0) {
        urlFinal = urlFinal + "&lgEvento=" + glEvento + "&lgEventoMedio=" + glEventoMedio;
    }
    window.location.href = urlFinal;
}
