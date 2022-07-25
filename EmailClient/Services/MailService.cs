using EmailClient.Entities;
using EmailClient.Exceptions;
using EmailClient.Services.Serializers;
using EmailClient.Utils;
using EmailClient.ViewModels;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace EmailClient.Services;

public class MailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailClientContext _context;
    private readonly MailSerializer _mailSerializer;
    
    public MailService(IConfiguration configuration, EmailClientContext context)
    {
        _configuration = configuration;
        _context = context;
        _mailSerializer = new MailSerializer(_context);
    }

    public async Task<JArray> GetMailList(int accountId)
    {
        var mails = await _context.Mails.Where(m => m.AccountId == accountId && m.Type == 1).ToListAsync();

        var res = new JArray();
        foreach (var mail in mails)
        {
            res.Add(MailSerializer.MailInfo(mail));
        }
        
        return res;
    }

    public async Task<JObject> GetMail(int accountId, int id)
    {
        var mail = await _context.Mails.FirstOrDefaultAsync(m => m.AccountId == accountId && m.Id == id);
        if (mail == null)
        {
            throw new AppError("A0510", "邮件不存在");
        }

        var msg = await MimeMessage.LoadAsync(await MailUtil.ToStream(mail.Content));

        return await MailSerializer.MailDetailAsync(mail, msg);
    }

    public async Task<JObject> SendMail(int accountId, MailViewModel mail)
    {
        // TODO: 发送邮件
        throw new NotImplementedException();
    }

    public async Task<object?> UpdateMailBox(int accountId)
    {
        throw new NotImplementedException();
    }
}