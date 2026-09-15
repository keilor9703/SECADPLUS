var glConsecutivoMedio = 0, glCedula = 0, glConsecutivoEquipo = 0;
var tablasId = ["tblMedios", "tblPersonas", "tblVehiculos", "tblArmas", "tblRadios"];
var botonesId = ["btnAgregarMedio", "btnAgregarPersona", "btnAgregarVehiculo", "btnAgregarArma", "btnAgregarRadio"]
var glLimite = 0;
var glNombrePatrulla = "";
var glTurnoId = 0;
var glMedioSelecciona = "";
///////VARIABLES PARA LA COPIA DEL TURNO
var glTurnoCopiar = 0, glClaseCopiar = "", glTipoCopiar = "", glFechaIniciaCopiar = "", glFechaTerminaCopiar = "", glSitioCopiar = 0, glFechaActual;

//Variable para la edición de conductas asociadas a un turno
var glEdicionConducta = 0, glEdicionTurno = 0, glEdicionSitio;
function SeleccionarUnidadMedio(codigo, descripcion, fuerza, unidad) {
    $("#hdCodigoPatrulla").val(codigo);
    $("#hdFuerzaPatrulla").val(fuerza);
    $("#hdUnidadPatrulla").val(unidad);
    $("#txtMedioUnidad").val(descripcion);
    $("#ModalPatrullas").modal("hide");
}
function SeleccionarCanal(codigo, descripcion, fuerza) {
    $("#hdCodigoCanal").val(codigo);
    $("#hdEntidadCanal").val(fuerza);
    $("#txtCanal").val(descripcion);
    $("#back-to-top").trigger("click");
    $("#ModalCanales").modal("toggle");
}
function IniElementos() {
    $(".chosen-select").chosen({ allow_single_deselect: true, width: "95%" });
    botonesId.forEach(OcultarBotones);
};
function CalcularFechas() {
    var tipoTurno = $("#ddlTipoTurno").val();
    var horasTurno = parseInt($("#hdHorasTurno").val());

    //Verifica que el turno sea diferente al de apoyo
    if (tipoTurno === "00") {
        horasTurno = 24;
    }
    var segundosTurno = horasTurno * 3600;
    var fechaInicia = $('#fecha_inicia').data("DateTimePicker").date();
    if (fechaInicia === null)
        return;
    //Asigna el dia de inicio
    $("#txtDiaInicia").val(fechaInicia.format("DD/MM/YYYY"));
    if (!isNaN(horasTurno)) {
        var segundosActuales = parseInt(fechaInicia.seconds());
        fechaInicia.seconds((segundosActuales) + segundosTurno - 1);
        $('#fecha_termina').data("DateTimePicker").date(fechaInicia);
        $("#txtDiaTermina").val(fechaInicia.format("DD/MM/YYYY"));
    }
}
$(function () {
    CambiarVisibilidadMenu();
    $('[data-toggle="tooltip"]').tooltip();
    //Calcula la fecha termina siempre que se cambie la fecha de inicio
    $("#fecha_inicia").on("dp.change", function (e) {
        CalcularFechas();
    });
    //Calcula la fecha termina siempre que se cambie la fecha de inicio
    $("#fecha_termina").on("dp.change", function (e) {
        var fechaTermina = $('#fecha_termina').data("DateTimePicker").date();
        if (fechaTermina === null) {
            $("#txtDiaTermina").val("");
        } else {
            $("#txtDiaTermina").val(fechaTermina.format("DD/MM/YYYY"));
        }
    });
    //recalcula las fechas siempre que se modifique el tipo de turno
    $("#ddlTipoTurno").change(function () {
        var tipo = $(this).val();
        if (tipo === null || tipo === "") {
            return;
        } else {
            CalcularFechas();
        }
    });
    //Consulta la fecha del siguiente turno disponible para el tipo seleccionado
    $("#fecha_inicia").click(function () {
        //verifica que se haya selecionado un turno
        if ($("#fecha_inicia").val() != "") { return; }
        var idEntidad = $("#hdEntidad").val();
        var tipoTurno = $("#ddlTipoTurno").val();
        if (tipoTurno === null || tipoTurno === "") {
            GestionarAlerta("warning", "Debe seleccionar un tipo de turno.");
            $("#ddlTipoTurno").focus();
            return;
        }
        $.ajax({
            method: "POST",
            url: urlObtenerSiguienteFecha,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: { strTipoTurno: tipoTurno, intEntidad: idEntidad },
            beforeSend: function () {
            },
            error: function () {
            }
        }).done(function (resultado) {
            ////console.log(resultado.result);
            if (resultado.result) {
                //$("#fecha_inicia").val(resultado.datos);
                $('#fecha_inicia').data("DateTimePicker").date(resultado.datos);
            }
        });
    });

    $("#txtMedioUnidad").keypress(function (e) {
        if (e.which == 13) {            
            CompletarPatrulla($(this).val());
        }
    });
    $("#txtMedioUnidad").dblclick(function (e) {
        $("#ModalPatrullas").modal("show");
    });
    $("#txtCanal").keypress(function (e) {
        if (e.which == 13) {
            CompletarCanal($(this).val());
        }
    });
    $("#txtCanal").dblclick(function (e) {
        $("#ModalCanales").modal("show");
    });
});

