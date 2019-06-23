using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Repository
{
    public class QuestionRepo : BaseRepository<Question>, IRepoQuestion 
    {
        public QuestionRepo(DbContext context):base(context)
        {

        }

        public IQueryable<Question> GetQuestionByDomainCode(int DomainID)
        {
           // return GetAll().Where(p => p.Domain.DomainID == DomainID);
            var query = DbSet.Include("Domain").Include("File");
            return query.Where(p => p.Domain.DomainID == DomainID);
        }

        public void loadRelatedEntities(Question Question)
        {
            //this.DbContext.Entry(Question).Reference(p => p.Domain).Load();
            this.DbContext.Entry(Question).Collection(p => p.Answers).Query().Include("Score").Include("File").Load();
        }

        public Boolean IsInUse(int QuestionID)
        {
            return GetAll().Any(e => e.QuestionID == QuestionID && e.Answers.Any());
        }
  
      
    }
}
