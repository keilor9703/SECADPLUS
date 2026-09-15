using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CadRadioMetaData))]
    public partial class CADRADIO
    {
    }

    public class CadRadioMetaData
    {
        [Display(Name = "Número de serie")]
        [StringLength(12)]
        public string NUME_SERIE { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200)]
        public string DESC_DESCRIPTIVO { get; set; }

        [Display(Name = "Cédula empleado")]
        [StringLength(12)]
        public string CADPOLI_CEDU_EMPLEADO { get; set; }

        [Required]
        [Display(Name = "Unidad Fuerza")]
        public int CADUNID_FUERZA { get; set; }

        [Required]
        [Display(Name = "Unidad Código")]
        public int CADUNID_CODIGO { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public byte CADESTAD_CODIGO { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public short CADMARC_CODIGO { get; set; }

        [Display(Name = "Clase Bien")]
        public Nullable<short> CADCLBI_TIBI_CODIGO { get; set; }

        [Display(Name = "Modelo")]
        public string CADMODELO_RADIO { get; set; }
    }
}