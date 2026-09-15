using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.MinutaSivicc
{
    public class MdReferencias
    {

        public string ReferenciaId { get; set; }

        [Required(ErrorMessage = "The field {0}")]
        [StringLength(50, ErrorMessage =
            "The flied {0} could contain maximun {1} and minumum {2} characters", MinimumLength = 3)]
        [Display(Name = "State Description")]
        public string Descripcion { get; set; }
    }
}
