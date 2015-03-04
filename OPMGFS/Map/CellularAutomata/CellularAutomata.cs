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

        private int YSize { get; set; }

        private int XSize { get; set; }

        private int caYStart, caYEnd, caXStart, caXEnd;

        protected Random Random { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ySize"></param>
        /// <param name="xSize"></param>
        /// <param name="oddsOfHeight1"></param>
        /// <param name="oddsOfHeight2"></param>
        public CellularAutomata(int xSize, int ySize, Enums.Half half, double oddsOfHeight1 = 0.50, double oddsOfHeight2 = 0.25)
        {
            this.Map = new Enums.HeightLevel[xSize, ySize];

            this.Random = new Random();

            this.XSize = xSize;
            this.YSize = ySize;

            // Figures out which part of the map that should be looked at.
            caXStart = (half == Enums.Half.Right) ? this.XSize / 2 : 0;
            caXEnd = (half == Enums.Half.Left) ? this.XSize / 2 : this.XSize;
            caYStart = (half == Enums.Half.Top) ? this.YSize / 2 : 0;
            caYEnd = (half == Enums.Half.Bottom) ? this.YSize / 2 : this.YSize;

            for (int y = this.caYStart; y < this.caYEnd; y++)
            {
                for (int x = this.caXStart; x < this.caXEnd; x++)
                {
                    if (this.Random.NextDouble() < oddsOfHeight1) this.Map[x, y] = Enums.HeightLevel.Height1;
                    if (this.Random.NextDouble() < oddsOfHeight2) this.Map[x, y] = Enums.HeightLevel.Height2;
                }
            }
        }
    }
}
