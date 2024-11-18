using Common.Events;
using EventBus.Base.Abstraction;

namespace NotificationService.EventHandler
{
    public class SmsEventHandler : IIntegrationEventHandler<NotifySmsEvent>
    {
        public Task Handle(NotifySmsEvent @event)
        {
            Random random = new Random();

            int randomInt = random.Next();

            if (randomInt % 2 == 0)
                return Task.FromResult(0);
            else
                throw new Exception("wrong ");
        }
    }
}
