using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCARACTERISTICAMetadata))]
    public partial class CADCARACTERISTICA
    {
    }

    public class CADCARACTERISTICAMetadata
    {
        [StringLength(40)]
        [Required]
        [Display(Name = "Caracteristicas")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Tipo")]
        public string TIPO { get; set; }
        [Range(typeof(short), "0", "999", ErrorMessage = "El valor {0} debe ser un valor entre {1} y {2}.")]
        public short? TAMANO { get; set; }
    }
}