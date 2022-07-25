using SharpMail.Entities;

namespace SharpMail.Services.Serializers;

/// <summary>
/// 序列化器基类
/// </summary>
public class BaseSerializer
{
    protected readonly EmailClientContext _context;

    protected BaseSerializer(EmailClientContext context)
    {
        _context = context;
    }
}