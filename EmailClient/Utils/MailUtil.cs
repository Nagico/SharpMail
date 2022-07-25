using System.Text;
using System.Text.RegularExpressions;

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

    public static string HtmlToText(string htmlCode)
    {
        // Remove new lines since they are not visible in HTML
        htmlCode = htmlCode.Replace("\n", " ");

        // Remove tab spaces
        htmlCode = htmlCode.Replace("\t", " ");

        // Remove multiple white spaces from HTML
        htmlCode = Regex.Replace(htmlCode, "\\s+", " ");

        // Remove HEAD tag
        htmlCode = Regex.Replace(htmlCode, "<head.*?</head>", ""
            , RegexOptions.IgnoreCase | RegexOptions.Singleline);
    
        // Remove style tag
        htmlCode = Regex.Replace(htmlCode, "<style.*?</style>", ""
            , RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Remove any JavaScript
        htmlCode = Regex.Replace(htmlCode, "<script.*?</script>", ""
            , RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Replace special characters like &, <, >, " etc.
        var sbHTML = new StringBuilder(htmlCode);
        // Note: There are many more special characters, these are just
        // most common. You can add new characters in this arrays if needed
        string[] oldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;",
            "&gt;", "&reg;", "&copy;", "&bull;", "&trade;","&#39;"};
        string[] newWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢","\'" };
        for (var i = 0; i < oldWords.Length; i++)
        {
            sbHTML.Replace(oldWords[i], newWords[i]);
        }

        // Check if there are line breaks (<br>) or paragraph (<p>)
        sbHTML.Replace("<br>", "\n<br>");
        sbHTML.Replace("<br ", "\n<br ");
        sbHTML.Replace("<p ", "\n<p ");

        // Finally, remove all HTML tags and return plain text
        var res = Regex.Replace(
            sbHTML.ToString(), "<[^>]*>", "");

        res = Regex.Replace(res, @"[ ]{1,}", " ").Trim();
        res = Regex.Replace(res, @"\n{1,}", "\n").Trim('\n');
    
        return res;
    }
}