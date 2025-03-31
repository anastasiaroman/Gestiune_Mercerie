using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mercerie
{
    public class Program
    {
        static void Main(string[] args)
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
                Console.WriteLine("5. Iesire");
                Console.Write("Optiune: ");

                var optiune = Console.ReadLine();
                switch (optiune)
                {
                    case "1":
                        Console.Write("Nume client: ");
                        var nume = Console.ReadLine();
                        var client = new Client(adminMem.GetClienti().Count + 1, nume);
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
                        }
                        else
                        {
                            Console.WriteLine("Client inexistent!");
                        }
                        break;
                    case "4":
                        Console.Write("Nume client: ");
                        string cautaNume = Console.ReadLine();
                        var gasit = adminMem.CautaClient(cautaNume);
                        Console.WriteLine(gasit != null ? gasit.ToString() : "Client inexistent!");
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            }
        }
    }
}
