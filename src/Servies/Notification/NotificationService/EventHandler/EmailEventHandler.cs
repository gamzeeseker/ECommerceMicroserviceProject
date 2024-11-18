using Common.Events;
using EventBus.Base.Abstraction;
using NotificationService.Services.Abstract;

namespace NotificationService.EventHandler
{
    public class EmailEventHandler : IIntegrationEventHandler<NotifyEmailEvent>
    {
        private readonly IEmailSender _emailSender;

        public EmailEventHandler(IEmailSender emailSender)
        {
            this._emailSender = emailSender;
        }

        public Task Handle(NotifyEmailEvent @event)
        {
            //send Email with _email sender
            Random random = new Random();

            int randomInt = random.Next();

            if (randomInt % 2 == 0)
                return Task.FromResult(0);
            else
                throw new Exception("wrong ");
        }
    }
}
