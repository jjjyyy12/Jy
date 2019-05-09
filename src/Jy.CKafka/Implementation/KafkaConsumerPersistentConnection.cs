using Confluent.Kafka;
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
        private IConsumer<string, MessageBase> _consumerClient;
        private readonly IDeserializer<string> _keyDeserializer;
        private readonly IDeserializer<MessageBase> _valueDeserializer;
        bool _disposed;

        public KafkaConsumerPersistentConnection(ILogger logger)
            : base(logger)
        {
            _logger = logger;
            _keyDeserializer = new Utf8Deserializer();
            _valueDeserializer = new MessageBaseDeserializer();
        }

        public override bool IsConnected => _consumerClient != null && !_disposed;

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
            var cConfig = new ConsumerConfig
            {
                BootstrapServers = borkerList,
                BrokerVersionFallback = "0.10.0.0",
                ApiVersionFallbackMs = 0,
                //SaslMechanism = SaslMechanism.Plain,
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //SslCaLocation = "/usr/local/etc/openssl/cert.pem", // suitable configuration for linux, osx.
                // SslCaLocation = "c:\\path\\to\\cacert.pem",     // windows
                //SaslUsername = "<confluent cloud key>",
                //SaslPassword = "<confluent cloud secret>",
                GroupId = Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            return () =>
            {
                var consumerBuilder = new ConsumerBuilder<string, MessageBase>(cConfig);
                consumerBuilder.SetErrorHandler(OnConnectionException);
                _consumerClient = consumerBuilder.Build();
            };
        }

        public override object CreateConnect()
        {
            return _consumerClient;
        }

        private void OnConsumeError(object sender, Message<string,MessageBase> e)
        {
            var message = e.Value;
            if (_disposed) return;

            _logger.LogWarning($"An error occurred during consume the message; Key:'{e.Key}'," +
                $"Value:'{e.Value}.");

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
        private class Utf8Deserializer : IDeserializer<string>
        {
            public string Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull)
                {
                    return null;
                }

#if NETCOREAPP2_1
                    return Encoding.UTF8.GetString(data);
#else
                return Encoding.UTF8.GetString(data.ToArray());
#endif
            }
        }
    }
}
