using Axpo;
using AxpoTest;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/AxpoTest-.log"),
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options => options.ServiceName = "AxpoTestService")
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IPowerService, PowerService>();
    })
    .Build();


host.Run();
