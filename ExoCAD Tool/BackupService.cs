using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ExoCAD_Tool
{
    public class SpazioEsauritoException : IOException
    {
        public SpazioEsauritoException() : base("Spazio sul disco di backup esaurito") { }
    }

    // Copia dei lavori nell'archivio di backup:
    //   <Backup>\<Anno>\<Studio>\<Nome> <Cognome> <Denti> (<CartellaLavoro>)\
    // Il nome della cartella deve terminare con "(<CartellaLavoro>)": è così che
    // ScansionaBackup riconosce i lavori già salvati.
    public static class BackupService
    {
        public const string MotivoDatiMancanti = "Dati mancanti (studio/denti/paziente)";

        // Restituisce null se il lavoro ha i dati minimi per il backup, altrimenti il motivo.
        // Il backup è permesso anche se nome o cognome mancano (ma almeno uno dei due serve).
        public static string Valida(JobInfo job)
        {
            if (SanificaNome(job.Studio) == "" ||
                SanificaNome(job.Denti) == "" ||
                (SanificaNome(job.Nome) == "" && SanificaNome(job.Cognome) == ""))
                return MotivoDatiMancanti;

            return null;
        }

        public static string CartellaDestinazione(string backupRoot, JobInfo job)
        {
            string nomeCartella = $"{SanificaNome(job.Nome)} {SanificaNome(job.Cognome)} {SanificaNome(job.Denti)} ({job.CartellaLavoro})";
            return Path.Combine(backupRoot, job.Data.Year.ToString(), SanificaNome(job.Studio), nomeCartella);
        }

        // -----------------------------------------------------------------------
        // Esegue il backup del lavoro e restituisce la cartella finale.
        // cartellaEsistente: backup già presente per questo lavoro, che viene
        // aggiornato invece di crearne un duplicato (anche se nel frattempo sono
        // cambiati denti o nome). Con sposta = true viene prima spostato e
        // rinominato secondo i dati attuali del lavoro.
        // -----------------------------------------------------------------------
        public static string Esegui(JobInfo job, string backupRoot, string cartellaEsistente, bool sposta, CancellationToken ct)
        {
            string desiderata = CartellaDestinazione(backupRoot, job);
            string destinazione = desiderata;

            if (cartellaEsistente != null && Directory.Exists(cartellaEsistente))
            {
                destinazione = cartellaEsistente;

                if (sposta && !Directory.Exists(desiderata))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(desiderata));
                    Directory.Move(cartellaEsistente, desiderata);
                    destinazione = desiderata;
                }
            }

            // Un backup nuovo viene scritto in "<cartella>.tmp" e rinominato solo a
            // copia completata: se si interrompe, il lavoro resta "non salvato"
            // (ScansionaBackup ignora le cartelle che non terminano con ')').
            bool cartellaNuova = !Directory.Exists(destinazione);
            string cartellaScrittura = cartellaNuova ? destinazione + ".tmp" : destinazione;

            Directory.CreateDirectory(cartellaScrittura);

            // --- Copia i file uno per uno ---
            foreach (string file in Directory.GetFiles(job.PercorsoLavoro))
            {
                ct.ThrowIfCancellationRequested();

                string destPath = Path.Combine(cartellaScrittura, Path.GetFileName(file));

                if (FileGiaCopiato(file, destPath))
                    continue; // già copiato e invariato, salta

                long fileSize = new FileInfo(file).Length;
                long freeSpace = SpazioLibero(backupRoot); // riletto ad ogni file, -1 se non disponibile

                if (freeSpace >= 0 && freeSpace <= fileSize)
                    throw new SpazioEsauritoException();

                CopiaFileSicura(file, destPath);
            }

            if (cartellaNuova)
                Directory.Move(cartellaScrittura, destinazione);

            return destinazione;
        }

        // -----------------------------------------------------------------------
        // Scansiona la cartella di backup e restituisce i lavori già salvati:
        // nome cartella lavoro -> percorso della cartella di backup.
        // Salta le cartelle di sistema (es. "System Volume Information" sulla
        // radice di un disco) e quelle non accessibili.
        // -----------------------------------------------------------------------
        public static Dictionary<string, string> ScansionaBackup(string backupRoot)
        {
            var lavoriSalvati = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(backupRoot) && Directory.Exists(backupRoot))
                ScansionaCartella(new DirectoryInfo(backupRoot), lavoriSalvati);

            return lavoriSalvati;
        }

        private static void ScansionaCartella(DirectoryInfo dir, Dictionary<string, string> lavoriSalvati)
        {
            List<DirectoryInfo> sottoCartelle;
            try
            {
                sottoCartelle = dir.EnumerateDirectories().ToList();
            }
            catch (UnauthorizedAccessException) { return; }
            catch (IOException) { return; }

            foreach (DirectoryInfo sub in sottoCartelle)
            {
                if ((sub.Attributes & FileAttributes.System) != 0)
                    continue;

                string workName = EstraiNomeLavoro(sub.Name);
                if (workName == null)
                    ScansionaCartella(sub, lavoriSalvati);
                else if (!lavoriSalvati.ContainsKey(workName))
                    lavoriSalvati.Add(workName, sub.FullName); // cartella paziente: non serve scendere oltre
            }
        }

        // -----------------------------------------------------------------------
        // Il nome cartella ha il formato: "Nome Cognome Denti (NomeLavoro)".
        // Restituisce il NomeLavoro contenuto nelle parentesi FINALI, cercando la
        // parentesi aperta corrispondente (così funziona anche se il nome del
        // lavoro contiene a sua volta delle parentesi). Le cartelle che non
        // terminano con ')' (es. backup incompleti "... (Lavoro).tmp") restituiscono null.
        // -----------------------------------------------------------------------
        public static string EstraiNomeLavoro(string dirName)
        {
            if (!dirName.EndsWith(")"))
                return null;

            int profondita = 0;
            for (int i = dirName.Length - 1; i >= 0; i--)
            {
                if (dirName[i] == ')') profondita++;
                else if (dirName[i] == '(') profondita--;

                if (profondita == 0)
                    return dirName.Substring(i + 1, dirName.Length - i - 2).Trim();
            }
            return null;
        }

        // -----------------------------------------------------------------------
        // Rende un testo valido come nome di cartella: sostituisce i caratteri non
        // ammessi da Windows con '-' e toglie spazi e punti finali.
        // -----------------------------------------------------------------------
        public static string SanificaNome(string testo)
        {
            char[] nonValidi = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder();
            foreach (char c in testo ?? "")
                sb.Append(nonValidi.Contains(c) ? '-' : c);

            return sb.ToString().Trim().TrimEnd('.', ' ');
        }

        // -----------------------------------------------------------------------
        // Un file è considerato già copiato solo se esiste con stessa dimensione e
        // stessa data di modifica (tolleranza 2s per dischi FAT/exFAT): così i file
        // copiati a metà o modificati in ExoCAD dopo il backup vengono ricopiati.
        // -----------------------------------------------------------------------
        public static bool FileGiaCopiato(string origine, string destinazione)
        {
            if (!File.Exists(destinazione))
                return false;

            var o = new FileInfo(origine);
            var d = new FileInfo(destinazione);
            return o.Length == d.Length &&
                   Math.Abs((o.LastWriteTimeUtc - d.LastWriteTimeUtc).TotalSeconds) <= 2;
        }

        // -----------------------------------------------------------------------
        // Copia su un file ".part" e lo rinomina solo a copia completata: un file
        // interrotto a metà non viene mai scambiato per un file valido.
        // -----------------------------------------------------------------------
        private static void CopiaFileSicura(string origine, string destinazione)
        {
            string temp = destinazione + ".part";

            if (File.Exists(temp))
                File.SetAttributes(temp, FileAttributes.Normal);
            File.Copy(origine, temp, true);

            if (File.Exists(destinazione))
            {
                File.SetAttributes(destinazione, FileAttributes.Normal);
                File.Delete(destinazione);
            }
            File.Move(temp, destinazione);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
            out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        // -----------------------------------------------------------------------
        // Spazio libero sul disco che contiene la cartella. Funziona anche con
        // percorsi di rete (\\NAS\backup), dove DriveInfo lancia un'eccezione.
        // Restituisce -1 se non è possibile determinarlo.
        // -----------------------------------------------------------------------
        public static long SpazioLibero(string cartella)
        {
            string percorso = cartella.EndsWith("\\") ? cartella : cartella + "\\";

            if (GetDiskFreeSpaceEx(percorso, out ulong libero, out _, out _))
                return (long)Math.Min(libero, (ulong)long.MaxValue);

            return -1;
        }
    }
}
