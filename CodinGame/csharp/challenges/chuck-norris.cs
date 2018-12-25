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
class Chuck_Norris
{
    static void Main(string[] args)
    {
        string MESSAGE = Console.ReadLine();

        // 1. Convert string to binary
        string binary = string.Join("", MESSAGE.Select(c => Convert.ToString((int)c, 2).PadLeft(7,'0')));
        Console.Error.WriteLine("{0} in binary: {1}", MESSAGE, binary);

        // Convert to Chuck-Norris
        var cn = new StringBuilder();
        int idx = 0;
        while (idx < binary.Length)
        {
            char c = binary[idx];
            if (c == '1') cn.Append("0 ");
            else cn.Append("00 ");

            cn.Append("0");
            while (++idx < binary.Length && binary[idx] == c) cn.Append("0");

            cn.Append(" ");
        }

        Console.WriteLine(cn.ToString().TrimEnd());
    }
}