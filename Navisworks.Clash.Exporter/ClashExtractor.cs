using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Navisworks.Clash.Exporter.Extensions;
using Navisworks.Clash.Exporter.Models;

namespace Navisworks.Clash.Exporter
{
    /// <summary>
    /// Reads every clash test in a document and flattens the results
    /// (including grouped clashes) into rows ready for Excel export.
    /// </summary>
    public class ExtractResult
    {
        public List<ClashTestSummaryRow> Summary { get; } = new List<ClashTestSummaryRow>();
        public List<ClashRow> Clashes { get; } = new List<ClashRow>();
    }

    public static class ClashExtractor
    {
        public static ExtractResult Extract(DocumentClashTests clashTests)
        {
            var result = new ExtractResult();
            var gridSystem = Application.MainDocument.Grids.ActiveSystem;

            foreach (var savedItem in clashTests.Tests)
            {
                if (!(savedItem is ClashTest test)) continue;

                var allResults = GetAllResults(test).ToList();

                result.Summary.Add(new ClashTestSummaryRow
                {
                    TestName = test.DisplayName,
                    Status = test.Status.ToString(),
                    TestType = test.TestType.ToString(),
                    ToleranceMm = test.Tolerance.ToMillimetres(),
                    LastRun = test.LastRun,
                    New = allResults.Count(r => r.Status == ClashResultStatus.New),
                    Active = allResults.Count(r => r.Status == ClashResultStatus.Active),
                    Reviewed = allResults.Count(r => r.Status == ClashResultStatus.Reviewed),
                    Approved = allResults.Count(r => r.Status == ClashResultStatus.Approved),
                    Resolved = allResults.Count(r => r.Status == ClashResultStatus.Resolved),
                    Total = allResults.Count
                });

                foreach (var clash in allResults)
                    result.Clashes.Add(BuildRow(test, clash, gridSystem));
            }

            return result;
        }

        /// <summary>Returns every ClashResult in a test, including those nested inside groups.</summary>
        private static IEnumerable<ClashResult> GetAllResults(ClashTest test)
        {
            foreach (var child in test.Children)
            {
                switch (child)
                {
                    case ClashResult direct:
                        yield return direct;
                        break;
                    case ClashResultGroup group:
                        foreach (var grouped in group.Children.OfType<ClashResult>())
                            yield return grouped;
                        break;
                }
            }
        }

        private static ClashRow BuildRow(ClashTest test, ClashResult clash, GridSystem gridSystem)
        {
            var row = new ClashRow
            {
                TestName = test.DisplayName,
                GroupName = (clash.Parent as ClashResultGroup)?.DisplayName,
                ClashName = clash.DisplayName,
                ClashGuid = clash.Guid.ToString(),
                Priority = clash.Priority.ToString(),
                Status = clash.Status.ToString(),
                DistanceMm = clash.Distance.ToMillimetres(),
                Description = clash.Description,
                DateFound = clash.CreatedTime,
                AssignedTo = clash.AssignedTo?.DisplayName,
                ApprovedBy = clash.ApprovedBy?.DisplayName,
                ApprovedTime = clash.ApprovedTime,
                Comments = clash.Comments != null
                    ? string.Join(" | ", clash.Comments.Select(c => c.Body))
                    : null
            };

            try
            {
                row.Location = $"{clash.Center.X:F3}, {clash.Center.Y:F3}, {clash.Center.Z:F3}";
            }
            catch
            {
                // Center can throw for some result types; leave blank.
            }

            if (gridSystem != null)
            {
                var intersection = gridSystem.ClosestIntersection(clash.Center);
                row.Level = intersection?.Level?.DisplayName;
                row.GridIntersection = intersection?.DisplayName?.Replace($" : {row.Level}", string.Empty);
            }

            FillItem(clash.Item1, name => row.Item1Name = name, id => row.Item1Id = id, src => row.Item1SourceFile = src);
            FillItem(clash.Item2, name => row.Item2Name = name, id => row.Item2Id = id, src => row.Item2SourceFile = src);

            return row;
        }

        private static void FillItem(ModelItem item, Action<string> setName, Action<string> setId,
            Action<string> setSource)
        {
            if (item == null) return;

            var identifiable = item.GetUniquelyIdentifiableItem(out var uniqueId);
            setName(item.DisplayName);
            setId(uniqueId);
            setSource(GetSourceFile(identifiable ?? item));
        }

        private static string GetSourceFile(ModelItem modelItem)
        {
            try
            {
                var source = modelItem.Model?.SourceFileName;
                if (!string.IsNullOrEmpty(source)) return source;

                var item = modelItem;
                while (item.Parent != null)
                {
                    item = item.Parent;
                    source = item.DisplayName;
                }

                return source;
            }
            catch
            {
                return null;
            }
        }
    }
}
