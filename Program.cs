using System;

namespace bank_system
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*******WELCOME********");
            menu();
        }
        public static void menu()
        {
            Bank bank_obj = new Bank();
            Console.WriteLine("1. Creat an account \n2. Log into account \n3. Exit");
            Console.WriteLine("please enter your choice number:");
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                bank_obj.Creat_Account();
            }
            else if (choice == "2")
            {
                bank_obj.Logged_in();
            }
            else if (choice == "3")
            {
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Invalid choice  please enter valid choice number:");
            }
            Console.WriteLine();
            menu();
        }

    }
}
