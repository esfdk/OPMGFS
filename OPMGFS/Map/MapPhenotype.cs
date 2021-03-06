﻿namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;

    using OPMGFS.Map.CellularAutomata;

    using Half = Enums.Half;
    using HeightLevel = Enums.HeightLevel;
    using Item = Enums.Item;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// The map phenotype.
    /// </summary>
    public class MapPhenotype
    {
        #region Fields

        /// <summary>
        /// The half that the map is being generated on.
        /// </summary>
        private Half mapHalf;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MapPhenotype"/> class. 
        /// </summary>
        /// <param name="xSize"> The height of the map. </param>
        /// <param name="ySize"> The width of the map. </param>
        public MapPhenotype(int xSize, int ySize)
        {
            this.HeightLevels = new HeightLevel[xSize, ySize];
            this.MapItems = new Item[xSize, ySize];
            this.DestructibleRocks = new bool[xSize, ySize];
            this.CliffPositions = new HashSet<Tuple<int, int>>();

            this.XSize = this.HeightLevels.GetLength(0);
            this.YSize = this.HeightLevels.GetLength(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPhenotype"/> class. 
        /// </summary>
        /// <param name="heightLevels"> The height levels of the map. </param>
        /// <param name="mapItems"> The items contained in the map. </param>
        public MapPhenotype(HeightLevel[,] heightLevels, Item[,] mapItems)
        {
            this.HeightLevels = heightLevels;
            this.MapItems = mapItems;

            this.XSize = this.HeightLevels.GetLength(0);
            this.YSize = this.HeightLevels.GetLength(1);

            this.DestructibleRocks = new bool[this.XSize, this.YSize];
            this.CliffPositions = new HashSet<Tuple<int, int>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPhenotype"/> class. 
        /// </summary>
        /// <param name="heightLevels"> The height levels of the map. </param>
        /// <param name="mapItems"> The items contained in the map. </param>
        /// <param name="destructibleRocks"> The array of destructible rocks. </param>
        public MapPhenotype(HeightLevel[,] heightLevels, Item[,] mapItems, bool[,] destructibleRocks)
            : this(heightLevels, mapItems)
        {
            this.DestructibleRocks = destructibleRocks;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the height of the map.
        /// </summary>
        public int XSize { get; private set; }

        /// <summary>
        /// Gets the width of the map.
        /// </summary>
        public int YSize { get; private set; }

        /// <summary>
        /// Gets the height levels at various parts of the map.
        /// </summary>
        public HeightLevel[,] HeightLevels { get; private set; }

        /// <summary>
        /// Gets the items in the map.
        /// </summary>
        public Item[,] MapItems { get; private set; }

        /// <summary>
        /// Gets the destructible rocks in the map.
        /// </summary>
        public bool[,] DestructibleRocks { get; private set; }

        /// <summary>
        /// Gets a list of all positions that contains a cliff.
        /// </summary>
        public HashSet<Tuple<int, int>> CliffPositions { get; private set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a finished map (placing cliffs, smoothing them, turning/mirroring the map, etc.)
        /// </summary>
        /// <param name="half"> The half to work on. </param>
        /// <param name="function"> The way to create the complete map. </param>
        /// <returns> The finished map. </returns>
        public MapPhenotype CreateFinishedMap(Half half, Enums.MapFunction function)
        {
            var tempMap = this.CreateCompleteMap(half, function);
            tempMap.PlaceCliffs();
            tempMap.SmoothCliffs();
            tempMap = tempMap.CreateCompleteMap(half, function);
            return tempMap;
        }

        /// <summary>
        /// Creates a map where one part has been turned onto the other part of the map.
        /// </summary>
        /// <param name="half"> The half to work on. </param>
        /// <param name="function"> The way to create the complete map. </param>
        /// <returns> The complete map. </returns>
        public MapPhenotype CreateCompleteMap(Half half, Enums.MapFunction function)
        {
            this.mapHalf = half;

            var newHeightLevels = (HeightLevel[,])this.HeightLevels.Clone();
            var newMapItems = (Item[,])this.MapItems.Clone();
            var rocks = (bool[,])this.DestructibleRocks.Clone();

            // Figures out which part of the map that should be looked at.
            var xStart = (half == Half.Right) ? this.XSize / 2 : 0;
            var xEnd = (half == Half.Left) ? this.XSize / 2 : this.XSize;
            var yStart = (half == Half.Top) ? this.YSize / 2 : 0;
            var yEnd = (half == Half.Bottom) ? this.YSize / 2 : this.YSize;

            for (var y = yStart; y < yEnd; y++)
            {
                var otherY = y;

                // If we mirror top or bottom or turn the map, find the height to copy to.
                if ((function == Enums.MapFunction.Mirror && (half == Half.Top || half == Half.Bottom))
                    || function == Enums.MapFunction.Turn)
                    otherY = this.YSize - y - 1;

                for (var x = xStart; x < xEnd; x++)
                {
                    var otherX = x;

                    // If we mirror left or right or turn the map, find the width to copy to.
                    if ((function == Enums.MapFunction.Mirror && (half == Half.Left || half == Half.Right))
                        || function == Enums.MapFunction.Turn)
                        otherX = this.XSize - x - 1;

                    newHeightLevels[otherX, otherY] = this.HeightLevels[x, y];
                    newMapItems[otherX, otherY] = this.MapItems[x, y];
                    rocks[otherX, otherY] = this.DestructibleRocks[x, y];
                }
            }

            var newMap = new MapPhenotype(newHeightLevels, newMapItems, rocks);
            return newMap;
        }

        /// <summary>
        /// Checks if a point is inside the top half of the map.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>True if the point is inside, otherwise false.</returns>
        public bool InsideTopHalf(int x, int y)
        {
            return !(x < 0 || y < (this.YSize / 2.0) || x >= this.XSize || y >= this.YSize);
        }

        /// <summary>
        /// Removes cliffs that are not placed between two different height levels.
        /// </summary>
        public void SmoothCliffs()
        {
            var xStart = (this.mapHalf == Half.Right) ? (this.XSize / 2) - (int)(this.XSize * 0.1) : 0;
            var xEnd = (this.mapHalf == Half.Left) ? (this.XSize / 2) + (int)(this.XSize * 0.1) : this.XSize;
            var yStart = (this.mapHalf == Half.Top) ? (this.YSize / 2) - (int)(this.YSize * 0.1) : 0;
            var yEnd = (this.mapHalf == Half.Bottom) ? (this.YSize / 2) + (int)(this.YSize * 0.1) : this.YSize;

            // Initial smoothing
            var positionsCliffAdded = new List<Position>();
            for (var tempY = yStart; tempY < yEnd; tempY++)
            {
                for (var tempX = xStart; tempX < xEnd; tempX++)
                {
                    var result = this.SmoothCliffAtPoint(tempX, tempY);
                    if (result != null)
                        positionsCliffAdded.Add(result);
                }
            }

            // More smoothing in case there are single tiles lying around.
            var iterations = 0;
            var moves = new List<int> { -3, -2, -1, 0, 1, 2, 3 };
            while (positionsCliffAdded.Count > 0)
            {
                var tempList = new List<Position>();

                foreach (var pca in positionsCliffAdded)
                {
                    foreach (var moveX in moves)
                    {
                        foreach (var moveY in moves)
                        {
                            var x = pca.Item1 + moveX;
                            var y = pca.Item2 + moveY;

                            if (!MapHelper.WithinMapBounds(x, y, this.XSize, this.YSize))
                                continue;

                            var result = this.SmoothCliffAtPoint(x, y);
                            if (result != null)
                                tempList.Add(result);
                        }
                    }
                }

                iterations++;
                positionsCliffAdded = tempList;
                if (iterations > 10) break;
            }

            ////// Old implementation.
            ////for (var tempY = yStart; tempY < yEnd; tempY++)
            ////{
            ////    for (var tempX = xStart; tempX < xEnd; tempX++)
            ////    {
            ////        if (this.HeightLevels[tempX, tempY] != HeightLevel.Cliff)
            ////            continue;

            ////        var neighbours = MapHelper.GetNeighbours(tempX, tempY, this.HeightLevels);

            ////        // Count number of different height levels that are around the tile.
            ////        var differentHeights = neighbours.Where(neighbour => neighbour.Key != HeightLevel.Cliff).Count(neighbour => neighbour.Value > 0);
            ////        if (differentHeights >= 2)
            ////            continue;

            ////        var convertTo = HeightLevel.Cliff;
            ////        foreach (var neighbour in neighbours)
            ////        {
            ////            if (neighbour.Key == HeightLevel.Cliff) continue;
            ////            if (neighbour.Value == 0) continue;

            ////            convertTo = neighbour.Key;
            ////            break;
            ////        }

            ////        this.HeightLevels[tempX, tempY] = convertTo;
            ////    }
            ////}
        }

        /// <summary>
        /// Smooth the terrain of the map.
        /// </summary>
        /// <param name="smoothingNormalNeighbourhood"> If the number of neighbours in the normal Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingExtNeighbourhood"> If the number of neighbours in the extended Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingGenerations"> The number of "generations" to run. </param>
        /// <param name="newRuleset"> The ruleset to use for smoothing. If null, default ruleset is used. </param>
        /// <param name="random"> The random to use during smoothing.</param>
        public void SmoothTerrain(
            int smoothingNormalNeighbourhood = 2,
            int smoothingExtNeighbourhood = 6,
            int smoothingGenerations = 10,
            List<Rule> newRuleset = null,
            Random random = null)
        {
            var smoothCA = new CellularAutomata.CellularAutomata(
                this.XSize,
                this.YSize,
                this.mapHalf,
                this.HeightLevels,
                random);

            if (newRuleset == null)
            {
                smoothCA.SetRuleset(this.GetSmoothingRules(smoothingNormalNeighbourhood, smoothingExtNeighbourhood));
                smoothCA.RunGenerations(smoothingGenerations);
                this.HeightLevels = smoothCA.Map;
            }
            else
            {
                smoothCA.SetRuleset(newRuleset);
                smoothCA.RunGenerations(smoothingGenerations);
                this.HeightLevels = smoothCA.Map;
            }
        }

        /// <summary>
        /// Places cliffs in the map.
        /// </summary>
        public void PlaceCliffs()
        {
            var tempMap = (HeightLevel[,])this.HeightLevels.Clone();

            var xStart = (this.mapHalf == Half.Right) ? (this.XSize / 2) - (int)(this.XSize * 0.1) : 0;
            var xEnd = (this.mapHalf == Half.Left) ? (this.XSize / 2) + (int)(this.XSize * 0.1) : this.XSize;
            var yStart = (this.mapHalf == Half.Top) ? (this.YSize / 2) - (int)(this.YSize * 0.1) : 0;
            var yEnd = (this.mapHalf == Half.Bottom) ? (this.YSize / 2) + (int)(this.YSize * 0.1) : this.YSize;

            for (var y = yStart; y < yEnd; y++)
            {
                for (var x = xStart; x < xEnd; x++)
                {
                    // If this position is a cliff or ramp, don't bother placing cliffs around.
                    if (this.HeightLevels[x, y] == HeightLevel.Cliff
                        || this.HeightLevels[x, y] == HeightLevel.Ramp01
                        || this.HeightLevels[x, y] == HeightLevel.Ramp12)
                        continue;

                    var positionsLowerThanMe = new List<Tuple<int, int>>();
                    var neighbourPositions = MapHelper.GetNeighbourPositions(
                        x,
                        y,
                        this.HeightLevels,
                        RuleEnums.Neighbourhood.VonNeumann);

                    foreach (var np in neighbourPositions)
                    {
                        // Don't do anything if we are looking at a cliff or a ramp.
                        if (this.HeightLevels[np.Item1, np.Item2] == HeightLevel.Cliff
                            || this.HeightLevels[np.Item1, np.Item2] == HeightLevel.Ramp01
                            || this.HeightLevels[np.Item1, np.Item2] == HeightLevel.Ramp12) continue;

                        // If there are no items on the position being checked, place a cliff.
                        if ((this.MapItems[np.Item1, np.Item2] == Item.None
                            || this.MapItems[np.Item1, np.Item2] == Item.Occupied)
                            && (int)this.HeightLevels[np.Item1, np.Item2] < (int)this.HeightLevels[x, y])
                            tempMap[np.Item1, np.Item2] = HeightLevel.Cliff;
                        else if (this.MapItems[np.Item1, np.Item2] != Item.None
                            && (int)this.HeightLevels[np.Item1, np.Item2] < (int)this.HeightLevels[x, y]) // If there is an item, place a cliff on x, y
                            tempMap[x, y] = HeightLevel.Cliff;

                        if ((int)this.HeightLevels[np.Item1, np.Item2] < (int)this.HeightLevels[x, y])
                            positionsLowerThanMe.Add(np);
                    }

                    if (positionsLowerThanMe.Count >= 2)
                    {
                        if (this.MapItems[x, y] == Item.None || this.MapItems[x, y] == Item.Occupied)
                        {
                            tempMap[x, y] = HeightLevel.Cliff;
                        }
                        else
                        {
                            // Iterate over all combinations of neighbours
                            foreach (var np1 in neighbourPositions)
                            {
                                foreach (var np2 in neighbourPositions)
                                {
                                    // If they are equal, don't do anything.
                                    if (np1.Equals(np2)) continue;
                                    if ((int)this.HeightLevels[np1.Item1, np1.Item2] >= (int)this.HeightLevels[x, y]) continue;
                                    if ((int)this.HeightLevels[np2.Item1, np2.Item2] >= (int)this.HeightLevels[x, y]) continue;

                                    // If they are opposite of each other, don't do anything.
                                    var diff = new Tuple<int, int>(
                                        np1.Item1 - np2.Item1,
                                        np2.Item2 - np1.Item2);
                                    if (diff.Item1 == 0 || diff.Item2 == 0) continue;

                                    // Place a cliff in the space "between" the neighbours.
                                    tempMap[x + diff.Item1, y + diff.Item2] = HeightLevel.Cliff;
                                }
                            }
                        }
                    }
                }
            }

            this.HeightLevels = tempMap;
        }

        /// <summary>
        /// Saves the map to a PNG file.
        /// </summary>
        /// <param name="fileNameAddition"> An extra part to add to the file name, when generating maps during testing.  </param>
        /// <param name="folder"> Save the map to a special folder in Images/Finished Maps. NOTE: Just give the name of the folder to create/save in, no extra characters such as \.  </param>
        /// <param name="heightMap"> Whether the height map should be printed. </param>
        /// <param name="itemMap"> Whether the item map should be printed. </param>
        public void SaveMapToPngFile(string fileNameAddition = "", string folder = "", bool heightMap = true, bool itemMap = true)
        {
            // The dictionaries and bitmap.
            var heightDic = MapHelper.GetHeightmapImageDictionary();
            var itemDic = MapHelper.GetItemImageDictionary();
            var bm = new Bitmap((this.XSize * MapHelper.SizeOfMapTiles) + 1, (this.YSize * MapHelper.SizeOfMapTiles) + 1);

            // The file names
            var currentTime = string.Format("{0}.{1}_{2}.{3}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
            currentTime = currentTime.Replace("/", ".");
            currentTime = currentTime.Replace(":", ".");
            currentTime = currentTime.Replace(" ", "_");

            var mapDir = Path.Combine(MapHelper.GetImageDirectory(), @"Finished Maps");

            // If the folder parameter is not empty, create a folder.
            if (!folder.Equals(string.Empty))
                mapDir = Path.Combine(mapDir, folder);

            // IF there is no file name addition, don't add an underscore.
            fileNameAddition = !fileNameAddition.Equals(string.Empty) ? "_" + fileNameAddition : fileNameAddition;

            Directory.CreateDirectory(mapDir);

            var mapHeightFile = @"Map_" + currentTime + fileNameAddition + ".png";
            var mapItemFile = @"Map_" + currentTime + "_Items" + fileNameAddition + ".png";

            // Creating heightmap
            using (var g = Graphics.FromImage(bm))
            {
                g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, bm.Width, bm.Height);

                for (var y = this.YSize - 1; y >= 0; y--)
                {
                    var drawY = ((bm.Height - 1) - (y * MapHelper.SizeOfMapTiles) + 1) - MapHelper.SizeOfMapTiles;

                    for (var x = 0; x < this.XSize; x++)
                    {
                        var drawX = (x * MapHelper.SizeOfMapTiles) + 1;
                        g.DrawImage(heightDic[this.HeightLevels[x, y]], drawX, drawY);
                    }
                }
            }

            if (heightMap)
            {
                // Saving map
                bm.Save(Path.Combine(mapDir, mapHeightFile));
            }

            // Adding Items to the map.
            using (var g = Graphics.FromImage(bm))
            {
                for (var y = this.YSize - 1; y >= 0; y--)
                {
                    var drawY = ((bm.Height - 1) - (y * MapHelper.SizeOfMapTiles) + 1) - MapHelper.SizeOfMapTiles;

                    for (var x = 0; x < this.XSize; x++)
                    {
                        var drawX = (x * MapHelper.SizeOfMapTiles) + 1;
                        if (this.DestructibleRocks[x, y])
                        {
                            g.DrawImage(itemDic[Item.DestructibleRocks], drawX, drawY);
                        }
                        else
                        {
                            g.DrawImage(itemDic[this.MapItems[x, y]], drawX, drawY);
                        }
                    }
                }
            }

            if (itemMap)
            {
                // Saving map
                bm.Save(Path.Combine(mapDir, mapItemFile));
            }

            bm.Dispose();
        }

        /// <summary>
        /// Builds both maps as strings.
        /// </summary>
        /// <param name="mapHeightLevels"> The string to return the heightlevel map to. </param>
        /// <param name="mapItems"> The string to return the item map to. </param>
        public void GetMapStrings(out string mapHeightLevels, out string mapItems)
        {
            var heightLevelBuilder = new StringBuilder();
            var itemBuilder = new StringBuilder();

            // Top border
            for (var x = 0; x < this.XSize + 2; x++)
            {
                heightLevelBuilder.Append("-");
                itemBuilder.Append("-");
            }

            heightLevelBuilder.AppendLine();
            itemBuilder.AppendLine();

            for (var y = this.YSize - 1; y >= 0; y--)
            {
                // Left border
                heightLevelBuilder.Append("|");
                itemBuilder.Append("|");

                // Actual map values
                for (int x = 0; x < this.XSize; x++)
                {
                    heightLevelBuilder.Append(Enums.GetCharValue(this.HeightLevels[x, y]));
                    itemBuilder.Append(Enums.GetCharValue(this.MapItems[x, y]));
                }

                // Right border
                heightLevelBuilder.Append("| " + y);
                itemBuilder.Append("|");

                heightLevelBuilder.AppendLine();
                itemBuilder.AppendLine();
            }

            // Bottom border
            for (var x = 0; x < this.XSize + 2; x++)
            {
                heightLevelBuilder.Append("-");
                itemBuilder.Append("-");
            }

            mapHeightLevels = heightLevelBuilder.ToString();
            mapItems = itemBuilder.ToString();
        }

        /// <summary>
        /// Updates the list of cliff positions.
        /// </summary>
        /// <param name="half">
        /// The half to look for cliffs on.
        /// </param>
        public void UpdateCliffPositions(Half half)
        {
            var xStart = (half == Half.Right) ? this.XSize / 2 : 0;
            var xEnd = (half == Half.Left) ? this.XSize / 2 : this.XSize;
            var yStart = (half == Half.Top) ? this.YSize / 2 : 0;
            var yEnd = (half == Half.Bottom) ? this.YSize / 2 : this.YSize;

            this.CliffPositions.Clear();

            for (var x = xStart; x < xEnd; x++)
            {
                for (var y = yStart; y < yEnd; y++)
                {
                    if (this.HeightLevels[x, y] == HeightLevel.Cliff)
                    {
                        this.CliffPositions.Add(new Tuple<int, int>(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the cliff positions.
        /// </summary>
        /// <param name="cliffPositions"> The new list of cliff positions. </param>
        public void UpdateCliffPositions(HashSet<Tuple<int, int>> cliffPositions)
        {
            this.CliffPositions.Clear();
            foreach (var t in cliffPositions)
            {
                this.CliffPositions.Add(t);
            }
        }

        /// <summary>
        /// Checks if a tile is passable.
        /// </summary>
        /// <param name="x"> The x-position. </param>
        /// <param name="y"> The y-position. </param>
        /// <returns> True if tile is passable, false if not. </returns>
        public bool IsTilePassable(int x, int y)
        {
            if (MapHelper.WithinMapBounds(x, y, this.XSize, this.YSize))
            {
                if (this.HeightLevels[x, y] != HeightLevel.Cliff
                    && this.HeightLevels[x, y] != HeightLevel.Impassable
                    && this.MapItems[x, y] != Item.Base
                    && this.MapItems[x, y] != Item.StartBase
                    && this.MapItems[x, y] != Item.GoldMinerals
                    && this.MapItems[x, y] != Item.BlueMinerals
                    && this.MapItems[x, y] != Item.Gas
                    && this.MapItems[x, y] != Item.XelNagaTower)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Cliff smoothing for a given point on the map. If a heightlevel only has 1 neighbour of its own kind,
        /// it is changed into a cliff instead.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <returns> A position if a point was changed to a cliff; null otherwise. </returns>
        private Position SmoothCliffAtPoint(int x, int y)
        {
            if (this.HeightLevels[x, y] != HeightLevel.Cliff)
            {
                if (this.MapItems[x, y] != Item.None) return null;

                var neighbours = MapHelper.GetNeighbours(x, y, this.HeightLevels, RuleEnums.Neighbourhood.VonNeumann);
                if (neighbours[this.HeightLevels[x, y]] <= 1)
                {
                    this.HeightLevels[x, y] = HeightLevel.Cliff;
                    return new Position(x, y);
                }
            }
            else
            {
                var neighbours = MapHelper.GetNeighbours(x, y, this.HeightLevels);

                // Count number of different height levels that are around the tile.
                var differentHeights = neighbours.Where(neighbour => neighbour.Key != HeightLevel.Cliff).Count(neighbour => neighbour.Value > 0);
                if (differentHeights >= 2)
                    return null;

                var convertTo = HeightLevel.Cliff;
                foreach (var neighbour in neighbours)
                {
                    if (neighbour.Key == HeightLevel.Cliff) continue;
                    if (neighbour.Key == HeightLevel.Ramp01) continue;
                    if (neighbour.Key == HeightLevel.Ramp12) continue;
                    if (neighbour.Value == 0) continue;

                    convertTo = neighbour.Key;
                    break;
                }

                this.HeightLevels[x, y] = convertTo;
            }

            return null;
        }

        /// <summary>
        /// Gets the rules used for smoothing terrain.
        /// </summary>
        /// <param name="smoothingNormalNeighbourhood"> If the number of neighbours in the normal Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingExtNeighbourhood"> If the number of neighbours in the extended Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <returns> A list of the rules that are used for smoothing. </returns>
        private List<Rule> GetSmoothingRules(int smoothingNormalNeighbourhood, int smoothingExtNeighbourhood)
        {
            var list = new List<Rule>();

            // Smooth down height 2
            var ruleH2ToH1 = new RuleDeterministic(HeightLevel.Height1, HeightLevel.Height2);
            ruleH2ToH1.AddCondition(smoothingNormalNeighbourhood, HeightLevel.Height2, RuleEnums.Comparison.LessThanEqualTo);

            var ruleH2ToH1Ext = new RuleDeterministic(HeightLevel.Height1, HeightLevel.Height2)
                {
                    Neighbourhood = RuleEnums.Neighbourhood.MooreExtended
                };
            ruleH2ToH1Ext.AddCondition(smoothingExtNeighbourhood, HeightLevel.Height2, RuleEnums.Comparison.LessThanEqualTo);

            var ruleH2ToH0 = new RuleDeterministic(HeightLevel.Height0, HeightLevel.Height2)
                {
                    Neighbourhood = RuleEnums.Neighbourhood.MooreExtended
                };
            ruleH2ToH0.AddCondition(12, HeightLevel.Height2, RuleEnums.Comparison.LessThanEqualTo);
            ruleH2ToH0.AddCondition(0, HeightLevel.Height1, RuleEnums.Comparison.LessThanEqualTo);

            var ruleH2TooFew = new RuleDeterministic(HeightLevel.Height1, HeightLevel.Height2)
                {
                    Neighbourhood = RuleEnums.Neighbourhood.VonNeumann
                };
            ruleH2TooFew.AddCondition(1, HeightLevel.Height2, RuleEnums.Comparison.LessThanEqualTo);

            list.Add(ruleH2ToH1);
            list.Add(ruleH2ToH1Ext);
            list.Add(ruleH2ToH0);
            list.Add(ruleH2TooFew);

            // Smooth down height 1
            var ruleH1ToH0 = new RuleDeterministic(HeightLevel.Height0, HeightLevel.Height1);
            ruleH1ToH0.AddCondition(smoothingNormalNeighbourhood, HeightLevel.Height1, RuleEnums.Comparison.LessThanEqualTo);

            var ruleH1ToH0Ext = new RuleDeterministic(HeightLevel.Height0, HeightLevel.Height1)
                {
                    Neighbourhood = RuleEnums.Neighbourhood.MooreExtended
                };
            ruleH1ToH0Ext.AddCondition(smoothingExtNeighbourhood, HeightLevel.Height1, RuleEnums.Comparison.LessThanEqualTo);

            list.Add(ruleH1ToH0);
            list.Add(ruleH1ToH0Ext);

            // Smooth up height 0
            var ruleH0ToH1 = new RuleDeterministic(HeightLevel.Height1, HeightLevel.Height0);
            ruleH0ToH1.AddCondition(smoothingNormalNeighbourhood, HeightLevel.Height0, RuleEnums.Comparison.LessThanEqualTo);

            var ruleH0ToH1Ext = new RuleDeterministic(HeightLevel.Height1, HeightLevel.Height0)
                {
                    Neighbourhood = RuleEnums.Neighbourhood.MooreExtended
                };
            ruleH0ToH1Ext.AddCondition(smoothingExtNeighbourhood, HeightLevel.Height0, RuleEnums.Comparison.LessThanEqualTo);

            var ruleH0TooFew = new RuleDeterministic(HeightLevel.Height1, HeightLevel.Height0)
                {
                    Neighbourhood = RuleEnums.Neighbourhood.VonNeumann
                };
            ruleH0TooFew.AddCondition(1, HeightLevel.Height0, RuleEnums.Comparison.LessThanEqualTo);

            list.Add(ruleH0ToH1);
            list.Add(ruleH0ToH1Ext);
            list.Add(ruleH0TooFew);

            return list;
        }

        #endregion
    }
}
