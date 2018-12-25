using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class the_gift
{
    static void Main(string[] args)
    {
        int participants = int.Parse(Console.ReadLine());
        int cost = int.Parse(Console.ReadLine());

        int optimalAvg = cost/participants;
        var budgets = new List<int>();
        for (int i = 0; i < participants; i++)
        {
            budgets.Add(int.Parse(Console.ReadLine()));
        }
        
        if( budgets.Sum() < cost )
        {
            Console.WriteLine("IMPOSSIBLE");
        }
        else
        {
            foreach( var budget in budgets.OrderBy(b => b) )
            {
                if( budget < optimalAvg )
                {
                    Console.WriteLine(budget);
                    cost -= budget;
                }
                else
                {
                    Console.WriteLine(optimalAvg);
                    cost -= optimalAvg;
                }
                if( -- participants > 0)
                    optimalAvg = cost / participants;            
            }
        }
    }
}