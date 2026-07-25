using Autodesk.Navisworks.Api.Plugins;

namespace Navisworks.Clash.Exporter
{
    /// <summary>
    /// Fallback entry point: puts "Export Clashes to Excel" in the standard Export add-ins area.
    /// This guarantees the tool is reachable even if the custom RME Tools ribbon
    /// (<see cref="RmeToolsRibbon"/>) fails to load. Both call the same <see cref="ClashExportCommand"/>.
    /// </summary>
    [Plugin("Navisworks.Clash.Exporter.Export", "COOL", DisplayName = "Export Clashes to Excel")]
    [AddInPlugin(AddInLocation.Export, LoadForCanExecute = true,
        CallCanExecute = CallCanExecute.DocumentNotClear,
        Icon = "Navisworks.Clash.Exporter.Images.Icon_16.png",
        LargeIcon = "Navisworks.Clash.Exporter.Images.Icon_32.png")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Export : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            return ClashExportCommand.Run();
        }
    }
}
