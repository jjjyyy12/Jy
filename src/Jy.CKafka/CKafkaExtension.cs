using Microsoft.Extensions.DependencyInjection;
using Jy.IMessageQueue;
using Microsoft.Extensions.Configuration;
using Jy.CKafka.Implementation;

namespace Jy.CKafka
{
    public static class CKafkaExtension
    {
        public static IServiceCollection AddCKafkaServices(this IServiceCollection services)
        {
            var options = new KafkaOptions();
            services.AddSingleton(options);
            AppConfig.KafkaConfig = options.GetConfig();

            //services.AddScoped<IKafkaPersisterConnection, KafkaProducerPersistentConnection>();
            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            return services.AddSingleton<IBigQueueOperation, QueueOperationCKafka>();
        }

        public static IServiceCollection AddCKafkaServices(this IServiceCollection services, KafkaOptions options)
        {
            services.AddSingleton(options);
            AppConfig.KafkaConfig = options.GetConfig();
             
            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            return services.AddSingleton<IBigQueueOperation, QueueOperationCKafka>();
        }
        public static IServiceCollection AddCKafkaServices(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            var options = new KafkaOptions()
            {
                Servers = Configuration.GetSection("KafkaConfig").GetValue<string>("BorkerList")
            };
            services.AddSingleton(options);
            AppConfig.KafkaConfig = options.GetConfig();

            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            return services.AddSingleton<IBigQueueOperation, QueueOperationCKafka>();
        }
    }
}
