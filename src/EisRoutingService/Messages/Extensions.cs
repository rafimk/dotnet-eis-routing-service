using ActiveMQ.Artemis.Client;
using EisRoutingService.Accessors;
using EisRoutingService.Connections;
using EisRoutingService.Consumers;
using EisRoutingService.Options;
using EisRoutingService.Publishers;
using EisRoutingService.Subscribers;

namespace EisRoutingService.Messages;

public static class Extensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var eisOptions = configuration.GetOptions<EisOption>("EisOptions");
        var endpoint = ActiveMQ.Artemis.Client.Endpoint.Create(host: eisOptions.Host, port: eisOptions.Port, eisOptions.UserName, eisOptions.Password);
        var connectionFactory = new ConnectionFactory();
        var connection = connectionFactory.CreateAsync(endpoint).Result;

        services.AddSingleton(eisOptions);
        services.AddSingleton(connection);
        services.AddSingleton<ChannelAccessor>();
        services.AddSingleton<IChannelFactory, ChannelFactory>();
        services.AddSingleton<IMessagePublisher, MessagePublisher>();
        services.AddSingleton<IMessageSubscriber, MessageSubscriber>();
        services.AddSingleton<IMessageDispatcher, MessageDispatcher>();
        services.AddSingleton<IMessageIdAccessor, MessageIdAccessor>();

        services.Scan(cfg => cfg.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(c => c.AssignableTo(typeof(IMessageHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}