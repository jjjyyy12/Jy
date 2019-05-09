using Confluent.Kafka;
using Jy.ILog;
using Jy.IMessageQueue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jy.CKafka.Implementation
{
    public class KafkaProducerPersistentConnection : KafkaPersistentConnectionBase
    {
        private IProducer<string, MessageBase> _connection;
        private readonly ILogger _logger;
        bool _disposed;

        public KafkaProducerPersistentConnection(ILogger logger)
            : base(logger)
        {
            _logger = logger;
        }

        public override bool IsConnected => _connection != null && !_disposed;


        public override Action Connection(IEnumerable<KeyValuePair<string, object>> options)
        {
            string borkerList = "";
            var list = options.GetEnumerator();
            while (list.MoveNext())
            {
                if ("BorkerList".Equals(list.Current.Key))
                {
                    borkerList = list.Current.Value.ToString();
                }
            }
            var config = new ProducerConfig { BootstrapServers = borkerList };
            return () =>
            {
                var producerBuilder = new ProducerBuilder<string, MessageBase>(config);
                _connection = producerBuilder.Build();
            };
        }

        public override object CreateConnect()
        {
            return _connection;
        }

        public override void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogError("KafkaProducerPersistentConnection Dispose Error ", ex);
            }
        }

        private void OnConnectionException(object sender, Error error)
        {
            if (_disposed) return;

            _logger.LogWarning($"A Kafka connection throw exception.info:{error} ,Trying to re-connect...");

            TryConnect();
        }


    }
}
