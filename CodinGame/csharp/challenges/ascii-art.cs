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
class Ascii_Art
{
    static void Main(string[] args)
    {
        int L = int.Parse(Console.ReadLine());
        int H = int.Parse(Console.ReadLine());
        string T = Console.ReadLine();

        var characters = Enumerable.Range(0, 27)
                            .Select(l => new List<string>()).ToArray();

        var word = new List<List<string>>();

        for (int i = 0; i < H; i++)
        {
            string ROW = Console.ReadLine();

            int idx = 0;
            int key = 0; // A
            do
            {
                characters[key].Add(ROW.Substring(idx, L));

                key = key + 1;
                idx += L;
            } while (idx < ROW.Length);
        }

        foreach(char c in T)
        {

            int key = 26; // ?
            if (c >= 65 && c <= 90)
            {
                key = c - 65;
            }                
            else if ( c >= 97 && c <= 122)
            {
                key = c - 97;
            }
            
            word.Add(characters[key]);
        }

        for (int i = 0; i < H; i++)
        {
            foreach (var l in word)
            {
                Console.Write(l[i]);
            }
            Console.WriteLine();
        }
    }
    

}