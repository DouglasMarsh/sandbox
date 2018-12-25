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
class War
{
    static void Main(string[] args)
    {
        var p1Deck = new Queue<Card>();
        var p2Deck = new Queue<Card>();
        var p1War = new List<Card>();
        var p2War = new List<Card>();


        int n = int.Parse(Console.ReadLine()); // the number of cards for player 1
        for (int i = 0; i < n; i++)
        {
            string cardp1 = Console.ReadLine(); // the n cards of player 1
            p1Deck.Enqueue(new Card(cardp1));
        }
        int m = int.Parse(Console.ReadLine()); // the number of cards for player 2
        for (int i = 0; i < m; i++)
        {
            string cardp2 = Console.ReadLine(); // the m cards of player 2
            p2Deck.Enqueue(new Card(cardp2));
        }

        Console.Error.WriteLine("P1 Deck: {0}", string.Join(",", p1Deck.Select(c => c)));
        Console.Error.WriteLine("P2 Deck: {0}", string.Join(",", p2Deck.Select(c => c)));

        bool atWar = false;
        int rounds = 0;
        string result = string.Empty;
        while( p1Deck.Any() && p2Deck.Any())
        {
            var p1Card = p1Deck.Dequeue();
            var p2Card = p2Deck.Dequeue();

            // War
            if( p1Card == p2Card )
            {
                Console.Error.WriteLine("At War. P1: {0}, P2: {1}", p1Card, p2Card);

                atWar = true;
                p1War.Add(p1Card);
                p2War.Add(p2Card);

                for (int i = 0; i < 3 && p1Deck.Any() && p2Deck.Any(); i++)
                {
                    p1War.Add(p1Deck.Dequeue());
                    p2War.Add(p2Deck.Dequeue());
                }
                
                Console.Error.WriteLine("P1 WarDeck: {0}", string.Join(",", p1War.Select(c => c)));
                Console.Error.WriteLine("P2 WarDeck: {0}", string.Join(",", p2War.Select(c => c)));
            }
            else if( p1Card > p2Card)
            {
                Console.Error.WriteLine(
                    "P1 Wins {0}: P1: {1}; P2: {2}",
                    atWar ? "War" : "Battle", p1Card, p2Card);
                
                p1War.ForEach(c => p1Deck.Enqueue(c));
                p1War.Clear();
                p1Deck.Enqueue(p1Card);

                p2War.ForEach(c => p1Deck.Enqueue(c));
                p2War.Clear();
                p1Deck.Enqueue(p2Card);

                rounds++;
                atWar = false;
            }
            else
            {
                Console.Error.WriteLine(
                    "P2 Wins {0}: P1: {1}; P2: {2}",
                    atWar ? "War" : "Battle", p1Card, p2Card);

                p1War.ForEach(c => p2Deck.Enqueue(c));
                p1War.Clear();
                p2Deck.Enqueue(p1Card);

                p2War.ForEach(c => p2Deck.Enqueue(c));
                p2War.Clear();
                p2Deck.Enqueue(p2Card);

                rounds++;
                atWar = false;
            }
            Console.Error.WriteLine("Round {0}{1}", rounds, atWar ? " WAR!!" : "");
            Console.Error.WriteLine("P1 Deck: {0}", string.Join(",", p1Deck.Select(c => c)));
            Console.Error.WriteLine("P2 Deck: {0}", string.Join(",", p2Deck.Select(c => c)));
        }

        result = atWar ? "PAT" : p1Deck.Any() ? "1 " + rounds : "2 " + rounds;
        
        Console.WriteLine(result);
    }
    struct Card
    {
        readonly string _card;
        readonly string _value;
        public int Value
        {
            get
            {
                switch (_value)
                {
                    case "2":
                        return 2;
                    case "3":
                        return 3;
                    case "4":
                        return 4;
                    case "5":
                        return 5;
                    case "6":
                        return 6;
                    case "7":
                        return 7;
                    case "8":
                        return 8;
                    case "9":
                        return 9;
                    case "10":
                        return 10;
                    case "J":
                        return 11;
                    case "Q":
                        return 12;
                    case "K":
                        return 13;
                    case "A":
                        return 14;
                }

                throw new ArgumentOutOfRangeException("Invalid Card Value {0}", _value);
            }
        }
        public char Suit { get; set; }

        public Card(string c)
        {
            _card = c;;
            Suit = c.Last();

            _value = string.Join("", c.Take(c.Length - 1));
        }
        
        public static bool operator ==(Card a, Card b)
        {
            return a.Value == b.Value;
        }
        public static bool operator !=(Card a, Card b)
        {
            return !(a == b);
        }
        public static bool operator <(Card a, Card b)
        {
            return a.Value < b.Value;
        }
        public static bool operator >(Card a, Card b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <=(Card a, Card b)
        {
            return a == b || a < b;
        }
        public static bool operator >=(Card a, Card b)
        {
            return a == b || a < b;
        }

        public override bool Equals(object obj)
        {
            if( obj is Card)
            {
                return this == (Card)obj;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return _card.GetHashCode();
        }
        public override string ToString()
        {
            return _card;
        }
    }
}