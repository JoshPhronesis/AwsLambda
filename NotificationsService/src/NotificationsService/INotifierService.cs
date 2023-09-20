using Amazon.Lambda.Core;

namespace NotificationsService;

public interface INotifierService
{
    string Notify(string message);
}

public class CloudWatchNotifierService : INotifierService
{

    public CloudWatchNotifierService()
    {
    }
    public string Notify(string message)
    {
        return message;
    }
}