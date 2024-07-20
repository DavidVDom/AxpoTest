using AxpoTest.Abstractions;

namespace AxpoTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGenerateCV _generateCV;

        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration,
            IGenerateCV generateCV)
        {
            _logger = logger;
            _configuration = configuration;
            _generateCV = generateCV;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var minutesInterval = int.Parse(_configuration.GetSection("minutesInterval").Value);

            while (!stoppingToken.IsCancellationRequested)
            {
                _generateCV.GenerateCSVAsync(DateTime.Now);

                await Task.Delay(60000 * minutesInterval, stoppingToken);
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
    }
}
