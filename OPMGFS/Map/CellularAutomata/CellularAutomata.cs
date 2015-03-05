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
    using System.Collections.Generic;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// TODO: Separate place cliff function
    /// The class that contains the cellular automata.
    /// </summary>
    /// <typeparam name="T"> The ruleset the cellular automata should use. </typeparam>
    public class CellularAutomata<T> where T : Ruleset
    {
        #region Fields
        /// <summary>
        /// The start position for Y when working on the map.
        /// </summary>
        private readonly int caYStart;

        /// <summary>
        /// The end position for Y when working on the map.
        /// </summary>
        private readonly int caYEnd;

        /// <summary>
        /// The start position for X when working on the map.
        /// </summary>
        private readonly int caXStart;

        /// <summary>
        /// The end position for X when working on the map.
        /// </summary>
        private readonly int caXEnd;

        /// <summary>
        /// The ruleset used by the cellular automata.
        /// </summary>
        private readonly T ruleSet;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CellularAutomata{T}"/> class. 
        /// </summary>
        /// <param name="xSize"> The x size of the map. </param>
        /// <param name="ySize"> The y size of the map. </param>
        /// <param name="half"> The half of the map to work on. </param>
        /// <param name="generations"> The number of generations to run. </param>
        /// <param name="oddsOfHeight1"> The odds of a tile being changed to height 1. </param>
        /// <param name="oddsOfHeight2"> The odds of a tile being changed to height 2. </param>
        public CellularAutomata(int xSize, int ySize, Enums.Half half, int generations = 5, double oddsOfHeight1 = 0.50, double oddsOfHeight2 = 0.25)
        {
            this.Map = new Enums.HeightLevel[xSize, ySize];

            this.Random = new Random();

            this.XSize = xSize;
            this.YSize = ySize;

            // Figures out which part of the map that should be looked at.
            this.caXStart = (half == Enums.Half.Right) ? this.XSize / 2 : 0;
            this.caXEnd = (half == Enums.Half.Left) ? this.XSize / 2 : this.XSize;
            this.caYStart = (half == Enums.Half.Top) ? this.YSize / 2 : 0;
            this.caYEnd = (half == Enums.Half.Bottom) ? this.YSize / 2 : this.YSize;

            for (var y = this.caYStart; y < this.caYEnd; y++)
            {
                for (var x = this.caXStart; x < this.caXEnd; x++)
                {
                    if (this.Random.NextDouble() < oddsOfHeight1) this.Map[x, y] = Enums.HeightLevel.Height1;
                    if (this.Random.NextDouble() < oddsOfHeight2) this.Map[x, y] = Enums.HeightLevel.Height2;
                }
            }

            this.ruleSet = (T)Activator.CreateInstance(typeof(T));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the map height levels that are being "evolved".
        /// </summary>
        public Enums.HeightLevel[,] Map { get; protected set; }

        /// <summary>
        /// Gets or sets the random generator.
        /// </summary>
        protected Random Random { get; set; }

        /// <summary>
        /// Gets or sets the x size.
        /// </summary>
        private int XSize { get; set; }

        /// <summary>
        /// Gets or sets the y size.
        /// </summary>
        private int YSize { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Runs the next generation on the cellular automata
        /// </summary>
        public void NextGeneration()
        {
            this.Map = this.ruleSet.NextGeneration(this.Map, this.caXStart, this.caXEnd, this.caYStart, this.caYEnd);
        }

        /// <summary>
        /// Places cliffs in the map.
        /// </summary>
        public void PlaceCliffs()
        {
            // TODO: Check if logic is correct.
            var tempMap = (Enums.HeightLevel[,])this.Map.Clone();

            for (var y = this.caYStart; y < this.caYEnd; y++)
            {
                for (var x = this.caXStart; x < this.caXEnd; x++)
                {
                    foreach (var neighbourPosition in this.getNeighbourPositions(x, y))
                    {
                        if (this.Map[x, y] == Enums.HeightLevel.Cliff
                            || this.Map[neighbourPosition.Item1, neighbourPosition.Item2] == Enums.HeightLevel.Cliff) continue;

                        if ((int)this.Map[neighbourPosition.Item1, neighbourPosition.Item2] < (int)this.Map[x, y])
                            tempMap[neighbourPosition.Item1, neighbourPosition.Item2] = Enums.HeightLevel.Cliff;
                    }
                }
            }

            this.Map = tempMap;
        }
        #endregion

        #region Private Methods

        private List<Position> getNeighbourPositions(int x, int y)
        {
            var list = new List<Position>();
            var moves = new[] { -1, 0, 1 };

            foreach (var moveX in moves)
            {
                foreach (var moveY in moves)
                {
                    if (moveX == 0 && moveY == 0) continue;
                    if (moveX != 0 && moveY != 0) continue;

                    var posToCheck = new Position(x + moveX, y + moveY);
                    if (!this.WithinMapBounds(posToCheck)) continue;
                    list.Add(posToCheck);
                }
            }

            return list;
        }

        /// <summary>
        /// Checks if a position is within the bounds of the map.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>True if within the bounds of the map; false otherwise.</returns>
        private bool WithinMapBounds(Position position)
        {
            if (position.Item1 < 0 || position.Item1 >= this.XSize) return false;
            if (position.Item2 < 0 || position.Item2 >= this.YSize) return false;
            return true;
        }
        #endregion
    }
}
