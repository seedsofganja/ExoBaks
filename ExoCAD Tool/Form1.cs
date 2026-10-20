using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

            // -----------------------------------------------------------------------
            // OTTIMIZZAZIONE: invece di cercare su disco per ogni lavoro (lentissimo
            // con tante cartelle), leggiamo TUTTE le sottocartelle del backup UNA SOLA
            // volta e le mettiamo in un HashSet in memoria. La ricerca diventa O(1).
            // -----------------------------------------------------------------------
            var backuppedWorks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(BackUpPath.Text) && Directory.Exists(BackUpPath.Text))
            {
                foreach (string dir in Directory.GetDirectories(BackUpPath.Text, "*", SearchOption.AllDirectories))
                {
                    string dirName = Path.GetFileName(dir);

                    // Il nome cartella ha il formato: "Nome Cognome Denti (NomeLavoro)"
                    // Estraiamo il NomeLavoro che sta dentro le ultime parentesi tonde
                    int parenOpen = dirName.LastIndexOf('(');
                    int parenClose = dirName.LastIndexOf(')');

                    if (parenOpen >= 0 && parenClose > parenOpen)
                    {
                        string workName = dirName.Substring(parenOpen + 1, parenClose - parenOpen - 1).Trim();
                        backuppedWorks.Add(workName);
                    }
                }
            }

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
                    if (xmlNode != null && DateTime.TryParse(xmlNode.InnerText, out DateTime dt))
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

                // Ordina e costruisce la stringa range (es. "11-13 15 43")
                Vars.Teeth.Sort();
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
                string nome = NomePaziente.Text.Trim();
                string cognome = CognomePaziente.Text.Trim();
                string studio = StudioDentistico.Text.Trim();
                string denti = Denti.Text.Trim();
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

                Directory.CreateDirectory(yearFolder);
                Directory.CreateDirectory(studioFolder);
                Directory.CreateDirectory(pazienteFolder);

                // --- Copia i file uno per uno ---
                DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(BackUpPath.Text));

                foreach (string file in Directory.GetFiles(workPath))
                {
                    string destPath = Path.Combine(pazienteFolder, Path.GetFileName(file));

                    if (File.Exists(destPath))
                        continue; // già copiato in precedenza, salta

                    long fileSize = new FileInfo(file).Length;
                    long freeSpace = driveInfo.AvailableFreeSpace; // riletto ad ogni file

                    if (freeSpace <= fileSize)
                    {
                        string motivo = "Spazio sul disco di backup esaurito";
                        MessageBox.Show(motivo, "Spazio insufficiente");
                        RegistraFallimento(workFolder, nome, cognome, denti, data, motivo);
                        return;
                    }

                    File.Copy(file, destPath);
                }

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

            int totale = MyWorksDataGridView.Rows.Count;
            progressBar1.Value = 0;
            progressBar1.Maximum = totale;
            progressBar1.Step = 1;

            LoopBackUpButton.Enabled = false;
            BackUpButton.Enabled = false;

            foreach (DataGridViewRow row in MyWorksDataGridView.Rows)
            {
                row.Selected = true;
                MyWorksDataGridView.CurrentCell = MyWorksDataGridView.Rows[row.Index].Cells[0];

                await Task.Delay(1); // lascia aggiornare la UI senza bloccare il thread

                DoBackUp(isLoopMode: true); // passa true: non ricarica la griglia ad ogni passo

                progressBar1.Value++;
                await Task.Delay(1);
            }

            CaricaLavori();          // ricarica la griglia UNA SOLA VOLTA alla fine
            ScriviLogFalliti();      // scrive il log solo se ci sono stati problemi

            LoopBackUpButton.Enabled = true;
            BackUpButton.Enabled = true;

            string msg = _backupFalliti.Count == 0
                ? "Backup completato senza errori ✔"
                : $"Backup completato con {_backupFalliti.Count} errori.\nVedi il file log nella cartella di backup.";

            MessageBox.Show(msg, "Backup");
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