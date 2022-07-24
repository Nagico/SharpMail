using EmailClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmailClient.Entities;
using EmailClient.ViewModels;

namespace EmailClient.Controllers;

[ApiController]
[Route("tokens")]
public class AuthenticateController : BaseController
{
    private readonly AuthenticateService _service;
    
    public AuthenticateController(IConfiguration configuration, EmailClientContext EmailClientContext)
    {
        _service = new AuthenticateService(configuration, EmailClientContext);
    }
    
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginViewModel">登录信息</param>
    /// <returns>token及用户信息</returns>
    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    public ActionResult<AuthenticateViewModel> Login([FromBody] LoginViewModel loginViewModel)
    {
        var result = _service.Login(loginViewModel.Email, loginViewModel.Password);
        return result;
    }
}