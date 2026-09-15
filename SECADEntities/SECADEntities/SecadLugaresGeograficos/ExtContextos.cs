using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadLugaresGeograficos
{
    public partial class EntitiesComunasBarrios : DbContext
    {
        public EntitiesComunasBarrios(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public EntitiesComunasBarrios(string strConexion)
            : base(strConexion)
        {
        }
    }
}
