using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Boggle;
using CustomNetworking;

namespace MyBoggleService
{
    class Program
    {
        static void Main (String [] args)
        {
            HttpStatusCode status;
            
            User name = new User { Nickname = "Joe" };
            BoggleService service = new BoggleService();
            Token user = service.CreateUser(name, out status);
            Console.WriteLine(user.UserToken.ToString());
            Console.WriteLine(status.ToString());

            // This is our way of preventing the main thread from
            // exiting while the server is in use
            Console.ReadLine();

            SS testing ;
        }
    }
}
