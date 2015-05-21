namespace OPMGFS.Novelty
{
    /// <summary>
    /// The novelty search options.
    /// </summary>
    public class NoveltySearchOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoveltySearchOptions"/> class.
        /// </summary>
        /// <param name="mutate">
        /// Whether the novelty search should use mutation.
        /// </param>
        /// <param name="recombine">
        /// Whether the novelty search should use recombination.
        /// </param>
        /// <param name="mutationChance">
        /// The chance of a mutation happening.
        /// </param>
        /// <param name="twoPointCrossover">
        /// Whether the novelty search should use single point or two point crossover when recombining.
        /// </param>
        /// <param name="numberOfNeighbours">
        /// The number of neighbours to use in calculation of novelty.
        /// </param>
        /// <param name="addToArchive">
        /// The amount of individuals to add to the novel archive.
        /// </param>
        /// <param name="minimumNovelty">
        /// The minimum novelty required for an individual to be added to the novel archive.
        /// </param>
        /// <param name="feasiblePopulationSize">
        /// The feasible Population Size.
        /// </param>
        /// <param name="infeasiblePopulationSize">
        /// The infeasible Population Size.
        /// </param>
        public NoveltySearchOptions(
            bool mutate = true,
            bool recombine = false,
            double mutationChance = 0.3,
            bool twoPointCrossover = false,
            int numberOfNeighbours = 5,
            int addToArchive = 1,
            double minimumNovelty = 5,
            int feasiblePopulationSize = 10,
            int infeasiblePopulationSize = 10)
        {
            this.InfeasiblePopulationSize = infeasiblePopulationSize;
            this.FeasiblePopulationSize = feasiblePopulationSize;
            this.MinimumNovelty = minimumNovelty;
            this.TwoPointCrossover = twoPointCrossover;
            this.MutationChance = mutationChance;
            this.Recombine = recombine;
            this.Mutate = mutate;
            this.NumberOfNeighbours = numberOfNeighbours;
            this.AddToArchive = addToArchive;
        }

        /// <summary>
        /// Gets the number of individuals added to the novelty archive per generation.
        /// </summary>
        public int AddToArchive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether mutation should happen.
        /// </summary>
        public bool Mutate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether recombination should happen.
        /// </summary>
        public bool Recombine { get; private set; }

        /// <summary>
        /// Gets the size that the feasible population should be.
        /// </summary>
        public int FeasiblePopulationSize { get; private set; }

        /// <summary>
        /// Gets the size that the infeasible population should be.
        /// </summary>
        public int InfeasiblePopulationSize { get; private set; }

        /// <summary>
        /// Gets the mutation chance.
        /// </summary>
        public double MutationChance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether two point crossover or single point crossover should be used.
        /// </summary>
        public bool TwoPointCrossover { get; private set; }

        /// <summary>
        /// Gets the number of neighbours that the novelty should be calculated for.
        /// </summary>
        public int NumberOfNeighbours { get; private set; }

        /// <summary>
        /// Gets the minimum novelty required for an individual to be added to the novel archive.
        /// </summary>
        public double MinimumNovelty { get; private set; }
    }
}
