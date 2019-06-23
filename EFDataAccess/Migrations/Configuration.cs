namespace EFDataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using VPPS.CSI.Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<EFDataAccess.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EFDataAccess.ApplicationDbContext context)
        {
            if (context.Roles.FirstOrDefault() == null)
            {
                context.Roles.AddOrUpdate(s => s.Description, new Role { RoleID = 2, Description = "Oficial-OCB" });
                context.Roles.AddOrUpdate(s => s.Description, new Role { RoleID = 3, Description = "Oficial-Provincial" });
                context.Roles.AddOrUpdate(s => s.Description, new Role { RoleID = 5, Description = "Administrador" });
                context.SaveChanges();
            }

            if (context.Users.Where(p => p.Username == "Admin").FirstOrDefault() == null)
            {
                var User = new User { FirstName = "Admin", LastName = "Admin", Username = "Admin", PasswordStr = "password1", DefSite = 1, Admin = true };
                context.Users.Add(User);
                context.SaveChanges();

                var NewUserRole = new UserRole();
                NewUserRole.UserID = context.Users.Where(u => u.Username == "Admin").SingleOrDefault().UserID;
                NewUserRole.RoleID = context.Roles.ToList().LastOrDefault().RoleID;
                context.UserRoles.Add(NewUserRole);
                context.SaveChanges();
            }
        }
    }
}
