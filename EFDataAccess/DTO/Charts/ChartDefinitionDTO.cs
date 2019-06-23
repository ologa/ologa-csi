using EFDataAccess.DTO.Charts;
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace EFDataAccess.DTO
{
    public class ChartDefinitionDTO
    {
        public string DataURL { get; set; }
        public string Title { get; set; }
        public dynamic ExtraYAxis { get; set; }
        public List<string> LegendLabels { get; set; }
        public List<ChartSerie> ChartSeries = new List<ChartSerie>();
        public List<String> XLabels = new List<string>();
    }
}
