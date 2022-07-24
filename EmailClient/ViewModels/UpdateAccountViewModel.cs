namespace EmailClient.ViewModels;

public class UpdateAccountViewModel
{
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
}