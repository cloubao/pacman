using DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class GameModule: IModule
    {
        private MainWindow mainWindow;
        private ScoreCounter scoreCounter;

        public GameModule(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.scoreCounter = new ScoreCounter(142);
            this.scoreCounter.Score = -1;
        }

        public ScoreCounter ProvideScoreCounter()
        {
            return scoreCounter;
        }

        public MainWindow ProvideMainWindow()
        {
            return mainWindow;
        }
    }
}
