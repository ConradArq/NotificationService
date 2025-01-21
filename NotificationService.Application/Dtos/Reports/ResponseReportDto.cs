namespace NotificationService.Application.Dtos.Reports
{
    public class ResponseReportDto
    {
        public string ContentHtml { get; set; } = string.Empty;
        public string ContentPdfInBase64 { get; set; } = string.Empty;
    }
}
