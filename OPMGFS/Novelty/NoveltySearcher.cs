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
        /// <param name="noveltySearchOptions">The options for this novelty search.</param>
        protected NoveltySearcher(Random random, NoveltySearchOptions noveltySearchOptions)
        {
            this.Random = random;
            this.NoveltySearchOptions = noveltySearchOptions;
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
        /// Gets the options for this search.
        /// </summary>
        public NoveltySearchOptions NoveltySearchOptions { get; private set; }

        /// <summary>
        /// Advances the search to the next generation.
        /// </summary>
        protected void NextGeneration()
        {
            var totalPopulation = this.FeasiblePopulation.CurrentGeneration.Count
                                  + this.InfeasiblePopulation.CurrentGeneration.Count;
            var halfPopulationSize = totalPopulation % 2 == 0 ? (totalPopulation / 2) : (totalPopulation / 2) + 1;

            List<Solution> feasibleIndividuals;
            List<Solution> infeasibleIndividuals;

            // If the feasible population is smaller than the infeasible population, apply the offspring boost mechanism.
            if (this.FeasiblePopulation.CurrentGeneration.Count < this.InfeasiblePopulation.CurrentGeneration.Count)
            {
                infeasibleIndividuals = this.FeasiblePopulation.AdvanceGeneration(
                    this.NoveltySearchOptions, 
                    this.InfeasiblePopulation, 
                    this.Archive, 
                    this.Random, 
                    halfPopulationSize);
                feasibleIndividuals = this.InfeasiblePopulation.AdvanceGeneration(
                    this.NoveltySearchOptions, 
                    this.FeasiblePopulation, 
                    this.Archive, 
                    this.Random, 
                    totalPopulation / 2);
            }
            else
            {
                infeasibleIndividuals = this.FeasiblePopulation.AdvanceGeneration(
                    this.NoveltySearchOptions, 
                    this.InfeasiblePopulation, 
                    this.Archive, 
                    this.Random, 
                    this.FeasiblePopulation.CurrentGeneration.Count);
                feasibleIndividuals = this.InfeasiblePopulation.AdvanceGeneration(
                    this.NoveltySearchOptions, 
                    this.FeasiblePopulation, 
                    this.Archive, 
                    this.Random, 
                    this.InfeasiblePopulation.CurrentGeneration.Count);
            }

            this.FeasiblePopulation.CurrentGeneration.AddRange(feasibleIndividuals);
            this.InfeasiblePopulation.CurrentGeneration.AddRange(infeasibleIndividuals);
        }
    }
}
