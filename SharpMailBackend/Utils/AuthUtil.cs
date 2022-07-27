using SharpMailBackend.Exceptions;
using SharpMailBackend.Net;

namespace SharpMailBackend.Utils;

public static class AuthUtil
{
    /// <summary>
    /// 账户有效性认证
    /// </summary>
    /// <returns>认证结果</returns>
    public static async Task<bool> Auth(string mail, string password,
        string? pop3Host, int pop3Port, bool pop3Ssl,
        string? smtpHost, int smtpPort, bool smtpSsl)
    {
        try
        {
            var pop3 = new Pop3Client(mail, password, new ServerUrl(pop3Host!, pop3Port, pop3Ssl));
            await pop3.ConnectAsync();
            await pop3.DisconnectAsync();

            var smtp = new SmtpClient(mail, password, new ServerUrl(smtpHost!, smtpPort, smtpSsl));
            await smtp.ConnectAsync();
            await smtp.DisconnectAsync();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}