using System.Net.Sockets;
using System.Text;
using System.Net;

namespace SharpMail.Net;

public class SmtpClient : BaseClient
{
    public Dictionary<int, string> SMTPResponseCode = new Dictionary<int, string>
    {
        {211, "系统状态或显示系统帮助"},
        {214, "帮助信息"},
        {220, "服务就绪"},
        {221, "服务关闭"},
        {250, "要求的邮件操作完成"},
        {251, "用户非本地，将转发向＜forward-path＞"},
        {354, "开始邮件输入，以'.'结束"},
        {421, "服务未就绪，关闭传输信道"},
        {450, "要求的邮件操作未完成，邮箱不可用"},
        {451, "放弃要求的操作；处理过程中出错"},
        {452, "系统存储不足，要求的操作未执行"},
        {500, "命令格式错误，不可识别。当命令行太长时也会发生这样的错误。"},
        {501, "参数格式错误"},
        {502, "命令不可实现"},
        {503, "错误的命令序列"},
        {504, "命令参数不可实现"},
        {550, "一个或多个收件人邮箱地址不存在"},
        {551, "用户非本地，请尝试＜forward-path＞"},
        {552, "过量的存储分配，要求的操作未执行"},
        {553, "邮箱名不可用，要求的操作未执行"},
        {554, "操作失败"}
    };

    public SmtpClient(string email, string password, ServerUrl smtpServer) : base(email, password, smtpServer)
    {
    }

    protected override void CheckResponse(string response, string? msg = null, ClientState next = ClientState.Unconnected)
    {
        Console.WriteLine("收到SMTP服务器回复：" + response);
        var code = int.Parse(response.Substring(0, 3));

        if (code == 221)
            State = ClientState.Unconnected;
        else if (code == 235)
            State = ClientState.Connected;
        else if (code >= 400)
            throw new SmtpException(msg ?? SMTPResponseCode[code]);
    }

    private static string ToBase64(string str)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    private static string FromBase64(string str)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(str));
    }

    /// <summary>
    /// 登录
    /// </summary>
    public new void Connect()
    {
        base.Connect();

        // 如果已经连接，则不再连接
        if (State == ClientState.Connected) return;

        // 验证服务器
        SendCommand("HELO " + Dns.GetHostName());
        var response = ReceiveFirstLine();
        while (response[3] == '-')
            response = ReceiveFirstLine();
        CheckResponse(response, "服务器连接失败");

        SendCommand("AUTH LOGIN");
        response = ReceiveFirstLine();
        CheckResponse(response, "登录失败");

        //验证用户名
        SendCommand(ToBase64(email));
        response = ReceiveFirstLine();
        CheckResponse(response, "用户名验证失败");

        //验证密码
        SendCommand(ToBase64(password));
        response = ReceiveFirstLine();
        CheckResponse(response, "密码验证失败");
    }

    /// <summary>
    /// 登录
    /// </summary>
    public new async Task ConnectAsync()
    {
        await base.ConnectAsync();

        // 如果已经连接，则不再连接
        if (State == ClientState.Connected) return;

        // 验证服务器
        await SendCommandAsync("HELO " + Dns.GetHostName());
        var response = await ReceiveFirstLineAsync();
        CheckResponse(response, "服务器验证失败");

        await SendCommandAsync("AUTH LOGIN");
        response = await ReceiveFirstLineAsync();
        CheckResponse(response, "登录失败");

        //验证用户名
        await SendCommandAsync(ToBase64(email));
        response = await ReceiveFirstLineAsync();
        CheckResponse(response, "用户名验证失败");

        //验证密码
        await SendCommandAsync(ToBase64(password));
        response = await ReceiveFirstLineAsync();
        CheckResponse(response, "密码验证失败");

        //验证成功，进入连接状态
        State = ClientState.Connected;
    }

    public async Task SendAsync(List<string> receivers, string content)
    {
        // 设置发件人
        await SendCommandAsync($"MAIL FROM:<{email}>");
        var response = await ReceiveFirstLineAsync();
        CheckResponse(response);

        // 设置收件人
        var currentReceiver = "";
        try
        {
            receivers.ForEach(async receiver => {
                currentReceiver = receiver;
                await SendCommandAsync($"RCPT TO:<{receiver}>");
                response = await ReceiveFirstLineAsync();
                CheckResponse(response);
            });
        }
        catch (SmtpException)
        {
            throw new SmtpException($"收件人'{currentReceiver}'的邮箱地址不存在");
        }

        // 发送邮件内容
        await SendCommandAsync("DATA");
        response = await ReceiveFirstLineAsync();
        CheckResponse(response);
        await SendCommandAsync(content);
        await SendCommandAsync(".");
        response = await ReceiveFirstLineAsync();
        CheckResponse(response);
    }
}