using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPMGFS.Map.CellularAutomata
{
    using Position = Tuple<int, int>;

    /// <summary>
    /// The abstract ruleset.
    /// Idea from http://www.primaryobjects.com/CMS/Article106
    /// </summary>
    public abstract class Ruleset
    {
        /// <summary>
        /// Gets the next generation following the ruleset.
        /// </summary>
        /// <param name="map"> The map to progress. </param>
        /// <param name="caXStart"> The x start position of the cellular automata. </param>
        /// <param name="caXEnd"> The x end position of the cellular automata. </param>
        /// <param name="caYStart"> The y start position of the cellular automata. </param>
        /// <param name="caYEnd"> The y end position of the cellular automata. </param>
        /// <returns> A map representing the next generation. </returns>
        public abstract Enums.HeightLevel[,] NextGeneration(
            Enums.HeightLevel[,] map,
            int caXStart,
            int caXEnd,
            int caYStart,
            int caYEnd);

        /// <summary>
        /// Counts the number of neighbours to (x, y) of the the given type.
        /// </summary>
        /// <param name="x"> The x-coordinate to check neighbours for. </param>
        /// <param name="y"> The y-coordinate to check neighbours for. </param>
        /// <param name="map"> The map. </param>
        /// <param name="type"> The type to count for. </param>
        /// <returns> The number of neighbours of the given type. </returns>
        protected int NumberOfNeighboursOfType(int x, int y, Enums.HeightLevel[,] map, Enums.HeightLevel type)
        {
            var moves = new[] { -1, 0, 1 };
            var neighbours = 0;
            var sizeX = map.GetLength(0);
            var sizeY = map.GetLength(1);

            foreach (var moveX in moves)
            {
                foreach (var moveY in moves)
                {
                    if (moveX == 0 && moveY == 0) continue;

                    var posToCheck = new Position(x + moveX, y + moveY);
                    if (!this.WithinMapBounds(posToCheck, sizeX, sizeY)) continue;
                    if (map[posToCheck.Item1, posToCheck.Item2] == type) neighbours++;
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
        protected bool WithinMapBounds(Position position, int sizeX, int sizeY)
        {
            if (position.Item1 < 0 || position.Item1 >= sizeX) return false;
            if (position.Item2 < 0 || position.Item2 >= sizeY) return false;
            return true;
        }
    }
}
