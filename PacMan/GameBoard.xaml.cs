using DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        private int[,] gamePath;


        private BoardLocation[,] bl;  // Locations on the board - used to contain dots, etc.

        public GameBoard()
        {
            InitializeComponent();
            
            DependencyInjectionHelper.Init(((MainWindow)Application.Current.MainWindow));
            DependencyInjectionHelper.getGameComponent().inject(this);

            this.Focusable = true;

            bl = new BoardLocation[13, 27];

            ResetBoardLocations();
            PM.StartTimer();

        }

        public BoardLocation[,] BoardLocations
        {
            get
            {
                return bl;
            }
        }

        public void OnUserWon()
        {
            MessageBoxResult result = MessageBox.Show("Play Again ?", "You won", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            {
                // Hack to restart the application
                Process.Start(Assembly.GetEntryAssembly().Location);
            }
            Application.Current.Shutdown();
        }

        public void OnUserLose()
        {
            MessageBoxResult result = MessageBox.Show("Play Again ?", "You loose", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Hack to restart the application
                Process.Start(Assembly.GetEntryAssembly().Location);
            }
            Application.Current.Shutdown();
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
                    if (gamePath[i, j] == 1 || gamePath[i, j] == 2 || gamePath[i,j] == 3)
                    {
                        gameBoard.Children.Add(bl[i, j]);
                        Canvas.SetTop(bl[i, j], 42 * i);
                        Canvas.SetLeft(bl[i, j], 42 * j);

                        if(gamePath[i,j] == 3)
                        {
                            
                            Ghost ghost = new Ghost();
                            gameBoard.Children.Add(ghost);

                            Canvas.SetTop(ghost, 42 * i);
                            Canvas.SetLeft(ghost, 42 * j);

                            ghost.StartTimer();

                        }

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

    

        public  class GameBoardInjector : Injector<GameModule, GameBoard>
        {
            public void inject(GameModule module, GameBoard mObject)
            {
                mObject.gamePath = module.ProvideGamePath();
                mObject.scoreCounter = module.ProvideScoreCounter();
            }
        }

    }
}
