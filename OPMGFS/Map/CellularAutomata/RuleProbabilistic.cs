namespace OPMGFS.Map.CellularAutomata
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class that represents a probabilistic rule.
    /// </summary>
    public class RuleProbabilistic : Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProbabilistic"/> class. 
        /// </summary>
        public RuleProbabilistic()
        {
            this.Deterministic = false;
            this.Self = null;
            this.Condition = new Tuple<Enums.HeightLevel, double[]>(Enums.HeightLevel.Impassable, null);
            this.Conditions = new List<Tuple<Enums.HeightLevel, double[]>>();
            this.TransformTo = Enums.HeightLevel.Height0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProbabilistic"/> class. 
        /// </summary>
        /// <param name="transformTo"> What to transform to if the rule holds. </param>
        public RuleProbabilistic(Enums.HeightLevel transformTo)
            : this()
        {
            this.TransformTo = transformTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleProbabilistic"/> class. 
        /// </summary>
        /// <param name="transformTo"> What to transform to if the rule holds. </param>
        /// <param name="self"> The height level the position being checked should have, in order for the rule to be applied. </param>
        public RuleProbabilistic(Enums.HeightLevel transformTo, Enums.HeightLevel self)
            : this()
        {
            this.Self = self;
            this.TransformTo = transformTo;
        }

        /// <summary>
        /// Gets the condition the rule should fulfill in order to be applied.
        /// </summary>
        public Tuple<Enums.HeightLevel, double[]> Condition { get; private set; }

        /// <summary>
        /// Gets the conditions the rule should fulfill in order to be applied.
        /// </summary>
        public List<Tuple<Enums.HeightLevel, double[]>> Conditions { get; private set; }

        /// <summary>
        /// Adds a condition to the rule.
        /// </summary>
        /// <param name="levelToCheckFor"> The level the neighbours should be. </param>
        /// <param name="probabilities"> How to compare. </param>
        public void AddCondition(Enums.HeightLevel levelToCheckFor, double[] probabilities)
        {
            this.Conditions.Add(
                probabilities == null
                    ? new Tuple<Enums.HeightLevel, double[]>(levelToCheckFor, new[] { 1d })
                    : new Tuple<Enums.HeightLevel, double[]>(levelToCheckFor, probabilities));
        }

        /// <summary>
        /// Adds a condition to the rule.
        /// </summary>
        /// <param name="levelToCheckFor"> The level the neighbours should be. </param>
        /// <param name="probabilities"> How to compare. </param>
        public void SetCondition(Enums.HeightLevel levelToCheckFor, double[] probabilities)
        {
            this.Conditions.Add(
                probabilities == null
                    ? this.Condition = new Tuple<Enums.HeightLevel, double[]>(levelToCheckFor, new[] { 1d })
                    : this.Condition = new Tuple<Enums.HeightLevel, double[]>(levelToCheckFor, probabilities));
        }
    }
}
