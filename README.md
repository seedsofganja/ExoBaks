# ExoBaks — ExoCAD Tool

Piccola applicazione Windows Forms per il laboratorio odontotecnico: esegue il backup dei lavori ExoCAD (`CAD-Data`) in un archivio organizzato per anno, studio e paziente.

```
<Backup>\<Anno>\<Studio>\<Nome> <Cognome> <Denti> (<CartellaLavoro>)\
```

## Download

L'ultima versione compilata è nelle [Releases](../../releases): scarica `ExoCAD-Tool.zip`, estrai e avvia `ExoCAD Tool.exe`.

## Requisiti

- Windows con .NET Framework 4.7.2
- Per compilare: Visual Studio (oppure MSBuild con i reference assemblies 4.7.2)

## Compilazione e test

```
msbuild "ExoCAD Tool.sln" /restore /p:Configuration=Release
dotnet test "ExoCAD Tool.Tests/ExoCAD Tool.Tests.csproj" -c Release --no-build
```

L'eseguibile viene generato in `ExoCAD Tool\bin\Release\ExoCAD Tool.exe`.
La GitHub Action compila e testa a ogni push; creando un tag `v*` (es. `git tag v1.1.0 && git push origin v1.1.0`) pubblica automaticamente una Release.

## Utilizzo

1. **Seleziona Cartella CAD-Data** (es. `C:\OPT-Exocad\CAD-Data`) e **Seleziona Cartella BackUp**. I percorsi vengono ricordati ai successivi avvii; al primo avvio la cartella CAD-Data viene cercata automaticamente.
2. L'elenco mostra i lavori in **verde** (già salvati) o **rosso** (non salvati), con paziente, studio e data. Usa **Cerca** e **Mostra solo i non salvati** per filtrare.
3. **BackUp Singolo** salva il lavoro selezionato con i dati dei campi a destra, che si possono correggere a mano prima del salvataggio.
4. **BackUp Completo** salva tutti i lavori non ancora salvati (con la spunta **Aggiorna anche i lavori già salvati** sincronizza anche quelli esistenti). Si può interrompere con **Annulla**.

Gli errori vengono scritti in `backup_falliti_<data>.txt` nella cartella di backup (un file per sessione).

### Sicurezza dei backup

- Un backup nuovo viene scritto in una cartella `.tmp` e rinominato solo a copia completata: un backup interrotto non viene mai mostrato come salvato e riprende al giro successivo.
- I file vengono copiati tramite un file `.part`; un file già presente viene ricopiato se dimensione o data di modifica sono cambiate.
- Se un lavoro è già salvato, il backup aggiorna la cartella esistente invece di crearne una duplicata.
- Il riconoscimento dei lavori già salvati si basa sul nome della cartella di backup, che deve terminare con `(<CartellaLavoro>)`.

## Privacy (GDPR)

Le cartelle di backup e i log contengono nomi di pazienti, cioè dati sanitari. **Non committare mai** dati reali di `CAD-Data`, cartelle di backup o log in questo repository. I test usano solo dati inventati.
