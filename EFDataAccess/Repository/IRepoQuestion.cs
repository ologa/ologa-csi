using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Repository
{
    public interface IRepoQuestion : IRepository<Question>
    {
        IQueryable<Question> GetQuestionByDomainCode(int DomainID);

        void loadRelatedEntities(Question Question);

        Boolean IsInUse(int QuestionID);
    }
}
