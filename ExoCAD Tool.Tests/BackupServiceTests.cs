using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoCAD_Tool.Tests
{
    // Tutti i dati sono inventati: non usare mai dati reali di pazienti nei test
    [TestClass]
    public class BackupServiceTests
    {
        private string _radice;
        private string _cadData;
        private string _backup;

        [TestInitialize]
        public void Inizializza()
        {
            _radice = Path.Combine(Path.GetTempPath(), "ExoCADToolTest_" + Guid.NewGuid().ToString("N"));
            _cadData = Path.Combine(_radice, "CAD-Data");
            _backup = Path.Combine(_radice, "Backup");
            Directory.CreateDirectory(_cadData);
            Directory.CreateDirectory(_backup);
        }

        [TestCleanup]
        public void Pulisci()
        {
            Directory.Delete(_radice, true);
        }

        private JobInfo CreaLavoro(string nomeCartella, string studio = "Studio Bianchi", string denti = "11-13")
        {
            string dir = Path.Combine(_cadData, nomeCartella);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "modello.stl"), "contenuto stl");
            File.WriteAllText(Path.Combine(dir, nomeCartella + ".dentalProject"), "<DentalProject />");

            return new JobInfo
            {
                CartellaLavoro = nomeCartella,
                PercorsoLavoro = dir,
                Nome = "Mario",
                Cognome = "Rossi",
                Studio = studio,
                Denti = denti,
                Data = new DateTime(2025, 6, 12)
            };
        }

        [DataTestMethod]
        [DataRow("Mario Rossi 11-13 (2025-06-12_00005-001)", "2025-06-12_00005-001")]
        [DataRow("Mario Rossi 11 (Lavoro x3(2025-11-03))", "Lavoro x3(2025-11-03)")]
        [DataRow("Rossi (bis) 11 (Lavoro2)", "Lavoro2")]
        [DataRow("Mario Rossi 11 (Lavoro).tmp", null)]
        [DataRow("2025", null)]
        public void EstraiNomeLavoro(string cartella, string atteso)
        {
            Assert.AreEqual(atteso, BackupService.EstraiNomeLavoro(cartella));
        }

        [DataTestMethod]
        [DataRow("Studio Rossi/Bianchi", "Studio Rossi-Bianchi")]
        [DataRow("Dr. Verdi S.r.l.", "Dr. Verdi S.r.l")]
        [DataRow(" A:B*C? ", "A-B-C-")]
        [DataRow(null, "")]
        public void SanificaNome(string testo, string atteso)
        {
            Assert.AreEqual(atteso, BackupService.SanificaNome(testo));
        }

        [TestMethod]
        public void Valida_RichiedeStudioDentiEAlmenoNomeOCognome()
        {
            JobInfo job = CreaLavoro("lavoro");
            Assert.IsNull(BackupService.Valida(job));

            job.Nome = "";
            Assert.IsNull(BackupService.Valida(job), "basta il cognome");

            job.Cognome = " ";
            Assert.AreEqual(BackupService.MotivoDatiMancanti, BackupService.Valida(job));

            job.Cognome = "Rossi";
            job.Studio = "...";
            Assert.AreEqual(BackupService.MotivoDatiMancanti, BackupService.Valida(job), "studio non valido dopo la sanificazione");
        }

        [TestMethod]
        public void Esegui_CreaCartellaFinaleSenzaTmpERiconoscibileDallaScansione()
        {
            JobInfo job = CreaLavoro("Lavoro x3(2025-11-03)", studio: "Studio Rossi/Bianchi");

            string dest = BackupService.Esegui(job, _backup, null, false, CancellationToken.None);

            Assert.AreEqual(Path.Combine(_backup, "2025", "Studio Rossi-Bianchi", "Mario Rossi 11-13 (Lavoro x3(2025-11-03))"), dest);
            Assert.IsTrue(File.Exists(Path.Combine(dest, "modello.stl")));
            Assert.IsFalse(Directory.Exists(dest + ".tmp"));
            Assert.IsFalse(Directory.EnumerateFiles(dest, "*.part").Any());

            var salvati = BackupService.ScansionaBackup(_backup);
            Assert.AreEqual(dest, salvati["Lavoro x3(2025-11-03)"]);
        }

        [TestMethod]
        public void ScansionaBackup_IgnoraBackupIncompleti()
        {
            Directory.CreateDirectory(Path.Combine(_backup, "2025", "Studio", "Mario Rossi 11 (Lavoro).tmp"));
            Assert.AreEqual(0, BackupService.ScansionaBackup(_backup).Count);
        }

        [TestMethod]
        public void ScansionaBackup_CartellaInesistente_RestituisceVuoto()
        {
            Assert.AreEqual(0, BackupService.ScansionaBackup(Path.Combine(_radice, "nonEsiste")).Count);
            Assert.AreEqual(0, BackupService.ScansionaBackup("").Count);
        }

        [TestMethod]
        public void Esegui_RiprendeUnBackupInterrotto()
        {
            JobInfo job = CreaLavoro("lavoro");
            string desiderata = BackupService.CartellaDestinazione(_backup, job);

            // Simula un backup interrotto: cartella .tmp con un file copiato a metà
            Directory.CreateDirectory(desiderata + ".tmp");
            File.WriteAllText(Path.Combine(desiderata + ".tmp", "modello.stl"), "cont");

            string dest = BackupService.Esegui(job, _backup, null, false, CancellationToken.None);

            Assert.AreEqual("contenuto stl", File.ReadAllText(Path.Combine(dest, "modello.stl")));
            Assert.IsFalse(Directory.Exists(desiderata + ".tmp"));
        }

        [TestMethod]
        public void Esegui_AggiornaIFileModificati()
        {
            JobInfo job = CreaLavoro("lavoro");
            string dest = BackupService.Esegui(job, _backup, null, false, CancellationToken.None);

            string sorgente = Path.Combine(job.PercorsoLavoro, "modello.stl");
            File.WriteAllText(sorgente, "contenuto stl modificato");
            File.SetLastWriteTimeUtc(sorgente, DateTime.UtcNow.AddMinutes(5));

            BackupService.Esegui(job, _backup, dest, false, CancellationToken.None);

            Assert.AreEqual("contenuto stl modificato", File.ReadAllText(Path.Combine(dest, "modello.stl")));
        }

        [TestMethod]
        public void Esegui_RiusaLaCartellaEsistenteSeICambianoIDenti()
        {
            JobInfo job = CreaLavoro("lavoro");
            string primo = BackupService.Esegui(job, _backup, null, false, CancellationToken.None);

            job.Denti = "11-14";
            string secondo = BackupService.Esegui(job, _backup, primo, false, CancellationToken.None);

            Assert.AreEqual(primo, secondo);
            Assert.AreEqual(1, Directory.GetDirectories(Path.GetDirectoryName(primo)).Length, "nessuna cartella duplicata");
        }

        [TestMethod]
        public void Esegui_ConSposta_RinominaLaCartellaEsistente()
        {
            JobInfo job = CreaLavoro("lavoro");
            string primo = BackupService.Esegui(job, _backup, null, false, CancellationToken.None);

            job.Denti = "11-14";
            string secondo = BackupService.Esegui(job, _backup, primo, true, CancellationToken.None);

            Assert.AreEqual(BackupService.CartellaDestinazione(_backup, job), secondo);
            Assert.IsFalse(Directory.Exists(primo));
            Assert.IsTrue(File.Exists(Path.Combine(secondo, "modello.stl")));
        }

        [TestMethod]
        public void Esegui_Annullato_NonSegnaIlLavoroComeSalvato()
        {
            JobInfo job = CreaLavoro("lavoro");
            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsException<OperationCanceledException>(
                () => BackupService.Esegui(job, _backup, null, false, cts.Token));
            Assert.AreEqual(0, BackupService.ScansionaBackup(_backup).Count);
        }

        [TestMethod]
        public void SpazioLibero_DiscoLocaleEPercorsoInesistente()
        {
            Assert.IsTrue(BackupService.SpazioLibero(_backup) > 0);
            Assert.AreEqual(-1, BackupService.SpazioLibero(@"Z:\nonEsiste\davvero"));
        }
    }
}
