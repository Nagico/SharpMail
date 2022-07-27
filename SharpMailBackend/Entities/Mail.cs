namespace SharpMailBackend.Entities;

public class Mail
{
    /// <summary>
    /// 邮件ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 账户ID
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// 账户
    /// </summary>
    public Account? Account { get; set; }
    
    /// <summary>
    /// 邮件Uid
    /// </summary>
    public string Uid { get; set; }

    /// <summary>
    /// 邮件类型
    /// </summary>
    public int Type { get; set; }
    
    /// <summary>
    /// 已读
    /// </summary>
    public bool Read { get; set; }
    
    /// <summary>
    /// 邮件主题
    /// </summary>
    public string? Subject { get; set; }
    
    /// <summary>
    /// 发件人
    /// </summary>
    public string? From { get; set; }
    
    /// <summary>
    /// 收件人
    /// </summary>
    public string? To { get; set; }
    
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// 邮件正文文本
    /// </summary>
    public string? Text { get; set; }
    
    /// <summary>
    /// 邮件内容
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// 添加时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}