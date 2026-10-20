# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

ExoCAD Tool is a small Windows Forms desktop app (C#, .NET Framework 4.7.2, old-style non-SDK `.csproj`) used by a dental lab to back up ExoCAD job folders into an organized archive. UI text, comments and identifiers are mostly in Italian; keep new code consistent with that.

## Build & run

There are no tests and no linter. Build with Visual Studio (open `ExoCAD Tool.sln`) or MSBuild from a Developer prompt:

```
msbuild "ExoCAD Tool.sln" /p:Configuration=Debug
msbuild "ExoCAD Tool.sln" /p:Configuration=Release
```

Output: `ExoCAD Tool\bin\<Config>\ExoCAD Tool.exe`. `dotnet build` is not the intended path (legacy csproj targeting .NET Framework; requires the 4.7.2 reference assemblies).

The project is configured for ClickOnce web publishing (`PublishUrl`/`InstallUrl` in the csproj, signed with `ExoCAD Tool_TemporaryKey.pfx`); don't change those settings unless asked.

## Architecture

All logic lives in `ExoCAD Tool/Form1.cs` (controls declared in `Form1.Designer.cs`, which is designer-generated — edit control layout via the designer pattern, not ad hoc). `Program.cs` just launches `Form1`.

Flow:
1. **`CaricaLavori()`** — lists every subfolder of the ExoCAD `CAD-Data` path (`WorksPathTextBox`, typically `C:\OPT-Exocad\CAD-Data`) into `MyWorksDataGridView`. It scans the backup path once into a `HashSet` of job names and colors each row green (backed up) / light coral (not backed up), then updates the counters in `label1`/`label2`.
2. **`LoadSelectedWork()`** — on grid selection, reads the job's XML files in `<CAD-Data>\<Job>\`:
   - `<Job>.dentalProject`: `PatientFirstName`, `PatientName` (surname), `PracticeName`, `DateTime`.
   - `<Job>.constructionInfo`: `//Tooth/Number` for teeth. If missing (partial job), falls back to `//Teeth/Tooth` in the `.dentalProject`, skipping `ReconstructionType == "Antagonist"`.
   - Teeth are sorted and compressed into a range string (e.g. `11-13 15 43`) shown in the `Denti` textbox. Fields are editable by the user before backup.
3. **`DoBackUp(isLoopMode)`** — copies the job's top-level files (not subfolders) into
   `<Backup>\<Year>\<Studio>\<Nome> <Cognome> <Denti> (<JobFolder>)\`, using the **form field values** (possibly user-edited), not the XML directly. Existing destination files are skipped; free disk space is checked per file. Requires studio, teeth, and at least one of name/surname.
4. **`LoopBackUpButton_Click`** — iterates every grid row, selecting it (which triggers `LoadSelectedWork` via `SelectionChanged`) and calling `DoBackUp(isLoopMode: true)`; the grid is reloaded only once at the end. Failures are accumulated via `RegistraFallimento` and written by `ScriviLogFalliti` to `<Backup>\backup_falliti_<timestamp>.txt`.

Key invariant: "backed up" detection depends on the backup folder name ending in `(<JobFolder>)` — the text inside the **last** parentheses is the ExoCAD job folder name. Any change to the backup folder naming must keep this, or `CaricaLavori` will mark jobs as not saved.

Notes:
- `Form1.Vars` is a static holder for the currently loaded job's data.
- Paths (`WorksPathTextBox`, `BackUpPath`) are not persisted between runs (`Settings.settings` is empty).
- `IsWorkBackedUp()` is unused (superseded by the `HashSet` approach in `CaricaLavori`).
