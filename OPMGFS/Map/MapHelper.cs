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
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using OPMGFS.Map.CellularAutomata;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// A class that contains methods used by multiple map classes.
    /// </summary>
    public static class MapHelper
    {
        #region Fields

        /// <summary>
        /// The size of map tiles.
        /// </summary>
        public const int SizeOfMapTiles = 17;

        /// <summary>
        /// The random generator.
        /// </summary>
        private static Random random = new Random();

        #endregion

        #region Properties

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the neighbour positions for the given position.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="map"> The map to get positions for. </param>
        /// <param name="neighbourhood"> The type of neighbourhood to look at. </param>
        /// <returns> A list of the coordinates for the neighbour positions. </returns>
        public static List<Position> GetNeighbourPositions(int x, int y, Enums.HeightLevel[,] map, RuleEnums.Neighbourhood neighbourhood = RuleEnums.Neighbourhood.Moore)
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
        /// Finds the reachable tiles within range from the given position.
        /// </summary>
        /// <param name="x"> The x-coordinate of the start position. </param>
        /// <param name="y"> The y-coordinate of the start position. </param>
        /// <param name="map"> The map to work on. </param>
        /// <param name="radius"> The radius to find tiles within.</param>
        /// <returns> A list of the positions. </returns>
        public static List<Position> GetReachableTilesFrom(
            int x,
            int y,
            Enums.HeightLevel[,] map,
            int radius = 5)
        {
            var tileList = new List<Enums.HeightLevel>();
            var visitedPositions = new List<Position>();
            var openPositions = new List<Position>();

            if (radius <= 0) radius = 1;

            openPositions.Add(new Position(x, y));

            while (!(openPositions.Count <= 0))
            {
                var currentPos = openPositions[0];
                openPositions.Remove(currentPos);

                if (Math.Abs(currentPos.Item1 - x) > radius || Math.Abs(currentPos.Item2 - y) > radius)
                    continue;

                visitedPositions.Add(currentPos);
                tileList.Add(map[currentPos.Item1, currentPos.Item2]);

                var neighbours = MapPathfinding.Neighbours(map, currentPos);
                foreach (var neighbour in neighbours)
                {
                    if (visitedPositions.Contains(neighbour) || openPositions.Contains(neighbour)) continue;
                    if (Math.Abs(neighbour.Item1 - x) > radius || Math.Abs(neighbour.Item2 - y) > radius) continue;
                    if (map[neighbour.Item1, neighbour.Item2] == Enums.HeightLevel.Cliff) continue;
                    if (map[neighbour.Item1, neighbour.Item2] == Enums.HeightLevel.Impassable) continue;

                    openPositions.Add(neighbour);
                }
            }

            return visitedPositions;
        }

        /// <summary>
        /// Finds the position of the nearest tile of the given type.
        /// </summary>
        /// <param name="x"> The x-coordinate of the start position. </param>
        /// <param name="y"> The y-coordinate of the start position. </param>
        /// <param name="map"> The map to work on. </param>
        /// <param name="toFind"> The type of tile to find. </param>
        /// <param name="adhereToPathfinding"> Determines if we are adhering to pathfinding or not. </param>
        /// <returns> The position of the nearest found  tile of the given type or null. </returns>
        public static Position FindNearestTileOfType(int x, int y, Enums.HeightLevel[,] map, Enums.HeightLevel toFind, bool adhereToPathfinding = false)
        {
            var tileList = new List<Enums.HeightLevel>();
            var visitedPositions = new List<Position>();
            var openPositions = new List<Position>();
            Position result = null;

            openPositions.Add(new Position(x, y));

            while (!(openPositions.Count <= 0))
            {
                if (result != null) break;

                var currentPos = openPositions[0];
                openPositions.Remove(currentPos);

                visitedPositions.Add(currentPos);
                tileList.Add(map[currentPos.Item1, currentPos.Item2]);

                var neighbours = MapPathfinding.Neighbours(map, currentPos);
                foreach (var neighbour in neighbours)
                {
                    if (map[neighbour.Item1, neighbour.Item2] == toFind) result = neighbour;
                    if (visitedPositions.Contains(neighbour) || openPositions.Contains(neighbour)) continue;
                    if (adhereToPathfinding
                        && map[neighbour.Item1, neighbour.Item2] == Enums.HeightLevel.Cliff) continue;
                    if (adhereToPathfinding
                        && map[neighbour.Item1, neighbour.Item2] == Enums.HeightLevel.Impassable) continue;

                    openPositions.Add(neighbour);
                }
            }

            return result;
        }

        /// <summary>
        /// Finds the position of the nearest tile of the given type.
        /// </summary>
        /// <param name="x"> The x-coordinate of the start position. </param>
        /// <param name="y"> The y-coordinate of the start position. </param>
        /// <param name="map"> The map to work on. </param>
        /// <param name="toFind"> The type of tile to find. </param>
        /// <returns> The position of the nearest found  tile of the given type or null. </returns>
        public static Position FindNearestTileOfType(int x, int y, Enums.Item[,] map, Enums.Item toFind)
        {
            var tileList = new List<Enums.Item>();
            var visitedPositions = new List<Position>();
            var openPositions = new List<Position>();
            Position result = null;

            openPositions.Add(new Position(x, y));

            while (!(openPositions.Count <= 0))
            {
                if (result != null) break;

                var currentPos = openPositions[0];
                openPositions.Remove(currentPos);

                visitedPositions.Add(currentPos);
                tileList.Add(map[currentPos.Item1, currentPos.Item2]);

                var neighbours = MapPathfinding.Neighbours(map, currentPos);
                foreach (var neighbour in neighbours)
                {
                    if (map[neighbour.Item1, neighbour.Item2] == toFind) result = neighbour;
                    if (visitedPositions.Contains(neighbour) || openPositions.Contains(neighbour)) continue;

                    openPositions.Add(neighbour);
                }
            }

            return result;
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
            foreach (var heightlevel in Enum.GetValues(typeof(Enums.HeightLevel)))
                neighbours[(Enums.HeightLevel)heightlevel] = 0;

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
        /// Checks if the given position is close to any of the positions in the list.
        /// </summary>
        /// <param name="pos"> The position to check. </param>
        /// <param name="positions"> The list of positions to compare to. </param>
        /// <param name="range"> The range that defines "close to". </param>
        /// <returns> True if the position is close to any of the positions in the list; false otherwise. </returns>
        public static bool CloseToAny(Position pos, IEnumerable<Position> positions, int range = 3)
        {
            return positions.Any(position => Math.Abs(pos.Item1 - position.Item1) <= range || Math.Abs(pos.Item2 - position.Item2) <= range);
        }

        /// <summary>
        /// Finds the position in the list that the given position is closest to (within range) and returns the actual
        /// distance to that position.
        /// </summary>
        /// <param name="pos"> The position to check. </param>
        /// <param name="positions"> The list of positions to compare to. </param>
        /// <param name="range"> The range that defines "close to". </param>
        /// <returns> The range to the closest position in the list within range. </returns>
        public static int ClosestTo(Position pos, IEnumerable<Position> positions, int range = 3)
        {
            var closestRange = 100000;
            foreach (var position in positions.Where(position => Math.Abs(pos.Item1 - position.Item1) <= range || Math.Abs(pos.Item2 - position.Item2) <= range))
            {
                if (Math.Abs(pos.Item1 - position.Item1) < closestRange) closestRange = Math.Abs(pos.Item1 - position.Item1);
                if (Math.Abs(pos.Item2 - position.Item2) < closestRange) closestRange = Math.Abs(pos.Item2 - position.Item2);
            }

            return closestRange;
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

        #endregion

        #region Map Drawing methods

        /// <summary>
        /// Gets the directory the Images folder is located at.
        /// </summary>
        /// <returns> The directory. </returns>
        public static string GetImageDirectory()
        {
            var imgDir = Directory.GetCurrentDirectory();
            imgDir = imgDir.Substring(0, imgDir.IndexOf(@"\bin", StringComparison.Ordinal));
            return Path.Combine(imgDir, @"Images");
        }

        /// <summary>
        /// Gets a dictionary that contains an image for every type of HeightLevel enum.
        /// </summary>
        /// <returns> A dictionary containing all the images. </returns>
        public static Dictionary<Enums.HeightLevel, Image> GetHeightmapImageDictionary()
        {
            var imgDir = Path.Combine(GetImageDirectory(), @"Tiles");
            var tileDic = new Dictionary<Enums.HeightLevel, Image>
                              {
                                  { Enums.HeightLevel.Height0, Image.FromFile(Path.Combine(imgDir, @"Height0.png")) },
                                  { Enums.HeightLevel.Height1, Image.FromFile(Path.Combine(imgDir, @"Height1.png")) },
                                  { Enums.HeightLevel.Height2, Image.FromFile(Path.Combine(imgDir, @"Height2.png")) },
                                  { Enums.HeightLevel.Ramp01, Image.FromFile(Path.Combine(imgDir, @"Ramp01.png")) },
                                  { Enums.HeightLevel.Ramp12, Image.FromFile(Path.Combine(imgDir, @"Ramp12.png")) },
                                  { Enums.HeightLevel.Cliff, Image.FromFile(Path.Combine(imgDir, @"Cliff.png")) },
                                  { Enums.HeightLevel.Impassable, Image.FromFile(Path.Combine(imgDir, @"Impassable.png")) }
                              };

            return tileDic;
        }

        /// <summary>
        /// Gets a dictionary that contains an image for every type of Item enum.
        /// </summary>
        /// <returns> A dictionary containing all the images. </returns>
        public static Dictionary<Enums.Item, Image> GetItemImageDictionary()
        {
            var imgDir = Path.Combine(GetImageDirectory(), @"Tiles");
            var tileDic = new Dictionary<Enums.Item, Image>
                              {
                                  { Enums.Item.None, Image.FromFile(Path.Combine(imgDir, @"Transparent.png")) },
                                  { Enums.Item.Base, Image.FromFile(Path.Combine(imgDir, @"Base.png")) },
                                  { Enums.Item.StartBase, Image.FromFile(Path.Combine(imgDir, @"StartBase.png")) },
                                  { Enums.Item.BlueMinerals, Image.FromFile(Path.Combine(imgDir, @"BlueMinerals.png")) },
                                  { Enums.Item.GoldMinerals, Image.FromFile(Path.Combine(imgDir, @"GoldMinerals.png")) },
                                  { Enums.Item.Gas, Image.FromFile(Path.Combine(imgDir, @"Gas.png")) },
                                  { Enums.Item.XelNagaTower, Image.FromFile(Path.Combine(imgDir, @"XelNaga.png")) },
                                  { Enums.Item.Occupied, Image.FromFile(Path.Combine(imgDir, @"Transparent.png")) },
                                  { Enums.Item.DestructibleRocks, Image.FromFile(Path.Combine(imgDir, @"DestructibleRocks.png")) },
                              };

            return tileDic;
        }

        #endregion
    }
}
