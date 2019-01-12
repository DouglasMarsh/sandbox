using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;


/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
public class bilbo
{
    static TextReader _input;
    static  TextWriter _output;
    static TextWriter Error;
    
    static Regex hasDuplicates = new Regex("(.+?)\\1+", RegexOptions.Compiled);
    static char[] zones = string.Empty.PadRight(30, ' ').ToArray();
    public static void Main(string[] args) => Process(Console.In, Console.Out, Console.Error);
    public static void Process(TextReader input, TextWriter output, TextWriter error)
    {
        string magicPhrase = input.ReadLine();
        string instruction = string.Empty;

        int z = 0;
        Tuple<int, string> solution = null;
        int i = 0;
        while(i < magicPhrase.Length)
        {
            error.WriteLine("Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
            string sequence = IdentifySequence(magicPhrase.Skip(i).ToArray());
            if( !string.IsNullOrEmpty(sequence))
            {
                error.WriteLine("Processing sequence: " + sequence);
                solution = ProcessSequence(sequence, z);
                instruction += solution.Item2;
                z = solution.Item1;
                i += sequence.Length;

                error.WriteLine("After Sequence: Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
                continue;
            }

            var duplicate = IdentifyDuplicates( string.Concat(magicPhrase.Skip(i)) );
            if( duplicate != null )
            {
                error.WriteLine("Processing duplicate {0} * {1}", duplicate.Item1, duplicate.Item2);
                
                solution = ProcessDuplicatePhrase(duplicate.Item1, duplicate.Item2, z);
                instruction += solution.Item2;
                z = solution.Item1;
                i += duplicate.Item1.Length * duplicate.Item2;
                
                error.WriteLine("After Duplicate: Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
                continue;
            }

            error.WriteLine("Processing rune: " + magicPhrase[i]);
                
            solution = ProcessRune(magicPhrase[i], z);
            instruction += solution.Item2;
            z = solution.Item1;
            i++;
                
            error.WriteLine("After ProcessRune: Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
                
        }
        while(instruction.Contains("<>") || instruction.Contains("><") || instruction.Contains("+-") || instruction.Contains("-+"))
        {
            instruction = instruction.Replace("><","");
            instruction = instruction.Replace("<>","");
            instruction = instruction.Replace("+-","");
            instruction = instruction.Replace("-+","");
        }

        output.WriteLine(instruction);
    }
    static Tuple<int, string> ProcessRune( char rune, int z)
    {
        var solution = ProcessPhrase( new[] { rune }, z);
        Array.Copy(solution.Item1, zones, 30);

        return new Tuple<int, string>(solution.Item2, solution.Item3);
    }
    static Tuple<char[], int,string> ProcessPhrase(char[] phrase, int z)
    {
        var memory = new char[30];
        Array.Copy(zones, memory,30);
        
        var solutions = new List<Tuple<int, string>>();
        string cmd = string.Empty;
        for (int i = 0; i < phrase.Length; i++)
        {
            char rune = phrase[i];

            solutions.Clear();
            
            if (zones.Contains(rune))
            {
                solutions.Add(MoveToRune(rune, z));
            }

            solutions.Add(new Tuple<int, string>(z, SetRune(rune, z)));

            var m2s = MoveToSpace(z, false);
            solutions.Add(new Tuple<int, string>(m2s.Item1, m2s.Item2 + SetRune(rune, m2s.Item1)));

            m2s = MoveToSpace(z, true);
            solutions.Add(new Tuple<int, string>(m2s.Item1, m2s.Item2 + SetRune(rune, m2s.Item1)));

            var solution = solutions.OrderBy(s => s.Item2.Length).First();

            z = solution.Item1;
            zones[z] = rune;
            cmd += solution.Item2 + ".";
        }

        var retval = new Tuple<char[], int, string>(new char[30], z, cmd);
        Array.Copy(zones, retval.Item1,30);    
        Array.Copy(memory, zones,30);

        return retval;
    }
    
    static string IdentifySequence(char[] phrase)
    {
        int i = 0;
        int direction = 0;
        string sequence = "";
        while( i+1 < phrase.Length)
        {
            char c = phrase[i];
            if( direction == 0)
            {
                if( phrase[i+1] == c+1) direction = 1;
                else if( phrase[i+1] == c-1) direction = -1;
                else break;

                sequence += phrase[i];                
            }
            
            if( phrase[i+1] == c+direction)
            {
                sequence += phrase[++i];              
                continue;              
            }
            break;
        }

        return sequence;
    }
    static Tuple<int, string> ProcessSequence(string sequence, int z)
    {
        
        string cmd = string.Empty;
        int nz = z;
        if( string.IsNullOrEmpty(sequence) ) return null;

        int direction = sequence[1] - sequence[0];
        
        if( sequence.Length == 26) // full alphabet
        {            
            if(zones[z] == (direction < 0 ? 'Z' : 'A'))
            {
                return new Tuple<int, string>(z, direction < 0 ? "[.-]" : "[.+]");
            }
            else
            {
                Tuple<int, string> m = null;
                if (zones.Contains(' '))
                {
                    m = MoveToSpace(z);
                    var mb = MoveToSpace(z, true);
                    if (mb.Item2.Length < m.Item2.Length) m = mb;
                }
                cmd = ClearRune(z);
                if (m != null)
                {
                    if (m.Item2.Length < cmd.Length)
                    {
                        z = m.Item1;
                        cmd = m.Item2;
                    }
                }
                return new Tuple<int, string>(z, cmd + (direction < 0 ? "-[.-]" : "+[.+]"));
            }
        }
        else 
        {
            Tuple<char[],int,string> solution = null;
            var memory = new char[30];
            Array.Copy(zones, memory,30);
        
            var solutions = new List<Tuple<char[],int,string>>();
            char rune = sequence[0];

            if (zones.Contains(rune))
            {
                var mtr = MoveToRune(rune, z);
                cmd = mtr.Item2;
                nz = mtr.Item1+1;

                // write counter
                cmd += ">";
                cmd += SetRune((char)(64 + sequence.Length), nz );

                // write the loop
                cmd += direction < 0 ? "[<.->-]" : "[<.+>-]";

                solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
                Array.Copy(zones, solution.Item1,30);  
                
                solutions.Add(solution);  
                Array.Copy(memory, zones, 30);
            }

            #region Sequence with Loop
            cmd = SetRune(rune, z);
            nz = z+1;

            // write counter
            cmd += ">";
            cmd += SetRune((char)(64 + sequence.Length), nz );

            // write the loop
            cmd += direction < 0 ? "[<.->-]" : "[<.+>-]";

            solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
            Array.Copy(zones, solution.Item1,30);  
            
            solutions.Add(solution);  
            Array.Copy(memory, zones, 30);
            #endregion

            // Sequence without loop
            solution = ProcessPhrase(sequence.ToArray(), z);
            solutions.Add(solution);  
            Array.Copy(memory, zones, 30);

            solution = solutions.OrderBy(s => s.Item3.Length).First();
            
            Array.Copy(solution.Item1, zones, 30);
            return new Tuple<int, string>(solution.Item2, solution.Item3);
        }
        
    }
    
    static Tuple<string,int> IdentifyDuplicates(string phrase)
    {
        var match = hasDuplicates.Match(phrase);
        if( match.Success && match.Index == 0 )
        {
            var dup = match.Groups[1].Value;
            int count = match.Value.Length / dup.Length;
            
            return new Tuple<string,int>(dup, Math.Min(26,count));
        }
        return null;
    }
    static Tuple<int, string> ProcessDuplicatePhrase(string phrase, int repeat, int z)
    {
        var memory = new char[30];
        Array.Copy(zones, memory, 30);

        string cmd = string.Empty;
        int nz = z;

        string sZones = new string(zones);
        Tuple<char[], int,string> solution = null;
        var solutions = new List<Tuple<char[], int,string>>();

        // does zones contain the phrase        
        if(sZones.Contains(phrase))
        {
            // find location of phrase that is nearest to current location
            int phraseIdx = sZones.IndexOf(phrase);
            int distTo = 30;
            do
            {
                if( Math.Abs(distTo) > Math.Abs(phraseIdx - z)) 
                    distTo = phraseIdx - z;

                if( phraseIdx + phrase.Length >= 30) break;

                phraseIdx = sZones.IndexOf(phrase, phraseIdx + phrase.Length);
            } while( phraseIdx > 0);

            // move to location
            nz = z + distTo + phrase.Length;
            if( nz < 0 ) nz = 30 - nz;
            if( nz > 29 ) nz = nz - 30;

            cmd = MoveToAddress(z, nz);
           
            // write counter
            //Console.Error.WriteLine("Contains: Writing counter value {0} at Memory Pos: {1}", (char)(64 + repeat), nz);
            cmd += SetRune((char)(64 + repeat), nz );

            // write the loop
            cmd += string.Format("[{0}{1}-]", 
                        "".PadRight(phrase.Length,'<'), 
                        string.Join("",Enumerable.Range(0, phrase.Length).Select(i => ".>")));

            
            solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
            Array.Copy(zones, solution.Item1,30);  
            
            solutions.Add(solution);  
            //Console.Error.WriteLine("Contains: Solution Memory: " + string.Concat(solution.Item1.Select(c => c == ' ' ? '-' : c)));
            Array.Copy(memory, zones, 30);
        }

        // does zones contain the reverse phrase  
        string rPhrase = string.Concat(phrase.Reverse());     
        if(sZones.Contains(rPhrase))
        {
            cmd = string.Empty;
            // find location of phrase that is nearest to current location
            int phraseIdx = sZones.IndexOf(rPhrase);
            int distTo = 30;
            do
            {
                if( Math.Abs(distTo) > Math.Abs(phraseIdx - z)) 
                    distTo = phraseIdx - z;

                if( phraseIdx + rPhrase.Length >= 30) break;

                phraseIdx = sZones.IndexOf(rPhrase, phraseIdx + phrase.Length);
            } while( phraseIdx > 0);

            // move to location
            nz = z + distTo;
            nz--;
            //Console.Error.WriteLine("ContainsR: nz" + nz);

            if( nz < 0 ) nz = 30 + nz;
            if( nz > 29 ) nz = nz - 30;
            cmd = MoveToAddress(z, nz);
            //Console.Error.WriteLine("ContainsR: Moved {0} units from {1} to {2}", distTo, z, nz);
            

            // write counter
            //Console.Error.WriteLine("ContainsR: Writing counter value {0} at Memory Pos: {1}", (char)(64 + repeat), nz);
            cmd += SetRune((char)(64 + repeat), nz );

            // write the loop
            cmd += string.Format("[{0}{1}-]", 
                        "".PadRight(phrase.Length,'>'), 
                        string.Join("",Enumerable.Range(0, rPhrase.Length).Select(i => ".<")));

            
            solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
            Array.Copy(zones, solution.Item1,30);  
            
            solutions.Add(solution);  
            //Console.Error.WriteLine("ContainsR: Solution Memory: " + string.Concat(solution.Item1.Select(c => c == ' ' ? '-' : c)));
            Array.Copy(memory, zones, 30);        
        }
        
        // write command for Phrase then write loop
        var ps = ProcessPhrase(phrase.ToArray(), z);
        Array.Copy(ps.Item1, zones, 30);
        cmd = ps.Item3.Replace(".","");
        nz = ps.Item2;
        int check = nz == 0 ? 29 : nz -1;
        int direction = 1;
        if( phrase.Length > 1 )
        {
            direction = zones[check] == phrase[phrase.Length-2] ? 1 : -1;
            
            nz += direction;
            if( nz < 0 ) nz = 30 - nz;
            if( nz > 29 ) nz = nz - 30;
        }

        // write counter
        cmd += direction > 0 ? '>' : '<';
        nz += direction;

        //Console.Error.WriteLine("Write: Writing counter value {0} at Memory Pos: {1}", (char)(64 + repeat), nz);
        cmd += SetRune( (char)(64 + repeat), nz );

        // write the loop
        cmd += string.Format("[{0}{1}-]", 
                    "".PadRight(phrase.Length,direction > 0 ? '<' : '>'), 
                    string.Join("",Enumerable.Range(0, phrase.Length).Select(i => direction > 0 ?".>" : ".<"))
                    );

        
        solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
        Array.Copy(zones, solution.Item1,30);  
        
        solutions.Add(solution);  
        //Console.Error.WriteLine("Write: Solution Memory: " + string.Concat(solution.Item1.Select(c => c == ' ' ? '-' : c)));
            
        Array.Copy(memory, zones, 30);
        
        
        solution = solutions.OrderBy(s => s.Item3.Length).First();
        
        Array.Copy(solution.Item1, zones, 30);

        return new Tuple<int, string>(solution.Item2, solution.Item3);
    }
    static Tuple<int, string> MoveToRune(char rune, int z)
    {
        var solutions = new List<Tuple<int, string>>();
        if (rune == ' ')
        {
            solutions.Add(MoveToSpace(z, false));
            solutions.Add(MoveToSpace(z, true));
        }
        else
        {
            solutions.Add(MoveToRune(rune, z, false));

            solutions.Add(MoveToRune(rune, z, true));


            // Forward to Space, Move Forward | Move Backward
            var lF = MoveToSpace(z);
            solutions.Add(MoveToRune(rune, lF.Item1, false, lF.Item2));
            solutions.Add(MoveToRune(rune, lF.Item1, true, lF.Item2));

            // Backward to Space, Move Forward | Move Backward
            var lB = MoveToSpace(z, true);
            solutions.Add(MoveToRune(rune, lB.Item1, false, lB.Item2));
            solutions.Add(MoveToRune(rune, lB.Item1, true, lB.Item2));
        }

        return solutions.OrderBy(s => s.Item2.Length).First();
    }
    static Tuple<int, string> MoveToRune(char rune, int z, bool backward, string prepend = "")
    {
        int nZ = z;
        string instruction = string.Empty;
        if (zones[nZ] == rune) return new Tuple<int, string>(nZ, instruction);

        while (zones[nZ] != rune)
        {
            if (backward)
            {
                instruction += '<';
                if (--nZ < 0) nZ = 29;
            }
            else
            {
                instruction += '>';
                if (++nZ >= 30) nZ = 0;
            }
        }

        return new Tuple<int, string>(nZ, prepend + instruction);
    }
    static string MoveToAddress(int z, int nz)
    {
        var solutions = new List<string>();
        var lF = MoveToSpace(z);
        var lB = MoveToSpace(z, true);
        
        Func<int,int,int> forward = (int x, int y) => x < y ? 30 + y - x : x - y;
        Func<int,int,int> backward = (int x, int y) => x > y ? 30 + y - x : y - x;

        solutions.Add("".PadRight(forward(nz,z), '>'));
        solutions.Add(lF.Item2 + "".PadRight(forward(nz,lF.Item1), '>'));
        solutions.Add(lB.Item2 + "".PadRight(forward(nz,lB.Item1), '>'));

        solutions.Add("".PadRight(backward(nz,z), '<'));
        solutions.Add(lF.Item2 + "".PadRight(backward(nz,lF.Item1), '<'));
        solutions.Add(lB.Item2 + "".PadRight(backward(nz,lB.Item1), '<'));
        
        return solutions.OrderBy(s => s.Length).First();
    }
    static Tuple<int, string> MoveToSpace(int z, bool backward = false)
    {

        var cmd = string.Empty;
        if (!zones.Contains(' '))
            return new Tuple<int, string>(z, "");

        if (backward)
        {
            while (zones[z] != ' ')
            {
                cmd += "<";
                if (--z < 0) z = 29;
            }
            return new Tuple<int, string>(z, cmd.Length < 3 ? cmd : "[<]");
        }

        while (zones[z] != ' ')
        {

            cmd += ">";
            if (++z > 29) z = 0;
        }
        return new Tuple<int, string>(z, cmd.Length < 3 ? cmd : "[>]");

    }

    static string SetRune(char rune, int z)
    {
        var solutions = new List<string>();

        var cR = zones[z];

        solutions.Add(ClearRune(z) + SetRune(rune, z, false));
        zones[z] = cR;

        solutions.Add(ClearRune(z) + SetRune(rune, z, true));
        zones[z] = cR;

        solutions.Add(SetRune(rune, z, false));
        zones[z] = cR;

        solutions.Add(SetRune(rune, z, true));
        zones[z] = cR;

        return solutions.OrderBy(s => s.Length).First();
    }
    static string SetRune(char rune, int z, bool backward)
    {
        string instruction = string.Empty;
        if (zones[z] == rune) return instruction;

        while (zones[z] != rune)
        {
            if (backward)
            {
                if (zones[z] == 'A') zones[z] = ' ';
                else if (zones[z] == ' ') zones[z] = 'Z';
                else zones[z] = (char)((int)zones[z] - 1);

                instruction += '-';
            }
            else
            {
                if (zones[z] == 'Z') zones[z] = ' ';
                else if (zones[z] == ' ') zones[z] = 'A';
                else zones[z] = (char)((int)zones[z] + 1);

                instruction += '+';
            }
        }

        return instruction;
    }
    static string ClearRune(int z)
    {
        if (zones[z] == ' ') return string.Empty;
        if (zones[z] == 'A') return "-";
        if (zones[z] == 'Z') return "+";

        return "[+]";
    }
}
