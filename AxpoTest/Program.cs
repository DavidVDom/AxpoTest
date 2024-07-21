using Axpo;
using AxpoTest;
using AxpoTest.Abstractions;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options => options.ServiceName = "AxpoTestService")
    .UseSerilog((hostingContext, services, loggerConfiguration) =>
    {
        loggerConfiguration
        .WriteTo.File(
            Path.Combine(hostingContext.Configuration.GetSection("logAbsolutePath").Value, "AxpoTest-.log"),
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IPowerService, PowerService>();
        services.AddSingleton<IGenerateCV, GenerateCV>();
    })
    .Build();

host.Run();
