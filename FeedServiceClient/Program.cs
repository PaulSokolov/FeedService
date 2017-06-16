using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FeedServiceSDK.FeedServiceClient user = new FeedServiceSDK.FeedServiceClient();

            string login = null;
            string password = null;
            do
            {
                Console.WriteLine("Login: ");
                login = Console.ReadLine();

                Console.WriteLine("Password: ");
                password = Console.ReadLine();
            }
            while (!user.Authorize(login, password).GetAwaiter().GetResult());

            foreach (var col in user.GetCollections().GetAwaiter().GetResult())
            {
                Console.WriteLine(col);
            }
            Console.Write("Choose collection: ");
            var answer = Console.ReadLine();
            int colId = int.Parse(answer.ToString());

            Console.WriteLine("News\n\n");
            foreach (var col in user.ReadNewsFromCollection(colId).GetAwaiter().GetResult())
            {
                Console.WriteLine(col);
            }
            // user.AddFeedToCollection("");
        }
    }
}
