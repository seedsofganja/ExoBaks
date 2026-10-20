using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExoCAD_Tool.Tests
{
    // Tutti i dati sono inventati: non usare mai dati reali di pazienti nei test
    [TestClass]
    public class JobReaderTests
    {
        private string _cadData;

        [TestInitialize]
        public void Inizializza()
        {
            _cadData = Path.Combine(Path.GetTempPath(), "ExoCADToolTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_cadData);
        }

        [TestCleanup]
        public void Pulisci()
        {
            Directory.Delete(_cadData, true);
        }

        private string CreaLavoro(string nome, string dentalProject, string constructionInfo = null)
        {
            string dir = Path.Combine(_cadData, nome);
            Directory.CreateDirectory(dir);
            if (dentalProject != null)
                File.WriteAllText(Path.Combine(dir, nome + ".dentalProject"), dentalProject);
            if (constructionInfo != null)
                File.WriteAllText(Path.Combine(dir, nome + ".constructionInfo"), constructionInfo);
            return dir;
        }

        private const string DentalProjectCompleto = @"<?xml version=""1.0""?>
<DentalProject>
  <PatientFirstName>Mario</PatientFirstName>
  <PatientName>Rossi</PatientName>
  <PracticeName>Studio Bianchi</PracticeName>
  <DateTime>2025-06-12T12:42:27.5863216</DateTime>
  <Teeth>
    <Tooth><Number>11</Number><ReconstructionType>Crown</ReconstructionType></Tooth>
    <Tooth><Number>12</Number><ReconstructionType>Crown</ReconstructionType></Tooth>
    <Tooth><Number>41</Number><ReconstructionType>Antagonist</ReconstructionType></Tooth>
  </Teeth>
</DentalProject>";

        [TestMethod]
        public void FormattaDenti_ComprimeIntervalli()
        {
            Assert.AreEqual("11-13 15 43", JobReader.FormattaDenti(new[] { 43, 12, 11, 15, 13 }));
        }

        [TestMethod]
        public void FormattaDenti_RimuoveDuplicati()
        {
            Assert.AreEqual("11-12", JobReader.FormattaDenti(new[] { 11, 11, 12 }));
        }

        [TestMethod]
        public void FormattaDenti_Vuoto()
        {
            Assert.AreEqual("", JobReader.FormattaDenti(new int[0]));
        }

        [TestMethod]
        public void Leggi_DatiDaDentalProject()
        {
            JobInfo job = JobReader.Leggi(CreaLavoro("2025-06-12_00005-001", DentalProjectCompleto));

            Assert.AreEqual("2025-06-12_00005-001", job.CartellaLavoro);
            Assert.AreEqual("Mario", job.Nome);
            Assert.AreEqual("Rossi", job.Cognome);
            Assert.AreEqual("Studio Bianchi", job.Studio);
            Assert.AreEqual(new DateTime(2025, 6, 12), job.Data.Date);
        }

        [TestMethod]
        public void Leggi_SenzaConstructionInfo_UsaDentalProjectEscludendoAntagonist()
        {
            JobInfo job = JobReader.Leggi(CreaLavoro("lavoro", DentalProjectCompleto));
            Assert.AreEqual("11-12", job.Denti);
        }

        [TestMethod]
        public void Leggi_ConConstructionInfo_UsaQuelloPerIDenti()
        {
            const string ci = @"<ConstructionInfo><Teeth>
                <Tooth><Number>21</Number></Tooth><Tooth><Number>22</Number></Tooth><Tooth><Number>23</Number></Tooth>
            </Teeth></ConstructionInfo>";

            JobInfo job = JobReader.Leggi(CreaLavoro("lavoro", DentalProjectCompleto, ci));
            Assert.AreEqual("21-23", job.Denti);
        }

        [TestMethod]
        public void Leggi_SenzaDateTime_UsaDataCartellaENonQuellaDiUnAltroLavoro()
        {
            // Prima un lavoro con data 2019, poi uno senza DateTime: non deve ereditare il 2019
            JobReader.Leggi(CreaLavoro("vecchio", DentalProjectCompleto.Replace("2025-06-12", "2019-01-01")));
            string dir = CreaLavoro("senzaData", "<DentalProject><PatientName>Verdi</PatientName></DentalProject>");

            JobInfo job = JobReader.Leggi(dir);
            Assert.AreEqual(Directory.GetCreationTime(dir).Year, job.Data.Year);
        }

        [TestMethod]
        public void Leggi_SenzaDenti_NonLeggeConstructionInfo()
        {
            JobInfo job = JobReader.Leggi(CreaLavoro("lavoro", DentalProjectCompleto), leggiDenti: false);
            Assert.AreEqual("", job.Denti);
            Assert.AreEqual("Rossi", job.Cognome);
        }
    }
}
