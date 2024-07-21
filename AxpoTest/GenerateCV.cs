﻿using Axpo;
using AxpoTest.Abstractions;
using AxpoTest.Model;
using System.Globalization;
using System.Text;

namespace AxpoTest
{
    public class GenerateCV : IGenerateCV
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPowerService _powerService;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<int, string> _utilsMap;

        public GenerateCV(ILogger<Worker> logger, IPowerService powerService, IConfiguration configuration)
        {
            _logger = logger;
            _powerService = powerService;
            _configuration = configuration;

            CreateCSVFolder(_configuration.GetSection("csvAbsolutePath").Value);
            _utilsMap = GetInitializedMap();
        }

        public async Task GenerateCSVAsync(DateTime date)
        {
            var currentDate = date;
            var csvAbsolutePath = _configuration.GetSection("csvAbsolutePath").Value;

            try
            {
                var trades = await _powerService.GetTradesAsync(currentDate);
                var csvList = new List<CsvResult>();

                foreach (var (key, value) in _utilsMap)
                {
                    double volumeAggregate = 0;
                    foreach (var trade in trades)
                    {
                        volumeAggregate += trade.Periods.Where(p => p.Period == key).Sum(p => p.Volume);
                    }
                    csvList.Add(new CsvResult
                    {
                        LocalTime = value,
                        Volume = volumeAggregate
                    });
                }

                var csv = new StringBuilder();
                csv.AppendLine("Local Time, Volume");
                foreach (var item in csvList)
                {
                    csv.AppendLine($"{item.LocalTime}, {item.Volume.ToString(CultureInfo.InvariantCulture)}");
                }

                File.WriteAllText(Path.Combine(csvAbsolutePath, $"PowerPosition_{currentDate:yyyyMMdd_HHmm}.csv"), csv.ToString());

                _logger.LogInformation($"PROCESSED trades with date {currentDate:yyyy-MM-dd H:mm:ss}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR processing trades with date {currentDate:yyyy-MM-dd H:mm:ss}");

                // TODO: Persist in db or json file the requests that failed
                // with fields: currentDate, errorMessage, retry and boolean successfullyExecutedInRetry
                // in order to automate retries.
                // Out of the scope of this challenge.
            }
        }

        private void CreateCSVFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private Dictionary<int, string> GetInitializedMap()
        {
            var map = new Dictionary<int, string>();

            for (var i = 1; i <= 24; i++)
            {
                if (i == 1)
                {
                    map.Add(i, "23:00");
                    continue;
                }
                if (i > 1 && i < 12)
                {
                    map.Add(i, $"0{i - 2}:00");
                    continue;
                }

                map.Add(i, $"{i - 2}:00");
            }

            return map;
        }
    }
}