function CompletarCanal(canal) {
    if (canal === "") {
        return;
    }
    console.log(canal);
    urlConsultaCanales = urlConsultaCanales.replace(/amp;/g, "");
    //Agrega parametros de la consutla de patrullas
    var urlCanalFinal = urlConsultaCanales;
    $.ajax({
        method: "POST",
        url: urlCanalFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { intFuerza : glEntidadId, strCanal: canal},
        beforeSend: function () {
            $("#mensajeCanal").html("Identificando el canal ingresado...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                SeleccionarCanal(data.datos.CODIGO, data.datos.DESCRIPCION, data.datos.FUERZA);
                $("#mensajeCanal").html("");
            } else {
                if (data.datos === "2") {
                    CargarTablaCanalesMediosBusqueda(canal);
                    $("#mensajeCanal").html("");
                    $("#ModalCanales").modal("show");
                } else {
                    $("#mensajeCanal").html("");
                    $("#ModalCanales").modal("show");
                }
            }
        }
    }).done(function (data) {
    });

}
function CompletarPatrulla(patrulla){
    if (patrulla === "") {        
        return;
    }
    urlConsultaPatrullas = urlConsultaPatrullas.replace(/amp;/g, "");
    //Agrega parametros de la consutla de patrullas
    var urlPatrullas = urlConsultaPatrullas + "?intSitio=" + glParametros[0] + "&lgTurno=" + glParametros[1] + "&intUndeCodigo=" + glUnidadCodigo + "&intFuerza=" + glUnidadFuerza + "&strPatrulla=" + patrulla;
    $.ajax({
        method: "POST",
        url: urlPatrullas,
        content: "application/json; charset=utf-8",
        dataType: "json",
        //data: datos,
        beforeSend: function () {
            $("#mensajeMedio").html("Identificando el medio ingresado...");            
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                console.log(data.result);
                SeleccionarUnidadMedio(data.datos.CODIGO, data.datos.DESCRIPCION, data.datos.FUERZA, data.datos.UNIDAD);
                $("#mensajeMedio").html("");      
            } else {
                if (data.datos === "2") {
                    console.log("accede");
                    CargarTablaPatrullasMediosBusqueda(patrulla);
                    $("#mensajeMedio").html("");
                    $("#ModalPatrullas").modal("show");
                } else {
                    console.log("accede");
                    $("#mensajeMedio").html("");
                    $("#ModalPatrullas").modal("show");
                }
            }
        }
    }).done(function (data) {
    });

}
//Permite consultar los turnos asociados
function CargarTablaTurnos() {
    // console.log(glTurnoEvento);

    urlObtenerTurnos = urlObtenerTurnos.replace(/amp;/g, "") + "&strConsulta=" + glConductas + "&intTurno=" + glTurnoEvento;
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "TURNO", "orderable": true, "width": "10%" },
                { "data": "ClaseTurno", "orderable": true },
                { "data": "TipoTurno", "orderable": true },
                { "data": "HoraInicia", "orderable": true },
                { "data": "HoraTermina", "orderable": true },
                { "data": "DiaInicia", "orderable": true },
                { "data": "DiaTermina", "orderable": true },
                { "data": "CONSIGNAS", "orderable": true }

    ];
    AplicarPluginTablaAjax("tablaTurnos", urlObtenerTurnos, parametrosAjax);
    //console.log(glConductas + "aca");
    if (glConductas != "") {
        MostrarConductas();
    }

    if (glTurnoEvento > 0 && glSitiograbaTurnoEvento > 0) {
        ConsultarConductas(glTurnoEvento, glSitiograbaTurnoEvento)
    }
}
F
//Permite consultar las unidades en servicio
function CargarTablaUnidadesServicio() {
    urlUnidadesServicio = urlUnidadesServicio.replace(/amp;/g, "");
    if ($.fn.DataTable.isDataTable("#unidades")) {
        $("#unidades").DataTable().destroy();
        $("#unidades tbody").empty();
    }
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Medio", "orderable": true },
                { "data": "CADUNIP_CODIGO", "orderable": true },
                { "data": "CONSIGNAS", "orderable": true }
    ];

    AplicarPluginTablaAjax("unidades", urlUnidadesServicio, parametrosAjax);
}

