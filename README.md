# Navisworks Clash Exporter (2026)

A Navisworks Manage **2026** add-in that exports the clash tests ("clash reports") in the
currently open document to a formatted Excel (`.xlsx`) workbook.

## Requirements

- Autodesk Navisworks Manage 2026 (references the API DLLs from its install folder).
- .NET Framework 4.8 (installed with Navisworks 2026).

## Build

```bash
dotnet build Navisworks.Clash.Exporter/Navisworks.Clash.Exporter.csproj -c Release
```

The build references the Navisworks API assemblies from
`C:\Program Files\Autodesk\Navisworks Manage 2026`. If Navisworks is installed elsewhere,
override the path:

```bash
dotnet build -c Release /p:NavisworksPath="D:\Autodesk\Navisworks Manage 2026"
```

The build produces a **single self-contained DLL**,
`bin\Release\Navisworks.Clash.Exporter.dll`. ClosedXML and every one of its dependencies are
merged into that one file by [ILRepack](https://github.com/gluck/il-repack), and the two ribbon
icons are embedded as resources — there are no loose dependency DLLs and no `Images` folder to
ship. (The accompanying `.pdb` is optional debug symbols and is not needed at runtime.)

## Install

Drop the single DLL into a folder named after it inside the Navisworks `Plugins` directory —
the folder name must match the DLL name:

```
C:\Program Files\Autodesk\Navisworks Manage 2026\Plugins\Navisworks.Clash.Exporter\Navisworks.Clash.Exporter.dll
```

Restart Navisworks and the add-in appears on the **Export add-ins** ribbon tab. That is the only
file you need to copy.

## Usage

In Navisworks Manage, open the **RME Tools** ribbon tab and, in the **Exports** panel, click
**Export Clashes to Excel**. Choose a destination `.xlsx` file and a single workbook is written
with a single **Clash Results** sheet holding every clash from every clash test in the document.

> The add-in registers its own **RME Tools** ribbon tab (a `CommandHandlerPlugin` with the ribbon
> layout in the embedded `RmeToolsRibbon.xaml`) rather than sitting in the generic add-ins tab.
> Future RME tools can be added under this same tab — see the extension notes in
> `RmeToolsRibbon.cs`.

### Clash Results sheet

One row per individual clash — grouped clashes are flattened alongside ungrouped ones, and every
clash test in the document lands on this one sheet. The test-level attributes are repeated on each
row, so no second sheet is needed. A clash test that found nothing still gets a row, with the
clash columns left blank.

| Column | Description |
| --- | --- |
| Clash Report | Name of the parent clash test |
| Test Status | Status of the clash test |
| Test Type | Hard / Clearance / Duplicate etc. |
| Tolerance (mm) | Test tolerance in millimetres |
| Last Run | When the test was last run |
| Group | Name of the clash group, if the clash belongs to one |
| Clash Name | Name of the clash |
| Priority | Clash priority value (as shown in Clash Detective) |
| Status | New / Active / Reviewed / Approved / Resolved |
| Distance (mm) | Overlap / clearance distance in millimetres |
| Description | Clash description |
| Date Found | When the clash was first created |
| Assigned To | Assignee display name |
| Approved By / Approved Time | Approval details |
| Level | Closest level to the clash centre |
| Grid Intersection | Closest grid intersection to the clash centre |
| Location (X, Y, Z) | Coordinates of the clash centre |
| Item 1 / Item 2 Name, Id, Source File | The two clashing elements |
| Comments | Clash comments joined with " \| " |
| Clash Guid | Unique clash identifier |
