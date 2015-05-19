namespace OPMGFS.Novelty
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Searches for novelty using a feasible and an infeasible population.
    /// </summary>
    public abstract class NoveltySearcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoveltySearcher"/> class.
        /// </summary>
        /// <param name="random">
        /// The random object to use.
        /// </param>
        protected NoveltySearcher(Random random)
        {
            this.Random = random;
        }

        /// <summary>
        /// Gets or sets the random object used to generate numbers.
        /// </summary>
        public Random Random { get; protected set; }

        /// <summary>
        /// Gets or sets the feasible population of the searcher.
        /// </summary>
        public Population FeasiblePopulation { get; protected set; }

        /// <summary>
        /// Gets or sets the infeasible population of the searcher.
        /// </summary>
        public Population InfeasiblePopulation { get; protected set; }

        /// <summary>
        /// Gets or sets the novel archive used by the searcher.
        /// </summary>
        public NovelArchive Archive { get; protected set; }

        /// <summary>
        /// Advances the search to the next generation.
        /// </summary>
        protected void NextGeneration()
        {
            int totalPopulation = FeasiblePopulation.CurrentGeneration.Count
                                  + InfeasiblePopulation.CurrentGeneration.Count;
            int halfPopulationSize = totalPopulation % 2 == 0 ? (totalPopulation / 2) : (totalPopulation / 2) + 1;

            List<Solution> feasibleIndividuals;
            List<Solution> infeasibleIndividuals;

            // If the feasible population is smaller than the infeasible population, apply the offspring boost mechanism.
            if (FeasiblePopulation.CurrentGeneration.Count < InfeasiblePopulation.CurrentGeneration.Count)
            {
                infeasibleIndividuals = FeasiblePopulation.AdvanceGeneration(
                    new NoveltySearchOptions(), 
                    InfeasiblePopulation, 
                    Archive, 
                    Random, 
                    halfPopulationSize);
                feasibleIndividuals = InfeasiblePopulation.AdvanceGeneration(
                    new NoveltySearchOptions(), 
                    FeasiblePopulation, 
                    Archive, 
                    Random, 
                    totalPopulation / 2);
            }
            else
            {
                infeasibleIndividuals = FeasiblePopulation.AdvanceGeneration(
                    new NoveltySearchOptions(), 
                    InfeasiblePopulation, 
                    Archive, 
                    Random, 
                    FeasiblePopulation.CurrentGeneration.Count);
                feasibleIndividuals = InfeasiblePopulation.AdvanceGeneration(
                    new NoveltySearchOptions(), 
                    FeasiblePopulation, 
                    Archive, 
                    Random, 
                    InfeasiblePopulation.CurrentGeneration.Count);
            }

            FeasiblePopulation.CurrentGeneration.AddRange(feasibleIndividuals);
            InfeasiblePopulation.CurrentGeneration.AddRange(infeasibleIndividuals);
        }
    }
}
