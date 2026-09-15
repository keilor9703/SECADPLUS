var stack_custom_top = { "dir1": "down", "dir2": "right", "push": "top", "spacing5": 1 };
var stack_custom_left = { "dir1": "right", "dir2": "down" };
function GenerarSweetAlert(Mensaje) {
}
function GenerarPnotifyAlert(Mensaje, Tipo) {
    // Custom top position

    var opts = {
        title: "",
        text: Mensaje,
        addclass: "stack-custom-left bg-primary alert-styled-right",
        stack: stack_custom_top
    };
    switch (Tipo) {
        case 'ERROR':
            opts.title = "Atención";
            opts.addclass = "stack-custom-center bg-info";
            opts.type = "warning";
            break;

        case 'INFO':
            opts.title = "Breaking News";
            opts.text = "Have you met Ted?";
            opts.addclass = "stack-custom-top bg-info";
            opts.type = "info";
            break;

        case 'SUCESS':
            opts.title = "Good News Everyone";
            opts.text = "I've invented a device that bites shiny metal asses.";
            opts.addclass = "stack-custom-left bg-success";
            opts.type = "success";
            break;
    }
    new PNotify(opts);
}