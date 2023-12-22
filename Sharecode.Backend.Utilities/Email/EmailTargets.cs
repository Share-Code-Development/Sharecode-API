namespace Sharecode.Backend.Utilities.Email;

public class EmailTargets
{
    private readonly Dictionary<string, string> _userNameEmailTargets = new();
    public IReadOnlyDictionary<string, string> Targets => _userNameEmailTargets;
    public EmailTargets()
    {
    }

    public EmailTargets AddTarget(string email, string? name = null)
    {
        if (name == null)
            name = email;

        _userNameEmailTargets[email] = name;
        return this;
    }
}