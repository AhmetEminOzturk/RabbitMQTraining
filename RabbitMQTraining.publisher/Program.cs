using System.Text;
using RabbitMQ.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://hhrxgvyw:nLanLp4m523bGry9nMprBsATwjmblzBy@cow.rmq2.cloudamqp.com/hhrxgvyw");

        using var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        //channel.QueueDeclare("hello-queue", true, false, false);

        channel.ExchangeDeclare("logs-fanout", durable:true, type: ExchangeType.Fanout);

        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            string message = $"log{x}";
            
            var messageBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("logs-fanout", "", null, messageBody);

            Console.WriteLine($"Mesajınız gönderilmiştir: {message}");
        });


        Console.ReadLine();
    }
}