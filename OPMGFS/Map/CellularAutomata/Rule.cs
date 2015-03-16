// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rule.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the CellularAutomata type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map.CellularAutomata
{
    /// <summary>
    /// A class that specifies a rule for the cellular automata.
    /// </summary>
    public abstract class Rule
    {
        /// <summary>
        /// Gets or sets a value indicating whether the rule is deterministic or not.
        /// </summary>
        public bool Deterministic { get; protected set; }

        /// <summary>
        /// Gets or sets a value that tells what type of height level to apply the rule to.
        /// null means that it applies to all types.
        /// </summary>
        public Enums.HeightLevel? Self { get; set; }

        /// <summary>
        /// Gets or sets a value that determines what to transform the position into, if the rule's conditions are fulfilled.
        /// </summary>
        public Enums.HeightLevel TransformTo { get; set; }

        /// <summary>
        /// Gets or sets a value that determines which neighbourhood the rule looks at.
        /// </summary>
        public RuleEnums.Neighbourhood Neighbourhood { get; set; }
    }
}
