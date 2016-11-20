using DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacMan
{

    /// <summary>
    /// Interaction logic for BoardLocation.xaml
    /// </summary>
    public partial class BoardLocation : UserControl
    {
        public  const int PATH_CODE_NONE = 0;
        public  const int PATH_CODE_NORMAL_DOT = 1;
        public  const int PATH_CODE_SPECIAL_DOT = 2;
        public  const int PATH_CODE_GHOST = 3;

        // Dependencies
        protected MainWindow mainWindow;
        protected ScoreCounter scoreCounter;

        // pathcode 
        // 0 = none
        // 1 = normal dot
        // 2 = special dot
        // 3 = ghosts only

    
        /*
         * The first ghost captured after an energizer has been eaten is always worth 200 points. 
         * Each additional ghost captured from the same energizer will then be worth twice as many points as the one before it—400, 800, and 1,600 points, 
         * respectively. If all four ghosts are captured at all four energizers, an additional 12,000 points can be earned on these earlier levels.
         * 
         * The 240 small dots are worth ten points each, and the four large, flashing dots—best known as energizers—are worth 50 points each. 
         * This yields a total of 2,600 points for clearing the maze of dots each round.
         * 
         */
        private Dot biscuit;
        private int dotValue;

        public BoardLocation(int pCode)
        {
            InitializeComponent();

            DependencyInjectionHelper.getGameComponent().inject<BoardLocation>(this);

            // Dots can be either normal (1) or special (2)
            if (pCode == 1 || pCode == 2)
            {
                if(pCode == 1)
                {
                    biscuit = new Dot(1);
                    dotValue = 10;
                }
                else
                {
                    biscuit = new Dot(2);
                    dotValue = 50;
                }
                bl.Children.Add(biscuit); // Dynamicall add the dot to our boardlocation object
                biscuit.Visibility = Visibility.Visible;  // make it visible
                Canvas.SetTop(biscuit, 0);
                Canvas.SetLeft(biscuit, 0);
            }
        }


        public Dot Biscuit
        {
            get { return biscuit; }
        }

        public int eatBiscuit()
        {
            int returnValue = 0;

            if(biscuit == null)
            {
                return returnValue;
            }

            // Hide the biscuit
            biscuit.Visibility = Visibility.Hidden;

            // Only allow the value to be recorded once
            if (dotValue > 0)
            {
                Console.WriteLine(dotValue);
                updateScore();

                returnValue = dotValue;
                dotValue = 0;
            }

            return dotValue;
        }

        private void updateScore()
        {
            scoreCounter.OnEatBiscuit(dotValue);
            mainWindow.debugx.Content = scoreCounter.Score.ToString();

            if(scoreCounter.UserWon())
            {
                mainWindow.GB.OnUserWon() ;
            }

        }

        public class BoardLocationInjector : Injector<GameModule, BoardLocation>
        {
            public void inject(GameModule module, BoardLocation mObject)
            {
                mObject.mainWindow = module.ProvideMainWindow();
                mObject.scoreCounter = module.ProvideScoreCounter();
            }
        }

    }
}
