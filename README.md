# ExoBaks — ExoCAD Tool

Piccola applicazione Windows Forms per il laboratorio odontotecnico: esegue il backup dei lavori ExoCAD (`CAD-Data`) in un archivio organizzato per anno, studio e paziente.

```
<Backup>\<Anno>\<Studio>\<Nome> <Cognome> <Denti> (<CartellaLavoro>)\
```

## Requisiti

- Windows con .NET Framework 4.7.2
- Visual Studio (oppure MSBuild con i reference assemblies 4.7.2) per compilare

## Compilazione

```
msbuild "ExoCAD Tool.sln" /p:Configuration=Release
```

L'eseguibile viene generato in `ExoCAD Tool\bin\Release\ExoCAD Tool.exe`.

## Utilizzo

1. **Seleziona Cartella CAD-Data** (di solito `C:\OPT-Exocad\CAD-Data`): l'elenco mostra i lavori in verde (già salvati) o in rosso (non salvati).
2. **Seleziona Cartella BackUp**: scegli la cartella di destinazione.
3. **BackUp Singolo** salva il lavoro selezionato. **BackUp Completo** li salva tutti; gli errori vengono scritti in `backup_falliti_<data>.txt` nella cartella di backup.

Il riconoscimento dei lavori già salvati si basa sul nome della cartella di backup, che termina con `(<CartellaLavoro>)`.

## Privacy (GDPR)

Le cartelle di backup e i log contengono nomi di pazienti, cioè dati sanitari. **Non committare mai** dati reali di `CAD-Data`, cartelle di backup o log in questo repository.
