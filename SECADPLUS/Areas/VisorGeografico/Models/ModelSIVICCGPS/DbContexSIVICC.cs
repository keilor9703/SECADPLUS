using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace SECADPLUS.Areas.VisorGeografico.Models.ModelSIVICCGPS
{
    public class DbContexSIVICC: DbContext
    {
        public DbContexSIVICC() : base("strConexionSivicc")
        {
            Database.SetInitializer<DbContexSIVICC>(null);
        }
        public virtual DbSet<V_CONSULTA_ACTUAL_GPS> UbicacionGPSActual { get; set; }
    }
}