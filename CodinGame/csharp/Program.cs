using System;
using System.IO;
using System.Text.RegularExpressions;

namespace csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var phrase = "BABCDEDCBABCDCB";
            //++.-.+.+.+.+.-.-.-.-.+.+.+.-.-.

            using( var reader = new StringReader(phrase))
            {
                bilbo.Process(reader, Console.Out, Console.Error);
            }
            Console.Write("Press Any key to exit");
            Console.ReadKey(false);
        }
    }
}
