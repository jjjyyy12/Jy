using Jy.Utility.Convert;
using Microsoft.Extensions.DependencyInjection;

namespace Jy.Utility
{
    public static class SerializerExtension
    {
        public static IServiceCollection AddSerializerServices(this IServiceCollection services)
        {  
            var jsonSerializer = new JsonSerializer();//ISerializer<string>
            var stringByteArraySerializer = new StringByteArraySerializer(jsonSerializer);//ISerializer<byte[]>
            var stringObjectSerializer = new StringObjectSerializer(jsonSerializer);//ISerializer<object>
            services.AddSingleton(jsonSerializer);
            services.AddSingleton(stringByteArraySerializer);
            return services.AddSingleton(stringObjectSerializer);
        }
         
    }
}
