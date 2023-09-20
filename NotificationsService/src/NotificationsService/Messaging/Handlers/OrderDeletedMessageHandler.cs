using MediatR;

namespace NotificationsService.Messaging.Handlers;

public class OrderDeletedMessageHandler : IRequestHandler<OrderDeletedMessage, string>
{
    private readonly INotifierService _notifierService;

    public OrderDeletedMessageHandler(INotifierService notifierService)
    {
        _notifierService = notifierService;
    }

    public Task<string> Handle(OrderDeletedMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_notifierService.Notify($"Order deleted. Order Id : {request.OrderId}"));
    }
}