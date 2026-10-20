using System;

namespace ExoCAD_Tool
{
    // Dati di un lavoro ExoCAD (letti dai file XML o inseriti a mano nel form)
    public class JobInfo
    {
        public string CartellaLavoro { get; set; } = "";   // nome della cartella in CAD-Data
        public string PercorsoLavoro { get; set; } = "";   // percorso completo della cartella
        public string Nome { get; set; } = "";
        public string Cognome { get; set; } = "";
        public string Studio { get; set; } = "";
        public string Denti { get; set; } = "";            // es. "11-13 15 43"
        public DateTime Data { get; set; }

        public string Paziente => (Nome + " " + Cognome).Trim();
    }
}