//Permite consultar las unidades en servicio
function CargarUnidadesDisponibles() {
    //Limpia los valores de los campos del modal
    $("#contenedorTabla").show();
    $("#hdUnidad").val("");
    $("#hdFuerza").val("");
    $("#txtUnidad").val("");
    $("#txtConsignas").val("");
    if ($.fn.DataTable.isDataTable("#tblUnidadesDisponibles")) {
        $("#tblUnidadesDisponibles").DataTable().destroy();
        $("#tblUnidadesDisponibles tbody").empty();
    }
    $("#tituloUnidad").text("Crear Unidad de Servicio");
    urlObtenerUnidadesDisponibles = urlObtenerUnidadesDisponibles.replace(/amp;/g, "");

    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Medio", "orderable": true },
                { "data": "CADUNIP_CODIGO", "orderable": true },
                { "data": "CADUNID_FUERZA", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblUnidadesDisponibles", urlObtenerUnidadesDisponibles, parametrosAjax);
}
//Permite editar las consignas de una unidad de servicio especifíca
function ConsultarConsigna(unidadFuerza, unidadCodigo, descripcion) {
    ruta = urlConsultarConsigna;
    var parametrosLocal = [glParametros[0], glParametros[1]];
    parametrosLocal.push(unidadFuerza);
    parametrosLocal.push(unidadCodigo);
    $("#contenedorTabla").hide();
    $("#tituloUnidad").text("Modificación");
    $.ajax({
        method: "POST",
        url: ruta,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: parametrosLocal },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                var codigo = $("#hdUnidad").val(unidadCodigo);
                var codigo = $("#txtUnidad").val(descripcion);
                var fuerza = $("#hdFuerza").val(unidadFuerza);

                $("#txtConsignas").val(data.datos);

                $("#ModalUnidadesDisponibles").modal("toggle");
            } else {
                GestionarAlertaModal("warning", data.datos);
            }
        },
        error: function () {
            //console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}

function InicializarElementos(opcion) {
    switch (opcion) {
        case "CREAR":
            AplicarChosen();
            AplicarDateTimePlugin();
            break;
    }
}
function TraerMinuta() {   
    TraerMinutaejecutar();
}

function TraerMinutaejecutar() {
    $("#modalAlerta").empty();
    var urlFinal = urlTraerMinutaSivic.replace(/amp;/g, "");
    $.ajax({
        method: "POST",
        url: urlTraerMinutaSivic,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { 'lgTurno': $("#TurnoID").val(), 'intSitio': $("#SitioID").val(), 'strFechaInicia': $("#FechaI").val(), 'strFechaTermina': $("#FechaF").val() },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                GestionarAlerta("success", data.datos);                
            } else {
                GestionarAlerta("success", data.datos);                
            }
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });   
}

function RecargarMinutaejecutar() {
    $("#modalAlerta").empty();
    var urlFinal = urlRecargarMinutaSivic.replace(/amp;/g, "");
    $.ajax({
        method: "POST",
        url: urlRecargarMinutaSivic,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { 'lgTurno': $("#TurnoID").val(), 'intSitio': $("#SitioID").val(), 'strFechaInicia': $("#FechaI").val(), 'strFechaTermina': $("#FechaF").val() },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                GestionarAlerta("success", data.datos);
            } else {
                GestionarAlerta("success", data.datos);
            }
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}


//Permite almacenar la unidad disponible asociada a un turno
function GuardarUnidad() {
    var parametrosLocal = [glParametros[0], glParametros[1]];
    $("#modalAlerta").empty();
    var ruta = urlGuardarUnidad;
    var codigo = $("#hdUnidad").val();
    var fuerza = $("#hdFuerza").val();
    if (codigo === null || codigo === "") {
        GestionarAlertaModal("warning", "No se ha seleccionado una unidad de servicio.", "modalAlerta");
        return;
    }
    var consignas = $("#txtConsignas").val();
    parametrosLocal.push(fuerza);
    parametrosLocal.push(codigo);
    parametrosLocal.push(consignas);
    $.ajax({
        method: "POST",
        url: ruta,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: parametrosLocal },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalUnidadesDisponibles").modal('toggle');
                GestionarAlerta("success", data.datos);
                CargarTablaUnidadesServicio();
                //limpia formulario de creación
                CargarUnidadesDisponibles();
            } else {
            }
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}
function AplicarChosen() {
    $(".chosen-select").chosen({ allow_single_deselect: true, width: "100%" });
}

function SeleccionarUnidad(codigo, descripcion, fuerza) {
    $("#txtUnidad").val(descripcion);
    $("#hdUnidad").val(codigo);
    $("#hdFuerza").val(fuerza);
}
function AplicarDateTimePlugin() {
    
    $(".FechaHora").datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        //minDate : new Date(),
        showClear: true,
        useCurrent: false,
        locale: 'es'
    });
}
//Permite consultar los medios disponibles asociados a un turno
function ConsultarMedios(unidadFuerza, unidadCodigo, entidadTurnoId, medio) {
    //$("#cadunip_codigo").val() = unidadCodigo;
    $("#spannivel5").html("Medios de atención para: " + medio);
    glMedioSeleccionado = medio;
    var parametrosLocal = [glParametros[0], glParametros[1]];
    //Asigna las variables globales de entidad y codigo de unidad
    
    glUnidadCodigo = unidadCodigo;
    glEntidadId = entidadTurnoId;
    glUnidadFuerza = unidadFuerza;
    if ($.fn.DataTable.isDataTable("#tblMedios")) {
        $("#tblMedios").DataTable().destroy();
        $("#tblMedios tbody").empty();
    }
    parametrosLocal.push(unidadFuerza);
    parametrosLocal.push(unidadCodigo);

    var Url = urlMediosDisponibles.replace(/amp;/g, "");
    Url += "?intSitio=" + parametrosLocal[0] + "&lgTurno=" + parametrosLocal[1] + "&intUndeCodigo=" + unidadCodigo + "&intFuerza=" + unidadFuerza;
    //console.log(Url);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Patrulla", "orderable": true },
                { "data": "Tipo", "orderable": true },
                { "data": "Canal", "orderable": true },
                { "data": "Estado", "orderable": true },
                { "data": "CuadranteNew", "orderable": true }

    ];
    AplicarPluginTablaAjax("tblMedios", Url, parametrosAjax);
    $("#tabMedios").trigger("click");
    //Limpia tablas
    glLimite = 0;
    tablasId.forEach(GestionarElementos);
    //Carga y limpia formulario para crear medio de atención
    CargarMediosDisponibles();
}
//Permite cargar el modal para la edición de un medio de atención
function EditarMedioDisp(consecutivo, patrullaId, patrulla, canalFrz, canalCod, canal, tipo, estado) {
    CargarMediosDisponibles();
    $("#tituloMedio").text("Edición");
    $("#hdCodigoPatrulla").val(patrullaId);
    $("#txtMedioUnidad").val(patrulla);
    $("#txtCanal").val(canal);
    $("#hdCodigoCanal").val(canalCod);
    $("#hdEntidadCanal").val(canalFrz);

    $("#tipoMedio").val(tipo);
    $("#tipoMedio").trigger("chosen:updated");
    $("#idEstado").val(estado);
    $("#idEstado").trigger("chosen:updated");
    glConsecutivoMedio = consecutivo;
}
//Permite cargar el modal de creación de medios
function CargarMediosDisponibles() {
    //Limpia los valores de los campos del modal
    $("#ModalMediosDisponibles").modal("toggle");
    $("#contenedorTablaMedios").show();
    CargarTablaPatrullasMedios();
    CargarTablaCanalesMedios();
    $("#tituloMedio").text("Crear Medio de Atención");
    LimpiarCamposMediosDispModal();
}

