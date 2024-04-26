using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        
        var randomQueueName = "log-database-save-queue";  //channel.QueueDeclare().QueueName;
        
        channel.QueueDeclare(randomQueueName, true, false, false);
  
        channel.QueueBind(randomQueueName, "logs-fanout", "", null);

        channel.BasicQos(0,1,false);
        var consumer = new EventingBasicConsumer(channel);

        Console.WriteLine("Loglar dinleniyor...");
        
        consumer.Received+= (object sender, BasicDeliverEventArgs e)=>
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            
            Thread.Sleep(1500);
            Console.WriteLine("Gelen Mesaj:" + message);

            channel.BasicAck(e.DeliveryTag,false);
        };
        channel.BasicConsume(randomQueueName, false, consumer);
        
        Console.ReadLine();
    }
}