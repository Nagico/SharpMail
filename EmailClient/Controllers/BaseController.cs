using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EmailClient.Controllers;

public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 登录用户的Id
    /// </summary>
    protected int AccountId => int.Parse(User.Claims.First(i => i.Type == "AccountId").Value);
}