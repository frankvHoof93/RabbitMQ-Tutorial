using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Worker
{
    class Program
    {
        private static IModel channel1;

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
                    channel1 = channel;
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.BasicQos(0, 1, false);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consume;
                    channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);

                    Console.WriteLine("Press Enter to Exit");
                    Console.ReadLine();
                }
            }
        }


        private static void Consume(object sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine(" [x] Received {0}", message);

            int dots = message.Split('.').Length - 1;
            Thread.Sleep(dots * 1000);
            IModel channel = ((EventingBasicConsumer)sender).Model;
            // Acknowledge
            channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
            Console.WriteLine(" [x] Finished Task");
        }
    }
}
