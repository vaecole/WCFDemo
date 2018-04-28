using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ_Demo
{
    internal class Demo
    {
        private IConnection connection = null;
        private IModel channel = null;
        private string exchangeName = "my_exchg";
        private string queueName = "my_queue";
        private string routingKey = "my_key";
        public Demo(string name)
        {
            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";

            connection = factory.CreateConnection();

            channel = connection.CreateModel();
            // channel.ExchangeDelete(exchangeName, false);
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            consumer = new EventingBasicConsumer(channel);
            this.name = name;
            consumer.Received += (ch, ea) =>
              {
                  var body = ea.Body;
                  Console.WriteLine(this.name + " Received: " + Encoding.UTF8.GetString(body));
                  channel.BasicAck(ea.DeliveryTag, false);
              };

            consumerTag = channel.BasicConsume(queueName, false, consumer);
        }

        private string name = "default";
        private EventingBasicConsumer consumer = null;
        private string consumerTag = null;
        public void SendMSG(string msg)
        {
            Console.WriteLine(name + " SendMSG: " + msg);
            var content = Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(exchangeName, routingKey, null, content);
        }
    }
}
