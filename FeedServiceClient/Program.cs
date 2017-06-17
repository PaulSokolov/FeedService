using System;
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
            object res = null;
            FeedServiceSDK.FeedServiceClient user = new FeedServiceSDK.FeedServiceClient();
            user.FeedServiceSuccessMessage += User_FeedServiceSuccessMessage; ;
            
            Console.ReadKey();
            string login = null;
            string password = null;
            do
            {
                flag = false;
                Console.WriteLine("1 - Sign in");
                Console.WriteLine("2 - Register new user");
                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        Console.Clear();
                        do
                        {
                            Console.WriteLine("Sign in");
                            Console.WriteLine("Login: ");
                            login = Console.ReadLine();

                            Console.WriteLine("Password: ");
                            password = Console.ReadLine();
                            Console.Clear();
                            Console.WriteLine("Wait...");
                        }
                        while (!user.Authorize(login, password).GetAwaiter().GetResult());
                        Console.Clear();
                        break;
                    case '2':
                        Console.Clear();
                        while (!flag)
                        {
                            Console.WriteLine("Registration");
                            Console.WriteLine("Login: ");
                            login = Console.ReadLine();

                            Console.WriteLine("Password: ");
                            password = Console.ReadLine();

                            Console.Clear();
                            Console.WriteLine("Wait...");
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
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
            while (flag);

            do {
                Console.WriteLine("1 - Create collection");
                Console.WriteLine("2 - Get news from collection");
                Console.WriteLine("3 - Add feed to collection");
                Console.WriteLine("4 - Get all collections");
                Console.WriteLine("5 - Exit");

                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        Console.Clear();
                        Console.WriteLine("Create collection");
                        Console.Write("Type name of the collection: ");

                        typedString = Console.ReadLine();

                        Console.Clear();
                        Console.WriteLine("Wait...");

                        var collectionId = user.CreateCollection(typedString).GetAwaiter().GetResult();
                        
                        Console.WriteLine($"Collection Id - {collectionId}");
                        GoToMenu();
                        break;
                    case '2':
                        Console.Clear();
                        bool tmpFlag = false;
                        while (!tmpFlag)
                        {
                            Console.WriteLine("Get news from collection");
                            Console.Write("Type ID of the collection: ");
                            typedString = Console.ReadLine();
                            Console.Clear();
                            Console.WriteLine("Wait...");
                            if (int.TryParse(typedString, out collectionId))
                            {
                                try
                                {
                                    var result = user.ReadNewsFromCollection(collectionId).GetAwaiter().GetResult();
                                    foreach (var item in result)
                                    {
                                        Console.WriteLine(item);
                                    }
                                    tmpFlag = true;
                                }
                                catch (FeedServiceException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                                tmpFlag = false;
                        }
                        GoToMenu();
                        break;
                    case '3':
                        Console.Clear();
                        Console.WriteLine("Add feed to collection");
                        Console.Write("Collection ID: ");
                        while (!int.TryParse(Console.ReadLine(), out collectionId)) { Console.WriteLine("Not a number. Try again"); }
                        Console.Write("Feed type (RSS-0, RDF-1, Atom-2): ");
                        FeedType type;
                        while (!Enum.TryParse(Console.ReadKey().KeyChar.ToString(), out type)) { Console.WriteLine("Not a number. Try again"); }
                        Console.WriteLine();
                        Console.Write("URL: ");
                        string url = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("Wait...");
                        try
                        {
                            string result = user.AddFeedToCollection(collectionId, type, url).GetAwaiter().GetResult();
                            Console.WriteLine(result);
                        }
                        catch (FeedServiceException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        GoToMenu();
                        break;
                    case '4':
                        Console.Clear();
                        Console.WriteLine("News");
                        foreach (var col in user.GetCollections().GetAwaiter().GetResult())
                        {
                            Console.WriteLine(col);
                        }

                        GoToMenu();
                        break;
                    case '5':
                        flag = true;
                        Console.Clear();
                        break;
                }
            }
            while (!flag);
            
        }

        private static void GoToMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Press enter to close news and go to Menu");
            Console.ReadLine();
            Console.Clear();
        }

        private static void User_FeedServiceSuccessMessage(object sender, EventArgs e)
        {
            Console.WriteLine(((SuccessEventArgs)e).Message);
        }
    }
}
