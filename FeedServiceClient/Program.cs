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
            while (!user.Authorize(login, "pass").GetAwaiter().GetResult());


        }
    }
}
