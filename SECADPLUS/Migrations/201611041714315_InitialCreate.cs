namespace SECADPLUS.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "USR_CAD.CADNETROLES",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "USR_CAD.CADNETUSERROLES",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("USR_CAD.CADNETROLES", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("USR_CAD.CADNETUSERS", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "USR_CAD.CADNETUSERS",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    cedu_empleado = c.String(),
                    fecha_creacion = c.DateTime(),
                    fecha_activacion = c.DateTime(),
                    fecha_inactivo = c.DateTime(),
                    estado = c.Boolean(),
                    sitio_grabacion = c.Decimal(precision: 18, scale: 2),
                    cadcan_cadfuerza = c.Decimal(precision: 18, scale: 2),
                    cadcan_codigo = c.Decimal(precision: 18, scale: 2),
                    condicion = c.String(),
                    luge_codigo_cad = c.Decimal(precision: 18, scale: 2),
                    luge_codigo_dpto = c.Decimal(precision: 18, scale: 2),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "USR_CAD.CADNETUSERCLAIMS",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("USR_CAD.CADNETUSERS", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "USR_CAD.CADNETUSERLOGINS",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("USR_CAD.CADNETUSERS", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
        }

        public override void Down()
        {
            DropForeignKey("USR_CAD.CADNETUSERROLES", "UserId", "USR_CAD.CADNETUSERS");
            DropForeignKey("USR_CAD.CADNETUSERLOGINS", "UserId", "USR_CAD.CADNETUSERS");
            DropForeignKey("USR_CAD.CADNETUSERCLAIMS", "UserId", "USR_CAD.CADNETUSERS");
            DropForeignKey("USR_CAD.CADNETUSERROLES", "RoleId", "USR_CAD.CADNETROLES");
            DropIndex("USR_CAD.CADNETUSERLOGINS", new[] { "UserId" });
            DropIndex("USR_CAD.CADNETUSERCLAIMS", new[] { "UserId" });
            DropIndex("USR_CAD.CADNETUSERS", "UserNameIndex");
            DropIndex("USR_CAD.CADNETUSERROLES", new[] { "RoleId" });
            DropIndex("USR_CAD.CADNETUSERROLES", new[] { "UserId" });
            DropIndex("USR_CAD.CADNETROLES", "RoleNameIndex");
            DropTable("USR_CAD.CADNETUSERLOGINS");
            DropTable("USR_CAD.CADNETUSERCLAIMS");
            DropTable("USR_CAD.CADNETUSERS");
            DropTable("USR_CAD.CADNETUSERROLES");
            DropTable("USR_CAD.CADNETROLES");
        }
    }
}