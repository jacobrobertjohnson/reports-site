namespace PalmzealotReports.Web.Config;

public class FormConfig
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string EditLink { get; set; } = string.Empty;
    public string ResponderLink { get; set; } = string.Empty;
    public string[] Header { get; set; } = [];
    public Column[] Columns { get; set; } = [];
}