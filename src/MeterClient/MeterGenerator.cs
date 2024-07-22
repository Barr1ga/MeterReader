using Google.Protobuf.WellKnownTypes;
using MeterReader.gRPC;

namespace MeterClient;
public class MeterGenerator
{
    public Task<MeterRequest> GenerateAsync(int customerId)
    {
        var reading = new MeterRequest()
        {
            CustomerId = customerId,
            Value = new Random().Next(10000),
            Notes = string.Empty,
            ReadingDate = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        return Task.FromResult(reading);
    }
}
