var controlRecepcion = true;
var columnsDef = [
    { "orderable": false, "targets": 0 }
]
var glOpcionesIdioma = {
    search: '<span>Filtro:</span> _INPUT_',
    lengthMenu: '<span>Mostrar:</span> _MENU_',
    paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
    info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
    infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
    loadingRecords: "Cargando registros...",
    zeroRecords: "No se han encontrado registros",
    processing: "Procesando...",
    infoFiltered: "(Filtrados de _MAX_ registros.)"
};
function AplicarColorFila(tablaId) {
    $("#" + tablaId + " tr").click(function () {
        $("#" + tablaId + " tr").each(function () {
            $(this).removeClass("selected");
        });
        $(this).toggleClass("selected");
    });
}
function AplicarPluginTabla(tablaId) {
    $("#" + tablaId).DataTable({
        responsive: true,
        rowReorder: true,
        autoWidth: false,
        dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
        language: glOpcionesIdioma,
    });
    $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    $('.dataTables_length select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });
    AplicarColorFila(tablaId);
}

function AplicarPluginTablaRegistraduriaResponse(tablaId) {
    $("#" + tablaId).DataTable({
        responsive: true,
        rowReorder: true,
        autoWidth: false,
        dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
        language: glOpcionesIdioma,
    });
    $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    $('.dataTables_length select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });
    AplicarColorFila(tablaId);
}

function AplicarPluginTablaModal(tablaId) {
    //console.log(tablaId);
    var otb = $("#" + tablaId).DataTable({
        responsive: true,
        rowReorder: true,
        autoWidth: false,

        dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
        language: glOpcionesIdioma,
        drawCallback: function () {
            $(this).find('tbody tr').slice(-3).find('.dropdown, .btn-group').addClass('dropup');
        },
        preDrawCallback: function () {
            $(this).find('tbody tr').slice(-3).find('.dropdown, .btn-group').removeClass('dropup');
        }
    });
    otb.columns.adjust().responsive.recalc();
    $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    // Enable Select2 select for the length option
    $('.dataTables_length select').select2({
        minimumResultsForSearch: Infinity,
        width: 'auto'
    });
    AplicarColorFila(tablaId);
}

function AplicarPluginTablaAjax(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}
function AplicarPluginTablaAjaxLocal(tablaId, ruta, columnas, parametros) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": false,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                data: parametros,
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}
function AplicarPluginTablaMinuta(tablaId, ruta, columnas, data) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                data: { strParametros: data },
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaCerrados(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    console.log(tablaId + "-" + filas);
    var tabla;
    if (filas <= 0) {
        tabla= $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "pageLength": 25,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                    controlRecepcion = false;
                },
                complete: function (datos) {
                    recorrerTablaRecepcion(); //Color
                    //AplicarColorFila(tablaId);
                }
            },
            //"columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
    return tabla;
}
function AplicarPluginTablaDespachos(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "pageLength": 25,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                    controlRecepcion = false;
                },
                complete: function (datos) {
                    recorrerTablaRecepcion(); //Color
                    //AplicarColorFila(tablaId);
                }
            },
            //"columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
    
}

function AplicarPluginTablaEventos(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (resultados) {
                    ConsultarTurno();
                    AplicarColorFila(tablaId);
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaAjaxPerRelacionadosMj(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosPerRelacionaMj(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}


function AplicarPluginTablaAjaxFichaSocial(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosFicha(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}




function AplicarPluginTablaAjaxFichaSocialTipos(tablaId, ruta, columnas, esTs) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);

                            if (esTs == 1) {
                                CargarDatosFichaTS(tablaId, this);
                            } else {
                                CargarDatosFichaTR(tablaId, this);
                            }
                          
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}


function AplicarPluginTablaAjaxFichaSocialPersonas(tablaId, ruta, columnas, esTs) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);

                            
                            CargarDatosPersonaSocial(tablaId, this);
                           

                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}



