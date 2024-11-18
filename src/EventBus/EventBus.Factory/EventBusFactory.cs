using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
    namespace EventBus.Factory
    {
        public static class EventBusFactory
        {
            public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
            {
                return new EventBusRabbitMQ(config, serviceProvider);
            }
        }
    }
}
