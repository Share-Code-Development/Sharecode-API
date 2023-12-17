namespace Sharecode.Backend.Utilities.Email;

public class EmailTemplate(string templateHtml, Dictionary<string, string>? placeholders)
{
    public string TemplateHtml { get; private set; } = templateHtml;
    private Dictionary<string, string>? Placeholders { get; set; } = placeholders;

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