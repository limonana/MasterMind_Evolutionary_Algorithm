using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterMindPlayer
{
    public class Hint
    {
        public int black;
        public int white;
    }

    class PlayedComibation
    {
        public PlayedComibation(string comb)
        {
            combination = comb;
        }
        public string combination;
        public Hint hint;
    }

    public class Player : IPlayer<string, Hint>
    {
        #region members
        List<PlayedComibation> _playedCombination = new List<PlayedComibation>();
        string[] _population;
        Dictionary<string, int> _combinationToFitness = new Dictionary<string, int>();
        int _combinationLength;
        char[] _colors;
        Random _randomGenerator = new Random();
        int _poplationSize;
        #endregion

        /// <summary>
        /// <remarks>support until combination length of 8 colors</remarks>
        /// </summary>
        public Player(int poplationSize, int combinationLength)
        {
            _poplationSize = poplationSize;
            _combinationLength = combinationLength;
            InitColors();
        }

        private void InitColors()
        {
            _colors = new char[8];
            _colors[0] = 'R';
            _colors[1] = 'G';
            _colors[2] = 'B';
            _colors[3] = 'Y';
            _colors[4] = 'C'; //chyan
            _colors[5] = 'P'; //Pink
            _colors[6] = 'O';
            _colors[7] = 'V'; //violet - purple
        }

        void CreateRandomPopulation()
        {
            _population = new string[_poplationSize];
            for (int i = 0; i < _poplationSize; ++i)
            {                
                _population[i] = GetRndComb();
            }
        }

        public string GetRndComb()
        {
            char[] combination = new char[_combinationLength];
            for (int j = 0; j < _combinationLength; ++j)
                combination[j] = GetRandomColor();
            return new String(combination);
        }

        private char GetRandomColor()
        {
            return _colors[_randomGenerator.Next(_combinationLength)];
        }

        private void CreateNewGeneration()
        {
            if (_population == null)
            {
                CreateRandomPopulation();
            }
            else
                selection();
        }

        string Transpose(string comb)
        {
            char[] newComb = new string('_', _combinationLength).ToCharArray();
            foreach (var color in comb)
            {
                int place;
                do
                {
                    place = _randomGenerator.Next(_combinationLength);
                }
                while (newComb[place] != '_');
                newComb[place] = color;
            }
            return new String(newComb);
        }

        string Mutation(string comb)
        {
            char[] newComb = comb.ToCharArray();
            int place = _randomGenerator.Next(_combinationLength);
            int nextPlace = (place + 1) % _combinationLength;
            newComb[place] = comb[nextPlace];
            newComb[nextPlace] = comb[place];
            return new String(newComb);
        }
        Tuple<string, string> Crossover(string c1, string c2)
        {
            int start = _combinationLength / 4;
            int end = start + _combinationLength / 2;
            char[] son1 = c1.ToCharArray();
            char[] son2 = c2.ToCharArray();
            for (int i = start; i < end; i++)
            {
                son1[i] = c2[i];
                son2[i] = c1[i];
            }
            return new Tuple<string, string>(new String(son1), new String(son2));
        }

        void selection()
        {
            //var sorted = _combinationToFitness.OrderByDescending(x => x.Value);
            var sorted = _population.OrderByDescending(comb => _combinationToFitness[comb]);
            //take the 50% best
            for (int i = 0; i < _poplationSize / 2; i++)
            {
                _population[i] = sorted.ElementAt(i);
            }
            for (int i = _poplationSize / 2; i < _poplationSize; i += 2)
            {
                var parents = GetRandomPair();
                var childs = Crossover(parents.Item1, parents.Item2);
                _population[i] = childs.Item1;
                _population[i + 1] = childs.Item2;
            }
            MakeOperation(Transpose, 0.2);
            MakeOperation(Mutation, 0.4);
        }

        private void MakeOperation(Func<string, string> op, double p)
        {
            int start = _poplationSize / 2;
            int end = _poplationSize;
            int amount = (int)Math.Round(p * _poplationSize / 2);
            while (amount > 0)
            {
                int i = _randomGenerator.Next(start, end);
                _population[i] = op(_population[i]);
                --amount;
            }
        }

        private Tuple<string, string> GetRandomPair()
        {
            int start = _poplationSize / 2;
            int i1 = _randomGenerator.Next(start);
            int i2;
            int tries = 0;
            do
            {
                i2 = _randomGenerator.Next(start);
                ++tries;
            } while (_population[i1] == _population[i2] && tries < 0.8 * _poplationSize / 2);
            return new Tuple<string, string>(_population[i1], _population[i2]);
        }

        private string FindMostConsistentCombination()
        {
            CalcFitness();
            int maxFitness = int.MinValue;
            string maxCombination = null;
            foreach (var item in _combinationToFitness)
            {
                //if this was already played skip it
                if (_playedCombination.Where(x => x.combination == item.Key).Count() > 0)
                    continue;
                if (maxFitness < item.Value)
                {
                    maxCombination = item.Key;
                    maxFitness = item.Value;
                }
            }
            if (maxCombination == null)
                throw new Exception("poplation is only the played combinations");
            return maxCombination;
        }

        int d(Hint hint1, Hint hint2)
        {
            return
                Math.Abs(hint1.black - hint2.black) +
                Math.Abs(hint1.white - hint2.white);
        }

        public Hint hint(string guess, string secret)
        {
            int blacks = 0;
            int white = 0;
            Dictionary<char, int> colorAmount = new Dictionary<char, int>();
            foreach (var color in secret)
            {
                if (!colorAmount.ContainsKey(color))
                    colorAmount[color] = 0;
                colorAmount[color] += 1;
            }

            for (int i = 0; i < _combinationLength; i++)
            {
                char curColor = guess[i];
                if (curColor == secret[i])
                {
                    ++blacks;                    
                    colorAmount[guess[i]] -= 1;
                    if (colorAmount[guess[i]] < 0)
                    {
                        --white;
                        ++colorAmount[guess[i]];
                    }
                    continue;
                }

                int index = secret.IndexOf(curColor);
                if (index >= 0 && colorAmount[curColor] > 0)
                {
                    ++white;
                    colorAmount[guess[i]] -= 1;
                }

            }
            Hint h = new Hint();
            h.white = white;
            h.black = blacks;
            return h;
        }

        int d(string c, PlayedComibation ci)
        {
            return d(hint(c, ci.combination),
                    ci.hint);
        }

        int fitness(string c)
        {
            return _playedCombination.Sum(ci => -d(c, ci));
        }

        private void CalcFitness()
        {
            _combinationToFitness.Clear();
            foreach (var comb in _population)
            {
                if (!_combinationToFitness.ContainsKey(comb))
                    _combinationToFitness[comb] = fitness(comb);
            }
        }

        private string InitialGuess()
        {
            char[] guess = new char[_combinationLength];
            for (int i = 0; i < _combinationLength / 2; i++)
            {
                guess[i * 2] = _colors[i];
                guess[i * 2 + 1] = _colors[i];
            }
            return new string(guess);
        }

        #region IPlayer<string,string> Members

        public string Play(Hint response)
        {
            _playedCombination.Last().hint = response;
            CreateNewGeneration();
            string comb = FindMostConsistentCombination();
            _playedCombination.Add(new PlayedComibation(comb));
            return comb;
        }

        public string Play()
        {
            string comb = InitialGuess();
            _playedCombination.Add(new PlayedComibation(comb));
            return comb;
        }

        #endregion
    }
}
