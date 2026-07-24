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

## Install

Copy the contents of `bin\Release` (the plugin DLL, the `Images` folder, and all the
ClosedXML dependency DLLs) into a plugin bundle folder:

```
%AppData%\Autodesk\ApplicationPlugins\Navisworks.Clash.Exporter.bundle\Contents\
```

The bundled dependencies are loaded at runtime by the `AssemblyLoader` plugin, so they must
sit next to the plugin DLL.

## Usage

In Navisworks Manage, open the **Export add-ins** ribbon tab and click **Export Clashes to
Excel**. Choose a destination `.xlsx` file and the workbook is written with two sheets.

### Summary sheet

One row per clash report, with:

| Column | Description |
| --- | --- |
| Clash Report | Name of the clash test |
| Status | Test status |
| Test Type | Hard / Clearance / Duplicate etc. |
| Tolerance (mm) | Test tolerance in millimetres |
| Last Run | When the test was last run |
| New / Active / Reviewed / Approved / Resolved | Clash counts per status |
| Total | Total clashes in the report |

### Clash Results sheet

One row per individual clash (grouped clashes are flattened), with:

| Column | Description |
| --- | --- |
| Clash Report | Name of the parent clash test |
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
