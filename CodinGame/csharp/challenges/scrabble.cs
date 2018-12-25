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
class scrabble
{
    
    static void Main(string[] args)
    {
        
        int N = int.Parse(Console.ReadLine());
        var words = new string[N];
        for (int i = 0; i < N; i++)
        {
            string W = Console.ReadLine();
            words[i] = W;
            Console.Error.WriteLine(W);
        }
        string LETTERS = Console.ReadLine();

        Console.Error.WriteLine(LETTERS);
        var root = new DawgNode('\0');
        
        int idx = 0;
        foreach( var word in words)
        {
            bool validWord = true;
            if( word.Length <= 7)
            {
                var node = root;
                foreach( var l in word)
                {
                    if( !LETTERS.Contains(l))
                    {
                        validWord = false;
                        break;
                    }
                    if( !node.Children.ContainsKey(l))
                    {
                        var n = new DawgNode(l);
                        node.Children.Add(l, n);
                    }

                    node = node.Children[l];
                }
                if( validWord )
                {
                    node.Word = word;
                    node.Value = Value(word);  
                    node.Index = idx ++;  

                    Console.Error.WriteLine("Found Word: " + node.Word);
                }       
            }
        }

        var found = new List<DawgNode>();
        FindWords(LETTERS, root, found);

        string maxWord = 
            found.OrderByDescending(w => w.Value)
                 .ThenBy(w => w.Index)
                 .First().Word;

        
        Console.WriteLine(maxWord);
    }
    static int Value(string word)
    {
        int value = 0;
        foreach(var c in word)
        {
            switch (c)
            {
                case 'd':
                case 'g':
                    value += 2;
                    break;
                
                case 'b':
                case 'c':
                case 'm':
                case 'p':
                    value += 3;
                    break;
                
                case 'f':
                case 'h':
                case 'v':
                case 'w':
                case 'y':
                    value += 4;
                    break;
                case 'k':
                    value += 5;
                    break;
                case 'j':
                case 'x':
                    value += 8;
                    break;
                case 'q':
                case 'z':
                    value += 10;
                    break;

                default:
                    value += 1;
                    break;
            }
        }

        return value;
    }
    
    static void FindWords(string letters, DawgNode node, List<DawgNode> words)
    {        
        Console.Error.WriteLine("Searching Letters: " + letters);
        
        if( letters.Length == 0) return;

        var matches = node.Children.Where(c => letters.Contains(c.Key))
                                   .Select(c => c.Value);

        Console.Error.WriteLine("Matched nodes: " + string.Join(" ", matches.Select(n => n.Letter)));
        foreach( var m in matches )
        {
            if( m.Word != null )
            {
                words.Add( m );                   

                Console.Error.WriteLine("Found Word: {0} with Value: {1} and Index: {2}",
                    m.Word, m.Value, m.Index); 
            }
            
            int pos = letters.IndexOf(m.Letter);            
            string newLetters = letters.Substring(0, pos) + letters.Substring(pos + 1);

            FindWords(newLetters, m, words);
        }
        
    }
    class DawgNode
    {
        public DawgNode(char l)
        { 
            Letter = l;            
            Children = new Dictionary<char, DawgNode>();
        }
        public char Letter { get; set; }   
        public int Value { get; set; }    
        public int Index { get; set; } 
        public string Word {get;set;}
        public  Dictionary<char,DawgNode> Children {get; private set;}  

        

    }    
    
}