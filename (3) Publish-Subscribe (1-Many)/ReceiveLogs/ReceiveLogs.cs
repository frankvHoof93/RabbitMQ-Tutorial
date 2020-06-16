using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace ReceiveLogs
{
    class ReceiveLogs
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    string queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                    Console.WriteLine(" [*] Waiting for Logs");

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                    consumer.Received += Consume;

                    channel.BasicConsume(queueName, true, consumer);

                    Console.WriteLine("Press [enter] to exit");

                    Console.ReadLine();
                }
            }
        }

        private static void Consume(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine(" [x] {0}", message);
        }
    }
}
