using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

// Direct Exchange
// A message goes to the queues whose binding key 
// matches the routing key of the message
// If no queue matches, message is DISCARDED!
// If multiple queues match, message is broadcast to all
namespace EmitLogsDirect
{
    class Program
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

                    string severity = args.Length > 0 ? args[0] : "info";
                    string message = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Hello World!";

                    byte[] body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: body);

                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);

                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
