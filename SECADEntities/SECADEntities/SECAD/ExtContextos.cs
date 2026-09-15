using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SECAD
{
    public partial class EntitiesCasos : DbContext
    {
        public EntitiesCasos(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public EntitiesCasos(string strConexion)
            : base(strConexion)
        {
        }
    }
    public partial class SecadEntities : DbContext
    {
        public SecadEntities(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public SecadEntities(string strConexion)
            : base(strConexion)
        {
        }
    }
}
