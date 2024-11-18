using EventBus.Base;
using EventBus.Base.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly IModel consumerChannel;
        private const string DeadLetterQueueName = "_dead_letter_queue";
        private const string RetryHeaderKey = "x-retry-count";
        private const int MaxRetries = 3;

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (EventBusConfig.Connection != null)
            {
                if (EventBusConfig.Connection is ConnectionFactory)
                    connectionFactory = EventBusConfig.Connection as ConnectionFactory;
                else
                {
                    var connJson = JsonSerializer.Serialize(EventBusConfig.Connection, new JsonSerializerOptions()
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve // Döngüsel referansları yönetmek için
                    });

                    connectionFactory = JsonSerializer.Deserialize<ConnectionFactory>(connJson);
                }
            }
            else
                connectionFactory = new ConnectionFactory(); // Create with default values

            persistentConnection = new RabbitMQPersistentConnection(connectionFactory, config.ConnectionRetryCount);

            consumerChannel = CreateConsumerChannel();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            consumerChannel.QueueUnbind(queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName);

            if (SubsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct"); // Ensure exchange exists while publishing

            consumerChannel.QueueDeclare(queue: eventName + DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Declare Main Queue with dead-letter configuration
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" }, // Direct to default exchange for simplicity
                { "x-dead-letter-routing-key", eventName + DeadLetterQueueName }
            };

            consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while publishing
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: queueArgs);

            consumerChannel.QueueBind(queue: GetSubName(eventName),
                              exchange: EventBusConfig.DefaultTopicName,
                              routingKey: eventName);


            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent

            consumerChannel.BasicPublish(
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                //consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while consuming
                //                     durable: true,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                //consumerChannel.QueueBind(queue: GetSubName(eventName),
                //                  exchange: EventBusConfig.DefaultTopicName,
                //                  routingKey: eventName);
            }

            SubsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void UnSubscribe<T, TH>()
        {
            SubsManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                                    type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(
                    queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            int currentRetryCount = 0;

            if (eventArgs.BasicProperties.Headers != null && eventArgs.BasicProperties.Headers.ContainsKey(RetryHeaderKey))
            {
                // Extract the current retry count from headers
                var retryHeaderValue = eventArgs.BasicProperties.Headers[RetryHeaderKey];
                if (retryHeaderValue is byte[] headerValueBytes)
                {
                    currentRetryCount = int.Parse(Encoding.UTF8.GetString(headerValueBytes));
                }
            }

            try
            {
                var result = await ProcessEvent(eventName, message);
                if (result)
                    consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                else
                    throw new Exception();

            }
            catch (Exception ex)
            {
                if (currentRetryCount < MaxRetries)
                {
                    // Increment retry count and requeue the message

                    var properties = consumerChannel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Headers = properties.Headers ?? new Dictionary<string, object>();
                    properties.Headers[RetryHeaderKey] = Encoding.UTF8.GetBytes((currentRetryCount + 1).ToString());

                    // Requeue the message
                    consumerChannel.BasicPublish(exchange: eventArgs.Exchange, routingKey: eventArgs.RoutingKey, basicProperties: properties, body: eventArgs.Body);
                    consumerChannel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
                else
                {
                    // Max retries reached, move to DLQ
                    consumerChannel.BasicReject(deliveryTag: eventArgs.DeliveryTag, requeue: false);
                }
            }

        }
    }
}
