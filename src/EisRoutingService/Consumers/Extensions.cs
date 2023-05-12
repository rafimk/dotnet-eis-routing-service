using ActiveMQ.Artemis.Client;
using ActiveMQ.Artemis.Client.AutoRecovering.RecoveryPolicy;
using ActiveMQ.Artemis.Client.Extensions.DependencyInjection;
using ActiveMQ.Artemis.Client.Extensions.Hosting;
using ActiveMQ.Artemis.Client.MessageIdPolicy;
using EisRoutingService.Client;
using EisRoutingService.Contracts;
using EisRoutingService.Messages;
using EisRoutingService.Options;
using EisRoutingService.Utilities;
using System.Text.Json;

namespace EisRoutingService.Consumers
{
    public static class Extensions
    {
        public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            var dispatcher = services.BuildServiceProvider().GetRequiredService<IMessageDispatcher>();

            var eisOptions = configuration.GetOptions<EisOption>("EisOptions");
            var routs = configuration.GetOptions<List<Rout>>("EisRoutingOptions");

            var eisRouts = new EisRoutingOption
            {
                Routs = routs
            };

            services.AddSingleton(eisRouts);

            services.AddActiveMq(name: eisOptions.ClusterName, endpoints: new[] { ActiveMQ.Artemis.Client.Endpoint.Create(host: eisOptions.Host, port: eisOptions.Port, eisOptions.UserName, eisOptions.Password) })
            .ConfigureConnectionFactory((provider, factory) =>
            {
                factory.LoggerFactory = provider.GetService<ILoggerFactory>();
                factory.RecoveryPolicy = RecoveryPolicyFactory.ExponentialBackoff(initialDelay: TimeSpan.FromSeconds(1), maxDelay: TimeSpan.FromSeconds(30), retryCount: 5);
                factory.MessageIdPolicyFactory = MessageIdPolicyFactory.GuidMessageIdPolicy;
                factory.AutomaticRecoveryEnabled = true;
            })
            .ConfigureConnection((_, connection) =>
            {
                connection.ConnectionClosed += (_, args) =>
                {
                    Console.WriteLine($"Connection closed: ClosedByPeer={args.ClosedByPeer}, Error={args.Error}");
                };
                connection.ConnectionRecovered += (_, args) =>
                {
                    Console.WriteLine($"Connection recovered: Endpoint={args.Endpoint}");
                };
                connection.ConnectionRecoveryError += (_, args) =>
                {
                    Console.WriteLine($"Connection recovered error: Exception={args.Exception}");
                };
            })
            .AddConsumer("topic.outbound.scmitemmanagement", RoutingType.Multicast, "topic.outbound.scmitemmanagement",
                        async (message, consumer, token, serviceProvider) =>
                        {
                            var body = message.GetBody<string>();
                            var msg = JsonSerializerUtil.DeserializeObject<EisEvent>(body);

                            if (msg is not null)
                            {
                                await dispatcher.DispatchAsync(msg!);
                            }
 
                            await consumer.AcceptAsync(message);
                        })
            .AddProducer<MyTypedMessageProducer>("topic.outbound.scmitemmanagement", RoutingType.Multicast)
            .EnableQueueDeclaration()
            .EnableAddressDeclaration();

            services.AddActiveMqHostedService();

            return services;
        }
        public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
        {
            var options = new T();
            var section = configuration.GetRequiredSection(sectionName);
            section.Bind(options);

            return options;
        }
    }
}
