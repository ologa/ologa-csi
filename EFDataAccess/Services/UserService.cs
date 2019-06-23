using EFDataAccess.UOW;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPPS.CSI.Domain;
using System.IO;
using System.Security.Cryptography;
using System.Data.Entity;
using EFDataAccess.DTO;
using System.Data.SqlClient;

namespace EFDataAccess.Services
{
    public class UserService : BaseService
    {
        public UserService(UnitOfWork uow) : base(uow)
        {}

        public UserService()
        {
        }

        private EFDataAccess.Repository.IRepository<User> UserRepository
        {
            get { return UnitOfWork.Repository<User>(); }
        }

        public User Reload(User entity)
        {
            UserRepository.FullReload(entity);
            return entity;
        }

        public User findByID(int ID)
        {
            return UnitOfWork.Repository<User>().GetById(ID);
        }

        public List<User> findAll()
        {
            return UserRepository.GetAll().ToList();
        }

        public User findBySyncGuid(Guid UserSyncGuid)
        {
            return UserRepository.GetAll().Where(e => e.SyncGuid == UserSyncGuid).FirstOrDefault();
        }

        public Language findLanguage(int LanguageID)
        {
            IEnumerable<Language> languagesList = UnitOfWork.Repository<Language>().Get().Where(x => x.LanguageID == LanguageID);
            return languagesList.Any() ? languagesList.Single() : null;
        }

        public Role findRole(int RoleID)
        {
            IEnumerable<Role> rolesList = UnitOfWork.Repository<Role>().Get().Where(x => x.RoleID == RoleID);
            return rolesList.Any() ? rolesList.Single() : null;
        }

        public void Delete(User User)
        {
            UserRepository.Delete(User);
        }

        public void SaveOrUpdate(User User)
        {
            if (User.UserID == 0)
            { UnitOfWork.Repository<User>().Add(User); }
            else
            { UnitOfWork.Repository<User>().Update(User); }
        }

        public void UpdateFull(User user)
        {
            List<UserRole> dbUserRoles = UnitOfWork.Repository<UserRole>().GetAll()
                .Where(x => x.UserID == user.UserID).ToList();

            // Delete, "removed" roles
            foreach (UserRole DBaseUserRole in dbUserRoles)
            {
                // check if role was removed, if so delet it
                if (!user.UserRoles.Where(x => x.RoleID == DBaseUserRole.RoleID).Any())
                { UnitOfWork.Repository<UserRole>().Delete(DBaseUserRole); }
            }

            // Create, new roles
            foreach (UserRole NewUserRole in user.UserRoles)
            {
                // check if role exists on db, if not create it
                if (NewUserRole.UserRoleID == 0)
                {
                    NewUserRole.User = null; NewUserRole.Role = null;
                    UnitOfWork.Repository<UserRole>().Add(NewUserRole);
                }
            }

            // Clean UserRoles before update
            user.UserRoles = null;

            // Update user
            UnitOfWork.Repository<User>().Update(user);
            UnitOfWork.Commit();
        }

        public void CreateFull(User user)
        {
            // Update user
            UnitOfWork.Repository<User>().Add(user);

            // Create, new roles
            foreach (UserRole NewUserRole in user.UserRoles)
            {
                NewUserRole.UserID = user.UserID;
                NewUserRole.User = null; NewUserRole.Role = null;
                UnitOfWork.Repository<UserRole>().Add(NewUserRole);
            }

            Commit();
        }

        public User findByUsername(string Username) => UserRepository.GetAll().Where(u => u.Username == Username).FirstOrDefault();

        public IQueryable<User> findByContainsUsername(string Username) => UserRepository.GetAll().Where(u => u.Username.Contains(Username));

        public User findUserByUsernameAndPassword(string Username, string Password)
        {
            byte[] PasswordInBytes = Encrypt(Password);

            return UserRepository.GetAll().
                Include(u => u.UserRoles).
                Include(u => u.UserRoles.Select(r => r.Role)).
                Where(u => u.Username == Username && u.Password == PasswordInBytes).FirstOrDefault();
        }

