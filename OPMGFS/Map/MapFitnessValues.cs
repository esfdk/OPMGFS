namespace OPMGFS.Map
{
    using System;

    public class MapFitnessValues
    {
        private double totalFitness;

        private bool totalFitnessCalculated;

        public MapFitnessValues(
            double baseSpaceFitness,
            double baseHeightLevelFitness,
            double newHeightReachedFitness,
            double pathBetweenStartBasesFitness,
            double chokePointFitness,
            double distanceToNaturalExpansionFitness,
            double distanceToNonNaturalExpansionFitness,
            double startBaseOpenessFitness,
            double baseOpenessFitness,
            double xelNagaPlacementFitness,
            double expansionsAvailableFitness)
        {
            this.BaseSpaceFitness = baseSpaceFitness;
            this.BaseHeightLevelFitness = baseHeightLevelFitness;
            this.NewHeightReachedFitness = newHeightReachedFitness;
            this.PathBetweenStartBasesFitness = pathBetweenStartBasesFitness;
            this.ChokePointFitness = chokePointFitness;
            this.DistanceToNaturalExpansionFitness = distanceToNaturalExpansionFitness;
            this.DistanceToNonNaturalExpansionFitness = distanceToNonNaturalExpansionFitness;
            this.StartBaseOpenessFitness = startBaseOpenessFitness;
            this.BaseOpenessFitness = baseOpenessFitness;
            this.XelNagaPlacementFitness = xelNagaPlacementFitness;
            this.ExpansionsAvailableFitness = expansionsAvailableFitness;
        }

        public double TotalFitness
        {
            get
            {
                if (!totalFitnessCalculated)
                {
                    totalFitness = this.CalculateTotalFitness();
                    totalFitnessCalculated = true;
                    return totalFitness;
                }

                return totalFitness;
            }
        }

        public double BaseSpaceFitness { get; private set; }

        public double BaseHeightLevelFitness { get; private set; }

        public double PathBetweenStartBasesFitness { get; private set; }

        public double NewHeightReachedFitness { get; private set; }

        public double ChokePointFitness { get; private set; }

        public double DistanceToNaturalExpansionFitness { get; private set; }

        public double DistanceToNonNaturalExpansionFitness { get; private set; }

        public double ExpansionsAvailableFitness { get; private set; }

        public double XelNagaPlacementFitness { get; private set; }

        public double StartBaseOpenessFitness { get; private set; }

        public double BaseOpenessFitness { get; private set; }

        public bool IsDominatedBy(MapFitnessValues mfv)
        {
            if (this.BaseSpaceFitness > mfv.BaseSpaceFitness) return false;
            if (this.BaseHeightLevelFitness > mfv.BaseHeightLevelFitness) return false;
            if (this.PathBetweenStartBasesFitness > mfv.PathBetweenStartBasesFitness) return false;
            if (this.NewHeightReachedFitness > mfv.NewHeightReachedFitness) return false;
            if (this.ChokePointFitness > mfv.ChokePointFitness) return false;
            if (this.DistanceToNaturalExpansionFitness > mfv.DistanceToNaturalExpansionFitness) return false;
            if (this.DistanceToNonNaturalExpansionFitness > mfv.DistanceToNonNaturalExpansionFitness) return false;
            if (this.ExpansionsAvailableFitness > mfv.ExpansionsAvailableFitness) return false;
            if (this.XelNagaPlacementFitness > mfv.XelNagaPlacementFitness) return false;
            if (this.StartBaseOpenessFitness > mfv.StartBaseOpenessFitness) return false;
            if (this.BaseOpenessFitness > mfv.BaseOpenessFitness) return false;

            return this.BaseSpaceFitness < mfv.BaseSpaceFitness
                   || this.BaseHeightLevelFitness < mfv.BaseHeightLevelFitness
                   || this.PathBetweenStartBasesFitness < mfv.PathBetweenStartBasesFitness
                   || this.NewHeightReachedFitness < mfv.NewHeightReachedFitness
                   || this.ChokePointFitness < mfv.ChokePointFitness
                   || this.DistanceToNaturalExpansionFitness < mfv.DistanceToNaturalExpansionFitness
                   || this.DistanceToNonNaturalExpansionFitness < mfv.DistanceToNonNaturalExpansionFitness
                   || this.ExpansionsAvailableFitness < mfv.ExpansionsAvailableFitness
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
            if (this.ChokePointFitness < mfv.ChokePointFitness) return false;
            if (this.DistanceToNaturalExpansionFitness < mfv.DistanceToNaturalExpansionFitness) return false;
            if (this.DistanceToNonNaturalExpansionFitness < mfv.DistanceToNonNaturalExpansionFitness) return false;
            if (this.ExpansionsAvailableFitness < mfv.ExpansionsAvailableFitness) return false;
            if (this.XelNagaPlacementFitness < mfv.XelNagaPlacementFitness) return false;
            if (this.StartBaseOpenessFitness < mfv.StartBaseOpenessFitness) return false;
            if (this.BaseOpenessFitness < mfv.BaseOpenessFitness) return false;

            return this.BaseSpaceFitness > mfv.BaseSpaceFitness
                   || this.BaseHeightLevelFitness > mfv.BaseHeightLevelFitness
                   || this.PathBetweenStartBasesFitness > mfv.PathBetweenStartBasesFitness
                   || this.NewHeightReachedFitness > mfv.NewHeightReachedFitness
                   || this.ChokePointFitness > mfv.ChokePointFitness
                   || this.DistanceToNaturalExpansionFitness > mfv.DistanceToNaturalExpansionFitness
                   || this.DistanceToNonNaturalExpansionFitness > mfv.DistanceToNonNaturalExpansionFitness
                   || this.ExpansionsAvailableFitness > mfv.ExpansionsAvailableFitness
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

        private double CalculateTotalFitness()
        {
            return this.BaseSpaceFitness
                 + this.BaseHeightLevelFitness
                 + this.PathBetweenStartBasesFitness
                 + this.NewHeightReachedFitness
                 + this.ChokePointFitness
                 + this.DistanceToNaturalExpansionFitness
                 + this.DistanceToNonNaturalExpansionFitness
                 + this.ExpansionsAvailableFitness
                 + this.XelNagaPlacementFitness
                 + this.StartBaseOpenessFitness
                 + this.BaseOpenessFitness;
        }
    }
}
