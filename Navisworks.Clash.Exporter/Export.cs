using System;
using System.Windows.Forms;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.Plugins;
using Application = Autodesk.Navisworks.Api.Application;

namespace Navisworks.Clash.Exporter
{
    [Plugin("Navisworks.Clash.Exporter.Export", "COOL", DisplayName = "Export Clashes to Excel")]
    [AddInPlugin(AddInLocation.Export, LoadForCanExecute = true,
        CallCanExecute = CallCanExecute.DocumentNotClear,
        Icon = "Images\\Icon_16.png", LargeIcon = "Images\\Icon_32.png")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Export : AddInPlugin
    {
        public override int Execute(params string[] parameters)
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
