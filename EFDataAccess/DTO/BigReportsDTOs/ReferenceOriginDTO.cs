using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class ReferenceOriginDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int Comunidade { get; set; }
        public int UnidadeSanitaria { get; set; }
        public int ParceiroClinico { get; set; }
        public int Nenhuma { get; set; }
        public int Outro { get; set; }

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(Comunidade);
            values.Add(UnidadeSanitaria);
            values.Add(ParceiroClinico);
            values.Add(Nenhuma);
            values.Add(Outro);

            return values;
        }
    }
}