$(function () {
    $(".styled, .multiselect-container input").uniform({
        radioClass: 'choice'
    });
    $("#btnLimpiar").click(function () {
        $("#resultado").empty();
        $("#txtIngreso").val("");
    });
    $("#consultar").click(function () {
        var valor = $("#txtIngreso").val();
        $("#resultado").empty();
        console.log($("#patrulla").prop("checked"));
        if ($("#patrulla").prop("checked")) {
            //Consutla patrullas
            $.ajax({
                type: "POST",
                url: urlConsultarPatrulla,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: { intSitio: 3947, strPatrulla: valor },
                beforeSend: function () {
                    $(".loader_decad").removeClass("hidden");
                    $(".loader_back").removeClass("hidden");
                }
            }).done(function (resultado) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                if (resultado.result) {
                    if (resultado.datos === "") {
                        GestionarAlerta("warning", "No se Encontraron resultados");
                    } else {
                        GestionarAlerta("success", "Consulta exitosa");
                    }
                    $("#resultado").html(resultado.datos);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
        if ($("#vehiculo").is(":checked")) {
            //consulta vehiculos
            $.ajax({
                type: "POST",
                url: urlConsultarVehiculo,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: { strSigla: valor },
                beforeSend: function () {
                    $(".loader_decad").removeClass("hidden");
                    $(".loader_back").removeClass("hidden");
                }
            }).done(function (resultado) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                if (resultado.result) {
                    if (resultado.datos === "") {
                        GestionarAlerta("warning", "No se Encontraron resultados");
                    } else {
                        GestionarAlerta("success", "Consulta exitosa");
                    }
                    $("#resultado").html(resultado.datos);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
        if ($("#unidad").is(":checked")) {
            //Consulta Unidades de rastreo
            $.ajax({
                type: "POST",
                url: urlConsultarRastreo,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: { strSigla: valor },
                beforeSend: function () {
                    $(".loader_decad").removeClass("hidden");
                    $(".loader_back").removeClass("hidden");
                }
            }).done(function (resultado) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                if (resultado.result) {
                    if (resultado.datos === "") {
                        GestionarAlerta("warning", "No se Encontraron resultados");
                    } else {
                        GestionarAlerta("success", "Consulta exitosa");
                    }
                    $("#resultado").html(resultado.datos);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    });
});