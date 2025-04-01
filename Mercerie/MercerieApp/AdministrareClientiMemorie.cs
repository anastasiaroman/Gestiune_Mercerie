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
            return clienti.FirstOrDefault(c => c.Nume == nume);
        }
    }
}
