using System;

namespace Navisworks.Clash.Exporter.Models
{
    /// <summary>
    /// Per-test totals. Not written to the workbook (everything goes on the single
    /// "Clash Results" sheet) - used to report what was exported.
    /// </summary>
    public class ClashTestSummaryRow
    {
        public string TestName { get; set; }
        public string Status { get; set; }
        public string TestType { get; set; }
        public double ToleranceMm { get; set; }
        public DateTime? LastRun { get; set; }
        public int New { get; set; }
        public int Active { get; set; }
        public int Reviewed { get; set; }
        public int Approved { get; set; }
        public int Resolved { get; set; }
        public int Total { get; set; }
    }
}
