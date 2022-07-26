using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpMail.Entities;
using SharpMail.Services;
using SharpMail.ViewModels;

namespace SharpMail.Controllers;

[ApiController]
[Route("accounts")]
[Authorize]
public class AccountController : BaseController
{
    private readonly AccountService _accountService;
    
    public AccountController(IConfiguration configuration, EmailClientContext context)
    {
        _accountService = new AccountService(configuration, context);
    }
    
    /// <summary>
    /// 获取当前用户详情
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet(Name = "GetAccount")]
    public async Task<ActionResult> GetAccount()
    {
        var account = await _accountService.GetAccountDetail(AccountId);
        return Ok(account);
    }

    /// <summary>
    /// 更新当前用户信息
    /// </summary>
    /// <param name="account">待更新信息</param>
    /// <returns></returns>
    [HttpPut(Name = "UpdateAccount")]
    public async Task<ActionResult> UpdateAccount(UpdateAccountViewModel account)
    {
        var newAccount = await _accountService.UpdateAccount(AccountId, account.Password, 
            account.SmtpHost, account.SmtpPort, account.SmtpSsl, 
            account.Pop3Host, account.Pop3Port, account.Pop3Ssl);
        return Ok(newAccount);
    }
}