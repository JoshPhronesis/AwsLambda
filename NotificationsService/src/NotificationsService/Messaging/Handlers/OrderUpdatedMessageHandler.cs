using MediatR;

namespace NotificationsService.Messaging.Handlers;

public class OrderUpdatedMessageHandler : IRequestHandler<OrderUpdatedMessage, string>
{
    private readonly INotifierService _notifierService;

    public OrderUpdatedMessageHandler(INotifierService notifierService)
    {
        _notifierService = notifierService;
    }

    public Task<string> Handle(OrderUpdatedMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_notifierService.Notify($"Order updated. Order Id : {request.Id}"));
    }
}