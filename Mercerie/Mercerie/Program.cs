using EvidentaMercerie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mercerie
{
    public class Program
    {
        static void Main()
        {
            List<Client> clienti = new List<Client>();
            List<Produs> produse = new List<Produs>();


            bool ruleaza = true;

            while (ruleaza)
            {
                Console.WriteLine("\n===== Meniu Principal =====");
                Console.WriteLine("1. Adauga Client Nou");
                Console.WriteLine("2. Vizualizare Lista Clienti");
                Console.WriteLine("3. Adauga Comanda");
                Console.WriteLine("4. Cauta Client");
                Console.WriteLine("5. Salveaza in fisier");
                Console.WriteLine("6. Iesire");
                Console.Write("Alege o optiune: ");

                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        Console.Write("Introdu ID client: ");
                        int idClient = int.Parse(Console.ReadLine());

                        Console.Write("Introdu nume client: ");
                        string numeClient = Console.ReadLine();

                        Console.Write("Introdu telefon client: ");
                        string telefonClient = Console.ReadLine();

                        clienti.Add(new Client(idClient, numeClient, telefonClient));
                        Console.WriteLine("Client adaugat cu succes!");
                        break;

                    case "2":
                        Console.WriteLine("\n===== Lista Clienti =====");
                        foreach (var client in clienti)
                        {
                            Console.WriteLine(client);
                        }
                        break;

                    case "3":
                        Console.Write("Introdu ID client pentru comanda: ");
                        int idCautat = int.Parse(Console.ReadLine());

                        Client clientGasit = clienti.Find(c => c.Id == idCautat);

                        if (clientGasit != null)
                        {
                            Console.WriteLine("Alege tipul produsului:");
                            foreach (TipProdus tip in Enum.GetValues(typeof(TipProdus)))
                            {
                                Console.WriteLine($"{(int)tip}. {tip}");
                            }

                            Console.Write("Introdu optiunea: ");
                            TipProdus tipProdus = (TipProdus)int.Parse(Console.ReadLine());

                            Console.Write("Introdu numele produsului: ");
                            string numeProdus = Console.ReadLine();

                            Console.Write("Introdu pretul produsului: ");
                            double pretProdus = double.Parse(Console.ReadLine());

                            Produs nouProdus = new Produs(produse.Count + 1, numeProdus, pretProdus, tipProdus);
                            clientGasit.AdaugaComanda(nouProdus);

                            Console.WriteLine("Comanda adaugata cu succes!");
                        }
                        else
                        {
                            Console.WriteLine("Clientul nu a fost gasit.");
                        }
                        break;

                    case "4":
                        Console.Write("Introdu ID client pentru cautare: ");
                        int idDeCautat = int.Parse(Console.ReadLine());

                        Client clientCautat = clienti.Find(c => c.Id == idDeCautat);

                        if (clientCautat != null)
                        {
                            Console.WriteLine(clientCautat);
                        }
                        else
                        {
                            Console.WriteLine("Clientul nu a fost gasit.");
                        }
                        break;

                    case "5":
                        AdministrareClientiFisier admin = new AdministrareClientiFisier();
                        admin.SalveazaTotiClientii(clienti);
                        Console.WriteLine("Datele au fost salvate in fisier!");
                        break;

                    case "6":
                        ruleaza = false;
                        Console.WriteLine("Iesire din aplicatie.");
                        break;

                    default:
                        Console.WriteLine("Optiune invalida. Reincearca.");
                        break;
                }
            }
        }
    }
}
