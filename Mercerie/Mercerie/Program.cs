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
            var adminMem = new AdministrareClientiMemorie();
            var adminFis = new AdministrareClientiFisier();
            var stoc = new Stoc();

            while (true)
            {
                Console.WriteLine("1. Client nou");
                Console.WriteLine("2. Vizualizare lista clienti");
                Console.WriteLine("3. Adauga comanda");
                Console.WriteLine("4. Cauta client");
                Console.WriteLine("5. Salveaza clienti in fisier");
                Console.WriteLine("6. Iesire");
                Console.Write("Optiune: ");

                var optiune = Console.ReadLine();
                switch (optiune)
                {
                    case "1":
                        Console.Write("Nume client: ");
                        var nume = Console.ReadLine();
                        Console.Write("Telefon client: ");
                        var telefon = Console.ReadLine();
                        var client = new Client(adminMem.GetClienti().Count + 1, nume, telefon);
                        adminMem.AdaugaClient(client);
                        adminFis.SalveazaClient(client);
                        break;
                    case "2":
                        foreach (var c in adminMem.GetClienti())
                            Console.WriteLine(c);
                        break;
                    case "3":
                        Console.Write("ID client: ");
                        int idClient = int.Parse(Console.ReadLine());
                        Console.Write("Nume produs: ");
                        string numeProdus = Console.ReadLine();
                        Console.Write("Pret produs: ");
                        double pret = double.Parse(Console.ReadLine());
                        var produs = new Produs(stoc.Produse.Count + 1, numeProdus, pret);
                        stoc.AdaugaProdus(produs);
                        var clientExist = adminMem.GetClienti().FirstOrDefault(c => c.Id == idClient);
                        if (clientExist != null)
                        {
                            clientExist.AdaugaComanda(produs);
                            Console.WriteLine("Comanda adaugata!");
                            adminFis.SalveazaTotiClientii(adminMem.GetClienti());
                        }
                        else
                        {
                            Console.WriteLine("Client inexistent!");
                        }
                        break;
                    case "4":
                        Console.Write("Nume client: ");
                        string cautaNume = Console.ReadLine();
                        var gasit = adminFis.CitesteClienti().FirstOrDefault(c => c.Nume.Equals(cautaNume, StringComparison.OrdinalIgnoreCase));
                        Console.WriteLine(gasit != null ? gasit.ToString() : "Client inexistent in fisier!");
                        break;
                    case "5":
                        adminFis.SalveazaTotiClientii(adminMem.GetClienti());
                        Console.WriteLine("Datele au fost salvate in fisier!");
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            }
        }
    }
}
