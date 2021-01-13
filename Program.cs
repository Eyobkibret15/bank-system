using System;

namespace bank_system
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Bank b = new Bank();
            b.Card_generator();
        }
    }
}
