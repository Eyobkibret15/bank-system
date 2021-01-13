using System;
using System.Collections.Generic;
using System.Linq;

public class Card
{
    public string Credit_card(int num)
    {
        int number = num;
        int[] IIN = { 4, 0, 0, 0, 0, 0 };
        int[] Account_number = new int[10];

        for (int i = 8; i >= 0; i--)
        {
            Account_number[i] = number % 10;
            number = number / 10;
        }

        var result1 = IIN.Concat(Account_number);
        int[] full_card = result1.ToArray();
        int temp = 0;
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
            full_card[15] = 10 - sum % 10;

        }

        return string.Join("", full_card);
    }
}
