using DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PacMan.BoardLocation;
using static PacMan.GameBoard;
using static PacMan.Ghost;
using static PacMan.Pacman;

namespace PacMan
{
    public class DependencyInjectionHelper
    {
        private static GameModule gameModule;
        private static Component<GameModule> component;

        public static void Init(GameModule gameModule, Component<GameModule> component)
        {
            DependencyInjectionHelper.gameModule = gameModule;
            DependencyInjectionHelper.component = component;

            addInjectors();

        }

        private static void addInjectors()
        {
            component.addInjector<BoardLocationInjector, BoardLocation>("BoardLocation", new BoardLocationInjector());
            component.addInjector<GameBoardInjector, GameBoard>("GameBoard", new GameBoardInjector());
            component.addInjector<PacmanInjector, Pacman>("Pacman", new PacmanInjector());
            component.addInjector<GhostInjector, Ghost>("Ghost", new GhostInjector());
        }

        /**
        public static GameModule GetGameModule()
        {
            return gameModule;
        }
        **/

        public static Component<GameModule> getGameComponent()
        {
            return component;
        }

        public static void Init(MainWindow window)
        {
            if(component != null)
            {
                Console.WriteLine("Dependency injection already setup");
                return;
            }
            GameModule gameModule = new GameModule(window);
            Component<GameModule> gameComponent = new Component<GameModule>(gameModule);
            Init(gameModule, gameComponent);
        }
    }
}
