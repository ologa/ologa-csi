namespace EFDataAccess.Migrations
{
    using EFDataAccess.Services;
    using EFDataAccess.Services.TrimesterServices;
    using EFDataAccess.UOW;
    using System;
    using System.Collections.Generic;
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
            if (context.OrgUnits.FirstOrDefault() == null)
            {
                context.OrgUnits.AddOrUpdate(s => s.Name, new OrgUnit { Name = "Default Location" });
                context.SaveChanges();
            }

            if (context.Sites.FirstOrDefault() == null)
            {
                var orgUnitID = context.OrgUnits.FirstOrDefault().OrgUnitID;
                context.Sites.AddOrUpdate(s => s.SiteName, new Site { SiteName = "Default Site", SiteType = "NGO" , orgUnitID = orgUnitID });
                context.SaveChanges();
            }

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

            if (System.Configuration.ConfigurationManager.AppSettings["AppVersion"].Contains("desktop"))
            {
                LoadTrimesters(context);
                EvaluateBeneficiaries();
            }
        }

        private async System.Threading.Tasks.Task EvaluateBeneficiaries()
        {
            UnitOfWork uow = new UnitOfWork();
            BeneficiaryService beneficiaryService = new BeneficiaryService(uow);

            
            string evaluationType = "InitialLoadingData";
            DateTime currentDate = DateTime.Now;
            beneficiaryService.cleanEvaluatedBeneficiaries(evaluationType);
            beneficiaryService.EvaluateAllBeneficiariesServicesState(evaluationType, currentDate);
        }

        private void LoadTrimesters(ApplicationDbContext context)
        {
            if(context.Trimesters.Count() == 0)
            {
                UnitOfWork uow = new UnitOfWork();
                TrimesterService trimesterService = new TrimesterService(uow, new TrimesterQueryService(uow));
                var trimesters = trimesterService.GenerateTrimestersForTheLastPastYears(20);
                trimesterService.SaveTrimesters(trimesters);
            }
        }

        private static void deprecatedActions(ApplicationDbContext context)
        {
            var siteGoals = new List<SiteGoal>
            {
                new SiteGoal {Indicator ="Número de beneficiários servido por programas de PEPFAR para OVC e famílias afetadas pelo HIV/AIDS", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""},
                new SiteGoal {Indicator ="Número de referencias de saúde e outros serviços sociais", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""},
                new SiteGoal {Indicator ="Número de referencias de saúde e outros serviços sociais designadas por completas", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""},
                new SiteGoal {Indicator ="Número de agregados familiares recebendo Kit Familiar", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""},
                new SiteGoal {Indicator ="Número de crianças dos 6 - 59 meses rastreados para malnutrição aguda ao nível comunitário (MUAC)", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""},
                new SiteGoal {Indicator ="Número de crianças  6 - 59 meses com malnutrição aguda, detetados ao nível  da comunidade (Muac)", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""},
                new SiteGoal {Indicator ="Percentagem de OVC com seroestado reportado ao parceiro de implementação (<18 anos)", GoalDate = DateTime.Now, GoalNumber = 0, SitePerformanceComment = ""}
            };

            siteGoals.ForEach(r => context.SiteGoals.AddOrUpdate(s => s.Indicator, r));
            context.SaveChanges();

            if (context.ChildStatus.FirstOrDefault() == null)
                context.ChildStatus.AddOrUpdate(
                    s => s.Description,
                    new ChildStatus { Description = "Active" },
                    new ChildStatus { Description = "Deceased" },
                    new ChildStatus { Description = "Left due to being LTF" },
                    new ChildStatus { Description = "Left due to graduation" },
                    new ChildStatus { Description = "Defaulted" }
                );

            var resources = new List<Resource>
            {
                new Resource {Description ="Família", IsOrganization = true},
                new Resource {Description ="Apoio Directo"},
                new Resource {Description ="Apoio por Referencia"}
            };

            resources.ForEach(r => context.Resources.AddOrUpdate(s => s.Description, r));

            var collaboratorRoles = new List<CollaboratorRole>
            {
                new CollaboratorRole { Code = "ACTIVIST", Description="Activista" },
                new CollaboratorRole { Code = "HEAD", Description="Activista Chefe" },
                new CollaboratorRole { Code = "SUPERVISOR", Description="Supervisor" }
            };

            collaboratorRoles.ForEach(c => context.CollaboratorRoles.AddOrUpdate(e => e.Code, c));
            context.SaveChanges();

            // Uncomment by DF

            var configTypes = new List<ConfigType>
           {
               new ConfigType {Description = "UserSurveyFrequency" },
               new ConfigType {Description = "UserInactivityReport" },
               new ConfigType {Description = "UserInactivityReportDayCountTrigger" }
           };

            configTypes.ForEach(s => context.ConfigTypes.AddOrUpdate(p => p.Description, s));

            context.SaveChanges();

            var sites = context.Sites.ToList();

            var siteConfigs = new List<SiteConfig>();

            // cross Join sites and configs
            foreach (Site site in sites)
            {
                var configs = new List<Config>
                       {
                           new Config { ConfigType = configTypes[0], Detail = "52"}, //52weeks
                           new Config { ConfigType = configTypes[1], Detail = "False"},
                           new Config { ConfigType = configTypes[2], Detail = "365"}, // 365 days
                       };

                foreach (Config config in configs)
                    siteConfigs.Add(new SiteConfig { Site = site, Config = config });
            }

            foreach (SiteConfig siteConfig in siteConfigs)
            {
                var siteConfigInDataBase = context.SiteConfigs.Include("Config.ConfigType").Include("Site").Where(
                    s =>
                        s.Config.ConfigType.ConfigTypeID == siteConfig.Config.ConfigType.ConfigTypeID &&
                        s.Site.SiteID == siteConfig.Site.SiteID
                    ).SingleOrDefault();

                if (siteConfigInDataBase == null)
                {
                    context.SiteConfigs.Add(siteConfig);
                }
            }

            context.SaveChanges();

            //add generic support services for all existing domains
            var domains = context.Domains.ToList();

            foreach (DomainEntity domain in domains)
            {
                var supportService = context.SupportServiceTypes.Where(s => s.Description == "GENERIC SERVICES SUPPORT"
                    && s.Domain.DomainID == domain.DomainID).SingleOrDefault();

                if (supportService == null)
                    context.SupportServiceTypes.Add(new SupportServiceType
                    {
                        Description = "GENERIC SERVICES SUPPORT",
                        Generic = true,
                        Domain = domain,
                        Default = false,
                        supportservicetype_guid = Guid.NewGuid()
                    });
            }

            context.SaveChanges();

            if (context.OrgUnitTypes.Count() == 0)
            {
                // UnitOrgType
                var national = new OrgUnitType { Description = "National", NounDescription = "National" };
                var province = new OrgUnitType { Description = "Province", NounDescription = "Province", Parent = national };
                var district = new OrgUnitType { Description = "District", NounDescription = "District", Parent = province };
                var adminPost = new OrgUnitType { Description = "Administrative post", NounDescription = "Administrative post", Parent = district };
                var Location = new OrgUnitType { Description = "Location", NounDescription = "Location", Parent = adminPost };

                var unitOrgTypes = new List<OrgUnitType>();

                unitOrgTypes.AddRange(new[] { national, province, district, adminPost, Location });

                unitOrgTypes.ForEach(u => context.OrgUnitTypes.AddOrUpdate(x => x.Description, u));

                context.SaveChanges();
            }

            // SERVICE TRACKING

            if (context.ReferenceTypes.Count() == 0)
            {
                var references = new List<ReferenceType>();

                references.Add(new ReferenceType { ReferenceName = "Maternidade p/ Parto", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 1 });
                references.Add(new ReferenceType { ReferenceName = "CPN", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 2 });
                references.Add(new ReferenceType { ReferenceName = "CPN Familiar", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 3 });
                references.Add(new ReferenceType { ReferenceName = "Consulta Pós-Parto", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 4 });
                references.Add(new ReferenceType { ReferenceName = "CCR", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 5 });
                references.Add(new ReferenceType { ReferenceName = "PTV", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 6 });

                references.Add(new ReferenceType { ReferenceName = "ATS", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 7 });
                references.Add(new ReferenceType { ReferenceName = "ITS", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 8 });
                references.Add(new ReferenceType { ReferenceName = "Pré-TARV/IO", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 9 });
                references.Add(new ReferenceType { ReferenceName = "Testado HIV+", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 10 });
                references.Add(new ReferenceType { ReferenceName = "Abandono TARV", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 11 });
                references.Add(new ReferenceType { ReferenceName = "PPE", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 12 });
                references.Add(new ReferenceType { ReferenceName = "Circuncisao Masculina", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 13 });


                references.Add(new ReferenceType { ReferenceName = "Suspeito de TB", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 14 });
                references.Add(new ReferenceType { ReferenceName = "Contacto de TB", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 15 });
                references.Add(new ReferenceType { ReferenceName = "Controlo de BK", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 16 });
                references.Add(new ReferenceType { ReferenceName = "Abandono de TTB", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 17 });
                references.Add(new ReferenceType { ReferenceName = "Reacções do TTB", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 18 });
                references.Add(new ReferenceType { ReferenceName = "Suspeito de Malária", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 19 });

                references.Add(new ReferenceType { ReferenceName = "OCB/Apoio Comunitário", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 20 });
                references.Add(new ReferenceType { ReferenceName = "Educação", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 21 });
                references.Add(new ReferenceType { ReferenceName = "Acção Social", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 22 });
                references.Add(new ReferenceType { ReferenceName = "GAVV", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 23 });
                references.Add(new ReferenceType { ReferenceName = "Apoio Psico-Social", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 24 });
                references.Add(new ReferenceType { ReferenceName = "Posto Policial", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 25 });

                references.Add(new ReferenceType { ReferenceName = "Suspeito de Malnutrição", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 26 });
                references.Add(new ReferenceType { ReferenceName = "Banco de Socorro/Controle de triagem", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 27 });
                references.Add(new ReferenceType { ReferenceName = "Controlo da Dor", FieldType = "CheckBox", ReferenceCategory = "Activist", ReferenceOrder = 28 });
                references.Add(new ReferenceType { ReferenceName = "Outros Motivos/Especificar cuidados/Serviço prestado ou a ser prestado", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Activist", ReferenceOrder = 29 });

                //Health

                references.Add(new ReferenceType { ReferenceName = "Maternidade p/ parto", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[0], ReferenceOrder = 1 });
                references.Add(new ReferenceType { ReferenceName = "CPN", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[1], ReferenceOrder = 2 });
                references.Add(new ReferenceType { ReferenceName = "CPN Familiar", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[2], ReferenceOrder = 3 });
                references.Add(new ReferenceType { ReferenceName = "Consulta Pós-parto", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[3], ReferenceOrder = 4 });
                references.Add(new ReferenceType { ReferenceName = "PTV", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[5], ReferenceOrder = 5 });
                references.Add(new ReferenceType { ReferenceName = "CCR", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[4], ReferenceOrder = 6 });
                references.Add(new ReferenceType { ReferenceName = "ATS", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[6], ReferenceOrder = 7 });
                references.Add(new ReferenceType { ReferenceName = "ITS", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[7], ReferenceOrder = 8 });
                references.Add(new ReferenceType { ReferenceName = "Testado HIV+", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[9], ReferenceOrder = 9 });
                references.Add(new ReferenceType { ReferenceName = "Pré TARV/IO", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[8], ReferenceOrder = 10 });
                references.Add(new ReferenceType { ReferenceName = "CD", FieldType = "CheckBox", ReferenceCategory = "Health", ReferenceOrder = 11 });
                references.Add(new ReferenceType { ReferenceName = "TARV", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[10], ReferenceOrder = 12 });
                references.Add(new ReferenceType { ReferenceName = "Sem TB", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[13], ReferenceOrder = 13 });
                references.Add(new ReferenceType { ReferenceName = "Profilaxia por contacto TB", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[14], ReferenceOrder = 14 });
                references.Add(new ReferenceType { ReferenceName = "Tratamento de TB", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[16], ReferenceOrder = 15 });
                references.Add(new ReferenceType { ReferenceName = "Doenças crónicas", FieldType = "CheckBox", ReferenceCategory = "Health", ReferenceOrder = 16 });
                references.Add(new ReferenceType { ReferenceName = "Controlo da Dor", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[27], ReferenceOrder = 17 });
                references.Add(new ReferenceType { ReferenceName = "Controlo de BK", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[15], ReferenceOrder = 18 });
                references.Add(new ReferenceType { ReferenceName = "Mês", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Health", ReferenceOrder = 19 });
                references.Add(new ReferenceType { ReferenceName = "Reacções do TTB", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[17], ReferenceOrder = 20 });
                references.Add(new ReferenceType { ReferenceName = "PPE", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[11], ReferenceOrder = 21 });
                references.Add(new ReferenceType { ReferenceName = "Terapia Nutricional", FieldType = "CheckBox", ReferenceCategory = "Health", OriginReference = references[25], ReferenceOrder = 22 });
                references.Add(new ReferenceType { ReferenceName = "CCS", FieldType = "CheckBox", ReferenceCategory = "Health", ReferenceOrder = 23 });
                references.Add(new ReferenceType { ReferenceName = "Outros atendimentos/Especifique", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Health", ReferenceOrder = 24 });
                references.Add(new ReferenceType { ReferenceName = "Referido para", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Health", ReferenceOrder = 25 });
                references.Add(new ReferenceType { ReferenceName = "Observações", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Health", ReferenceOrder = 26 });
                references.Add(new ReferenceType { ReferenceName = "Motivo de referência", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Health", ReferenceOrder = 27 });

                // Social

                references.Add(new ReferenceType { ReferenceName = "Subsídios de alimentos", FieldType = "CheckBox", ReferenceCategory = "Social", OriginReference = references[21], ReferenceOrder = 1 });
                references.Add(new ReferenceType { ReferenceName = "Habitação", FieldType = "CheckBox", ReferenceCategory = "Social", ReferenceOrder = 2 });
                references.Add(new ReferenceType { ReferenceName = "Educação", FieldType = "CheckBox", ReferenceCategory = "Social", OriginReference = references[20], ReferenceOrder = 3 });
                references.Add(new ReferenceType { ReferenceName = "Atestado de pobreza", FieldType = "CheckBox", ReferenceCategory = "Social", ReferenceOrder = 4 });
                references.Add(new ReferenceType { ReferenceName = "OCB", FieldType = "CheckBox", ReferenceCategory = "Social", OriginReference = references[19], ReferenceOrder = 5 });
                references.Add(new ReferenceType { ReferenceName = "Acção Social produtiva", FieldType = "CheckBox", ReferenceCategory = "Social", ReferenceOrder = 6 });
                references.Add(new ReferenceType { ReferenceName = "GAVV", FieldType = "CheckBox", ReferenceCategory = "Social", OriginReference = references[22], ReferenceOrder = 7 });
                references.Add(new ReferenceType { ReferenceName = "Apoio monetário", FieldType = "CheckBox", ReferenceCategory = "Social", ReferenceOrder = 8 });
                references.Add(new ReferenceType { ReferenceName = "INAS", FieldType = "CheckBox", ReferenceCategory = "Social", ReferenceOrder = 9 });
                references.Add(new ReferenceType { ReferenceName = "Outras redes/grupos/instituição de Apoio", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Social", ReferenceOrder = 10 });
                references.Add(new ReferenceType { ReferenceName = "Especificar cuidado/serviço prestado ou a ser prestado", FieldType = "AlphaNumericTextbox", ReferenceCategory = "Social", ReferenceOrder = 11 });

                references.ForEach(x => context.ReferenceTypes.AddOrUpdate(x));

                context.SaveChanges();


            }

            // TODO: remove deprecated

            if (context.ServiceTrackSections.ToList().Count == 0 && domains.Count > 0)
            {
                var sections = new List<ServiceTrackSection>();

                var domain = domains.FirstOrDefault(d => d.DomainID == 3);  // Health

                ServiceTrackSection section = new ServiceTrackSection { SectionName = "ACTIVISTA : Motivos de Referência", SectionOrder = 1, ServiceCategorys = new HashSet<ServiceCategory>() };

                ServiceCategory serviceCategory = new ServiceCategory { CategoryName = "Serviços SMI", CategoryOrder = 1, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "Maternidade p/ Parto", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "CPN", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "CPN Familiar", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Consulta Pós-Parto", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "CCR", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "PTV", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "CheckBox", Domain = domain });

                serviceCategory = new ServiceCategory { CategoryName = "Serviços HIV", CategoryOrder = 2, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "ATS", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "ITS", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Pré-TARV/IO", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Testado HIV+", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Abandono TARV", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "PPE", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Circuncisao Masculina", ServiceCategory = serviceCategory, ServiceOrder = 7, ServiceFieldType = "CheckBox", Domain = domain });

                serviceCategory = new ServiceCategory { CategoryName = "Serviços TB/Malária", CategoryOrder = 3, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "Suspeito de TB", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Contacto de TB", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Controlo de BK", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Abandono de TTB", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Reacções do TTB", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Suspeito de Malária", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "CheckBox", Domain = domain });

                serviceCategory = new ServiceCategory { CategoryName = "Serviços Sociais", CategoryOrder = 4, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "OCB/Apoio Comunitário", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Educação", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 2) });
                serviceCategory.Services.Add(new Service { ServiceName = "Acção Social - Protecção", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 4) });
                serviceCategory.Services.Add(new Service { ServiceName = "Acção Social - Alimentação", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 1) });
                serviceCategory.Services.Add(new Service { ServiceName = "Acção Social - Fortalecimento Económico", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 6) });
                serviceCategory.Services.Add(new Service { ServiceName = "Acção Social - Habitação", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 7) });
                serviceCategory.Services.Add(new Service { ServiceName = "GAVV", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 4) });
                serviceCategory.Services.Add(new Service { ServiceName = "Apoio Psico-Social", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 5) });
                serviceCategory.Services.Add(new Service { ServiceName = "Posto Policial", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "CheckBox", Domain = domains.FirstOrDefault(d => d.DomainID == 4) });

                serviceCategory = new ServiceCategory { CategoryName = "", CategoryOrder = 5, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "Suspeito de Malnutrição", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Banco de Socorro/Controle de triagem", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Controlo da Dor", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox", Domain = domain });
                serviceCategory.Services.Add(new Service { ServiceName = "Outros Motivos/Especificar cuidados/Serviço prestado ou a ser prestado", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Nome da Pessoa que referiu", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Data", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "Date" });

                sections.Add(section);

                // section UNIDADE SANITÁRIA

                section = new ServiceTrackSection { SectionName = "UNIDADE SANITÁRIA", SectionOrder = 2, ServiceCategorys = new HashSet<ServiceCategory>() };

                serviceCategory = new ServiceCategory { CategoryName = "", CategoryOrder = 1, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "Maternidade p/ parto", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "CPN", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "CPN Familiar", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Consulta Pós-parto", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "PTV", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "CCR", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "ATS", ServiceCategory = serviceCategory, ServiceOrder = 7, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "ITS", ServiceCategory = serviceCategory, ServiceOrder = 8, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Testado HIV+", ServiceCategory = serviceCategory, ServiceOrder = 9, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Pré TARV/IO", ServiceCategory = serviceCategory, ServiceOrder = 10, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "CD", ServiceCategory = serviceCategory, ServiceOrder = 11, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "TARV", ServiceCategory = serviceCategory, ServiceOrder = 12, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Sem TB", ServiceCategory = serviceCategory, ServiceOrder = 13, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Profilaxia por contacto TB", ServiceCategory = serviceCategory, ServiceOrder = 14, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Tratamento de TB", ServiceCategory = serviceCategory, ServiceOrder = 15, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Doenças crónicas", ServiceCategory = serviceCategory, ServiceOrder = 16, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Controlo da Dor", ServiceCategory = serviceCategory, ServiceOrder = 17, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Controlo de BK", ServiceCategory = serviceCategory, ServiceOrder = 18, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "mês", ServiceCategory = serviceCategory, ServiceOrder = 19, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Reacções do TTB", ServiceCategory = serviceCategory, ServiceOrder = 20, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "PPE", ServiceCategory = serviceCategory, ServiceOrder = 21, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Terapia Nutricional", ServiceCategory = serviceCategory, ServiceOrder = 22, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "CCS", ServiceCategory = serviceCategory, ServiceOrder = 23, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Outros atendimentos/Especifique", ServiceCategory = serviceCategory, ServiceOrder = 24, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Referido para", ServiceCategory = serviceCategory, ServiceOrder = 25, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Observações", ServiceCategory = serviceCategory, ServiceOrder = 26, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Motivo de referência", ServiceCategory = serviceCategory, ServiceOrder = 27, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Nome do trabalhador de saúde", ServiceCategory = serviceCategory, ServiceOrder = 28, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Nome da US", ServiceCategory = serviceCategory, ServiceOrder = 29, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Data", ServiceCategory = serviceCategory, ServiceOrder = 30, ServiceFieldType = "Date" });

                sections.Add(section);

                // section ACÇÃO SOCIAL

                section = new ServiceTrackSection { SectionName = "ACÇÃO SOCIAL", SectionOrder = 3, ServiceCategorys = new HashSet<ServiceCategory>() };

                serviceCategory = new ServiceCategory { CategoryName = "", CategoryOrder = 1, ServiceTrackSection = section, Services = new HashSet<Service>() };
                section.ServiceCategorys.Add(serviceCategory);

                serviceCategory.Services.Add(new Service { ServiceName = "Subsídios de alimentos", ServiceCategory = serviceCategory, ServiceOrder = 1, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Habitação", ServiceCategory = serviceCategory, ServiceOrder = 2, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Educação", ServiceCategory = serviceCategory, ServiceOrder = 3, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Atestado de pobreza", ServiceCategory = serviceCategory, ServiceOrder = 4, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "OCB", ServiceCategory = serviceCategory, ServiceOrder = 5, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Acção Social produtiva", ServiceCategory = serviceCategory, ServiceOrder = 6, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "GAVV", ServiceCategory = serviceCategory, ServiceOrder = 7, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Apoio monetário", ServiceCategory = serviceCategory, ServiceOrder = 8, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "INAS", ServiceCategory = serviceCategory, ServiceOrder = 9, ServiceFieldType = "CheckBox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Outras redes/grupos/instituição de Apoio", ServiceCategory = serviceCategory, ServiceOrder = 10, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Especificar cuidado/serviço prestado ou a ser prestado", ServiceCategory = serviceCategory, ServiceOrder = 11, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Nome do trabalhador que referiu", ServiceCategory = serviceCategory, ServiceOrder = 12, ServiceFieldType = "AlphaNumericTextbox" });
                serviceCategory.Services.Add(new Service { ServiceName = "Data", ServiceCategory = serviceCategory, ServiceOrder = 13, ServiceFieldType = "Date" });

                sections.Add(section);

                sections.ForEach(x => context.ServiceTrackSections.Add(x));

                context.SaveChanges();
            }

            if (context.SimpleEntities.Count() != 17)
            {
                // Simple Entity
                var simpleEntities = new List<SimpleEntity>();
                var fht0 = new SimpleEntity { Type = "fam-head-type", Code = "00", Description = "Nenhum" };
                var fht1 = new SimpleEntity { Type = "fam-head-type", Code = "01", Description = "Avô/Idoso" };
                var fht2 = new SimpleEntity { Type = "fam-head-type", Code = "02", Description = "Criança" };
                var fht3 = new SimpleEntity { Type = "fam-head-type", Code = "03", Description = "Mãe/Pai Solteiro" };
                var fht4 = new SimpleEntity { Type = "fam-head-type", Code = "04", Description = "Doente Crónico Debilitado" };
                var fht5 = new SimpleEntity { Type = "fam-head-type", Code = "05", Description = "Outro" };
                var fort0 = new SimpleEntity { Type = "fam-origin-ref-type", Code = "00", Description = "Nenhuma" };
                var fort1 = new SimpleEntity { Type = "fam-origin-ref-type", Code = "01", Description = "Comunidade" };
                var fort2 = new SimpleEntity { Type = "fam-origin-ref-type", Code = "02", Description = "Unidade Sanitária" };
                var fort3 = new SimpleEntity { Type = "fam-origin-ref-type", Code = "03", Description = "CHASS" };
                var fort4 = new SimpleEntity { Type = "fam-origin-ref-type", Code = "04", Description = "Outra" };
                var dok0 = new SimpleEntity { Type = "degree-of-kinship", Code = "00", Description = "Neto(a)" };
                var dok1 = new SimpleEntity { Type = "degree-of-kinship", Code = "01", Description = "Filho(a)" };
                var dok2 = new SimpleEntity { Type = "degree-of-kinship", Code = "02", Description = "Irmão(a)" };
                var dok3 = new SimpleEntity { Type = "degree-of-kinship", Code = "03", Description = "Sobrinho(a)" };
                var dok4 = new SimpleEntity { Type = "degree-of-kinship", Code = "04", Description = "Avô/Avó" };
                var dok5 = new SimpleEntity { Type = "degree-of-kinship", Code = "05", Description = "Outro" };

                simpleEntities.AddRange(new[] { fht0, fht1, fht2, fht3, fht4, fht5, fort0, fort1, fort2, fort3, fort4, dok0, dok1, dok2, dok3, dok4, dok5 });
                simpleEntities.ForEach(u => context.SimpleEntities.AddOrUpdate(x => x.Description, u));

                context.SaveChanges();
            }
        }
    }
}
