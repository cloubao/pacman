using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class ScoreCounter
    {
        private int maxScore;
        private int score;

        public ScoreCounter(int maxScore)
        {
            this.maxScore = maxScore;
        }

        public ScoreCounter()
        {
            this.maxScore = -1;
        }

		public int MaxScore
        {
			get
            {
                return maxScore;
            }
			set
            {
                maxScore = value;
            }
        }

		public int Score {
			get
            {
                return score;
            }
			set
            {
                score = value;
                Console.WriteLine("New Score {0}", score);
            }
        }

        public bool hasMaxScore()
        {
            return maxScore != -1;
        }
    }
}
