using Confluent.Kafka;
using Jy.ILog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jy.CKafka.Implementation
{
    public class KafkaProducerPersistentConnection : KafkaPersistentConnectionBase
    {
        private Producer _connection;
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
            return () =>
            {
                _connection = new Producer(options);
                _connection.OnError += OnConnectionException;

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
