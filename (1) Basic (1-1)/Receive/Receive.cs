using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Receive
{
    class Receive
    {
        static void Main()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consume;
                    channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                    Console.WriteLine("Press Enter to Exit");
                    Console.ReadLine();
                }
            }
        }

        private static void Consume(object model, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine(" [x] Received {0}", message);
        }
    }
}
