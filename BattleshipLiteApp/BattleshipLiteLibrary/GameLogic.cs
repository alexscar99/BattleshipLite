using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLiteLibrary
{
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            List<int> numbers = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };

            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };

            model.ShotGrid.Add(spot);
        }

        public static bool PlayerStillActive(PlayerInfoModel opponent)
        {
            int sunkCount = 0;

            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.Status == GridSpotStatus.Sunk)
                {
                    sunkCount++;
                }
            }

            return (sunkCount < 5) ? true : false;
        }

        public static int GetShotCount(PlayerInfoModel winner)
        {
            int shotCount = 0;

            foreach (var spot in winner.ShotGrid)
            {
                if (spot.Status == GridSpotStatus.Hit || spot.Status == GridSpotStatus.Miss)
                {
                    shotCount++;
                }
            }

            return shotCount;
        }

        public static bool PlaceShip(PlayerInfoModel model, string location)
        {
            (string row, int column) = SplitShotIntoRowAndColumn(location);

            foreach (var shipLocation in model.ShipLocations)
            {
                if (shipLocation.SpotLetter == row && shipLocation.SpotNumber == column)
                {
                    return false;
                }
            }

            GridSpotModel ship = new GridSpotModel
            {
                SpotLetter = row,
                SpotNumber = column,
                Status = GridSpotStatus.Ship
            };

            if (ValidateShot(model, row, column))
            {
                model.ShipLocations.Add(ship);
                return true;
            }

            return false;
        }

        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            return (shot.Substring(0, 1), Convert.ToInt32(shot.Substring(1)));
        }

        public static bool ValidateShot(PlayerInfoModel activePlayer, string row, int column)
        {
            bool isValidRow = (row.ToLower() == "a") || (row.ToLower() == "b") || (row.ToLower() == "c") || 
                (row.ToLower() == "d") || (row.ToLower() == "e");
            bool isValidColumn = (column > 0) && (column < 6);

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (isValidRow && isValidColumn && (gridSpot.Status != GridSpotStatus.Hit || gridSpot.Status != GridSpotStatus.Miss))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IdentifyShotResult(PlayerInfoModel opponent, string row, int column)
        {
            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.SpotLetter.ToLower() == row.ToLower() && ship.SpotNumber == column)
                {
                    ship.Status = GridSpotStatus.Sunk;
                    return true;
                }
            }

            return false;
        }

        public static void MarkShotResult(PlayerInfoModel activePlayer, string row, int column, bool isAHit)
        {
            foreach (var spot in activePlayer.ShotGrid)
            {
                if (spot.SpotLetter.ToLower() == row.ToLower() && spot.SpotNumber == column)
                {
                    spot.Status = isAHit ? GridSpotStatus.Hit : GridSpotStatus.Miss;
                }
            }
        }
    }
}
