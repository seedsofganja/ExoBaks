# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

ExoCAD Tool is a small Windows Forms desktop app (C#, .NET Framework 4.7.2, old-style non-SDK `.csproj`) used by a dental lab to back up ExoCAD job folders into an organized archive. UI text, comments and identifiers are mostly in Italian; keep new code consistent with that. GitHub: https://github.com/seedsofganja/ExoBaks

## Build & test

Build with Visual Studio (open `ExoCAD Tool.sln`) or MSBuild from a Developer prompt (`/restore` is needed for the test project's NuGet packages):

```
msbuild "ExoCAD Tool.sln" /restore /p:Configuration=Debug
dotnet test "ExoCAD Tool.Tests/ExoCAD Tool.Tests.csproj"
```

Output: `ExoCAD Tool\bin\<Config>\ExoCAD Tool.exe`. `dotnet build` of the app project is not the intended path (legacy csproj targeting .NET Framework).

`ExoCAD Tool.Tests` is an SDK-style MSTest project (net472) that compiles `JobInfo.cs`, `JobReader.cs` and `BackupService.cs` directly via linked `<Compile>` items (no reference to the WinForms project). Test data must be invented — never real patient data.

CI: `.github/workflows/build.yml` builds and tests on push/PR; a `v*` tag publishes a GitHub Release with the zipped exe.

The project is also configured for ClickOnce web publishing (`PublishUrl`/`InstallUrl` in the csproj, pointing to 000webhost, which is likely dead); don't change those settings unless asked. `*.pfx` (signing key) is gitignored and must never be committed.

## Architecture

- `JobInfo.cs` — data of one job (folder, path, name, surname, studio, teeth string, date).
- `JobReader.cs` — reads a job from `<CAD-Data>\<Job>\`:
  - `<Job>.dentalProject`: `PatientFirstName`, `PatientName` (surname), `PracticeName`, `DateTime` (ISO; default = folder creation time).
  - `<Job>.constructionInfo`: `//Tooth/Number` for teeth. If missing (partial job), falls back to `//Teeth/Tooth` in the `.dentalProject`, skipping `ReconstructionType == "Antagonist"`.
  - `FormattaDenti` dedups, sorts and compresses teeth into ranges (e.g. `11-13 15 43`).
- `BackupService.cs` — no UI; copies a job into `<Backup>\<Year>\<Studio>\<Nome> <Cognome> <Denti> (<JobFolder>)\` (top-level files only — real ExoCAD jobs have no subfolders). Name parts are sanitized (`SanificaNome`). New backups are written to `<folder>.tmp` and renamed when complete; files go through `.part` and are recopied if size/mtime differ. An existing backup folder for the job is reused (optionally moved/renamed). Free space via `GetDiskFreeSpaceEx` (works on UNC). `ScansionaBackup` returns job → backup folder.
- `Form1.cs` — UI only (controls in `Form1.Designer.cs`, designer-generated — edit via the designer pattern). Loads the job list in background (`CaricaLavori`), filters (`PopolaGriglia`), single backup uses the **editable form fields**, full backup (`LoopBackUpButton_Click` → `BackupCompleto`) runs in `Task.Run` with progress and cancel, reading each job from XML and by default only backing up jobs not yet saved. The UI is disabled during operations (`InizioOperazione`/`FineOperazione`). Failures are appended to `<Backup>\backup_falliti_<session>.txt`.
- Paths are persisted in user settings (`Properties.Settings`: `WorksPath`, `BackUpPath`).

Key invariant: "backed up" detection depends on the backup folder name **ending** in `(<JobFolder>)`; `EstraiNomeLavoro` matches the balanced parentheses from the end (real job folder names can contain parentheses, e.g. `...(2025-11-03)`). Folders not ending in `)` (e.g. `.tmp`) are not considered backed up. Any change to the naming must keep this.
