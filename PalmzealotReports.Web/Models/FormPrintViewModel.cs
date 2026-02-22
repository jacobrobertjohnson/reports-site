namespace PalmzealotReports.Web.Models;

public class FormPrintViewModel
{
    public string Header { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string[] Classes { get; set; } = [];
    public string[] Columns { get; set; } = [];
    public string[][] Rows { get; set; } = [];
}