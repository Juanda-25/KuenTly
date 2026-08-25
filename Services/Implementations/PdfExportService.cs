using KuenTly.Helpers;
using KuenTly.Models;
using KuenTly.Services.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

namespace KuenTly.Services.Implementations
{
    public class PdfExportService : IPdfExportService
    {
        private static readonly KuenTlyFontResolver _fontResolver = new();
        private readonly EstadoCuentaToTextoConverter _estadoTexto = new();

        public async Task<string> ExportarVentasAsync(List<VentaResumen> ventas, string tituloReporte)
        {
            await AsegurarFuentesAsync();

            var rutaArchivo = Path.Combine(FileSystem.CacheDirectory, $"KuenTly_Reporte_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            var totalSaldo = ventas.Sum(v => v.Saldo);

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            const double margen = 30;

            var fontTitulo = new XFont("OpenSans", 18, XFontStyleEx.Bold);
            var fontSubtitulo = new XFont("OpenSans", 12, XFontStyleEx.Regular);
            var fontPequeno = new XFont("OpenSans", 9, XFontStyleEx.Regular);
            var fontEncabezado = new XFont("OpenSans", 10, XFontStyleEx.Bold);
            var fontCelda = new XFont("OpenSans", 9, XFontStyleEx.Regular);

            var colorAzul = new XSolidBrush(XColor.FromArgb(13, 59, 102));
            var colorGris = new XSolidBrush(XColor.FromArgb(107, 119, 133));
            var colorNegro = new XSolidBrush(XColors.Black);

            double y = margen;

            gfx.DrawString("KuenTly", fontTitulo, colorAzul, new XPoint(margen, y + 16));
            y += 26;
            gfx.DrawString(tituloReporte, fontSubtitulo, colorNegro, new XPoint(margen, y + 12));
            y += 20;
            gfx.DrawString($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}", fontPequeno, colorGris, new XPoint(margen, y + 9));
            y += 26;

            gfx.DrawString($"Total de ventas: {ventas.Count}    Saldo total: $ {totalSaldo:N0}", fontEncabezado, colorNegro, new XPoint(margen, y + 10));
            y += 22;

            double colCliente = margen;
            double colDescripcion = margen + 130;
            double colSaldo = margen + 330;
            double colEstado = margen + 420;

            gfx.DrawString("Cliente", fontEncabezado, colorNegro, new XPoint(colCliente, y));
            gfx.DrawString("Descripción", fontEncabezado, colorNegro, new XPoint(colDescripcion, y));
            gfx.DrawString("Saldo", fontEncabezado, colorNegro, new XPoint(colSaldo, y));
            gfx.DrawString("Estado", fontEncabezado, colorNegro, new XPoint(colEstado, y));
            y += 5;
            gfx.DrawLine(XPens.LightGray, margen, y, page.Width - margen, y);
            y += 16;

            const double altoLinea = 18;
            double limiteInferior = page.Height - margen;

            foreach (var venta in ventas)
            {
                if (y > limiteInferior)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = margen;
                }

                var nombreCliente = venta.Venta.Cliente?.Nombre ?? "-";
                var descripcion = venta.Venta.Descripcion.Length > 32
                    ? venta.Venta.Descripcion.Substring(0, 29) + "..."
                    : venta.Venta.Descripcion;
                var estadoTexto = (string)_estadoTexto.Convert(venta.Estado, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);

                gfx.DrawString(nombreCliente, fontCelda, colorNegro, new XPoint(colCliente, y));
                gfx.DrawString(descripcion, fontCelda, colorNegro, new XPoint(colDescripcion, y));
                gfx.DrawString($"$ {venta.Saldo:N0}", fontCelda, colorNegro, new XPoint(colSaldo, y));
                gfx.DrawString(estadoTexto, fontCelda, colorNegro, new XPoint(colEstado, y));

                y += altoLinea;
            }

            document.Save(rutaArchivo);

            return rutaArchivo;
        }

        private static async Task AsegurarFuentesAsync()
        {
            if (_fontResolver.Inicializado)
                return;

            await _fontResolver.InicializarAsync();
            GlobalFontSettings.FontResolver = _fontResolver;
        }
    }
}