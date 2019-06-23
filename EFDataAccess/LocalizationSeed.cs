using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace EFDataAccess
{
    public class LocalizationSeed
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public void seed(string culture)
        {
            if (culture == "pt-PT")
                SeedPortuguese();
            else
                SeedEnglish();
        }

        private void SeedPortuguese()
        {
            context.ChildStatus.AddOrUpdate(
                            s => s.StatusID,
                            new VPPS.CSI.Domain.ChildStatus { StatusID=1, Description = "Inicial" },
                            // new VPPS.CSI.Domain.ChildStatus { StatusID = 2, Description = "Manutenção" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 3, Description = "Transferência" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 4, Description = "Desistência" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 5, Description = "Perdido" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 6, Description = "Adulto" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 7, Description = "Óbito" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 8, Description = "Outras Saídas" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 9, Description = "Graduação" }
                        );

            var roles = new List<VPPS.CSI.Domain.Role>
            {
                new VPPS.CSI.Domain.Role { RoleID = 1, Description = "Trabalhador de campo"},
                new VPPS.CSI.Domain.Role { RoleID = 2, Description = "Digitador de Dados"},
                new VPPS.CSI.Domain.Role { RoleID = 3, Description = "M & E"},
                new VPPS.CSI.Domain.Role { RoleID = 4, Description = "Gestor"},
                new VPPS.CSI.Domain.Role { RoleID = 5, Description = "Director"},
                new VPPS.CSI.Domain.Role { RoleID = 6, Description = "ICT"}
            };

            roles.ForEach(r => context.Roles.AddOrUpdate(s => s.RoleID, r));
            context.SaveChanges();
        }

        private void SeedEnglish()
        {
            context.ChildStatus.AddOrUpdate(
                            s => s.StatusID,
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 1, Description = "Initial" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 2, Description = "Maintenance" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 3, Description = "Transfer" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 4, Description = "Quitting" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 5, Description = "Lost" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 6, Description = "Adult" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 7, Description = "Death" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 8, Description = "Other" },
                            new VPPS.CSI.Domain.ChildStatus { StatusID = 9, Description = "Graduated" }
            );

            var roles = new List<VPPS.CSI.Domain.Role>
            {
                new VPPS.CSI.Domain.Role { RoleID = 1, Description = "Field Worker"},
                new VPPS.CSI.Domain.Role { RoleID = 2, Description = "Data Capturer"},
                new VPPS.CSI.Domain.Role { RoleID = 3, Description = "M & E"},
                new VPPS.CSI.Domain.Role { RoleID = 4, Description = "Line Manager"},
                new VPPS.CSI.Domain.Role { RoleID = 5, Description = "Director"},
                new VPPS.CSI.Domain.Role { RoleID = 6, Description = "ICT"}
            };

            roles.ForEach(r => context.Roles.AddOrUpdate(s => s.RoleID, r));

            context.SaveChanges();
        }
    }
}
