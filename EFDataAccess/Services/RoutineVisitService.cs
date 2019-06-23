using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using VPPS.CSI.Domain;
using System.Threading.Tasks;
using static VPPS.CSI.Domain.RoutineVisitSupport;
using System.Collections;
using EFDataAccess.DTO;

namespace EFDataAccess.Services
{
    public class RoutineVisitService : BaseService
    {
        private UserService UserService;
        private BeneficiaryService BeneficiaryService;
        private HouseholdService HouseholdService;

        public RoutineVisitService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            BeneficiaryService = new BeneficiaryService(uow);
            HouseholdService = new HouseholdService(uow);
        }

        public RoutineVisit fetchRoutineVisitByID(int RoutineVisitID)
        {
            return UnitOfWork.Repository<RoutineVisit>().GetAll()
                .Where(x => x.RoutineVisitID == RoutineVisitID)
                .Include(x => x.RoutineVisitMembers.Select( y => y.RoutineVisitSupports.Select(e => e.Support)))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Adult).Select(e => e.ChildStatusHistories.Select(z => z.ChildStatus)))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Child).Select(e => e.ChildStatusHistories.Select(z => z.ChildStatus)))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Beneficiary).Select(e => e.ChildStatusHistories.Select(z => z.ChildStatus)))
                .FirstOrDefault();
        }

        public RoutineVisitMember fetchRoutineVisitMemberByID(int RoutineVisitMemberID)
        {
            return UnitOfWork.Repository<RoutineVisitMember>().GetAll()
                .Where(x => x.RoutineVisitMemberID == RoutineVisitMemberID)
                .FirstOrDefault();
        }

        public RoutineVisit findRoutineVisitByHouseHoldID(int HouseholdID)
        {
            return UnitOfWork.Repository<RoutineVisit>().GetAll()
                .Where(x => x.Household.HouseHoldID == HouseholdID)
                .Include(x => x.RoutineVisitMembers.Select(y => y.RoutineVisitSupports))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Adult))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Child))
                .FirstOrDefault();
        }

        public List<RoutineVisit> findRoutineVisitsByRoutineVisitCode(string RoutineVisitCode, int HouseholdID)
        {
            return UnitOfWork.Repository<RoutineVisit>().GetAll()
                .Where(x => x.VisitRecordCode == RoutineVisitCode && x.HouseHoldID == HouseholdID)
                .Include(x => x.RoutineVisitMembers.Select(y => y.RoutineVisitSupports))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Adult).Select(e => e.ChildStatusHistories.Select(z => z.ChildStatus)))
                .Include(x => x.RoutineVisitMembers.Select(y => y.Child).Select(e => e.ChildStatusHistories.Select(z => z.ChildStatus))).ToList();
        }

        private EFDataAccess.Repository.IRepository<RoutineVisit> RoutineVisitRepository => UnitOfWork.Repository<RoutineVisit>();

        public RoutineVisit FindRoutineVisitBySyncGuid(Guid SyncGuid) => UnitOfWork.Repository<RoutineVisit>().GetAll().Where(x => x.SyncGuid == SyncGuid).FirstOrDefault();

        public RoutineVisitMember FindRoutineVisitMemberBySyncGuid(Guid SyncGuid) => UnitOfWork.Repository<RoutineVisitMember>().GetAll().Where(x => x.SyncGuid == SyncGuid).FirstOrDefault();

        public RoutineVisitSupport FindRoutineVisitSupportBySyncGuid(Guid SyncGuid) => UnitOfWork.Repository<RoutineVisitSupport>().GetAll().Where(x => x.SyncGuid == SyncGuid).FirstOrDefault();
        
        public List<UniqueEntity> FindAllRoutineVisitUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, RoutineVisitID As ID from RoutineVisit").ToList();

        public List<UniqueEntity> FindAllRoutineVisitMemberUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, RoutineVisitMemberID As ID from RoutineVisitMember").ToList();

        public List<UniqueEntity> FindAllRoutineVisitSupportUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, RoutineVisitSupportID As ID from RoutineVisitSupport").ToList();

        public List<ServiceTypeDTO> FindServiceTypesForRoutineVisit() => UnitOfWork.DbContext.Database.SqlQuery<ServiceTypeDTO>("select TypeCode, TypeDescription from SupportServiceType Where TypeCode is not null and TypeDescription is not null group by TypeCode, TypeDescription, DomainOrder order by DomainOrder").ToList();

        public List<RoutineVisitMember> findAllRoutineVisitMembers() => UnitOfWork.Repository<RoutineVisitMember>().GetAll().Include(x => x.Adult).Include(x => x.Child).ToList();

        public RoutineVisit Reload(RoutineVisit entity) { RoutineVisitRepository.FullReload(entity); return entity; }

        public void SaveOrUpdateRoutineVisit(RoutineVisit RoutineVisit)
        {
            if (RoutineVisit.RoutineVisitID == 0) { RoutineVisitRepository.Add(RoutineVisit); }
            else { RoutineVisitRepository.Update(RoutineVisit); }
        }

        public void SaveRoutineVisitMember(RoutineVisitMember RoutineVisitMember)
        {
            if (RoutineVisitMember.RoutineVisitMemberID == 0)
            { UnitOfWork.Repository<RoutineVisitMember>().Add(RoutineVisitMember); }
        }

        public void SaveOrUpdateRoutineVisitSupport(RoutineVisitSupport RoutineVisitSupport)
        {
            if (RoutineVisitSupport.RoutineVisitSupportID == 0)
            { UnitOfWork.Repository<RoutineVisitSupport>().Add(RoutineVisitSupport); }
            else
            { UnitOfWork.DbContext.Entry(RoutineVisitSupport).State = EntityState.Modified; }
        }

        public void CreateRoutineVisitFull(RoutineVisit routineVisit)
        {
            routineVisit.TransformDateToCode();
            routineVisit = PrepareToPersist(routineVisit);
            UnitOfWork.Repository<RoutineVisit>().Add(routineVisit);
            routineVisit.RoutineVisitMembers.ToList().ForEach(x => UnitOfWork.Repository<RoutineVisitMember>().Add(x));
            routineVisit.RoutineVisitMembers.ToList().ForEach(x => x.RoutineVisitSupports.ToList().ForEach(y => UnitOfWork.Repository<RoutineVisitSupport>().Add(y)));
            Commit();
        }

        public void UpdateRoutineVisitFull(RoutineVisit routineVisit)
        {
            routineVisit.TransformDateToCode();
            routineVisit = PrepareToPersist(routineVisit);
            routineVisit.RoutineVisitMembers.ToList().ForEach(x => x.RoutineVisitSupports.ToList().ForEach(y => UnitOfWork.Repository<RoutineVisitSupport>().Update(y)));
            routineVisit.RoutineVisitMembers.ToList().ForEach(x => UnitOfWork.Repository<RoutineVisitMember>().Update(x));
            routineVisit.RoutineVisitMembers = null;
            UnitOfWork.Repository<RoutineVisit>().Update(routineVisit);
            Commit();
        }

        public void DeleteRoutineVisitFull(RoutineVisit routineVisit)
        {
            routineVisit.RoutineVisitMembers.ToList().ForEach(x => x.RoutineVisitSupports.ToList().ForEach(y => UnitOfWork.Repository<RoutineVisitSupport>().Delete(y)));
            routineVisit.RoutineVisitMembers.ToList().ForEach(x => UnitOfWork.Repository<RoutineVisitMember>().Delete(x));
            UnitOfWork.Repository<RoutineVisit>().Delete(routineVisit);
            Commit();
        }

        public RoutineVisit CreateRoutineVisitForHouseholdWithouSave(Household household)
        {
            RoutineVisit routineVisit = new RoutineVisit();
            routineVisit.RoutineVisitDate = null;
            routineVisit.HouseHoldID = household.HouseHoldID;
            routineVisit.Version = "v1";

            RoutineVisitSupport support;
            RoutineVisitMember member;

            List<DomainEntity> domains = UnitOfWork.Repository<DomainEntity>().GetAll().ToList();

            foreach (Beneficiary b in household.Beneficiaries)
            {
                member = new RoutineVisitMember { BeneficiaryID = b.BeneficiaryID, MemberFullName = b.FullName, Beneficiary = b };
                support = new RoutineVisitSupport { SupportID = 1, SupportType = "SAVINGS_GROUP_MEMBER", SupportValue = "False" };
                member.RoutineVisitSupports.Add(support);

                foreach (DomainEntity domain in domains.OrderBy(x => x.OrderForRoutineVisit))
                {
                    support = new RoutineVisitSupport { SupportID = domain.DomainID, SupportType = domain.GetType().Name, SupportValue = "0" };
                    member.RoutineVisitSupports.Add(support);
                }

                support = new RoutineVisitSupport { SupportID = 1, SupportType = "DPI", SupportValue = "0" };
                member.RoutineVisitSupports.Add(support);
                support = new RoutineVisitSupport { SupportID = (int)MUAC.GREEN, SupportType = "MUAC", SupportValue = "0" };
                member.RoutineVisitSupports.Add(support);
                support = new RoutineVisitSupport { SupportID = (int)MUAC.YELLOW, SupportType = "MUAC", SupportValue = "0" };
                member.RoutineVisitSupports.Add(support);
                support = new RoutineVisitSupport { SupportID = (int)MUAC.RED, SupportType = "MUAC", SupportValue = "0" };
                member.RoutineVisitSupports.Add(support);
                support = new RoutineVisitSupport { SupportID = 1, SupportType = "COMMENT", SupportValue = "" };
                member.RoutineVisitSupports.Add(support);
                support = new RoutineVisitSupport { SupportID = 1, SupportType = "HIVRISK", SupportValue = "False" };
                member.RoutineVisitSupports.Add(support);
                routineVisit.RoutineVisitMembers.Add(member);
            }

            //foreach (Child c in household.Children.Where( c => ! c.BeneficiaryStatus().Equals("Adulto")))
            //{
            //    member = new RoutineVisitMember { ChildID = c.ChildID, MemberFullName = c.FullName, Child = c, BeneficiaryAgeInMonths = c.AgeInMonths, BeneficiaryStatus = c.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate).ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description };
            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "SAVINGS_GROUP_MEMBER", SupportValue = "False" };
            //    member.RoutineVisitSupports.Add(support);

            //    foreach (DomainEntity domain in domains.OrderBy(x => x.OrderForRoutineVisit))
            //    {
            //        support = new RoutineVisitSupport { SupportID = domain.DomainID, SupportType = domain.GetType().Name, SupportValue = "0" };
            //        member.RoutineVisitSupports.Add(support);
            //    }

            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "DPI", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = (int) MUAC.GREEN, SupportType = "MUAC", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = (int) MUAC.YELLOW, SupportType = "MUAC", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = (int) MUAC.RED, SupportType = "MUAC", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "COMMENT", SupportValue = "" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "HIVRISK", SupportValue = "False" };
            //    member.RoutineVisitSupports.Add(support);
            //    routineVisit.RoutineVisitMembers.Add(member);
            //}

            //foreach (Adult a in household.Adults)
            //{
            //    member = new RoutineVisitMember { AdultId = a.AdultId, MemberFullName = a.FullName, Adult = a };
            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "SAVINGS_GROUP_MEMBER", SupportValue = "False" };
            //    member.RoutineVisitSupports.Add(support);

            //    foreach (DomainEntity domain in domains.OrderBy(x => x.OrderForRoutineVisit))
            //    {
            //        support = new RoutineVisitSupport {SupportID = domain.DomainID, SupportType = domain.GetType().Name, SupportValue = "0"};
            //        member.RoutineVisitSupports.Add(support);
            //    }

            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "DPI", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = (int) MUAC.GREEN, SupportType = "MUAC", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = (int) MUAC.YELLOW, SupportType = "MUAC", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = (int) MUAC.RED, SupportType = "MUAC", SupportValue = "0" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "COMMENT", SupportValue = "" };
            //    member.RoutineVisitSupports.Add(support);
            //    support = new RoutineVisitSupport { SupportID = 1, SupportType = "HIVRISK", SupportValue = "False" };
            //    member.RoutineVisitSupports.Add(support);
            //    routineVisit.RoutineVisitMembers.Add(member);
            //}

            return routineVisit;
        }

        public RoutineVisit CreateNewRoutineVisitForHouseholdWithouSave(Household household)
        {
            RoutineVisit routineVisit = new RoutineVisit();
            routineVisit.RoutineVisitDate = null;
            routineVisit.HouseHoldID = household.HouseHoldID;
            routineVisit.Version = "v2";

            RoutineVisitSupport support;
            RoutineVisitMember member;

            List<DomainEntity> domains = UnitOfWork.Repository<DomainEntity>().GetAll().ToList();

            foreach (Beneficiary b in household.Beneficiaries.Where(c => !c.BeneficiaryStatus().Equals("Adulto")))
            {
                member = new RoutineVisitMember { BeneficiaryID = b.BeneficiaryID, MemberFullName = b.FullName,
                                                  Beneficiary = b, BeneficiaryStatus = b.BeneficiaryStatus()};

                foreach (SupportServiceType sst in UnitOfWork.Repository<SupportServiceType>().GetAll().AsQueryable().Where(x => x.Tool.Equals("routine-visit")).ToList())
                {
                    support = new RoutineVisitSupport { Support = sst, SupportID = sst.SupportServiceTypeID, SupportType = sst.TypeCode };
                    member.RoutineVisitSupports.Add(support);
                }

                routineVisit.RoutineVisitMembers.Add(member);
            }

            return routineVisit;
        }

        public RoutineVisit PrepareToPersist(RoutineVisit routineVisit)
        {
            foreach (RoutineVisitMember rvm in routineVisit.RoutineVisitMembers)
            {
                // Evitar persistencia de beneficiarios
                rvm.Beneficiary = null;
                rvm.RoutineVisit = null;
            }

            foreach (RoutineVisitMember rvm in routineVisit.RoutineVisitMembers)
            {
                foreach (RoutineVisitSupport rvs in rvm.RoutineVisitSupports)
                {
                    // Evitar persistencia de beneficiarios
                    rvs.RoutineVisitMember = null;
                    // Verificar e settar o campo que informa se os Beneficiario tem ou nao servicos
                    rvm.BeneficiaryHasServices = (rvs.Checked && "v2".Equals(routineVisit.Version)) ? true : rvm.BeneficiaryHasServices;
                }
            }

            return routineVisit;
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO FICHAS DE VISITA ...");

            int ImportedRoutineVisitObjects = 0;
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());
            List<RoutineVisit> RoutineVisitsToPersist = new List<RoutineVisit>();
            List<Guid> excludedObjects = new List<Guid>();
            FileImporter imp = new FileImporter();
            string lastGuidToImport = null;

            try
            {
                // ################## Import Routine Visit  ################

                string fullPathRV = path + @"\RoutineVisits.csv";
                Hashtable HouseholdAll = ConvertListToHashtable(HouseholdService.findAllHouseholdUniqueEntities());
                Hashtable RoutineVisitAll = ConvertListToHashtable(FindAllRoutineVisitUniqueEntity());
                IEnumerable<DataRow> rowsRV = imp.ImportFromCSV(fullPathRV).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsRV, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid RoutineVisitGuid = new Guid(row["RoutineVisit_guid"].ToString());
                    Guid HouseHoldGuid = new Guid(row["HouseHoldGuid"].ToString());
                    lastGuidToImport = RoutineVisitGuid.ToString();
                    RoutineVisit RoutineVisit = (RoutineVisitAll[RoutineVisitGuid] == null) ? new RoutineVisit() : FindRoutineVisitBySyncGuid(RoutineVisitGuid);
                    RoutineVisit.HouseHoldID = ParseStringToIntSafe(HouseholdAll[HouseHoldGuid]);

                    if (RoutineVisit.HouseHoldID == 0)
                    {
                        _logger.Error("Ficha de seguimento (RoutineVisit) '{0}' nao importada, porque o agregado '{1}' nao existe na BD", RoutineVisitGuid, HouseHoldGuid);
                        excludedObjects.Add(RoutineVisitGuid);
                    }
                    else
                    {
                        RoutineVisit.RoutineVisitDate = (row["RoutineVisitDate"].ToString()).Length == 0 ? RoutineVisit.RoutineVisitDate : DateTime.Parse(row["RoutineVisitDate"].ToString());
                        RoutineVisit.FamilyKitReceived = row["FamilyKitReceivedInt"].ToString().Equals("1") ? true : false;
                        RoutineVisit.FirstTimeSavingGroupMember = row["FirstTimeSavingGroupMemberInt"].ToString().Equals("1") ? true : false;
                        RoutineVisit.VisitRecordCode = row["VisitRecordCode"].ToString();
                        RoutineVisit.Version = row["Version"].ToString();
                        SetCreationDataFields(RoutineVisit, row, RoutineVisitGuid);
                        SetUpdatedDataFields(RoutineVisit, row);
                        RoutineVisitsToPersist.Add(RoutineVisit);
                        ImportedRoutineVisitObjects++;
                    }

                    if (ImportedRoutineVisitObjects % 100 == 0)
                    { _logger.Information(ImportedRoutineVisitObjects + " de " + rowsRV.Count() + " Objectos da Ficha de Visita (RoutineVisit) importados."); }
                });
                RoutineVisitsToPersist.ForEach(x => SaveOrUpdateRoutineVisit(x));
                UnitOfWork.Commit();
                Rename(fullPathRV, fullPathRV + IMPORTED);

                // ################## Import Routine Visit Member ################ 

                ImportedRoutineVisitObjects = 0;
                string fullPathRVM = path + @"\RoutineVisitMembers.csv";
                RoutineVisitAll = ConvertListToHashtable(FindAllRoutineVisitUniqueEntity());
                Hashtable RoutineVisitMemberAll = ConvertListToHashtable(FindAllRoutineVisitMemberUniqueEntity());
                List<UniqueEntity> BeneficiaryAll = BeneficiaryService.FindAllBeneficiaryUniqueEntities();
                List<RoutineVisitMember> RoutineVisitMembersToPersist = new List<RoutineVisitMember>();
                IEnumerable<DataRow> rowsRVM = imp.ImportFromCSV(fullPathRVM).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsRVM, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row => 
                {
                    Guid RoutineVisitMemberGuid = new Guid(row["RoutineVisitMember_guid"].ToString());
                    Guid RoutineVisitGuid = new Guid(row["RoutineVisitGuid"].ToString());
                    Guid BeneficiaryGuid = new Guid(row["BeneficiaryGuid"].ToString());
                    RoutineVisitMember RoutineVisitMember = null;
                    lastGuidToImport = RoutineVisitMemberGuid.ToString();

                    if (excludedObjects.Contains(RoutineVisitGuid))
                    {
                        _logger.Error("Ficha de seguimento (RoutineVisitMember) '{0}' nao importada, porque o obj RoutineVisit '{1}' foi excluido da importacao", RoutineVisitMemberGuid, RoutineVisitGuid);
                        excludedObjects.Add(RoutineVisitMemberGuid);
                    }
                    else
                    {
                        if (RoutineVisitMemberAll[RoutineVisitMemberGuid] == null)
                        {
                            UniqueEntity Beneficiary = FindBySyncGuid(BeneficiaryAll, BeneficiaryGuid);
                            RoutineVisitMember = new RoutineVisitMember();

                            RoutineVisitMember.RoutineVisitID = ParseStringToIntSafe(RoutineVisitAll[RoutineVisitGuid]);
                            RoutineVisitMember.BeneficiaryHasServices = row["DoesBeneficiaryHasServices"].ToString().Equals("1") ? true : false;
                            RoutineVisitMember.RoutineVisitDate = (row["RoutineVisitDate"].ToString()).Length == 0 ? RoutineVisitMember.RoutineVisitDate : DateTime.Parse(row["RoutineVisitDate"].ToString());
                            SetCreationDataFields(RoutineVisitMember, row, RoutineVisitMemberGuid);
                            SetUpdatedDataFields(RoutineVisitMember, row);

                            if (Beneficiary != null)
                            {
                                RoutineVisitMember.BeneficiaryID = Beneficiary.ID;
                                RoutineVisitMembersToPersist.Add(RoutineVisitMember);
                                ImportedRoutineVisitObjects++;
                            }
                            else
                            {
                                _logger.Error("Não existe nem Beneficiario com o Guid '{0}'. O RoutineVisitMemeber com Guid '{1}' nao sera importado.", BeneficiaryGuid, RoutineVisitMemberGuid);
                                excludedObjects.Add(RoutineVisitMemberGuid);
                            }


                            if (ImportedRoutineVisitObjects % 100 == 0)
                            { _logger.Information(ImportedRoutineVisitObjects + " de " + rowsRVM.Count() + " Objectos da Ficha de Visita (RoutineVisitMember) importados."); }
                        }
                    }
                });
                RoutineVisitMembersToPersist.ForEach(x => SaveRoutineVisitMember(x));
                UnitOfWork.Commit();
                Rename(fullPathRVM, fullPathRVM + IMPORTED);

                // ################## Import Routine Visit Support ################

                ImportedRoutineVisitObjects = 0;
                string fullPathRVS = path + @"\RoutineVisitSupports.csv";
                RoutineVisitMemberAll = ConvertListToHashtable(FindAllRoutineVisitMemberUniqueEntity());
                Hashtable RoutineVisitSupportAll = ConvertListToHashtable(FindAllRoutineVisitSupportUniqueEntity());
                List<RoutineVisitSupport> RoutineVisitSupportsToPersist = new List<RoutineVisitSupport>();
                IEnumerable<DataRow> rowsRVS = imp.ImportFromCSV(fullPathRVS).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsRVS, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row => 
                {
                    Guid RoutineVisitSupportGuid = new Guid(row["RoutineVisitSupport_guid"].ToString());
                    Guid RoutineVisitMemberGuid = new Guid(row["RoutineVisitMemberGuid"].ToString());
                    lastGuidToImport = RoutineVisitSupportGuid.ToString();
                    RoutineVisitSupport RoutineVisitSupport = (RoutineVisitSupportAll[RoutineVisitSupportGuid] == null) ? new RoutineVisitSupport() : FindRoutineVisitSupportBySyncGuid(RoutineVisitSupportGuid);
                    RoutineVisitSupport.RoutineVisitSupportID = ParseStringToIntSafe(RoutineVisitSupportAll[RoutineVisitSupportGuid]);
                    RoutineVisitSupport.SupportType = row["SupportType"].ToString();
                    RoutineVisitSupport.SupportValue = row["SupportValue"].ToString();
                    RoutineVisitSupport.SupportID = int.Parse(row["SupportID"].ToString());
                    RoutineVisitSupport.Checked = row["IsCheckedInt"].ToString().Equals("1") ? true : false;
                    RoutineVisitSupport.RoutineVisitMemberID = ParseStringToIntSafe(RoutineVisitMemberAll[RoutineVisitMemberGuid]);
                    SetCreationDataFields(RoutineVisitSupport, row, RoutineVisitSupportGuid);

                    if (excludedObjects.Contains(RoutineVisitMemberGuid))
                    {
                        _logger.Error("Ficha de seguimento (RoutineVisitSupport) '{0}' nao importada, porque o obj RoutineVisitMember '{1}' foi excluido da importacao", RoutineVisitSupportGuid, RoutineVisitMemberGuid);
                        excludedObjects.Add(RoutineVisitSupportGuid);
                    }
                    else
                    {
                        if (RoutineVisitSupport.RoutineVisitSupportID == 0 || (RoutineVisitSupport.RoutineVisitSupportID > 0 && !row["SupportValue"].ToString().Equals(RoutineVisitSupport.SupportValue)))
                        {
                            RoutineVisitSupport.SupportValue = row["SupportValue"].ToString();
                            SetUpdatedDataFields(RoutineVisitSupport, row);
                            RoutineVisitSupportsToPersist.Add(RoutineVisitSupport);
                            ImportedRoutineVisitObjects++;
                        }

                        if (ImportedRoutineVisitObjects != 0 && ImportedRoutineVisitObjects % 500 == 0)
                        { _logger.Information(ImportedRoutineVisitObjects + " de " + rowsRVS.Count() + " Objectos da Ficha de Visita (RoutineVisitSupport) importados."); }
                    }
                });
                RoutineVisitSupportsToPersist.ForEach(x => SaveOrUpdateRoutineVisitSupport(x));
                UnitOfWork.Commit();
                Rename(fullPathRVS, fullPathRVS + IMPORTED);

            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Fichas de Visita", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return RoutineVisitsToPersist.Count();
        }

        public int LockData() => LockAndUnlockData(3, new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 20));

        public int UnlockData(DateTime unlockDate) => LockAndUnlockData(0, unlockDate);

        public int LockAndUnlockData(int state, DateTime lockOrUnlockDate)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            List<RoutineVisit> routineVisitsToLockOrUnlock = UnitOfWork.Repository<RoutineVisit>().GetAll()
                .Include(x => x.RoutineVisitMembers.Select( y => y.RoutineVisitSupports))
                .Where(x => x.RoutineVisitDate <= lockOrUnlockDate).ToList();

            foreach (RoutineVisit rv in routineVisitsToLockOrUnlock)
            {
                // Routine Visit
                rv.State = state;
                SaveOrUpdateRoutineVisit(rv);

                // RoutineVisitMember
                rv.RoutineVisitMembers.ToList().ForEach(x => x.State = state);
                rv.RoutineVisitMembers.ToList().ForEach(x => SaveRoutineVisitMember(x));

                // RoutineVisitSupport
                rv.RoutineVisitMembers.ToList().ForEach(x => x.RoutineVisitSupports.ToList().ForEach(y => y.State = state));
                rv.RoutineVisitMembers.ToList().ForEach(x => x.RoutineVisitSupports.ToList().ForEach(y => SaveOrUpdateRoutineVisitSupport(y)));
            }

            Commit();
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
            return routineVisitsToLockOrUnlock.Count();
        }
    }
}
