namespace OPMGFS.Novelty
{
    using System;

    /// <summary>
    /// A solution in constrained novelty search.
    /// </summary>
    public abstract class Solution
    {
        /// <summary>
        /// Controls whether the feasibility should be calculated when asked whether the solution is feasible.
        /// </summary>
        private bool isFeasibilityCalculated;

        /// <summary>
        /// Keeps track of the feasibility of the solution.
        /// </summary>
        private bool isFeasible;

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        protected Solution()
        {
            this.isFeasible = false;
            this.isFeasibilityCalculated = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this solution is part of the feasible set of solutions.
        /// </summary>
        public bool IsFeasible
        {
            get
            {
                return this.isFeasibilityCalculated ? this.isFeasible : this.CheckFeasibility();
            }

            protected set
            {
                this.isFeasible = value;
            }
        }

        /// <summary>
        /// Gets or sets the distance of this solution to feasibility. If the score is 0 or lower, then the solution is considered feasible.
        /// </summary>
        public double DistanceToFeasibility { get; protected set; }

        /// <summary>
        /// Gets or sets the novelty of this solution.
        /// </summary>
        public double Novelty { get; protected set; }

        /// <summary>
        /// Mutates this solution into a new solution.
        /// </summary>
        /// <param name="r">The random generator to use in mutation.</param>
        /// <returns>The newly mutated solution.</returns>
        public abstract Solution Mutate(Random r);

        /// <summary>
        /// Combines this solution with another to create a new solution.
        /// </summary>
        /// <param name="other">The solution to combine with.</param>
        /// <returns>The newly created solution.</returns>
        public abstract Solution Recombine(Solution other);

        /// <summary>
        /// Calculates the novelty of the solution based on the current population and the novel Archive.
        /// </summary>
        /// <param name="feasible">The population of feasible solutions.</param>
        /// <param name="archive">The Archive of novel solutions.</param>
        /// <param name="numberOfNeighbours">The number of neighbours that the novelty should be averaged over.</param>
        /// <returns>The novelty of the solution.</returns>
        public abstract double CalculateNovelty(Population feasible, NovelArchive archive, int numberOfNeighbours);

        /// <summary>
        /// Checks whether the solution is part of the feasible set of solutions.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> value indicating the feasibility of the solution.
        /// </returns>
        protected virtual bool CheckFeasibility()
        {
            this.CalculateDistanceToFeasibility();
            this.isFeasibilityCalculated = true;

            if (!(this.DistanceToFeasibility <= 0))
            {
                return false;
            }

            this.IsFeasible = true;

            return true;
        }

        /// <summary>
        /// Calculates the distance between this solution and a target solution.
        /// </summary>
        /// <param name="other">The solution to calculate the distance to.</param>
        /// <returns>The distance from this solution to the target solution.</returns>
        protected abstract double CalculateDistance(Solution other);

        /// <summary>
        /// Calculates how far the solution is from reaching feasibility.
        /// </summary>
        /// <returns>The distance to feasibility.</returns>
        protected abstract double CalculateDistanceToFeasibility();
    }
}
