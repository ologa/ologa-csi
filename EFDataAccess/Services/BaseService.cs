using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using EFDataAccess.Logging;
using System.IO;
using System.Collections;
using EFDataAccess.DTO;
using Utilities;

namespace EFDataAccess.Services
{
    public class BaseService
    {
        public const string ZEROSGUID = "00000000-0000-0000-0000-000000000000";
        public const string IMPORTED = ".imported";
        public const int MAXTHREADS = 1;
        protected UnitOfWork UnitOfWork { get; set; }
        protected Hashtable UsersDB { get; set; }
        protected UserInfo UserInfo { get; set; }
        protected ILogger _logger;

        public BaseService(){}

        public BaseService(UnitOfWork uow)
        {
            UnitOfWork = uow;
            _logger = new Logger();
        }

        public void MarkEntityAsSynced(List<int> ids, string table)
        {
            UnitOfWork.DbContext.Database.ExecuteSqlCommand("Update [" + table + "] Set SyncState = 1"); // " where id in (" + String.Join(",", ids) + ")");
            UnitOfWork.Commit();
        }

        public List<UniqueEntity> findAllUsersUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, UserID As ID from  [User] where SyncGuid is not null").ToList();

        public UniqueEntity FindBySyncGuid(List<UniqueEntity> list, Guid SyncGuid) => list.Where(x => x.SyncGuid == SyncGuid).SingleOrDefault();

        public bool isZerosGuid(String Guid) => ZEROSGUID.Equals(Guid);

        public void SetCreationDataFields(AuditedEntity AuditedEntity, DataRow row, Guid Guid)
        {
            if (AuditedEntity.CreatedDate != null && AuditedEntity.SyncDate != null) return;

            String userGuid = (row["CreatedUserGuid"].ToString() == "" || row["CreatedUserGuid"] == null) ? userGuid = ZEROSGUID : userGuid = row["CreatedUserGuid"].ToString();
            Guid CreatedUserGuid = new Guid(userGuid);
            AuditedEntity.CreatedDate = (row["CreatedDate"].ToString()).Length == 0 ? AuditedEntity.CreatedDate : DateTime.Parse(row["CreatedDate"].ToString());
            AuditedEntity.CreatedUser = ZEROSGUID.Equals(CreatedUserGuid.ToString()) ? AuditedEntity.CreatedUser : (User) UsersDB[CreatedUserGuid];
            AuditedEntity.SyncDate = DateTime.Now;
            AuditedEntity.SyncGuid = Guid;
        }

        public void SetUpdatedDataFields(AuditedEntity AuditedEntity, DataRow row)
        {
            String userGuid = (row["LastUpdatedUserGuid"].ToString() == "" || row["LastUpdatedUserGuid"] == null) ? userGuid = ZEROSGUID : userGuid = row["LastUpdatedUserGuid"].ToString();
            Guid LastUpdatedUserGuid = new Guid(userGuid);
            AuditedEntity.LastUpdatedDate = (row["LastUpdatedDate"].ToString()).Length == 0 ? AuditedEntity.LastUpdatedDate : DateTime.Parse(row["LastUpdatedDate"].ToString());
            AuditedEntity.LastUpdatedUser = ZEROSGUID.Equals(LastUpdatedUserGuid) ? AuditedEntity.LastUpdatedUser : (User) UsersDB[LastUpdatedUserGuid];
            AuditedEntity.SyncDate = DateTime.Now;
        }

        public void Rename(string actual, string tobe)
        {
            if (System.IO.File.Exists(actual))
            {
                System.IO.File.Move(actual, tobe);
            }
        }

        public Hashtable ConvertListToHashtable(List<UniqueEntity> list)
        {
            Hashtable ht = new Hashtable();
            foreach (var obj in list)
            {
                if (!ht.ContainsKey(obj.SyncGuid))
                {
                    ht.Add(obj.SyncGuid, obj.ID);
                }
            }

            return ht;
        }

        public Hashtable ConvertListToHashtableUsers(List<UniqueEntity> list)
        {
            Hashtable ht = new Hashtable();
            foreach (var obj in list) { ht.Add(obj.SyncGuid, UnitOfWork.Repository<User>().GetById(obj.ID)); }
            return ht;
        }

        protected int ParseStringToIntSafe(object obj)
        {
            return (obj == null) ? 0 : int.Parse(obj.ToString());
        }

        protected int? ParseStringToIntNullable(string number)
        {
            int convertedNumber;
            if (int.TryParse(number, out convertedNumber)) { return convertedNumber; }
            else { return null; }
        }

        // Workaround to manage duplications for a while
        public int getNonDuplicatedOrgUnitID(Hashtable orgUnitsDB, int id, string Guid)
        {
            Hashtable orgUnitsMap = new Hashtable();
            orgUnitsMap.Add(3313, 3316); // Matola (Cidade)
            orgUnitsMap.Add(3314, 2133); // (Infulene)
            orgUnitsMap.Add(3315, 2134); // (Machava)

            if (!orgUnitsDB.ContainsValue(id))
            { _logger.Information("Unidade orgânica '{0}' não existe nesta base de dados para o agregado '{1}'.", id, Guid); }

            bool needToChangeTheID = orgUnitsMap.ContainsKey(id);

            if (needToChangeTheID)
            { _logger.Information("Guid '{0}' com Unidade orgânica '{1}' passou a estar associado a Unidade '{2}'.", Guid, id, orgUnitsMap[id]); }

            return needToChangeTheID ? (int) orgUnitsMap[id] : id;
        }

        public int getCorrectSupportServiceTypeID(Hashtable supportServiceTypesDB, int id, string Guid)
        {
            Hashtable supportServiceTypeIdMapping = new Hashtable();
            supportServiceTypeIdMapping.Add(8, 14);
            supportServiceTypeIdMapping.Add(18, 17);
            supportServiceTypeIdMapping.Add(42, 47);
            supportServiceTypeIdMapping.Add(43, 47);
            supportServiceTypeIdMapping.Add(48, 47);
            supportServiceTypeIdMapping.Add(49, 72);
            supportServiceTypeIdMapping.Add(50, 74);
            supportServiceTypeIdMapping.Add(51, 75);
            supportServiceTypeIdMapping.Add(52, 70);
            supportServiceTypeIdMapping.Add(53, 73);
            supportServiceTypeIdMapping.Add(54, 71);
            supportServiceTypeIdMapping.Add(55, 86);
            supportServiceTypeIdMapping.Add(56, 77);
            supportServiceTypeIdMapping.Add(57, 78);
            supportServiceTypeIdMapping.Add(58, 75);    
            supportServiceTypeIdMapping.Add(79, 75);
            supportServiceTypeIdMapping.Add(76, 86);

            if (supportServiceTypesDB.ContainsValue(id))
            {
                return id;
            }

            if(!supportServiceTypeIdMapping.ContainsKey(id))
            {
                throw new SystemException("O SupportServiceTypeID ("+id+") nao existe na base e nao esta mapeado");
            }

            return  (int)supportServiceTypeIdMapping[id];
        }


        public void Commit()
        {
            UnitOfWork.Commit();
        }
    }
}