using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            bool isActive = false;

            foreach (var ship in player.ShipLocations)
            {
                if (ship.Status != GridSpotStatus.Sunk)
                {
                    isActive = true;
                }
            }

            return isActive;
        }

        public static int GetShotCount(PlayerInfoModel player)
        {
            int shotCount = 0;

            foreach (var shot in player.ShotGrid)
            {
                if (shot.Status != GridSpotStatus.Empty)
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
                if (IsMatchingRowAndColumn(shipLocation, row, column))
                {
                    return false;
                }
            }

            if (ValidateShot(model, row, column))
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row,
                    SpotNumber = column,
                    Status = GridSpotStatus.Ship
                });

                return true;
            }

            return false;
        }

        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            if (shot.Length != 2)
            {
                throw new ArgumentException("Location is out of the bounds of the grid.");
            }

            return (shot.Substring(0, 1), Convert.ToInt32(shot.Substring(1)));
        }

        public static bool ValidateShot(PlayerInfoModel player, string row, int column)
        {
            var validRows = new List<string> { "a", "b", "c", "d", "e" };
            bool isValidRow = validRows.Contains(row, StringComparer.OrdinalIgnoreCase);
            bool isValidColumn = column > 0 && column < 6;
            bool isValidShot = false;

            foreach (var gridSpot in player.ShotGrid)
            {
                if (isValidRow && isValidColumn)
                {
                    if (IsMatchingRowAndColumn(gridSpot, row, column))
                    {
                        isValidShot = (gridSpot.Status == GridSpotStatus.Empty) ? true : false;
                    }
                }
            }

            return isValidShot;
        }

        public static bool IdentifyShotResult(PlayerInfoModel opponent, string row, int column)
        {
            foreach (var ship in opponent.ShipLocations)
            {
                if (IsMatchingRowAndColumn(ship, row, column))
                {
                    ship.Status = GridSpotStatus.Sunk;
                    return true;
                }
            }

            return false;
        }

        public static void MarkShotResult(PlayerInfoModel player, string row, int column, bool isAHit)
        {
            foreach (var spot in player.ShotGrid)
            {
                if (IsMatchingRowAndColumn(spot, row, column))
                {
                    spot.Status = isAHit ? GridSpotStatus.Hit : GridSpotStatus.Miss;
                }
            }
        }

        private static bool IsMatchingRowAndColumn(GridSpotModel gridSpot, string row, int column)
        {
            if (gridSpot.SpotLetter.Equals(row, StringComparison.OrdinalIgnoreCase) && gridSpot.SpotNumber == column)
            {
                return true;
            }

            return false;
        }
    }
}
