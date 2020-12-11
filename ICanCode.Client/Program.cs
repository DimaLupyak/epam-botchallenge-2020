using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICanCode.Client
{
    class Program
    {
        // you can get this code after registration on the server with your email
        static string ServerUrl = "https://epam-botchallenge.com/codenjoy-contest/board/player/uda5i8hzlz8khmmvcv1s?code=7996357795976478622";

        static void Main(string[] args)
        {
            //Console.SetWindowSize(Console.LargestWindowWidth - 3, Console.LargestWindowHeight - 3);

            // creating custom AI client
            var bot = new YourSolver(ServerUrl);

            // starting thread with playing game
            (new Thread(bot.Play)).Start();

            // waiting for any key
            Console.ReadKey();

            // on any key - asking AI client to stop.
            bot.InitiateExit();
        }
    }
}
