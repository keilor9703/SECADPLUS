using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClProcesosIntegracion
{
    public class ClGrabacionesICP
    {
        public class ClCadPedidosDatosICP
        {
            public string cadpedi_sitiograba { get; set; }
            public string cadpedi_numellamada { get; set; }
            public string acd { get; set; }
            public string taskid { get; set; }
            public string caller { get; set; }
            public string recordid { get; set; }
            public string called { get; set; }
            public string callid { get; set; }
            public string usersecad { get; set; }
            public string cadequipo { get; set; }

        }

        public class ClCadPedidosDatosICPOut
        {
            public string codSalida { get; set; }
            public string desSalida { get; set; }

        }

        // clases para consumir el servicio de los datos  de grabacion de las llamadas
        public class ClDatosGrabacionIn
        {
            public string sitioGr { get; set; }
            public string Llamada { get; set; }
            public string fechaInicial { get; set; }
            public string fechaFinal { get; set; }
            public string acd { get; set; }

        }

        public class ClDatosGrabacionOut
        {
            public string datosSecad { get; set; }
            public string codSalida { get; set; }
            public string descSalida { get; set; }
        }

    }
}
