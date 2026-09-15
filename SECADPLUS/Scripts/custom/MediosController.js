$(function () {
    $(".chosen-select").chosen();
    $(".chosen-limpiar").chosen({ allow_single_deselect: true });
    //Evento al presionar un checkbox on switch
    $('input[name="cbxVigente"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            $("#hdVigente").val("S");
        } else {
            $("#hdVigente").val("N");
        }
    });
    $("#fecha_desde").change(function () {
        validarFechas();
    });
    $("#fecha_hasta").change(function () {
        validarFechas();
    });
    $("#listaCanales").chosen().change(function () {
        var valores = $(this).val().split("-");
        console.log(valores);
        if (valores.length > 0) {
            $("#hdTIBI").val(valores[0]);
            $("#hdCODIGO").val(valores[1]);
        }
    });
    $("#txtCedula").change(function () {
        var cedula = $(this).val();
        console.log("cambio");
        console.log(cedula);
        if (cedula === "") { return; }
        $.ajax({
            type: "POST",
            url: urlConsultarPersona,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: { strCedula: cedula },
            beforeSend: function () {
                $(".loader_decad").removeClass("hidden");
                $(".loader_back").removeClass("hidden");
            },
            error: function () {
                $("#txtNombre").val("");
                $("#txtCedula").val("");
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
            }
        }).done(function (resultado) {
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
            if (resultado.result) {
                $("#txtNombre").val(resultado.datos);
            } else {
                $("#txtNombre").val("");
                $("#txtCedula").val("");
                //implementar cuando existan problemas
                GestionarAlerta("warning", resultado.datos);
            }
        });
    });
})

//Carga los elementos para la edicion de registros
function IniciarEdicion() {
    if ($("#hdVigente").val() === "S") {
        $('#cbxVigente').bootstrapSwitch('state', true)
    } else {
        $('#cbxVigente').bootstrapSwitch('state', false)
    }

    $("#listaClase").val($("#hdClaseTurno").val());    
    AplicarControlFecha("#fecha_desde", fecha_desde);
    AplicarControlFecha("#fecha_hasta", fecha_hasta);
}
//Permite inicializar los elementos del formulario de creación
function IniciarElementos() {
    //Identifica el estado actual del valor vigente
    if ($('#cbxVigente').bootstrapSwitch('state')) {
        $("#hdVigente").val("S");
    } else {
        $("#hdVigente").val("N");
    }
    AplicarControlFecha("#fecha_desde", null);
    AplicarControlFecha("#fecha_hasta", null);
}

function validarFechas() {
    var fechaDesde = $("#fecha_desde").val();
    var fechaHasta = $("#fecha_hasta").val();
    if(fechaHasta ===""){
        return;
    }
    if(fechaDesde > fechaHasta){
        $("#fecha_hasta").val("");
        GestionarAlerta("warning","El valor de 'Fecha Hasta' no puede ser menor a la 'Fecha Desde'");
    }
}