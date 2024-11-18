using Common.Models;
using EventBus.Base.Events;

namespace Common.Events
{
    public class NotifySmsEvent : IntegrationEvent
    {
        public SmsNotifyModel NotifyModel { get; set; }
    }

    public class NotifyEmailEvent : IntegrationEvent
    {
        public EmailNotifyModel NotifyModel { get; set; }
    }
}
