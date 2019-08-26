using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue)
                {
                    // Swap positions using tuple
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }
                
            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to { winner.UsersName } for winning!");
            Console.WriteLine($"{ winner.UsersName } took { GameLogic.GetShotCount(winner) } shots to sink all 5 battleships!");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            do
            {
                Console.WriteLine();
                string shot = AskForShot(activePlayer.UsersName);
                (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                isValidShot = GameLogic.ValidateShot(activePlayer, row, column);

                if (!isValidShot)
                {
                    Console.WriteLine("Invalid shot location. Please try again.");
                }

            } while (!isValidShot);

            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);
        }

        private static string AskForShot(string activePlayerName)
        {
            Console.WriteLine();
            Console.Write($"{ activePlayerName }, enter your shot selection: ");
            string output = Console.ReadLine();
            Console.WriteLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" { gridSpot.SpotLetter }{ gridSpot.SpotNumber } ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O ");
                }
                else
                {
                    Console.Write(" ? ");
                }
            }
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("created by Alex Scarlett");
            Console.WriteLine();
            Console.WriteLine("This is a miniature version of the popular game Battleship, with a 25 spot grid");
            Console.WriteLine("Rows: A-E, Columns: 1-5");
            Console.WriteLine("'X' marks a hit/sunk ship, 'O' marks a miss");
            Console.WriteLine("TIP: When entering a location (placing ships or firing), enter the row followed by the column (A1, B3, etc.)");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();
            Console.WriteLine($"Player information for { playerTitle }");

            output.UsersName = AskForUsersName();

            GameLogic.InitializeGrid(output);

            PlaceShips(output);

            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place ship number { model.ShipLocations.Count + 1 }: ");
                string location = Console.ReadLine();

                bool isValidLocation = GameLogic.PlaceShip(model, location);

                if (!isValidLocation)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
