using Grpc.Net.Client;
using MeterReader.gRPC;
using static MeterReader.gRPC.MeterService;

namespace MeterClient;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly MeterGenerator _generator;
    private readonly int _customerId;
    private readonly string _serviceUrl;

    public Worker(ILogger<Worker> logger, MeterGenerator generator, IConfiguration config)
    {
        _logger = logger;
        _generator = generator;
        _customerId = config.GetValue<int>("CustomerId");
        _serviceUrl = config["ServiceUrl"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var channel = GrpcChannel.ForAddress(_serviceUrl);
            var client = new MeterServiceClient(channel);

            var packet = new MeterPacket()
            {
                Status = MeterStatus.Success,
            };

            for (var x = 0; x < 5; x++)
            {
                var data = await _generator.GenerateAsync(_customerId);
                packet.Data.Add(data);
            }

            var res = client.AddMeter(packet);
            if (res.Status == MeterStatus.Success)
            {
                _logger.LogInformation("Successfully called gRPC");
            }
            else
            {
                _logger.LogInformation("Failed to call gRPC");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}
