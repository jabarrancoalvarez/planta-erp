using System.Globalization;
using PlanTA.Facturacion.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PlanTA.API.Services;

public sealed class FacturaPdfService
{
    private static readonly CultureInfo Es = CultureInfo.GetCultureInfo("es-ES");

    public byte[] Generate(FacturaDto f, string empresaNombre, string? empresaCif)
    {
        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(t => t.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(empresaNombre).Bold().FontSize(16);
                        if (!string.IsNullOrWhiteSpace(empresaCif))
                            col.Item().Text($"CIF: {empresaCif}").FontSize(9).FontColor(Colors.Grey.Darken2);
                    });
                    row.ConstantItem(180).AlignRight().Column(col =>
                    {
                        col.Item().Text("FACTURA").Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                        col.Item().Text(f.NumeroCompleto).FontSize(11);
                        col.Item().Text($"Fecha: {f.FechaEmision.LocalDateTime:dd/MM/yyyy}").FontSize(9);
                        if (f.FechaVencimiento.HasValue)
                            col.Item().Text($"Vencimiento: {f.FechaVencimiento.Value.LocalDateTime:dd/MM/yyyy}").FontSize(9);
                        col.Item().Text($"Estado: {f.Estado}").FontSize(9);
                    });
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(cli =>
                    {
                        cli.Item().Text("CLIENTE").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        cli.Item().Text(f.ClienteNombre).Bold();
                        if (!string.IsNullOrWhiteSpace(f.ClienteNIF))
                            cli.Item().Text($"NIF: {f.ClienteNIF}");
                        if (!string.IsNullOrWhiteSpace(f.ClienteDireccion))
                            cli.Item().Text(f.ClienteDireccion);
                    });

                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(25);
                            c.RelativeColumn(4);
                            c.ConstantColumn(50);
                            c.ConstantColumn(70);
                            c.ConstantColumn(45);
                            c.ConstantColumn(45);
                            c.ConstantColumn(70);
                        });

                        t.Header(h =>
                        {
                            static IContainer H(IContainer c) => c
                                .Background(Colors.Blue.Darken2)
                                .PaddingVertical(6).PaddingHorizontal(4);
                            h.Cell().Element(H).Text("#").FontColor(Colors.White).Bold();
                            h.Cell().Element(H).Text("Descripción").FontColor(Colors.White).Bold();
                            h.Cell().Element(H).AlignRight().Text("Cant.").FontColor(Colors.White).Bold();
                            h.Cell().Element(H).AlignRight().Text("Precio").FontColor(Colors.White).Bold();
                            h.Cell().Element(H).AlignRight().Text("Dto%").FontColor(Colors.White).Bold();
                            h.Cell().Element(H).AlignRight().Text("IVA%").FontColor(Colors.White).Bold();
                            h.Cell().Element(H).AlignRight().Text("Total").FontColor(Colors.White).Bold();
                        });

                        foreach (var l in f.Lineas)
                        {
                            static IContainer B(IContainer c) => c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(4);
                            t.Cell().Element(B).Text(l.NumeroLinea.ToString());
                            t.Cell().Element(B).Text(l.Descripcion);
                            t.Cell().Element(B).AlignRight().Text(l.Cantidad.ToString("0.##", Es));
                            t.Cell().Element(B).AlignRight().Text(l.PrecioUnitario.ToString("C2", Es));
                            t.Cell().Element(B).AlignRight().Text(l.DescuentoPct.ToString("0.##", Es));
                            t.Cell().Element(B).AlignRight().Text(l.IvaPct.ToString("0.##", Es));
                            t.Cell().Element(B).AlignRight().Text(l.Total.ToString("C2", Es));
                        }
                    });

                    col.Item().AlignRight().Width(240).Column(tot =>
                    {
                        tot.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Base imponible");
                            r.ConstantItem(100).AlignRight().Text(f.BaseImponible.ToString("C2", Es));
                        });
                        tot.Item().Row(r =>
                        {
                            r.RelativeItem().Text("IVA");
                            r.ConstantItem(100).AlignRight().Text(f.TotalIva.ToString("C2", Es));
                        });
                        tot.Item().BorderTop(1).BorderColor(Colors.Grey.Darken2).PaddingTop(4).Row(r =>
                        {
                            r.RelativeItem().Text("TOTAL").Bold().FontSize(12);
                            r.ConstantItem(100).AlignRight().Text(f.Total.ToString("C2", Es)).Bold().FontSize(12);
                        });
                    });

                    if (!string.IsNullOrWhiteSpace(f.Observaciones))
                    {
                        col.Item().PaddingTop(10).Column(obs =>
                        {
                            obs.Item().Text("Observaciones").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                            obs.Item().Text(f.Observaciones);
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(f.VerifactuCsv))
                    {
                        col.Item().PaddingTop(8).Background(Colors.Green.Lighten5).Padding(8).Column(v =>
                        {
                            v.Item().Text("VERI*FACTU").Bold().FontSize(9).FontColor(Colors.Green.Darken3);
                            v.Item().Text($"CSV: {f.VerifactuCsv}").FontSize(8);
                            if (!string.IsNullOrWhiteSpace(f.HashActual))
                                v.Item().Text($"Hash: {f.HashActual}").FontSize(7).FontColor(Colors.Grey.Darken1);
                        });
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                    x.Span(" / ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    x.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();
    }
}
