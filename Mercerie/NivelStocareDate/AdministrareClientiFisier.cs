using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mercerie;

namespace Mercerie
{
    public class AdministrareClientiFisier
    {
        private string filePath = "clienti.txt";

        public void SalveazaClient(Client client)
        {
            File.AppendAllText(filePath, $"{client.Id},{client.Nume}\n");
        }

        public List<Client> CitesteClienti()
        {
            var clienti = new List<Client>();
            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        clienti.Add(new Client(int.Parse(parts[0]), parts[1]));
                    }
                }
            }
            return clienti;
        }
    }
}
