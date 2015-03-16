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

                    // Iterate over every rule
                    foreach (var rule in this.Rules)
                    {
                        // TODO: Should only one rule be applied for every position?
                        if (ruleApplied) break;
                        var neighbours = MapHelper.GetNeighbours(x, y, map, rule.Neighbourhood);

                        if (rule.Deterministic)
                        {
                            // Deterministic rule
                            var applyRule = true;
                            var tempRule = (RuleDeterministic)rule;
                            if (rule.Self != null && map[x, y] != rule.Self) continue;

                            // Check all conditions in the rule. If any of them is not fulfilled, do not apply the rule.
                            foreach (var condition in tempRule.Conditions)
                            {
                                switch (condition.Item3)
                                {
                                    case RuleEnums.Comparison.EqualTo:
                                        if (neighbours[condition.Item2] != condition.Item1) 
                                            applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.GreaterThan:
                                        if (!(neighbours[condition.Item2] > condition.Item1)) 
                                            applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.GreaterThanEqualTo:
                                        if (!(neighbours[condition.Item2] >= condition.Item1)) 
                                            applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.LessThan:
                                        if (!(neighbours[condition.Item2] < condition.Item1)) 
                                            applyRule = false;
                                        break;

                                    case RuleEnums.Comparison.LessThanEqualTo:
                                        if (!(neighbours[condition.Item2] <= condition.Item1)) 
                                            applyRule = false;
                                        break;
                                }
                            }

                            if (!applyRule) continue;
                            tempMap[x, y] = rule.TransformTo;
                            ruleApplied = true;
                        }
                        else
                        {
                            // Probabilistic rule.
                            // Check if the rule spplies to a specific heightlevel and whether the position is that type of heightlevel.
                            if (rule.Self != null && map[x, y] != rule.Self) 
                                continue;

                            var tempRule = (RuleProbabilistic)rule;
                            double probability;
                            var condition = tempRule.Condition;

                            // If there are no neighbours of the type required, the probability is 0%.
                            if (!(neighbours[condition.Item1] > 0))
                                probability = 0d;
                            
                            // ... else if there are more neighbours that required, the probability is 100%
                            else if (neighbours[condition.Item1] >= condition.Item2.GetLength(0)) 
                                probability = 1d;
                            
                            // ... else get the new probability.
                            else 
                                probability = condition.Item2[neighbours[condition.Item1]];

                            /*foreach (var condition in tempRule.Conditions)
                            {
                                if (neighbours[condition.Item1] >= condition.Item2.GetLength(0)) probability = 1d;
                                if (!(neighbours[condition.Item1] > 0))
                                {
                                    applyRule = false;
                                    break;
                                }
                                if (condition.Item2[neighbours[condition.Item1]] > probability) probability = condition.Item2[neighbours[condition.Item1]];
                            }*/

                            if (MapHelper.Random.NextDouble() <= probability) 
                                tempMap[x, y] = rule.TransformTo;

                            ruleApplied = true;
                        }

                        if (tempMap[x, y] == Enums.HeightLevel.Height0 && MapHelper.Random.NextDouble() < 0.01) 
                            tempMap[x, y] = Enums.HeightLevel.Height1;
                    }
                }
            }

            return tempMap;
        }
    }
}
