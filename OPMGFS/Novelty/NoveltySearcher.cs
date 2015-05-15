namespace OPMGFS.Novelty
{
    using System;

    /// <summary>
    /// Searches for novelty using a feasible and an infeasible population.
    /// </summary>
    public abstract class NoveltySearcher
    {
        // ITODO: Implement offspring boost
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
        protected abstract void NextGeneration();
    }
}
