using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

internal class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://hhrxgvyw:nLanLp4m523bGry9nMprBsATwjmblzBy@cow.rmq2.cloudamqp.com/hhrxgvyw");

        using var connection = factory.CreateConnection();

        var channel = connection.CreateModel();
       
        channel.ExchangeDeclare("header-exchange", durable:true, type: ExchangeType.Headers);

        channel.BasicQos(0,1,false);
        var consumer = new EventingBasicConsumer(channel);

        var queueName = channel.QueueDeclare().QueueName;

        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape", "a4");
        headers.Add("x-match", "any");
        channel.QueueBind(queueName, "header-exchange" , string.Empty, headers);
        Console.WriteLine("Loglar dinleniyor...");
        
        consumer.Received+= (object sender, BasicDeliverEventArgs e)=>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            Product product = JsonSerializer.Deserialize<Product>(message);
            
            Thread.Sleep(1500);
            Console.WriteLine($"Gelen Mesaj: {product.Id} - {product.Name} - {product.Price} - {product.Stock}" );
            
            channel.BasicAck(e.DeliveryTag,false);
        };
        channel.BasicConsume(queueName, false, consumer);
        
        Console.ReadLine();
    }
}