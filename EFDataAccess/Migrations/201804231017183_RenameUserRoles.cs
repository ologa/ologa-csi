namespace EFDataAccess.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using VPPS.CSI.Domain;

    public partial class RenameUserRoles : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE u SET u.RoleID = (SELECT r.RoleID FROM  [Role] r WHERE r.Description = 'Digitador de Dados') 
                FROM  [User] u JOIN  [Role] r ON r.RoleID = u.RoleID
                WHERE r.Description not in ('Digitador de Dados','M & E','Director')", false);
            Sql(@"Delete FROM  [Role] WHERE Description not in ('Digitador de Dados','M & E','Director')", false);
            Sql(@"Update  [Role] SET [Description] = 'Oficial-Provincial' WHERE Description = 'Digitador de Dados'", false);
            Sql(@"Update  [Role] SET [Description] = 'Oficial-OCB' WHERE Description = 'M & E'", false);
            Sql(@"Update  [Role] SET [Description] = 'Administrador' WHERE Description = 'Director'", false);
            Sql(@"Insert INTO  [UserRole] (UserID, RoleID, CreatedDate)
                 SELECT u.UserID, 5, GETDATE() FROM  [User] u WHERE u.UserName = 'Admin'", false);
        }

        public override void Down()
        {
            // There is not suppose to be a rollback instruction
        }
    }
}
