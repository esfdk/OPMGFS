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
    using System.Diagnostics.CodeAnalysis;

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
            /// The number of neighbours must be greater than, or equal to, the given number.
            /// </summary>
            GreaterThanEqualTo = 0,

            /// <summary>
            /// The number of neighbours must be greater than the given number.
            /// </summary>
            GreaterThan = 1,

            /// <summary>
            /// The number of neighbours must be equal to the given number.
            /// </summary>
            EqualTo = 2,

            /// <summary>
            /// The number of neighbours must be less than the given number.
            /// </summary>
            LessThan = 3,

            /// <summary>
            /// The number of neighbours must be less than, or equal to, the given number.
            /// </summary>
            LessThanEqualTo = 4
        }

        /// <summary>
        /// An enum that determines what kind of neighbourhood a rule is looking at.
        /// </summary>
        public enum Neighbourhood
        {
            /// <summary>
            /// Standard Moore neighbourhood (8 tiles).
            /// <para/>OOO
            /// <para/>OXO
            /// <para/>OOO
            /// </summary>
            Moore = 0,

            /// <summary>
            /// The extended Moore neighbourhood (24 tiles).
            /// <para/>OOOOO
            /// <para/>OOOOO
            /// <para/>OOXOO
            /// <para/>OOOOO
            /// <para/>OOOOO
            /// </summary>
            MooreExtended = 1,

            /// <summary>
            /// Standard Von Neumann neighbourhood (4 tiles).
            /// <para/>_o_
            /// <para/>oxo
            /// <para/>_o_
            /// </summary>
            VonNeumann = 2,

            /// <summary>
            /// The extended Von Neumann neighbourhood (8 tiles).
            /// <para/>___o__
            /// <para/>___o__
            /// <para/>ooxoo
            /// <para/>___o__
            /// <para/>___o__
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            VonNeumannExtended = 3
        }
    }
}
