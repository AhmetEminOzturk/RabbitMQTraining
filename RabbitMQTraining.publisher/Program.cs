using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared;

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

        channel.ExchangeDeclare("header-exchange", durable:true, type: ExchangeType.Headers);

        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape2", "a4");
        var properties = channel.CreateBasicProperties();
        properties.Headers = headers;
        properties.Persistent = true;

        var product = new Product {Id= 1, Name="Kalem", Price=100, Stock=15};
        var productJsonString = JsonSerializer.Serialize(product);

        channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

        Console.WriteLine("Mesaj gönderilmiştir");

        Console.ReadLine();
    }
}