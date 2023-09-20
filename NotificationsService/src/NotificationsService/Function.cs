using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NotificationsService.Messaging;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NotificationsService;

public class Function
{
    private ServiceProvider _serviceProvider;

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<Function>();
        serviceCollection.AddScoped<INotifierService, CloudWatchNotifierService>();
        serviceCollection.AddMediatR((config) => { config.RegisterServicesFromAssembly(typeof(Function).Assembly); });

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent @event, ILambdaContext context)
    {
        foreach (var message in @event.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");
        var messageType = message.MessageAttributes["MessageType"].StringValue;
        var type = Type.GetType($"NotificationsService.Messaging.{messageType}");

        if (type is not { })
        {
            context.Logger.LogWarning($"Unknown message type: {messageType}");
            await Task.CompletedTask;
        }

        var typedMessage = (IMessage)JsonSerializer.Deserialize(message.Body, type!)!;
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(typedMessage);

        context.Logger.LogInformation(result);
        
        await Task.CompletedTask;
    }

    ~Function()
    {
        _serviceProvider.Dispose();
    }
}