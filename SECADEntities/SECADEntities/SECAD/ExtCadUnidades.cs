using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataTypeAttribute(typeof(CadUnidadMetaData))]
    public partial class CADUNIDADE
    {
    }

    public class CadUnidadMetaData
    {
        [Required]
        public string SIGLA { get; set; }

        [Required]
        public string DESCRIPCION { get; set; }

        [Display(Name = "LUGAR")]
        public int LUGE_CODIGO { get; set; }

        [Required]
        public byte NIVEL { get; set; }

        [Required]
        public Nullable<int> CONS_SIEDCO { get; set; }
    }
}