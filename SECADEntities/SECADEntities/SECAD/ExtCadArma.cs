using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CadArmaMetaData))]
    public partial class CADARMA
    {
    }

    public class CadArmaMetaData
    {
        [Required]
        [StringLength(15)]
        [Display(Name = "Número de serie")]
        public string NUME_SERIE { get; set; }

        [Display(Name = "Calibre")]
        [Required]
        public string CALIBRE { get; set; }

        [Display(Name = "Unidad Fuerza")]
        [Required]
        public int CADUNID_FUERZA { get; set; }

        [Display(Name = "Unidad Codigo")]
        [Required]
        public int CADUNID_CODIGO { get; set; }

        [Display(Name = "Marca")]
        public Nullable<short> CADMARC_CODIGO { get; set; }

        [Display(Name = "Estado")]
        public string CADESTAD_CODIGO { get; set; }

        [Display(Name = "Clase Bien")]
        public Nullable<short> CADCLBI_TIBI_CODIGO { get; set; }

        [Display(Name = "Clase Bien Codigo")]
        public Nullable<short> CADCLBI_CODIGO { get; set; }
    }
}