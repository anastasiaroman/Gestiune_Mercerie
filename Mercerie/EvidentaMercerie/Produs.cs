using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercerie
{
        public class Produs
        {
            public int Id { get; set; }
            public string Nume { get; set; }
            public double Pret { get; set; }

            public Produs(int id, string nume, double pret)
            {
                Id = id;
                Nume = nume;
                Pret = pret;
            }

            public override string ToString()
            {
                return $"ID: {Id}, Nume: {Nume}, Pret: {Pret} RON";
            }
        }
}