function LimpiarCamposMediosDispModal() {
    //Limpia los elementos del modal
    $("#hdCodigoPatrulla").val(0);
    $("#hdFuerzaPatrulla").val(0);
    $("#hdUnidadPatrulla").val(0);
    $("#txtMedioUnidad").val("");
    $("#txtCanal").val("");
    $("#hdCodigoCanal").val(0);
    $("#hdEntidadCanal").val(0);
    glConsecutivoMedio = 0;
}
function CargarTablaCanalesMediosBusqueda(canal) {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblCanales")) {
        $("#tblCanales").DataTable().destroy();
        $("#tblCanales tbody").empty();
    }
    urlConsultaCanales = urlConsultaCanales.replace(/amp;/g, "");
    var datos = { intFuerza: glEntidadId };
    //Agrega parametros de la consulta de canales
    var urlFinalCanal = urlConsultaCanales;

    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "DESCRIPCION", "orderable": true },
                { "data": "CODIGO", "orderable": true },
                { "data": "CADFUERZ_ID", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblCanales", urlFinalCanal, parametrosAjax);
    AplicarTablaConBusqueda("tblCanales", urlFinalCanal, parametrosAjax,datos, canal);
}
function CargarTablaPatrullasMediosBusqueda(busqueda) {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblPatrullas")) {
        $("#tblPatrullas").DataTable().destroy();
        $("#tblPatrullas tbody").empty();
    }
    urlConsultaPatrullas = urlConsultaPatrullas.replace(/amp;/g, "");

    //Agrega parametros de la consutla de patrullas
    var urlPatrullas = urlConsultaPatrullas;
    var datos = { intSitio: glParametros[0], lgTurno: glParametros[1], intUndeCodigo: glUnidadCodigo, intFuerza:glUnidadFuerza}
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Patrulla", "orderable": true },
                { "data": "CADUNID_CODIGO", "orderable": true },
                { "data": "CADUNID_FUERZA", "orderable": true },
                { "data": "CODIGO", "orderable": true },
                { "data": "Movil", "orderable": true },
                { "data": "Estado", "orderable": true }
    ];
    //AplicarPluginTablaAjax("tblPatrullas", urlPatrullas, parametrosAjax);
    AplicarTablaConBusqueda("tblPatrullas", urlPatrullas, parametrosAjax, datos, busqueda);
}
function CargarTablaPatrullasMedios() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblPatrullas")) {
        $("#tblPatrullas").DataTable().destroy();
        $("#tblPatrullas tbody").empty();
    }
    urlConsultaPatrullas = urlConsultaPatrullas.replace(/amp;/g, "");

    //Agrega parametros de la consutla de patrullas
    var urlPatrullas = urlConsultaPatrullas + "?intSitio=" + glParametros[0] + "&lgTurno=" + glParametros[1] + "&intUndeCodigo=" + glUnidadCodigo + "&intFuerza=" + glUnidadFuerza;
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Patrulla", "orderable": true },
                { "data": "CADUNID_CODIGO", "orderable": true },
                { "data": "CADUNID_FUERZA", "orderable": true },
                { "data": "CODIGO", "orderable": true },
                { "data": "Movil", "orderable": true },
                { "data": "Estado", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblPatrullas", urlPatrullas, parametrosAjax);
}
//Permite cargar la tabla de canales para medios disponibles
function CargarTablaCanalesMedios() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblCanales")) {
        $("#tblCanales").DataTable().destroy();
        $("#tblCanales tbody").empty();
    }
    urlConsultaCanales = urlConsultaCanales.replace(/amp;/g, "");

    //Agrega parametros de la consulta de canales
    var urlFinalCanal = urlConsultaCanales + "?intFuerza=" + glEntidadId;

    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "DESCRIPCION", "orderable": true },
                { "data": "CODIGO", "orderable": true },
                { "data": "CADFUERZ_ID", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblCanales", urlFinalCanal, parametrosAjax);
}
//Permite guardar un medios disponible asociado a una unidad
function GuardarMedio() {
    $("#modalAlertaMedio").empty();
    var urlGuardarM = urlGuardarMedio;
    var tipo = $("#tipoMedio").val();
    var estado = $("#idEstado").val();
    var patrullaCodigo = $("#hdCodigoPatrulla").val();
    var canalCodigo = $("#hdCodigoCanal").val();
    var localParams = [glParametros[0], glParametros[1], glConsecutivoMedio];

    if (tipo === null || tipo === "" || estado === null || estado === "" || patrullaCodigo === null || patrullaCodigo === "0"
        || canalCodigo === null || canalCodigo === "0") {
        GestionarAlertaModal("warning", "Por favor seleccione un valor para todos los campos obligatorios.", "modalAlertaMedio");
        return;
    }
    localParams.push(tipo);
    localParams.push(glParametros[0]);
    localParams.push(estado);
    localParams.push(glUnidadFuerza);
    localParams.push(glUnidadCodigo);
    localParams.push(patrullaCodigo);
    localParams.push($("#hdEntidadCanal").val());
    localParams.push(canalCodigo);
    $.ajax({
        method: "POST",
        url: urlGuardarM,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: localParams },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalMediosDisponibles").modal('toggle');
                GestionarAlerta("success", data.datos);
                ConsultarMedios(glUnidadFuerza, glUnidadCodigo, glEntidadId, glMedioSeleccionado);
                //Carga y limpia formulario para crear medio de atención
                CargarMediosDisponibles();
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaMedio");
            }
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
    //localParams.empty();
}

