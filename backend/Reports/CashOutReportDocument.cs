using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using backend.DTO.Reports;

namespace backend.Report;
public class CashOutReportDocument : IDocument
    {
        private readonly List<CashOutReportDto> _cashOuts;
        private readonly DateOnly _startDate;
        private readonly DateOnly _endDate;

        public CashOutReportDocument(
            List<CashOutReportDto> cashOuts,
            DateOnly startDate,
            DateOnly endDate)
        {
            _cashOuts = cashOuts;
            _startDate = startDate;
            _endDate = endDate;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = "Cash Out Report",
                Author = "GYM Management System",
                Subject = "Cash Out Report"
            };
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());

                page.Margin(30);

                page.DefaultTextStyle(x =>
                    x.FontSize(9));

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
            var totalAmount = _cashOuts.Sum(x => x.Amount);

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
                    .Text("CASH OUT REPORT")
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

                        // row.RelativeItem()
                        //     .AlignRight()
                        //     .Text(
                        //         $"Total Records: {_cashOuts.Count}"
                        //     )
                        //     .FontSize(10)
                        //     .Bold();
                    });

                column.Item()
                    .PaddingTop(5)
                    .AlignRight()
                    .Text(
                        $"Total Cash Out: {totalAmount:N2}"
                    )
                    .FontSize(10)
                    .Bold();

                column.Item()
                    .PaddingTop(3)
                    .AlignLeft()
                    .Text(
                        $"Generated: {DateTime.Now:dd-MM-yyyy HH:mm}"
                    )
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
                        columns.ConstantColumn(35);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(5f);
                    });

                    table.Header(header =>
                    {
                        header.Cell()
                            .Element(HeaderCellStyle)
                            .Text("#");

                        header.Cell()
                            .Element(HeaderCellStyle)
                            .Text("Date");

                        header.Cell()
                            .Element(HeaderCellStyle)
                            .Text("Amount");

                        header.Cell()
                            .Element(HeaderCellStyle)
                            .Text("Description");
                    });

                    for (int i = 0; i < _cashOuts.Count; i++)
                    {
                        var cashOut = _cashOuts[i];

                        table.Cell()
                            .Element(DataCellStyle)
                            .AlignCenter()
                            .Text((i + 1).ToString());

                        table.Cell()
                            .Element(DataCellStyle)
                            .AlignCenter()
                            .Text(
                                cashOut.ExpenseDate
                                    .ToString("dd-MM-yyyy")
                            );

                        table.Cell()
                            .Element(DataCellStyle)
                            .AlignCenter()
                            .Text(
                                cashOut.Amount.ToString("N2")
                                );

                        table.Cell()
                            .Element(DataCellStyle)
                            .AlignLeft()
                            .Text(cashOut.Description);
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
                        .Text(
                            "GYM Management System"
                        )
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

        private static IContainer HeaderCellStyle(
            IContainer container)
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

        private static IContainer DataCellStyle(
            IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5)
                .PaddingHorizontal(5)
                .AlignMiddle();
        }
    }
