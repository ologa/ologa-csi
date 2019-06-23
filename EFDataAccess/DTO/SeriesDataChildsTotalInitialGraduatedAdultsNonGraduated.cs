using System;

namespace EFDataAccess.DTO
{
    public class SeriesDataChildsTotalInitialGraduatedAdultsNonGraduated
    {
        public String SiteName { get; set; }
        public int Total { get; set; }
        public int Initial { get; set; }
        public int Graduated { get; set; }
        public int Adult { get; set; }
        //public int NonGraduated { get; set; }
    }
}
