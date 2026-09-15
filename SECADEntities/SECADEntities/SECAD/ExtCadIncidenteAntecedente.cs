using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADINCIDENTES_ANTECEDENTEMetadata))]
    public partial class CADINCIDENTES_ANTECEDENTE
    {
    }

    public class CADINCIDENTES_ANTECEDENTEMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int SITIO_GRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public decimal CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Tipo Antecedente")]
        public string TIPO_ANTECEDENTE { get; set; }

        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> PERS_CONSECUTIVO { get; set; }

        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> VEHI_CONSECUTIVO { get; set; }

        [StringLength(200)]
        [Required]
        [Display(Name = "Patrulla Solicitante")]
        public string PATRULLA_SOLICITA { get; set; }

        [StringLength(200)]
        [Required]
        [Display(Name = "Integrantes Patrulla")]
        public string INTEGRANTES_PATRULLA { get; set; }

        [Required]
        [Display(Name = "Motivo")]
        public string MOTIVO { get; set; }
    }
}