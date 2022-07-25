using EmailClient.Entities;
using EmailClient.Exceptions;
using EmailClient.Services.Serializers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace EmailClient.Services;

public class AccountService
{
    private readonly IConfiguration _configuration;
    private readonly EmailClientContext _context;
    private readonly AccountSerializer _accountSerializer;
    
    public AccountService(IConfiguration configuration, EmailClientContext context)
    {
        _configuration = configuration;
        _context = context;
        _accountSerializer = new AccountSerializer(_context);
    }
    
    /// <summary>
    /// 获取账户信息
    /// </summary>
    /// <param name="id">账户id</param>
    /// <returns>account 对象</returns>
    /// <exception cref="AppError">账户id不存在</exception>
    public async Task<Account> GetAccount(int id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if (account == null)
            throw new AppError("A0514");
        
        return account;
    }
    
    /// <summary>
    /// 获取账户信息
    /// </summary>
    /// <param name="id">账户ID</param>
    /// <returns>账户信息JSON对象</returns>
    public async Task<JObject> GetAccountDetail(int id)
    {
        var account = await GetAccount(id);
        return _accountSerializer.AccountDetail(account);
    }
    
    
    /// <summary>
    /// 更新账户信息
    /// </summary>
    /// <param name="id">待更新账户ID</param>
    /// <param name="SmtpHost">smtp服务器地址</param>
    /// <param name="SmtpPort">smtp服务器端口</param>
    /// <param name="SmtpSsl">smtp启用ssl</param>
    /// <param name="Pop3Host">pop3服务器地址</param>
    /// <param name="Pop3Port">pop3服务器端口</param>
    /// <param name="Pop3Ssl">pop3启用ssl</param>
    /// <returns>账户信息JSON对象</returns>
    /// <exception cref="AppError">未激活且参数不全</exception>
    public async Task<JObject> UpdateAccount(int id,
                                             string? SmtpHost,
                                             int? SmtpPort,
                                             bool? SmtpSsl,
                                             string? Pop3Host,
                                             int? Pop3Port,
                                             bool? Pop3Ssl)
    {
        var account = await GetAccount(id);
        
        // 检查激活
        if (!account.IsActive)
        {
            // 参数不全
            if (string.IsNullOrWhiteSpace(SmtpHost) ||
                !SmtpPort.HasValue ||
                !SmtpSsl.HasValue ||
                string.IsNullOrWhiteSpace(Pop3Host) ||
                !Pop3Port.HasValue ||
                !Pop3Ssl.HasValue)
                throw new AppError("A0420", "您填写的参数不全");
            
            // 激活账户
            account.IsActive = true;
        }
        
        if (SmtpHost != null)
            account.SmtpHost = SmtpHost;
        if (SmtpPort != null)
            account.SmtpPort = SmtpPort.Value;
        if (SmtpSsl != null)
            account.SmtpSsl = SmtpSsl.Value;
        if (Pop3Host != null)
            account.Pop3Host = Pop3Host;
        if (Pop3Port != null)
            account.Pop3Port = Pop3Port.Value;
        if (Pop3Ssl != null)
            account.Pop3Ssl = Pop3Ssl.Value;
        
        account.UpdateTime = DateTime.Now;
        
        await _context.SaveChangesAsync();
        
        return _accountSerializer.AccountDetail(account);
    }
}