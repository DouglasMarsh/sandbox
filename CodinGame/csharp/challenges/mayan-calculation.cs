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
class mayan_calculation
{
    static void Main(string[] args)
    {
        var parsedMayan = 
            Enumerable.Range(0, 20)
                      .Select(l => new List<string>()).ToArray();

        string[] inputs = Console.ReadLine().Split(' ');
        int L = int.Parse(inputs[0]);
        int H = int.Parse(inputs[1]);
        for (int i = 0; i < H; i++)
        {
            string numeral = Console.ReadLine();
            int idx = 0;
            int base10 = 0;
            do
            {
                parsedMayan[base10].Add(numeral.Substring(idx, L));

                base10 = base10 + 1;
                idx += L;
            } while (idx < numeral.Length);
        }

        var baseMayan = new MayanNumeral[20];
        for( int i = 0; i < 20; i++ )
        {
            baseMayan[i] = new MayanNumeral(i, parsedMayan[i]);
            Console.Error.WriteLine("#: " + i);
            Console.Error.WriteLine(baseMayan[i]);
        }

        long s1 = 0;
        int s1Pow = (int.Parse(Console.ReadLine()) / H) - 1;
        for (int i = s1Pow; i >= 0; i--)
        {
            var mn = new List<string>();
            for(int ni = 0; ni < H; ni++)
            {
                mn.Add(Console.ReadLine());
            }
            int base10 = baseMayan.First(m => m.Equals(mn)).Base10;
            Console.Error.WriteLine("S1: Base10: {1}, Pow: {0}", i, base10);
            Console.Error.WriteLine(string.Join("\n", mn));
            s1 += (base10 * (int)Math.Pow(20,i));
        }
        Console.Error.WriteLine("S1: " + s1);

        long s2 = 0;
        int s2Pow = (int.Parse(Console.ReadLine()) / H) - 1;
        for (int i = s2Pow; i >= 0; i--)
        {
            var mn = new List<string>();
            for(int ni = 0; ni < H; ni++)
            {
                mn.Add(Console.ReadLine());
            }

            int base10 = baseMayan.First(m => m.Equals(mn)).Base10;
            Console.Error.WriteLine("S2: Base10: {1}, Pow: {0}", i, base10);
            Console.Error.WriteLine(string.Join("\n", mn));
            s2 += (base10 * (int)Math.Pow(20,i));
        }
        Console.Error.WriteLine("S2: " + s2);        

        string operation = Console.ReadLine();

        
        long base10Result = 0;
        switch (operation)
        {
            case "+":
                base10Result = s1 + s2;
                break;
            case "-":
                base10Result = s1 - s2;
                break;
            case "/":
                base10Result = s1 / s2;
                break;
            case "*":
                base10Result = s1 * s2;
                break;
            
        }
        Console.Error.WriteLine("Operation: {0} {1} {2} = {3}", s1, operation, s2, base10Result);

        Console.Error.WriteLine();
        WriteMayanNumber(base10Result, baseMayan);
        
    }

    static void WriteMayanNumber(long value, IEnumerable<MayanNumeral> numerals)
    {
        var places = new List<long>();
        long wn = value;
        Console.Error.WriteLine("Value: " + wn);
        while( wn > 19 )
        {
            long r = wn % 20;
            wn = wn / 20;

            Console.Error.WriteLine("wn: {0}; remainder: {1}", wn, r);

            places.Add( r );
        }
        places.Add(wn);

        places.Reverse();
        Console.Error.WriteLine("Mayan#: " + string.Join(" ", places));
        foreach( var p in places )
        {
            Console.WriteLine(numerals.First(n => n.Base10 == p ));
        }
    }

    struct MayanNumeral
    {
        public MayanNumeral(int base10, List<string> ascii)
        {
            Base10 = base10;
            Ascii = ascii;
        }
        public int Base10 { get; set; }
        public List<string> Ascii { get; set; }

        public bool Equals(MayanNumeral n)
        {
            return Base10 == n.Base10;          
        }
        public bool Equals(IEnumerable<string> ascii)
        {
            if (ascii == null || !ascii.Any() || Ascii.Count != ascii.Count())
            {
                return false;
            }
            
            for(int i = 0; i < Ascii.Count; i++)
            {
                if( Ascii[i] != ascii.ElementAt(i)) {
                    return false;
                }
            }  
            return true;          
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return Equals((MayanNumeral)obj);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 0;
            foreach( var l in Ascii )
            {
                hash = hash ^ l.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return string.Join("\n", Ascii);
        }

    }
    
}