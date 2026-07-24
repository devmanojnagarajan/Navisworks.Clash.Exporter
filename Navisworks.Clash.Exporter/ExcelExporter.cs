using System.Collections.Generic;
using ClosedXML.Excel;
using Navisworks.Clash.Exporter.Models;

namespace Navisworks.Clash.Exporter
{
    /// <summary>Writes extracted clash data to an .xlsx workbook using ClosedXML.</summary>
    public static class ExcelExporter
    {
        public static void Save(ExtractResult data, string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                WriteSummary(workbook, data.Summary);
                WriteClashes(workbook, data.Clashes);
                workbook.SaveAs(fileName);
            }
        }

        private static void WriteSummary(XLWorkbook workbook, IReadOnlyList<ClashTestSummaryRow> rows)
        {
            var ws = workbook.Worksheets.Add("Summary");

            var headers = new[]
            {
                "Clash Report", "Status", "Test Type", "Tolerance (mm)", "Last Run",
                "New", "Active", "Reviewed", "Approved", "Resolved", "Total"
            };
            WriteHeader(ws, headers);

            var r = 2;
            foreach (var row in rows)
            {
                ws.Cell(r, 1).Value = row.TestName;
                ws.Cell(r, 2).Value = row.Status;
                ws.Cell(r, 3).Value = row.TestType;
                ws.Cell(r, 4).Value = row.ToleranceMm;
                if (row.LastRun.HasValue) ws.Cell(r, 5).Value = row.LastRun.Value;
                ws.Cell(r, 6).Value = row.New;
                ws.Cell(r, 7).Value = row.Active;
                ws.Cell(r, 8).Value = row.Reviewed;
                ws.Cell(r, 9).Value = row.Approved;
                ws.Cell(r, 10).Value = row.Resolved;
                ws.Cell(r, 11).Value = row.Total;
                r++;
            }

            Finalise(ws, headers.Length, r - 1);
        }

        private static void WriteClashes(XLWorkbook workbook, IReadOnlyList<ClashRow> rows)
        {
            var ws = workbook.Worksheets.Add("Clash Results");

            var headers = new[]
            {
                "Clash Report", "Group", "Clash Name", "Priority", "Status", "Distance (mm)",
                "Description", "Date Found", "Assigned To", "Approved By", "Approved Time",
                "Level", "Grid Intersection", "Location (X, Y, Z)",
                "Item 1 Name", "Item 1 Id", "Item 1 Source File",
                "Item 2 Name", "Item 2 Id", "Item 2 Source File",
                "Comments", "Clash Guid"
            };
            WriteHeader(ws, headers);

            var r = 2;
            foreach (var row in rows)
            {
                ws.Cell(r, 1).Value = row.TestName;
                ws.Cell(r, 2).Value = row.GroupName;
                ws.Cell(r, 3).Value = row.ClashName;
                ws.Cell(r, 4).Value = row.Priority;
                ws.Cell(r, 5).Value = row.Status;
                ws.Cell(r, 6).Value = row.DistanceMm;
                ws.Cell(r, 7).Value = row.Description;
                if (row.DateFound.HasValue) ws.Cell(r, 8).Value = row.DateFound.Value;
                ws.Cell(r, 9).Value = row.AssignedTo;
                ws.Cell(r, 10).Value = row.ApprovedBy;
                if (row.ApprovedTime.HasValue) ws.Cell(r, 11).Value = row.ApprovedTime.Value;
                ws.Cell(r, 12).Value = row.Level;
                ws.Cell(r, 13).Value = row.GridIntersection;
                ws.Cell(r, 14).Value = row.Location;
                ws.Cell(r, 15).Value = row.Item1Name;
                ws.Cell(r, 16).Value = row.Item1Id;
                ws.Cell(r, 17).Value = row.Item1SourceFile;
                ws.Cell(r, 18).Value = row.Item2Name;
                ws.Cell(r, 19).Value = row.Item2Id;
                ws.Cell(r, 20).Value = row.Item2SourceFile;
                ws.Cell(r, 21).Value = row.Comments;
                ws.Cell(r, 22).Value = row.ClashGuid;
                r++;
            }

            Finalise(ws, headers.Length, r - 1);
        }

        private static void WriteHeader(IXLWorksheet ws, IReadOnlyList<string> headers)
        {
            for (var c = 0; c < headers.Count; c++)
                ws.Cell(1, c + 1).Value = headers[c];

            var headerRange = ws.Range(1, 1, 1, headers.Count);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0x1F, 0x4E, 0x79);
            headerRange.Style.Font.FontColor = XLColor.White;
        }

        private static void Finalise(IXLWorksheet ws, int columnCount, int lastRow)
        {
            if (lastRow < 1) lastRow = 1;
            ws.Range(1, 1, lastRow, columnCount).SetAutoFilter();
            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents();
        }
    }
}
