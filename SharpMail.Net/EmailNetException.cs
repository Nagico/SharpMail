namespace SharpMail.Net;

public class EmailNetException : Exception
{
    public EmailNetException(string message) : base(message)
    {
    }

    public EmailNetException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class EmailNetConnectException : EmailNetException
{
    public EmailNetConnectException(string message) : base(message)
    {
    }

    public EmailNetConnectException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class EmailNetStateException : EmailNetException
{
    public EmailNetStateException(string message) : base(message)
    {
    }

    public EmailNetStateException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class Pop3Exception : EmailNetException
{
    public Pop3Exception(string message) : base(message)
    {
    }

    public Pop3Exception(string message, Exception inner) : base(message, inner)
    {
    }
}

public class Pop3DeleteException : Pop3Exception
{
    public Pop3DeleteException(string message) : base(message)
    {
    }

    public Pop3DeleteException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class Pop3ConnectException : Pop3Exception
{
    public Pop3ConnectException(string message) : base(message)
    {
    }

    public Pop3ConnectException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class SmtpException : EmailNetException
{
    public SmtpException(string message) : base(message)
    {
    }

    public SmtpException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class SmtpConnectException : SmtpException
{
    public SmtpConnectException(string message) : base(message)
    {
    }

    public SmtpConnectException(string message, Exception inner) : base(message, inner)
    {
    }
}