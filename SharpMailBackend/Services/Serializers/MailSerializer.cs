﻿using System.Collections;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpMailBackend.Entities;
using SharpMailBackend.Utils;

namespace SharpMailBackend.Services.Serializers;

public class MailSerializer : BaseSerializer
{
    public MailSerializer(EmailClientContext context) : base(context) {}

    public static JObject MailInfo(Mail mail)
    {
        var res = new JObject
        {
            ["id"] = mail.Id,
            ["from"] = mail.From,
            ["to"] = mail.To,
            ["subject"] = mail.Subject,
            ["text"] = mail.Text,
            ["date"] = mail.Date,
            ["read"] = mail.Read
        };
        
        return res;
    }

    public static JObject AddressDetail(MailboxAddress address)
    {
        return new JObject
        {
            ["address"] = address.Address,
            ["name"] = address.Name
        };
    }

    public static async Task<JObject> AttachmentDetailAsync(MimePart attachment)
    {
        return new JObject
        {
            ["filename"] = attachment.FileName,
            ["content_type"] = attachment.ContentType.ToString(),
            ["content_disposition"] = attachment.ContentDisposition.ToString(),
            ["content_transfer_encoding"] = attachment.ContentTransferEncoding.ToString(),
            ["content"] = await MailUtil.ToString(attachment.Content.Stream),
            ["size"] = attachment.Content.Stream.Length
        };
    }

    public static async Task<JArray> AttachmentListAsync(List<MimeEntity> attachments)
    {
        var res = new JArray();

        foreach (var attachment in attachments)
        {
            if (attachment is MimePart part)
            {
                res.Add(await AttachmentDetailAsync(part));
            }
        }

        return res;
    }

    public string AddressInfo(InternetAddressList addressList)
    {
        var res = new List<string>();
        
        foreach (var address in addressList)
        {
            if (address is MailboxAddress mailboxAddress)
            {
                res.Add(string.IsNullOrEmpty(mailboxAddress.Name) ? mailboxAddress.Address : mailboxAddress.Name);
            }
        }
        
        return string.Join(", ", res);
    }
    
    private static JArray AddressList(InternetAddressList addresses)
    {
        var res = new JArray();
        foreach (var address in addresses)
        {
            if (address is MailboxAddress mailboxAddress)
            {
                res.Add(AddressDetail(mailboxAddress));
            }
        }
        return res;
    }
    
    public static async Task<JObject> MailDetailAsync(Mail mail, MimeMessage msg)
    {
        var res = new JObject
        {
            ["id"] = mail.Id,
            ["from"] = AddressList(msg.From),
            ["to"] = AddressList(msg.To),
            ["cc"] = AddressList(msg.Cc),
            ["bcc"] = AddressList(msg.Bcc),
            ["subject"] = msg.Subject,
            ["date"] = mail.Date,
            ["content"] = msg.HtmlBody ?? msg.TextBody,
            ["importance"] = msg.Importance.ToString(),
            ["attachments"] = await AttachmentListAsync(msg.Attachments.ToList())
        };
        return res;
    }
}