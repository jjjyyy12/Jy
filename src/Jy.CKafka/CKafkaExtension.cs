using Microsoft.Extensions.DependencyInjection;
using Jy.IMessageQueue;
using Microsoft.Extensions.Configuration;
using Jy.CKafka.Implementation;
using System;

namespace Jy.CKafka
{
    public static class CKafkaExtension
    {
        public static IServiceCollection AddCKafkaServices(this IServiceCollection services)
        {
            var options = new KafkaOptions();
            services.AddSingleton(options);
            AppConfig.KafkaConfig = options.GetConfig();

            return CKafkaServices(services);
        }

        public static IServiceCollection AddCKafkaServices(this IServiceCollection services, KafkaOptions options)
        {
            services.AddSingleton(options);
            AppConfig.KafkaConfig = options.GetConfig();
             
            return CKafkaServices(services);
        }
        public static IServiceCollection AddCKafkaServices(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            var options = new KafkaOptions()
            {
                Servers = Configuration.GetSection("KafkaConfig").GetValue<string>("BorkerList")
            };
            services.AddSingleton(options);
            AppConfig.KafkaConfig = options.GetConfig();

            return CKafkaServices(services);
        }
        private static IServiceCollection CKafkaServices(IServiceCollection services)
        {
            services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            services.AddScoped(factory => {
                Func<string, IKafkaPersisterConnection> accesor = (key) =>
                {
                    if (key.Equals("KafkaProducer"))
                    {
                        return factory.GetService<KafkaProducerPersistentConnection>();
                    }
                    else if (key.Equals("KafkaConsumer"))
                    {
                        return factory.GetService<KafkaConsumerPersistentConnection>();
                    }
                    else
                    {
                        throw new ArgumentException($"Not Support key :{key}");
                    }
                };
                return accesor;
            });
            return services.AddSingleton<IBigQueueOperation, QueueOperationCKafka>();
        }
    }
}
