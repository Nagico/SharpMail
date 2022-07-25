using EmailClient.Entities;
using EmailClient.Services;
using EmailClient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailClient.Controllers;

[ApiController]
[Route("mails")]
[Authorize]
public class MailController : BaseController
{
    private readonly AccountService _accountService;
    private readonly MailService _mailService;
    
    public MailController(IConfiguration configuration, EmailClientContext context)
    {
        _accountService = new AccountService(configuration, context);
        _mailService = new MailService(configuration, context);
    }
    
    /// <summary>
    /// 获取邮件列表
    /// </summary>
    /// <returns>邮件列表</returns>
    [HttpGet(Name = "GetMailList")]
    public async Task<IActionResult> GetMailList()
    {
        var mails = await _mailService.GetMailList(AccountId);
        return Ok(mails);
    }
    
    /// <summary>
    /// 获取邮件
    /// </summary>
    /// <param name="id">邮件ID</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "GetMailDetail")]
    public async Task<IActionResult> GetMail(int id)
    {
        var mail = await _mailService.GetMailDetail(AccountId, id);
        return Ok(mail);
    }
    
    /// <summary>
    /// 手动已读
    /// </summary>
    /// <param name="id">邮件ID</param>
    /// <returns>结果</returns>
    [HttpPost("{id}/read", Name = "ReadMail")]
    public async Task<IActionResult> ReadMail(int id)
    {
        var res = await _mailService.ReadMail(AccountId, id);
        return Ok(res);
    }
    
    /// <summary>
    /// 删除邮件
    /// </summary>
    /// <param name="id">邮件ID</param>
    /// <returns>204</returns>
    [HttpDelete("{id}", Name = "DeleteMail")]
    public async Task<IActionResult> DeleteMail(int id)
    {
        await _mailService.DeleteMail(AccountId, id);
        return NoContent();
    }
    
    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="mail">邮件内容</param>
    /// <returns>发送结果</returns>
    [HttpPost(Name = "SendMail")]
    public async Task<IActionResult> SendMail([FromBody] MailViewModel mail)
    {
        var result = await _mailService.SendMail(AccountId, mail);
        return Ok(result);
    }
    
    /// <summary>
    /// 拉取邮件
    /// </summary>
    /// <returns></returns>
    [HttpPost("pull", Name = "PullMail")]
    public async Task<IActionResult> PullMail()
    {
        var result = await _mailService.UpdateMailBox(AccountId);
        return Ok(result);
    }


}