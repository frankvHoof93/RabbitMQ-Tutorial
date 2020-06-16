using RabbitMQ.Client;
using System;
using System.Text;

namespace EmitLogs
{
    class EmitLogs
    {
        public static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    // Possible Types: Direct, Topic, Headers, Fanout
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    string message = GetMessage(args);
                    byte[] body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }

                Console.WriteLine("Press [enter] to exit");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "info: Hello World";
        }
    }
}
