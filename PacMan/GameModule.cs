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
        private int[,] gamePath;
        private MainWindow mainWindow;
        private ScoreCounter scoreCounter;
        private int[,] ghostGamePath;
        private PositionTracker positionTracker;

        public GameModule(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.scoreCounter = new ScoreCounter();
            this.positionTracker = new PositionTracker();

            // 0 = Unavail  1= Normal Dots  2= Power Dots  3= Ghosts Only
            //                  1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2
                                   // 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 
            this.gamePath = new int[,]{
                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },   //0
                                    { 0,0,0,0,2,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,1,1,1,2,0 },   //1
                                    { 0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0 },   //2
                                    { 0,0,1,1,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,1,1,1,1,0 },   //3
                                    { 0,0,1,0,0,0,1,0,1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0 },   //4
                                    { 0,0,1,0,1,1,1,0,1,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0 },   //5
                                    { 1,1,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,1,1 },   //6
                                    { 0,0,1,0,3,1,1,0,1,1,1,0,1,0,1,1,1,0,0,0,3,1,2,0,1,0,0 },   //7
                                    { 0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,0 },   //8
                                    { 0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0 },   //9
                                    { 0,1,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0 },   //10
                                    { 0,2,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0 },   //11
                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }   //12
            };                                                                                  

                this.ghostGamePath =  new int[,]{
                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },   //0
                                    { 0,0,0,0,2,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,1,1,1,2,0 },   //1
                                    { 0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0 },   //2
                                    { 0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },   //3
                                    { 0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0 },   //4
                                    { 0,0,1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,0,0,0,1,0,0,0,1,0,0 },   //5
                                    { 1,1,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,1,1 },   //6
                                    { 0,0,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,0,0,1,1,2,0,1,0,0 },   //7
                                    { 0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,0 },   //8
                                    { 0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0 },   //9
                                    { 0,1,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0 },   //10
                                    { 0,2,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0 },   //11
                                    { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }   //12
            };                                                                                     
        }

        public ScoreCounter ProvideScoreCounter()
        {
            return scoreCounter;
        }

        public MainWindow ProvideMainWindow()
        {
            return mainWindow;
        }

        public int[,] ProvideGamePath()
        {
            return gamePath;
        }

        public int[,] ProvideGhostGamePath()
        {
            return ghostGamePath;
        }

        public PositionTracker ProvidePositionTracker()
        {
            return positionTracker;
        }
    }
}
