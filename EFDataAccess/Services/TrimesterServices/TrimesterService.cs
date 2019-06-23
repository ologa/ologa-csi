using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using EFDataAccess.UOW;
using VPPS.CSI.Domain;
using Domain;

namespace EFDataAccess.Services.TrimesterServices
{
    public class TrimesterService : BaseService
    {
        private readonly Repository.IRepository<TrimesterDefinition> TrimesterDefinitionRepository;
        private Repository.IRepository<Trimester> TrimesterRepository;
        private readonly ITrimesterQueryService QueryService;

        public TrimesterService(UnitOfWork unitOfWork,
            ITrimesterQueryService trimesterQueryService) : base(unitOfWork)
        {
            TrimesterDefinitionRepository = unitOfWork.Repository<TrimesterDefinition>();
            TrimesterRepository = unitOfWork.Repository<Trimester>();
            QueryService = trimesterQueryService;
        }

        public TrimesterService(ITrimesterQueryService trimesterQueryService)
        {
            QueryService = trimesterQueryService;
        }

        public List<Trimester> GenerateTrimestersForTheLastPastYears(int numberOfYears)
        {
            List<Trimester> trimesters = new List<Trimester>();
            List<TrimesterDefinition> trimesterDefinitions = QueryService.GetAllTrimesterDefinitions();
            int currentYear = DateTime.Now.Year;
            int startingYear = DateTime.Now.Year - numberOfYears;
            int seq = 1;

            while (currentYear >= startingYear)
            {
                trimesterDefinitions.ForEach(td =>
                {
                    if (startingYear < 2018 && td.FiscalYearVersion == 1)
                    {
                        var startDateYear = td.TrimesterSequence > 2 ? startingYear + 1 : startingYear;
                        var endDateYear = td.TrimesterSequence == 1 ? startingYear : startingYear + 1;

                        int startDateDay = td.FirstDay;
                        int lastDateDay = td.LastDay;

                        trimesters.Add(new Trimester()
                        {
                            Seq = seq,
                            StartDate = new DateTime(startDateYear, td.FirstMonth, startDateDay),
                            EndDate = new DateTime(endDateYear, td.LastMonth, lastDateDay),
                            TrimesterDescription = startDateDay + "/" + td.FirstMonth + "/" + startDateYear+" à "+ lastDateDay + "/" + td.LastMonth + "/" + endDateYear,
                            TrimesterDefinitionID = td.TrimesterDefinitionID
                        });

                        seq++;
                    }
                    else if (startingYear == 2018 && td.FiscalYearVersion == 2)
                    {
                        var startDateYear = td.TrimesterSequence > 2 ? startingYear + 1 : startingYear;
                        var endDateYear = td.TrimesterSequence == 1 ? startingYear : startingYear + 1;

                        int startDateDay = td.FirstDay;
                        int lastDateDay = td.LastDay;
                        trimesters.Add(new Trimester()
                        {
                            Seq = seq,
                            StartDate = new DateTime(startDateYear, td.FirstMonth, startDateDay),
                            EndDate = new DateTime(endDateYear, td.LastMonth, lastDateDay),
                            TrimesterDescription = startDateDay + "/" + td.FirstMonth + "/" + startDateYear + " à " + lastDateDay + "/" + td.LastMonth + "/" + endDateYear,
                            TrimesterDefinitionID = td.TrimesterDefinitionID
                        });

                        seq++;
                    }
                    else if (startingYear > 2018 && td.FiscalYearVersion == 3)
                    {
                        var startDateYear = td.TrimesterSequence > 2 ? startingYear + 1 : startingYear;
                        var endDateYear = td.TrimesterSequence == 1 ? startingYear : startingYear + 1;

                        int startDateDay = td.FirstDay;
                        int lastDateDay = td.LastDay;
                        trimesters.Add(new Trimester()
                        {
                            Seq = seq,
                            StartDate = new DateTime(startDateYear, td.FirstMonth, startDateDay),
                            EndDate = new DateTime(endDateYear, td.LastMonth, lastDateDay),
                            TrimesterDescription = startDateDay + "/" + td.FirstMonth + "/" + startDateYear + " à " + lastDateDay + "/" + td.LastMonth + "/" + endDateYear,
                            TrimesterDefinitionID = td.TrimesterDefinitionID
                        });

                        seq++;
                    }
                });
                startingYear++;
            }

            return trimesters;
        }

        public void SaveTrimesters(List<Trimester> trimesters)
        {
            trimesters.ForEach(trimester =>
            {
                TrimesterRepository.Add(trimester);
            });

            UnitOfWork.Commit();
        }
    }
}
