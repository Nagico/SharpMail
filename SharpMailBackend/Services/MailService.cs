using System.Text;
using Microsoft.AspNetCore.Http.Features;
using SharpMailBackend.Net;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json.Linq;
using SharpMailBackend.Entities;
using SharpMailBackend.Exceptions;
using SharpMailBackend.Services.Serializers;
using SharpMailBackend.Utils;
using SharpMailBackend.ViewModels;

namespace SharpMailBackend.Services;

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
    /// 获取邮件对象
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="id">邮件ID</param>
    /// <returns>Mail对象</returns>
    /// <exception cref="AppError">无当前邮件</exception>
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
    /// <param name="type">筛选类型 1-收件 2-发件</param>
    /// <param name="read">筛选是否已读</param>
    /// <param name="orderBy">排序条件</param>
    /// <param name="page">页数</param>
    /// <param name="pageSize">分页</param>
    /// <returns>邮件总数, 邮件列表</returns>
    public async Task<(int, JArray)> GetMailList(int accountId, int? type, bool? read, string? orderBy, int page, int pageSize)
    {
        var mails = BuildQuery(_context.Mails.Where(m => m.AccountId == accountId), type, read, orderBy);
        var total = await mails.CountAsync();
        var mailList = await mails.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        

        var res = new JArray();
        foreach (var mail in mailList)
        {
            res.Add(MailSerializer.MailInfo(mail));
        }
        
        return (total, res);
    }
    
    /// <summary>
    /// 获取未读邮件总数
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <returns>未读邮件总数</returns>
    public async Task<int> GetUnreadMailCount(int accountId)
    {
        return await _context.Mails.CountAsync(m => m.AccountId == accountId && m.Read == false);
    }
    
    private IQueryable<Mail> BuildQuery(IQueryable<Mail> mails, int? type, bool? read, string? orderBy)
    {
        if (type.HasValue)
        {
            mails = mails.Where(m => m.Type == type.Value);
        }
        
        if (read.HasValue)
        {
            mails = mails.Where(m => m.Read == read.Value);
        }
        
        // 排序
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            var orderByList = orderBy!.Split(',').ToList().ConvertAll(s => s.Trim());
            foreach (var order in orderByList)
            {
                var asc = true;
                var orderField = order;
                
                // 处理排序方式
                if (order.StartsWith('-'))
                {
                    asc = false;
                    orderField = order[1..];
                }

                mails = orderField switch
                {
                    "date" => asc ? mails.OrderBy(m => m.Date) : mails.OrderByDescending(m => m.Date),
                    "subject" => asc ? mails.OrderBy(m => m.Subject) : mails.OrderByDescending(m => m.Subject),
                    "from" => asc ? mails.OrderBy(m => m.From) : mails.OrderByDescending(m => m.From),
                    "to" => asc ? mails.OrderBy(m => m.To) : mails.OrderByDescending(m => m.To),
                    _ => mails
                };
            }
        }
        else
        {
            mails = mails.OrderByDescending(m => m.Date);
        }

        return mails;
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
    
    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="mail">待发送邮件内容</param>
    /// <returns>已发送邮件ID</returns>
    /// <exception cref="AppError">发送失败</exception>
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
        
        foreach (var cc in mail.Cc ?? new List<string>())
            msg.Cc.Add(new MailboxAddress(GetMailAddressName(cc), cc));
        
        foreach (var bcc in mail.Bcc ?? new List<string>())
            msg.Bcc.Add(new MailboxAddress(GetMailAddressName(bcc), bcc));
        
        msg.Subject = mail.Subject;

        var html = new TextPart("html");
        html.Text = mail.HtmlBody;
        html.ContentTransferEncoding = ContentEncoding.Base64;
        
        var plain = new TextPart("plain");
        plain.Text = MailUtil.HtmlToText(mail.HtmlBody);
        plain.ContentTransferEncoding = ContentEncoding.Base64;
        
        var multipart = new Multipart("mixed");
        //multipart.Add(plain);
        multipart.Add(html);
        
        msg.Body = multipart;

        var streamer = new MemoryStream();
        await msg.WriteToAsync(streamer);
        streamer.Position = 0;
        
        var mailContent = await MailUtil.ToString(streamer);
        var receivers = mail.To.Concat(mail.Cc ?? new List<string>()).Concat(mail.Bcc ?? new List<string>()).ToList();

        try
        {
            var client = new SmtpClient(account.Email, account.Password!,
                new ServerUrl(account.SmtpHost!, account.SmtpPort, account.SmtpSsl));
            await client.ConnectAsync();
            await client.SendAsync(receivers, mailContent);
            await client.DisconnectAsync();
            
            var newMail = new Mail
            {
                AccountId = accountId,
                Uid = "",
                Type = 2,  // 已发送
                Read = true,
                Text = MailUtil.HtmlToSingleLineText(mail.HtmlBody),
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
                ["id"] = newMail.Id
            };
        }
        catch (SharpMailNetException e)
        {
            throw new AppError("C0000", "发送邮件失败", e.Message);
        }
    }
    
    /// <summary>
    /// 拉取新邮件
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
                    Text = MailUtil.HtmlToSingleLineText(msg.HtmlBody ?? msg.TextBody),
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
        catch (SharpMailNetException e)
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
        catch (SharpMailNetException e)
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