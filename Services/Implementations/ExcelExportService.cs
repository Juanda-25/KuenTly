using ClosedXML.Excel;
using KuenTly.Helpers;
using KuenTly.Models;
using KuenTly.Services.Interfaces;

namespace KuenTly.Services.Implementations
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly EstadoCuentaToTextoConverter _estadoTexto = new();

        public Task<string> ExportarVentasAsync(List<VentaResumen> ventas)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Ventas");

            worksheet.Cell(1, 1).Value = "Cliente";
            worksheet.Cell(1, 2).Value = "Zona";
            worksheet.Cell(1, 3).Value = "Descripción";
            worksheet.Cell(1, 4).Value = "Fecha venta";
            worksheet.Cell(1, 5).Value = "Fecha pago";
            worksheet.Cell(1, 6).Value = "Valor total";
            worksheet.Cell(1, 7).Value = "Saldo";
            worksheet.Cell(1, 8).Value = "Estado";
            worksheet.Row(1).Style.Font.Bold = true;

            var fila = 2;
            foreach (var venta in ventas)
            {
                worksheet.Cell(fila, 1).Value = venta.Venta.Cliente?.Nombre ?? "-";
                worksheet.Cell(fila, 2).Value = venta.Venta.Cliente?.Zona ?? "-";
                worksheet.Cell(fila, 3).Value = venta.Venta.Descripcion;
                worksheet.Cell(fila, 4).Value = venta.Venta.FechaVenta;
                worksheet.Cell(fila, 5).Value = venta.Venta.FechaPagoAcordada;
                worksheet.Cell(fila, 6).Value = venta.Venta.ValorTotal;
                worksheet.Cell(fila, 7).Value = venta.Saldo;
                worksheet.Cell(fila, 8).Value = (string)_estadoTexto.Convert(venta.Estado, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);
                fila++;
            }

            worksheet.Column(6).Style.NumberFormat.Format = "#,##0";
            worksheet.Column(7).Style.NumberFormat.Format = "#,##0";
            worksheet.Columns().AdjustToContents();

            var rutaArchivo = Path.Combine(FileSystem.CacheDirectory, $"KuenTly_Reporte_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            workbook.SaveAs(rutaArchivo);

            return Task.FromResult(rutaArchivo);
        }
    }
}