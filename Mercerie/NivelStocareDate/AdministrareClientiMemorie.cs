using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercerie
{
    public class AdministrareClientiMemorie
    {
        private List<Client> clienti = new List<Client>();

        public void AdaugaClient(Client client)
        {
            clienti.Add(client);
        }

        public List<Client> GetClienti()
        {
            return clienti;
        }

        public Client CautaClient(string nume)
        {
            return clienti.FirstOrDefault(c => c.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase));
        }

        public List<Client> CautaClienti(string criteriu)
        {
            if (string.IsNullOrWhiteSpace(criteriu))
                return clienti;

            criteriu = criteriu.ToLower();
            return clienti.Where(c =>
                c.Id.ToString().Contains(criteriu) ||
                c.Nume.ToLower().Contains(criteriu) ||
                c.Telefon.ToLower().Contains(criteriu)
            ).ToList();
        }
    }
}