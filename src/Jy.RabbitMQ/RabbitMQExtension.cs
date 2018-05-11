using Microsoft.Extensions.DependencyInjection;
using Jy.IMessageQueue;
using Microsoft.Extensions.Configuration;

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
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            services.AddSingleton(new RabbitMQOptions()
            {
                HostName = Configuration.GetSection("RabbitMQConfig").GetValue<string>("HostName"),
                UserName = Configuration.GetSection("RabbitMQConfig").GetValue<string>("UserName"),
                Password = Configuration.GetSection("RabbitMQConfig").GetValue<string>("Password"),
                Port = Configuration.GetSection("RabbitMQConfig").GetValue<int>("Port")
            });

            services.AddSingleton<ConnectionPool>();
            services.AddSingleton<PublishQueueExecutor>();
            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            return services.AddSingleton<IQueueOperation, QueueOperation>();
        }
    }
}
