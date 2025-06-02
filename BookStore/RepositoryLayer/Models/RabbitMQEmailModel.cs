using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class RabbitMQEmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
