﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// The random generator.
        /// </summary>
        private readonly Random random;

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
        /// <param name="xSize"> The x size of the map. </param>
        /// <param name="ySize"> The y size of the map. </param>
        /// <param name="half"> The half of the map to work on. </param>
        /// <param name="oddsOfHeight1"> The odds of a tile being changed to height 1. </param>
        /// <param name="oddsOfHeight2"> The odds of a tile being changed to height 2. </param>
        /// <param name="groupPoints"> The number of points where terrain should be group during the initial seeding. </param>
        /// <param name="generateHeight2"> Determines if the cellular automata should generate height2 or not. </param>
        /// <param name="r"> The random to use. If null, will use the MapHelper Random. </param>
        public CellularAutomata(int xSize, int ySize, Enums.Half half, double oddsOfHeight1 = 0.50, double oddsOfHeight2 = 0.25, int groupPoints = 0, bool generateHeight2 = true, Random r = null)
        {
            this.Map = new Enums.HeightLevel[xSize, ySize];

            this.random = r ?? MapHelper.Random;

            this.XSize = xSize;
            this.YSize = ySize;

            // Figure out which part of the map that should be looked at.
            // Make sure we work on a bit more than half the map, in order to avoid that the edge along the middle does not have
            // "empty" neighbours from the beginning.
            this.caXStart = (half == Enums.Half.Right) ? (this.XSize / 2) - (int)(this.XSize * 0.1) : 0;
            this.caXEnd = (half == Enums.Half.Left) ? (this.XSize / 2) + (int)(this.XSize * 0.1) : this.XSize;
            this.caYStart = (half == Enums.Half.Top) ? (this.YSize / 2) - (int)(this.YSize * 0.1) : 0;
            this.caYEnd = (half == Enums.Half.Bottom) ? (this.YSize / 2) + (int)(this.YSize * 0.1) : this.YSize;

            // Create points around which the terrain should be grouped
            var groupList = new List<Position>();
            for (var i = 0; i < groupPoints; i++)
            {
                var x = this.random.Next(this.caXStart, this.caXEnd);
                var y = this.random.Next(this.caYStart, this.caYEnd);
                groupList.Add(new Position(x, y));
            }

            for (var y = this.caYStart; y < this.caYEnd; y++)
            {
                for (var x = this.caXStart; x < this.caXEnd; x++)
                {
                    var odds = this.random.NextDouble();

                    if (groupList.Count > 0)
                    {
                        // TODO: How do we want "closest to" to work? Look in a square or look in a circle?
                        var closest = (double)MapHelper.ClosestTo(new Position(x, y), groupList, 8);
                        if (closest <= 8) odds = odds - ((8d - closest) / 20d);
                    }

                    if (generateHeight2 && odds < oddsOfHeight2) this.Map[x, y] = Enums.HeightLevel.Height2;
                    else if (odds < oddsOfHeight1) this.Map[x, y] = Enums.HeightLevel.Height1;
                }
            }

            this.ruleSet = new Ruleset();
            this.LoadBasicRuleset();
        }

        public CellularAutomata(int xSize, int ySize, Enums.Half half, Enums.HeightLevel[,] map, Random r = null)
        {
            this.Map = (Enums.HeightLevel[,])map.Clone();

            this.random = r ?? MapHelper.Random;

            this.XSize = xSize;
            this.YSize = ySize;

            // Figure out which part of the map that should be looked at.
            // Make sure we work on a bit more than half the map, in order to avoid that the edge along the middle does not have
            // "empty" neighbours from the beginning.
            this.caXStart = (half == Enums.Half.Right) ? (this.XSize / 2) - (int)(this.XSize * 0.1) : 0;
            this.caXEnd = (half == Enums.Half.Left) ? (this.XSize / 2) + (int)(this.XSize * 0.1) : this.XSize;
            this.caYStart = (half == Enums.Half.Top) ? (this.YSize / 2) - (int)(this.YSize * 0.1) : 0;
            this.caYEnd = (half == Enums.Half.Bottom) ? (this.YSize / 2) + (int)(this.YSize * 0.1) : this.YSize;

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
        private int XSize { get; set; }

        /// <summary>
        /// Gets or sets the y size.
        /// </summary>
        private int YSize { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Runs a given amount of generations on the cellular automata
        /// </summary>
        /// <param name="generations"> The number of generations to run. </param>
        /// <param name="generateHeight2ThroughRules"> Determines if height2 should be generated through rules or not. </param>
        public void RunGenerations(int generations = 10, bool generateHeight2ThroughRules = true)
        {
            for (var generation = 0; generation < generations; generation++)
                this.NextGeneration(generateHeight2ThroughRules);
        }

        /// <summary>
        /// Runs the next generation on the cellular automata
        /// </summary>
        /// <param name="generateHeight2ThroughRules"> Determines if height2 should be generated through rules or not. </param>
        public void NextGeneration(bool generateHeight2ThroughRules = true)
        {
            this.Map = this.ruleSet.NextGeneration(this.Map, this.caXStart, this.caXEnd, this.caYStart, this.caYEnd, generateHeight2ThroughRules, this.random);
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
        /// Adds impassable terrain at random positions in the map.
        /// </summary>
        /// <param name="drops"> The number of impassable terrain drops. </param>
        /// <param name="radius"> The radius of each drop zone. </param>
        public void AddImpassableTerrain(int drops, int radius)
        {
            // TODO: Change from circle drop to more natural method.
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
                var startX = this.random.Next(this.caXStart, this.caXEnd);
                var startY = this.random.Next(this.caYStart, this.caYEnd);

                Console.WriteLine("Placing shit around " + startX + " - " + startY);

                foreach (var moveX in moves)
                {
                    foreach (var moveY in moves)
                    {
                        if (moveX == 0 && moveY == 0) continue;
                        if (moveX + moveY > 5) continue;

                        var posToCheck = new Position(startX + moveX, startY + moveY);
                        if (!MapHelper.WithinMapBounds(posToCheck, this.XSize, this.YSize)) continue;

                        if (this.random.NextDouble() <= 0.66)
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
                tempMap = tempRuleSet.NextGeneration(tempMap, this.caXStart, this.caXEnd, this.caYStart, this.caYEnd, false, this.random);
            }

            this.Map = tempMap;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the basic rulesets.
        /// </summary>
        private void LoadBasicRuleset()
        {
            // ITODO: (DONE) May want to use ruleset from Program.cs. Thsi one seems shit.
            var ruleExtBasicHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2)
            {
                Neighbourhood =
                    RuleEnums.Neighbourhood
                    .MooreExtended
            };
            ruleExtBasicHeight2.AddCondition(18, Enums.HeightLevel.Height1);

            var ruleExtBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height0)
            {
                Neighbourhood =
                    RuleEnums.Neighbourhood
                    .MooreExtended
            };
            ruleExtBasicHeight1.AddCondition(18, Enums.HeightLevel.Height1);

            var ruleExtAdvHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2)
            {
                Neighbourhood =
                    RuleEnums.Neighbourhood
                    .MooreExtended
            };
            ruleExtAdvHeight2.AddCondition(18, Enums.HeightLevel.Height2);

            var ruleBasicHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2);
            ruleBasicHeight2.AddCondition(5, Enums.HeightLevel.Height2);

            var ruleBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleBasicHeight1.AddCondition(5, Enums.HeightLevel.Height1);

            var ruleAdvHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height0);
            ruleAdvHeight1.AddCondition(3, Enums.HeightLevel.Height1);
            ruleAdvHeight1.AddCondition(3, Enums.HeightLevel.Height2);

            var ruleAdvHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2, Enums.HeightLevel.Height1);
            ruleAdvHeight2.AddCondition(3, Enums.HeightLevel.Height1);
            ruleAdvHeight2.AddCondition(3, Enums.HeightLevel.Height2);

            var ruleRemoveHeight0 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height0);
            ruleRemoveHeight0.AddCondition(2, Enums.HeightLevel.Height0, RuleEnums.Comparison.LessThanEqualTo);

            var ruleList = new List<Rule>
                   {
                       ruleExtBasicHeight2, ruleExtBasicHeight1, ruleExtAdvHeight2, ruleBasicHeight2, ruleBasicHeight1, ruleAdvHeight1, ruleRemoveHeight0
                   };

            this.ruleSet = new Ruleset(ruleList);
        }

        /// <summary>
        /// Loads the old rulesets.
        /// </summary>
        private void LoadOldRuleset()
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
            ////ruleList.Add(ruleSmoothLandscape);
            ruleList.Add(ruleSimpleHeight2);
            ruleList.Add(ruleSimpleHeight2Again);
            ruleList.Add(ruleSimpleHeight1);
            ruleList.Add(ruleShrinkHeight2);

            this.ruleSet = new Ruleset(ruleList);
        }
        #endregion
    }
}
