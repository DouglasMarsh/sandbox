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
class MIME_TYPE
{
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine()); // Number of elements which make up the association table.
        int Q = int.Parse(Console.ReadLine()); // Number Q of file names to be analyzed.

        var mimeTypes = new Dictionary<string, string>();

        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            mimeTypes.Add(inputs[0].ToLower(), inputs[1]);
        }
        for (int i = 0; i < Q; i++)
        {
            string FNAME = Console.ReadLine(); // One file name per line.
            if (!FNAME.Contains('.')) Console.WriteLine("UNKNOWN");
            else
            {
                var tokens = FNAME.Split('.');
                var extension = tokens.Last().ToLower();
                if( mimeTypes.ContainsKey( extension ))
                {
                    Console.WriteLine(mimeTypes[extension]);
                    continue;
                }
                Console.WriteLine("UNKNOWN");
            }
        }

    }
}