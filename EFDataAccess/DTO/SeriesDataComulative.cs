using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EFDataAccess.UOW;

namespace EFDataAccess.DTO
{
    public class SeriesDataComulative:SeriesData
    {
        public int comulative { get; set; }
    }
}