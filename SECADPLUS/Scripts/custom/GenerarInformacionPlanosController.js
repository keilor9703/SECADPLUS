$(function () {
    Date.prototype.ddmmyyyyhhmm = function () {
        var yyyy = this.getFullYear();
        var mm = this.getMonth() < 9 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1); // getMonth() is zero-based
        var dd = this.getDate() < 10 ? "0" + this.getDate() : this.getDate();
        var hh = this.getHours() < 10 ? "0" + this.getHours() : this.getHours();
        var min = this.getMinutes() < 10 ? "0" + this.getMinutes() : this.getMinutes();
        return "".concat(dd).concat(mm).concat(yyyy).concat(hh).concat(min);
    };

    //funcion para reemplazar todo en un string
    String.prototype.replaceAll = function (str1, str2, ignore) {
        return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
    };

    AplicarDateTimePlugin();
});

function GeneraPlano() {
    var idPlano = parseInt($("#Plano").val().toString());
    var nombreArchivo = "";
    if ($("#FechaInicio").val() == "" || $("#FechaFinal").val() == "") {
        GestionarAlerta("warning", "Debe Seleccionar una fecha de inicio y una fecha fin");
        return;
    }

    var fechaInicio = $("#FechaInicio").val();
    var fechaFin = $("#FechaFinal").val();
    var fechaArchivo = new Date();

    switch (idPlano) {
        case 1:
            nombreArchivo = "PlanoLlamadas" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 2:
            if ($("#TipoPedido").val() == "-1") {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                GestionarAlerta("warning", "Primero debe seleccionar el tipo de pedido...");
                return;
            } else {
                nombreArchivo = "PlanoResumenCasos" + fechaArchivo.ddmmyyyyhhmm();
            }
            break;
        case 3:
            if ($("#TipoPedido").val() == "-1") {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                GestionarAlerta("warning", "Primero debe seleccionar el tipo de pedido...");
                return;
            } else {
                nombreArchivo = "PlanoCasosPatrullas" + fechaArchivo.ddmmyyyyhhmm();
            }
            break;
        case 4:
            nombreArchivo = "PlanoRevistas" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 5:
            nombreArchivo = "PlanoLlamadasVsMedios" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 6:
            nombreArchivo = "ResumenLlamadas" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 7:
            nombreArchivo = "ConsultaAntecedentes" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 8:
            nombreArchivo = "ConsultasPositivas" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 9:
            nombreArchivo = "Llamadas_improcedentes" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 10:
            nombreArchivo = "Encuestas_calidad" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 11:
            nombreArchivo = "PlanoResumenIncidentesCNP" + fechaArchivo.ddmmyyyyhhmm();
            break;
        case 12:
            nombreArchivo = "PlanoResumenAlcaldia" + fechaArchivo.ddmmyyyyhhmm();
            break;
        default:
            GestionarAlerta("warning", "Primero debe seleccionar el tipo de planilla");
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
            return;
            break;
    }

    var entityData = {
        "intTipoPlano": idPlano,
        "strFecha1": fechaInicio,
        "strFecha2": fechaFin,
        "intTipoPedido": $("#TipoPedido").val() != undefined ? $("#TipoPedido").val() : ""
    };

    $.ajax({
        type: "POST",
        url: urlCargarDataPlanos,
        content: "application/json; charset=utf-8",
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        cache: false,
        data: entityData,
        dataType: "json",
        success: function (d) {
            a = document.createElement('a');
            a.download = nombreArchivo + ".txt";
            var csvData = new Blob(d.archivo.split("|"), { type: 'text/csv' });
            var csvUrl = URL.createObjectURL(csvData);
            a.href = csvUrl;
            $("#plano").empty();
            $("#plano").append(a);
            a.click();
        },
        error: function (xhr, textStatus, errorThrown) {
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    });
}

function AplicarDateTimePlugin() {
    $(".FechaHora").datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        defaultDate: new Date(),
        showClear: true
    });
}