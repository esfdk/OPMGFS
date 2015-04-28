namespace OPMGFS.Novelty
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A population in a novelty search.
    /// </summary>
    public abstract class Population
    {
        // ITODO: Melnyk - Implement FI2NS

        /// <summary>
        /// Initializes a new instance of the <see cref="Population"/> class.
        /// </summary>
        /// <param name="isFeasiblePopulation">
        /// Determines if this is a feasible or infeasible population.
        /// </param>
        /// <param name="populationSize">Size of the population</param>
        protected Population(bool isFeasiblePopulation, int populationSize)
        {
            this.IsFeasiblePopulation = isFeasiblePopulation;
            this.PopulationSize = populationSize;
        }

        /// <summary>
        /// Gets a value indicating whether this is a population of feasible or infeasible individuals.
        /// </summary>
        public bool IsFeasiblePopulation { get; private set; }

        /// <summary>
        /// Gets or sets the current generation of individuals.
        /// </summary>
        public List<Solution> CurrentGeneration { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating the size of the population.
        /// </summary>
        protected int PopulationSize { get; set; }

        /// <summary>
        /// Advances the generation to the next generation.
        /// </summary>
        /// <param name="nso">The options to use for search.</param>
        /// <param name="other"> The other population.</param>
        /// <param name="na">The Archive of novel solutions.</param>
        /// <param name="r">The random generator to use.</param>
        /// <returns>The children that are not part of this feasible/infeasible set.</returns>
        public abstract List<Solution> AdvanceGeneration(NoveltySearchOptions nso, Population other, NovelArchive na, Random r);
    }
}
