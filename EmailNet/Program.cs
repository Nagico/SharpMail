// See https://aka.ms/new-console-template for more information

using EmailNet;

var email = "co_test1@163.com";
var password = "ASSKUOTNRNQETWPU";
var pop3Server = new ServerUrl("pop3.163.com", 110);
var smtpServer = new ServerUrl("smtp.163.com", 25);

var pop3Client = new Pop3Client(email, password, pop3Server);
pop3Client.Connect();

var count = pop3Client.GetMailCount();
Console.WriteLine("You have {0} emails in your inbox.", count);

for (var i = 1; i <= count; i++)
{
    var uid = pop3Client.GetMaidUid(i);
    var mail = pop3Client.GetMailContent(i);
    Console.WriteLine("[{0}]\n{1}", uid, mail);
}

pop3Client.Disconnect();

var smtpClient = new SmtpClient(email, password, smtpServer);
smtpClient.Connect();
smtpClient.Disconnect();