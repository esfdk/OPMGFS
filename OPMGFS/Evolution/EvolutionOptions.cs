namespace OPMGFS.Evolution
{
    using System;

    /// <summary>
    /// A class that contains options for evolution.
    /// </summary>
    public class EvolutionOptions
    {
        /// <summary>
        /// The tolerance when comparing doubles.
        /// </summary>
        public static readonly double Tolerance = 0.000001d;

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
    }
}
