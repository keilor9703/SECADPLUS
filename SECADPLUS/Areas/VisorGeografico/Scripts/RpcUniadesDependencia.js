$(function () {

    //Consultar la Unidad de la Región
    $("#ddlRegion").change(function () {
        var valor = document.getElementById("ddlRegion").value;

        //Limpiar la dependencia
        $("#ddlUnidad").empty();

        $.ajax({
            type: 'POST',
            url: urlDatosUnidad + "/" + valor,
            dataType: 'json',
            data: { id: $("#ddlRegion").val() },

            success: function (dependencias) {
                $.each(dependencias, function (i, dependencia) {
                    $("#ddlUnidad").append('<option value="' + dependencia.Value + '">' + dependencia.Text + '</option>');
                });
                $("#ddlUnidad").trigger("chosen:updated");
            },
            error: function (ex) {
                alert('Seleccione una unidad de la lista !!!');

            }
        })
    })

    //Consultar la dependencia de la unidad
    $("#ddlUnidad").change(function () {
        var valor = document.getElementById("ddlUnidad").value;

        //Limpiar la dependencia
        $("#ddlDependencia").empty();

        $.ajax({
            type: 'POST',
            url: urlDatosDependencia + "/" + valor,
            dataType: 'json',
            data: { id: $("#ddlUnidad").val() },

            success: function (dependencias) {
                $.each(dependencias, function (i, dependencia) {
                    $("#ddlDependencia").append('<option value="' + dependencia.Value + '">' + dependencia.Text + '</option>');
                });
                $("#ddlDependencia").trigger("chosen:updated");
            },
            error: function (ex) {
                alert('Seleccione una dependencia de la lista !!!');

            }
        })
    })

});