using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Jy.ILog;
using Jy.IMessageQueue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jy.CKafka.Implementation
{
    public class KafkaConsumerPersistentConnection : KafkaPersistentConnectionBase
    {
        private readonly ILogger _logger;
        private Consumer<string, MessageBase> _consumerClient;
        private readonly IDeserializer<string> _keyDeserializer;
        private readonly IDeserializer<MessageBase> _valueDeserializer;
        bool _disposed;

        public KafkaConsumerPersistentConnection(ILogger logger)
            : base(logger)
        {
            _logger = logger;
            _keyDeserializer = new StringDeserializer(Encoding.UTF8);
            _valueDeserializer = new MessageBaseDeserializer();
        }

        public override bool IsConnected => _consumerClient != null && !_disposed;

        public override Action Connection(IEnumerable<KeyValuePair<string, object>> options)
        {
            return () =>
            {
                _consumerClient = new Consumer<string, MessageBase>(options, _keyDeserializer, _valueDeserializer);
                _consumerClient.OnConsumeError += OnConsumeError;
                _consumerClient.OnError += OnConnectionException;
            };
        }

        public override object CreateConnect()
        {
            return _consumerClient;
        }

        private void OnConsumeError(object sender, Message e)
        {
            var message = e.Deserialize<string, MessageBase>( _keyDeserializer, _valueDeserializer);
            if (_disposed) return;

            _logger.LogWarning($"An error occurred during consume the message; Topic:'{e.Topic}'," +
                $"Message:'{message.Value}', Reason:'{e.Error}'.");

            TryConnect();
        }

        private void OnConnectionException(object sender, Error error)
        {
            if (_disposed) return;

            _logger.LogWarning($"A Kafka connection throw exception.info:{error} ,Trying to re-connect...");

            TryConnect();
        }

        public override void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _consumerClient.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogError("KafkaConsumerPersistentConnection Dispose Error ", ex);
            }
        }
    }
}
