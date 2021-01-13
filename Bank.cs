using System;
using System.Collections.Generic;
using System.Text;

namespace bank_system
{
    class Bank
    {
        public void Menu()
        {
            Card_generator();

        }

        public void Card_generator()
        {
            Card card_obj = new Card();
            Random random = new Random();
            int Account_number = random.Next(0, 999999999);
            Console.WriteLine(card_obj.Credit_card(Account_number));
        }
    }
}
