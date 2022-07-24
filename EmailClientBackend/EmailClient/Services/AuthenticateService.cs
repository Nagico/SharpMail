using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmailClient.Exceptions;
using Microsoft.IdentityModel.Tokens;
using EmailClient.Entities;
using EmailClient.ViewModels;

namespace EmailClient.Services;

/// <summary>
/// 认证服务
/// </summary>
public class AuthenticateService
{
    private readonly IConfiguration _configuration;
    private readonly EmailClientContext _context;
    private readonly string _salt;
    
    public AuthenticateService(IConfiguration configuration, EmailClientContext EmailClientContext)
    {
        _configuration = configuration;
        _context = EmailClientContext;
        // _salt = _configuration.GetValue<string>("Salt");
    }
    
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <param name="password">登录密码</param>
    /// <returns>token及用户数据</returns>
    /// <exception cref="AppError">登陆失败</exception>
    public AuthenticateViewModel Login(string email, string password)
    {
        // 参数校验
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new AppError("A0210", "请输入邮箱地址和登录密码");
        
        // 获取当前用户名对应的用户
        var account = _context.Accounts.FirstOrDefault(u => u.Email == email);
        
        // 用户不存在
        if (account == null)
        {
            return Register(email, password);
        }

        // 生成token
        var token = GenerateJwt(account);
        
        // 修改用户登录时间，记录密码
        account.LastLoginTime = DateTime.Now;
        account.Password = password;
        _context.SaveChanges();
        
        // 返回结果
        return new AuthenticateViewModel
        {
            Id = account.Id,
            IsActive = account.IsActive,
            Email = email,
            Token = token
        };
    }
    
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <param name="password">登录密码</param>
    /// <returns>token及用户信息</returns>
    /// <exception cref="AppError">注册失败</exception>
    private AuthenticateViewModel Register(string email, string password)
    {
        // 用户名已存在
        if (AccountCount(email) > 0)
            throw new AppError("A0111", "邮箱地址已存在，请直接登录");
        
        // 新用户
        var newAccount = new Account
        {
            Email = email,
            Password = password,
            IsActive = false,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now,
            LastLoginTime = DateTime.Now
        };
        
        // 添加用户
        _context.Accounts.Add(newAccount);
        _context.SaveChanges();
        
        // 生成token
        var token = GenerateJwt(newAccount);
        
        // 返回结果
        return new AuthenticateViewModel
        {
            Id = newAccount.Id,
            IsActive = newAccount.IsActive,
            Email = email,
            Token = token
        };
    }
    
    /// <summary>
    /// 统计邮箱数量
    /// </summary>
    /// <param name="email">邮箱账户</param>
    /// <returns>数量</returns>
    private int AccountCount(string email)
    {
        return _context.Accounts.Count(u => u.Email == email);
    }
    
    /// <summary>
    /// 生成JWT
    /// </summary>
    /// <param name="account">邮箱账户</param>
    /// <returns>token</returns>
    private string GenerateJwt(Account account)
    {
        var claims = new[] {
            // 用户id
            new Claim("UserId", account.Id.ToString()), 
            // 过期时间
            new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddMinutes(Convert.ToInt32(_configuration.GetSection("JWT")["Expires"]))).ToUnixTimeSeconds()}"),
            // 签发时间
            new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
        };
        
        // 加密
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT")["IssuerSigningKey"]));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // 生成token对象
        var securityToken = new JwtSecurityToken(
            issuer: _configuration.GetSection("JWT")["ValidIssuer"],
            audience: _configuration.GetSection("JWT")["ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration.GetSection("JWT")["Expires"])),
            signingCredentials: signingCredentials
        );
        
        // 获取token
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
 
        return token;
    }
}