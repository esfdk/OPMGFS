﻿namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using OPMGFS.Evolution;
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
        /// Gets or sets the random generator.
        /// </summary>
        public static Random Random 
        { 
            get
            {
                return random;
            }

            set
            {
                random = value;
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
        /// <param name="destructibleRocks"> The destructible rocks of the map. </param>
        /// <returns> A list of the positions. </returns>
        public static List<Position> GetReachableTilesFrom(
            int x,
            int y,
            Enums.HeightLevel[,] map,
            int radius = 5,
            bool[,] destructibleRocks = null)
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
                    if (destructibleRocks != null && destructibleRocks[neighbour.Item1, neighbour.Item2]) continue;

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
            return positions.Any(position => CloseTo(pos, position, range));
        }

        /// <summary>
        /// Checks if one position is close to another position.
        /// </summary>
        /// <param name="pos1"> One of the positions. </param>
        /// <param name="pos2"> The other position. </param>
        /// <param name="range"> The range that defines "close to". </param>
        /// <returns> True if the positions are close to each other; false otherwise. </returns>
        public static bool CloseTo(Position pos1, Position pos2, int range = 5)
        {
            return (Math.Pow(Math.Abs(pos1.Item1 - pos2.Item1), 2) + Math.Pow(Math.Abs(pos1.Item2 - pos2.Item2), 2)) <= Math.Pow(range, 2);
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
            foreach (var position in positions.Where(position => CloseTo(pos, position, range)))
            {
                if (Math.Abs(pos.Item1 - position.Item1) < closestRange) closestRange = Math.Abs(pos.Item1 - position.Item1);
                if (Math.Abs(pos.Item2 - position.Item2) < closestRange) closestRange = Math.Abs(pos.Item2 - position.Item2);
            }

            return closestRange;
        }

        /// <summary>
        /// Determines if moving from one position to another is a valid move.
        /// </summary>
        /// <param name="mapHeightLevels"> The height levels of the map. </param>
        /// <param name="fromPos"> The position to travel from. </param>
        /// <param name="toPos">The position to travel to. </param>
        /// <returns> True if the move is valid; false otherwise. </returns>
        public static bool ValidMove(Enums.HeightLevel[,] mapHeightLevels, Position fromPos, Position toPos)
        {
            // If not within map, it is not a valid move.
            if (!WithinMapBounds(toPos, mapHeightLevels.GetLength(0), mapHeightLevels.GetLength(1))) return false;

            // If the height difference is 2 or higher, it is not a valid move.
            if (
                Math.Abs(
                    (int)mapHeightLevels[fromPos.Item1, fromPos.Item2] - (int)mapHeightLevels[toPos.Item1, toPos.Item2])
                >= 2) return false;

            // If the positions are not adjacent, it is not a valid move.
            if (Math.Abs(fromPos.Item1 - toPos.Item1) >= 2 || Math.Abs(fromPos.Item2 - toPos.Item2) >= 2) return false;

            if (mapHeightLevels[toPos.Item1, toPos.Item2] == Enums.HeightLevel.Cliff) return false;

            return true;
        }

        /// <summary>
        /// Checks if a position is within the bounds of the map.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <param name="sizeX">The size of the map on the x-axis.</param>
        /// <param name="sizeY">The size of the map on the y-axis.</param>
        /// <returns>True if within the bounds of the map; false otherwise.</returns>
        public static bool WithinMapBounds(Position position, int sizeX, int sizeY)
        {
            if (position.Item1 < 0 || position.Item1 >= sizeX) return false;
            if (position.Item2 < 0 || position.Item2 >= sizeY) return false;
            return true;
        }

        /// <summary>
        /// Checks if a position is within the bounds of the map.
        /// </summary>
        /// <param name="x"> The x-coordinate to check. </param>
        /// <param name="y"> The y-coordinate to check. </param>
        /// <param name="sizeX">The size of the map on the x-axis.</param>
        /// <param name="sizeY">The size of the map on the y-axis.</param>
        /// <returns>True if within the bounds of the map; false otherwise.</returns>
        public static bool WithinMapBounds(int x, int y, int sizeX, int sizeY)
        {
            return WithinMapBounds(new Position(x, y), sizeX, sizeY);
        }

        public static MapPhenotype IncreaseSizeOfMap(MapPhenotype mp, int size)
        {
            var newMap = new MapPhenotype(mp.XSize * size, mp.YSize * size);

            for (var x = 0; x < mp.XSize; x++)
            {
                for (var y = 0; y < mp.YSize; y++)
                {
                    var newMapX = x * size;
                    var newMapY = y * size;

                    for (var i = 0; i < size; i++)
                    {
                        for (var j = 0; j < size; j++)
                        {
                            newMap.HeightLevels[newMapX + i, newMapY + j] = mp.HeightLevels[x,y];
                        }
                    }
                }
            }

            return newMap;
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

        /// <summary>
        /// Creates a greyscale map that shows novelty, based on how different every tile is in each of the given maps.
        /// </summary>
        /// <param name="maps"> The maps to create a greyscale novelty map from. </param>
        /// <param name="fileNameAddition"> An extra part to add to the file name, when generating maps during testing.  </param>
        /// <param name="folder"> Save the map to a special folder in Images/Finished Maps. NOTE: Just give the name of the folder to create/save in, no extra characters such as \.  </param>
        /// <param name="heightMap"> Whether the height map should be printed. </param>
        /// <param name="itemMap"> Whether the item map should be printed. </param>
        /// <param name="combinedMap"> Whether the combined map should be printed. </param>
        public static void SaveGreyscaleNoveltyMap(
            List<EvolvableMap> maps,
            string fileNameAddition = "",
            string folder = "",
            bool heightMap = true,
            bool itemMap = true,
            bool combinedMap = true)
        {
            var mapList = maps.Select(map => map.ConvertedPhenotype).ToList();
            SaveGreyscaleNoveltyMap(mapList, fileNameAddition, folder, heightMap, itemMap, combinedMap);
        }

        /// <summary>
        /// Creates a greyscale map that shows novelty, based on how different every tile is in each of the given maps.
        /// </summary>
        /// <param name="maps"> The maps to create a greyscale novelty map from. </param>
        /// <param name="fileNameAddition"> An extra part to add to the file name, when generating maps during testing.  </param>
        /// <param name="folder"> Save the map to a special folder in Images/Finished Maps. NOTE: Just give the name of the folder to create/save in, no extra characters such as \.  </param>
        /// <param name="heightMap"> Whether the height map should be printed. </param>
        /// <param name="itemMap"> Whether the item map should be printed. </param>
        /// <param name="combinedMap"> Whether the combined map should be printed. </param>
        public static void SaveGreyscaleNoveltyMap(List<MapPhenotype> maps, string fileNameAddition = "", string folder = "", bool heightMap = true, bool itemMap = true, bool combinedMap = true)
        {
            if (maps == null) return;
            if (maps.Count <= 0) return;

            // Map size
            var xSize = maps[0].HeightLevels.GetLength(0);
            var ySize = maps[0].HeightLevels.GetLength(1);

            // The dictionaries and bitmap.
            var bm = new Bitmap((xSize * SizeOfMapTiles) + 1, (ySize * SizeOfMapTiles) + 1);

            // The file names
            var currentTime = string.Format("{0}.{1}_{2}.{3}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
            currentTime = currentTime.Replace("/", ".");
            currentTime = currentTime.Replace(":", ".");
            currentTime = currentTime.Replace(" ", "_");

            var mapDir = Path.Combine(GetImageDirectory(), @"Finished Maps");

            // If the folder parameter is not empty, create a folder.
            if (!folder.Equals(string.Empty))
                mapDir = Path.Combine(mapDir, folder);

            // IF there is no file name addition, don't add an underscore.
            fileNameAddition = !fileNameAddition.Equals(string.Empty) ? "_" + fileNameAddition : fileNameAddition;

            Directory.CreateDirectory(mapDir);

            var mapHeightFile = @"Map_" + currentTime + fileNameAddition + ".png";
            var mapItemFile = @"Map_" + currentTime + fileNameAddition + "_Items" + ".png";
            var combinedFile = @"Map_" + currentTime + fileNameAddition + "_Combined" + ".png";

            var greyHeightmap = new double[xSize, ySize];
            var greyItemmap = new double[xSize, ySize];
            var greyCombined = new double[xSize, ySize];

            for (var y = ySize - 1; y >= 0; y--)
            {
                for (var x = 0; x < xSize; x++)
                {
                    var heightDict = new Dictionary<Enums.HeightLevel, int>();
                    var itemDict = new Dictionary<Enums.Item, int>();
                    var combinedDict = new Dictionary<Tuple<Enums.HeightLevel, Enums.Item>, int>();

                    // Ensure that all keys are in the dictionary
                    foreach (var heightlevel in Enum.GetValues(typeof(Enums.HeightLevel)))
                    {
                        heightDict[(Enums.HeightLevel)heightlevel] = 0;

                        foreach (var item in Enum.GetValues(typeof(Enums.Item)))
                        {
                            itemDict[(Enums.Item)item] = 0;

                            combinedDict[new Tuple<Enums.HeightLevel, Enums.Item>(
                                (Enums.HeightLevel)heightlevel,
                                    (Enums.Item)item)] = 0;
                        }
                    }

                    // Count the occourance of every item for the specific position in each map.
                    foreach (var mapPhenotype in maps)
                    {
                        heightDict[mapPhenotype.HeightLevels[x, y]] += 1;

                        // If a tile is occupied, count it as none
                        if (mapPhenotype.MapItems[x, y] == Enums.Item.Occupied)
                        {
                            var tuple = new Tuple<Enums.HeightLevel, Enums.Item>(
                                mapPhenotype.HeightLevels[x, y],
                                Enums.Item.None);

                            itemDict[Enums.Item.None] += 1;
                            combinedDict[tuple] += 1;
                        }
                        else
                        {
                            var tuple = new Tuple<Enums.HeightLevel, Enums.Item>(
                                mapPhenotype.HeightLevels[x, y],
                                mapPhenotype.MapItems[x, y]);

                            itemDict[mapPhenotype.MapItems[x, y]] += 1;
                            combinedDict[tuple] += 1;
                        }

                        if (mapPhenotype.DestructibleRocks[x, y]) itemDict[Enums.Item.DestructibleRocks] += 1;
                    }

                    var normalizedHeights = heightDict.Select(tuple => ((tuple.Value - 0d) / (maps.Count - 0d))).ToList();
                    var normalizedItems = itemDict.Select(tuple => ((tuple.Value - 0d) / (maps.Count - 0d))).ToList();
                    var normalizedCombined = combinedDict.Select(tuple => ((tuple.Value - 0d) / (maps.Count - 0d))).ToList();

                    greyHeightmap[x, y] = normalizedHeights.Max(value => value);
                    greyItemmap[x, y] = normalizedItems.Max(value => value);
                    greyCombined[x, y] = normalizedCombined.Max(value => value);
                }
            }

            // Creating heightmap
            using (var g = Graphics.FromImage(bm))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, bm.Width, bm.Height);

                for (var y = ySize - 1; y >= 0; y--)
                {
                    var drawY = ((bm.Height - 1) - (y * SizeOfMapTiles) + 1) - SizeOfMapTiles;

                    for (var x = 0; x < xSize; x++)
                    {
                        var drawX = (x * SizeOfMapTiles) + 1;

                        var newColor = Color.FromArgb((int)(greyHeightmap[x, y] * 255), Color.Black);
                        g.FillRectangle(new SolidBrush(newColor), drawX, drawY, SizeOfMapTiles, SizeOfMapTiles);
                    }
                }
            }

            if (heightMap)
            {
                // Saving map
                bm.Save(Path.Combine(mapDir, mapHeightFile));
            }

            // Creating heightmap
            using (var g = Graphics.FromImage(bm))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, bm.Width, bm.Height);

                for (var y = ySize - 1; y >= 0; y--)
                {
                    var drawY = ((bm.Height - 1) - (y * SizeOfMapTiles) + 1) - SizeOfMapTiles;

                    for (var x = 0; x < xSize; x++)
                    {
                        var drawX = (x * SizeOfMapTiles) + 1;

                        var newColor = Color.FromArgb((int)(greyItemmap[x, y] * 255), Color.Black);
                        g.FillRectangle(new SolidBrush(newColor), drawX, drawY, SizeOfMapTiles, SizeOfMapTiles);
                    }
                }
            }

            if (heightMap)
            {
                // Saving map
                bm.Save(Path.Combine(mapDir, mapItemFile));
            }

            // Creating heightmap
            using (var g = Graphics.FromImage(bm))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, bm.Width, bm.Height);

                for (var y = ySize - 1; y >= 0; y--)
                {
                    var drawY = ((bm.Height - 1) - (y * SizeOfMapTiles) + 1) - SizeOfMapTiles;

                    for (var x = 0; x < xSize; x++)
                    {
                        var drawX = (x * SizeOfMapTiles) + 1;

                        var drawValue = greyCombined[x, y];
                        var newColor = Color.FromArgb((int)(drawValue * 255), Color.Black);
                        g.FillRectangle(new SolidBrush(newColor), drawX, drawY, SizeOfMapTiles, SizeOfMapTiles);
                    }
                }
            }

            if (combinedMap)
            {
                // Saving map
                bm.Save(Path.Combine(mapDir, combinedFile));
            }

            bm.Dispose();
        }

        #endregion
    }
}
