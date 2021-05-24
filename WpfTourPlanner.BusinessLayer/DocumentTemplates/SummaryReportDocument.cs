using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WpfTourPlanner.Models;

namespace WpfTourPlanner.BusinessLayer.DocumentTemplates
{
    public class SummaryReportDocument : IDocument
    {
        public IList<Tour> ToursModel { get; }

        public SummaryReportDocument(IEnumerable<Tour> toursModel)
        {
            ToursModel = new List<Tour>(toursModel);
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
                stack.Item().AlignCenter().Text($"Summary Report", TextStyle.Default.Size(20));
                stack.Spacing(15);
            });
        }


        private void ComposeContent(IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Spacing(15);
                stack.Item().Text("General Information:", TextStyle.Default.Size(16).Bold());
                stack.Item().Text($"Number of tours: {ToursModel.Count}");
                stack.Item().Text($"Total number of logs: {ToursModel.Sum(t => t.Logs.Count)}");
                stack.Item().Text($"Total length of all tours: {ToursModel.Sum(t => t.DistanceInKm)} km");
                stack.Item()
                    .Text(
                        $"Total length of all tours and logs: {ToursModel.Sum(t => t.DistanceInKm * t.Logs.Count)} km");
                stack.Spacing(15);
                stack.Item().Element(ComposeSummary);
            });
        }

        private void ComposeSummary(IContainer container)
        {
            container.Stack(stack =>
            {
                foreach (Tour tour in ToursModel)
                {
                    stack.Item().Text($"{ToursModel.IndexOf(tour) + 1} {tour.Name}", TextStyle.Default.Size(16).Bold());
                    stack.Item().Row(row =>
                    {
                        row.RelativeColumn(2).Text("Description:", TextStyle.Default.Bold());
                        row.RelativeColumn(6).Text(tour.Description);
                    });
                    stack.Item().Row(row =>
                    {
                        row.RelativeColumn(2).Text("Distance:", TextStyle.Default.Bold());
                        row.RelativeColumn(6).Text($"{tour.DistanceInKm} km");
                    });
                    stack.Item().Row(row =>
                    {
                        row.RelativeColumn(2).Text("Total distance:", TextStyle.Default.Bold());
                        row.RelativeColumn(6).Text($"{tour.DistanceInKm * tour.Logs.Count} km");
                    });
                    stack.Item().Row(row =>
                    {
                        row.RelativeColumn(2).Text("Log Entries:", TextStyle.Default.Bold());
                        row.RelativeColumn(6).Text(tour.Logs.Count);
                    });

                    if (tour.Logs.Count > 0)
                    {
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Total time:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Sum(l => l.TotalTimeInH)} h");
                        });
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Average time:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Sum(l => l.TotalTimeInH)} h");
                        });
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Average temperature:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Average(l => l.TemperatureInC)} °C");
                        });
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Average heart rate:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Average(l => l.HeartRate)} bpm");
                        });
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Average Breaks:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Average(l => l.Breaks)}");
                        });
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Average Steps:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Average(l => l.Steps)}");
                        });
                        stack.Item().Row(row =>
                        {
                            row.RelativeColumn(2).Text("Average Rating:", TextStyle.Default.Bold());
                            row.RelativeColumn(6).Text($"{tour.Logs.Average(l => l.Rating)}");
                        });
                        // stack.Item().Text($"Total time: {tour.Logs.Sum(l => l.TotalTimeInH)} h");
                        // stack.Item().Text($"Average time: {tour.Logs.Sum(l => l.TotalTimeInH)} h");
                        // stack.Item().Text($"Average temperature: {tour.Logs.Average(l => l.TemperatureInC)} °C");
                        // stack.Item().Text($"Average heart rate: {tour.Logs.Average(l => l.HeartRate)} bpm");
                        // stack.Item().Text($"Average Breaks: {tour.Logs.Average(l => l.Breaks)}");
                        // stack.Item().Text($"Average Steps: {tour.Logs.Average(l => l.Steps)}");
                        // stack.Item().Text($"Average Rating: {tour.Logs.Average(l => l.Rating)}");
                    }

                    stack.Item().BorderBottom(2);
                    stack.Spacing(15);
                }
            });
        }
    }
}