﻿namespace SharpMail.ViewModels;

/// <summary>
/// 登录返回数据
/// </summary>
public class AuthenticateViewModel
{
    public int Id { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public string Token { get; set; }
}