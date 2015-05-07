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
            /// Terrain that is impassable by ground movement.
            /// </summary>
            Impassable = -20,

            /// <summary>
            /// Represents a cliff.
            /// </summary>
            Cliff = 9,

            /// <summary>
            /// Used to mark things when testing.
            /// </summary>
            Marker = -1000
        }

        /// <summary>
        /// A value representing what the map contains in various locations.
        /// </summary>
        public enum Item
        {
            /// <summary>
            /// The location contains nothing.
            /// </summary>
            None = 0,

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
            XelNagaTower = 'x',

            /// <summary>
            /// The location contains destructible rocks.
            /// </summary>
            DestructibleRocks = 'd',
            
            /// <summary>
            /// The location contains a starting base.
            /// </summary>
            StartBase = 's',

            /// <summary>
            /// An "occupied space" preventing other elements from being placed there.
            /// </summary>
            Occupied = 'o'
        }

        /// <summary>
        /// A value that represents what half of the map that should be copied/mirrored.
        /// </summary>
        public enum Half
        {
            /// <summary>
            /// The top half.
            /// </summary>
            Top = 0,

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
        /// A point on the map in a solution.
        /// </summary>
        public enum MapPointType
        {
            /// <summary>
            /// A base on the map.
            /// </summary>
            Base,

            /// <summary>
            /// A Xel'Naga tower on the map.
            /// </summary>
            XelNagaTower, 
            
            /// <summary>
            /// A gold base on the map.
            /// </summary>
            GoldBase, 
            
            /// <summary>
            /// A ramp on the map.
            /// </summary>
            Ramp, 
            
            /// <summary>
            /// A starting base on the map.
            /// </summary>
            StartBase, 
            
            /// <summary>
            /// Destructible rocks on the map.
            /// </summary>
            DestructibleRocks
        }

        /// <summary>
        /// Whether a map point was placed or not.
        /// </summary>
        public enum WasPlaced
        {
            /// <summary>
            /// Placement has not yet been attempted.
            /// </summary>
            NotAttempted,
            
            /// <summary>
            /// Placement was successful
            /// </summary>
            Yes,

            /// <summary>
            /// Placement was unsuccessful
            /// </summary>
            No
        }

        /// <summary>
        /// A value that determines how parents are selected for the population step.
        /// </summary>
        public enum SelectionStrategy
        {
            /// <summary>
            /// Select the individuals with highest fitness.
            /// </summary>
            HighestFitness,

            /// <summary>
            /// Select based on chance, with higher fitness resulting in higher chance of being selected.
            /// </summary>
            ChanceBased
        }


        /// <summary>
        /// A value that determines how a new population is created.
        /// </summary>
        public enum PopulationStrategy
        {
            /// <summary>
            /// Uses mutation to create the new population
            /// </summary>
            Mutation,

            /// <summary>
            /// Select based on chance, with higher fitness resulting in higher chance of being selected.
            /// </summary>
            Recombination
        }

        /// <summary>
        /// Gets the char value of an Item.
        /// </summary>
        /// <param name="item">The item to get the char value of</param>
        /// <returns>A string containing the char value of the item.</returns>
        public static string GetCharValue(Item item)
        {
            if (item == Item.None) return ".";
            return ((char)item).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the char value of a Heightlevel.
        /// </summary>
        /// <param name="level">The heightlevel to get the char value of</param>
        /// <returns>A string containing the char value of the item.</returns>
        public static string GetCharValue(HeightLevel level)
        {
            return string.Empty + (int)level;
        }
    }
}
