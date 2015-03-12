// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleEnums.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPathfinding type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map.CellularAutomata
{
    /// <summary>
    /// A class that holds enums used by the cellular automata rules.
    /// </summary>
    public class RuleEnums
    {
        /// <summary>
        /// Decides whether the number of neighbours should be greater than, less than or equal to the number
        /// decided on in the rule.
        /// </summary>
        public enum Comparison
        {
            /// <summary>
            /// The number of neighbours must be equal to the given number.
            /// </summary>
            EqualTo,

            /// <summary>
            /// The number of neighbours must be greater than the given number.
            /// </summary>
            GreaterThan,

            /// <summary>
            /// The number of neighbours must be greater than, or equal to, the given number.
            /// </summary>
            GreaterThanEqualTo,

            /// <summary>
            /// The number of neighbours must be less than the given number.
            /// </summary>
            LessThan,

            /// <summary>
            /// The number of neighbours must be less than, or equal to, the given number.
            /// </summary>
            LessThanEqualTo
        }
    }
}
