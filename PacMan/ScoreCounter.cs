using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class ScoreCounter
    {
        private int dotCount;

        private int maxScore;
        private int score;

        public ScoreCounter(int maxScore)
        {
            this.maxScore = maxScore;
            Reset();
        }

        public ScoreCounter()
        {
            this.maxScore = -1;
            Reset();
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
        }

        public bool UserWon()
        {
                return dotCount == 0 || score == maxScore;
        }
        
        public void OnEatBiscuit(int dotValue)
        {
            if(score == -1)
            {
                score = 0;
            }
            else {
                score += dotValue;
                dotCount -= 1;
            }
        }

        public void Reset()
        {
            score = -1;
            dotCount = 141;
        }

    }
}
