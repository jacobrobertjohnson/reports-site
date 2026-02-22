using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Forms.v1;
using Google.Apis.Forms.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PalmzealotReports.Web.Config;
using PalmzealotReports.Web.Enums;
using PalmzealotReports.Web.Models;

namespace PalmzealotReports.Web.Controllers;

public class PrintController(
    AppSettings _appSettings,
    IGoogleAuthProvider _googleAuth
) : Controller
{
    public async Task<IActionResult> Index(string id, CancellationToken ct)
    {
        FormConfig form = _appSettings.GetForm(id);
        FormsResource forms = await GetFormsResourceAsync(ct);
        string?[] questionIds = await GetQuestionIds(forms, form, ct);
        ListFormResponsesResponse answers = await forms.Responses.List(id).ExecuteAsync(ct);

        List<string[]> rows = [];

        foreach (FormResponse? answer in answers.Responses)
        {
            List<string> row = [];

            for (int i = 0; i < questionIds.Length; i++)
            {
                if (answer is null)
                    continue;

                if (form.Columns[i].Source == ColumnSource.Hardcoded)
                {
                    row.Add(form.Columns[i].Value);
                    continue;
                }

                if (!answer.Answers.Any(a => a.Value?.QuestionId == questionIds[i]))
                {
                    row.Add("");
                    continue;
                }

                Answer question = answer.Answers.First(a => a.Value.QuestionId == questionIds[i]).Value;

                row.Add(question?.TextAnswers?.Answers?.FirstOrDefault()?.Value ?? "");
            }

            rows.Add(row.ToArray());
        }

        return View(new FormPrintViewModel()
        {
            Header = string.Join("<br />", form.Header)
                .Replace("{year}", $"{DateTime.Now:yyyy}")
                .Replace("{monthName}", $"{DateTime.Now:MMMM}")
                .Replace("{weekday}", $"{DateTime.Now:dddd}")
                .Replace("{dayOfMonth}", $"{DateTime.Now.Day}"),
            
            Filename = form.Filename.Replace("{datestampYYYY-MM-DD}", $"{DateTime.Now:yyyy-MM-dd}"),

            Classes = [..form.Columns.Select(c => c.CellClass)],
            Columns = [..form.Columns.Select(c => c.Heading)],
            Rows = rows.ToArray(),
        });
    }

    private async Task<FormsResource> GetFormsResourceAsync(CancellationToken ct)
    {
        GoogleCredential credential = await _googleAuth.GetCredentialAsync(null, ct);

        FormsService service = new(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "PalmzealotReports.Web"
        });
        
        return service.Forms;
    }

    private async Task<string?[]> GetQuestionIds(FormsResource forms, FormConfig form, CancellationToken ct)
    {
        Form questions = await forms.Get(form.Id).ExecuteAsync(ct);

        return form.Columns
            .Select(c => 
            {
                var headings = questions
                    .Items
                    .FirstOrDefault(i => i.Title == c.Value);

                return headings?
                    .QuestionItem?
                    .Question?
                    .QuestionId;
            })
            .ToArray();
    }
}