function ConsultarPerMedios(sitio, consecutivo, nombre) {
    //Limpia tablas
    var titulo = "Persona que integra el medio de atención:";
    glLimite = 1;
    glNombrePatrulla = nombre;
    tablasId.forEach(GestionarElementos);
    CargarTablaPersonal(sitio, consecutivo);
    $("#tituloPersonas").empty();
    $("#tituloPersonas").html(titulo + " " + nombre);
    $("#tabPersonas").trigger("click");
    VerEmpleados();
}
function LimpiarCamposPersonalModal() {
    var urlGuardarP = urlGuardarPersona;
    $("#txtCedula").val("");
    $("#txtObservaciones").val("");
    $("#idCargo").val("");
    $("#idCargo").trigger("chosen:updated");
    $("#txtOrdenamiento").val("");
    $("#txtNombre").val("");
}
function CargarTablaPersonal(sitio, consecutivo) {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblPersonas")) {
        $("#tblPersonas").DataTable().destroy();
        $("#tblPersonas tbody").empty();
    }
    urlPerMedDisp = urlPerMedDisp.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var urlFinalPersonas = urlPerMedDisp + "?intSitio=" + sitio + "&lgConsecutivo=" + consecutivo
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADPOLI_CEDU_EMPLEADO", "orderable": true },
                { "data": "Empleado", "orderable": true },
                { "data": "Cargo", "orderable": true },
                { "data": "ORDENAMIENTO", "orderable": true },
                { "data": "OBSERVACION", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblPersonas", urlFinalPersonas, parametrosAjax);
    glConsecutivoMedio = consecutivo;
}
function VerEmpleados() {
    //Limpia los valores de los campos del modal
    $("#ModalPersonal").modal("toggle");
    CargarTablaPersonalModal();
    LimpiarCamposPersonalModal();
}
function CargarTablaPersonalModal() {
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblPersonalDisponible")) {
        $("#tblPersonalDisponible").DataTable().destroy();
        $("#tblPersonalDisponible tbody").empty();
    }
    urlPersonasXEntidad = urlPersonasXEntidad.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var urlFinalPersonal = urlPersonasXEntidad + "?intEntidadId=" + glEntidadId;
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CEDU_EMPLEADO", "orderable": true },
                { "data": "NOMB_EMPLEADO", "orderable": true },
                { "data": "GRAD_ALFABETICO", "orderable": true },
                { "data": "CADUNID_FUERZA", "orderable": true },
                { "data": "CADUNID_CODIGO", "orderable": true },
                { "data": "NUME_PLACA", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblPersonalDisponible", urlFinalPersonal, parametrosAjax);
}
function SeleccionaPersona(cedula, nombre) {
    $("#txtCedula").val(cedula);
    $("#txtNombre").val(nombre);
}
//Permite guardar un policia o personal a un medio disponible
function GuardarPersonal() {
    
    var urlGuardarP = urlGuardarPersona;
    var cedula = $("#txtCedula").val();
    var observacion = $("#txtObservaciones").val();
    var cargo = $("#idCargo").val();
    var ordenamiento = $("#txtOrdenamiento").val();
    var nombre = $("#txtNombre").val();
    $("#modalAlertaPersonal").empty();
    var localParams = [glParametros[0], glParametros[1], glConsecutivoMedio];
    if ((cedula === null || cedula === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione un empleado antes de continuar.", "modalAlertaPersonal");
        return;
    }
    localParams.push(cedula);
    localParams.push(cargo);
    localParams.push(observacion);
    localParams.push(ordenamiento);
    localParams.push(nombre);
    $.ajax({
        method: "POST",
        url: urlGuardarP,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: localParams },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalPersonal").modal('toggle');
                GestionarAlerta("success", data.datos);
                CargarTablaPersonal(glParametros[0], glConsecutivoMedio)
                VerEmpleados();//carga y limpia formulario de creación
            } else {
                GestionarAlertaModal("warning", data.datos);
                VerEmpleados();
            }
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}

function EditarPersonal(cedula, nombre, cargo, ordenamiento, observaciones) {
    LimpiarCamposPersonalModal();
    $("#txtCedula").val(cedula);
    $("#txtObservaciones").val(observaciones);
    $("#idCargo").val(cargo);
    $("#idCargo").trigger("chosen:updated");
    $("#txtOrdenamiento").val(ordenamiento);
    $("#txtNombre").val(nombre);
    $("#ModalPersonal").modal('toggle');

    CargarTablaPersonalModal();
}

function ConsultarVehiculos() {
    var urlVehiculos = urlConsultarMedios;
    var localParams = [glParametros[0], glConsecutivoMedio];
    if ((glCedula === null || glCedula === 0)) {
    }
    localParams.push("VEHICULOS");
    localParams.push(glCedula);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblVehiculos")) {
        $("#tblVehiculos").DataTable().destroy();
        $("#tblVehiculos tbody").empty();
    }
    urlVehiculos = urlVehiculos.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //console.log(urlVehiculos);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Descripcion", "orderable": true },
                { "data": "Observaciones", "orderable": true }
    ];
    AplicarPluginTablaMinuta("tblVehiculos", urlVehiculos, parametrosAjax, localParams);
}
function AgregarVehiculo() {
    //Limpia los valores de los campos del modal
    $("#tituloVehiculo").val("Creación");
    $("#ModalVehiculosDisp").modal("toggle");
    ConsultarVehiculosDisp();
    LimpiarVehiculosModal();
}
function LimpiarVehiculosModal() {
    $("#txtSiglaVehi").val("");
    $("#txtVehiculo").val("");
    $("#txtVehiculoObsv").val("");
}
function ConsultarVehiculosDisp() {
    var urlVehiculos = urlConsultarMediosDips;
    var localParams = [glParametros[0], glParametros[1]];
    localParams.push("VEHICULOSDISP");
    localParams.push(glUnidadCodigo);
    localParams.push(glEntidadId);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblVehiculoDisp")) {
        $("#tblVehiculoDisp").DataTable().destroy();
        $("#tblVehiculoDisp tbody").empty();
    }
    urlVehiculos = urlVehiculos.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //console.log(urlVehiculos);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Descripcion", "orderable": true },
                { "data": "Unidad", "orderable": true }
    ];
    AplicarPluginTablaMinuta("tblVehiculoDisp", urlVehiculos, parametrosAjax, localParams);
}
function ConsultarArmas() {
    var urlArmas = urlConsultarMedios;
    var localParams = [glParametros[0], glConsecutivoMedio];
    if ((glCedula === null || glCedula === "")) {
    }
    localParams.push("ARMAS");
    localParams.push(glCedula);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblArmas")) {
        $("#tblArmas").DataTable().destroy();
        $("#tblArmas tbody").empty();
    }
    urlArmas = urlArmas.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Descripcion", "orderable": true },
                { "data": "Observaciones", "orderable": true }
    ];
    AplicarPluginTablaMinuta("tblArmas", urlArmas, parametrosAjax, localParams);
}
function ConsultarRadios() {
    var urlRadios = urlConsultarMedios;
    var localParams = [glParametros[0], glConsecutivoMedio];
    if ((glCedula === null || glCedula === "")) {
    }
    localParams.push("RADIOS");
    localParams.push(glCedula);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblRadios")) {
        $("#tblRadios").DataTable().destroy();
        $("#tblRadios tbody").empty();
    }
    urlRadios = urlRadios.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Observaciones", "orderable": true }
    ];
    AplicarPluginTablaMinuta("tblRadios", urlRadios, parametrosAjax, localParams);
}
function CargarTablaMedios(cedula) {
    glCedula = cedula;
    glLimite = botonesId.length;
    tablasId.forEach(GestionarElementos);

    ConsultarVehiculos();
    ConsultarArmas();
    ConsultarRadios();
    $("#tabVehiculos").trigger("click");
    AgregarVehiculo();
    AgregarArma();
    AgregarRadio();
}
function SeleccionarEquipo(serie, nombre, opcion) {
    switch (opcion) {
        case "VEHICULOSDISP":
            $("#txtSiglaVehi").val(serie);
            $("#txtVehiculo").val(nombre);
            break;
        case "ARMASDISP":
            $("#txtSiglaArma").val(serie);
            $("#txtArma").val(nombre);
            break;
        case "RADIOSDISP":
            $("#txtSiglaRadio").val(serie);
            $("#txtRadio").val(nombre);
            break;
    }
}
function AgregarArma() {
    //Limpia los valores de los campos del modal
    $("#tituloArmas").val("Creación");
    $("#ModalArmasDisp").modal("toggle");
    LimpiarArmasModal();
    ConsultarArmasDisp();
}
function LimpiarArmasModal() {
    $("#txtSiglaArma").val("");
    $("#txtArma").val("");
    $("#txtArmaObsv").val("");
}
function ConsultarArmasDisp() {
    var urlArmas = urlConsultarMediosDips;
    var localParams = [glParametros[0], glParametros[1]];
    localParams.push("ARMASDISP");
    localParams.push(glUnidadCodigo);
    localParams.push(glEntidadId);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblArmaDisp")) {
        $("#tblArmaDisp").DataTable().destroy();
        $("#tblArmaDisp tbody").empty();
    }
    urlArmas = urlArmas.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //console.log(urlArmas);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Descripcion", "orderable": true },
                { "data": "Unidad", "orderable": true }
    ];
    AplicarPluginTablaMinuta("tblArmaDisp", urlArmas, parametrosAjax, localParams);
}
function AgregarRadio() {
    //Limpia los valores de los campos del modal
    $("#tituloRadios").val("Creación");
    $("#ModalRadiosDisp").modal("toggle");
    LimpiarRadiosModal();
    ConsultarRadiosDisp();
}
function LimpiarRadiosModal() {
    $("#txtSiglaRadio").val("");
    $("#txtRadio").val("");
    $("#txtRadioObsv").val("");
}
function ConsultarRadiosDisp() {
    var urlRadios = urlConsultarMediosDips;
    var localParams = [glParametros[0], glParametros[1]];
    localParams.push("RADIOSDISP");
    localParams.push(glUnidadCodigo);
    localParams.push(glEntidadId);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblRadioDisp")) {
        $("#tblRadioDisp").DataTable().destroy();
        $("#tblRadioDisp tbody").empty();
    }
    urlRadios = urlRadios.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //console.log(urlRadios);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Descripcion", "orderable": true }
    ];
    AplicarPluginTablaMinuta("tblRadioDisp", urlRadios, parametrosAjax, localParams);
}
//Permite guardar un vehículo asociado a un medio disponible
function GuardarVehiculo() {
    var observacion = $("#txtVehiculoObsv").val();
    var serie = $("#txtSiglaVehi").val();
    $("#modalAlertaVehiculo").empty();
    var localParams = [glParametros[0], glConsecutivoMedio];
    if ((serie === null || serie === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione un vehículo antes de continuar.", "modalAlertaVehiculo");
        return;
    }
    localParams.push(glCedula);
    localParams.push(serie);
    localParams.push(observacion);
    $.ajax({
        method: "POST",
        url: urlGuardarVehiculo,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: localParams },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalVehiculosDisp").modal('toggle');
                GestionarAlerta("success", data.datos);
                ConsultarVehiculos();
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaVehiculo");
            }
            AgregarVehiculo();//carga y limpia formulario de crear vehículo
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        //Se reinicia el valor del consecutivo asociado al vehiculo/arma/radio
        glConsecutivoEquipo = 0;
    });
}
//Permite guardar un arma asociada a un medio disponible
function GuardarArma() {
    var observacion = $("#txtArmaObsv").val();
    var serie = $("#txtSiglaArma").val();
    $("#modalAlertaArma").empty();
    var localParams = [glParametros[0], glConsecutivoMedio];
    if ((serie === null || serie === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione un arma antes de continuar.", "modalAlertaArma");
        return;
    }
    localParams.push(glCedula);
    localParams.push(serie);
    localParams.push(observacion);
    $.ajax({
        method: "POST",
        url: urlGuardarArma,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: localParams },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalArmasDisp").modal('toggle');
                GestionarAlerta("success", data.datos);
                ConsultarArmas();
                AgregarArma();//carga y limpia formulario de crear arma
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaArma");
            }
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        //Se reinicia el valor del consecutivo asociado al vehiculo/arma/radio
        glConsecutivoEquipo = 0;
    });
}
//Permite guardar un arma asociada a un medio disponible
function GuardarRadio() {
    var observacion = $("#txtRadioObsv").val();
    var serie = $("#txtSiglaRadio").val();
    $("#modalAlertaRadio").empty();
    var localParams = [glParametros[0], glConsecutivoMedio];
    if ((serie === null || serie === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione un radio antes de continuar.", "modalAlertaRadio");
        return;
    }
    localParams.push(glCedula);
    localParams.push(serie);
    localParams.push(observacion);
    $.ajax({
        method: "POST",
        url: urlGuardarRadio,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: localParams },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalRadiosDisp").modal('toggle');
                GestionarAlerta("success", data.datos);
                ConsultarRadios();
                AgregarRadio();//carga y limpia formulario de crear radio
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaRadio");
            }
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        //Se reinicia el valor del consecutivo asociado al vehiculo/arma/radio
        glConsecutivoEquipo = 0;
    });
}

