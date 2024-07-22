using Grpc.Core;
using MeterReader.gRPC;
using static MeterReader.gRPC.MeterService;

namespace MeterReader.Services;

public class MeterService : MeterServiceBase
{
    private readonly IReadingRepository _repository;
    private readonly ILogger<MeterService> _logger;

    public MeterService(IReadingRepository repository, ILogger<MeterService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<MeterResponse> AddMeter(MeterPacket request, ServerCallContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (request.Status == MeterStatus.Success)
        {
            foreach (var meter in request.Data)
            {
                var meterResponse = new MeterRequest()
                {
                    CustomerId = meter.CustomerId,
                    Notes = meter.Notes,
                    ResCreatedAt = meter.ResCreatedAt,
                    ResponseValue = meter.ResponseValue,
                };

                _repository.AddEntity(meterResponse);
            }

            if (await _repository.SaveAllAsync())
            {
                return new MeterResponse()
                {
                    Success = MeterStatus.Success,
                    Message = "Meter successfully added"
                };
            }
        }

        return new MeterResponse()
        {
            Success = MeterStatus.Fail,
            Message = "There was an error adding the meter"
        };
    }
}
