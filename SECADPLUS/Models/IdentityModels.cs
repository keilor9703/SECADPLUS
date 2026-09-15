using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SECADPLUS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class,
    // please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        //Atributos adicionales para el correcto funcionamiento de SECAD PLUS
        //public string IDENTIFICACION { get; set; }
        public DateTime? fecha_creacion {  get; set; }

        public bool? estado { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public ApplicationDbContext()
        //    : base("IdentityConnection", throwIfV1Schema: false)
        //{
        //}

        public ApplicationDbContext(DbConnection oracleConexion)
            : base(oracleConexion, contextOwnsConnection: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            try
            {
                //return new ApplicationDbContext();
                return new ApplicationDbContext(new OracleConnection(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString)));
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                return null;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // MUST go first.
                                                //modelBuilder.Conventions.Remove<ColumnTypeCasingConvention>();
                                                //modelBuilder.Conventions.Remove<ComplexTypeAttributeConvention>();
                                                // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("USR_CAD"); // Use uppercase!
            modelBuilder.Entity<ApplicationUser>().ToTable("CADNETOID");
            modelBuilder.Entity<IdentityRole>().ToTable("CADNETROLES");
            modelBuilder.Entity<IdentityUserRole>().ToTable("CADNETOIDROLES");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("CADNETCLAIMS");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("CADNETLOGINS");
        }
    }
}