using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterMind
{
    class Program
    {
        static void Main(string[] args)
        {
            int compGuess = 0;
            System.Console.WriteLine("welcome to MasterMind,playing with 4 colors:R,G,B,Y.");
            System.Console.WriteLine("please choose your secret combination and then press enter.");
            System.Console.ReadLine();
            System.Console.WriteLine("after each guess of the compter, enter the hint as a string of B and W.");
            MasterMindPlayer.Player player = new MasterMindPlayer.Player(100, 4);            
            string res = player.Play();
            ++compGuess;
            System.Console.WriteLine("compter guess {0}: {1}", compGuess, res);
            string userAnser = System.Console.ReadLine();
            while (userAnser != "V")
            {                 
                MasterMindPlayer.Hint hint = new MasterMindPlayer.Hint();
                hint.black = 0;
                hint.white = 0;
                foreach (var peg in userAnser)
                {
                    if (peg == 'B')
                        ++hint.black;
                    if (peg == 'W')
                        ++hint.white;                   
                }
                res = player.Play(hint);
                ++compGuess;
                System.Console.WriteLine("computer guess {0}: {1}",compGuess, res);
                userAnser = System.Console.ReadLine();
            }                                               
        }
    }
}