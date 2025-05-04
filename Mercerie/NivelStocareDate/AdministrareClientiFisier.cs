using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mercerie;
using EvidentaMercerie;

namespace Mercerie
{
    public class AdministrareClientiFisier
    {
        private string filePath = "clienti.txt";

        public void SalveazaClient(Client client)
        {
            string comenzi = client.Comenzi.Count > 0 ? string.Join("|", client.Comenzi.Select(p => $"{p.Id}-{p.Nume}-{p.Pret}-{p.Tip}")) : "Nicio comanda";
            File.AppendAllText(filePath, $"{client.Id},{client.Nume},{client.Telefon},{comenzi}\n");
        }

        public List<Client> CitesteClienti()
        {
            var clienti = new List<Client>();
            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        var client = new Client(int.Parse(parts[0]), parts[1], parts[2]);
                        if (parts.Length > 3 && parts[3] != "Nicio comanda")
                        {
                            var produse = parts[3].Split('|');
                            foreach (var produs in produse)
                            {
                                var detalii = produs.Split('-');
                                if (detalii.Length == 4) // Id, Nume, Pret, Tip
                                {
                                    client.AdaugaComanda(new Produs(
                                        int.Parse(detalii[0]),
                                        detalii[1],
                                        double.Parse(detalii[2]),
                                        (TipProdus)Enum.Parse(typeof(TipProdus), detalii[3])
                                    ));
                                }
                            }
                        }
                        clienti.Add(client);
                    }
                }
            }
            return clienti;
        }

        public void SalveazaTotiClientii(List<Client> clienti)
        {
            File.WriteAllLines(filePath, clienti.Select(c =>
            {
                string comenzi = c.Comenzi.Count > 0 ? string.Join("|", c.Comenzi.Select(p => $"{p.Id}-{p.Nume}-{p.Pret}-{p.Tip}")) : "Nicio comanda";
                return $"{c.Id},{c.Nume},{c.Telefon},{comenzi}";
            }));
        }
    }
}