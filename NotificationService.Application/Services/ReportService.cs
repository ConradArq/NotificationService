using NotificationService.Application.Dtos;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Application.Interfaces.Factories;
using iText.Html2pdf;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Properties;
using NotificationService.Application.Dtos.Reports;
using Document = iText.Layout.Document;

namespace NotificationService.Application.Services
{
    public class ReportService<TModel> : IReportService<TModel>
    {
        private readonly IReportHandlerFactory _reportHandlerFactory;

        public ReportService(IReportHandlerFactory reportHandlerFactory)
        {
            _reportHandlerFactory = reportHandlerFactory;
        }

        public async Task<ResponseDto<ResponseReportDto>> GenerateReportAsync()
        {
            var handler = _reportHandlerFactory.GetHandler<TModel>();
            var templateHtml = await handler.HandleAsync();
            var templatePdfInBase64 = HtmlToPdfInBase64(templateHtml);

            ResponseReportDto responseReportDto = new()
            {
                ContentHtml = templateHtml,
                ContentPdfInBase64 = templatePdfInBase64
            };

            var response = new ResponseDto<ResponseReportDto>(responseReportDto);
            return response;
        }

        private string HtmlToPdfInBase64(string html)
        {
            byte[] pdfBytes;

            // Step 1: Convert HTML to PDF
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
                    document.SetMargins(10, 20, 30, 20);

                    ConverterProperties properties = new ConverterProperties();
                    HtmlConverter.ConvertToPdf(html, pdf, properties);

                    // Ensure all content is written to memoryStream
                    pdf.Close();
                }

                pdfBytes = memoryStream.ToArray();
            }

            // Step 2: Add Footer to PDF
            byte[] outputPdfBytes;

            using (var inputPdfStream = new MemoryStream(pdfBytes))
            using (var outputPdfStream = new MemoryStream())
            {
                using (var reader = new PdfReader(inputPdfStream))
                using (var writer = new PdfWriter(outputPdfStream))
                using (var pdf = new PdfDocument(reader, writer))
                {
                    for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
                    {
                        var page = pdf.GetPage(i);
                        var pageSize = page.GetPageSize();

                        // Use PdfCanvas to draw footer line
                        PdfCanvas pdfCanvas = new PdfCanvas(page);
                        pdfCanvas.SetLineWidth(0.8f);
                        pdfCanvas.MoveTo(20, 30)
                                 .LineTo(pageSize.GetWidth() - 20, 30)
                                 .Stroke();

                        // Add footer text
                        using (var canvas = new Canvas(page, pageSize))
                        {
                            canvas.SetFontSize(10);
                            canvas.ShowTextAligned($"Organization Name © {DateTime.Now.Year}",
                                pageSize.GetWidth() / 2, 45, TextAlignment.CENTER, VerticalAlignment.TOP, 0);

                            canvas.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
                            canvas.SetFontSize(8);
                            canvas.ShowTextAligned($"Page {i} of {pdf.GetNumberOfPages()}",
                                pageSize.GetWidth() - 20, 25, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
                        }
                    }

                    pdf.Close();
                }

                outputPdfBytes = outputPdfStream.ToArray();
            }

            return Convert.ToBase64String(outputPdfBytes);
        }
    }
}
