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

        public List<Client> CautaClienti(string criteriu, string campCautare)
        {
            if (string.IsNullOrWhiteSpace(criteriu))
                return clienti;

            criteriu = criteriu.ToLower();
            return clienti.Where(c =>
                (campCautare == "ID" && c.Id.ToString().Contains(criteriu)) ||
                (campCautare == "Nume" && c.Nume.ToLower().Contains(criteriu)) ||
                (campCautare == "Telefon" && c.Telefon.ToLower().Contains(criteriu))
            ).ToList();
        }

        public void ActualizeazaClient(Client clientActualizat)
        {
            int index = clienti.FindIndex(c => c.Id == clientActualizat.Id);
            if (index != -1)
            {
                clienti[index] = clientActualizat;
            }
            else
            {
                throw new Exception($"Clientul cu ID-ul {clientActualizat.Id} nu a fost găsit în memorie.");
            }
        }
    }
}