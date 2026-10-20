using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace ExoCAD_Tool
{
    // Legge i dati di un lavoro dai file XML di ExoCAD in <CAD-Data>\<Lavoro>\
    public static class JobReader
    {
        // Lavoro con i soli dati ricavabili dalla cartella (usato anche se gli XML sono illeggibili)
        public static JobInfo SoloCartella(string percorsoLavoro)
        {
            return new JobInfo
            {
                CartellaLavoro = Path.GetFileName(percorsoLavoro),
                PercorsoLavoro = percorsoLavoro,
                // Data di default = creazione della cartella, usata se manca il DateTime nel .dentalProject
                Data = Directory.GetCreationTime(percorsoLavoro)
            };
        }

        // leggiDenti = false salta il .constructionInfo (più veloce, per l'elenco dei lavori)
        public static JobInfo Leggi(string percorsoLavoro, bool leggiDenti = true)
        {
            JobInfo job = SoloCartella(percorsoLavoro);
            string dentalProject = Path.Combine(percorsoLavoro, job.CartellaLavoro + ".dentalProject");
            string constructionInfo = Path.Combine(percorsoLavoro, job.CartellaLavoro + ".constructionInfo");

            // DentalProject — nome, cognome, studio, data
            XmlDocument xml = null;
            if (File.Exists(dentalProject))
            {
                xml = new XmlDocument();
                xml.Load(dentalProject);

                job.Nome = Testo(xml, "//PatientFirstName");
                job.Cognome = Testo(xml, "//PatientName");
                job.Studio = Testo(xml, "//PracticeName");

                if (ProvaLeggiData(Testo(xml, "//DateTime"), out DateTime data))
                    job.Data = data;
            }

            if (!leggiDenti)
                return job;

            // Denti — prima provo .constructionInfo, altrimenti leggo dal .dentalProject già caricato
            var denti = new List<int>();
            if (File.Exists(constructionInfo))
            {
                var xmlCI = new XmlDocument();
                xmlCI.Load(constructionInfo);

                foreach (XmlNode node in xmlCI.SelectNodes("//Tooth/Number"))
                {
                    if (int.TryParse(node.InnerText, out int t))
                        denti.Add(t);
                }
            }
            else if (xml != null)
            {
                // Lavoro parziale: leggo i denti dal .dentalProject, escludo gli Antagonist
                foreach (XmlNode node in xml.SelectNodes("//Teeth/Tooth"))
                {
                    XmlNode recType = node.SelectSingleNode("ReconstructionType");
                    if (recType != null && recType.InnerText == "Antagonist") continue;

                    XmlNode numNode = node.SelectSingleNode("Number");
                    if (numNode != null && int.TryParse(numNode.InnerText, out int t))
                        denti.Add(t);
                }
            }

            job.Denti = FormattaDenti(denti);
            return job;
        }

        // Rimuove i duplicati, ordina e comprime in intervalli (es. "11-13 15 43")
        public static string FormattaDenti(IEnumerable<int> denti)
        {
            List<int> ordinati = denti.Distinct().OrderBy(t => t).ToList();
            var parti = new List<string>();

            int i = 0;
            while (i < ordinati.Count)
            {
                int start = ordinati[i];
                int end = start;
                while (i + 1 < ordinati.Count && ordinati[i + 1] == ordinati[i] + 1)
                {
                    end = ordinati[i + 1];
                    i++;
                }
                parti.Add(end > start ? start + "-" + end : start.ToString());
                i++;
            }
            return string.Join(" ", parti);
        }

        private static string Testo(XmlDocument xml, string xpath)
        {
            return xml.SelectSingleNode(xpath)?.InnerText ?? "";
        }

        // Prima con le impostazioni internazionali del PC (comportamento originale),
        // poi in formato invariante (es. date ISO su un PC con impostazioni diverse)
        private static bool ProvaLeggiData(string testo, out DateTime data)
        {
            return DateTime.TryParse(testo, out data) ||
                   DateTime.TryParse(testo, CultureInfo.InvariantCulture, DateTimeStyles.None, out data);
        }
    }
}
