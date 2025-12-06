namespace SWP391_Project.Services
{
    public interface IEmailService
    {
        void SendMail(string toEmail, string subject, string body);
    }
}
