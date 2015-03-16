// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapHelper.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPathfinding type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;

    using OPMGFS.Map.CellularAutomata;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// A class that contains methods used by multiple map classes.
    /// </summary>
    public static class MapHelper
    {
        /// <summary>
        /// The random generator.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Gets the random generator.
        /// </summary>
        public static Random Random 
        { 
            get
            {
                return random;
            }
        }

        /// <summary>
        /// Gets the neighbour positions for the given position.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="map"> The map to get positions for. </param>
        /// <param name="neighbourhood"> The type of neighbourhood to look at. </param>
        /// <returns> A list of the coordinates for the neighbour positions. </returns>
        public static List<Position> GetNeighbourPositions(int x, int y, Enums.HeightLevel[,] map, RuleEnums.Neighbourhood neighbourhood = RuleEnums.Neighbourhood.VonNeumann)
        {
            var list = new List<Position>();
            var moves = new[] { -1, 0, 1 };
            var sizeX = map.GetLength(0);
            var sizeY = map.GetLength(1);
            var neumann = neighbourhood == RuleEnums.Neighbourhood.VonNeumann
                           || neighbourhood == RuleEnums.Neighbourhood.VonNeumannExtended;

            if (neighbourhood == RuleEnums.Neighbourhood.MooreExtended
                || neighbourhood == RuleEnums.Neighbourhood.VonNeumannExtended)
                moves = new[] { -2, -1, 0, 1, 2 };

            foreach (var moveX in moves)
            {
                foreach (var moveY in moves)
                {
                    // Uses Von Neumann right now. Remove second check to use Moore.
                    if (moveX == 0 && moveY == 0) continue;
                    if (neumann && (moveX != 0 && moveY != 0)) continue;

                    var posToCheck = new Position(x + moveX, y + moveY);
                    if (!WithinMapBounds(posToCheck, sizeX, sizeY)) continue;
                    list.Add(posToCheck);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the number of different neighbours surrounding the given position.
        /// </summary>
        /// <param name="x"> The x-coordinate to check neighbours for. </param>
        /// <param name="y"> The y-coordinate to check neighbours for. </param>
        /// <param name="map"> The map. </param>
        /// <param name="neighbourhood"> The type of neighbourhood to look at. </param>
        /// <returns> A dictionary containing the neighbours and the count of them. </returns>
        public static Dictionary<Enums.HeightLevel, int> GetNeighbours(int x, int y, Enums.HeightLevel[,] map, RuleEnums.Neighbourhood neighbourhood = RuleEnums.Neighbourhood.Moore)
        {
            var neighbours = new Dictionary<Enums.HeightLevel, int>();
            var moves = new[] { -1, 0, 1 };
            var sizeX = map.GetLength(0);
            var sizeY = map.GetLength(1);
            var neumann = neighbourhood == RuleEnums.Neighbourhood.VonNeumann
                           || neighbourhood == RuleEnums.Neighbourhood.VonNeumannExtended;

            if (neighbourhood == RuleEnums.Neighbourhood.MooreExtended
                || neighbourhood == RuleEnums.Neighbourhood.VonNeumannExtended)
                moves = new[] { -2, -1, 0, 1, 2 };

            // Ensure that all keys are in the dictionary
            neighbours[Enums.HeightLevel.Height0] = 0;
            neighbours[Enums.HeightLevel.Height1] = 0;
            neighbours[Enums.HeightLevel.Height2] = 0;
            neighbours[Enums.HeightLevel.Impassable] = 0;

            foreach (var moveX in moves)
            {
                foreach (var moveY in moves)
                {
                    if (moveX == 0 && moveY == 0) continue;
                    if (neumann && (moveX != 0 && moveY != 0)) continue;

                    var posToCheck = new Position(x + moveX, y + moveY);
                    if (!WithinMapBounds(posToCheck, sizeX, sizeY)) continue;
                    neighbours[map[posToCheck.Item1, posToCheck.Item2]]++;
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Checks if a position is within the bounds of the map.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <param name="sizeX">The size of the map on the x-axis.</param>
        /// <param name="sizeY">The size of the map on the y-axis.</param>
        /// <returns>True if within the bounds of the map; false otherwise.</returns>
        public static bool WithinMapBounds(Tuple<int, int> position, int sizeX, int sizeY)
        {
            if (position.Item1 < 0 || position.Item1 >= sizeX) return false;
            if (position.Item2 < 0 || position.Item2 >= sizeY) return false;
            return true;
        }
    }
}
