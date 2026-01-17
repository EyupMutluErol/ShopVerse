using System.Net;
using System.Net.Mail;

namespace ShopVerse.WebUI.Utils;

public class EmailHelper
{
    public void SendEmail(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("eyupmutluerol@gmail.com", "ShopVerse");

        const string fromPassword = "nwpw xzdc hkbp nysh";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using (var message = new MailMessage(fromAddress, new MailAddress(toEmail))
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        })
        {
            smtp.Send(message);
        }
    }
}
