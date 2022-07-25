﻿using System.Net.Sockets;
using System.Text;

namespace EmailNet;

/// <summary>
/// 会话连接状态
/// </summary>
public enum ClientState
{
    Connected,    //连接登录成功
    Unconnected  //未连接
}

public abstract class BaseClient
{
    protected readonly string email;
    protected readonly string password;
    protected readonly Uri server;
    
    protected readonly TcpClient _client;   //客户端
    protected readonly NetworkStream _streamWriter;  //写,发送命令
    protected readonly StreamReader _streamReader;   //读
    public ClientState State { get; set; } //当前连接状态

    protected BaseClient(string email, string password, Uri server)
    {
        this.email = email;
        this.password = password;
        this.server = server;
        
        try
        {
            _client = new TcpClient(server.Host, server.Port);
            _streamWriter = _client.GetStream();
            _streamReader = new StreamReader(_client.GetStream());
            //连接返回的返回结果
            var response = ReceiveFirstLine();
            CheckResponse(response);
        }
        catch (Exception e)
        {
            State = ClientState.Unconnected;
            throw new EmailNetConnectException("创建连接失败", e);
        }

        State = ClientState.Unconnected;
    }
    
    public abstract void Connect();

    public void Disconnect()
    {
        CheckState();
        
        SendCommand("QUIT");
        var response = ReceiveFirstLine();
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
    /// 检查返回结果
    /// </summary>
    /// <param name="response">返回结果</param>
    /// <param name="msg">失败时异常的msg</param>
    /// <param name="next">下一个状态</param>
    protected abstract void CheckResponse(string response, string msg="连接失败", ClientState next=ClientState.Unconnected);
    
    /// <summary>
    /// 检查连接状态
    /// </summary>
    /// <exception cref="EmailNetStateException">未连接</exception>
    protected void CheckState()
    {
        if (State != ClientState.Connected) throw new EmailNetStateException("未连接");
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
}