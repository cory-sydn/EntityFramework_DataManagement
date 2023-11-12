namespace ExcelToMSSQL.Services.Email
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
