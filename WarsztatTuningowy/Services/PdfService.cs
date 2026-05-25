using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WarsztatTuningowy.Extensions;
using WarsztatTuningowy.Models.Domain;

namespace WarsztatTuningowy.Services
{
    public class PdfService
    {
        public byte[] GenerateInvoice(Invoice invoice)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeInvoiceHeader);
                    page.Content().Element(c => ComposeInvoiceContent(c, invoice));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public byte[] GenerateTuningProtocol(Order order)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeTuningHeader);
                    page.Content().Element(c =>
                        ComposeTuningContent(c, order));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        private void ComposeInvoiceHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("FAKTURA VAT")
                            .FontSize(22).Bold().FontColor("#1F5C99");
                        c.Item().Text("Warsztat Tuningowy")
                            .FontSize(14).Bold();
                    });

                    row.ConstantItem(150).Column(c =>
                    {
                        c.Item().AlignRight().Text(text =>
                        {
                            text.Span("Data wystawienia: ").Bold();
                            text.Span(DateTime.Now.ToString("dd.MM.yyyy"));
                        });
                    });
                });

                col.Item().PaddingTop(10)
                    .LineHorizontal(1).LineColor("#1F5C99");
            });
        }

        private void ComposeInvoiceContent(IContainer container, Invoice invoice)
        {
            container.Column(col =>
            {
                col.Spacing(15);

                col.Item().Background("#F3F4F6").Padding(10).Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span("Numer faktury: ").Bold();
                        text.Span(invoice.Number);
                    });
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Data wystawienia: ").Bold();
                        text.Span(invoice.IssuedAt.ToString("dd.MM.yyyy"));
                    });
                });

                col.Item().Row(row =>
                {
                    row.RelativeItem().Border(1).BorderColor("#E5E7EB")
                        .Padding(10).Column(c =>
                        {
                            c.Item().Text("NABYWCA").Bold()
                                .FontColor("#6B7280").FontSize(8);
                            c.Item().PaddingTop(5)
                                .Text(invoice.Order?.Vehicle?.Client?.FullName
                                    ?? "—").Bold();
                            c.Item()
                                .Text(invoice.Order?.Vehicle?.Client?.Email
                                    ?? "—");
                            c.Item()
                                .Text(invoice.Order?.Vehicle?.Client?.Phone
                                    ?? "—");
                        });

                    row.ConstantItem(20);

                    row.RelativeItem().Border(1).BorderColor("#E5E7EB")
                        .Padding(10).Column(c =>
                        {
                            c.Item().Text("POJAZD").Bold()
                                .FontColor("#6B7280").FontSize(8);
                            c.Item().PaddingTop(5).Text(
                                $"{invoice.Order?.Vehicle?.Brand} " +
                                $"{invoice.Order?.Vehicle?.Model} " +
                                $"({invoice.Order?.Vehicle?.Year})")
                                .Bold();
                            c.Item().Text(
                                $"VIN: {invoice.Order?.Vehicle?.VIN}");
                            c.Item().Text(
                                $"ECU: {invoice.Order?.Vehicle?.ECUType}");
                        });
                });

                col.Item().Text("POZYCJE").Bold()
                    .FontColor("#6B7280").FontSize(8);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(25);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn();
                        cols.RelativeColumn();
                        cols.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#1F5C99").Padding(5)
                            .Text("Lp").FontColor(Colors.White).Bold();
                        header.Cell().Background("#1F5C99").Padding(5)
                            .Text("Nazwa").FontColor(Colors.White).Bold();
                        header.Cell().Background("#1F5C99").Padding(5)
                            .AlignRight().Text("Ilość")
                            .FontColor(Colors.White).Bold();
                        header.Cell().Background("#1F5C99").Padding(5)
                            .AlignRight().Text("Cena jedn.")
                            .FontColor(Colors.White).Bold();
                        header.Cell().Background("#1F5C99").Padding(5)
                            .AlignRight().Text("Wartość")
                            .FontColor(Colors.White).Bold();
                    });

                    int lp = 1;

                    if (invoice.Order?.OrderParts != null)
                    {
                        foreach (var op in invoice.Order.OrderParts
                            .Where(op => op.IsUsed))
                        {
                            var bg = lp % 2 == 0 ? (Color)"#F9FAFB" : Colors.White;
                            table.Cell().Background(bg).Padding(5)
                                .Text(lp.ToString());
                            table.Cell().Background(bg).Padding(5)
                                .Text(op.Part?.Name ?? "—");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight().Text($"{op.Quantity} szt.");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight()
                                .Text(op.Part?.RetailPrice
                                    .ToString("C") ?? "—");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight()
                                .Text(op.RetailCost().ToString("C"));
                            lp++;
                        }
                    }

                    if (invoice.Order?.ServiceTasks != null)
                    {
                        foreach (var st in invoice.Order.ServiceTasks
                            .Where(st => st.EndTime != null))
                        {
                            var bg = lp % 2 == 0 ? (Color)"#F9FAFB" : Colors.White;
                            var hours = st.TotalMinutes / 60m;
                            var rate = st.AssignedEmployee?.HourlyRateClient
                                ?? invoice.Order.DefaultMechanic?.HourlyRateClient
                                ?? 0;
                            var value = hours * rate;

                            table.Cell().Background(bg).Padding(5)
                                .Text(lp.ToString());
                            table.Cell().Background(bg).Padding(5)
                                .Text($"Roboczogodziny — {st.Name}");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight()
                                .Text($"{hours:F1} h");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight()
                                .Text($"{rate:C}/h");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight().Text(value.ToString("C"));
                            lp++;
                        }
                    }
                });

                col.Item().AlignRight().Width(250).Column(summary =>
                {
                    summary.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Netto:").Bold();
                        row.ConstantItem(100).AlignRight()
                            .Text(invoice.NetAmount.ToString("C"));
                    });
                    summary.Item().Row(row =>
                    {
                        row.RelativeItem().Text("VAT 23%:").Bold();
                        row.ConstantItem(100).AlignRight()
                            .Text(invoice.VatAmount.ToString("C"));
                    });
                    summary.Item().Background("#1F5C99").Padding(5).Row(row =>
                    {
                        row.RelativeItem()
                            .Text("DO ZAPŁATY:").Bold()
                            .FontColor(Colors.White).FontSize(12);
                        row.ConstantItem(100).AlignRight()
                            .Text(invoice.GrossAmount.ToString("C"))
                            .Bold().FontColor(Colors.White).FontSize(12);
                    });
                });

                col.Item().PaddingTop(20).Text(text =>
                {
                    text.Span("Zlecenie nr: ").Bold();
                    text.Span($"#{invoice.OrderId}");
                    text.Span("  |  Cel tuningu: ").Bold();
                    text.Span(invoice.Order?.TuningGoal.GetDisplayName()
                        ?? "—");
                });
            });
        }
        private void ComposeTuningHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("TUNING PROTOCOL")
                            .FontSize(22).Bold().FontColor("#27500A");
                        c.Item().Text("Warsztat Tuningowy")
                            .FontSize(14).Bold();
                    });

                    row.ConstantItem(150).Column(c =>
                    {
                        c.Item().AlignRight()
                            .Text(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    });
                });

                col.Item().PaddingTop(10)
                    .LineHorizontal(1).LineColor("#27500A");
            });
        }
        private void ComposeTuningContent(IContainer container, Order order)
        {
            container.Column(col =>
            {
                col.Spacing(15);

                col.Item().Background("#F3F4F6").Padding(10).Column(c =>
                {
                    c.Item().Text("DANE POJAZDU").Bold()
                        .FontColor("#6B7280").FontSize(8);
                    c.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Pojazd: ").Bold();
                            text.Span($"{order.Vehicle?.Brand} " +
                                      $"{order.Vehicle?.Model} " +
                                      $"({order.Vehicle?.Year})");
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("VIN: ").Bold();
                            text.Span(order.Vehicle?.VIN ?? "—");
                        });
                    });
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Silnik: ").Bold();
                            text.Span(order.Vehicle?.EngineType ?? "—");
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("ECU: ").Bold();
                            text.Span(order.Vehicle?.ECUType ?? "—");
                        });
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Klient: ").Bold();
                        text.Span(order.Vehicle?.Client?.FullName ?? "—");
                    });
                });

                col.Item().Column(c =>
                {
                    c.Item().Text("ZAKRES TUNINGU").Bold()
                        .FontColor("#6B7280").FontSize(8);
                    c.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Cel tuningu: ").Bold();
                            text.Span(order.TuningGoal.GetDisplayName());
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Status: ").Bold();
                            text.Span(order.Status.GetDisplayName());
                        });
                    });
                });

                col.Item().Text("WYKONANE ZADANIA").Bold()
                    .FontColor("#6B7280").FontSize(8);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn();
                        cols.RelativeColumn();
                        cols.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#27500A").Padding(5)
                            .Text("Zadanie")
                            .FontColor(Colors.White).Bold();
                        header.Cell().Background("#27500A").Padding(5)
                            .Text("Mechanik")
                            .FontColor(Colors.White).Bold();
                        header.Cell().Background("#27500A").Padding(5)
                            .AlignRight().Text("Czas (h)")
                            .FontColor(Colors.White).Bold();
                        header.Cell().Background("#27500A").Padding(5)
                            .AlignRight().Text("Koszt")
                            .FontColor(Colors.White).Bold();
                    });

                    int i = 0;
                    foreach (var st in order.ServiceTasks)
                    {
                        var bg = i % 2 == 0 ? (Color)"#F9FAFB" : Colors.White;
                        var hours = st.TotalMinutes / 60m;
                        var cost = hours * (st.AssignedEmployee?
                            .HourlyRateClient ?? 0);

                        table.Cell().Background(bg).Padding(5)
                            .Text(st.Name);
                        table.Cell().Background(bg).Padding(5)
                            .Text(st.AssignedEmployee?.FullName ?? "—");
                        table.Cell().Background(bg).Padding(5)
                            .AlignRight().Text($"{hours:F1}");
                        table.Cell().Background(bg).Padding(5)
                            .AlignRight().Text(cost.ToString("C"));
                        i++;
                    }
                });

                if (order.OrderParts.Any(op => op.IsUsed))
                {
                    col.Item().Text("UŻYTE CZĘŚCI").Bold()
                        .FontColor("#6B7280").FontSize(8);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#633806").Padding(5)
                                .Text("Część")
                                .FontColor(Colors.White).Bold();
                            header.Cell().Background("#633806").Padding(5)
                                .AlignRight().Text("Ilość")
                                .FontColor(Colors.White).Bold();
                            header.Cell().Background("#633806").Padding(5)
                                .AlignRight().Text("Wartość")
                                .FontColor(Colors.White).Bold();
                        });

                        int j = 0;
                        foreach (var op in order.OrderParts
                            .Where(op => op.IsUsed))
                        {
                            var bg = j % 2 == 0 ? (Color)"#F9FAFB" : Colors.White;
                            table.Cell().Background(bg).Padding(5)
                                .Text(op.Part?.Name ?? "—");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight()
                                .Text($"{op.Quantity} szt.");
                            table.Cell().Background(bg).Padding(5)
                                .AlignRight()
                                .Text(op.RetailCost().ToString("C"));
                            j++;
                        }
                    });
                }

                var notes = order.ServiceTasks
                    .Where(st => !string.IsNullOrEmpty(st.Notes))
                    .ToList();

                if (notes.Any())
                {
                    col.Item().Text("NOTATKI TECHNICZNE").Bold()
                        .FontColor("#6B7280").FontSize(8);

                    foreach (var st in notes)
                    {
                        col.Item().Border(1).BorderColor("#E5E7EB")
                            .Padding(10).Column(c =>
                            {
                                c.Item().Text(st.Name).Bold();
                                c.Item().PaddingTop(5)
                                    .Text(st.Notes ?? "")
                                    .FontColor("#374151");
                            });
                    }
                }

                col.Item().AlignRight().Width(280).Column(summary =>
                {
                    summary.Item().Row(row =>
                    {
                        row.RelativeItem()
                            .Text("Koszt robocizny:").Bold();
                        row.ConstantItem(120).AlignRight().Text(
                            order.ServiceTasks
                                .Sum(st => (st.TotalMinutes / 60m) *
                                    (st.AssignedEmployee?.HourlyRateClient?? 0))
                                .ToString("C"));
                    });
                    summary.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Koszt części:").Bold();
                        row.ConstantItem(120).AlignRight().Text(
                            order.OrderParts.Where(op => op.IsUsed)
                                .Sum(op => op.RetailCost()).ToString("C"));
                    });
                    summary.Item().Background("#27500A").Padding(5).Row(row =>
                    {
                        row.RelativeItem().Text("RAZEM:").Bold()
                            .FontColor(Colors.White).FontSize(12);
                        row.ConstantItem(120).AlignRight()
                            .Text(order.TotalClientPrice().ToString("C"))
                            .Bold().FontColor(Colors.White).FontSize(12);
                    });
                });

                col.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span("Mechanik prowadzący: ").Bold();
                        text.Span(order.DefaultMechanic?.FullName ?? "—");
                    });
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Data: ").Bold();
                        text.Span(DateTime.Now.ToString("dd.MM.yyyy"));
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text("Warsztat Tuningowy — System PSI")
                    .FontSize(8).FontColor("#9CA3AF");
                row.ConstantItem(100).AlignRight().Text(text =>
                {
                    text.Span("Strona ").FontSize(8).FontColor("#9CA3AF");
                    text.CurrentPageNumber().FontSize(8)
                        .FontColor("#9CA3AF");
                    text.Span(" z ").FontSize(8).FontColor("#9CA3AF");
                    text.TotalPages().FontSize(8).FontColor("#9CA3AF");
                });
            });
        }
        public byte[] GenerateStatement(Order order)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => ComposeStatementHeader(c, order));
                    page.Content().Element(c => ComposeStatementContent(c, order));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        private void ComposeStatementHeader(IContainer container, Order order)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("OŚWIADCZENIE KLIENTA")
                            .FontSize(22).Bold().FontColor("#1F5C99");
                        c.Item().Text("Warsztat Tuningowy")
                            .FontSize(14).Bold();
                    });
                    row.ConstantItem(150).Column(c =>
                    {
                        c.Item().AlignRight()
                            .Text(order.StatementAcceptedAt?
                                .ToString("dd.MM.yyyy HH:mm") ?? "—");
                    });
                });
                col.Item().PaddingTop(10)
                    .LineHorizontal(1).LineColor("#1F5C99");
            });
        }

        private void ComposeStatementContent(
            IContainer container, Order order)
        {
            container.Column(col =>
            {
                col.Spacing(15);

                col.Item().Background("#F3F4F6").Padding(10).Column(c =>
                {
                    c.Item().Text("DANE ZLECENIA").Bold()
                        .FontColor("#6B7280").FontSize(8);
                    c.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Zlecenie nr: ").Bold();
                            text.Span($"#{order.Id}");
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Data: ").Bold();
                            text.Span(order.CreatedAt
                                .ToString("dd.MM.yyyy"));
                        });
                    });
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Klient: ").Bold();
                            text.Span(order.Vehicle?.Client?.FullName ?? "—");
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Pojazd: ").Bold();
                            text.Span($"{order.Vehicle?.Brand} {order.Vehicle?.Model} ({order.Vehicle?.Year})");
                        });
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("VIN: ").Bold();
                        text.Span(order.Vehicle?.VIN ?? "—");
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Cel tuningu: ").Bold();
                        text.Span(order.TuningGoal.GetDisplayName());
                    });
                });

                col.Item().Text("TREŚĆ OŚWIADCZENIA").Bold()
                    .FontColor("#6B7280").FontSize(8);

                col.Item().Border(1).BorderColor("#E5E7EB")
                    .Padding(15).Column(c =>
                    {
                        c.Item().Text(
                            "Ja, niżej podpisany/a, oświadczam że zostałem/am poinformowany/a o zakresie oraz konsekwencjach " +
                            "technicznych i prawnych zleconych modyfikacji pojazdu.")
                            .Italic();

                        c.Item().PaddingTop(10).Column(points =>
                        {
                            var items = new[]
                            {
                                ("✓", "Akceptuję zakres modyfikacji pojazdu określony w zleceniu nr #" + order.Id),
                                ("✓", "Rozumiem możliwy wpływ modyfikacji na emisję spalin oraz systemy DPF/OPF/GPF"),
                                ("✓", "Akceptuję ryzyko utraty gwarancji producenta pojazdu"),
                                ("✓", "Potwierdzam że zostałem/am poinformowany/a o konsekwencjach prawnych modyfikacji (m.in. badanie techniczne, ubezpieczenie)")
                            };

                            foreach (var (check, text) in items)
                            {
                                points.Item().PaddingTop(5).Row(row =>
                                {
                                    row.ConstantItem(20)
                                        .Text(check)
                                        .FontColor("#27500A").Bold();
                                    row.RelativeItem().Text(text);
                                });
                            }
                        });
                    });

                col.Item().Background("#EAF3DE").Padding(10).Column(c =>
                {
                    c.Item().Text("POTWIERDZENIE ELEKTRONICZNE").Bold()
                        .FontColor("#27500A").FontSize(8);
                    c.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Oświadczenie zaakceptowano elektronicznie w systemie w dniu: ").Bold();
                        text.Span(order.StatementAcceptedAt?
                            .ToString("dd.MM.yyyy o HH:mm") ?? "—");
                    });
                    c.Item().Text(text =>
                    {
                        text.Span("Przez: ").Bold();
                        text.Span(order.StatementAcceptedBy ?? "—");
                    });
                    c.Item().PaddingTop(5).Text(
                        "Niniejszy dokument stanowi potwierdzenie elektronicznej akceptacji oświadczenia w systemie informatycznym warsztatu.")
                        .FontSize(8).Italic().FontColor("#6B7280");
                });
            });
        }
    }
}