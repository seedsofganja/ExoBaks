using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace ExoCAD_Tool
{

    public partial class Form1 : Form
    {
        public static class Vars
        {
            public static string nl = Environment.NewLine;
            public static string[] WokrsArray;
            public static string NomePaziente;
            public static string CognomePaziente;
            public static string StudioDentistico;
            public static DateTime dateTime = DateTime.Now;
            public static int Year;
            public static List<int> Teeth = new List<int>();
            public static string TeethString;
           
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                      
        }

        private void ChoosePathButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    WorksPathTextBox.Text = fbd.SelectedPath;
                    CaricaLavori();
                }
            }
        }

        public void CaricaLavori()
        {
            // --- Validazioni iniziali ---
            if (string.IsNullOrWhiteSpace(WorksPathTextBox.Text))
            {
                MessageBox.Show(
                    "Seleziona il percorso CAD-Data dei lavori di ExoCAD, di solito in \"C:\\OPT-Exocad\\CAD-Data\"",
                    "Percorso non specificato");
                return;
            }

            if (!Directory.Exists(WorksPathTextBox.Text))
            {
                MessageBox.Show(
                    "Il percorso specificato non esiste:\n" + WorksPathTextBox.Text,
                    "Percorso non trovato");
                return;
            }

            MyWorksDataGridView.Rows.Clear();
            MyWorksDataGridView.Refresh();

            try
            {
                // -----------------------------------------------------------------------
                // OTTIMIZZAZIONE: invece di cercare su disco per ogni lavoro (lentissimo
                // con tante cartelle), leggiamo TUTTE le sottocartelle del backup UNA SOLA
                // volta e le mettiamo in un HashSet in memoria. La ricerca diventa O(1).
                // -----------------------------------------------------------------------
                var backuppedWorks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(BackUpPath.Text) && Directory.Exists(BackUpPath.Text))
                    ScansionaBackup(new DirectoryInfo(BackUpPath.Text), backuppedWorks);

                // -----------------------------------------------------------------------
                // Popola la griglia e colora ogni riga in base allo stato di backup
                // -----------------------------------------------------------------------
                string worksDir = WorksPathTextBox.Text;

                foreach (string d in Directory.GetDirectories(worksDir))
                {
                    string workName = Path.GetFileName(d);
                    MyWorksDataGridView.Rows.Add(workName);

                    DataGridViewRow row = MyWorksDataGridView.Rows[MyWorksDataGridView.Rows.Count - 1];

                    bool salvato = backuppedWorks.Contains(workName); // ricerca in memoria, istantanea

                    row.DefaultCellStyle.BackColor = salvato ? Color.LightGreen : Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento dei lavori:\n" + ex.Message, "Errore");
            }

            // -----------------------------------------------------------------------
            // Aggiorna i contatori in fondo al form
            // -----------------------------------------------------------------------
            int totale = MyWorksDataGridView.Rows.Count;
            int nonSalvati = MyWorksDataGridView.Rows
                .Cast<DataGridViewRow>()
                .Count(r => r.DefaultCellStyle.BackColor == Color.LightCoral);

            label1.ForeColor = Color.Black;
            label1.Text = $"Lavori trovati: {totale}";

            label2.ForeColor = nonSalvati > 0 ? Color.Red : Color.Green;
            label2.Text = nonSalvati > 0
                ? $"⚠ {nonSalvati} lavori non salvati"
                : "✔ Tutti i lavori sono salvati";
        }

        // -----------------------------------------------------------------------
        // Scansiona ricorsivamente la cartella di backup raccogliendo i nomi dei
        // lavori salvati. Salta le cartelle di sistema (es. "System Volume
        // Information" sulla radice di un disco) e quelle non accessibili.
        // -----------------------------------------------------------------------
        private static void ScansionaBackup(DirectoryInfo dir, HashSet<string> lavoriSalvati)
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
                if (workName != null)
                    lavoriSalvati.Add(workName); // cartella paziente: non serve scendere oltre
                else
                    ScansionaBackup(sub, lavoriSalvati);
            }
        }

        // -----------------------------------------------------------------------
        // Il nome cartella ha il formato: "Nome Cognome Denti (NomeLavoro)".
        // Restituisce il NomeLavoro contenuto nelle parentesi FINALI, cercando la
        // parentesi aperta corrispondente (così funziona anche se il nome del
        // lavoro contiene a sua volta delle parentesi). Le cartelle che non
        // terminano con ')' (es. backup incompleti "... (Lavoro).tmp") restituiscono null.
        // -----------------------------------------------------------------------
        private static string EstraiNomeLavoro(string dirName)
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

        private void LoadWorksButton_Click(object sender, EventArgs e)
        {

                CaricaLavori();

        }
        public void ClearForm()
        {
            Denti.Text = "";
            NomePaziente.Text = ""; ;
            CognomePaziente.Text = ""; ;
            StudioDentistico.Text = ""; ;
            dateTimePicker1.Value = DateTime.Now;
        }
        public void LoadSelectedWork()
        {
            try
            {
                label1.Text = "";
                Vars.TeethString = null;
                Vars.NomePaziente = null;
                Vars.CognomePaziente = null;
                Vars.StudioDentistico = null;

                if (MyWorksDataGridView.Rows.Count < 1 || MyWorksDataGridView.SelectedRows.Count != 1)
                    return;

                string dP = ".dentalProject";
                string cI = ".constructionInfo";
                string ProjectName = MyWorksDataGridView.SelectedRows[0].Cells[0].Value.ToString();
                string path = WorksPathTextBox.Text + "\\" + ProjectName;
                string dentalProject = path + "\\" + ProjectName + dP;
                string constructionInfo = path + "\\" + ProjectName + cI;

                XmlDocument xml = new XmlDocument();

                // Data di default = creazione della cartella del lavoro, così un lavoro
                // senza DateTime non eredita la data (e l'anno) del lavoro precedente
                Vars.dateTime = Directory.GetCreationTime(path);
                Vars.Year = Vars.dateTime.Year;

                // DentalProject — carica sempre nome, cognome, studio, data
                if (File.Exists(dentalProject))
                {
                    xml.Load(dentalProject);

                    XmlNode xmlNode;

                    xmlNode = xml.SelectSingleNode("//PatientFirstName");
                    Vars.NomePaziente = xmlNode?.InnerText ?? "";

                    xmlNode = xml.SelectSingleNode("//PatientName");
                    Vars.CognomePaziente = xmlNode?.InnerText ?? "";

                    xmlNode = xml.SelectSingleNode("//PracticeName");
                    Vars.StudioDentistico = xmlNode?.InnerText ?? "";

                    xmlNode = xml.SelectSingleNode("//DateTime");
                    if (xmlNode != null &&
                        (DateTime.TryParse(xmlNode.InnerText, out DateTime dt) ||
                         DateTime.TryParse(xmlNode.InnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)))
                    {
                        Vars.dateTime = dt;
                        Vars.Year = dt.Year;
                    }
                }
                else
                {
                    Vars.NomePaziente = "";
                    Vars.CognomePaziente = "";
                    Vars.StudioDentistico = "";
                }

                // Denti — prima provo .constructionInfo, altrimenti leggo dal .dentalProject già caricato
                if (File.Exists(constructionInfo))
                {
                    XmlDocument xmlCI = new XmlDocument();
                    xmlCI.Load(constructionInfo);

                    var teeth = xmlCI.SelectNodes("//Tooth/Number");
                    Vars.Teeth.Clear();
                    foreach (XmlNode node in teeth)
                    {
                        if (int.TryParse(node.InnerText, out int t))
                            Vars.Teeth.Add(t);
                    }
                }
                else
                {
                    // Lavoro parziale: leggo i denti dal .dentalProject già caricato, escludo gli Antagonist
                    var teeth = xml.SelectNodes("//Teeth/Tooth");
                    Vars.Teeth.Clear();
                    foreach (XmlNode node in teeth)
                    {
                        XmlNode recType = node.SelectSingleNode("ReconstructionType");
                        if (recType != null && recType.InnerText == "Antagonist") continue;

                        XmlNode numNode = node.SelectSingleNode("Number");
                        if (numNode != null && int.TryParse(numNode.InnerText, out int t))
                            Vars.Teeth.Add(t);
                    }
                }

                // Rimuove i duplicati, ordina e costruisce la stringa range (es. "11-13 15 43")
                Vars.Teeth = Vars.Teeth.Distinct().OrderBy(t => t).ToList();
                string teethString = "";
                int count = Vars.Teeth.Count;
                int i = 0;
                while (i < count)
                {
                    int start = Vars.Teeth[i];
                    int end = start;
                    while (i + 1 < count && Vars.Teeth[i + 1] == Vars.Teeth[i] + 1)
                    {
                        end = Vars.Teeth[i + 1];
                        i++;
                    }
                    if (teethString != "") teethString += " ";
                    if (end > start)
                        teethString += start + "-" + end;
                    else
                        teethString += start;
                    i++;
                }
                Vars.TeethString = teethString;

                // Aggiorna i campi del form
                Denti.Text = Vars.TeethString;
                NomePaziente.Text = Vars.NomePaziente;
                CognomePaziente.Text = Vars.CognomePaziente;
                StudioDentistico.Text = Vars.StudioDentistico;
                dateTimePicker1.Value = Vars.dateTime;
            }
            catch (Exception ex)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Errore nel caricamento: " + ex.Message;
            }
        }
        private void LoadSelectedWorkButton_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadSelectedWork();
        }

        private void ChooseBackUpFolderButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    BackUpPath.Text = fbd.SelectedPath;
                    CaricaLavori(); // aggiorna i colori
                }
            }
        }
        // Variabile di classe per accumulare i fallimenti durante il loop
        private readonly List<string> _backupFalliti = new List<string>();

        // Impostata da DoBackUp in modalità loop quando il disco di backup è pieno:
        // il loop si interrompe invece di proseguire con tutti i lavori rimanenti
        private bool _spazioEsaurito;

        public void DoBackUp(bool isLoopMode = false)
        {
            try
            {
                // --- Validazioni ---
                if (string.IsNullOrWhiteSpace(BackUpPath.Text))
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "Cartella di backup non specificata";
                    return;
                }

                if (MyWorksDataGridView.SelectedRows.Count < 1)
                {
                    label1.ForeColor = Color.Red;
                    label1.Text = "Nessun lavoro selezionato";
                    return;
                }

                // Legge i campi dal form (potrebbero essere stati modificati manualmente)
                // e li rende validi come nomi di cartella (es. "Rossi/Bianchi" -> "Rossi-Bianchi")
                string nome = SanificaNome(NomePaziente.Text);
                string cognome = SanificaNome(CognomePaziente.Text);
                string studio = SanificaNome(StudioDentistico.Text);
                string denti = SanificaNome(Denti.Text);
                DateTime data = dateTimePicker1.Value;
                string workFolder = MyWorksDataGridView.SelectedRows[0].Cells[0].Value.ToString();

                // Permette il backup anche se nome o cognome mancano (ma almeno uno dei due serve)
                if (string.IsNullOrWhiteSpace(studio) ||
                    string.IsNullOrWhiteSpace(denti) ||
                    (string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(cognome)))
                {
                    string motivo = "Dati mancanti (studio/denti/paziente)";
                    label1.ForeColor = Color.Red;
                    label1.Text = $"Impossibile salvare: {motivo}";

                    // Registra il fallimento con i dati disponibili
                    RegistraFallimento(workFolder, nome, cognome, denti, data, motivo);
                    return;
                }

                // --- Costruisce la struttura di cartelle ---
                string workPath = WorksPathTextBox.Text + "\\" + workFolder;
                string yearFolder = Path.Combine(BackUpPath.Text, data.Year.ToString());
                string studioFolder = Path.Combine(yearFolder, studio);
                string pazienteFolder = Path.Combine(
                    studioFolder,
                    $"{nome} {cognome} {denti} ({workFolder})"
                );

                // Un backup nuovo viene scritto in "<cartella>.tmp" e rinominato solo a
                // copia completata: se si interrompe, il lavoro resta "non salvato"
                // (CaricaLavori ignora le cartelle che non terminano con ')').
                // Se la cartella esiste già, si aggiornano i file al suo interno.
                bool cartellaNuova = !Directory.Exists(pazienteFolder);
                string cartellaDestinazione = cartellaNuova ? pazienteFolder + ".tmp" : pazienteFolder;

                Directory.CreateDirectory(cartellaDestinazione);

                // --- Copia i file uno per uno ---
                foreach (string file in Directory.GetFiles(workPath))
                {
                    string destPath = Path.Combine(cartellaDestinazione, Path.GetFileName(file));

                    if (FileGiaCopiato(file, destPath))
                        continue; // già copiato e invariato, salta

                    long fileSize = new FileInfo(file).Length;
                    long freeSpace = SpazioLibero(BackUpPath.Text); // riletto ad ogni file, -1 se non disponibile

                    if (freeSpace >= 0 && freeSpace <= fileSize)
                    {
                        string motivo = "Spazio sul disco di backup esaurito";
                        RegistraFallimento(workFolder, nome, cognome, denti, data, motivo);

                        if (isLoopMode)
                            _spazioEsaurito = true; // il messaggio lo mostra il loop, una volta sola
                        else
                            MessageBox.Show(motivo, "Spazio insufficiente");

                        label1.ForeColor = Color.Red;
                        label1.Text = motivo;
                        return;
                    }

                    CopiaFileSicura(file, destPath);
                }

                if (cartellaNuova)
                    Directory.Move(cartellaDestinazione, pazienteFolder);

                label1.ForeColor = Color.Green;
                label1.Text = "Lavoro salvato";

                // In modalità loop non ricarichiamo la griglia ad ogni passo (troppo lento):
                // lo farà LoopBackUpButton_Click alla fine
                if (!isLoopMode)
                    CaricaLavori();
            }
            catch (Exception ex)
            {
                string motivo = ex.Message;
                label1.ForeColor = Color.Red;
                label1.Text = "Errore backup: " + motivo;

                // Tentiamo di registrare il fallimento anche in caso di eccezione
                try
                {
                    string workFolder = MyWorksDataGridView.SelectedRows.Count > 0
                        ? MyWorksDataGridView.SelectedRows[0].Cells[0].Value.ToString()
                        : "Sconosciuto";

                    RegistraFallimento(
                        workFolder,
                        NomePaziente.Text.Trim(),
                        CognomePaziente.Text.Trim(),
                        Denti.Text.Trim(),
                        dateTimePicker1.Value,
                        motivo
                    );
                }
                catch { /* ignora errori secondari nel log */ }
            }
        }

        // -----------------------------------------------------------------------
        // Rende un testo valido come nome di cartella: sostituisce i caratteri non
        // ammessi da Windows con '-' e toglie spazi e punti finali.
        // -----------------------------------------------------------------------
        private static string SanificaNome(string testo)
        {
            char[] nonValidi = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(testo.Length);
            foreach (char c in testo)
                sb.Append(nonValidi.Contains(c) ? '-' : c);

            return sb.ToString().Trim().TrimEnd('.', ' ');
        }

        // -----------------------------------------------------------------------
        // Un file è considerato già copiato solo se esiste con stessa dimensione e
        // stessa data di modifica (tolleranza 2s per dischi FAT/exFAT): così i file
        // copiati a metà o modificati in ExoCAD dopo il backup vengono ricopiati.
        // -----------------------------------------------------------------------
        private static bool FileGiaCopiato(string origine, string destinazione)
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
        private static long SpazioLibero(string cartella)
        {
            string percorso = cartella.EndsWith("\\") ? cartella : cartella + "\\";

            if (GetDiskFreeSpaceEx(percorso, out ulong libero, out _, out _))
                return (long)Math.Min(libero, (ulong)long.MaxValue);

            return -1;
        }

        // -----------------------------------------------------------------------
        // Aggiunge una riga alla lista dei fallimenti (usata poi da ScriviLogFalliti)
        // -----------------------------------------------------------------------
        private void RegistraFallimento(string workFolder, string nome, string cognome,
                                         string denti, DateTime data, string motivo)
        {
            string riga = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                          $"Lavoro: {workFolder} | " +
                          $"Paziente: {nome} {cognome} | " +
                          $"Denti: {(string.IsNullOrWhiteSpace(denti) ? "N/D" : denti)} | " +
                          $"Data lavoro: {data:dd/MM/yyyy} | " +
                          $"Motivo: {motivo}";

            _backupFalliti.Add(riga);
        }

        // -----------------------------------------------------------------------
        // Scrive il file di log nella cartella di backup.
        // Chiamato alla fine del loop (o del singolo backup se vuoi).
        // -----------------------------------------------------------------------
        private void ScriviLogFalliti()
        {
            if (_backupFalliti.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(BackUpPath.Text) || !Directory.Exists(BackUpPath.Text))
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string logPath = Path.Combine(BackUpPath.Text, $"backup_falliti_{timestamp}.txt");

            var righe = new List<string>();
            righe.Add($"=== Log backup falliti — {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            righe.Add($"Totale fallimenti: {_backupFalliti.Count}");
            righe.Add("");
            righe.AddRange(_backupFalliti);

            File.WriteAllLines(logPath, righe, Encoding.UTF8);

            label1.ForeColor = Color.OrangeRed;
            label1.Text = $"⚠ {_backupFalliti.Count} lavori non salvati — vedi log in backup";
        }
        private void BackUpButton_Click(object sender, EventArgs e)
        {
            DoBackUp();
        }

        private void MyWorksDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ClearForm();
            LoadSelectedWork();
        }

        private async void LoopBackUpButton_Click(object sender, EventArgs e)
        {
            _backupFalliti.Clear(); // pulisce i fallimenti del run precedente
            _spazioEsaurito = false;

            int totale = MyWorksDataGridView.Rows.Count;
            progressBar1.Value = 0;
            progressBar1.Maximum = totale;
            progressBar1.Step = 1;

            // Blocca tutta l'interfaccia: un click su griglia o pulsanti durante il
            // loop cambierebbe il lavoro selezionato o svuoterebbe la griglia
            ImpostaInterfacciaAttiva(false);

            string msg;
            try
            {
                foreach (DataGridViewRow row in MyWorksDataGridView.Rows)
                {
                    row.Selected = true;
                    MyWorksDataGridView.CurrentCell = MyWorksDataGridView.Rows[row.Index].Cells[0];

                    await Task.Delay(1); // lascia aggiornare la UI senza bloccare il thread

                    DoBackUp(isLoopMode: true); // passa true: non ricarica la griglia ad ogni passo

                    progressBar1.Value++;

                    if (_spazioEsaurito)
                        break;

                    await Task.Delay(1);
                }

                CaricaLavori();          // ricarica la griglia UNA SOLA VOLTA alla fine
                ScriviLogFalliti();      // scrive il log solo se ci sono stati problemi

                if (_spazioEsaurito)
                    msg = "Backup interrotto: spazio esaurito sul disco di backup.";
                else if (_backupFalliti.Count == 0)
                    msg = "Backup completato senza errori ✔";
                else
                    msg = $"Backup completato con {_backupFalliti.Count} errori.\nVedi il file log nella cartella di backup.";
            }
            catch (Exception ex)
            {
                msg = "Backup interrotto per un errore:\n" + ex.Message;
            }
            finally
            {
                ImpostaInterfacciaAttiva(true);
            }

            MessageBox.Show(msg, "Backup");
        }

        private void ImpostaInterfacciaAttiva(bool attiva)
        {
            MyWorksDataGridView.Enabled = attiva;
            ChoosePathButton.Enabled = attiva;
            LoadWorksButton.Enabled = attiva;
            LoadSelectedWorkButton.Enabled = attiva;
            ChooseBackUpFolderButton.Enabled = attiva;
            BackUpButton.Enabled = attiva;
            LoopBackUpButton.Enabled = attiva;
            UseWaitCursor = !attiva;
        }

        private bool IsWorkBackedUp(string workName)
        {
            if (string.IsNullOrWhiteSpace(BackUpPath.Text) || !Directory.Exists(BackUpPath.Text))
                return false;

            // Cerca ricorsivamente una cartella che contenga (NomeLavoro) nel nome
            var found = Directory.GetDirectories(BackUpPath.Text, $"*({workName})", SearchOption.AllDirectories);
            return found.Length > 0;
        }
    }

}