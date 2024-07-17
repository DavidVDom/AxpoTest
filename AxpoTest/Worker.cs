using Axpo;

namespace AxpoTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPowerService _powerService;

        public Worker(ILogger<Worker> logger, IPowerService powerService)
        {
            _logger = logger;
            _powerService = powerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //if (_logger.IsEnabled(LogLevel.Information))
                //{
                //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //}
                //await Task.Delay(1000, stoppingToken);

                var algo = await _powerService.GetTradesAsync(DateTime.Now);
            }
        }
    }
}
