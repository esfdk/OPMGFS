namespace OPMGFS.Novelty.IntegerNoveltySearch
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A solution in integer novelty search.
    /// </summary>
    public class IntegerSolution : Solution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerSolution"/> class.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        public IntegerSolution(int number)
        {
            this.Number = number;
        }

        /// <summary>
        /// Gets the number value of the solution.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Mutates this solution into a new solution.
        /// </summary>
        /// <param name="r">
        /// The random generator to use in mutation.
        /// </param>
        /// <returns>
        /// The newly mutated solution.
        /// </returns>
        public override Solution Mutate(Random r)
        {
            var newValue = this.Number + (r.Next(20000) - 10000);
            return new IntegerSolution(newValue);
        }

        /// <summary>
        /// Combines this solution with another to create a new solution.
        /// </summary>
        /// <param name="other">The solution to combine with.</param>
        /// <returns>The newly created solution.</returns>
        public override Solution Recombine(Solution other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the novelty of the solution based on the current population and the novel Archive.
        /// </summary>
        /// <param name="feasible">
        /// The population of feasible solutions.
        /// </param>
        /// <param name="archive">
        /// The Archive of novel solutions.
        /// </param>
        /// <param name="numberOfNeighbours">
        /// The number of neighbours that the novelty should be averaged over.
        /// </param>
        /// <returns>
        /// The novelty of the solution.
        /// </returns>
        public override double CalculateNovelty(Population feasible, NovelArchive archive, int numberOfNeighbours)
        {
            var distanceValues = new List<Tuple<double, int, int>>();

            for (var i = 0; i < feasible.CurrentGeneration.Count; i++)
            {
                if (feasible.CurrentGeneration[i] == this)
                {
                    continue;
                }

                var distance = this.CalculateDistance(feasible.CurrentGeneration[i]);

                var wasAdded = false;

                for (var j = 0; j < distanceValues.Count; j++)
                {
                    if (!(distance < distanceValues[j].Item1))
                    {
                        continue;
                    }

                    distanceValues.Insert(j, new Tuple<double, int, int>(distance, i, 1));
                    wasAdded = true;
                    break;
                }

                if (!wasAdded)
                {
                    distanceValues.Add(new Tuple<double, int, int>(distance, i, 1));
                }
            }

            for (var i = 0; i < archive.Archive.Count; i++)
            {
                var distance = this.CalculateDistance(archive.Archive[i]);

                var wasAdded = false;

                for (var j = 0; j < distanceValues.Count; j++)
                {
                    if (!(distance < distanceValues[j].Item1))
                    {
                        continue;
                    }

                    distanceValues.Insert(j, new Tuple<double, int, int>(distance, i, 2));
                    wasAdded = true;
                    break;
                }

                if (!wasAdded)
                {
                    distanceValues.Add(new Tuple<double, int, int>(distance, i, 2));
                }
            }

            var novelty = 0.0;

            for (var i = 0; i < numberOfNeighbours; i++)
            {
                novelty += distanceValues[i].Item1;
            }

            this.Novelty = novelty / numberOfNeighbours;
            return this.Novelty;
        }

        /// <summary>
        /// Calculates the distance between this solution and a target solution.
        /// </summary>
        /// <param name="other">
        /// The solution to calculate the distance to.
        /// </param>
        /// <returns>
        /// The distance from this solution to the target solution.
        /// </returns>
        protected override double CalculateDistance(Solution other)
        {
            return Math.Abs(this.Number - ((IntegerSolution)other).Number);
        }

        /// <summary>
        /// Calculates how far the solution is from reaching feasibility.
        /// </summary>
        /// <returns>The distance to feasibility.</returns>
        protected override double CalculateDistanceToFeasibility()
        {
            this.DistanceToFeasibility = 0 - this.Number;

            return this.DistanceToFeasibility;
        }
    }
}
