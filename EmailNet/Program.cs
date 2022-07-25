// See https://aka.ms/new-console-template for more information

using EmailNet;
using MimeKit;

async Task<Stream> ToStream(string? text)
{
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    await writer.WriteAsync(text);
    await writer.FlushAsync();
    stream.Position = 0;
    return stream;
}

var email = "co_test1@163.com";
var password = "ASSKUOTNRNQETWPU";
var pop3Server = new ServerUrl("pop3.163.com", 110);
var smtpServer = new ServerUrl("smtp.163.com", 25);

var pop3Client = new Pop3Client(email, password, pop3Server);
await pop3Client.ConnectAsync();

var count = await pop3Client.GetMailCountAsync();
Console.WriteLine("You have {0} emails in your inbox.", count);

for (var i = 1; i <= count; i++)
{
    var uid = await pop3Client.GetMaidUidAsync(i);
    var mail = await pop3Client.GetMailContentAsync(i);
    var msg = await MimeMessage.LoadAsync(await ToStream(mail));
    Console.WriteLine("[{0}]\n{1}", uid, mail);
}

await pop3Client.DisconnectAsync();




var smtpClient = new SmtpClient(email, password, smtpServer);
await smtpClient.ConnectAsync();
await smtpClient.DisconnectAsync();