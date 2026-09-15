using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    public partial class PermisosEntities : DbContext
    {
        public PermisosEntities(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public PermisosEntities(string strConexion)
            : base(strConexion)
        {
        }
    }
    public partial class IngresoSistemaEntities : DbContext
    {
        public IngresoSistemaEntities(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public IngresoSistemaEntities(string strConexion)
            : base(strConexion)
        {
        }
    }
    public partial class SECADExcepcionEntities : DbContext
    {
        public SECADExcepcionEntities(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public SECADExcepcionEntities(string strConexion)
            : base(strConexion)
        {
        }
    }
    public partial class DesRepMinEntities : DbContext
    {
        public DesRepMinEntities(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public DesRepMinEntities(string strConexion)
            : base(strConexion)
        {
        }
    }
    public partial class AuditoriaEntities : DbContext
    {
        public AuditoriaEntities(EntityConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }
        public AuditoriaEntities(string strConexion)
            : base(strConexion)
        {
        }
    }
}
