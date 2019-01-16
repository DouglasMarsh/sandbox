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
    public static string ExecuteCmd(string cmd) 
    {
        string spell = string.Empty;
            
        Console.WriteLine();
        using(var output = new StringWriter())
        {
            char[] memory = string.Empty.PadRight(30, ' ').ToArray();
            int addr = 0;
            var loops = new Stack<int>();
            bool skipLoop = false;
            for( int i = 0; i < cmd.Length; i++)
            {
                if( skipLoop )
                {
                    skipLoop = cmd[i] == ']';
                    continue;
                }
                switch (cmd[i])
                {
                    case '.':
                        spell += memory[addr];
                        Console.Write(memory[addr]);
                        break;
                    case '+':
                        IncrementMemory(memory, addr);
                        break;
                    case '-':
                        DecrementMemory(memory,addr);
                        break;
                    case '>':
                        addr = AdjustPosition(++ addr);
                        break;
                    case '<':
                        addr = AdjustPosition(-- addr);
                        break;
                    case '[':
                        loops.Push(i);
                        skipLoop = memory[addr] == ' ';
                        break;
                    case ']':
                        int loopI = loops.Pop();
                        if(memory[addr] != ' ' )
                            i = loopI - 1;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown Operator: " + cmd[i]);
                        
                }
            }
        }
        Console.WriteLine();
        return spell;
    }
    public static void Process(TextReader input, TextWriter output, TextWriter error)
    {
        string magicPhrase = input.ReadLine();
        string instruction = string.Empty;

        int z = 0;
        Tuple<int, string> solution = null;
        int i = 0;
        while(i < magicPhrase.Length)
        {
            var duplicate = IdentifyDuplicates( string.Concat(magicPhrase.Skip(i)) );
            if( duplicate != null )
            {
                //error.WriteLine("Processing duplicate {0} * {1}", duplicate.Item1, duplicate.Item2);
                
                solution = ProcessDuplicatePhrase(duplicate.Item1, duplicate.Item2, z);
                instruction += solution.Item2;
                z = solution.Item1;
                i += duplicate.Item1.Length * duplicate.Item2;
                
                //error.WriteLine("After Duplicate: Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
                continue;
            }

            string sequence = IdentifySequence(magicPhrase.Skip(i).ToArray());
            if( !string.IsNullOrEmpty(sequence))
            {
                //error.WriteLine("Processing sequence: " + sequence);
                solution = ProcessSequence(sequence, z);
                instruction += solution.Item2;
                z = solution.Item1;
                i += sequence.Length;

                //error.WriteLine("After Sequence: Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
                continue;
            }

            solution = ProcessRune(magicPhrase[i], z);
            instruction += solution.Item2 + ".";
            z = solution.Item1;
            zones[z] = magicPhrase[i];
            i++;
                
            //error.WriteLine("After ProcessRune: Spell idx {0}: Memory: {1}", i, string.Concat(zones.Select(c => c == ' ' ? '-' : c)));
                
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
        var memory = new char[30];
        Array.Copy(zones, memory,30);

        Tuple<char[], int, string> solution = null;
        var solutions = new List<Tuple<char[], int, string>>();
        solutions.Clear();
        
        if (zones.Contains(rune))
        {
            var s = MoveToRune(rune, z);
            solution = new Tuple<char[], int, string>(new char[30], s.Item1, s.Item2);
            Array.Copy(zones, solution.Item1,30);
            solutions.Add(solution);
            
            Array.Copy(memory, zones,30);
        }
        
        solution = new Tuple<char[], int, string>(new char[30], z, SetRune(rune, z));
        Array.Copy(zones, solution.Item1,30);
        solutions.Add(solution);

        Array.Copy(memory, zones,30);

        var m2s = MoveToSpace(z, false);
        solution = new Tuple<char[], int, string>(new char[30], m2s.Item1, m2s.Item2 + SetRune(rune, m2s.Item1));
        Array.Copy(zones, solution.Item1,30);
        solutions.Add(solution);

        Array.Copy(memory, zones,30);

        m2s = MoveToSpace(z, true);
        solution = new Tuple<char[], int, string>(new char[30], m2s.Item1, m2s.Item2 + SetRune(rune, m2s.Item1));
        Array.Copy(zones, solution.Item1,30);
        solutions.Add(solution);

        Array.Copy(memory, zones,30);

        solution = solutions.OrderBy(s => s.Item3.Length).First();
        Array.Copy(solution.Item1, zones, 30);

        return new Tuple<int, string>(solution.Item2, solution.Item3);
    }
    static Tuple<char[], int,string> WritePhrase(char[] phrase, int z)
    {
        var memory = new char[30];
        Array.Copy(zones, memory,30);
        var sZones = string.Concat(zones);
        var sPhrase = string.Concat(phrase);
        string cmd = string.Empty;
        int nz = z;
        Tuple<char[],int, string> solution = null;
        var solutions = new List<Tuple<char[],int, string>>();
        
        // find full phrase
        if( !phrase.Except(zones).Any() )
        {
            // find location of phrase that is nearest to current location
            int phraseIdx = sZones.IndexOf(sPhrase);
            int distTo = 30;
            do
            {
                if( Math.Abs(distTo) > Math.Abs(phraseIdx - z)) 
                    distTo = phraseIdx - z;

                if( phraseIdx + phrase.Length >= 30) break;

                phraseIdx = sZones.IndexOf(sPhrase, phraseIdx + phrase.Length);
            } while( phraseIdx > 0);

            // move to location
            nz = AdjustPosition(z + distTo + phrase.Length);

            cmd = MoveToAddress(z, nz);
            solution = new Tuple<char[], int, string>(new char[30], nz, cmd);
            Array.Copy(zones, solution.Item1,30);
            solutions.Add(solution);
            Array.Copy(memory, zones,30);
        }

        // find partial phrase;
        var subPhrase = zones.Intersect(phrase).ToArray();
        if( subPhrase.Length > 1)
        {
            // find location of phrase that is nearest to current location
            var subP = string.Concat(subPhrase);
            int spi = sPhrase.IndexOf(subP);
            int phraseIdx = sZones.IndexOf(subP);
            int distTo = 30;
            do
            {
                if( Math.Abs(distTo) > Math.Abs(phraseIdx - z)) 
                    distTo = phraseIdx - z;

                if( phraseIdx + subP.Length >= 30) break;

                phraseIdx = sZones.IndexOf(subP, phraseIdx + subP.Length);
            } while( phraseIdx > 0);

            // move to location
            nz = AdjustPosition(phraseIdx - spi);

            cmd = MoveToAddress(z, nz);
            for(int i = 0; i < phrase.Length; i++)
            {
                if( i == spi)
                {
                    cmd += "".PadRight(subPhrase.Length,'>');
                    nz += subPhrase.Length;
                    i += subPhrase.Length-1;
                }
                else
                {
                    cmd += SetRune(phrase[i], nz);
                    cmd += '>';
                    nz ++;
                }
                
                nz = AdjustPosition(nz);
            }
            solution = new Tuple<char[], int, string>(new char[30], nz, cmd);
            Array.Copy(zones, solution.Item1,30);
            solutions.Add(solution);
            Array.Copy(memory, zones,30);
        }
        
        // write entire phrase
        Tuple<int,string> partial = null;
        if( zones.Contains(phrase[0])) partial = MoveToRune(phrase[0], z);
        else partial = ProcessRune(phrase[0], z);

        nz = partial.Item1;
        cmd = partial.Item2;
        #region Phrase
        for(int i = 1; i < phrase.Length; i++)
        {
            cmd += '>' + SetRune(phrase[i], ++nz);
        }
        solution = new Tuple<char[], int, string>(new char[30], nz, cmd);
        Array.Copy(zones, solution.Item1,30);
        solutions.Add(solution);

        Array.Copy(memory, zones,30);
        #endregion

        return solutions.OrderBy(s => s.Item3.Length).First();
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
                zones[mtr.Item1] = sequence.Last();
                nz = AdjustPosition(mtr.Item1+1);

                // write counter
                cmd += ">";
                cmd += SetRune((char)(64 + sequence.Length), nz );

                // write the loop
                cmd += direction < 0 ? "[<.->-]" : "[<.+>-]";
                zones[nz] = ' ';
                solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
                Array.Copy(zones, solution.Item1,30);  
                
                solutions.Add(solution);  
                Array.Copy(memory, zones, 30);
            }

            #region Sequence with Loop
            cmd = SetRune(rune, z);
            zones[z] = sequence.Last();
            nz = AdjustPosition(z+1);

            // write counter
            cmd += ">";
            cmd += SetRune((char)(64 + sequence.Length), nz );

            // write the loop
            cmd += direction < 0 ? "[<.->-]" : "[<.+>-]";
            zones[nz] = ' ';
            solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
            Array.Copy(zones, solution.Item1,30);  
            
            solutions.Add(solution);  
            Array.Copy(memory, zones, 30);
            #endregion

            // Sequence without loop
            solution = WritePhrase(sequence.ToArray(), z);
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
            if( count > dup.Length)
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

        solution = WritePhrase(phrase.ToArray(), z);
        Array.Copy(solution.Item1, zones, 30);
        nz = AdjustPosition(solution.Item2 + 1);
        cmd = solution.Item3;
        
        // write counter
        cmd += '>' + SetRune((char)(64 + repeat), nz );

        // write the loop
        cmd += string.Format("[{0}{1}-]", 
                    "".PadRight(phrase.Length,'<'), 
                    string.Join("",Enumerable.Range(0, phrase.Length).Select(i => ".>")));

        zones[nz] = ' ';    
        solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
        Array.Copy(zones, solution.Item1,30);  
            
        solutions.Add(solution);  
        Array.Copy(memory, zones, 30);

        
        solution = WritePhrase(phrase.Reverse().ToArray(), z);
        Array.Copy(solution.Item1, zones, 30);
        nz = AdjustPosition(solution.Item2 - 1);
        cmd = solution.Item3;
        // write counter
        cmd += '<' + SetRune((char)(64 + repeat), nz );

        // write the loop
        cmd += string.Format("[{0}{1}-]", 
                    "".PadRight(phrase.Length,'>'), 
                    string.Join("",Enumerable.Range(0, phrase.Length).Select(i => ".<")));
   
        zones[nz] = ' ';    
        solution = new Tuple<char[],int,string>(new char[30], nz, cmd);
        Array.Copy(zones, solution.Item1,30);  
            
        solutions.Add(solution);  
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
                DecrementMemory (zones, z);
                instruction += '-';
            }
            else
            {
                IncrementMemory(zones, z);
                instruction += '+';
            }
        }

        return instruction;
    }
    static string ClearRune(int z)
    {
        if (zones[z] == ' ') return string.Empty;
        if (zones[z] == 'A') return "-";
        if (zones[z] == 'B') return "--";

        if (zones[z] == 'Z') return "+";
        if (zones[z] == 'Y') return "++";

        return "[+]";
    }
    static int AdjustPosition(int z)
    {
        if( z < 0 ) return 30 + z;
        else if( z > 29 ) return z - 30;

        return z;
    }
    static void IncrementMemory(char[] memory, int addr)
    {
        if (memory[addr] == 'Z') memory[addr] = ' ';
        else if (memory[addr] == ' ') memory[addr] = 'A';
        else memory[addr] = (char)((int)memory[addr] + 1);
        
    }
    static void DecrementMemory(char[] memory, int addr)
    {
        if (memory[addr] == 'A') memory[addr] = ' ';
        else if (memory[addr] == ' ') memory[addr] = 'Z';
        else memory[addr] = (char)((int)memory[addr] - 1);
    }
}
