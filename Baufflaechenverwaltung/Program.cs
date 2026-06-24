using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Baufflaechenverwaltung
{
    public enum Nutzung
    {
        Gewerbe, Landwirtschaft, Forst, Wohnnutzung, Brachfläche
    }

    public enum Bebaubarkeit
    {
        Ja, Nein, Auflagen
    }

    public enum FlaechenStatus
    {
        Frei, Reserviert, Bebaut
    }

    public enum BauvorhabenStatus
    {
        AntragEingereicht, Genehmigt, Abgelehnt, InBearbeitung, Abgeschlossen
    }

    public class Antragsteller
    {
        public string Name { get; set; } = string.Empty;
        public string Kontaktdaten { get; set; } = string.Empty;
        public string Firma { get; set; } = string.Empty;
    }

    public class Bauflaeche
    {
        public string FlurstueckNummer { get; set; } = string.Empty;
        public double Groesse { get; set; }
        public string Lage { get; set; } = string.Empty;
        public Nutzung AktuelleNutzung { get; set; }
        public Bebaubarkeit Bebaubarkeit { get; set; }
        public string BPlanNummer { get; set; } = string.Empty;
        public decimal Bodenrichtwert { get; set; }
        public string Eigentuemer { get; set; } = string.Empty;
        public FlaechenStatus Status { get; set; } = FlaechenStatus.Frei;
        public void FlaecheReservieren()
        {
            if (Status == FlaechenStatus.Frei)
            {
                Status = FlaechenStatus.Reserviert;
            } else {
                Console.WriteLine($"Fläche {FlurstueckNummer} kann nicht reserviert werden, aktueller Status: {Status}");
            }
        }
    }

    public class Grundstueck
    {
        public string Bezeichnung { get; set; } = string.Empty;
        public List<Bauflaeche> Bauflaechen { get; set; } = new List<Bauflaeche>();
    }

    public class Bauvorhaben
    {
        public string Titel { get; set; } = string.Empty;
        public Antragsteller Antragsteller { get; set; } = new Antragsteller();
        public string GeplanteNutzung { get; set; } = string.Empty;
        public DateTime Beginn { get; set; }
        public DateTime Fertigstellung { get; set; }
        public BauvorhabenStatus Status { get; set; } = BauvorhabenStatus.AntragEingereicht;
        public List<Bauflaeche> ZugeordneteFlaechen { get; set; } = new List<Bauflaeche>();

        public void StatusAktualisieren(BauvorhabenStatus neuerStatus)
        {
            Status = neuerStatus;
        }

    }
public class Persistencemanager
    {
        public void saveToJson(string filepath)
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filepath, json);

        }
        public void loadFromJson(string filepath)
        {
            string json = File.ReadAllText(filepath);
            var obj = JsonSerializer.Deserialize<Persistencemanager>(json);
            if (obj != null)
            {
                this = obj;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Demonstration der Funktionalität
            var flaeche1 = new Bauflaeche
            {
                FlurstueckNummer = "0015 00012 001/002",
                Groesse = 500.0,
                Lage = "Leipzig-Nord",
                AktuelleNutzung = Nutzung.Brachfläche,
                Bebaubarkeit = Bebaubarkeit.Ja,
                BPlanNummer = "BP-2022-089 – Wohngebiet Leipzig-Nord",
                Bodenrichtwert = 500m,
                Eigentuemer = "Max Mustermann"
            };
            flaeche1.saveToJson("bauflaeche.json");
            var grundstueck = new Grundstueck
            {
                Bezeichnung = "Grundstück Nord 1",
                Bauflaechen = new List<Bauflaeche> { flaeche1 }
            };

            var vorhaben = new Bauvorhaben
            {
                Titel = "Neubau Wohnhaus",
                Antragsteller = new Antragsteller { Name = "Erika Musterfrau", Firma = "Bau GmbH" },
                GeplanteNutzung = "Wohngebäude",
                Beginn = DateTime.Now.AddMonths(1),
                Fertigstellung = DateTime.Now.AddMonths(12)
            };
            vorhaben.ZugeordneteFlaechen.Add(flaeche1);
            vorhaben.saveToJson("bauvorhaben.json");
            flaeche1.FlaecheReservieren();
            vorhaben.StatusAktualisieren(BauvorhabenStatus.Genehmigt);

            Console.WriteLine($"Bauvorhaben '{vorhaben.Titel}' Status: {vorhaben.Status}");
            Console.WriteLine($"Fläche {flaeche1.FlurstueckNummer} Status: {flaeche1.Status}");
        }
    }
}