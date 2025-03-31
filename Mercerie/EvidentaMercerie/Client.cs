using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercerie
{
    public class Client
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public string Telefon { get; set; }
        public List<Produs> Comenzi { get; set; } = new List<Produs>();

        public Client(int id, string nume, string telefon)
        {
            Id = id;
            Nume = nume;
            Telefon = telefon;
        }

        public void AdaugaComanda(Produs produs)
        {
            Comenzi.Add(produs);
        }

        public override string ToString()
        {
            string comenziStr = Comenzi.Count > 0 ? string.Join("; ", Comenzi.Select(p => p.Nume)) : "Nicio comanda";
            return $"ID: {Id}, Nume: {Nume}, Telefon: {Telefon}, Comenzi: {comenziStr}";
        }
    }
}
