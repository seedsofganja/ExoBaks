using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExoCAD_Tool
{
    public partial class Form1 : Form
    {
        // Lavori trovati in CAD-Data (letti solo i dati principali, senza denti)
        private List<JobInfo> _lavori = new List<JobInfo>();

        // Lavori già presenti nel backup: nome cartella lavoro -> cartella di backup
        private Dictionary<string, string> _backupEsistenti = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Lavoro selezionato nella griglia e caricato nei campi del form
        private JobInfo _lavoroSelezionato;

        // Non null mentre è in corso un backup (singolo o completo): usato da Annulla
        private CancellationTokenSource _cts;

        // > 0 mentre è in corso un'operazione: interfaccia bloccata
        private int _operazioniInCorso;

        // Evita di ricaricare il lavoro selezionato mentre la griglia viene ripopolata
        private bool _popolamentoInCorso;

        // Un solo file di log per sessione: backup_falliti_<avvio>.txt nella cartella di backup
        private readonly string _inizioSessione = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var impostazioni = Properties.Settings.Default;
            if (impostazioni.UpgradeRichiesto)
            {
                // Dopo un aggiornamento dell'app recupera i percorsi della versione precedente
                impostazioni.Upgrade();
                impostazioni.UpgradeRichiesto = false;
                impostazioni.Save();
            }

            if (Directory.Exists(impostazioni.BackUpPath))
                BackUpPath.Text = impostazioni.BackUpPath;

            string worksPath = Directory.Exists(impostazioni.WorksPath)
                ? impostazioni.WorksPath
                : CercaCadDataPredefinito();

            if (worksPath != null)
            {
                WorksPathTextBox.Text = worksPath;
                await CaricaLavori();
            }
        }

        // Percorsi tipici di ExoCAD: C:\OPT-Exocad\CAD-Data oppure C:\exocad-DentalCAD...\CAD-Data
        private static string CercaCadDataPredefinito()
        {
            const string opt = @"C:\OPT-Exocad\CAD-Data";
            if (Directory.Exists(opt))
                return opt;

            try
            {
                return Directory.GetDirectories(@"C:\", "exocad*")
                    .Select(d => Path.Combine(d, "CAD-Data"))
                    .Where(Directory.Exists)
                    .OrderByDescending(d => d)   // la versione più recente per prima
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_operazioniInCorso > 0)
            {
                MessageBox.Show(
                    _cts != null
                        ? "Backup in corso: premi Annulla e attendi la fine prima di chiudere."
                        : "Operazione in corso: attendi la fine prima di chiudere.",
                    "Operazione in corso");
                e.Cancel = true;
            }
        }

        private async void ChoosePathButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    WorksPathTextBox.Text = fbd.SelectedPath;
                    Properties.Settings.Default.WorksPath = fbd.SelectedPath;
                    Properties.Settings.Default.Save();
                    await CaricaLavori();
                }
            }
        }

        private async void ChooseBackUpFolderButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    BackUpPath.Text = fbd.SelectedPath;
                    Properties.Settings.Default.BackUpPath = fbd.SelectedPath;
                    Properties.Settings.Default.Save();

                    if (!string.IsNullOrWhiteSpace(WorksPathTextBox.Text))
                        await CaricaLavori(); // aggiorna i colori
                }
            }
        }

        private async void LoadWorksButton_Click(object sender, EventArgs e)
        {
            await CaricaLavori();
        }

        // -----------------------------------------------------------------------
        // Legge l'elenco dei lavori da CAD-Data e lo stato del backup.
        // La lettura da disco avviene in background: la finestra resta reattiva.
        // -----------------------------------------------------------------------
        public async Task CaricaLavori()
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

            string worksDir = WorksPathTextBox.Text;
            string backupDir = BackUpPath.Text;

            InizioOperazione();
            ImpostaStato("Caricamento lavori...", Color.Black);
            try
            {
                // Le cartelle del backup vengono lette UNA SOLA volta in un dizionario:
                // la verifica "già salvato" per ogni lavoro diventa istantanea
                _backupEsistenti = await Task.Run(() => BackupService.ScansionaBackup(backupDir));
                _lavori = await Task.Run(() => Directory.GetDirectories(worksDir).Select(LeggiPerElenco).ToList());

                PopolaGriglia();
                AggiornaContatori();
            }
            catch (Exception ex)
            {
                ImpostaStato("Errore durante il caricamento dei lavori", Color.Red);
                MessageBox.Show("Errore durante il caricamento dei lavori:\n" + ex.Message, "Errore");
            }
            finally
            {
                FineOperazione();
            }
        }

        private static JobInfo LeggiPerElenco(string percorsoLavoro)
        {
            try
            {
                return JobReader.Leggi(percorsoLavoro, leggiDenti: false);
            }
            catch (Exception)
            {
                // XML illeggibile: il lavoro compare comunque, con i soli dati della cartella
                return JobReader.SoloCartella(percorsoLavoro);
            }
        }

        // -----------------------------------------------------------------------
        // Riempie la griglia applicando i filtri e colora ogni riga in base allo
        // stato di backup (verde = salvato, rosso = non salvato)
        // -----------------------------------------------------------------------
        private void PopolaGriglia()
        {
            string selezionato = _lavoroSelezionato?.CartellaLavoro;
            string filtro = CercaTextBox.Text.Trim();

            _popolamentoInCorso = true;
            try
            {
                MyWorksDataGridView.Rows.Clear();

                var righe = new List<DataGridViewRow>();
                foreach (JobInfo job in _lavori)
                {
                    bool salvato = _backupEsistenti.ContainsKey(job.CartellaLavoro);

                    if (SoloNonSalvatiCheckBox.Checked && salvato) continue;
                    if (filtro != "" && !Corrisponde(job, filtro)) continue;

                    var row = new DataGridViewRow();
                    row.CreateCells(MyWorksDataGridView, job.CartellaLavoro, job.Paziente, job.Studio, job.Data);
                    row.Tag = job;
                    ColoraRiga(row, salvato);
                    righe.Add(row);
                }
                MyWorksDataGridView.Rows.AddRange(righe.ToArray());

                // Ripristina la selezione precedente se il lavoro è ancora visibile
                MyWorksDataGridView.ClearSelection();
                DataGridViewRow daSelezionare =
                    righe.FirstOrDefault(r => ((JobInfo)r.Tag).CartellaLavoro == selezionato) ?? righe.FirstOrDefault();
                if (daSelezionare != null)
                {
                    daSelezionare.Selected = true;
                    MyWorksDataGridView.CurrentCell = daSelezionare.Cells[0];
                }
            }
            finally
            {
                _popolamentoInCorso = false;
            }

            LoadSelectedWork();
        }

        private static bool Corrisponde(JobInfo job, string filtro)
        {
            return new[] { job.CartellaLavoro, job.Paziente, job.Studio }
                .Any(campo => campo.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void ColoraRiga(DataGridViewRow row, bool salvato)
        {
            row.DefaultCellStyle.BackColor = salvato ? Color.LightGreen : Color.LightCoral;
            row.DefaultCellStyle.ForeColor = Color.Black;
        }

        private void AggiornaRigaSalvata(string cartellaLavoro)
        {
            foreach (DataGridViewRow row in MyWorksDataGridView.Rows)
            {
                if (((JobInfo)row.Tag).CartellaLavoro == cartellaLavoro)
                    ColoraRiga(row, true);
            }
        }

        private void AggiornaContatori()
        {
            int totale = _lavori.Count;
            int nonSalvati = _lavori.Count(j => !_backupEsistenti.ContainsKey(j.CartellaLavoro));

            ContatoreLabel.ForeColor = nonSalvati > 0 ? Color.Red : Color.Green;
            ContatoreLabel.Text = $"Lavori trovati: {totale}  —  " +
                (nonSalvati > 0 ? $"⚠ {nonSalvati} lavori non salvati" : "✔ Tutti i lavori sono salvati");
        }

        private void CercaTextBox_TextChanged(object sender, EventArgs e)
        {
            PopolaGriglia();
        }

        private void SoloNonSalvatiCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PopolaGriglia();
        }

        public void ClearForm()
        {
            _lavoroSelezionato = null;
            Denti.Text = "";
            NomePaziente.Text = "";
            CognomePaziente.Text = "";
            StudioDentistico.Text = "";
            DataLavoroPicker.Value = DateTime.Now;
        }

        // -----------------------------------------------------------------------
        // Carica nei campi del form i dati del lavoro selezionato (letti dagli XML).
        // I campi sono modificabili prima del backup singolo.
        // -----------------------------------------------------------------------
        public void LoadSelectedWork()
        {
            ClearForm();

            if (MyWorksDataGridView.SelectedRows.Count != 1)
                return;

            var job = (JobInfo)MyWorksDataGridView.SelectedRows[0].Tag;
            try
            {
                _lavoroSelezionato = JobReader.Leggi(job.PercorsoLavoro, leggiDenti: true);
                ImpostaStato("", Color.Black);
            }
            catch (Exception ex)
            {
                // I campi restano vuoti ma modificabili: si possono completare a mano
                _lavoroSelezionato = JobReader.SoloCartella(job.PercorsoLavoro);
                ImpostaStato("Errore nel caricamento: " + ex.Message, Color.Red);
            }

            Denti.Text = _lavoroSelezionato.Denti;
            NomePaziente.Text = _lavoroSelezionato.Nome;
            CognomePaziente.Text = _lavoroSelezionato.Cognome;
            StudioDentistico.Text = _lavoroSelezionato.Studio;
            DataLavoroPicker.Value = _lavoroSelezionato.Data;
        }

        private void LoadSelectedWorkButton_Click(object sender, EventArgs e)
        {
            LoadSelectedWork(); // ricarica dagli XML, annullando le modifiche manuali
        }

        private void MyWorksDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (!_popolamentoInCorso)
                LoadSelectedWork();
        }

        private bool ControllaCartellaBackup()
        {
            if (!string.IsNullOrWhiteSpace(BackUpPath.Text) && Directory.Exists(BackUpPath.Text))
                return true;

            ImpostaStato("Cartella di backup non specificata o non raggiungibile", Color.Red);
            return false;
        }

        // -----------------------------------------------------------------------
        // Backup del lavoro selezionato, usando i valori dei campi del form
        // (possono essere stati modificati manualmente)
        // -----------------------------------------------------------------------
        private async void BackUpButton_Click(object sender, EventArgs e)
        {
            if (!ControllaCartellaBackup())
                return;

            if (_lavoroSelezionato == null)
            {
                ImpostaStato("Nessun lavoro selezionato", Color.Red);
                return;
            }

            var job = new JobInfo
            {
                CartellaLavoro = _lavoroSelezionato.CartellaLavoro,
                PercorsoLavoro = _lavoroSelezionato.PercorsoLavoro,
                Nome = NomePaziente.Text,
                Cognome = CognomePaziente.Text,
                Studio = StudioDentistico.Text,
                Denti = Denti.Text,
                Data = DataLavoroPicker.Value
            };

            string motivo = BackupService.Valida(job);
            if (motivo != null)
            {
                ImpostaStato("Impossibile salvare: " + motivo, Color.Red);
                RegistraFallimenti(new[] { RigaFallimento(job, motivo) });
                return;
            }

            string backupRoot = BackUpPath.Text;
            _backupEsistenti.TryGetValue(job.CartellaLavoro, out string esistente);

            // Se il lavoro è già salvato con dati diversi (es. denti o nome corretti a mano)
            // chiede se spostare la cartella esistente invece di aggiornarla dov'è
            bool sposta = false;
            if (esistente != null && Directory.Exists(esistente))
            {
                string desiderata = BackupService.CartellaDestinazione(backupRoot, job);
                if (!string.Equals(esistente, desiderata, StringComparison.OrdinalIgnoreCase))
                {
                    DialogResult risposta = MessageBox.Show(
                        "Questo lavoro è già salvato in:\n" + esistente +
                        "\n\nVuoi spostare e rinominare la cartella con i dati attuali?\n" + desiderata +
                        "\n\nSì = sposta e aggiorna\nNo = aggiorna la cartella esistente",
                        "Backup già presente", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (risposta == DialogResult.Cancel)
                        return;
                    sposta = risposta == DialogResult.Yes;
                }
            }

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;
            InizioOperazione();
            AnnullaButton.Enabled = true;
            ImpostaStato("Backup in corso: " + job.CartellaLavoro, Color.Black);
            try
            {
                string destinazione = await Task.Run(() => BackupService.Esegui(job, backupRoot, esistente, sposta, ct));

                _backupEsistenti[job.CartellaLavoro] = destinazione;
                AggiornaRigaSalvata(job.CartellaLavoro);
                AggiornaContatori();
                ImpostaStato("Lavoro salvato", Color.Green);
            }
            catch (OperationCanceledException)
            {
                ImpostaStato("Backup annullato", Color.OrangeRed);
            }
            catch (SpazioEsauritoException ex)
            {
                ImpostaStato(ex.Message, Color.Red);
                RegistraFallimenti(new[] { RigaFallimento(job, ex.Message) });
                MessageBox.Show(ex.Message, "Spazio insufficiente");
            }
            catch (Exception ex)
            {
                ImpostaStato("Errore backup: " + ex.Message, Color.Red);
                RegistraFallimenti(new[] { RigaFallimento(job, ex.Message) });
            }
            finally
            {
                AnnullaButton.Enabled = false;
                _cts.Dispose();
                _cts = null;
                FineOperazione();
            }
        }

        private class EsitoBackup
        {
            public readonly List<string> Falliti = new List<string>();
            public int Elaborati;
            public bool Annullato;
            public bool SpazioEsaurito;
        }

        // -----------------------------------------------------------------------
        // Backup completo: di default salva solo i lavori non ancora salvati; con
        // la spunta "Aggiorna anche i lavori già salvati" sincronizza tutto.
        // Usa i dati letti dagli XML di ogni lavoro.
        // -----------------------------------------------------------------------
        private async void LoopBackUpButton_Click(object sender, EventArgs e)
        {
            if (!ControllaCartellaBackup())
                return;

            bool aggiornaSalvati = AggiornaSalvatiCheckBox.Checked;
            List<JobInfo> daSalvare = _lavori
                .Where(j => aggiornaSalvati || !_backupEsistenti.ContainsKey(j.CartellaLavoro))
                .ToList();

            if (daSalvare.Count == 0)
            {
                MessageBox.Show("Tutti i lavori sono già salvati ✔", "Backup");
                return;
            }

            string backupRoot = BackUpPath.Text;
            var esistenti = new Dictionary<string, string>(_backupEsistenti, StringComparer.OrdinalIgnoreCase);

            BackupProgressBar.Value = 0;
            BackupProgressBar.Maximum = daSalvare.Count;
            var progresso = new Progress<int>(i =>
            {
                BackupProgressBar.Value = i;
                ImpostaStato($"Backup {i + 1} di {daSalvare.Count}: {daSalvare[i].CartellaLavoro}", Color.Black);
            });

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;
            InizioOperazione();
            AnnullaButton.Enabled = true;

            string msg;
            try
            {
                EsitoBackup esito = await Task.Run(() => BackupCompleto(daSalvare, esistenti, backupRoot, progresso, ct));

                BackupProgressBar.Value = esito.Elaborati;
                RegistraFallimenti(esito.Falliti);
                await CaricaLavori(); // ricarica la griglia UNA SOLA VOLTA alla fine

                if (esito.SpazioEsaurito)
                    msg = "Backup interrotto: spazio esaurito sul disco di backup.";
                else if (esito.Annullato)
                    msg = $"Backup annullato dopo {esito.Elaborati} lavori su {daSalvare.Count}.";
                else if (esito.Falliti.Count == 0)
                    msg = "Backup completato senza errori ✔";
                else
                    msg = "Backup completato.";

                if (esito.Falliti.Count > 0)
                    msg += $"\n{esito.Falliti.Count} lavori non salvati: vedi il file log nella cartella di backup.";
            }
            catch (Exception ex)
            {
                msg = "Backup interrotto per un errore:\n" + ex.Message;
            }
            finally
            {
                AnnullaButton.Enabled = false;
                _cts.Dispose();
                _cts = null;
                FineOperazione();
            }

            ImpostaStato(msg.Split('\n')[0], Color.Black);
            MessageBox.Show(msg, "Backup");
        }

        // Eseguito in background: non deve toccare i controlli del form
        private static EsitoBackup BackupCompleto(List<JobInfo> lavori, Dictionary<string, string> esistenti,
                                                  string backupRoot, IProgress<int> progresso, CancellationToken ct)
        {
            var esito = new EsitoBackup();

            for (int i = 0; i < lavori.Count; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    esito.Annullato = true;
                    break;
                }

                progresso.Report(i);

                JobInfo job = lavori[i];
                try
                {
                    job = JobReader.Leggi(job.PercorsoLavoro, leggiDenti: true);

                    string motivo = BackupService.Valida(job);
                    if (motivo != null)
                    {
                        esito.Falliti.Add(RigaFallimento(job, motivo));
                    }
                    else
                    {
                        esistenti.TryGetValue(job.CartellaLavoro, out string esistente);
                        BackupService.Esegui(job, backupRoot, esistente, false, ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    esito.Annullato = true;
                    break;
                }
                catch (SpazioEsauritoException ex)
                {
                    esito.Falliti.Add(RigaFallimento(job, ex.Message));
                    esito.SpazioEsaurito = true;
                    esito.Elaborati = i + 1;
                    break;
                }
                catch (Exception ex)
                {
                    esito.Falliti.Add(RigaFallimento(job, ex.Message));
                }

                esito.Elaborati = i + 1;
            }

            return esito;
        }

        private void AnnullaButton_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            AnnullaButton.Enabled = false;
            ImpostaStato("Annullamento in corso: attendo la fine del file corrente...", Color.OrangeRed);
        }

        // -----------------------------------------------------------------------
        // Riga di log per un lavoro non salvato
        // -----------------------------------------------------------------------
        private static string RigaFallimento(JobInfo job, string motivo)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                   $"Lavoro: {job.CartellaLavoro} | " +
                   $"Paziente: {job.Nome} {job.Cognome} | " +
                   $"Denti: {(string.IsNullOrWhiteSpace(job.Denti) ? "N/D" : job.Denti)} | " +
                   $"Data lavoro: {job.Data:dd/MM/yyyy} | " +
                   $"Motivo: {motivo}";
        }

        // -----------------------------------------------------------------------
        // Aggiunge i fallimenti al log della sessione nella cartella di backup
        // (un file per ogni avvio dell'app, sia per i backup singoli che completi)
        // -----------------------------------------------------------------------
        private void RegistraFallimenti(IList<string> righe)
        {
            if (righe.Count == 0 || !Directory.Exists(BackUpPath.Text))
                return;

            try
            {
                string logPath = Path.Combine(BackUpPath.Text, $"backup_falliti_{_inizioSessione}.txt");

                var testo = new List<string>();
                if (!File.Exists(logPath))
                {
                    testo.Add($"=== Log backup falliti — sessione del {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
                    testo.Add("");
                }
                testo.AddRange(righe);

                File.AppendAllLines(logPath, testo, Encoding.UTF8);
            }
            catch (Exception)
            {
                // es. disco pieno: l'errore è già mostrato nella barra di stato
            }
        }

        private void ImpostaStato(string testo, Color colore)
        {
            StatoLabel.ForeColor = colore;
            StatoLabel.Text = testo;
        }

        private void InizioOperazione()
        {
            if (_operazioniInCorso++ == 0)
                ImpostaControlliAttivi(false);
        }

        private void FineOperazione()
        {
            if (--_operazioniInCorso == 0)
                ImpostaControlliAttivi(true);
        }

        // Blocca l'interfaccia durante le operazioni: un click su griglia o pulsanti
        // cambierebbe il lavoro selezionato o svuoterebbe la griglia
        private void ImpostaControlliAttivi(bool attivi)
        {
            MyWorksDataGridView.Enabled = attivi;
            ChoosePathButton.Enabled = attivi;
            LoadWorksButton.Enabled = attivi;
            ChooseBackUpFolderButton.Enabled = attivi;
            CercaTextBox.Enabled = attivi;
            SoloNonSalvatiCheckBox.Enabled = attivi;
            LoadSelectedWorkButton.Enabled = attivi;
            StudioDentistico.Enabled = attivi;
            NomePaziente.Enabled = attivi;
            CognomePaziente.Enabled = attivi;
            Denti.Enabled = attivi;
            DataLavoroPicker.Enabled = attivi;
            AggiornaSalvatiCheckBox.Enabled = attivi;
            BackUpButton.Enabled = attivi;
            LoopBackUpButton.Enabled = attivi;
            UseWaitCursor = !attivi;
        }
    }
}
