using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;
using System.Linq.Expressions;
using System.Data.Entity;
using EFDataAccess.CustomQuery;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace EFDataAccess.Services
{
    public class SiteGoalService : BaseService
    {
        public SiteGoalService(UnitOfWork uow) : base(uow) { }

        private EFDataAccess.Repository.IRepository<SiteGoal> SiteGoalRepository
        {
            get { return UnitOfWork.Repository<SiteGoal>(); }
        }

        public SiteGoal Reload(SiteGoal entity)
        {
            SiteGoalRepository.FullReload(entity);
            return entity;
        }

        public List<GraduationCriteria> findAllGraduationCriteria()
        {
            return UnitOfWork.Repository<GraduationCriteria>().GetAll().ToList();
        }

        public List<OrgUnit> findAllOrgUnitLocations()
        {
            return UnitOfWork.Repository<OrgUnit>().GetAll().Where(e => e.OrgUnitType.OrgUnitTypeID == 3).ToList();
        }

        public List<SiteGoal> findAll()
        {
            return UnitOfWork.Repository<SiteGoal>().GetAll().ToList();
        }

        public SiteGoal findById(int id)
        {
            return UnitOfWork.Repository<SiteGoal>().GetAll().Where(e => e.SiteGoalID == id).FirstOrDefault();
        }

        public void Delete(SiteGoal SiteGoal)
        {
            UnitOfWork.Repository<SiteGoal>().Delete(SiteGoal);
            UnitOfWork.Commit();
        }

        public void Save(SiteGoal SiteGoal)
        {
            if (SiteGoal.SiteGoalID > 0)
            {
                UnitOfWork.Repository<SiteGoal>().Update(SiteGoal);
                UnitOfWork.Commit();
            }
            else
            {
                UnitOfWork.Repository<SiteGoal>().Add(SiteGoal);
                UnitOfWork.Commit();
            }
        }
    }
}
