using Common.Events;
using EventBus.Factory.EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.EventHandler;
var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);

// Build the service provider to resolve services
var serviceProvider = serviceCollection.BuildServiceProvider();

var eventBus =
               EventBusFactory.Create(new EventBus.Base.EventBusConfig()
               {
                   EventBusConnectionString = "http:\\localhost:5672",
                   SubscriberClientAppName = "order"
               }, serviceProvider);

eventBus.Subscribe<NotifySmsEvent, SmsEventHandler>();
eventBus.Subscribe<NotifyEmailEvent, EmailEventHandler>();

static void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<EventHandler>();
    services.AddTransient<EmailEventHandler>();
}

Console.ReadLine();
