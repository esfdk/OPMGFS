namespace OPMGFS.Novelty.IntegerNoveltySearch
{
    using System;

    /// <summary>
    /// Searches for novelty in integers.
    /// </summary>
    public class IntegerSearcher : NoveltySearcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerSearcher"/> class.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="feasibleSize">
        /// The feasible size.
        /// </param>
        /// <param name="infeasibleSize">
        /// The infeasible size.
        /// </param>
        public IntegerSearcher(Random r, int feasibleSize, int infeasibleSize) : base(r)
        {
            this.FeasiblePopulation = new IntegerPopulation(true, feasibleSize);
            this.InfeasiblePopulation = new IntegerPopulation(false, infeasibleSize);
            this.Archive = new IntegerNovelArchive();

            for (var i = 0; i < feasibleSize; i++)
            {
                FeasiblePopulation.CurrentGeneration.Add(new IntegerSolution(Random.Next(50000)));
            }

            for (var i = 0; i < feasibleSize; i++)
            {
                InfeasiblePopulation.CurrentGeneration.Add(new IntegerSolution(-Random.Next(50000)));
            }
        }

        /// <summary>
        /// Runs a number of generations on this novelty searcher.
        /// </summary>
        /// <param name="generations">Number of generations to run.</param>
        public void RunGenerations(int generations)
        {
            Console.WriteLine("Generation 0");

            Console.WriteLine("-----------------");
            Console.WriteLine("Feasible Population");
            foreach (var i in FeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((IntegerSolution)i).Number);
            }

            Console.WriteLine("------------------");
            Console.WriteLine("Infeasible Population");
            foreach (var i in InfeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((IntegerSolution)i).Number);
            }

            Console.WriteLine("------------------");

            Console.WriteLine("-------------------");
            for (var i = 0; i < generations; i++)
            {
                Console.WriteLine("Generation " + i);

                this.NextGeneration();
            }
        }
    }
}
