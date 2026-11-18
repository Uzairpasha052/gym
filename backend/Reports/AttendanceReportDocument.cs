using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using backend.DTO.Reports;

namespace backend.Report;

public class AttendanceReportDocument : IDocument
{
    private readonly List<AttendanceReportDto> _attendance;
    private readonly DateOnly _startDate;
    private readonly DateOnly _endDate;

    public AttendanceReportDocument(
        List<AttendanceReportDto> attendance,
        DateOnly startDate,
        DateOnly endDate)
    {
        _attendance = attendance;
        _startDate = startDate;
        _endDate = endDate;
    }

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title = "Attendance Report",
            Author = "GYM Management System",
            Subject = "Player Attendance Report"
        };
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(30);

            page.DefaultTextStyle(x =>
                x.FontSize(9)
            );

            page.Header()
                .Element(ComposeHeader);

            page.Content()
                .Element(ComposeContent);

            page.Footer()
                .Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item()
                .AlignCenter()
                .Text("ASTRIK GYM ACADEMY")
                .FontSize(22)
                .Bold();

            column.Item()
                .PaddingTop(3)
                .AlignCenter()
                .Text("ATTENDANCE REPORT")
                .FontSize(15)
                .Bold();

            column.Item()
                .PaddingTop(10)
                .LineHorizontal(1);

            column.Item()
                .PaddingTop(8)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Text(
                            $"From: {_startDate:dd-MM-yyyy}    To: {_endDate:dd-MM-yyyy}"
                        )
                        .FontSize(10)
                        .Bold();

                    row.RelativeItem()
                        .AlignRight()
                        .Text($"Total Records: {_attendance.Count}")
                        .FontSize(10)
                        .Bold();
                });

            column.Item()
                .PaddingTop(3)
                .AlignRight()
                .Text($"Generated: {DateTime.Now:dd-MM-yyyy HH:mm}")
                .FontSize(8);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container
            .PaddingTop(15)
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);       // #
                    columns.RelativeColumn(2.5f);      // Player Name
                    columns.RelativeColumn(2.5f);      // Father Name
                    columns.RelativeColumn(1.5f);      // Date
                    columns.RelativeColumn(1.5f);      // Status
                    columns.RelativeColumn(4f);        // Remarks
                });

                table.Header(header =>
                {
                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("#");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Player Name");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Father Name");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Date");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Status");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Remarks");
                });

                for (int i = 0; i < _attendance.Count; i++)
                {
                    var record = _attendance[i];

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text((i + 1).ToString());

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(record.PlayerName);

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(record.FatherName);

                    table.Cell()
                        .Element(DataCellStyle)
                        .AlignCenter()
                        .Text(record.AttendanceDate.ToString("dd-MM-yyyy"));

                    table.Cell()
                        .Element(DataCellStyle)
                        .AlignCenter()
                        .Text(record.Status);

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(record.Remarks);
                }
            });
    }

    private void ComposeFooter(IContainer container)
    {
        container
            .PaddingTop(10)
            .Row(row =>
            {
                row.RelativeItem()
                    .Text("GYM Management System")
                    .FontSize(8);

                row.RelativeItem()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                    // .FontSize(8);
            });
    }

    private static IContainer HeaderCellStyle(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten2)
            .Border(1)
            .BorderColor(Colors.Grey.Medium)
            .PaddingVertical(6)
            .PaddingHorizontal(5)
            .AlignCenter()
            .AlignMiddle();
    }

    private static IContainer DataCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .PaddingHorizontal(5)
            .AlignMiddle();
    }
    
}
