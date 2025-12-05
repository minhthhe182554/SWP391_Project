using System.Net.Mail;
using System.Net;

namespace SWP391_Project.Services
{
    public class EmailService : IEmailService
    {
        private readonly string fromMail = "kakassj25@gmail.com";
        private readonly string fromPassword = "mviantbwravxpyon";

        public void SendMail(string toEmail, string subject, string body)
        {
            var message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);
            }
        }
    }
}
