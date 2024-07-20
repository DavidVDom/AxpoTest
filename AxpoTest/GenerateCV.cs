using Axpo;
using AxpoTest.Abstractions;

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
        }

        public async void GenerateCSVAsync(DateTime date)
        {
            var currentDate = date;
            var csvAbsolutePath = _configuration.GetSection("csvAbsolutePath").Value;

            try
            {
                var trades = await _powerService.GetTradesAsync(currentDate);

                //if (_logger.IsEnabled(LogLevel.Information))
                //{
                //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //}

                _logger.LogInformation($"returned {trades.Count()} items");
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