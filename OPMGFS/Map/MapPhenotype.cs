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
    using System.Text;

    using Half = Enums.Half;
    using HeightLevel = Enums.HeightLevel;
    using Item = Enums.Item;

    /// <summary>
    /// The map phenotype.
    /// </summary>
    public class MapPhenotype
    {
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
            var yStart = (half == Half.Bottom) ? this.YSize / 2 : 0;
            var yEnd = (half == Half.Top) ? this.YSize / 2 : this.YSize;

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
    }
}
