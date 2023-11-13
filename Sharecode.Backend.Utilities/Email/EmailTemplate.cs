namespace Sharecode.Backend.Utilities.Email;

public class EmailTemplate
{
    public EmailTemplate(string templateHtml, Dictionary<string, string>? placeholders)
    {
        TemplateHtml = templateHtml;
        Placeholders = placeholders;
    }

    public string TemplateHtml { get; private set; }
    public Dictionary<string, string>? Placeholders { get; private set; }
    public void ParseAsync()
    {
        if (Placeholders != null)
        {
            foreach (var placeholderKeyValue in Placeholders)
            {
                TemplateHtml = TemplateHtml.Replace("{("+placeholderKeyValue.Key+")}", placeholderKeyValue.Value);
            }
        }

        
    }
}