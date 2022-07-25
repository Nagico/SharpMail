using System.Net.Sockets;
using System.Text;

namespace EmailNet;

public class SmtpClient : BaseClient
{
    public SmtpClient(string email, string password, ServerUrl smtpServer) : base(email, password, smtpServer)
    {
    }
    
    protected override void CheckResponse(string response, string msg="连接失败", ClientState next=ClientState.Unconnected)
    {
        // TODO: check response
        return;
        
        State = next;
        throw new SmtpConnectException(msg);
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
        SendCommand("HELO " + server.Host);
        var response = ReceiveFirstLine();
        CheckResponse(response, "服务器验证失败");
        
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
        
        //验证成功，进入连接状态
        State = ClientState.Connected;
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
        await SendCommandAsync("HELO " + server.Host);
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
}