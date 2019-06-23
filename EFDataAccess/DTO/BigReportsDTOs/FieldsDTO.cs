using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class FieldsDTO : ISiteNameReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }
        public string QueryCode { get; set; }
        public int Field1 { get; set; }
        public int Field2 { get; set; }
        public int Field3 { get; set; }
        public int Field4 { get; set; }
        public int Field5 { get; set; }
        public int Field6 { get; set; }
        public int Field7 { get; set; }
        public int Field8 { get; set; }
        public int Field9 { get; set; }
        public int Field10 { get; set; }
        public int Field11 { get; set; }
        public int Field12 { get; set; }
        public int Field13 { get; set; }
        public int Field14 { get; set; }
        public int Field15 { get; set; }
        public int Field16 { get; set; }
        public int Field17 { get; set; }
        public int Field18 { get; set; }
        public int Field19 { get; set; }
        public int Field20 { get; set; }
        public int Field21 { get; set; }
        public int Field22 { get; set; }
        public int Field23 { get; set; }
        public int Field24 { get; set; }
        public int Field25 { get; set; }
        public int Field26 { get; set; }
        public int Field27 { get; set; }
        public int Field28 { get; set; }
        public int Field29 { get; set; }
        public int Field30 { get; set; }
        public int Field31 { get; set; }
        public int Field32 { get; set; }
        public int Field33 { get; set; }
        public int Field34 { get; set; }
        public int Field35 { get; set; }


        private List<Object> values { get; set; } = new List<Object>();
        
        public List<Object> populatedValues()
        {
            values.Add(Field1);
            values.Add(Field2);
            values.Add(Field3);
            values.Add(Field4);
            values.Add(Field5);
            values.Add(Field6);
            values.Add(Field7);
            values.Add(Field8);
            values.Add(Field9);
            values.Add(Field10);
            values.Add(Field11);
            values.Add(Field12);
            values.Add(Field13);
            values.Add(Field14);
            values.Add(Field15);
            values.Add(Field16);
            values.Add(Field17);
            values.Add(Field18);
            values.Add(Field19);
            values.Add(Field20);
            values.Add(Field21);
            values.Add(Field22);
            values.Add(Field23);
            values.Add(Field24);
            values.Add(Field25);
            values.Add(Field26);
            values.Add(Field27);
            values.Add(Field28);
            values.Add(Field29);
            values.Add(Field30);
            values.Add(Field31);
            values.Add(Field32);
            values.Add(Field33);
            values.Add(Field34);
            values.Add(Field35);

            return values;
        }
    }
}