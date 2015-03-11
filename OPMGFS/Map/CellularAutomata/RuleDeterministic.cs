// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleDeterministic.cs" company="Derps">
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

    /// <summary>
    /// A class that represents a deterministic rule.
    /// </summary>
    public class RuleDeterministic : Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDeterministic"/> class. 
        /// </summary>
        public RuleDeterministic()
        {
            this.Deterministic = true;
            this.Self = null;
            this.Conditions = new List<Tuple<int, Enums.HeightLevel, RuleEnums.Comparison>>();
            this.TransformTo = Enums.HeightLevel.Height0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDeterministic"/> class. 
        /// </summary>
        /// <param name="transformTo"> What to transform to if the rule holds. </param>
        public RuleDeterministic(Enums.HeightLevel transformTo)
            : this()
        {
            this.TransformTo = transformTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDeterministic"/> class. 
        /// </summary>
        /// <param name="transformTo"> What to transform to if the rule holds. </param>
        /// <param name="self"> The height level the position being checked should have, in order for the rule to be applied. </param>
        public RuleDeterministic(Enums.HeightLevel transformTo, Enums.HeightLevel self)
            : this()
        {
            this.Self = self;
            this.TransformTo = transformTo;
        }

        /// <summary>
        /// Gets conditions the rule should fulfill in order to be applied.
        /// </summary>
        public List<Tuple<int, Enums.HeightLevel, RuleEnums.Comparison>> Conditions { get; private set; }

        /// <summary>
        /// Adds a condition to the rule.
        /// </summary>
        /// <param name="number"> The number of neighbours that should satisfy the condition. </param>
        /// <param name="levelToCheckFor"> The level the neighbours should be. </param>
        /// <param name="comparisonType"> How to compare. </param>
        public void AddCondition(int number, Enums.HeightLevel levelToCheckFor, RuleEnums.Comparison comparisonType = RuleEnums.Comparison.GreaterThanEqualTo)
        {
            this.Conditions.Add(new Tuple<int, Enums.HeightLevel, RuleEnums.Comparison>(number, levelToCheckFor, comparisonType));
        }
    }
}
