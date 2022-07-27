using Newtonsoft.Json.Linq;
using SharpMailBackend.Entities;

namespace SharpMailBackend.Services.Serializers;

public class AccountSerializer : BaseSerializer
{
    public AccountSerializer(EmailClientContext context) : base(context) {}

    public JObject AccountDetail(Account account)
    {
        var res = JObject.FromObject(account);
        
        res.Remove("password");
        res.Remove("mails");
        
        return res;
    }
}