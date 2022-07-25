using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EmailNet;


public class Pop3Client : BaseClient
{
    public Pop3Client(string email, string password, ServerUrl pop3Server) : base(email, password, pop3Server)
    {
    }

    protected override void CheckResponse(string response, string msg="连接失败", ClientState next=ClientState.Unconnected)
    {
        if (response.IndexOf("+OK", StringComparison.Ordinal) == 0) return;
        
        State = next;
        throw new Pop3ConnectException(msg);
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <returns>返回是否登录成功</returns>
    public override void Connect()
    {
        // 如果已经连接，则不再连接
        if (State == ClientState.Connected) return;
    
        //验证用户名
        SendCommand("USER " + email);
        var response = ReceiveFirstLine();
        CheckResponse(response, "用户名验证失败");
        
        //验证密码
        SendCommand("PASS " + password);
        response = ReceiveFirstLine();
        CheckResponse(response, "密码验证失败");
        
        //验证成功，进入连接状态
        State = ClientState.Connected;
    }
    
    /// <summary>
    /// 获取邮件数量
    /// </summary>
    public int GetMailCount()
    {
        CheckState();
        
        SendCommand("STAT");
        var response = ReceiveFirstLine();
        CheckResponse(response, "获取邮件数量失败", State);
        
        var parts = response.Split(' ');
        return int.Parse(parts[1]);
    }
    
    /// <summary>
    /// 根据邮件id获取邮件
    /// </summary>
    /// <param name="emailId">邮件id</param>
    /// <returns>邮件内容</returns>
    public string GetMailContent(int emailId)
    {
        CheckState();
        
        SendCommand("RETR " + emailId);
        var response = ReceiveFirstLine();
        CheckResponse(response, "获取邮件内容失败", State);
        
        response = ReceiveResponse();
        return response;
    }
    
    /// <summary>
    /// 获取邮件uid
    /// </summary>
    /// <param name="emailId">邮件id</param>
    /// <returns>uid</returns>
    public string GetMaidUid(int emailId)
    {
        CheckState();
        
        SendCommand("UIDL " + emailId);
        var response = ReceiveFirstLine();
        CheckResponse(response, "获取邮件uid失败", State);
        
        var parts = response.Split(' ');
        return parts[2];
    }
    
    /// <summary>
    /// 查询邮件ID
    /// </summary>
    private int FindEmailId(string uid)
    {
        CheckState();
        
        SendCommand("UIDL");
        var response = ReceiveResponse();
        CheckResponse(response, "获取邮件id失败", State);
        
        var lines = response.Split('\n');
        foreach (var line in lines)
        {
            if (!line.StartsWith(uid)) continue;
            var parts = line.Split(' ');
            return int.Parse(parts[1]);
        }
        
        return -1;
    }
    
    public void DeleteMail(string uid)
    {
        CheckState();
        
        var emailId = FindEmailId(uid);
        
        if (emailId == -1) throw new Pop3DeleteException("未找到邮件");
        
        SendCommand("DELE " + uid);
        
        var response = ReceiveResponse();
        CheckResponse(response, "删除邮件失败", State);
    }

    /// <summary>
    /// 执行空操作
    /// </summary>
    public void Noop()
    {
        if (State == ClientState.Connected)
        {
            SendCommand("NOOP");
        }
    }
    
    public void Reset()
    {
        if (State == ClientState.Connected)
        {
            SendCommand("RSET");
        }
    }
}
