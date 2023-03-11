using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EventBusRabbitMQ.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQProducer> _logger;
        private readonly int _retryCount;

        public EventBusRabbitMQProducer(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQProducer> logger, int retryCount = 5)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _retryCount = retryCount;
        }

        public void Publish(string queueName, IEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<SocketException>()
               .Or<BrokerUnreachableException>()
               .WaitAndRetry(_retryCount, rettryAttempt => TimeSpan.FromSeconds(Math.Pow(2, rettryAttempt)), (ex, time) =>
               {
                   _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.Seconds}", ex.Message);
               });

            using IModel channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            string message = JsonConvert.SerializeObject(@event);
            byte[] body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine("Sent RabbitMQ");

                };
            });

        }
    }
}
