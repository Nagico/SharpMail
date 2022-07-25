using EmailClient.Entities;
using EmailClient.Exceptions;
using EmailClient.Services.Serializers;
using EmailClient.Utils;
using EmailClient.ViewModels;
using EmailNet;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace EmailClient.Services;

/// <summary>
/// 邮件服务类
/// </summary>
public class MailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailClientContext _context;
    private readonly MailSerializer _mailSerializer;
    
    /// <summary>
    /// 初始化
    /// </summary>
    public MailService(IConfiguration configuration, EmailClientContext context)
    {
        _configuration = configuration;
        _context = context;
        _mailSerializer = new MailSerializer(_context);
    }

    private async Task<Mail> GetMail(int accountId, int id)
    {
        var mail = await _context.Mails.FirstOrDefaultAsync(m => m.AccountId == accountId && m.Id == id);
        if (mail == null)
        {
            throw new AppError("A0510", "邮件不存在");
        }
        return mail;
    }
    
    /// <summary>
    /// 获取邮件列表
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <returns>邮件列表</returns>
    public async Task<JArray> GetMailList(int accountId)
    {
        var mails = await _context.Mails
            .Where(m => m.AccountId == accountId && m.Type == 1)
            .OrderByDescending(m => m.Date)
            .ToListAsync();

        var res = new JArray();
        foreach (var mail in mails)
        {
            res.Add(MailSerializer.MailInfo(mail));
        }
        
        return res;
    }
    
    /// <summary>
    /// 获取邮件详情
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="id">邮件ID</param>
    /// <returns>邮件详情</returns>
    /// <exception cref="AppError">无当前ID邮件</exception>
    public async Task<JObject> GetMailDetail(int accountId, int id)
    {
        var mail = await GetMail(accountId, id);
        
        mail.Read = true;
        await _context.SaveChangesAsync();

        var msg = await MimeMessage.LoadAsync(await MailUtil.ToStream(mail.Content));

        return await MailSerializer.MailDetailAsync(mail, msg);
    }

    private string GetMailAddressName(string address)
    {
        return address.Split("@")[0];
    }
    
    public async Task<JObject> SendMail(int accountId, MailViewModel mail)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new AppError("A0510", "账户不存在");
        }
        
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(GetMailAddressName(account.Email), account.Email));
        foreach (var to in mail.To)
            msg.To.Add(new MailboxAddress(GetMailAddressName(to), to));
        
        foreach (var cc in mail.Cc)
            msg.Cc.Add(new MailboxAddress(GetMailAddressName(cc), cc));
        
        foreach (var bcc in mail.Bcc)
            msg.Bcc.Add(new MailboxAddress(GetMailAddressName(bcc), bcc));
        
        msg.Subject = mail.Subject;
        
        var builder = new BodyBuilder();
        builder.TextBody = MailUtil.HtmlToText(mail.HtmlBody);
        builder.HtmlBody = mail.HtmlBody;
        
        msg.Body = builder.ToMessageBody();

        var streamer = new MemoryStream();
        await msg.WriteToAsync(streamer);
        streamer.Position = 0;
        
        var mailContent = await MailUtil.ToString(streamer);

        try
        {
            var client = new SmtpClient(account.Email, account.Password!,
                new ServerUrl(account.SmtpHost!, account.SmtpPort, account.SmtpSsl));
            await client.ConnectAsync();
            await client.SendAsync(mailContent);
            await client.DisconnectAsync();
            
            var newMail = new Mail
            {
                AccountId = accountId,
                Uid = "",
                Type = 2,  // 已发送
                Read = true,
                Subject = msg.Subject,
                From = _mailSerializer.AddressInfo(msg.From),
                To = _mailSerializer.AddressInfo(msg.To),
                Content = mailContent,
                Date = msg.Date.DateTime,
                CreateTime = DateTime.Now
            };
            _context.Mails.Add(newMail);
            await _context.SaveChangesAsync();

            return new JObject
            {
                ["status"] = "success"
            };
        }
        catch (EmailNetException e)
        {
            throw new AppError("C0000", "发送邮件失败", e.Message);
        }
    }
    
    /// <summary>
    /// 更新邮箱
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <returns>更新结果</returns>
    public async Task<JObject> UpdateMailBox(int accountId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new AppError("A0310", "账户不存在");
        }

        try
        {
            var client = new Pop3Client(account.Email, account.Password!,
                new ServerUrl(account.Pop3Host!, account.Pop3Port, account.Pop3Ssl));
            await client.ConnectAsync();
            var mailCount = await client.GetMailCountAsync();

            var cnt = 0; // 记录新增邮件个数

            for (var i = mailCount; i > 0; i--)
            {
                var mailUid = await client.GetMaidUidAsync(i);
                if (await _context.Mails.AnyAsync(m => m.AccountId == accountId && m.Uid == mailUid))
                {
                    break;
                }

                cnt++;
                var mailContent = await client.GetMailContentAsync(i);
                var msg = await MimeMessage.LoadAsync(await MailUtil.ToStream(mailContent));
                var mail = new Mail
                {
                    AccountId = accountId,
                    Uid = mailUid,
                    Type = 1,  // 收件箱
                    Read = false,
                    Subject = msg.Subject,
                    From = _mailSerializer.AddressInfo(msg.From),
                    To = _mailSerializer.AddressInfo(msg.To),
                    Content = mailContent,
                    Date = msg.Date.DateTime,
                    CreateTime = DateTime.Now
                };

                await _context.Mails.AddAsync(mail);
            }

            await client.DisconnectAsync();
            await _context.SaveChangesAsync();
            
            return new JObject
            {
                ["new"] = cnt
            };
        }
        catch (EmailNetException e)
        {
            throw new AppError("C0000", e.Message);
        }
    }
    
    /// <summary>
    /// 邮件已读
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="id">邮件ID</param>
    /// <returns>结果</returns>
    /// <exception cref="AppError">无当前邮件</exception>
    public async Task<JObject> ReadMail(int accountId, int id)
    {
        var mail = await GetMail(accountId, id);
        
        mail.Read = true;
        await _context.SaveChangesAsync();

        return new JObject
        {
            ["read"] = true
        };
    }
    
    /// <summary>
    /// 删除邮件
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="id">邮件ID</param>
    /// <exception cref="AppError">无当前邮件</exception>
    public async Task DeleteMail(int accountId, int id)
    {
        var mail = await GetMail(accountId, id);
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new AppError("A0310", "账户不存在");
        }

        try
        {
            var client = new Pop3Client(account.Email, account.Password!,
                new ServerUrl(account.Pop3Host!, account.Pop3Port, account.Pop3Ssl));

            await client.ConnectAsync();
            await client.DeleteMailAsync(mail.Uid);
            await client.DisconnectAsync();
        }
        catch (EmailNetException e)
        {
            throw new AppError("C0000", e.Message);
        }
        finally
        {
            _context.Mails.Remove(mail);
            await _context.SaveChangesAsync();
        }
    }
}