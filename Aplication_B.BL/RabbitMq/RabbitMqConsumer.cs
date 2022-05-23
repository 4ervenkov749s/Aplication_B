using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aplication_B.BL.RabbitMq
{
    public class RabbitMqConsumer : IHostedService, IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IPersonDataFlow _personDataFlow;
        public RabbitMqConsumer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };


            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("person",ExchangeType.Direct, durable: true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                _channel.QueueDeclare(queue: "person", durable: true, exclusive: false, autoDelete: false);

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (sender, ea) =>
                {

                    _personDataFlow.SendPerson(ea.Body.ToArray());

                };
                _channel.BasicConsume(queue: "person", autoAck: true, consumer: consumer);
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

    }
}
