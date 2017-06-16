using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedServiceSDK;
using FeedServiceSDK.Exceptions;

namespace FeedServiceClient
{
    class Program
    {
        static bool flag=false;
        static string typedString = string.Empty;

        static void Main(string[] args)
        {
            FeedServiceSDK.FeedServiceClient user = new FeedServiceSDK.FeedServiceClient();

            string login = null;
            string password = null;

            Console.WriteLine("1 - Login\n2 - Regiter new user");
            Console.Write("Choose operation: ");

            typedString = Console.ReadLine();
            if(int.TryParse(typedString, out int operation))
                switch (operation)
                {
                    case 1:
                        do
                        {
                            Console.WriteLine("Registration");
                            Console.WriteLine("Login: ");
                            login = Console.ReadLine();

                            Console.WriteLine("Password: ");
                            password = Console.ReadLine();
                        }
                        while (!user.Authorize(login, password).GetAwaiter().GetResult());
                        break;
                    case 2:
                        while (!flag)
                        {
                            Console.WriteLine("Sign in");
                            Console.WriteLine("Login: ");
                            login = Console.ReadLine();

                            Console.WriteLine("Password: ");
                            password = Console.ReadLine();

                            try
                            {
                                var result = user.Register(login, password).GetAwaiter().GetResult();
                                Console.WriteLine(result);
                                flag = true;
                            }
                            catch (FeedServiceException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        goto case 1;
                    default:
                        Console.WriteLine("There is no such operation.");
                        break;
                }
            Console.WriteLine("1 - Create collection\n2 - Get news from collection\n3 - Add feed to collection\n4 - Get all collections");
            Console.Write("Choose operation: ");

            typedString = Console.ReadLine();
            int collectionId = 0;
            if (int.TryParse(typedString, out operation))
                switch (operation)
                {
                    case 1:
                        Console.WriteLine("Create collection");
                        Console.Write("Type name of the collection: ");
                        typedString = Console.ReadLine();
                        collectionId = user.CreateCollection(typedString).GetAwaiter().GetResult();
                        goto case 3;
                    case 2:
                        while (!flag)
                        {
                            Console.WriteLine("Get news from collection");
                            Console.Write("Type ID of the collection: ");
                            typedString = Console.ReadLine();

                            if (int.TryParse(typedString, out collectionId))
                            {
                                var result = user.ReadNewsFromCollection(collectionId);
                                Console.WriteLine(result);
                                flag = true;
                            }
                            else
                                flag = false;
                        }
                        break;
                    case 3:
                        Console.WriteLine("Add feed to collection");
                        Console.Write("Collection ID: ");
                        while (!int.TryParse(Console.ReadLine(), out collectionId)) { Console.WriteLine("Not a number. Try again"); }
                        Console.Write("Feed type (RSS-0, RDF-1, Atom-2): ");
                        FeedType type;
                        while (!Enum.TryParse(Console.ReadLine(), out type)) { Console.WriteLine("Not a number. Try again"); }
                        Console.Write("URL: ");
                        string url = Console.ReadLine();
                        try
                        {
                            string result = user.AddFeedToCollection(collectionId, type, url).GetAwaiter().GetResult();
                        }
                        catch(FeedServiceException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                     goto case 3;
                    case 4:
                        Console.WriteLine("News");
                        foreach(var col in user.GetCollections().GetAwaiter().GetResult())
                        {
                            Console.WriteLine(col);
                        }
                        goto case 2;
                    default:
                        break;
                }

            
        }
    }
}
