namespace PalmzealotReports.Web.Models;

public class FormListViewModel
{
    public FormListItemViewModel[] Forms { get; set; } = [];
}

public class FormListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EditLink { get; set; } = string.Empty;
    public string ResponderLink { get; set; } = string.Empty;
}