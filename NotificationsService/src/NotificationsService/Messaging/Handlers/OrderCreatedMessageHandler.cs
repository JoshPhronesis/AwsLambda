using MediatR;

namespace NotificationsService.Messaging.Handlers;

public class OrderCreatedMessageHandler : IRequestHandler<OrderCreatedMessage, string>
{
    private readonly INotifierService _notifierService;

    public OrderCreatedMessageHandler(INotifierService notifierService)
    {
        _notifierService = notifierService;
    }

    public Task<string> Handle(OrderCreatedMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_notifierService.Notify($"Order created. Order Id : {request.Id}"));
    }
}