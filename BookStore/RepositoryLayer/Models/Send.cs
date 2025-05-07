using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace RepositoryLayer.Models
{
    public class Send
    {
        public string SendMail(string ToMail, string Token)
        {
            //forMailContent
            string FromEmail = "pujaborse13@gmail.com";
            MailMessage Message = new MailMessage(FromEmail, ToMail);
            string MailBody = "Token Generated :" + Token;


            //this if for Angular

            // string resultUrl = $"https://4200/rest-password?token= {Token}";
            //string MailBody = $@"
            //<p>Your Password Reset token : <strong>{Token}</strong></p>
            //<p>Click Below Link To Rest Your Password :</p>
            //<p><a.href='{restUrl}'> {resetUrl}</a></p>;   
            Message.Subject = "Token Generated for Forgot Password";
            Message.Body = MailBody.ToString();
            Message.BodyEncoding = Encoding.UTF8;
            Message.IsBodyHtml = true;


            //SMTP client 
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); //587 gmail port no
            NetworkCredential credential = new NetworkCredential("pujaborse13@gmail.com", "tswm qqrv onim xmol");

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = credential;

            smtpClient.Send(Message);
            return ToMail;



        }

        }
    }
