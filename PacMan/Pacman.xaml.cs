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


namespace PacMan
{
    /// <summary>
    /// Interaction logic for Pacman.xaml
    /// </summary>
    /// 
    public partial class Pacman : UserControl
    {


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

        // Constructor
        public Pacman()
        {
            InitializeComponent();
            
            speed = 2;
            offsetX = 0;
            offsetY = 0;
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
                            startFrame = 6;
                            endFrame = 7;
                            break;
                        case 1: // down
                            startFrame = 2;
                            endFrame = 3;
                            break;

                        case 2: // left
                            startFrame = 4;
                            endFrame = 5;
                            break;
                        case 3: // right
                            startFrame = 0;
                            endFrame = 1;
                            break;
                        default:
                            offsetX = 0;
                            offsetY = 0;
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
    }
}
