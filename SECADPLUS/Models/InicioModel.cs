using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECADPLUS.Models
{
    public class InicioModel
    {
        public IEnumerable<SECADEntities.SecadLugaresGeograficos.CADBANNERIMAGENE> Imagenes { get; set; }
        public List<System.Web.Mvc.SelectListItem> ListaACD { get; set; }
        public List<System.Web.Mvc.SelectListItem> ListaMaquinas { get; set; }
        public string ACDseleccionado { get; set; }
        public string MaquinaSeleccionada { get; set; }


    }

    public class ImagenModel
    {

        public string Ruta { get; set; }
        public string Alt { get; set; }
    }


}