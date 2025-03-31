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
        public List<Produs> Comenzi { get; set; } = new List<Produs>();

        public Client(int id, string nume)
        {
            Id = id;
            Nume = nume;
        }

        public void AdaugaComanda(Produs produs)
        {
            Comenzi.Add(produs);
        }

        public override string ToString()
        {
            return $"ID: {Id}, Nume: {Nume}, Comenzi: {Comenzi.Count}";
        }
    }
}
