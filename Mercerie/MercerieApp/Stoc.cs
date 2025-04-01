using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercerie
{
    public class Stoc
    {
        public List<Produs> Produse { get; set; } = new List<Produs>();

        public void AdaugaProdus(Produs produs)
        {
            Produse.Add(produs);
        }

        public Produs CautaProdus(int id)
        {
            return Produse.FirstOrDefault(p => p.Id == id);
        }
    }
}
