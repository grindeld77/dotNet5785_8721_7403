using System;

namespace targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome8721();
            Welcome7403();
            Console.ReadKey();

        }
        static partial void Welcome7403();
        private static void Welcome8721()
        {
            Console.WriteLine("Enter your name:");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}