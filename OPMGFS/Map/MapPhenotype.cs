// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapPhenotype.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPhenotype type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;

    using OPMGFS.Map.CellularAutomata;

    using Half = Enums.Half;
    using HeightLevel = Enums.HeightLevel;
    using Item = Enums.Item;

    /// <summary>
    /// The map phenotype.
    /// </summary>
    public class MapPhenotype
    {
        #region Fields

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
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a map where one part has been turned onto the other part of the map.
        /// </summary>
        /// <param name="half"> The half to turn. </param>
        /// <param name="function"> The way to create the complete map. </param>
        /// <returns> The complete map. </returns>
        public MapPhenotype CreateCompleteMap(Half half, Enums.MapFunction function)
        {
            var newHeightLevels = (HeightLevel[,])this.HeightLevels.Clone();
            var newMapItems = (Item[,])this.MapItems.Clone();

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
                }
            }

            var newMap = new MapPhenotype(newHeightLevels, newMapItems);
            return newMap;
        }

        /// <summary>
        /// Checks if a point is inside the boundaries of the map.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>True if the point is inside, otherwise false.</returns>
        public bool InsideTopHalf(int x, int y)
        {
            return !(x < 0 || y < (YSize / 2.0) || x >= this.XSize || y >= this.YSize);
        }

        /// <summary>
        /// Smooth the terrain of the map.
        /// </summary>
        /// <param name="smoothingNormalNeighbourhood"> If the number of neighbours in the normal Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingExtNeighbourhood"> If the number of neighbours in the extended Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingGenerations"> The number of "generations" to run. </param>
        public void SmoothTerrain(int smoothingNormalNeighbourhood = 2, int smoothingExtNeighbourhood = 6, int smoothingGenerations = 10)
        {
            var currentMap = (HeightLevel[,])this.HeightLevels.Clone();
            var tempMap = (HeightLevel[,])this.HeightLevels.Clone();

            for (var generations = 0; generations < smoothingGenerations; generations++)
            {
                for (var y = 0; y < this.YSize; y++)
                {
                    for (var x = 0; x < this.XSize; x++)
                    {
                        var neighbours = MapHelper.GetNeighbours(x, y, currentMap);
                        var neighboursExt = MapHelper.GetNeighbours(x, y, currentMap, RuleEnums.Neighbourhood.MooreExtended);

                        // Smooth Height2
                        if (currentMap[x, y] == HeightLevel.Height2)
                        {
                            // If there are very few other Height2 nearby, smooth down to Height1
                            if (neighbours[HeightLevel.Height2] <= smoothingNormalNeighbourhood)
                                    tempMap[x, y] = HeightLevel.Height0;
                            else if (neighboursExt[HeightLevel.Height2] <= smoothingExtNeighbourhood) // If there are very few Height2 in the extended Moore neighbourhood, smooth down to Height1
                                tempMap[x, y] = HeightLevel.Height1;
                            else if (neighboursExt[HeightLevel.Height1] <= 0 && (neighboursExt[HeightLevel.Height2] <= 12)) // If Height2 does not have any Height1 nearby, smooth down to Height0
                                tempMap[x, y] = HeightLevel.Height0;
                        }

                        // Smooth Height1
                        if (currentMap[x, y] == HeightLevel.Height1)
                        {
                            // If there are very few other Height1 nearby, smooth down to Height0
                            if (neighbours[HeightLevel.Height1] <= smoothingNormalNeighbourhood)
                                    tempMap[x, y] = HeightLevel.Height0;
                            else if (neighboursExt[HeightLevel.Height1] <= smoothingExtNeighbourhood) // If there are very few Height2 in the extended Moore neighbourhood, smooth down to Height1
                                tempMap[x, y] = HeightLevel.Height0;
                        }

                        // Smooth Height0
                        if (currentMap[x, y] == HeightLevel.Height0)
                        {
                            // If there are very few other Height0 nearby, smooth up to either Height2 or Height1 depending on what there are most of.
                            if (neighbours[HeightLevel.Height0] <= smoothingNormalNeighbourhood)
                            {
                                if (neighbours[HeightLevel.Height2] > neighbours[HeightLevel.Height1])
                                    tempMap[x, y] = HeightLevel.Height2;
                                else
                                    tempMap[x, y] = HeightLevel.Height1;
                            }
                            else if (neighboursExt[HeightLevel.Height0] <= smoothingExtNeighbourhood) // If there are very few Height0 in the extended Moore neighbourhood, smooth up to Height1
                                tempMap[x, y] = HeightLevel.Height1;
                            ////else if (neighboursExt[HeightLevel.Height0] <= neighboursExt[HeightLevel.Height1]) // If there are less Height0 than Height1 in the extended More neighbourhood, smooth up to Height1
                            ////    tempMap[x, y] = HeightLevel.Height1;
                        }
                    }
                }

                currentMap = (HeightLevel[,])tempMap.Clone();
            }

            this.HeightLevels = tempMap;
        }

        /// <summary>
        /// Places cliffs in the map.
        /// </summary>
        public void PlaceCliffs()
        {
            // TODO: Allow for defining how much of the map should be "cliffed"
            var tempMap = (HeightLevel[,])this.HeightLevels.Clone();

            for (var y = 0; y < this.YSize; y++)
            {
                for (var x = 0; x < this.XSize; x++)
                {
                    foreach (var neighbourPosition in MapHelper.GetNeighbourPositions(x, y, this.HeightLevels, RuleEnums.Neighbourhood.VonNeumann))
                    {
                        // Don't do anything if we are at, or are looking at, a cliff or a ramp.
                        if (this.HeightLevels[x, y] == HeightLevel.Cliff
                            || this.HeightLevels[x, y] == HeightLevel.Ramp01
                            || this.HeightLevels[x, y] == HeightLevel.Ramp12
                            || this.HeightLevels[neighbourPosition.Item1, neighbourPosition.Item2] == HeightLevel.Cliff
                            || this.HeightLevels[neighbourPosition.Item1, neighbourPosition.Item2] == HeightLevel.Ramp01
                            || this.HeightLevels[neighbourPosition.Item1, neighbourPosition.Item2] == HeightLevel.Ramp12) continue;

                        // If there are no items on the position being checked, place a cliff.
                        if (this.MapItems[neighbourPosition.Item1, neighbourPosition.Item2] == Item.None 
                            && (int)this.HeightLevels[neighbourPosition.Item1, neighbourPosition.Item2] < (int)this.HeightLevels[x, y])
                            tempMap[neighbourPosition.Item1, neighbourPosition.Item2] = HeightLevel.Cliff;
                        else if (this.MapItems[neighbourPosition.Item1, neighbourPosition.Item2] != Item.None
                            && (int)this.HeightLevels[neighbourPosition.Item1, neighbourPosition.Item2] < (int)this.HeightLevels[x, y]) // If there is an item, place a cliff on x, y
                            tempMap[x, y] = HeightLevel.Cliff;
                    }
                }
            }

            this.HeightLevels = tempMap;
        }

        /// <summary>
        /// Saves the map to a PNG file.
        /// </summary>
        /// <param name="fileNameAddition"> An extra part to add to the file name, when generating maps during testing. </param>
        /// <param name="folder"> Save the map to a special folder in Images/Finished Maps. NOTE: Just give the name of the folder to create/save in, no extra characters such as \. </param>
        public void SaveMapToPngFile(string fileNameAddition = "", string folder = "")
        {
            // The dictionaries and bitmap.
            var heightDic = MapHelper.GetHeightmapImageDictionary();
            var itemDic = MapHelper.GetItemImageDictionary();
            var bm = new Bitmap((this.XSize * MapHelper.SizeOfMapTiles) + 1, (this.YSize * MapHelper.SizeOfMapTiles) + 1);

            // The file names
            var currentTime = string.Empty + DateTime.Now;
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
            var mapItemFile = @"Map_" + currentTime + "_With_Items" + fileNameAddition + ".png";

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

            // Saving map
            bm.Save(Path.Combine(mapDir, mapHeightFile));

            // Adding Items to the map.
            using (var g = Graphics.FromImage(bm))
            {
                for (var y = this.YSize - 1; y >= 0; y--)
                {
                    var drawY = ((bm.Height - 1) - (y * MapHelper.SizeOfMapTiles) + 1) - MapHelper.SizeOfMapTiles;

                    for (var x = 0; x < this.XSize; x++)
                    {
                        var drawX = (x * MapHelper.SizeOfMapTiles) + 1;
                        g.DrawImage(itemDic[this.MapItems[x, y]], drawX, drawY);
                    }
                }
            }

            // Saving map
            bm.Save(Path.Combine(mapDir, mapItemFile));
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
        #endregion
    }
}
