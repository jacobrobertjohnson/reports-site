namespace PalmzealotReports.Web.Config;

public class AppSettings
{
    public Auth Auth { get; set;} = new();
    public FormConfig[] Forms { get; set; } = [];

    public FormConfig GetForm(string id)
        => Forms.First(f => f.Id == id);
}