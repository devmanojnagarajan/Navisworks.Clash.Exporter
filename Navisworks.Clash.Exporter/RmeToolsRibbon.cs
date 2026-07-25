using Autodesk.Navisworks.Api.Plugins;
using Application = Autodesk.Navisworks.Api.Application;

namespace Navisworks.Clash.Exporter
{
    /// <summary>
    /// Adds the custom "RME Tools" ribbon tab to Navisworks and routes its commands.
    ///
    /// The tab, its "Exports" panel, and the buttons are laid out in en-US\RmeToolsRibbon.xaml,
    /// a loose file deployed next to the DLL and referenced by <see cref="RibbonLayoutAttribute"/>.
    /// Navisworks requires this layout to be a loose file inside a culture sub-folder (en-US), not
    /// an embedded resource. To add a future RME tool, register another [Command(...)] here, add a
    /// matching &lt;RibbonButton&gt; to the XAML (in the "Exports" panel or a new &lt;RibbonPanel&gt;),
    /// and handle its id in <see cref="ExecuteCommand"/>.
    /// </summary>
    [Plugin("Navisworks.Clash.Exporter.RmeTools", "RME",
        DisplayName = "RME Tools",
        ToolTip = "RME Tools")]
    [RibbonLayout("RmeToolsRibbon")]
    [RibbonTab("ID_RmeTools", DisplayName = "RME Tools", LoadForCanExecute = true)]
    [Command("ID_ExportClashesToExcel",
        DisplayName = "Export Clashes to Excel",
        ToolTip = "Export every clash test in the current document to an Excel workbook.",
        Icon = "Navisworks.Clash.Exporter.Images.Icon_16.png",
        LargeIcon = "Navisworks.Clash.Exporter.Images.Icon_32.png")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RmeToolsRibbon : CommandHandlerPlugin
    {
        public override int ExecuteCommand(string name, params string[] parameters)
        {
            switch (name)
            {
                case "ID_ExportClashesToExcel":
                    return ClashExportCommand.Run();
            }

            return 0;
        }

        public override CommandState CanExecuteCommand(string name)
        {
            switch (name)
            {
                case "ID_ExportClashesToExcel":
                    var doc = Application.ActiveDocument;
                    return new CommandState(doc != null && !doc.IsClear);
            }

            return new CommandState(true);
        }

        public override bool CanExecuteRibbonTab(string name)
        {
            return true;
        }
    }
}