function GestionarElementos(item, index) {
    if (index <= glLimite) {
        //Visualiza botones de acción
        $("#" + botonesId[index]).show();
    } else {
        //console.log("limpia :"+item);
        //Oculta botones de acción
        $("#" + botonesId[index]).hide();
        //limpia tablas
        if ($.fn.DataTable.isDataTable("#" + item)) {
            $("#" + item).DataTable().destroy();
            $("#" + item + " tbody").empty();
        }
    }
}
function OcultarBotones(item, index) {
    $("#" + item).hide();
}
//Permite realizar la eliminación de un registro especifíco
function EliminarUnidad(EntidadId, UnidadCodigo) {
    var datos = { intSitio: glParametros[0], lgConsecutivo: glParametros[1], intEntidad: EntidadId, intUnidadCodigo: UnidadCodigo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Unidad Seleccionada",
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
                url: urlEliminarUniturno,
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
                    //recarga la tabla de unidades
                    CargarTablaUnidadesServicio()
                    //limpia formulario de creación
                    CargarUnidadesDisponibles();
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}
//Permite realizar la eliminación de un registro especifíco
function EliminarMedioDisponible(ConsecutivoMedio) {
    var datos = { intSitio: glParametros[0], lgConsecutivo: ConsecutivoMedio };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Medio Disponible: " + ConsecutivoMedio,
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
                url: urlEliminarMedioDisp,
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
                    //recarga la tabla de unidades
                    ConsultarMedios(glUnidadFuerza, glUnidadCodigo, glEntidadId, glMedioSeleccionado);
                    
                    //Carga y limpia formulario para crear medio de atención
                    CargarMediosDisponibles();
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}
//Permite realizar la eliminación de un registro especifíco
function EliminarPersonaMedio(Cedula, nombre) {
    var datos = { intSitio: glParametros[0], lgConsecutivo: glConsecutivoMedio, strCedula: Cedula };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Cédula: " + Cedula + " Nombre: " + nombre,
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
                    //recarga la tabla de unidades
                    ConsultarPerMedios(glParametros[0], glConsecutivoMedio, glNombrePatrulla);
                    VerEmpleados();//carga y limpía formulario de creación
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}
function EditarEquipamiento(consecutivo, serie, descripcion, observacion, opcion) {
    glConsecutivoEquipo = consecutivo;
    switch (opcion) {
        case "VEHICULOS":
            LimpiarVehiculosModal();
            $("#txtSiglaVehi").val(serie);
            $("#txtVehiculo").val(descripcion);
            $("#txtVehiculoObsv").val(observacion);
            $("#tituloVehiculo").val("Edición");
            $("#ModalVehiculosDisp").modal('toggle');
            ConsultarVehiculosDisp();
            break;
        case "ARMAS":
            LimpiarArmasModal();
            $("#txtSiglaArma").val(serie);
            $("#txtArma").val(descripcion);
            $("#txtArmaObsv").val(observacion);
            $("#tituloArmas").val("Edición");
            $("#ModalArmasDisp").modal('toggle');
            ConsultarArmasDisp();
            break;
        case "RADIOS":
            LimpiarRadiosModal();
            $("#txtSiglaRadio").val(serie);
            $("#txtRadio").val(descripcion);
            $("#txtRadioObsv").val(observacion);
            $("#tituloRadios").val("Edición");
            $("#ModalRadiosDisp").modal('toggle');
            ConsultarRadiosDisp();
            break;
    }
}
//Permite realizar la eliminación de un registro especifíco
function EliminarEquipo(serie, opcion) {
    var datos = { intSitio: glParametros[0], lgConsecutivo: glConsecutivoMedio, strCedula: glCedula, strSerie: serie, strOpcion: opcion };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Sigla: " + serie,
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
                url: urlEliminarEquipo,
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
                    //recarga la tabla de unidades
                    switch (opcion) {
                        case "ARMAS":
                            AgregarArma();//carga y limpia formulario de crear arma
                            ConsultarArmas();
                            break;
                        case "RADIOS":
                            AgregarRadio();//carga y limpia formulario de crear radio
                            ConsultarRadios();
                            break;
                        case "VEHICULOS":
                            AgregarVehiculo();//carga y limpia formulario de crear vehículo
                            ConsultarVehiculos();
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
function MostrarConductas() {
    $("#crearTurno").hide();
}
function ConsultarConductas(turno, sitio) {
    //  console.log("turno=" +  turno + "sitio=" + sitio)
    glTurnoId = turno;
    var parametros = { dcSitio: sitio, lgTurno: turno, intEntidad: glEntidadId, strSitio: glSitio, strEntidad: glEntidad };
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaConductas")) {
        $("#tablaConductas").DataTable().destroy();
        $("#tablaConductas tbody").empty();
    }
    var urlConductas = urlObtenerConductas.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Tipo", "orderable": true },
                { "data": "CADCONDCODIGO", "orderable": true },
                { "data": "Conducta", "orderable": true }
    ];
    AplicarPluginTablaParams("tablaConductas", urlConductas, parametrosAjax, parametros);
}

//Permite guardar una conducta asociada a un turno
function GuardarConducta() {
    $("#modalAlertaConducta").empty();
    var codigo = $("#txtCodigo").val();
    var tipo = $("#Tipos").val();

    var localParams = [glParametros[0], glTurnoId];
    if ((codigo === null || codigo === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione una conducta antes de continuar.", "modalAlertaConducta");
        return;
    }
    if ((tipo === null || tipo === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione un tipo antes de continuar.", "modalAlertaConducta");
        return;
    }
    localParams.push(codigo);
    localParams.push(tipo);
    $.ajax({
        method: "POST",
        url: urlGuardarConducta,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strParametros: localParams },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalConductas").modal('toggle');
                GestionarAlerta("success", data.datos);
                ConsultarConductas(glTurnoId, glParametros[0]);
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaConducta");
            }
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}
function ConsultarConductasDisp(objeto) {
    var tipo = $(objeto).val();
    if ((tipo === null || tipo === "")) {
        GestionarAlertaModal("warning", "Por favor seleccione un tipo antes de continuar.", "modalAlertaConducta");
        return;
    }
    var parametros = { strTipo: tipo };
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblConductasDisp")) {
        $("#tblConductasDisp").DataTable().destroy();
        $("#tblConductasDisp tbody").empty();
    }
    var urlConductas = urlObtenerConductasDisp.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true },
    ];
    AplicarPluginTablaParams("tblConductasDisp", urlConductas, parametrosAjax, parametros);
}

