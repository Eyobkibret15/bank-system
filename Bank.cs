using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace bank_system
{
    class Bank
    {
        private long Account_number { get; set; }
        private string Credit_card { get; set; }
        private string Pin { get; set; }
        string current_acc_number;
        int current_balance;
        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataReader table;

        Card card_obj = new Card();
        public void Creat_Account()
        {

            if (!File.Exists("./card.s3db"))
            {
                Creat_Database();
            }

            Random random = new Random();
            Open_database();
            table = cmd.ExecuteReader();

            bool first_check = false; // for first while loop
            bool second_check = false;  // for creation of credit card

            while (first_check == false)
            {
                Account_number = random.Next(000000001, 999999999);
                // read all table data line by line to check weather  the generated account_number exist or not
                while (table.Read())
                {
                    string temp_acc_number = table.GetString(1);  // get account_number of each user from the table

                    // second_check becomes true if the generated account number matchs
                    if (temp_acc_number != null)
                    {
                        second_check = (("400000" + Account_number) == temp_acc_number.Substring(0, 15));
                    }

                    first_check = second_check;

                    if (first_check)
                    {
                        // if acc_num exist it is to generate acc_num again
                        first_check = false;
                        break;
                    }
                }

                // creat cradit card using luhn algorthim if the generated account number does not exist in our database
                if (second_check == false)
                {
                    Credit_card = card_obj.Credit_card(Account_number);
                    break;
                }

            }

            table.Close();
            int pin = random.Next(0001, 9999);
            Pin = String.Format("{0:0000}", pin);  // to store as 4 digit string

            cmd.CommandText = "INSERT INTO card(number,pin ) VALUES(@Credit_card,@Pin)";
            cmd.Parameters.AddWithValue("@Credit_card", Credit_card);
            cmd.Parameters.AddWithValue("@Pin", Pin);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Your card has been created");
            Console.WriteLine("Your card number:\n" + Credit_card);
            Console.WriteLine("Your card PIN:\n" + Pin);
        }


        public void Logged_in()
        {
            Console.WriteLine("Enter your card number:");
            string user_card_num = Console.ReadLine();
            Console.WriteLine("Enter your PIN:");
            string user_pin = Console.ReadLine();
            if (card_obj.luhn_check(user_card_num) == false)
            {
                Console.WriteLine("please enter a valid card number");
                return;
            }
            Open_database();
            table = cmd.ExecuteReader();

            while (table.Read())
            {
                if (table.GetString(1) == user_card_num && table.GetString(2) == user_pin)
                {
                    current_acc_number = table.GetString(1);
                    current_balance = Convert.ToInt32(table.GetValue(3));
                    Console.WriteLine("You have successfully logged in!");
                    table.Close();
                    Menu();
                }

            }

            Console.WriteLine("Incorrect Card number or PIN!");

        }

        void Menu()
        {
            Console.WriteLine("1.Balance\n2.Add income\n3.Do transfer_amount\n4.Close account\n5.Log out\n0.Exit");
            Console.WriteLine("Enter your choice");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Balance: $");
                    Console.WriteLine(current_balance);
                    break;
                case "2":
                    Console.WriteLine("Enter income:");
                    try
                    {
                        int income = Convert.ToInt32(Console.ReadLine());
                        Deposit(income);
                    }
                    catch
                    {
                        Console.WriteLine("\n please enter only number");
                    }

                    break;
                case "3":

                    Open_database();
                    table = cmd.ExecuteReader();
                    Console.WriteLine("Transfer\nEnter card number:");
                    string new_transfer_account = Console.ReadLine();
                    if (card_obj.luhn_check(new_transfer_account) == false)
                    {
                        Console.WriteLine("please enter a valid card number");
                        return;
                    }
                    while (table.Read())
                    {
                        if (new_transfer_account == table.GetString(1) && new_transfer_account != current_acc_number)
                        {

                            int old_ammount = Convert.ToInt32(table.GetValue(3));
                            Console.WriteLine("Enter how much money you want to transfer_amount:");
                            int transfer_amount = 0;
                            try
                            {
                                transfer_amount = Convert.ToInt32(Console.ReadLine());

                            }
                            catch
                            {
                                Console.WriteLine("\n please enter only number");
                            }
                            table.Close();
                            bool transfer_validity = Transfer(new_transfer_account, transfer_amount, old_ammount);
                            if (transfer_validity)
                            {
                                cmd.CommandText = "UPDATE card SET balance = '" + current_balance + "'  WHERE number = '" + current_acc_number + "'";
                                cmd.ExecuteNonQuery();
                                Console.WriteLine("you transfer_amount the money sucessfully");
                            }

                            Menu();
                        }
                    }
                    if (new_transfer_account == current_acc_number)
                    {
                        Console.WriteLine("you can not transfer to your own account\nPlease try to add income");
                    }
                    else
                    {
                        Console.WriteLine("account number does'exist");
                    }

                    Menu();

                    break;
                case "4":
                    Open_database();
                    table.Close();
                    cmd.CommandText = "DELETE FROM card WHERE number = '" + current_acc_number + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("your account has been closed!");
                    Program.menu();
                    break;
                case "5":
                    Console.WriteLine("you are sucessfully logged out");
                    table.Close();
                    con.Close();
                    Program.menu();
                    break;
                default:
                    Environment.Exit(1);
                    break;

            }
            Menu();
        }

        public void Deposit(int income)
        {
            current_balance += income;
            table.Close();
            cmd.CommandText = "UPDATE card SET balance = '" + current_balance + "'  WHERE number = '" + current_acc_number + "'";
            cmd.ExecuteNonQuery();
            Console.WriteLine("you add money sucessfelly to your account");
        }
        public bool Transfer(string account, int amount, int old_amount)
        {
            string transfer_account = account;
            int transfer_amount = amount;
            int previous_amount = old_amount;
            if (current_balance - transfer_amount >= 0)
            {

                current_balance -= transfer_amount;
                transfer_amount += previous_amount;
                cmd.CommandText = "UPDATE card SET balance = '" + transfer_amount + "'  WHERE number = '" + transfer_account + "'";
                cmd.ExecuteNonQuery();
                return true;
            }
            else
            {
                Console.WriteLine("Not enough amount!");
                return false;
            }
        }
        public void Open_database()
        {

            con = new SQLiteConnection("Data Source=card.s3db; Version = 3; New = True; Compress = True; ");
            con.Open();
            string stm = "SELECT id,number,pin,balance FROM card";
            cmd = new SQLiteCommand(stm, con);
        }
        public void Creat_Database()
        {
            SQLiteConnection con;

            con = new SQLiteConnection("Data Source=card.s3db; Version = 3; New = True; Compress = True; ");

            con.Open();

            SQLiteCommand cmd;
            cmd = con.CreateCommand();

            cmd.CommandText = "CREATE TABLE card(id INTEGER PRIMARY KEY,number TEXT UNIQUE,pin TEXT,balance INTEGER DEFAULT 0)";
            cmd.ExecuteNonQuery();


        }
    }
}
