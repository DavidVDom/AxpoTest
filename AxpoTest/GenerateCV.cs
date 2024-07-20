using Axpo;
using AxpoTest.Abstractions;
using System.Text;

namespace AxpoTest
{
    public class GenerateCV : IGenerateCV
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPowerService _powerService;
        private readonly IConfiguration _configuration;

        public GenerateCV(ILogger<Worker> logger, IPowerService powerService, IConfiguration configuration)
        {
            _logger = logger;
            _powerService = powerService;
            _configuration = configuration;

            CreateCSVFolder(_configuration.GetSection("csvAbsolutePath").Value);
        }

        public void CreateCSVFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public async void GenerateCSVAsync(DateTime date)
        {
            var currentDate = date;
            var csvAbsolutePath = _configuration.GetSection("csvAbsolutePath").Value;

            try
            {
                var trades = await _powerService.GetTradesAsync(currentDate);

                // TODO: de momento volcamos lo que venga a un csv, luego ya agregamos

                var csv = new StringBuilder();
                csv.AppendLine("Local Time, Volume");
                foreach (var trade in trades)
                {
                    csv.AppendLine($"{trade.Date.ToString()}, {trade.Periods.Length}");
                }

                File.WriteAllText(Path.Combine(csvAbsolutePath, $"PowerPosition_{currentDate:yyyyMMdd_HHmm}.csv"), csv.ToString());

                //using (var writer = new StreamWriter(Path.Combine(csvAbsolutePath, $"PowerPosition_{currentDate:yyyyMMdd_HHmm}.csv")));
                //using (var csv=new CsvWriter)

                //_logger.LogInformation($"returned {trades.Count()} items");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR processing trades with date {currentDate:yyyy-MM-dd H:mm:ss}");

                // TODO: Persist in db or json file the requests that failed
                // with fields: currentDate, errorMessage, retry and boolean successfullyExecuted
                // in order to automate retries.
                // Out of the scope of this challenge.
            }
        }
    }
}