using Mercerie;
using EvidentaMercerie;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NivelStocareDate
{
    public class AdministrareProduseFisier
    {
        private string caleFisier;

        // Constructorul nu are tip de returnare
        public AdministrareProduseFisier(string caleFisier)
        {
            this.caleFisier = caleFisier;
        }

        public void AdaugaProdus(Produs produs)
        {
            string linie = $"{produs.Id},{produs.Nume},{produs.Pret},{(int)produs.Tip}";
            File.AppendAllText(caleFisier, linie + Environment.NewLine);
        }
    }
}
