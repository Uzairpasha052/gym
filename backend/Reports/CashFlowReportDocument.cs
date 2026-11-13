using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using backend.DTO.Reports;

namespace backend.Report;

public class CashFlowReportDocument : IDocument
{
    private readonly CashFlowReportDto _report;

    public CashFlowReportDocument(CashFlowReportDto report)
    {
        _report = report;
    }

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title = "Cash Flow Report",
            Author = "GYM Management System",
            Subject = "Cash Flow Report"
        };
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(30);

            page.DefaultTextStyle(text =>
                text.FontSize(9));

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
                .Text("CASH FLOW REPORT")
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
                            $"From: {_report.StartDate:dd-MM-yyyy}    " +
                            $"To: {_report.EndDate:dd-MM-yyyy}"
                        )
                        .FontSize(10)
                        .Bold();

                    row.RelativeItem()
                        .AlignRight()
                        .Text(
                            $"Generated: {DateTime.Now:dd-MM-yyyy HH:mm}"
                        )
                        .FontSize(8);
                });
        });
    }

    private void ComposeContent(IContainer container)   
    {
        container
            .PaddingTop(15)
            .Column(column =>
            {
                // -------------------------------
                // SUMMARY
                // -------------------------------

                column.Item()
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Element(SummaryCard)
                            .Column(card =>
                            {
                                card.Item()
                                    .AlignCenter()
                                    .Text("TOTAL CASH IN")
                                    .Bold()
                                    .FontSize(9);

                                card.Item()
                                    .PaddingTop(5)
                                    .AlignCenter()
                                    .Text(
                                        _report.TotalCashIn
                                            .ToString("N2")
                                    )
                                    .FontSize(15)
                                    .Bold();
                            });

                        row.RelativeItem()
                            .Element(SummaryCard)
                            .Column(card =>
                            {
                                card.Item()
                                    .AlignCenter()
                                    .Text("TOTAL CASH OUT")
                                    .Bold()
                                    .FontSize(9);

                                card.Item()
                                    .PaddingTop(5)
                                    .AlignCenter()
                                    .Text(
                                        _report.TotalCashOut
                                            .ToString("N2")
                                    )
                                    .FontSize(15)
                                    .Bold();
                            });

                        row.RelativeItem()
                            .Element(SummaryCard)
                            .Column(card =>
                            {
                                card.Item()
                                    .AlignCenter()
                                    .Text("PROFIT AMOUNT")
                                    .Bold()
                                    .FontSize(9);

                                card.Item()
                                    .PaddingTop(5)
                                    .AlignCenter()
                                    .Text(
                                        _report.ProfitAmount
                                            .ToString("N2")
                                    )
                                    .FontSize(15)
                                    .Bold();
                            });
                    });


                // -------------------------------
                // CHART
                // -------------------------------

                column.Item()
                    .PaddingTop(20)
                    .Text("Cash Flow Summary")
                    .FontSize(13)
                    .Bold()
                    .AlignCenter();

                column.Item()
                    .PaddingTop(10)
                    .Height(260)
                    .Svg(GenerateCashFlowChart);


                // -------------------------------
                // TABLE TITLE
                // -------------------------------

                column.Item()
                    .PaddingTop(20)
                    .PaddingBottom(8)
                    .Text("Cash Flow Details")
                    .FontSize(13)
                    .Bold();


                // -------------------------------
                // DATA TABLE
                // -------------------------------

                column.Item()
                    .Element(ComposeDataTable);
            });
    }

    // ============================================================
    // CASH FLOW TABLE
    // ============================================================

    private void ComposeDataTable(IContainer container)
    {
        var transactions = BuildTransactions();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(1.5f); // Player
                columns.RelativeColumn(1.5f); // Father
                columns.RelativeColumn(1.2f); // Cash In
                columns.RelativeColumn(1.3f); // Payment Date
                columns.RelativeColumn(2.2f); // Description
                columns.RelativeColumn(1.2f); // Cash Out
                columns.RelativeColumn(1.3f); // Expense Date
            });


            // ============================
            // HEADER
            // ============================

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
                    .Text("Cash In");

                header.Cell()
                    .Element(HeaderCellStyle)
                    .Text("Date");

                header.Cell()
                    .Element(HeaderCellStyle)
                    .Text("Description");

                header.Cell()
                    .Element(HeaderCellStyle)
                    .Text("Cash Out");

                header.Cell()
                    .Element(HeaderCellStyle)
                    .Text("Expense Date");
            });


            // ============================
            // DATA
            // ============================

            if (transactions.Count == 0)
            {
                table.Cell()
                    .ColumnSpan(8)
                    .Element(DataCellStyle)
                    .AlignCenter()
                    .Text("No cash transactions found.");
                
                return;
            }


            for (int i = 0; i < transactions.Count; i++)
            {
                var transaction = transactions[i];

                table.Cell()
                    .Element(DataCellStyle)
                    .AlignCenter()
                    .Text((i + 1).ToString());


                // PLAYER NAME
                table.Cell()
                    .Element(DataCellStyle)
                    .Text(transaction.PlayerName);


                // FATHER NAME
                table.Cell()
                    .Element(DataCellStyle)
                    .Text(transaction.FatherName);


                // CASH IN
                table.Cell()
                    .Element(DataCellStyle)
                    .AlignRight()
                    .Text(
                        transaction.CashInAmount.HasValue
                            ? transaction.CashInAmount.Value.ToString("N2")
                            : "-"
                    );


                // PAYMENT DATE
                table.Cell()
                    .Element(DataCellStyle)
                    .AlignCenter()
                    .Text(
                        transaction.PaymentDate.HasValue
                            ? transaction.PaymentDate.Value
                                .ToString("dd-MM-yyyy")
                            : "-"
                    );


                // DESCRIPTION
                table.Cell()
                    .Element(DataCellStyle)
                    .Text(transaction.Description);


                // CASH OUT
                table.Cell()
                    .Element(DataCellStyle)
                    .AlignRight()
                    .Text(
                        transaction.CashOutAmount.HasValue
                            ? transaction.CashOutAmount.Value.ToString("N2")
                            : "-"
                    );


                // EXPENSE DATE
                table.Cell()
                    .Element(DataCellStyle)
                    .AlignCenter()
                    .Text(
                        transaction.ExpenseDate.HasValue
                            ? transaction.ExpenseDate.Value
                                .ToString("dd-MM-yyyy")
                            : "-"
                    );
            }
        });
    }


    // ============================================================
    // BUILD INDEPENDENT TRANSACTIONS
    // ============================================================

    private List<CashFlowTransaction> BuildTransactions()
    {
        var transactions = new List<CashFlowTransaction>();

        // Cash In records
        foreach (var cashIn in _report.CashIns)
        {
            transactions.Add(new CashFlowTransaction
            {
                PlayerName = cashIn.PlayerName,
                FatherName = cashIn.FatherName,

                CashInAmount = cashIn.Amount,
                PaymentDate = cashIn.PaymentDate,

                Description = "-",

                CashOutAmount = null,
                ExpenseDate = null,

                SortDate = cashIn.PaymentDate
            });
        }


        // Cash Out records
        foreach (var cashOut in _report.CashOuts)
        {
            transactions.Add(new CashFlowTransaction
            {
                PlayerName = "-",
                FatherName = "-",

                CashInAmount = null,
                PaymentDate = null,

                Description = cashOut.Description,

                CashOutAmount = cashOut.Amount,
                ExpenseDate = cashOut.ExpenseDate,

                SortDate = cashOut.ExpenseDate
            });
        }


        return transactions
            .OrderByDescending(x => x.SortDate)
            .ToList();
    }


    // ============================================================
    // CHART
    // ============================================================

    private string GenerateCashFlowChart(Size size)
    {
        const double chartWidth = 700;
        const double chartHeight = 250;

        const double left = 70;
        const double right = 30;
        const double top = 25;
        const double bottom = 50;

        // var plotWidth = chartWidth - left - right;
        var plotHeight = chartHeight - top - bottom;

        var values = new[]
        {
            (Name: "Cash In", Value: _report.TotalCashIn),
            (Name: "Cash Out", Value: _report.TotalCashOut),
            (Name: "Profit", Value: _report.ProfitAmount)
        };

        var maximum = values
            .Select(x => Math.Abs((double)x.Value))
            .DefaultIfEmpty(1)
            .Max();

        if (maximum == 0)
        {
            maximum = 1;
        }

        var scale = plotHeight / maximum;

        // Zero line
        var zeroY = _report.ProfitAmount < 0
            ? top + (plotHeight / 2)
            : top + plotHeight;

        // Keep chart readable for positive values.
        if (_report.TotalCashIn >= 0 &&
            _report.TotalCashOut >= 0 &&
            _report.ProfitAmount >= 0)
        {
            zeroY = top + plotHeight;
        }

        var barWidth = 90;
        var spacing = 70;

        var firstX = left + 45;

        var cashInX = firstX;
        var cashOutX = cashInX + barWidth + spacing;
        var profitX = cashOutX + barWidth + spacing;


        string CreateBar(
            double x,
            decimal value,
            string label,
            string fill)
        {
            var numericValue = (double)value;

            var barHeight =
                Math.Abs(numericValue) * scale;

            var y = numericValue >= 0
                ? zeroY - barHeight
                : zeroY;

            var height = Math.Max(barHeight, 1);

            var valueText =
                value.ToString("N2");

            return $"""
                <rect
                    x="{x}"
                    y="{y}"
                    width="{barWidth}"
                    height="{height}"
                    fill="{fill}"
                    rx="2"
                />

                <text
                    x="{x + barWidth / 2}"
                    y="{Math.Max(y - 7, 15)}"
                    text-anchor="middle"
                    font-size="12"
                    font-family="Arial"
                    font-weight="bold"
                    fill="#333333">
                    {valueText}
                </text>

                <text
                    x="{x + barWidth / 2}"
                    y="{zeroY + 25}"
                    text-anchor="middle"
                    font-size="12"
                    font-family="Arial">
                    {label}
                </text>
                """;
        }


        var chart = $"""
            <svg
                width="{chartWidth}"
                height="{chartHeight}"
                viewBox="0 0 {chartWidth} {chartHeight}"
                xmlns="http://www.w3.org/2000/svg">

                <rect
                    x="0"
                    y="0"
                    width="{chartWidth}"
                    height="{chartHeight}"
                    fill="white"
                />

                <!-- Grid lines -->

                <line
                    x1="{left}"
                    y1="{top}"
                    x2="{left}"
                    y2="{top + plotHeight}"
                    stroke="#777777"
                    stroke-width="1"
                />

                <line
                    x1="{left}"
                    y1="{zeroY}"
                    x2="{chartWidth - right}"
                    y2="{zeroY}"
                    stroke="#777777"
                    stroke-width="1"
                />


                <!-- Chart title -->

                {CreateBar(
                    cashInX,
                    _report.TotalCashIn,
                    "Cash In",
                    "#1976D2"
                )}

                {CreateBar(
                    cashOutX,
                    _report.TotalCashOut,
                    "Cash Out",
                    "#D32F2F"
                )}

                {CreateBar(
                    profitX,
                    _report.ProfitAmount,
                    "Profit",
                    "#388E3C"
                )}

            </svg>
            """;

        return chart;
    }


    // ============================================================
    // FOOTER
    // ============================================================

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


    // ============================================================
    // STYLES
    // ============================================================

    private static IContainer SummaryCard(
        IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten1)
            .Background(Colors.Grey.Lighten4)
            .Padding(10);
    }


    private static IContainer HeaderCellStyle(
        IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten2)
            .Border(1)
            .BorderColor(Colors.Grey.Medium)
            .PaddingVertical(6)
            .PaddingHorizontal(4)
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
            .PaddingHorizontal(4)
            .AlignMiddle();
    }


    // ============================================================
    // INTERNAL TABLE MODEL
    // ============================================================

    private class CashFlowTransaction
    {
        public string PlayerName { get; set; } = "-";

        public string FatherName { get; set; } = "-";

        public decimal? CashInAmount { get; set; }

        public DateOnly? PaymentDate { get; set; }

        public string Description { get; set; } = "-";

        public decimal? CashOutAmount { get; set; }

        public DateOnly? ExpenseDate { get; set; }

        public DateOnly SortDate { get; set; }
    }
}
