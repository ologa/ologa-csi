using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDataAccess.UOW;

namespace EFDataAccess.DTO
{
    public class SeriesDataFamilyOrigin
    {
        public String name { get; set; }
        public int none { get; set; }
        public int community { get; set; }
        public int health_facility { get; set; }
        public int chass { get; set; }
        public int other { get; set; }
    }
}