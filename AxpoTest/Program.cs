using Axpo;
using AxpoTest;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<IPowerService, PowerService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
