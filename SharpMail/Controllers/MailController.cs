using SharpMail.Entities;
using SharpMail.Services;
using SharpMail.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SharpMail.Controllers;

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
    /// <param name="type">筛选类型 1-收件 2-发件</param>
    /// <param name="read">筛选是否已读</param>
    /// <param name="orderBy">排序条件</param>
    /// <param name="page">页数=1</param>
    /// <param name="pageSize">分页=10</param>
    /// <returns></returns>
    [HttpGet(Name = "GetMailList")]
    public async Task<IActionResult> GetMailList(int type, bool? read, string? orderBy, int? page, int? pageSize)
    {
        var pageR = page ?? 1;
        var pageSizeR = pageSize ?? 10;
        var (total, mails) = await _mailService.GetMailList(AccountId, type, read, orderBy, pageR, pageSizeR);
        var totalPage = total / pageSizeR;
        if (total % pageSizeR != 0)
        {
            totalPage++;
        }
        
        var result = new
        {
            total,
            totalPage,
            page = pageR,
            pageSize = pageSizeR,
            unread = await _mailService.GetUnreadMailCount(AccountId),
            data = mails
        };
        
        return Ok(result);
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