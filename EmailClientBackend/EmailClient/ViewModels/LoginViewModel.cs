using System.ComponentModel.DataAnnotations;

namespace EmailClient.ViewModels;

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