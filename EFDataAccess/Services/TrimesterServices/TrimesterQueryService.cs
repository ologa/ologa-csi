using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services.TrimesterServices
{
    public class TrimesterQueryService: BaseService, ITrimesterQueryService
    {
        private readonly Repository.IRepository<TrimesterDefinition> TrimesterDefinitionRepository;
        private readonly Repository.IRepository<Trimester> TrimesterRepository;

        public TrimesterQueryService(UnitOfWork unitOfWork)
        {
            TrimesterDefinitionRepository = unitOfWork.Repository<TrimesterDefinition>();
            TrimesterRepository = unitOfWork.Repository<Trimester>();
        }

        public List<TrimesterDefinition> GetAllTrimesterDefinitions()
        {
            return TrimesterDefinitionRepository.GetAll().ToList();
        }

        public List<Trimester> GetAllTrimesters()
        {
            return TrimesterRepository.GetAll().ToList();
        }
        
        
        public List<Trimester> GetPreviousTrimesters(int numberOfTrimesters, bool includeCurrentTrimester, Trimester currentTrimester)
        {
            List<Trimester> trimesters = new List<Trimester>();
            if(includeCurrentTrimester)
            {
                trimesters.Add(currentTrimester);
            }
            List<Trimester> previousTrimesters = TrimesterRepository
                                                    .GetAll()
                                                    .Where(t => t.Seq < currentTrimester.Seq)
                                                    .OrderByDescending(t => t.Seq)
                                                    .Take(numberOfTrimesters)
                                                    .ToList();
            trimesters.AddRange(previousTrimesters);

            return trimesters;
        }

        public Trimester GetTrimesterByDate(DateTime date)
        {
            List<Trimester> trimesters = new List<Trimester>();
            trimesters = TrimesterRepository.GetAll().ToList();

            Trimester trimester = null;
            foreach (Trimester t in trimesters)
            {
                if (t.StartDate <= date && t.EndDate >= date)
                {
                    trimester = t;
                }
            }
            return trimester;
        }

        public Trimester GetTrimesterBySequence(int Sequence)
        {
            return TrimesterRepository
                .Get()
                .Where(t => t.Seq == Sequence)
                .SingleOrDefault();
        }


        public DateTime getTrimesterStartOrEndDateByID(int sequenceID, string startOrEndDate)
        {
            Trimester trimester = TrimesterRepository
                                    .GetAll()
                                    .Where(t => t.Seq == sequenceID)
                                    .FirstOrDefault();
            
            return (startOrEndDate == "start") ? trimester.StartDate: trimester.EndDate;
        }
         

    }
}
