using EvidentaMercerie;
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
        public TipProdus Tip { get; set; }

        public Produs(int id, string nume, double pret, TipProdus tip)
        {
            Id = id;
            Nume = nume;
            Pret = pret;
            Tip = tip;
        }

        public override string ToString()
        {
            return $"\n==============================\n" +
                   $" ID: {Id}\n" +
                   $" Nume: {Nume}\n" +
                   $" Pret: {Pret} RON\n" +
                   $" Tip: {(int)Tip} - {Tip}\n" +
                   "==============================\n";
        }
    }
}
