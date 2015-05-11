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
            // ITODO: Implement IsDominatedBy
            throw new NotImplementedException();
        }

        public bool Dominates(MapFitnessValues mfv)
        {
            // ITODO: Implement IsDominatedBy
            throw new NotImplementedException();
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
            // ITODO: implement total fitness calculation
            throw new NotImplementedException();
        }
    }
}
