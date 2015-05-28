﻿namespace OPMGFS.Map
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the different fitness values for a map.
    /// </summary>
    public class MapFitnessValues
    {
        /// <summary>
        /// The total fitness of the different fitness measures.
        /// </summary>
        private double totalFitness;

        /// <summary>
        /// Indicates whether the total fitness has been calculated previously.
        /// </summary>
        private bool totalFitnessCalculated;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapFitnessValues"/> class.
        /// </summary>
        /// <param name="baseSpaceFitness"> The base space fitness. </param>
        /// <param name="baseHeightLevelFitness"> The base height level fitness. </param>
        /// <param name="pathBetweenStartBasesFitness"> The path between start bases fitness. </param>
        /// <param name="newHeightReachedFitness"> The new height reached fitness. </param>
        /// <param name="distanceToNaturalExpansionFitness"> The distance to natural expansion fitness. </param>
        /// <param name="distanceToNonNaturalExpansionFitness"> The distance to non natural expansion fitness. </param>
        /// <param name="expansionsAvailableFitness"> The expansions available fitness. </param>
        /// <param name="chokePointFitness"> The choke point fitness. </param>
        /// <param name="xelNagaPlacementFitness"> The Xel'Naga placement fitness. </param>
        /// <param name="startBaseOpennessFitness"> The start base openness fitness. </param>
        /// <param name="baseOpennessFitness"> The base openness fitness. </param>
        public MapFitnessValues(
            double baseSpaceFitness,
            double baseHeightLevelFitness,
            double pathBetweenStartBasesFitness,
            double newHeightReachedFitness,
            double distanceToNaturalExpansionFitness,
            double distanceToNonNaturalExpansionFitness,
            double expansionsAvailableFitness,
            double chokePointFitness,
            double xelNagaPlacementFitness,
            double startBaseOpennessFitness,
            double baseOpennessFitness)
        {
            this.BaseSpaceFitness = baseSpaceFitness;
            this.BaseHeightLevelFitness = baseHeightLevelFitness;
            this.PathBetweenStartBasesFitness = pathBetweenStartBasesFitness;
            this.NewHeightReachedFitness = newHeightReachedFitness;
            this.DistanceToNaturalExpansionFitness = distanceToNaturalExpansionFitness;
            this.DistanceToNonNaturalExpansionFitness = distanceToNonNaturalExpansionFitness;
            this.ExpansionsAvailableFitness = expansionsAvailableFitness;
            this.ChokePointFitness = chokePointFitness;
            this.XelNagaPlacementFitness = xelNagaPlacementFitness;
            this.StartBaseOpennessFitness = startBaseOpennessFitness;
            this.BaseOpennessFitness = baseOpennessFitness;
        }

        /// <summary>
        /// Gets the sum of the different fitness parts.
        /// </summary>
        public double TotalFitness
        {
            get
            {
                if (!this.totalFitnessCalculated)
                {
                    this.totalFitness = this.CalculateTotalFitness();
                    this.totalFitnessCalculated = true;
                    return this.totalFitness;
                }

                return this.totalFitness;
            }
        }

        /// <summary>
        /// Gets the base space fitness.
        /// </summary>
        public double BaseSpaceFitness { get; private set; }

        /// <summary>
        /// Gets the base height level fitness.
        /// </summary>
        public double BaseHeightLevelFitness { get; private set; }

        /// <summary>
        /// Gets the fitness of the path between starting bases.
        /// </summary>
        public double PathBetweenStartBasesFitness { get; private set; }

        /// <summary>
        /// Gets the fitness for the number of times a new height level is reached on the path between start bases.
        /// </summary>
        public double NewHeightReachedFitness { get; private set; }

        /// <summary>
        /// Gets the fitness of the distance to the natural expansion.
        /// </summary>
        public double DistanceToNaturalExpansionFitness { get; private set; }

        /// <summary>
        /// Gets the fitness for the distances to non natural expansions.
        /// </summary>
        public double DistanceToNonNaturalExpansionFitness { get; private set; }

        /// <summary>
        /// Gets the fitness of the number of available expansions.
        /// </summary>
        public double ExpansionsAvailableFitness { get; private set; }

        /// <summary>
        /// Gets the fitness of the number of choke points on the path between the starting bases.
        /// </summary>
        public double ChokePointFitness { get; private set; }

        /// <summary>
        /// Gets the fitness of the Xel'Naga tower placement.
        /// </summary>
        public double XelNagaPlacementFitness { get; private set; }

        /// <summary>
        /// Gets the fitness of the start base openness.
        /// </summary>
        public double StartBaseOpennessFitness { get; private set; }

        /// <summary>
        /// Gets the
        /// </summary>
        public double BaseOpennessFitness { get; private set; }

        /// <summary>
        /// Checks if these fitness values are dominated by another set of fitness values.
        /// </summary>
        /// <param name="mfv">The other set of fitness values.</param>
        /// <returns>True if these fitness values are dominated by the input values, false if not.</returns>
        public bool IsDominatedBy(MapFitnessValues mfv)
        {
            if (this.BaseSpaceFitness > mfv.BaseSpaceFitness) return false;
            if (this.BaseHeightLevelFitness > mfv.BaseHeightLevelFitness) return false;
            if (this.PathBetweenStartBasesFitness > mfv.PathBetweenStartBasesFitness) return false;
            if (this.NewHeightReachedFitness > mfv.NewHeightReachedFitness) return false;
            if (this.DistanceToNaturalExpansionFitness > mfv.DistanceToNaturalExpansionFitness) return false;
            if (this.DistanceToNonNaturalExpansionFitness > mfv.DistanceToNonNaturalExpansionFitness) return false;
            if (this.ExpansionsAvailableFitness > mfv.ExpansionsAvailableFitness) return false;
            if (this.ChokePointFitness > mfv.ChokePointFitness) return false;
            if (this.XelNagaPlacementFitness > mfv.XelNagaPlacementFitness) return false;
            if (this.StartBaseOpennessFitness > mfv.StartBaseOpennessFitness) return false;
            if (this.BaseOpennessFitness > mfv.BaseOpennessFitness) return false;

            return this.BaseSpaceFitness < mfv.BaseSpaceFitness
                   || this.BaseHeightLevelFitness < mfv.BaseHeightLevelFitness
                   || this.PathBetweenStartBasesFitness < mfv.PathBetweenStartBasesFitness
                   || this.NewHeightReachedFitness < mfv.NewHeightReachedFitness
                   || this.DistanceToNaturalExpansionFitness < mfv.DistanceToNaturalExpansionFitness
                   || this.DistanceToNonNaturalExpansionFitness < mfv.DistanceToNonNaturalExpansionFitness
                   || this.ExpansionsAvailableFitness < mfv.ExpansionsAvailableFitness
                   || this.ChokePointFitness < mfv.ChokePointFitness
                   || this.XelNagaPlacementFitness < mfv.XelNagaPlacementFitness
                   || this.StartBaseOpennessFitness < mfv.StartBaseOpennessFitness
                   || this.BaseOpennessFitness < mfv.BaseOpennessFitness;
        }

        /// <summary>
        /// Checks if these fitness values dominate another set of fitness values.
        /// </summary>
        /// <param name="mfv">The other set of fitness values.</param>
        /// <returns>True if these fitness values dominate the input values, false if not.</returns>
        public bool Dominates(MapFitnessValues mfv)
        {
            if (this.BaseSpaceFitness < mfv.BaseSpaceFitness) return false;
            if (this.BaseHeightLevelFitness < mfv.BaseHeightLevelFitness) return false;
            if (this.PathBetweenStartBasesFitness < mfv.PathBetweenStartBasesFitness) return false;
            if (this.NewHeightReachedFitness < mfv.NewHeightReachedFitness) return false;
            if (this.DistanceToNaturalExpansionFitness < mfv.DistanceToNaturalExpansionFitness) return false;
            if (this.DistanceToNonNaturalExpansionFitness < mfv.DistanceToNonNaturalExpansionFitness) return false;
            if (this.ExpansionsAvailableFitness < mfv.ExpansionsAvailableFitness) return false;
            if (this.ChokePointFitness < mfv.ChokePointFitness) return false;
            if (this.XelNagaPlacementFitness < mfv.XelNagaPlacementFitness) return false;
            if (this.StartBaseOpennessFitness < mfv.StartBaseOpennessFitness) return false;
            if (this.BaseOpennessFitness < mfv.BaseOpennessFitness) return false;

            return this.BaseSpaceFitness > mfv.BaseSpaceFitness
                   || this.BaseHeightLevelFitness > mfv.BaseHeightLevelFitness
                   || this.PathBetweenStartBasesFitness > mfv.PathBetweenStartBasesFitness
                   || this.NewHeightReachedFitness > mfv.NewHeightReachedFitness
                   || this.DistanceToNaturalExpansionFitness > mfv.DistanceToNaturalExpansionFitness
                   || this.DistanceToNonNaturalExpansionFitness > mfv.DistanceToNonNaturalExpansionFitness
                   || this.ExpansionsAvailableFitness > mfv.ExpansionsAvailableFitness
                   || this.ChokePointFitness > mfv.ChokePointFitness
                   || this.XelNagaPlacementFitness > mfv.XelNagaPlacementFitness
                   || this.StartBaseOpennessFitness > mfv.StartBaseOpennessFitness
                   || this.BaseOpennessFitness > mfv.BaseOpennessFitness;
        }

        /// <summary>
        /// Gets a list that contains all the fitness values.
        /// </summary>
        /// <returns> A list containing the fitness values. </returns>
        public List<double> FitnessList()
        {
            var list = new List<double>
                           {
                               this.BaseSpaceFitness,
                               this.BaseHeightLevelFitness,
                               this.PathBetweenStartBasesFitness,
                               this.NewHeightReachedFitness,
                               this.DistanceToNaturalExpansionFitness,
                               this.DistanceToNonNaturalExpansionFitness,
                               this.ExpansionsAvailableFitness,
                               this.ChokePointFitness,
                               this.XelNagaPlacementFitness,
                               this.StartBaseOpennessFitness,
                               this.BaseOpennessFitness
                           };

            return list;
        }
        
        /// <summary>
        /// Calculates the total fitness of the map these fitness values relates to.
        /// </summary>
        /// <returns>The total fitness.</returns>
        private double CalculateTotalFitness()
        {
            return this.BaseSpaceFitness
                 + this.BaseHeightLevelFitness
                 + this.PathBetweenStartBasesFitness
                 + this.NewHeightReachedFitness
                 + this.DistanceToNaturalExpansionFitness
                 + this.DistanceToNonNaturalExpansionFitness
                 + this.ExpansionsAvailableFitness
                 + this.ChokePointFitness
                 + this.XelNagaPlacementFitness
                 + this.StartBaseOpennessFitness
                 + this.BaseOpennessFitness;
        }
    }
}
