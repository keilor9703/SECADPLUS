using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadLugaresGeograficos
{
    [MetadataType(typeof(CadBarrioMetaData))]
    public partial class CADBARRIO
    {
    }

    public class CadBarrioMetaData
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int CODIGO { get; set; }

        [Display(Name = "Código Comuna")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int COZO_CODIGO { get; set; }

        [Display(Name = "Código Lugar Geográfico")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int LUGE_CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Código Lugar")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int COZO_LUGE_CODIGO { get; set; }

        [Display(Name = "Código Municipio")]
        [StringLength(20, ErrorMessage = "El campo admite solo 20 caracteres")]
        public string CODIGO_MPIO { get; set; }

        [Display(Name = "Tipo de Registro")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string TIPO_REGISTRO { get; set; }
    }
}