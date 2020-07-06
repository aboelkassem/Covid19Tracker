using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Tracker.Models
{
    public class Covid19RecordModel
    {
        public string Province_State { get; set; }
        public string Country_Region { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int LastTotalCases { get; set; }
    }
}
