// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the Enums type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// The class that contains the different enums used by the map.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// A value representing the height at a position of the map.
        /// </summary>
        public enum HeightLevel
        {
            /// <summary>
            /// Represents a height of 0.
            /// </summary>
            Height0 = 0,

            /// <summary>
            /// Represents a ramp going from level 0 to 1.
            /// </summary>
            Ramp01 = 1,

            /// <summary>
            /// Represents a height of 1.
            /// </summary>
            Height1 = 2,

            /// <summary>
            /// Represents a ramp going from level 1 to 2.
            /// </summary>
            Ramp12 = 3,

            /// <summary>
            /// Represents a height of 2.
            /// </summary>
            Height2 = 4,

            /// <summary>
            /// Represents a cliff.
            /// </summary>
            Cliff = 9
        }

        /// <summary>
        /// A value representing what the map contains in various locations.
        /// </summary>
        public enum Item
        {
            /// <summary>
            /// The location contains nothing.
            /// </summary>
            None = '.',

            /// <summary>
            /// A ramp.
            /// </summary>
            Ramp = 'r',

            /// <summary>
            /// A ramp That goes from north to south.
            /// </summary>
            RampNorthSouth = '|',

            /// <summary>
            /// A ramp That goes from north-east to south-west.
            /// </summary>
            RampNortheastSouthwest = '/',

            /// <summary>
            /// A ramp That goes from east to west.
            /// </summary>
            RampEastWest = '-',

            /// <summary>
            /// A ramp That goes from north-west to south-east.
            /// </summary>
            RampNorthwestSoutheast = '\\',

            /// <summary>
            /// The cliff.
            /// </summary>
            Cliff = 'c',

            /// <summary>
            /// A base.
            /// </summary>
            Base = 'B',

            /// <summary>
            /// Blue minerals.
            /// </summary>
            BlueMinerals = 'm',

            /// <summary>
            /// Gold minerals.
            /// </summary>
            GoldMinerals = 'M',

            /// <summary>
            /// A gas deposit.
            /// </summary>
            Gas = 'g',

            /// <summary>
            /// Xel'naga tower.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            XelNagaTower = 'x',

            /// <summary>
            /// The location contains destructible rocks.
            /// </summary>
            DestructibleRocks = 'd'
        }

        /// <summary>
        /// A value that represents what half of the map that should be copied/mirrored.
        /// </summary>
        public enum Half
        {
            /// <summary>
            /// The top half.
            /// </summary>
            Top,

            /// <summary>
            /// The bottom half.
            /// </summary>
            Bottom,

            /// <summary>
            /// The left half.
            /// </summary>
            Left,

            /// <summary>
            /// The right half.
            /// </summary>
            Right
        }

        /// <summary>
        /// A value that represents different functions of the map.
        /// </summary>
        public enum MapFunction
        {
            /// <summary>
            /// Do nothing.
            /// </summary>
            None,

            /// <summary>
            /// Mirror the map.
            /// </summary>
            Mirror,

            /// <summary>
            /// Turn the map.
            /// </summary>
            Turn
        }

        /// <summary>
        /// Gets the char value of an Item.
        /// </summary>
        /// <param name="item">The item to get the char value of</param>
        /// <returns>A string containing the char value of the item.</returns>
        public static string GetItemCharValue(Item item)
        {
            return ((char)item).ToString(CultureInfo.InvariantCulture);
        }
    }
}
