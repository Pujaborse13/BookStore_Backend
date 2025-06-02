using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RepositoryLayer.Models;

namespace RepositoryLayer.Helper
{
    public class RabbitMQProducer
    {
        private readonly ConnectionFactory factory;

        public RabbitMQProducer()
        {
            //Creates connection to RabbitMQ server.
            factory = new ConnectionFactory() { 
                HostName = "localhost" };
        }

        public void SendMessage(RabbitMQEmailModel emailModel)
        {
            //establish connection and open channel to communicate with queue
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "emailQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var json = JsonConvert.SerializeObject(emailModel);
                var body = Encoding.UTF8.GetBytes(json);


                //Publishes the message
                channel.BasicPublish(exchange: "", //Empty "" means default direct exchange.
                                     routingKey: "emailQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}

