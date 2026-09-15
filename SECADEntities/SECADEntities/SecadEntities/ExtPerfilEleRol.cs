using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataTypeAttribute(typeof(CadNetPerfilEleRolMetaData))]
    public partial class CADNETPERFIL_ELE_PERMISO
    {
    }

    public class CadNetPerfilEleRolMetaData
    {
        [Display(Name = "Perfil")]
        public decimal PEP_PERFIL_ID { get; set; }

        [Display(Name = "Permiso")]
        public decimal PEP_PERMISO_ID { get; set; }

        [Display(Name = "Elemento")]
        public decimal PEP_ELEMENTO_ID { get; set; }
    }
}