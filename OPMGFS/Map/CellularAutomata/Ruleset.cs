// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ruleset.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPhenotype type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map.CellularAutomata
{
    using System;
    using System.Collections.Generic;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// The abstract ruleset.
    /// </summary>
    public class Ruleset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ruleset"/> class with an empty list of rules. 
        /// </summary>
        public Ruleset()
        {
            this.Rules = new List<Rule>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ruleset"/> class. 
        /// </summary>
        /// <param name="cellularAutomataRules"> The list of rules for the ruleset to use. </param>
        public Ruleset(IEnumerable<Rule> cellularAutomataRules)
            : this()
        {
            this.Rules.AddRange(cellularAutomataRules);
        }

        /// <summary>
        /// Gets or sets the rules used by the ruleset.
        /// </summary>
        private List<Rule> Rules { get; set; }

        /// <summary>
        /// Gets the next generation following the ruleset.
        /// </summary>
        /// <param name="map"> The map to progress. </param>
        /// <param name="caXStart"> The x start position of the cellular automata. </param>
        /// <param name="caXEnd"> The x end position of the cellular automata. </param>
        /// <param name="caYStart"> The y start position of the cellular automata. </param>
        /// <param name="caYEnd"> The y end position of the cellular automata. </param>
        /// <returns> A map representing the next generation. </returns>
        public Enums.HeightLevel[,] NextGeneration(
            Enums.HeightLevel[,] map,
            int caXStart,
            int caXEnd,
            int caYStart,
            int caYEnd)
        {
            var tempMap = (Enums.HeightLevel[,])map.Clone();

            // Iterate over every row and column
            for (var y = caYStart; y < caYEnd; y++)
            {
                for (var x = caXStart; x < caXEnd; x++)
                {
                    var ruleApplied = false;
                    var neighbours = this.GetNeighbours(x, y, map);

                    // Iterate over every rule
                    foreach (var rule in this.Rules)
                    {
                        // TODO: Should only one rule be applied for every position?
                        if (ruleApplied) break;

                        // If the rule is deterministic, check if all its conditions are fulfilled
                        if (rule.Deterministic)
                        {
                            var applyRule = true;
                            var tempRule = (RuleDeterministic)rule;
                            if (rule.Self != null && map[x, y] != rule.Self) continue;

                            foreach (var condition in tempRule.Conditions)
                            {
                                switch (condition.Item3)
                                {
                                    case RuleEnums.Comparison.EqualTo:
                                        if (neighbours[condition.Item2] != condition.Item1) applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.GreaterThan:
                                        if (!(neighbours[condition.Item2] > condition.Item1)) applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.GreaterThanEqualTo:
                                        if (!(neighbours[condition.Item2] >= condition.Item1)) applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.LessThan:
                                        if (!(neighbours[condition.Item2] < condition.Item1)) applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.LessThanEqualTo:
                                        if (!(neighbours[condition.Item2] <= condition.Item1)) applyRule = false;
                                        break;
                                }
                            }

                            if (!applyRule) continue;
                            tempMap[x, y] = rule.TransformTo;
                            ruleApplied = true;
                        }
                        else
                        {
                            // If the rule is probabilistic, figure out if a change should happen.
                            // TODO: Do probabilistic stuff here.
                        }
                    }
                }
            }

            return tempMap;
        }

        /// <summary>
        /// Gets the number of different neighbours surrounding the given position.
        /// </summary>
        /// <param name="x"> The x-coordinate to check neighbours for. </param>
        /// <param name="y"> The y-coordinate to check neighbours for. </param>
        /// <param name="map"> The map. </param>
        /// <returns> A dictionary containing the neighbours and the count of them. </returns>
        private Dictionary<Enums.HeightLevel, int> GetNeighbours(int x, int y, Enums.HeightLevel[,] map)
        {
            var neighbours = new Dictionary<Enums.HeightLevel, int>();
            var moves = new[] { -1, 0, 1 };
            var sizeX = map.GetLength(0);
            var sizeY = map.GetLength(1);

            // Ensure that all keys are in the dictionary
            neighbours[Enums.HeightLevel.Height0] = 0;
            neighbours[Enums.HeightLevel.Height1] = 0;
            neighbours[Enums.HeightLevel.Height2] = 0;
            neighbours[Enums.HeightLevel.Impassable] = 0;

            foreach (var moveX in moves)
            {
                foreach (var moveY in moves)
                {
                    if (moveX == 0 && moveY == 0) continue;

                    var posToCheck = new Position(x + moveX, y + moveY);
                    if (!this.WithinMapBounds(posToCheck, sizeX, sizeY)) continue;
                    neighbours[map[posToCheck.Item1, posToCheck.Item2]]++;
                }
            }

            return neighbours;
        }

        /*
        /// <summary>
        /// Counts the number of neighbours to (x, y) of the the given type.
        /// </summary>
        /// <param name="x"> The x-coordinate to check neighbours for. </param>
        /// <param name="y"> The y-coordinate to check neighbours for. </param>
        /// <param name="map"> The map. </param>
        /// <param name="type"> The type to count for. </param>
        /// <returns> The number of neighbours of the given type. </returns>
        private int NumberOfNeighboursOfType(int x, int y, Enums.HeightLevel[,] map, Enums.HeightLevel type)
        {
            var moves = new[] { -1, 0, 1 };
            var neighbours = 0;
            var sizeX = map.GetLength(0);
            var sizeY = map.GetLength(1);

            foreach (var moveX in moves)
            {
                foreach (var moveY in moves)
                {
                    if (moveX == 0 && moveY == 0) continue;

                    var posToCheck = new Position(x + moveX, y + moveY);
                    if (!this.WithinMapBounds(posToCheck, sizeX, sizeY)) continue;
                    if (map[posToCheck.Item1, posToCheck.Item2] == type) neighbours++;
                }
            }

            return neighbours;
        }*/

        /// <summary>
        /// Checks if a position is within the bounds of the map.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <param name="sizeX">The size of the map on the x-axis.</param>
        /// <param name="sizeY">The size of the map on the y-axis.</param>
        /// <returns>True if within the bounds of the map; false otherwise.</returns>
        private bool WithinMapBounds(Position position, int sizeX, int sizeY)
        {
            if (position.Item1 < 0 || position.Item1 >= sizeX) return false;
            if (position.Item2 < 0 || position.Item2 >= sizeY) return false;
            return true;
        }
    }
}
