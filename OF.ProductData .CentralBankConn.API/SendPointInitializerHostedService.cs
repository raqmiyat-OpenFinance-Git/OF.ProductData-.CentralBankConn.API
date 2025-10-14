using OF.ProductData.CentralBankConn.API.Models;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.Common;

namespace OF.ProductData.CentralBankConn.API;

public class SendPointInitializerHostedService : IHostedService
{
    private readonly IBus _bus;
    private readonly RabbitMqSettings _settings;
    private readonly SendPointInitialize _sendPoint;
    private readonly QueueLogger _queueLogger;

    public SendPointInitializerHostedService(
        IBus bus,
        IOptions<RabbitMqSettings> options,
        SendPointInitialize sendPoint,
        QueueLogger queueLogger)
    {
        _bus = bus;
        _settings = options.Value;
        _sendPoint = sendPoint;
        _queueLogger = queueLogger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
     

        _sendPoint.GetProductDataRequest = await _bus.GetSendEndpoint(
        new Uri("queue:" + _settings.GetProductDataRequest));

        _queueLogger.Info("Queue GetProductDataRequest has been created");


        _sendPoint.GetProductDataResponse = await _bus.GetSendEndpoint(
        new Uri("queue:" + _settings.GetProductDataResponse));

        _queueLogger.Info("Queue GetProductDataResponse has been created");

        _sendPoint.AuditLog = await _bus.GetSendEndpoint(
            new Uri("queue:" + _settings.AuditLog));

        _queueLogger.Info("Queue AuditLog has been created");

      

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
