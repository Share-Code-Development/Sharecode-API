namespace Sharecode.Backend.Utilities.Email;

public class EmailTemplateKey
{
    public EmailTemplateKey(string key, string subject)
    {
        Key = key;
        Subject = subject;
    }

    public readonly string Key;
    public readonly string Subject;

    public async Task<string> GetOrFetchAsync(string baseUrl)
    {
        string filePath = Path.Combine(baseUrl, Key);
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }
        else
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}

public static class EmailTemplateKeys
{
    public static readonly EmailTemplateKey EmailValidation = new EmailTemplateKey("validate-email.html", "Welcome to Sharecode! Verify your email address");
    public static readonly EmailTemplateKey WelcomeUser = new EmailTemplateKey("welcome-email.html", "Welcome onboard {(WELCOME_USER)}");
    public static readonly EmailTemplateKey ResetPassword = new EmailTemplateKey("reset-password.html", "Forgot your password? Here is your reset request {(USER)}");
    public static readonly EmailTemplateKey AccountLocked = new EmailTemplateKey("account-locked.html", "Your sharecode account has been locked!");
}