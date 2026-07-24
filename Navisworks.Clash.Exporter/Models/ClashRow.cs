using System;

namespace Navisworks.Clash.Exporter.Models
{
    /// <summary>One row per clash result on the "Clash Results" worksheet.</summary>
    public class ClashRow
    {
        public string TestName { get; set; }
        public string GroupName { get; set; }
        public string ClashName { get; set; }
        public string ClashGuid { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public double DistanceMm { get; set; }
        public string Description { get; set; }
        public DateTime? DateFound { get; set; }
        public string AssignedTo { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string Level { get; set; }
        public string GridIntersection { get; set; }
        public string Location { get; set; }
        public string Item1Name { get; set; }
        public string Item1Id { get; set; }
        public string Item1SourceFile { get; set; }
        public string Item2Name { get; set; }
        public string Item2Id { get; set; }
        public string Item2SourceFile { get; set; }
        public string Comments { get; set; }
    }
}
