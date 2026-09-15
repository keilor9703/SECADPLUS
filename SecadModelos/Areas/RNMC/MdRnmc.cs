using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.RNMC
{
    public class MdRnmc
    {
        public string cnp_orden_comparendo { get; set; }

        public int cadeven_sitiograba { get; set; }

        public int cadeven_numeevento { get; set; }

        public int cnp_comportamiento_id { get; set; }

        public string tipo_idpersona { get; set; }

        public string idpersona { get; set; }

        public string nombre_persona { get; set; }
        public string apellido_persona { get; set; }
        public string cadpoli_cedula { get; set; }
        public DateTime fecha_grabacion { get; set; }
        public string maquina_grabacion { get; set; }
        public string usua_grabacion { get; set; }
        public string validado_cnp { get; set; }
        public string persona_registraduria { get; set; }
        public int comporta_id_articulo { get; set; }
        public int comporta_id_numeral { get; set; }
        public int comporta_id_literal { get; set; }
        public string Descripcion { get; set; }
        public int Id_Comportamiento { get; set; }
    }
}