        public byte[] Encrypt(String password)
        {
            byte[] pwd = new byte[100001];
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider key = new DESCryptoServiceProvider();

            string k = "WLFPSMXR";
            byte[] key1 = new byte[17];
            key1 = Encoding.UTF8.GetBytes(k);
            byte[] vec = new byte[17];

            CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(key1, vec), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(encStream);
            sw.WriteLine(password);
            sw.Close();
            encStream.Close();
            pwd = ms.ToArray();
            ms.Close();

            return pwd;
        }

        public void CacheUserInfo(User user)
        {
            UnitOfWork.CacheUserInfo(user);
        }

        /*
        * ################################################
        * ########## UserRecordCountingReport ############
        * ################################################
        */

        public List<UserRecordCountingReportDTO> getUserRecordCountingReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            ISNULL(agregados.FullName,0) AS FullName
	                            ,ISNULL(agregados.HouseholdTotal,0) AS HouseholdTotal
	                            ,ISNULL(mac.CSITotal ,0) AS CSITotal
	                            ,ISNULL(pAccao.CarePlanTotal,0) AS CarePlanTotal
	                            ,ISNULL(ficha.RoutineVisitTotal,0) AS RoutineVisitTotal
	                            ,ISNULL(GuiaContra.ReferenceServiceTotal,0) AS ReferenceServiceTotal
	                            ,ISNULL(GuiaContra.CounterReferenceServiceTotal,0) AS CounterReferenceServiceTotal
                            FROM 
                            ----------INICIO DO JOIN DO (AGREGADO = MAC)
                            (--AGREGADOS
	                            SELECT
			                            (u.FirstName) + ' ' + (u.LastName) As FullName
			                            ,COUNT(distinct hh.HouseHoldID) AS HouseholdTotal
	                            FROM 
			                             HouseHold AS hh
		                            LEFT JOIN 
			                             [User] AS u ON hh.CreatedUserID = u.UserID
		                            WHERE 
			                            hh.CreatedDate >= @initialDate AND hh.CreatedDate < @lastDate 
			                            --hh.CreatedDate >= '2017/01/01' AND hh.CreatedDate < '2017/10/01'
			                            --hh.CreatedDate < '2017/08/01'

	                            GROUP BY 
		                            u.FirstName,u.LastName
	                            --ORDER BY
		                            --u.FirstName
                            )
                            agregados LEFT JOIN
                            (--MAC's
	                            SELECT
			                            (u.FirstName) + ' ' + (u.LastName) As FullName
			                            ,COUNT(distinct mac.CSIID) AS CSITotal
	                            FROM 
			                             [User] AS u
		                            LEFT JOIN 
			                             CSI AS mac ON mac.CreatedUserID = u.UserID
		                            WHERE 
			                            mac.CreatedDate >= @initialDate AND mac.CreatedDate < @lastDate 
			                            --mac.CreatedDate >= '2017/01/01' AND mac.CreatedDate < '2017/10/01'
			                            --mac.CreatedDate < '2017/08/01'
	                            GROUP BY 
		                            u.FirstName,u.LastName
	                            --ORDER BY
		                            --u.FirstName
                            )
                            mac
                            ON
                            (
	                            agregados.FullName = mac.FullName
                            )
                            ----------FIM DO JOIN DO (AGREGADO = MAC)

