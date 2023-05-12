using EisRoutingService.Contracts;
using EisRoutingService.Options;
using EisRoutingService.Publishers;
using Microsoft.AspNetCore.Routing;

namespace EisRoutingService.Messages.Handlers;

public class EisEventeHandler : IMessageHandler<EisEvent>
{
    private readonly IMessagePublisher _publisher;
    private readonly EisRoutingOption _option;
    private readonly ILogger<EisEventeHandler> _logger;

    public EisEventeHandler(IMessagePublisher publisher, ILogger<EisEventeHandler> logger)
    {
        _logger = logger;
        _publisher = publisher;
        //        _option = option;
    }

    public async Task HandleAsync(EisEvent message)
    {
        _logger.LogInformation($"Routing Handler Event Id : {message.EventId} | Source System: {message.SourceSystemName} | Created : {message.CreatedDate.ToString("yyyy-MM-dd:hh:mm:ss")}");

        await _publisher.PublishAsync("queue.inbound.scmstoremanagement", "queue.inbound.scmstoremanagement", message);
        //foreach (var rout in _option.Routs)
        //{
        //    if (rout.Name.ToLower() != message.SourceSystemName.ToLower())
        //    {
        //        await _publisher.PublishAsync(rout.InboundQueue, rout.InboundQueue, message);
        //    }
        //}
    }
}