function MostrarModalConductas() {
    $("#modalAlertaConducta").empty();
    if (glTurnoId === 0) {
        GestionarAlerta("warning", "Debe consultar las conductas de un turno antes de continuar.");
        return;
    }
    //Limpia los campos
    $("#txtCodigo").val("");
    $("#Tipos").val("");
    $("#Tipos").trigger("chosen:updated");
    $("#txtConducta").val("");
    $("#ModalConductas").modal("toggle");
    if ($.fn.DataTable.isDataTable("#tblConductasDisp")) {
        $("#tblConductasDisp").DataTable().destroy();
        $("#tblConductasDisp tbody").empty();
    }
}
function SeleccionarConducta(codigo, descripcion) {
    $("#txtCodigo").val(codigo);
    $("#txtConducta").val(descripcion);
}
function EliminarConducta(sitio, turno, conducta) {
    var datos = { intSitio: sitio, lgTurno: turno, lgConducta: conducta };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        //text: "Sigla: " + serie,
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
                url: urlEliminarConducta,
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
                    //se consulta conductas asociadas
                    ConsultarConductas(turno, sitio);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

function CopiarTurno(turno, sitio, horaInicia, horaTermina, tipo, clase, fecha) {
    $("modalAlertaCopiar").empty();
    $("#ModalCopiar").modal("show");

    console.log(turno + " " + sitio + " " + horaInicia + " " + horaTermina + " " + tipo + " " + clase + " " + fecha);
    glSitioCopiar = sitio;
    glTurnoCopiar = turno;
    glFechaIniciaCopiar = horaInicia;
    glFechaTerminaCopiar = horaTermina;
    glTipoCopiar = tipo;
    glClaseCopiar = clase;
    glFechaActual = fecha;
}
function Copiar() {
    ////Valida los datos
    var horasTurno = parseInt($("#hdHorasTurno").val());
    var HoraInicia = $("#fecha_inicia").val();
    var HoraTermina = $("#fecha_termina").val();
    var tipo = $("#ddlTipoTurno").val();
    var fechaInicia = $('#fecha_inicia').data("DateTimePicker").date();
    var fechaTermina = $('#fecha_termina').data("DateTimePicker").date();
    var horas_maximas = 0;
    GestionarAlerta("info", "Validando datos..", "modalAlertaCopiar");
    if (HoraInicia === "" || HoraTermina === "") {
        GestionarAlertaModal("warning", "Por favor ingrese fecha-hora inicial y fecha-hora de termino del turno..", "modalAlertaCopiar");
        return;
    }
    if (glTipoCopiar === "00" && tipo != "00") {
        GestionarAlertaModal("warning", "Un turno de apoyo no puede copiarse a un turno de normal..", "modalAlertaCopiar");
        return;
    }
    if (glTipoCopiar != "00" && tipo === "00") {
        GestionarAlertaModal("warning", "Un turno normal no puede ser copiado a un turno de apoyo..", "modalAlertaCopiar");
        return;
    }
    GestionarAlertaModal("info", "Validando datos2..", "modalAlertaCopiar");
    var DiaMes = fechaInicia.format("DD") + "/" + (fechaInicia.month() + 1);
    var Dia = fechaInicia.format("DD");
    var AnioInicia = fechaInicia.format("YYYY");
    var AnioTermina = fechaTermina.format("YYYY");

    if (AnioInicia > glFechaActual || AnioTermina > glFechaActual) {
        GestionarAlertaModal("warning", "'Revise el año del turno a crear...", "modalAlertaCopiar");
        return;
    }
    GestionarAlertaModal("info", "Validando datos3..", "modalAlertaCopiar");
    if (glClaseCopiar === "03") {
        tipo = "00";
        $("#ddlTipoTurno").val("00");
        $("#ddlTipoTurno").trigger("chosen:updated");
    }
    if (tipo === "00") {
        console.log("aplica");
        $('#fecha_inicia').data("DateTimePicker").date((fechaInicia.format("DD/MM/YYYY") + "00:00:00"));
        $('#fecha_termina').data("DateTimePicker").date((fechaTermina.format("DD/MM/YYYY") + "23:59:59"));
    } else {
        $('#fecha_inicia').data("DateTimePicker").date((fechaInicia.format("DD/MM/YYYY HH") + ":00:00"));
        $('#fecha_termina').data("DateTimePicker").date((fechaTermina.format("DD/MM/YYYY HH") + ":59:59"));
    }
    GestionarAlertaModal("info", "Validando datos4..", "modalAlertaCopiar");
    fechaInicia = $('#fecha_inicia').data("DateTimePicker").date();
    fechaTermina = $('#fecha_termina').data("DateTimePicker").date();

    var duration = moment.duration(fechaTermina.diff(fechaInicia));
    var minutes = duration.asMinutes();
    if (minutes < 0) {
        GestionarAlertaModal("warning", "Tiene las horas invertidas por favor revise..", "modalAlertaCopiar");
        return;
    }
    if (minutes === 0) {
        GestionarAlertaModal("warning", "'Las horas de inicio y termino son iguales.. revise ...", "modalAlertaCopiar");
        return;
    } else {
        if (tipo == "00") {
            horas_maximas = 24;
        } else {
            horas_maximas = horasTurno;
        }
        if (!(minutes >= 1 && minutes <= (horas_maximas * 60))) {
            GestionarAlertaModal("warning", "Revise la duración del turno se pasa de " + horas_maximas + " horas.", "modalAlertaCopiar");
            return;
        }
    }
    console.log("Ejecuta");
    GestionarAlertaModal("info", "Inicia Copiado de turno", "modalAlertaCopiar");
    var fechaI = fechaInicia.format("DD/MM/YYYY HH:mm:ss");
    var fechaF = fechaTermina.format("DD/MM/YYYY HH:mm:ss");
    console.log(glTurnoCopiar);
    console.log(glSitioCopiar);
    console.log(tipo);
    console.log(fechaI);
    console.log(fechaF);

    $.ajax({
        method: "POST",
        url: urlCopiarTurno,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { lgTurno: glTurnoCopiar, intSitio: glSitioCopiar, strTipo: tipo, strFechaInicia: fechaI, strFechaTermina: fechaF },
        beforeSend: function () {
            console.log("Acceder");
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                $("#ModalCopiar").modal('hide');
                var urlFinal = urlTurnos.replace(/amp;/g, "");
                window.location.href = urlFinal + "&strMensaje=" + data.datos;
                //Limpia variables
                glSitioCopiar = 0;
                glTurnoCopiar = 0;
                glFechaIniciaCopiar = "";
                glFechaTerminaCopiar = "";
                glTipoCopiar = "";
                glClaseCopiar = "";
                glFechaActual = "";
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaCopiar");
            }
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}

function AbrirVentana(opcion, valor, url) {
    if (valor === "N") {
        GestionarAlerta("warning", "Esta conducta no esta validad para "+opcion+"...");
        return false;
    }
    
    var urlFinal = url.replace(/amp;/g, "") + "&lgEvento=" + glEvento + "&lgEventoMedio=" + glEventoMedio;
    window.location.href = urlFinal;
}

function AbrirUnidades(clase, horaInicia, horaTermina, numeTurno, url) {
    //verifica si el turno se encuentra cruzado con otro
    var urlFinal = urlVerificarTruncado.replace(/amp;/g, "");
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strSitio: glParametros[0], strTurno: numeTurno, strClase: clase, strFechaInicia: horaInicia, strFechaFin: horaTermina },
        beforeSend: function () {
            console.log("Acceder");
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (data) {
            if (data.result) {
                console.log(data);
                //if (data.datos === 0) { cambio rmr 02-03-2020
                if (data.datos <= 1) {
                    window.location.replace("" + url + "");
                } else {
                    GestionarAlerta("warning", "No es posible completar la acción solicitada debido a que el turno seleccionado (" + numeTurno + ") se encuentra truncado por fechas con un turno existente.");
                }
            } else {
                GestionarAlertaModal("warning", data.datos, "modalAlertaCopiar");
            }
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function () {
            ////console.log("Error");
        }
    }).done(function (data) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}
function ActivarConductas() {
    console.log(glConsulta);
    if(glConsulta===""){
        $("#contenedorConductas").hide();
    }
}


