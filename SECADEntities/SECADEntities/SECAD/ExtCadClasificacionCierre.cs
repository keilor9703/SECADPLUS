using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(ClasificacionCierreMetaData))]
    public partial class CADCLASIFICACION_CIERRE
    {
    }

    public class ClasificacionCierreMetaData
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public int CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Fuerza")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int ID_FUERZA { get; set; }
    }
}