using System;
using System.Collections.Generic;
using Utilities;

namespace EFDataAccess.DTO
{
    public class RoutineVisitChildDTO : AgreggatedBaseDataDTO
    {
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
        public int TotalCount { get; set; }
    }
}