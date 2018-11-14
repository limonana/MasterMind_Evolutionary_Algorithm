using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Test()
        {
            MasterMindPlayer.Player p = new MasterMindPlayer.Player(100, 4);
            string secret = p.GetRndComb();
            int moves = 0;            
            try
            {
                string guess = p.Play();
                ++moves;
                var hint = p.hint(guess, secret);
                while (hint.black < 4)
                {
                    guess = p.Play(hint);
                    ++moves;
                    hint = p.hint(guess, secret);
                }
                System.Console.WriteLine("secret:{0},computer guess after {1}", secret, moves);
            }
            catch (Exception)
            {
                System.Console.WriteLine("secret:{0},computer fail", secret);
            }
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < 50; i++)
            {
                Test();
            }
            Console.ReadLine();
        }
    }
}
