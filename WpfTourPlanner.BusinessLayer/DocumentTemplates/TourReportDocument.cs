using System.Globalization;
using System.Windows.Documents;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.BusinessLayer.DocumentTemplates
{
    public class TourReportDocument : IDocument
    {
        public Tour TourModelModel { get; }
        public byte[] ImageData { get; }

        public TourReportDocument(Tour tourModel, byte[] imageData)
        {
            TourModelModel = tourModel;
            ImageData = imageData;
        }

        public DocumentMetadata GetMetadata()
        {
            DocumentMetadata dm = DocumentMetadata.Default;
            return dm;
        }

        public void Compose(IContainer container)
        {
            container
                .PaddingHorizontal(16)
                .PaddingVertical(16)
                .Page(page =>
                {
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().PageNumber();
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Text($"Tour: {TourModelModel.Name}", TextStyle.Default.Size(20));
                stack.Spacing(5);
                // stack.Item().Text($"{TourModelModel.Description}");
                // stack.Item().Text($"Length in Km: {TourModelModel.DistanceInKm}");
                // stack.Item().Image(ImageData);
            });
        }


        private void ComposeContent(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Text($"{TourModelModel.Description}");
                stack.Item().Text($"Length in Km: {TourModelModel.DistanceInKm}");
                stack.Item().Image(ImageData);
                stack.Spacing(5);
                stack.Item().Text("Reports:", TextStyle.Default.Size(18));
                stack.Item().Element(ComposeTable);
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.PaddingTop(10).Decoration(decoration =>
            {
                // header
                decoration.Header().BorderBottom(2).BorderColor("CCC").Row(row =>
                {
                    row.RelativeColumn(4).Border(1).BorderColor("CCC").Text("Report");
                    row.RelativeColumn(2).Border(1).BorderColor("CCC").Text("Date Time");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Time in h");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Rating");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Heart rate");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Mean Speed (km/h)");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Temp. in °C");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Breaks");
                    row.RelativeColumn().Border(1).BorderColor("CCC").Text("Steps");
                });

                // content
                decoration
                    .Content()
                    .Stack(stack =>
                    {
                        foreach (TourLog log in TourModelModel.Logs)
                        {
                            stack.Item().BorderBottom(2).BorderColor("CCC").Row(row =>
                            {
                                row.RelativeColumn(4).Border(1).BorderColor("CCC").Text($"{log.Report}");
                                row.RelativeColumn(2).Border(1).BorderColor("CCC").Text($"{log.LogDateTime:d}");
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.TotalTimeInH);
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.Rating);
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.HeartRate);
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.AverageSpeedInKmH);
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.TemperatureInC);
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.Breaks);
                                row.RelativeColumn().Border(1).BorderColor("CCC").Text(log.Steps);
                            });
                        }
                    });
            });
        }
    }
}