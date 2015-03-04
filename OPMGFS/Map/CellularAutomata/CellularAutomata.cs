// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CellularAutomata.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the CellularAutomata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map.CellularAutomata
{
    using System;

    /// <summary>
    /// TODO: Seperate place cliff function
    /// </summary>
    public class CellularAutomata
    {
        public Enums.HeightLevel[,] Map { get; protected set; }

        private int Height { get; set; }

        private int Width { get; set; }

        private int caStartHeight, caEndHeight, caStartWidth, caEndWidth;

        protected Random Random { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="oddsOfHeight1"></param>
        /// <param name="oddsOfHeight2"></param>
        public CellularAutomata(int height, int width, Enums.Half half, double oddsOfHeight1 = 0.50, double oddsOfHeight2 = 0.25)
        {
            this.Map = new Enums.HeightLevel[height, width];

            this.Random = new Random();

            this.Height = height;
            this.Width = width;

            // Figures out which part of the map that should be looked at.
            this.caStartHeight = (half == Enums.Half.Bottom) ? (this.Height / 2) - 2 : 0;
            this.caEndHeight = (half == Enums.Half.Top) ? (this.Height / 2) + 2 : this.Height;
            this.caStartWidth = (half == Enums.Half.Right) ? (this.Width / 2) - 2 : 0;
            this.caEndWidth = (half == Enums.Half.Left) ? (this.Width / 2) + 2 : this.Width;

            for (int tempHeight = this.caStartHeight; tempHeight < this.caEndHeight; tempHeight++)
            {
                for (int tempWidth = this.caStartHeight; tempWidth < this.caEndWidth; tempWidth++)
                {
                    if (this.Random.NextDouble() < oddsOfHeight1) this.Map[tempHeight, tempWidth] = Enums.HeightLevel.Height1;
                    if (this.Random.NextDouble() < oddsOfHeight2) this.Map[tempHeight, tempWidth] = Enums.HeightLevel.Height2;
                }
            }
        }
    }
}
