using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;


namespace EmailConsumer
{
    public class RabbitMQEmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    public class Program
    {
        static void Main()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" }; 

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "emailQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var emailModel = JsonConvert.DeserializeObject<RabbitMQEmailModel>(json);

                    // Send Email
                    SendMail(emailModel.ToEmail, emailModel.Subject, emailModel.Body);
                };

                //Tells RabbitMQ to start consuming messages from "emailQueue".
                channel.BasicConsume(queue: "emailQueue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Listening for messages. Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static void SendMail(string toEmail, string subject, string body)
        {
            Console.WriteLine($"Sending email to {toEmail} with subject '{subject}'");

            string fromEmail = "pujaborse13@gmail.com";
            string fromPassword = "tswm qqrv onim xmol";

            MailMessage message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, fromPassword)
            };

            try
            {
                smtpClient.Send(message);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
            }
        }


    }
}