function AplicarPluginTablaAjaxRAEMujer(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            //"searching": false,
            // "paging": false,

            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    // CargarPriemraFilaRAE(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosSelectRAEMujer(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaAjaxRMMujer(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            //"searching": false,
            // "paging": false,

            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    //  CargarPriemraFilaRM(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosSelectRMMujer(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaAjaxROIMujer(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            //"searching": false,
            // "paging": false,

            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    //  CargarPriemraFilaRM(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosSelectROIMujer(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaConexiones(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                beforeSend: function () {
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                },
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    CargarPriemraFila(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatos(tablaId, this);
                        });
                    });
                    InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro (min. 2 carácteres):</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaAjaxDirectorioTel(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosDirecotio(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaVehiculosDijin(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "searching": false,

            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    CargarPriemraFilaVehiculo(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosSelect(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    recorrerTablaAplicaColor();

                    $(".loader_decad").addClass("hidden");
                    $(".loader_back").addClass("hidden");
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}
function AplicarPluginTablaRegistraduria(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "searching": false,
            "paging": false,

            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaOrdenCaptura(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "searching": false,

            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    // console.log("Entre");
                    CargarCaposOrdenCap(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            OrdenesCapturaSelect(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                    $(".loader_decad").addClass("hidden");
                    $(".loader_back").addClass("hidden");
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaPatrullas(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    // console.log("Entre");
                    CargarPriemraFilaCanal(tablaId);

                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosUnidadCanal(tablaId, this);
                        });
                    });

                    AplicarColorFila(tablaId);
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaEntidades(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                beforeSend: function () {
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                },
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    CargarPrimerafilaCanales(tablaId);
                    AplicarColorFila(tablaId);
                    InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                    $("#" + tablaId + " > tbody  > tr").each(function () {
                        // console.log("tr");
                        $(this).on("click", function () {
                            //  console.log(tablaId);
                            CargarDatosCanales(tablaId, this);
                        });
                    });
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaCanales(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    var filasActuales = $("#" + tablaId + " tbody tr").length;
                    if (filasActuales != 0) {
                       // CargarPrimerafilaPedidos(tablaId);

                        $("#" + tablaId + " > tbody  > tr").each(function () {
                            // console.log("tr");
                            $(this).on("click", function () {
                                //  console.log(tablaId);
                                CargarDatosPedido(tablaId, this);
                            });
                        });
                        AplicarColorFila(tablaId);
                    }
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaPedidos(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                beforeSend: function () {
                    
                    ActivarLoaderAvanzado($("#_tblCanales").parent().parent());
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                },
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    var filasActuales = $("#" + tablaId + " tbody tr").length;
                    // console.log("Fillas cuenta = " + filasActuales);
                    if (filasActuales != 0) {
                        CargarPrimerafilaComentarios(tablaId);

                        $("#" + tablaId + " > tbody  > tr").each(function () {
                            // console.log("tr");
                            $(this).on("click", function () {
                                //  console.log(tablaId);
                                CargarDatosComentarios(tablaId, this);
                            });
                        });

                        PintarCeldasTabla();
                        InactivarLoaderAvanzado($("#_tblCanales").parent().parent());
                        InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                    }
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaMediosDisponibles(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    recorrerTablaMediosDisponibles(); //Color
                    AplicarColorFila(tablaId);

                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();

                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaParams(tablaId, ruta, columnas, data) {
    var filas = $("#" + tablaId + " tbody tr").length;
    
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                data: data,
                beforeSend: function () {
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                }
                ,
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                    InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}

function AplicarPluginTablaMediosControl(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    recorrerTablaMediosControl(); //Color
                    AplicarColorFila(tablaId);

                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
    }
}

function AplicarPluginTablaEncuestar(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    var otb=null;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                beforeSend: function () {
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                    console.log($('.dataTables_filter input').val());
                },
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                   // $("#tabla tbody").empty();
                    recorrerTabla(); //Color
                    ConsultarPrimer();

                    AplicarColorFila(tablaId);

                    otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                    InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
            search: "<span>Filtro (min. 5 carácteres):</span> _INPUT_",
            lengthMenu: '<span>Mostrar:</span> _MENU_',
            paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
            info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
            infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
            loadingRecords: "Cargando registros...",
            zeroRecords: "No se han encontrado registros",
            processing: "Procesando...",
            infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_filter input').unbind().keyup(function () {
            var value = $(this).val();
            if (value.length > 4 || value.length === 0) {
                otb.search(value).draw();
            }
        });
    }
}

function AplicarPluginTablaPedidosRecepcion(tablaId, ruta, columnas, data) {
    var filas = $("#" + tablaId + " tbody tr").length;
    var otb = null;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                data: data,
                beforeSend: function () {
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                }
                ,
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                    InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                    //$('#' + tablaId).dataTable().fnFilterOnReturn();
                    $('.dataTable').dataTable().fnFilterOnReturn();
                }
            },
            "columns": columnas,
            
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro:</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
       
    }
}
function AplicarPluginTablaControlRecepcion(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    var tabla;
    var otb =null;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        tabla = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                beforeSend: function () {                    
                    ActivarLoaderAvanzado($("#" + tablaId).parent().parent());
                    
                },
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function (datos) {
                    // $("#tabla tbody").empty();
                    recorrerTabla(); //Color
                    ConsultarPrimer();

                    AplicarColorFila(tablaId);

                    otb= $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                    InactivarLoaderAvanzado($("#" + tablaId).parent().parent());
                }
            },
            //            "columnsDef": columnsDef,
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: {
                search: "<span>Filtro (min. 5 carácteres):</span> _INPUT_",
                lengthMenu: '<span>Mostrar:</span> _MENU_',
                paginate: { 'first': 'Primero', 'last': 'Último', 'next': '→', 'previous': '←' },
                info: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                infoEmpty: "Mostranto _START_ a _END_ de _TOTAL_ registros.",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No se han encontrado registros",
                processing: "Procesando...",
                infoFiltered: "(Filtrados de _MAX_ registros.)"
            },
        });
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_filter input').unbind().keyup(function () {
            var value = $(this).val();
            if (value.length > 4 || value.length===0) {
                otb.search(value).draw();
            }
        });
    }
    return tabla;
}

