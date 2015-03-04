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
        /// <param name="height"> The height of the map. </param>
        /// <param name="width"> The width of the map. </param>
        public MapPhenotype(int height, int width)
        {
            this.HeightLevels = new HeightLevel[height, width];
            this.MapItems = new Item[height, width];

            this.Height = this.HeightLevels.GetLength(0);
            this.Width = this.HeightLevels.GetLength(1);
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

            this.Height = this.HeightLevels.GetLength(0);
            this.Width = this.HeightLevels.GetLength(1);
        }

        /// <summary>
        /// Gets the height of the map.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width of the map.
        /// </summary>
        public int Width { get; private set; }

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
            var startHeight = (half == Half.Bottom) ? this.Height / 2 : 0;
            var endHeight = (half == Half.Top) ? this.Height / 2 : this.Height;
            var startWidth = (half == Half.Right) ? this.Width / 2 : 0;
            var endWidth = (half == Half.Left) ? this.Width / 2 : this.Width;

            for (var tempHeight = startHeight; tempHeight < endHeight; tempHeight++)
            {
                var otherHeight = tempHeight;

                // If we mirror top or bottom or turn the map, find the height to copy to.
                if ((function == Enums.MapFunction.Mirror && (half == Half.Top || half == Half.Bottom)) 
                    || function == Enums.MapFunction.Turn)
                    otherHeight = this.Height - tempHeight - 1;

                for (var tempWidth = startWidth; tempWidth < endWidth; tempWidth++)
                {
                    var otherWidth = tempWidth;

                    // If we mirror left or right or turn the map, find the width to copy to.
                    if ((function == Enums.MapFunction.Mirror && (half == Half.Left || half == Half.Right))
                        || function == Enums.MapFunction.Turn)
                        otherWidth = this.Width - tempWidth - 1;

                    switch (half)
                    {
                        case Half.Top:
                            newHeightLevels[otherHeight, otherWidth] = this.HeightLevels[tempHeight, tempWidth];
                            newMapItems[otherHeight, otherWidth] = this.MapItems[tempHeight, tempWidth];
                            break;

                        case Half.Bottom:
                            newHeightLevels[otherHeight, otherWidth] = this.HeightLevels[tempHeight, tempWidth];
                            newMapItems[otherHeight, otherWidth] = this.MapItems[tempHeight, tempWidth];
                            break;

                        case Half.Left:
                            newHeightLevels[otherHeight, otherWidth] = this.HeightLevels[tempHeight, tempWidth];
                            newMapItems[otherHeight, otherWidth] = this.MapItems[tempHeight, tempWidth];
                            break;

                        case Half.Right:
                            newHeightLevels[otherHeight, otherWidth] = this.HeightLevels[tempHeight, tempWidth];
                            newMapItems[otherHeight, otherWidth] = this.MapItems[tempHeight, tempWidth];
                            break;
                    }
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
            for (var i = 0; i < this.Height + 2; i++)
            {

                heightLevelBuilder.Append("-");
                itemBuilder.Append("-");
            }

            heightLevelBuilder.AppendLine();
            itemBuilder.AppendLine();
            
            for (var i = 0; i < this.Height; i++)
            {
                // Left border
                heightLevelBuilder.Append("|");
                itemBuilder.Append("|");

                // Actual map values
                for (int j = 0; j < this.Width; j++)
                {
                    heightLevelBuilder.Append(Enums.GetCharValue(this.HeightLevels[i, j]));
                    itemBuilder.Append(Enums.GetCharValue(this.MapItems[i, j]));
                }

                // Right border
                heightLevelBuilder.Append("| " + i);
                itemBuilder.Append("|");

                heightLevelBuilder.AppendLine();
                itemBuilder.AppendLine();
            }
            
            // Bottom border
            for (var i = 0; i < this.Height + 2; i++)
            {
                heightLevelBuilder.Append("-");
                itemBuilder.Append("-");
            }

            mapHeightLevels = heightLevelBuilder.ToString();
            mapItems = itemBuilder.ToString();
        }
    }
}
