using System;
using System.IO;
using System.Text.RegularExpressions;

namespace csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var expected = "NONONONONONONONONONONONONONONONONONONONO";

            using( var reader = new StringReader(expected))
            {
                using( var writer = new StringWriter())
                {
                    //bilbo.Process(reader, writer, Console.Error);
                    //var cmd = writer.ToString().Trim();
                    //var cmd = "------------>-------[<-.+.>-]";
                    var actual = bilbo.Execute("<-[>+]");
                
                    if( !actual.Equals(expected))
                    {
                        throw new ApplicationException(
                            string.Format("Invalid Command. Expected output {0}. Actual {1}", expected, actual));
                    }
                }
            }
            Console.Write("Press Any key to exit");
            Console.Read();
        }
    }
}
