namespace EmailClient.Utils;

public static class MailUtil
{
    public static async Task<Stream> ToStream(string? text)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(text);
        await writer.FlushAsync();
        stream.Position = 0;
        return stream;
    }
    
    public static async Task<string> ToString(Stream stream)
    {
        var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}