namespace SharpMail.Net;

public class SharpMailNetException : Exception
{
    public SharpMailNetException(string message) : base(message)
    {
    }

    public SharpMailNetException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class SharpMailNetConnectException : SharpMailNetException
{
    public SharpMailNetConnectException(string message) : base(message)
    {
    }

    public SharpMailNetConnectException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class SharpMailNetStateException : SharpMailNetException
{
    public SharpMailNetStateException(string message) : base(message)
    {
    }

    public SharpMailNetStateException(string message, Exception inner) : base(message, inner)
    {
    }
}

public class Pop3Exception : SharpMailNetException
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

public class SmtpException : SharpMailNetException
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