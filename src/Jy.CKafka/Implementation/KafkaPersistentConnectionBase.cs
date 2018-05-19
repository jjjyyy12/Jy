using Confluent.Kafka;
using Jy.ILog;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CKafka.Implementation
{
    public abstract class KafkaPersistentConnectionBase : IKafkaPersisterConnection
    {
        private readonly ILogger _logger;

        object sync_root = new object();

        public KafkaPersistentConnectionBase(ILogger logger)
        {
            this._logger = logger;
        }


        public abstract bool IsConnected { get; }

        public bool TryConnect()
        {
            _logger.LogInformation("Kafka Client is trying to connect");

            lock (sync_root)
            {
                var policy = RetryPolicy.Handle<KafkaException>()
                    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex.ToString());
                    }
                );

                policy.Execute(() =>
                {
                    Connection(AppConfig.KafkaConfig);
                });

                if (IsConnected)
                {
                    return true;
                }
                else
                {
                    _logger.LogInformation("FATAL ERROR: Kafka connections could not be created and opened");
                    return false;
                }
            }
        }

        public abstract Action Connection(IEnumerable<KeyValuePair<string, object>> options);


        public abstract object CreateConnect();
        public abstract void Dispose();
    }
}
