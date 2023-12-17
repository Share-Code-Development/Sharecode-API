using System.Text;
using Serilog;

namespace Sharecode.Backend.Utilities.Email;

public class EmailTemplateKey(string key, string subject)
{
    private static readonly ILogger Logger = Log.ForContext(typeof(EmailTemplateKey));
    private const string GithubUrl = "https://raw.githubusercontent.com/Share-Code-Development/Email-templates/main/";
    public readonly string Subject = subject;
    public readonly string Key = key;

    public async Task<string> GetOrFetchAsync(string baseUrl)
    {
        string filePath = Path.Combine(baseUrl, key);
        if (!File.Exists(filePath))
        {
            var htmlTemplate = await GetAndSave(filePath, key);
            return htmlTemplate ?? string.Empty;
        }
        else
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }

    private async Task<string?> GetAndSave(string filePathToSave, string key)
    {
        try
        {
            HttpClient client = new HttpClient();
            var htmlContentAsync = await client.GetStringAsync($"{GithubUrl}/{key}");
            await File.WriteAllTextAsync(filePathToSave, htmlContentAsync, Encoding.UTF8);
            return htmlContentAsync;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to get the template of {TemplateKey} from Github.", key);
            return null;
        }
    }
    
    
}

public static class EmailTemplateKeys
{
    public static readonly EmailTemplateKey EmailValidation = new("validate-email.html", "Welcome to Sharecode! Verify your email address");
    public static readonly EmailTemplateKey WelcomeUser = new("welcome-email.html", "Welcome onboard {(USER)}");
    public static readonly EmailTemplateKey ResetPassword = new("reset-password.html", "Forgot your password? Here is your reset request {(USER)}");
    public static readonly EmailTemplateKey AccountLocked = new("account-locked.html", "Your sharecode account has been locked!");
}