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
    /// The class that contains the cellular automata.
    /// </summary>
    public class CellularAutomata
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
        private Ruleset ruleSet;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CellularAutomata"/> class. 
        /// </summary>
        /// <param name="sizeX"> The x size of the map. </param>
        /// <param name="sizeY"> The y size of the map. </param>
        /// <param name="half"> The half of the map to work on. </param>
        /// <param name="oddsOfHeight1"> The odds of a tile being changed to height 1. </param>
        /// <param name="oddsOfHeight2"> The odds of a tile being changed to height 2. </param>
        public CellularAutomata(int sizeX, int sizeY, Enums.Half half, double oddsOfHeight1 = 0.50, double oddsOfHeight2 = 0.25)
        {
            this.Map = new Enums.HeightLevel[sizeX, sizeY];

            this.SizeX = sizeX;
            this.SizeY = sizeY;

            // Figures out which part of the map that should be looked at.
            this.caXStart = (half == Enums.Half.Right) ? this.SizeX / 2 : 0;
            this.caXEnd = (half == Enums.Half.Left) ? this.SizeX / 2 : this.SizeX;
            this.caYStart = (half == Enums.Half.Top) ? this.SizeY / 2 : 0;
            this.caYEnd = (half == Enums.Half.Bottom) ? this.SizeY / 2 : this.SizeY;

            for (var y = this.caYStart; y < this.caYEnd; y++)
            {
                for (var x = this.caXStart; x < this.caXEnd; x++)
                {
                    if (MapHelper.Random.NextDouble() < oddsOfHeight1) this.Map[x, y] = Enums.HeightLevel.Height1;
                    if (MapHelper.Random.NextDouble() < oddsOfHeight2) this.Map[x, y] = Enums.HeightLevel.Height2;
                }
            }

            this.ruleSet = new Ruleset();
            this.LoadBasicRuleset();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the map height levels that are being "evolved".
        /// </summary>
        public Enums.HeightLevel[,] Map { get; protected set; }

        /// <summary>
        /// Gets or sets the x size.
        /// </summary>
        private int SizeX { get; set; }

        /// <summary>
        /// Gets or sets the y size.
        /// </summary>
        private int SizeY { get; set; }
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
        /// Gives the cellular automata a new ruleset to work with.
        /// Overwrites old ruleset.
        /// </summary>
        /// <param name="newRuleSet"> The new ruleset. </param>
        public void SetRuleset(List<Rule> newRuleSet)
        {
            this.ruleSet = new Ruleset(newRuleSet);
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
                    foreach (var neighbourPosition in MapHelper.GetNeighbourPositions(x, y, this.Map))
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

        /// <summary>
        /// Adds impassable terrain at random positions in the map.
        /// </summary>
        /// <param name="drops"> The number of impassable terrain drops. </param>
        /// <param name="radius"> The radius of each drop zone. </param>
        public void AddImpassableTerrain(int drops, int radius)
        {
            // TODO: Still needs work.
            var tempMap = (Enums.HeightLevel[,])this.Map.Clone();
            var moves = new int[(radius * 2) + 1];
            var movesPosition = 1;

            // Get the moves to make to reach the entire area around the drop zone.
            moves[0] = 0;
            for (int r = 1; r <= radius; r++)
            {
                moves[movesPosition] = r;
                moves[movesPosition + 1] = -r;

                movesPosition += 2;
            }

            // Randomly choses an area and drops impassable terrain in it.
            for (int drop = 0; drop < drops; drop++)
            {
                var startX = MapHelper.Random.Next(this.caXStart, this.caXEnd);
                var startY = MapHelper.Random.Next(this.caYStart, this.caYEnd);

                Console.WriteLine("Placing shit around " + startX + " - " + startY);

                foreach (var moveX in moves)
                {
                    foreach (var moveY in moves)
                    {
                        if (moveX == 0 && moveY == 0) continue;
                        if (moveX + moveY > 5) continue;

                        var posToCheck = new Position(startX + moveX, startY + moveY);
                        if (!MapHelper.WithinMapBounds(posToCheck, this.SizeX, this.SizeY)) continue;

                        if (MapHelper.Random.NextDouble() <= 0.66)
                            tempMap[posToCheck.Item1, posToCheck.Item2] = Enums.HeightLevel.Impassable;
                    }
                }
            }

            var ruleSmoothImpassable = new RuleDeterministic(Enums.HeightLevel.Impassable);
            ruleSmoothImpassable.AddCondition(4, Enums.HeightLevel.Impassable);
            
            var ruleList = new List<Rule> { ruleSmoothImpassable };

            var tempRuleSet = new Ruleset(ruleList);

            for (int g = 0; g < 10; g++)
            {
                tempMap = tempRuleSet.NextGeneration(tempMap, this.caXStart, this.caXEnd, this.caYStart, this.caYEnd);
            }

            this.Map = tempMap;
        }

        #endregion

        #region Private Methods

        private void LoadBasicRuleset()
        {
            var ruleList = new List<Rule>();
            var ruleNoInterestingNeighbours = new RuleDeterministic(Enums.HeightLevel.Height0);
            ruleNoInterestingNeighbours.AddCondition(8, Enums.HeightLevel.Height0);

            var ruleSmoothLandscape = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleSmoothLandscape.AddCondition(3, Enums.HeightLevel.Height2);
            ruleSmoothLandscape.AddCondition(3, Enums.HeightLevel.Height0);

            var ruleSimpleHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleSimpleHeight1.AddCondition(5, Enums.HeightLevel.Height1);

            var ruleSimpleHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2, Enums.HeightLevel.Height1);
            ruleSimpleHeight2.AddCondition(5, Enums.HeightLevel.Height2);

            var ruleSimpleHeight2Again = new RuleDeterministic(Enums.HeightLevel.Height2, Enums.HeightLevel.Height1);
            ruleSimpleHeight2Again.AddCondition(5, Enums.HeightLevel.Height1);
            ruleSimpleHeight2Again.AddCondition(2, Enums.HeightLevel.Height2);

            var ruleShrinkHeight2 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height2);
            ruleShrinkHeight2.AddCondition(0, Enums.HeightLevel.Height2, RuleEnums.Comparison.LessThanEqualTo);

            ruleList.Add(ruleNoInterestingNeighbours);
            //ruleList.Add(ruleSmoothLandscape);
            ruleList.Add(ruleSimpleHeight2);
            ruleList.Add(ruleSimpleHeight2Again);
            ruleList.Add(ruleSimpleHeight1);
            ruleList.Add(ruleShrinkHeight2);

            this.ruleSet = new Ruleset(ruleList);
        }
        #endregion
    }
}
