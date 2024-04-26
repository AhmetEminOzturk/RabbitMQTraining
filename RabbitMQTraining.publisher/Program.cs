using System.Text;
using RabbitMQ.Client;

public enum LogNames
{
    Critical=1,
    Error =2,
    Warning=3,
    Info=4
}
internal class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://hhrxgvyw:nLanLp4m523bGry9nMprBsATwjmblzBy@cow.rmq2.cloudamqp.com/hhrxgvyw");

        using var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        channel.ExchangeDeclare("logs-direct", durable:true, type: ExchangeType.Direct);

        Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
        {
            var routeKey = $"route-{x}";    
            
            var queueName = $"direct-queue-{x}";

            channel.QueueDeclare(queueName,true,false,false);

            channel.QueueBind(queueName,"logs-direct",routeKey,null);
        });

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            LogNames log = (LogNames)new Random().Next(1,5);

            string message = $"log type: {log}";
            
            var routeKey = $"route-{log}";

            var messageBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("logs-direct", routeKey, null, messageBody);

            Console.WriteLine($"Log gönderilmiştir: {message}");
        });


        Console.ReadLine();
    }
}