namespace EmailClient.Entities;

/// <summary>
/// Email账户
/// </summary>
public class Account
{
    /// <summary>
    /// 账户ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// email账户名
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// smtp服务器地址
    /// </summary>
    public string? SmtpHost { get; set; }
    
    /// <summary>
    /// smtp服务器端口
    /// </summary>
    public int? SmtpPort { get; set; }
    
    /// <summary>
    /// smtp服务器SSL
    /// </summary>
    public bool? SmtpSsl { get; set; }
    
    /// <summary>
    /// pop3服务器地址
    /// </summary>
    public string? Pop3Host { get; set; }
    
    /// <summary>
    /// pop3服务器SSL
    /// </summary>
    public bool? Pop3Ssl { get; set; }
    
    /// <summary>
    /// pop3服务器端口
    /// </summary>
    public int? Pop3Port { get; set; }
    
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// 邮件列表
    /// </summary>
    public List<Mail>? Mails { get; set; }
    
    /// <summary>
    /// 注册时间
    /// </summary>
    public DateTime CreateTime { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
    
    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LastLoginTime { get; set; }
}