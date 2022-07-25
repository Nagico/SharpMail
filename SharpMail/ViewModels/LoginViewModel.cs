using System.ComponentModel.DataAnnotations;

namespace SharpMail.ViewModels;

/// <summary>
/// 登录数据
/// </summary>
public class LoginViewModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}