function AplicarTablaConBusqueda(tablaId, ruta, columnas, data, busca) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "oSearch": { "sSearch": busca },
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                data: data,
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        //otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
        AplicarColorFila(tablaId);
    }
}

function AplicarTablaPendientesAjax(tablaId, ruta, columnas) {
    var filas = $("#" + tablaId + " tbody tr").length;
    //console.log( tablaId + "-"+ filas);
    if (filas <= 0) {
        var otb = $('#' + tablaId).DataTable({
            "processing": true,
            "serverSide": true,
            "ajax": {
                url: ruta,
                method: "POST",
                dataType: "json",
                error: function () {
                    GestionarAlerta("warning", "Se presento un problema al cargar la información de la tabla.");
                },
                complete: function () {
                    AplicarColorFila(tablaId);
                    var otb = $('#' + tablaId).DataTable();
                    otb.columns.adjust().responsive.recalc();
                    var filasActuales = $("#" + tablaId + " tbody tr").length;
                    // console.log("Fillas cuenta = " + filasActuales);
                    if (filasActuales != 0) {
                        $("#" + tablaId + " > tbody  > tr").each(function () {
                            // console.log("tr");
                            $(this).on("click", function () {
                                //  console.log(tablaId);
                                otb
                                totalTabla = otb.rows().data();

                                if (totalTabla.length == 0) {
                                    return;
                                }
                                var datosTabla = otb.row(this).data();
                                glEventoMedio = datosTabla["CADMEDISP_CONSECUTIVO"];
                                console.log(glEventoMedio);
                            });
                        });

                    }
                }
            },
            "columns": columnas,
            responsive: true,
            rowReorder: true,
            autoWidth: false,
            dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
            language: glOpcionesIdioma,
        });
        // otb.columns.adjust().responsive.recalc();
        $('.dataTables_filter input[type=search]').attr('placeholder', 'Buscar...');
        $('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
            width: 'auto'
        });
    }
}