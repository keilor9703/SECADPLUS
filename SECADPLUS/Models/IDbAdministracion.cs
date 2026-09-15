using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADPLUS.Models
{
    public interface IDbAdministracion
    {
         DataTable F_GetImagenes(int Consecutivo);

         Task<DtoResultado<List<DtoSlider>>> F_GetSilerSuperior();
    }
}
