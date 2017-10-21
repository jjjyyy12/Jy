using RabbitMQ.Client;


namespace Jy.RabbitMQ
{
    public interface IConnectionPool
    {
        IConnection Rent();

        bool Return(IConnection context);
    }
}
