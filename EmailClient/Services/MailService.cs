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
    public async Task<JObject> GetMail(int accountId, int id)
    {
        var mail = await _context.Mails.FirstOrDefaultAsync(m => m.AccountId == accountId && m.Id == id);
        if (mail == null)
        {
            throw new AppError("A0510", "邮件不存在");
        }
        
        mail.Read = true;
        await _context.SaveChangesAsync();

        var msg = await MimeMessage.LoadAsync(await MailUtil.ToStream(mail.Content));

        return await MailSerializer.MailDetailAsync(mail, msg);
    }
    
    public async Task<JObject> SendMail(int accountId, MailViewModel mail)
    {
        // TODO: 发送邮件
        throw new NotImplementedException();
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
                Type = 1,
                Read=false,
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
    
    /// <summary>
    /// 邮件已读
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="id">邮件ID</param>
    /// <returns>结果</returns>
    /// <exception cref="AppError">无当前邮件</exception>
    public async Task<JObject> ReadMail(int accountId, int id)
    {
        var mail = await _context.Mails.FirstOrDefaultAsync(m => m.AccountId == accountId && m.Id == id);
        if (mail == null)
        {
            throw new AppError("A0510", "邮件不存在");
        }
        
        mail.Read = true;
        await _context.SaveChangesAsync();

        return new JObject
        {
            ["read"] = true
        };
    }
}