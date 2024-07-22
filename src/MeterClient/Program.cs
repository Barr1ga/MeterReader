using MeterClient;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<MeterGenerator>();

var host = builder.Build();
host.Run();
