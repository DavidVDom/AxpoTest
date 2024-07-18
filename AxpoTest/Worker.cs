using Axpo;
using System.ComponentModel.DataAnnotations;

namespace AxpoTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPowerService _powerService;
        private readonly IConfiguration _configuration;
        private readonly string? _csvPath;
        private readonly int _minutesInterval;

        public Worker(ILogger<Worker> logger, IPowerService powerService, IConfiguration configuration)
        {
            _logger = logger;
            _powerService = powerService;
            _configuration = configuration;

            _csvPath = _configuration.GetSection("csvPath").Value;
            _minutesInterval = int.Parse(_configuration.GetSection("minutesInterval").Value);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                GenerateCSVAsync();

                await Task.Delay(1000 * 60 * _minutesInterval, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker START: {time}", DateTimeOffset.Now);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker STOP: {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Worker DISPOSED at: {time}", DateTimeOffset.Now);
        }

        private async void GenerateCSVAsync()
        {
            var algo = await _powerService.GetTradesAsync(DateTime.Now);
        }
    }
}
