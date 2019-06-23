using System;
using System.Collections.Generic;
using Utilities;

namespace EFDataAccess.DTO
{
    public class AgreggatedBeforeAndActualChildStatusReportDTO : AgreggatedBaseDataDTO
    {
        public string BeforeActualChildStatus { get; set; }
        public string ActualChildStatus { get; set; }
        public int MaleTotal { get; set; }
        public int FemaleTotal { get; set; }
        public int Total { get; set; }

        public List<Object> populatedAgreggatedValues()
        {
            values.Add(Partner);
            values.Add(BeforeActualChildStatus);
            values.Add(ActualChildStatus);
            values.Add(MaleTotal);
            values.Add(FemaleTotal);
            values.Add(Total);


            return values;
        }

    }
}
