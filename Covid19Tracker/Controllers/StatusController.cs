using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Covid19Tracker.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Covid19Tracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var confirmedData = GetData("confirmed");
            var deathsData = GetData("deaths");
            var recoveredData = GetData("recovered");

            List<GlobalStatusModel> countries = new List<GlobalStatusModel>();
            
            // Mapping the model


            return Ok(countries.OrderByDescending(x=>x.Confirmed));
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Confirmed()
        {
            var data = GetData("confirmed");
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Deaths()
        {
            var data = GetData("deaths");
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Recovered()
        {
            var data = GetData("recovered");
            return Ok(data);
        }

        public IEnumerable<Covid19RecordModel> GetData(string type)
        {
            var dataUrl = "";
            switch (type)
            {
                case "confirmed":
                    dataUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
                    break;
                case "deaths":
                    dataUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv";
                    break;
                case "recovered":
                    dataUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_recovered_global.csv";
                    break;
                default:
                    dataUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
                    break;
            }

            // Read CSV data from url
            WebClient client = new WebClient();
            var coronaData = client.DownloadString(new Uri(dataUrl));

            var stringReader = new StringReader(coronaData);
            var lastRecords = new List<Covid19RecordModel>();

            using (var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture))
            {
                //var records = csv.GetRecords<Covid19RecordModel>().ToList();

                using (var dataReader = new CsvDataReader(csv))
                {
                    var coronaDT = new DataTable();
                    coronaDT.Load(dataReader);


                    foreach (DataRow record in coronaDT.Rows)
                    {
                        var province_State = record["Province/State"].ToString();
                        var country_Region = record["Country/Region"].ToString();
                        var lat = record["Lat"].ToString();
                        var longitude = record["Long"].ToString();
                        var lastTotalCases = record[record.ItemArray.Length - 1].ToString();

                        var cornaRecord = new Covid19RecordModel
                        {
                            Country_Region = country_Region,
                            Province_State = province_State,
                            Lat = lat,
                            Long = longitude,
                            LastTotalCases = int.Parse(lastTotalCases)
                        };
                        lastRecords.Add(cornaRecord);
                    }
                }
            }

            return lastRecords.OrderByDescending(x => x.LastTotalCases);
        }
    }
}