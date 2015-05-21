namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;

    public class MapFitnessValues
    {
        private double totalFitness;

        private bool totalFitnessCalculated;

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
            double startBaseOpenessFitness,
            double baseOpenessFitness)
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
            this.StartBaseOpenessFitness = startBaseOpenessFitness;
            this.BaseOpenessFitness = baseOpenessFitness;
        }

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

        public double BaseSpaceFitness { get; private set; }

        public double BaseHeightLevelFitness { get; private set; }

        public double PathBetweenStartBasesFitness { get; private set; }

        public double NewHeightReachedFitness { get; private set; }

        public double DistanceToNaturalExpansionFitness { get; private set; }

        public double DistanceToNonNaturalExpansionFitness { get; private set; }

        public double ExpansionsAvailableFitness { get; private set; }

        public double ChokePointFitness { get; private set; }

        public double XelNagaPlacementFitness { get; private set; }

        public double StartBaseOpenessFitness { get; private set; }

        public double BaseOpenessFitness { get; private set; }

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
            if (this.StartBaseOpenessFitness > mfv.StartBaseOpenessFitness) return false;
            if (this.BaseOpenessFitness > mfv.BaseOpenessFitness) return false;

            return this.BaseSpaceFitness < mfv.BaseSpaceFitness
                   || this.BaseHeightLevelFitness < mfv.BaseHeightLevelFitness
                   || this.PathBetweenStartBasesFitness < mfv.PathBetweenStartBasesFitness
                   || this.NewHeightReachedFitness < mfv.NewHeightReachedFitness
                   || this.DistanceToNaturalExpansionFitness < mfv.DistanceToNaturalExpansionFitness
                   || this.DistanceToNonNaturalExpansionFitness < mfv.DistanceToNonNaturalExpansionFitness
                   || this.ExpansionsAvailableFitness < mfv.ExpansionsAvailableFitness
                   || this.ChokePointFitness < mfv.ChokePointFitness
                   || this.XelNagaPlacementFitness < mfv.XelNagaPlacementFitness
                   || this.StartBaseOpenessFitness < mfv.StartBaseOpenessFitness
                   || this.BaseOpenessFitness < mfv.BaseOpenessFitness;
        }

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
            if (this.StartBaseOpenessFitness < mfv.StartBaseOpenessFitness) return false;
            if (this.BaseOpenessFitness < mfv.BaseOpenessFitness) return false;

            return this.BaseSpaceFitness > mfv.BaseSpaceFitness
                   || this.BaseHeightLevelFitness > mfv.BaseHeightLevelFitness
                   || this.PathBetweenStartBasesFitness > mfv.PathBetweenStartBasesFitness
                   || this.NewHeightReachedFitness > mfv.NewHeightReachedFitness
                   || this.DistanceToNaturalExpansionFitness > mfv.DistanceToNaturalExpansionFitness
                   || this.DistanceToNonNaturalExpansionFitness > mfv.DistanceToNonNaturalExpansionFitness
                   || this.ExpansionsAvailableFitness > mfv.ExpansionsAvailableFitness
                   || this.ChokePointFitness > mfv.ChokePointFitness
                   || this.XelNagaPlacementFitness > mfv.XelNagaPlacementFitness
                   || this.StartBaseOpenessFitness > mfv.StartBaseOpenessFitness
                   || this.BaseOpenessFitness > mfv.BaseOpenessFitness;
        }

        public bool GreaterThan(MapFitnessValues mfv)
        {
            if (this.Dominates(mfv))
            {
                return this.TotalFitness > mfv.TotalFitness;
            }

            return false;
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
                               this.StartBaseOpenessFitness,
                               this.BaseOpenessFitness
                           };

            return list;
        }

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
                 + this.StartBaseOpenessFitness
                 + this.BaseOpenessFitness;
        }
    }
}
