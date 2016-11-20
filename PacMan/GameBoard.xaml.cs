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
using System.Windows.Threading;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl
    {

        private ScoreCounter scoreCounter;

        // Movement timer for Pacman
        private DispatcherTimer PMmovementTimer;
        private int PMMovementWindow = 7; // This is used as a 'fudge factor' in determining if Pacman can move up/down/left/right to another space

        private BoardLocation[,] bl;  // Locations on the board - used to contain dots, etc.

        // 0 = Unavail  1= Normal Dots  2= Power Dots  3= Ghosts Only
        //                                              1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2
        //                          0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 
        private int[,] gamePath = { {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },   //0
                                    {0,0,0,0,2,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,1,1,1,2,0 },   //1
                                    {0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,1,0 },   //2
                                    {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0 },   //3
                                    {0,0,1,0,0,0,1,0,1,0,0,0,3,0,0,0,1,0,0,0,1,0,0,0,1,0,0 },   //4
                                    {0,0,1,0,1,1,1,0,1,0,3,3,3,3,3,0,1,0,0,0,1,0,0,0,1,0,0 },   //5
                                    {1,1,1,0,1,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0,1,1,1 },   //6
                                    {0,0,1,0,1,1,1,0,1,1,1,0,1,0,1,1,1,0,0,0,1,1,2,0,1,0,0 },   //7
                                    {0,0,1,0,0,0,1,0,1,0,0,0,1,0,0,0,1,0,0,0,1,0,1,0,1,0,0 },   //8
                                    {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0 },   //9
                                    {0,1,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0 },   //10
                                    {0,2,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0 },   //11
                                    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 } }; //12

        public GameBoard()
        {
            InitializeComponent();
            
            // We setup dependency injection when the game board is created
            DependencyInjectionHelper.Init(((MainWindow)Application.Current.MainWindow));
            DependencyInjectionHelper.getGameComponent().inject(this);
            this.Focusable = true;

            // Initialize Pacman's direction as invalid
            PM.Direction = -1;

            // Create the Pacman movement timer for our keyboard movement
            PMmovementTimer = new DispatcherTimer();
            PMmovementTimer.Interval = TimeSpan.FromMilliseconds(2); // make this whatever interval you'd like there to be in between movements
            PMmovementTimer.Tick += new EventHandler(PMmovementTimer_Tick);  // Every 2 milliseconds call the movement function
            PMmovementTimer.Start(); // get the timer going
            

            bl = new BoardLocation[13, 27];

            ResetBoardLocations();
        }

        internal void OnUserWon()
        {
            MessageBox.Show("You won");
            ResetBoardLocations();
        }


        private void ResetBoardLocations()
        {

            // We reset the score counter
            scoreCounter.Reset();

            // Initialize the Board Locations
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 27; j++)
                {
                    bl[i, j] = new BoardLocation(gamePath[i, j]);

                    // place the location if we have a dot
                    if (gamePath[i, j] == 1 || gamePath[i, j] == 2)
                    {
                        gameBoard.Children.Add(bl[i, j]);
                        Canvas.SetTop(bl[i, j], 42 * i);
                        Canvas.SetLeft(bl[i, j], 42 * j);

                    }
                }
            }
        }

        // Keyboard movement of PacMan
        public void GBKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    PM.Direction = 0;
                    break;
                case Key.Down:
                    PM.Direction = 1;
                    break;
                case Key.Left:
                    PM.Direction = 2;
                    break;
                case Key.Right:
                    PM.Direction = 3;
                    break;
            }

        }

        // This is where movement animation is controlled
        // Handle all collision stuff here
        void PMmovementTimer_Tick(object sender, EventArgs e)
        {
            // Variables to hold current Row/Col square of Pacman
            // Keep in mind everything is based on top left of pacman square.. this can be problematic in trying to determine what square he is mostly in.. would rather have the center
            // So with that in mind we'll need to make sure PacMan stays on the 'rails' based on Width/2 (the center of pacman).. but we also don't want to make the game extremely hard, 
            // so put in ranges and then reset the rails
            // Rails: - Note: looking at the center of Pacman
            // Rows:    21, 63, 105, 147, 189, 231, 273, 315, 357, 399, 441, 483, 525
            // Columns: 21, 63, 105, 147, 189, 231, 273, 315, 357, 399, 441, 483, 525, 567, 609, 651, 693, 735, 777, 819, 861, 903, 945, 987, 1029, 1071, 1113
            int pmNextRow = 0;
            int pmNextCol = 0;

            //////////////////////////////////////////
            // UP
            if (PM.Direction == 0) 
            {

                // Trying to move up
                pmNextRow = (int)((Canvas.GetTop(PM) - PM.Speed) / PM.ActualHeight);
                pmNextCol = (int)((Canvas.GetLeft(PM) + PM.ActualWidth/2) / PM.ActualWidth); // Use Pacman midpoint to see what column they are trying to use
                   
                if ((gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2)   // Make sure the next square is movable
                    && Canvas.GetLeft(PM) > ((pmNextCol * PM.ActualWidth) - PMMovementWindow)      // Make sure we're pretty close to the path, will adjust during move
                    && Canvas.GetLeft(PM) < ((pmNextCol * PM.ActualWidth) + PMMovementWindow))
                {         
                    Canvas.SetTop(PM, Canvas.GetTop(PM) - PM.Speed);
                    Canvas.SetLeft(PM, pmNextCol * PM.ActualHeight);
                }   

            }
            //////////////////////////////////////////
            // Down
            else if (PM.Direction == 1) 
            {
    
                pmNextRow = (int)((Canvas.GetTop(PM) + PM.Speed + PM.ActualHeight) / PM.ActualHeight);
                pmNextCol = (int)((Canvas.GetLeft(PM) + PM.ActualWidth / 2) / PM.ActualWidth);

                if ( (gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2)  // Make sure the next square is movable 
                    && Canvas.GetLeft(PM) > ((pmNextCol * PM.ActualWidth) - PMMovementWindow)      // Make sure we're pretty close to the path, will adjust during move
                    && Canvas.GetLeft(PM) < ((pmNextCol * PM.ActualWidth) + PMMovementWindow))
                {
                    Canvas.SetTop(PM, Canvas.GetTop(PM) + PM.Speed);
                }
             
            }
            //////////////////////////////////////////
            // Left
            else if (PM.Direction == 2) 
            {

                pmNextRow = (int)((Canvas.GetTop(PM) + PM.ActualWidth / 2) / PM.ActualHeight);  // Use midpoint to see what row we are trying to be in
                pmNextCol = (int)((Canvas.GetLeft(PM) - PM.Speed) / PM.ActualHeight);

                // Check to see if pmNextCol is -1.. if it is teleport pacman to the right side of the board!
                if(pmNextCol == -1)
                {
                    Canvas.SetLeft(PM, gameBoard.ActualWidth - PM.ActualWidth - 1);
                }
                else
                if ( (gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2) // Make sure the next square is movable
                    && Canvas.GetTop(PM) > ((pmNextRow * PM.ActualHeight) - PMMovementWindow)     // Make sure we're pretty close to the path, will adjust during move
                    && Canvas.GetTop(PM) < ((pmNextRow * PM.ActualHeight) + PMMovementWindow))
                {
                    Canvas.SetLeft(PM, Canvas.GetLeft(PM) - PM.Speed);
                }

            }
            //////////////////////////////////////////
            // Right
            else if (PM.Direction == 3) 
            {

                pmNextRow = (int)((Canvas.GetTop(PM) + PM.ActualWidth / 2) / PM.ActualHeight);  // Use midpoint to see what row we are trying to be in
                pmNextCol = (int)((Canvas.GetLeft(PM) + PM.Speed + PM.ActualWidth) / PM.ActualHeight);

                // Check to see if NextCol is 27.. if it is teleport pacman to the left side of the board!
                if(pmNextCol == 27)
                {
                    Canvas.SetLeft(PM, 0);
                }
                else
                if ( (gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2)  // Make sure the next square is movable
                    && Canvas.GetTop(PM) > ((pmNextRow * PM.ActualHeight) - PMMovementWindow)      // Make sure we're pretty close to the path, will adjust during move
                    && Canvas.GetTop(PM) < ((pmNextRow * PM.ActualHeight) + PMMovementWindow))
                {
                    Canvas.SetLeft(PM, Canvas.GetLeft(PM) + PM.Speed);
                }

            }



            //////////////////////////////////////////////////////////////////////////////////////////////
            // Figure out if we are close to a biscuit (dot) location and if it is, eat it.
            // Eating a biscuit the first time will return the value of that biscuit and hide the graphic.  Subsequent overlaps will do nothing
            // Just look at current X/Y of Pacman center and X/Y of current block center

            int currentRow = (int)((Canvas.GetTop(PM) + PM.ActualHeight / 2) / PM.ActualHeight);
            int currentCol = (int)((Canvas.GetLeft(PM) + PM.ActualWidth / 2) / PM.ActualWidth);

            if ( (Canvas.GetTop(PM) + PM.ActualHeight/2) < (currentRow * PM.ActualHeight + ((PM.ActualHeight/2) + PMMovementWindow)) &&    //Top Max
                 (Canvas.GetTop(PM) + PM.ActualHeight/2) > (currentRow * PM.ActualHeight + ((PM.ActualHeight/2) - PMMovementWindow)) &&    //Top Min
                 (Canvas.GetLeft(PM) + PM.ActualWidth/2) < (currentCol * PM.ActualWidth + ((PM.ActualWidth/2) + PMMovementWindow)) &&      //Left Max
                 (Canvas.GetLeft(PM) + PM.ActualWidth / 2) > (currentCol * PM.ActualWidth + ((PM.ActualWidth / 2) - PMMovementWindow)))    //Left Min
            {

                
                bl[currentRow, currentCol].eatBiscuit();

            }


            // debug
            //Console.WriteLine("X= " + Canvas.GetLeft(PM) + "    Y= " + Canvas.GetTop(PM));

        }

        public  class GameBoardInjector : Injector<GameModule, GameBoard>
        {
            public void inject(GameModule module, GameBoard mObject)
            {
                mObject.scoreCounter = module.ProvideScoreCounter();
            }
        }

    }
}
