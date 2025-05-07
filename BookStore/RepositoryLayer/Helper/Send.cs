using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RepositoryLayer.Helper
{
    public class Send
    {
        public string SendMail(string toMail, string token)
        {
            //forMailContent
            string fromEmail = "pujaborse13@gmail.com";
            string fromPassword = "tswm qqrv onim xmol";

            MailMessage message = new MailMessage(fromEmail, toMail);
            message.Subject = "Password Reset Token";
            message.Body = $"Your password reset token is: {token}";
            message.IsBodyHtml = true;


            //string MailBody = "Token Generated :" + Token;


            //this if for Angular

            // string resultUrl = $"https://4200/rest-password?token= {Token}";
            //string MailBody = $@"
            //<p>Your Password Reset token : <strong>{Token}</strong></p>
            //<p>Click Below Link To Rest Your Password :</p>
            //<p><a.href='{restUrl}'> {resetUrl}</a></p>;   

            //Message.Subject = "Token Generated for Forgot Password";
            //Message.Body = MailBody.ToString();
            //Message.BodyEncoding = Encoding.UTF8;
            //Message.IsBodyHtml = true;


            //SMTP client 
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); //587 gmail port no
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);

            //NetworkCredential credential = new NetworkCredential("pujaborse13@gmail.com", "tswm qqrv onim xmol");

           // smtpClient.EnableSsl = true;
            //smtpClient.UseDefaultCredentials = false;
            //smtpClient.Credentials = credential;

            smtpClient.Send(message);
            return toMail;



        }

        }
    }
