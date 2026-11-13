using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using backend.DTOs.Reports;

namespace backend.Report;

public class PlayerReasonReportDocument : IDocument
{
    private readonly List<PlayerReasonReportDto> _players;

    public PlayerReasonReportDocument(List<PlayerReasonReportDto> players)
    {
        _players = players;
    }

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title = "Player Reason Report",
            Author = "GYM Management System",
            Subject = "Player Reasons"
        };
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(40);

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
                .AlignCenter()
                .Text("PLAYER REASON REPORT")
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
                        .AlignRight()
                        .Text($"Total Players: {_players.Count}")
                        .FontSize(10)
                        .Bold();               
                });

            column.Item()
                .PaddingTop(5)
                .AlignRight()
                .Text($"Generated: {DateTime.Now:dd-MM-yyyy HH:mm}")
                .FontSize(9);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container
            .PaddingTop(20)
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(35);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(65);
                    columns.RelativeColumn(4);
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
                        .Text("Age");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Gender");

                    header.Cell()
                        .Element(HeaderCellStyle)
                        .Text("Reason");
                });

                for (int i = 0; i < _players.Count; i++)
                {
                    var player = _players[i];

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text((i + 1).ToString());

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(player.Name);

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(player.FName);

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(player.Age);

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(player.Gender);

                    table.Cell()
                        .Element(DataCellStyle)
                        .Text(player.Reason);
                }
            });
    }

        private void ComposeFooter(IContainer container)
    {
        container.PaddingTop(10)
            .Row(row =>
            {
               row.RelativeItem()
                .Text("GYM Management System")
                .FontSize(10);

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
            .Padding(5)
            .AlignCenter()
            .AlignMiddle();
    }

    private static IContainer DataCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .AlignMiddle();
    }
}
