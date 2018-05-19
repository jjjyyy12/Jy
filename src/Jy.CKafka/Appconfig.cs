using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CKafka
{
    public class AppConfig
    {

        public static IConfigurationRoot Configuration { get; set; }

        private static IEnumerable<KeyValuePair<string, object>> _kafkaConfig;

        public static IEnumerable<KeyValuePair<string, object>> KafkaConfig
        {
            get
            {
                return _kafkaConfig;
            }
            internal set
            {
                _kafkaConfig = value;
            }
        }
    }
}
