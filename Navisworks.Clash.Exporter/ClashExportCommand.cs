using System;
using System.Windows.Forms;
using Autodesk.Navisworks.Api.Clash;
using Application = Autodesk.Navisworks.Api.Application;

namespace Navisworks.Clash.Exporter
{
    /// <summary>
    /// The "Export Clashes to Excel" action, invoked from the RME Tools ribbon.
    /// Kept separate from the ribbon plugin so future RME tools can reuse or sit alongside it.
    /// </summary>
    public static class ClashExportCommand
    {
        public static int Run()
        {
            try
            {
                var clash = Application.MainDocument.GetClash();
                if (clash.TestsData.Tests.Count == 0)
                {
                    MessageBox.Show("There are no clash tests in this document to export.",
                        "Export Clashes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return 0;
                }

                string fileName;
                using (var dialog = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                    OverwritePrompt = true,
                    FileName = $"{Application.MainDocument.Title}_Clashes.xlsx"
                })
                {
                    if (dialog.ShowDialog() != DialogResult.OK) return 0;
                    fileName = dialog.FileName;
                }

                var progress = Application.BeginProgress("Reading clash results from the document...");

                var data = ClashExtractor.Extract(clash.TestsData);

                progress.Update(0.85);
                ExcelExporter.Save(data, fileName);

                Application.EndProgress();

                MessageBox.Show(
                    $"Exported {data.Clashes.Count} clashes from {data.Summary.Count} clash reports to:\n{fileName}",
                    "Export Clashes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n\n{e.StackTrace}", "Export Clashes",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            return 1;
        }
    }
}
