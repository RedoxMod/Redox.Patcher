using System;
using System.IO;

using Redox.Patcher.Games;

namespace Redox.Patcher
{
    class Program
    {
        static Rust Rust;
        static void Main(string[] args)
        {
            if (!Directory.Exists("Patched"))
                Directory.CreateDirectory("Patched");
            Console.Title = "Redox patcher";
            Console.WriteLine("Choose the game you want to patch by selecting one of the listed numbers:\n 1. Rust");

            string result = Console.ReadLine();
            if(int.TryParse(result, out int number))
            {
                switch(number)
                {
                    case 1:
                        Rust = new Rust();
                        Rust.Patch();
                        break;
                }
            }
        }
    }
}
