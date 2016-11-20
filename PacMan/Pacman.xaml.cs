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
using System.Windows.Media.Animation;
using DependencyInjection;
using System.Windows.Threading;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for Pacman.xaml
    /// </summary>
    /// 
    public partial class Pacman : UserControl
    {
        private PositionTracker positionTracker;
        private MainWindow mainWindow;
        private int[,] gamePath;

        private int PMMovementWindow = 7; // This is used as a 'fudge factor' in determining if Pacman can move up/down/left/right to another space


        private int speed;      // How many pixels per tick pacman moves
        private int direction;  //0: up //1: down //2: left //3: right
        private int offsetX;    // This is used to move horizontally across the sprite image
        private int offsetY;    // This is used to move vertically through the sprite image

        // Use for animations
        public static readonly TimeSpan TimePerFrame = TimeSpan.FromSeconds(0.5);  // 
        private int startFrame; // Number of first frame
        private int endFrame;   // Number of last frame
        private int currentFrame; // The current frame that is shown to the user
        private TimeSpan timeTillNextFrame;

    
        // Movement timer for Pacman
        private DispatcherTimer PMmovementTimer;

        protected int upStartFrame;
        protected int upEndFrame;
        
        protected int downStartFrame;
        protected int downEndFrame;

        protected int rightStartFrame;
        protected int rightEndFrame;


        protected int leftStartFrame;
        protected int leftEndFrame;

        protected int defaultStartFrame;
        protected int defaultEndFrame;

        // Constructor
        public Pacman()
        {
            InitializeComponent();

            DependencyInjectionHelper.Init(((MainWindow)Application.Current.MainWindow));
            DependencyInjectionHelper.getGameComponent().inject(this);
            
        
            upStartFrame = 6;
            upEndFrame = 7;

            downStartFrame = 2;
            downEndFrame = 3;

            leftStartFrame = 4;
            leftEndFrame = 5;

            rightStartFrame = 0;
            rightEndFrame = 1;

            defaultStartFrame = 0;
            defaultEndFrame = 0;

            speed = 2;
            offsetX = 0;
            offsetY = 0;
        }

        public void StartTimer()
        {
            // Initialize Pacman's direction as invalid
            Direction = -1;

            // Create the Pacman movement timer for our keyboard movement
            PMmovementTimer = new DispatcherTimer();
            PMmovementTimer.Interval = TimeSpan.FromMilliseconds(2); // make this whatever interval you'd like there to be in between movements
            PMmovementTimer.Tick += new EventHandler(PMmovementTimer_Tick);  // Every 2 milliseconds call the movement function
            PMmovementTimer.Start(); // get the timer going
        }

        public int Speed
        {
            get { return speed;  }
            set { speed = value; }
        }

        public int Direction
        {
            get { return direction; }
            set {

                // Only do this if the direction changed
                // for animation purposes
                if(direction != value)
                {

                    direction = value;


                    switch (direction)
                    {
                        case 0: // up
                            startFrame = upStartFrame; // 6;
                            endFrame = upEndFrame; // 7;
                            break;
                        case 1: // down
                            startFrame = downStartFrame; // 2
                            endFrame =   downEndFrame; // 3
                            break;

                        case 2: // left
                            startFrame = leftStartFrame; // 4
                            endFrame = leftEndFrame; // 5
                            break;
                        case 3: // right
                            startFrame = rightStartFrame; // 0
                            endFrame = rightEndFrame; // 1
                            break;
                        default:
                            offsetX = defaultStartFrame; // 0
                            offsetY = defaultEndFrame; // 0
                            break;
                    }
                }


            }
        }

        private void Pacman_Animation(object sender, EventArgs e)
        {

            // Create a delay based on the number of times this function is seen.. every time it is assume 0.1 seconds have gone by
            this.timeTillNextFrame += TimeSpan.FromSeconds(1 / 10f); // current value is 0.1

            // If the number of ticks add up to 0.5 .. so every 5th tick, run this animation
            if (this.timeTillNextFrame > TimePerFrame)
            {

                // Set the current frame
                // Right now it's just doing a 2 frame animation
                if(currentFrame == startFrame)
                {
                    currentFrame = endFrame;
                }
                else
                {
                    currentFrame = startFrame;
                }

                // Pacman is all in the first row (except his death)
                offsetX = -(int)this.ActualWidth * currentFrame;
                offsetY = 0;

                // This moves the background of the triangle to the appropriate 42x42 x y
                SpriteSheetOffset.X = offsetX;
                SpriteSheetOffset.Y = offsetY;

                // Reset the update flag to 0 
                this.timeTillNextFrame = TimeSpan.FromSeconds(0);
            }

               
        }


        // This is where movement animation is controlled
        // Handle all collision stuff here
        public void PMmovementTimer_Tick(object sender, EventArgs e)
        {

            PacMan.Pacman PM = this;
            GameBoard gameBoard = mainWindow.GB;

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
                pmNextCol = (int)((Canvas.GetLeft(PM) + PM.ActualWidth / 2) / PM.ActualWidth); // Use Pacman midpoint to see what column they are trying to use

                if ((gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2 || gamePath[pmNextRow, pmNextCol] == 3)   // Make sure the next square is movable
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

                if ((gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2 || gamePath[pmNextRow, pmNextCol] == 3)  // Make sure the next square is movable 
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
                if (pmNextCol == -1)
                {
                    Canvas.SetLeft(PM, gameBoard.ActualWidth - PM.ActualWidth - 1);
                }
                else
                if ((gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2 || gamePath[pmNextRow, pmNextCol] == 3) // Make sure the next square is movable
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
                if (pmNextCol == 27)
                {
                    Canvas.SetLeft(PM, 0);
                }
                else
                if ((gamePath[pmNextRow, pmNextCol] == 1 || gamePath[pmNextRow, pmNextCol] == 2 || gamePath[pmNextRow, pmNextCol] == 3)  // Make sure the next square is movable
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

            if ((Canvas.GetTop(PM) + PM.ActualHeight / 2) < (currentRow * PM.ActualHeight + ((PM.ActualHeight / 2) + PMMovementWindow)) &&    //Top Max
                 (Canvas.GetTop(PM) + PM.ActualHeight / 2) > (currentRow * PM.ActualHeight + ((PM.ActualHeight / 2) - PMMovementWindow)) &&    //Top Min
                 (Canvas.GetLeft(PM) + PM.ActualWidth / 2) < (currentCol * PM.ActualWidth + ((PM.ActualWidth / 2) + PMMovementWindow)) &&      //Left Max
                 (Canvas.GetLeft(PM) + PM.ActualWidth / 2) > (currentCol * PM.ActualWidth + ((PM.ActualWidth / 2) - PMMovementWindow)))    //Left Min
            {


                gameBoard.BoardLocations[currentRow, currentCol].eatBiscuit();

            }

            if(positionTracker.isGhostPosition(new int[] {currentRow, currentCol }))
            {
                Visibility = Visibility.Hidden;
                mainWindow.GB.OnUserLose();
            }


            // debug
            //Console.WriteLine("X= " + Canvas.GetLeft(PM) + "    Y= " + Canvas.GetTop(PM));

        }

        public class PacmanInjector : Injector<GameModule, Pacman>
        {
            public void inject(GameModule module, Pacman mObject)
            {
                mObject.positionTracker = module.ProvidePositionTracker();
                mObject.gamePath = module.ProvideGamePath();
                mObject.mainWindow = module.ProvideMainWindow();
            }
        }

    }
}
