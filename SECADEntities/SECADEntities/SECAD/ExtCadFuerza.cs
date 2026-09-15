using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CadFuerzaMetaData))]
    public partial class CADFUERZA
    { }

    public class CadFuerzaMetaData
    {
        [Display(Name = "ID")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int ID { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(60, ErrorMessage = "El campo admite solo 60 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Abreviatura")]
        [StringLength(10, ErrorMessage = "El campo admite solo 10 caracteres")]
        public string ABREVIATURA { get; set; }

        [Display(Name = "Fuerza")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Este campo es necesario")]
        public int CADUNDE_FUERZA { get; set; }

        [Display(Name = "Unidad")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Este campo es necesario")]
        public int CADUNDE_CODIGO { get; set; }

        [Display(Name = "Vigente")]
        [StringLength(1, ErrorMessage = "El campo admite solo 1 carácter")]
        public string VIGENTE { get; set; }

        [Display(Name = "Desde")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public DateTime DESDE { get; set; }

        [Display(Name = "Observación")]
        [StringLength(200, ErrorMessage = "El campo admite solo 200 caracteres")]
        public string OBSERVACION { get; set; }

        [Display(Name = "Máximo de Horas Turno")]
        [Range(1, 999, ErrorMessage = "Valor debería ser entre 1 hasta 999")]
        public int HORAS_TURNO { get; set; }

        [Display(Name = "Fuerza Principal")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Este campo es necesario")]
        public int CADUNDE_FUERZA_PPAL { get; set; }

        [Display(Name = "Código Unidad Principal")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Este campo es necesario")]
        public int CADUNDE_CODIGO_PPAL { get; set; }

        [Display(Name = "Sitio de Grabación")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int SITIO_GRABA { get; set; }

        [Display(Name = "Fecha Transacción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public DateTime FECH_TRANSACCION { get; set; }
    }
}