using System;
using System.Collections.Generic;
using System.Linq;

public class Card
{
    public string Credit_card(long num)
    {
        int number = Convert.ToInt32(num);
        int[] IIN_number = { 4, 0, 0, 0, 0, 0 };
        int[] Account_number = new int[10];

        for (int i = 8; i >= 0; i--)
        {
            Account_number[i] = number % 10;
            number = number / 10;
        }

        var result1 = IIN_number.Concat(Account_number);
        int[] full_card = result1.ToArray();
        int temp;
        int sum = 0;

        for (int i = 0; i < 15; i++)
        {
            if (i % 2 == 0)
            {
                temp = full_card[i] * 2;
                if (temp > 9)
                {
                    temp = temp - 9;
                }

                sum += temp;
            }
            else
            {
                sum += full_card[i];
            }
        }
        if (sum % 10 != 0)
        {
            full_card[15] = 10 - sum % 10;  // updating the last item 

        }
        return string.Join("", full_card);
    }

    public bool luhn_check(string num)
    {
        if (num.Length == 16)
        {
            long number = Convert.ToInt64(num);
            long extract1 = number % 100000000000;   // to get only 10 digit account number from credit card 16 digit
            long extract2 = extract1 / 10;   // to drop last item in order to chuck the lhun algorthim

            string check = Credit_card(extract2);
            // if the user account number equals to checked lhun algorthim return ture 
            if (num == check)
            {
                return true;
            }

            return false;
        }
        else
        {
            return false;
        }
    }
}