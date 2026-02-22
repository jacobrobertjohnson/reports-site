using PalmzealotReports.Web.Enums;

namespace PalmzealotReports.Web.Config;

public class Column
{
    public ColumnSource Source { get; set; } 
    public string Heading { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string CellClass { get; set; } = "";
}