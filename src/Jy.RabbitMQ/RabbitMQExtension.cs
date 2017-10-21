using Microsoft.Extensions.DependencyInjection;
using Jy.IMessageQueue;

namespace Jy.RabbitMQ
{
    public static class RabbitMQExtension  
    {
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services)
        {
            var options = new RabbitMQOptions();
            services.AddSingleton(options);

            services.AddSingleton<ConnectionPool>();
            services.AddSingleton<PublishQueueExecutor>();
            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            return services.AddSingleton<IQueueOperation, QueueOperation>();
        }

        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, RabbitMQOptions options)
        {
            services.AddSingleton(options);

            services.AddSingleton<ConnectionPool>();
            services.AddSingleton<PublishQueueExecutor>(); 
            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            return services.AddSingleton<IQueueOperation, QueueOperation>();
        }
    }
}
