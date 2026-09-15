$(function () {

    //Iniciar listas de selección
    $('.select2').select2();


    //cargar unidad   
    $.ajax({
        type: 'POST',
        url: urlDepartamento,
        dataType: 'json',
        data: {},

        success: function (datos) {
            $.each(datos, function (i, dato) {
                $("#ddlDto").append('<option value="' + dato.Value + '">' + dato.Text + '</option>');
            });
            $("#ddlDto").trigger("chosen:updated");
        },
        error: function (ex) {
            $("#notificacion").html('<div class="alert alert-danger alert-dismissible"> <h4><i class="fa fa-search"></i> <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>'
                + "Error al consultar: " + ex + '</h4></div>');
        }
    })

    //Consultar municipios
    $("#ddlDto").change(function () {
        var valor = document.getElementById("ddlDto").value;

        //Limpiar el campo cuadrante
        $("#ddlLugar").empty();

        $.ajax({
            type: 'POST',
            url: urlMunicipio + "/" + valor,
            dataType: 'json',
            data: { id: $("#ddlDto").val() },

            success: function (datos) {
                $.each(datos, function (i, dato) {
                    $("#ddlLugar").append('<option value="' + dato.Value + '">' + dato.Text + '</option>');
                });
                $("#ddlLugar").trigger("chosen:updated");
            },
            error: function (ex) {
                $("#notificacion").html('<div class="alert alert-danger alert-dismissible"> <h4><i class="fa fa-search"></i> <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>'
                    + "Error al consultar: " + ex + '</h4></div>');
            }
        })
    })


});