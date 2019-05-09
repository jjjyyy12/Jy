using Confluent.Kafka;
using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CKafka.Implementation
{
    public class MessageBaseDeserializer : IDeserializer<MessageBase>
    {
        /// <summary>
        ///     Deserializes a System.Byte[] value (or null) from a byte array.
        /// </summary>
        /// <param name="topic">
        ///     The topic associated with the data (ignored by this deserializer).
        /// </param>
        /// <param name="data">
        ///     A byte array containing the serialized System.Byte[] value (or null).
        /// </param>
        /// <returns>
        ///     The deserialized System.Byte[] value.
        /// </returns>
        public MessageBase Deserialize(string topic, byte[] data)
        {
            return ByteConvertHelper.Bytes2Object<MessageBase>(data);
        }

        /// <include file='../include_docs.xml' path='API/Member[@name="IDeserializer_Configure"]/*' />
        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
            => config;

        /// <summary>
        ///     Releases any unmanaged resources owned by the deserializer (noop for this type).
        /// </summary>
        public void Dispose() { }

        public MessageBase Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return ByteConvertHelper.Bytes2Object<MessageBase>(data.ToArray());
        }
    }
}