                            ----------INICIO DO JOIN DO (PLANO DE ACCAO = FICHA SEGUIMENTO, GUIA REF, CONTRA REF)
                            LEFT JOIN
                            (--PLANO DE ACCAO
	                            SELECT
			                            (u.FirstName) + ' ' + (u.LastName) As FullName
			                            ,COUNT(distinct cplan.CSIID) AS CarePlanTotal
	                            FROM 
			                             [User] AS u 
		                            LEFT JOIN
			                             CarePlan AS cplan ON cplan.CreatedUserID = u.UserID
		                            WHERE
			                            cplan.CreatedDate >= @initialDate AND cplan.CreatedDate < @lastDate 
			                            --cplan.CreatedDate >= '2017/01/01' AND cplan.CreatedDate < '2017/10/01'
			                            --cplan.CreatedDate < '2017/08/01'
	                            GROUP BY 
		                            u.FirstName,u.LastName
	                            --ORDER BY
		                            --u.FirstName
                            )
                            pAccao 
                            ON
                            (
	                            mac.FullName = pAccao.FullName
                            )
                            LEFT JOIN
                            (--FICHA SEGUIMENTO, GUIA REF, CONTRA REF
	                            SELECT
		                            (u.FirstName) + ' ' + (u.LastName) As FullName
		                            ,COUNT(distinct hh.HouseHoldID) AS HouseholdTotal
		                            ,COUNT(rv.HouseHoldID) AS RoutineVisitTotal
		                            --,COUNT(distinct rs.ReferenceServiceID) AS ReferenceServiceTotal
		                            --,SUM(CASE WHEN rs.HealthAttendedDate<>'' OR rs.SocialAttendedDate<>'' THEN 1 ELSE 0 END) as CounterReferenceServiceTotal
	                            FROM 
		                             HouseHold AS hh
	                            LEFT JOIN 
		                             [User] AS u ON hh.CreatedUserID = u.UserID
	                            LEFT JOIN
		                             RoutineVisit AS rv ON rv.HouseholdID = hh.HouseHoldID
	                            --LEFT JOIN
		                            -- RoutineVisitMember AS rvm ON rvm.RoutineVisitID = rv.RoutineVisitID
	                            --LEFT JOIN 
		                            -- [ReferenceService] AS rs ON rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID
	                            --WHERE 
		                            --hh.CreatedDate >= @initialDate AND hh.CreatedDate <= @lastDate
		                            --hh.CreatedDate >= '2017/01/01' AND hh.CreatedDate <= '2017/09/30'
		                            --hh.CreatedDate < '2017/08/01'
	                            GROUP BY 
		                            u.FirstName,u.LastName
	                            --ORDER BY
		                            --u.FirstName
                            )
                            ficha
                            ON
                            (
	                            pAccao.FullName = ficha.FullName
                            )
                            LEFT JOIN
                            (--FICHA SEGUIMENTO, GUIA REF, CONTRA REF
	                            SELECT
		                            (u.FirstName) + ' ' + (u.LastName) As FullName
		                            ,COUNT(distinct hh.HouseHoldID) AS HouseholdTotal
		                            --,COUNT(distinct rv.HouseHoldID) AS RoutineVisitTotal
		                            ,COUNT(distinct rs.ReferenceServiceID) AS ReferenceServiceTotal
		                            ,SUM(CASE WHEN rs.HealthAttendedDate<>'' OR rs.SocialAttendedDate<>'' THEN 1 ELSE 0 END) as CounterReferenceServiceTotal
	                            FROM 
		                             HouseHold AS hh
	                            LEFT JOIN 
		                             [User] AS u ON hh.CreatedUserID = u.UserID
	                            LEFT JOIN
		                             RoutineVisit AS rv ON rv.HouseholdID = hh.HouseHoldID
	                            LEFT JOIN
		                             RoutineVisitMember AS rvm ON rvm.RoutineVisitID = rv.RoutineVisitID
	                            LEFT JOIN 
		                             [ReferenceService] AS rs ON rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID
	                            --WHERE 
		                            --hh.CreatedDate >= @initialDate AND hh.CreatedDate <= @lastDate
		                            --hh.CreatedDate >= '2017/01/01' AND hh.CreatedDate <= '2017/09/30'
		                            --hh.CreatedDate < '2017/08/01'
	                            GROUP BY 
		                            u.FirstName,u.LastName
	                            --ORDER BY
		                            --u.FirstName
                            )
                            GuiaContra

                            ON
                            (
	                            pAccao.FullName = GuiaContra.FullName
                            )
                            ----------FIM DO JOIN DO (PLANO DE ACCAO = FICHA SEGUIMENTO, GUIA REF, CONTRA REF)
                            ORDER BY
		                            agregados.FullName";

            return UnitOfWork.DbContext.Database.SqlQuery<UserRecordCountingReportDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }
    }
}
