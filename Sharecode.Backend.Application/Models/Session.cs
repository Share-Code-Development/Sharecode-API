namespace Sharecode.Backend.Application.Models;

public readonly struct Session
{

    #region Static Variables

    public static Session Create(string serverId)
    {
        return new Session(serverId);
    }

    public static Session Parse(string a, Guid b, long c)
    {
        return new Session(a, b.ToString(), c);
    }

    #endregion
    
    public readonly string ServerId;
    public readonly string SessionId;
    public readonly long SessionTimeStamp;

    private Session(string serverId)
    {
        ServerId = serverId
            .Replace("-", "_")
            .Replace(":", string.Empty)
            .ToLower();
        SessionId = Guid.NewGuid()
            .ToString()
            .Replace("-", string.Empty)
            .ToLower();
        SessionTimeStamp = DateTime.UtcNow.Ticks;
    }

    private Session(string serverId, string sessionId, long sessionTimeStamp)
    {
        ServerId = serverId.Replace("-", "_").ToLower();
        SessionId = sessionId.ToString().Replace("-", string.Empty).ToLower();
        SessionTimeStamp = sessionTimeStamp;
    }

    public override string ToString()
    {
        return $"{ServerId}-{SessionId}-{SessionTimeStamp}";
    }
    
    public override bool Equals(object obj)
    {
        if (!(obj is Session))
            return false;

        var other = (Session)obj;

        return ServerId == other.ServerId &&
               SessionId == other.SessionId &&
               SessionTimeStamp == other.SessionTimeStamp;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ServerId, SessionId, SessionTimeStamp);
    }

    public static bool operator ==(Session left, Session right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Session left, Session right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(string left, Session right)
    {
        return left == right.ToString();
    }

    public static bool operator !=(string left, Session right)
    {
        return left != right.ToString();
    }
}