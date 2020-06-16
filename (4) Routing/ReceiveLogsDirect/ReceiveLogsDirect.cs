using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

// Direct Exchange
// A message goes to the queues whose binding key 
// matches the routing key of the message
// If no queue matches, message is DISCARDED!
// If multiple queues match, message is broadcast to all
namespace ReceiveLogsDirect
{
    class ReceiveLogsDirect
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
                    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
                    string queueName = channel.QueueDeclare().QueueName;

                    if (args.Length < 1)
                    {
                        Console.Error.WriteLine("Usage {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
                        Console.WriteLine("Press [enter] to exit");
                        Console.ReadLine();
                        Environment.ExitCode = 1;
                        return;
                    }

                    // Severities
                    for (int i = 0; i < args.Length; i++)
                    {
                        channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: args[i]);
                    }

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consume;

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine(" [*] Awaiting Messages");
                    Console.WriteLine(" Press [enter] to exit");
                    Console.ReadLine();
                }
            }
        }

        private static void Consume(object model, BasicDeliverEventArgs e)
        {
            byte[] body = e.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);
            string routingKey = e.RoutingKey;
            Console.WriteLine(" [x] Received '{0}':'{1}", routingKey, message);
        }
    }
}
