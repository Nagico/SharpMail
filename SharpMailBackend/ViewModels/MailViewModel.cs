namespace SharpMailBackend.ViewModels;

public class MailViewModel
{
    /// <summary>
    /// 收件人列表
    /// </summary>
    public List<string> To { get; set; }
    /// <summary>
    /// 抄送列表
    /// </summary>
    public List<string>? Cc { get; set; }
    /// <summary>
    /// 密送列表
    /// </summary>
    public List<string>? Bcc { get; set; }
    /// <summary>
    /// 主题
    /// </summary>
    public string Subject { get; set; }
    /// <summary>
    /// Html内容
    /// </summary>
    public string HtmlBody { get; set; }
}