using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(VehiculoMetaData))]
    public partial class CADVEHICULO
    {
    }

    public class VehiculoMetaData
    {
        [Required]
        [StringLength(7)]
        [Display(Name = "Sigla")]
        public string NUME_SIGLA { get; set; }

        [Required]
        [StringLength(8)]
        [Display(Name = "Placa")]
        public string NUME_PLACA { get; set; }

        [Required]
        [Display(Name = "Unidad Fuerza")]
        public int CADUNID_FUERZA { get; set; }

        [Required]
        [Display(Name = "Unidad Código")]
        public int CADUNID_CODIGO { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public short CADMARC_CODIGO { get; set; }

        [StringLength(2)]
        [Display(Name = "Estado")]
        public string CADESTAD_CODIGO { get; set; }

        [Display(Name = "Clase Bien")]
        public Nullable<short> CADCLBI_TIBI_CODIGO { get; set; }
    }
}