using System.Net.Sockets;
using System.Text;
using System;
using System.Net.Security;

namespace SharpMail.Net;

/// <summary>
/// 会话连接状态
/// </summary>
public enum ClientState
{
    Connected,    //连接登录成功
    Unconnected,  //未连接
}

public class ServerUrl
{
    public string Host { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    
    public ServerUrl(string host, int port, bool useSsl = false)
    {
        Host = host;
        Port = port;
        UseSsl = useSsl;
    }
}

public abstract class BaseClient
{
    protected readonly string email;
    protected readonly string password;
    protected readonly ServerUrl server;
    
    protected readonly TcpClient _client;   //客户端
    protected readonly Stream _streamWriter;  //写,发送命令
    protected readonly StreamReader _streamReader;   //读
    public ClientState State { get; set; } //当前连接状态
    
    private static int TIMEOUT = 2000; //超时时间

    protected BaseClient(string email, string password, ServerUrl server)
    {
        this.email = email;
        this.password = password;
        this.server = server;
        
        try
        {
            if (!server.UseSsl)
            {
                _client = new TcpClient(server.Host, server.Port);
                _streamWriter = _client.GetStream();
                _streamReader = new StreamReader(_client.GetStream());
                _streamReader.BaseStream.ReadTimeout = TIMEOUT;
            }
            else
            {
                _client = new TcpClient(server.Host, server.Port);
                
                var sslStream = new SslStream(_client.GetStream(), false);
                sslStream.AuthenticateAsClient(server.Host);
                
                _streamWriter = sslStream;
                
                _streamReader = new StreamReader(sslStream);
                _streamReader.BaseStream.ReadTimeout = TIMEOUT;
            }
        }
        catch (Exception e)
        {
            State = ClientState.Unconnected;
            throw new SharpMailNetConnectException("创建连接失败", e);
        }

        State = ClientState.Unconnected;
    }

    public void Connect()
    {
        //连接返回的返回结果
        var response = ReceiveFirstLine();
        CheckResponse(response);
    }
    
    public async Task ConnectAsync()
    {
        //连接返回的返回结果
        var response = await ReceiveFirstLineAsync();
        CheckResponse(response);
    }

    public void Disconnect()
    {
        CheckState();
        
        SendCommand("QUIT");
        var response = ReceiveFirstLine();
        _streamReader.Close();
        _client.Close();
        State = ClientState.Unconnected;
    }
    
    public async Task DisconnectAsync()
    {
        CheckState();
        
        await SendCommandAsync("QUIT");
        var response = await ReceiveFirstLineAsync();
        _streamReader.Close();
        _client.Close();
        State = ClientState.Unconnected;
    }
    
    /// <summary>
    /// 向服务器发送命令
    /// </summary>
    /// <param name="command">命令</param>
    protected void SendCommand(string command)
    {
        var cmdData = command + "\r\n";
        var arrayToSend = Encoding.ASCII.GetBytes(cmdData.ToCharArray());
        
        _streamWriter.Write(arrayToSend, 0, arrayToSend.Length);
    }
    
    /// <summary>
    /// 向服务器发送命令
    /// </summary>
    /// <param name="command">命令</param>
    protected async Task SendCommandAsync(string command)
    {
        var cmdData = command + "\r\n";
        var arrayToSend = Encoding.ASCII.GetBytes(cmdData.ToCharArray());
        
        await _streamWriter.WriteAsync(arrayToSend);
    }
    
    /// <summary>
    /// 检查返回结果
    /// </summary>
    /// <param name="response">返回结果</param>
    /// <param name="msg">失败时异常的msg</param>
    /// <param name="next">下一个状态</param>
    protected abstract void CheckResponse(string response, string msg="连接失败", ClientState next=ClientState.Unconnected);
    
    /// <summary>
    /// 检查连接状态
    /// </summary>
    /// <exception cref="SharpMailNetStateException">未连接</exception>
    protected void CheckState()
    {
        if (State != ClientState.Connected) throw new SharpMailNetStateException("未连接");
    }

    /// <summary>
    /// 从返回回来的流中读取第一行数据
    /// </summary>
    /// <returns>第一行数据</returns>
    protected string ReceiveFirstLine()
    {
        var line = _streamReader.ReadLine();
        return line ?? string.Empty;
    }
    
    /// <summary>
    /// 从返回回来的流中读取第一行数据
    /// </summary>
    /// <returns>第一行数据</returns>
    protected async Task<string> ReceiveFirstLineAsync()
    {
        // var task = _streamReader.ReadLineAsync();
        // var res = task.Wait(TIMEOUT);
        //
        // if (!res)
        //     return string.Empty;
        //
        // return task.Result ?? string.Empty;
        return ReceiveFirstLine();
    }
    
    /// <summary>
    /// 从返回回来的流中获取文本信息
    /// </summary>
    /// <returns>返回最后的文本信息</returns>
    protected string ReceiveResponse()
    {
        var response = new StringBuilder();
        var temp = _streamReader.ReadLine() + "\n";
        response.Append(temp);
        while (temp != null && temp != ".")
        {
            temp = _streamReader.ReadLine();
            response.Append(temp + "\n");
        }
        
        return response.ToString();
    }
    
    /// <summary>
    /// 从返回回来的流中获取文本信息
    /// </summary>
    /// <returns>返回最后的文本信息</returns>
    protected async Task<string> ReceiveResponseAsync()
    {
        var response = new StringBuilder();
        var temp = await _streamReader.ReadLineAsync() + "\n";
        response.Append(temp);
        while (temp != null && temp != ".")
        {
            temp = await _streamReader.ReadLineAsync();
            response.Append(temp + "\n");
        }
        
        return response.ToString();
    }
}