using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services.TrimesterServices
{
    public interface ITrimesterQueryService
    {
        List<TrimesterDefinition> GetAllTrimesterDefinitions();
        List<Trimester> GetAllTrimesters();
        Trimester GetTrimesterByDate(DateTime date);
        List<Trimester> GetPreviousTrimesters(int numberOfTrimester, bool includeCurrentTrimester, Trimester currentTrimester);
        DateTime getTrimesterStartOrEndDateByID(int trimesterID, string startOrEndDate);
    }
}
