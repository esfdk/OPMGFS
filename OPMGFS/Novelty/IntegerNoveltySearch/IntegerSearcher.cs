namespace OPMGFS.Novelty.IntegerNoveltySearch
{
    using System;

    public class IntegerSearcher : NoveltySearcher
    {
        private Random random;

        public IntegerSearcher(int feasibleSize, int infeasibleSize)
        {
            this.FeasiblePopulation = new IntegerPopulation(true, feasibleSize);
            this.InfeasiblePopulation = new IntegerPopulation(false, infeasibleSize);
            this.Archive = new IntegerNovelArchive();
            random = new Random();

            for (var i = 0; i < feasibleSize; i++)
            {
                FeasiblePopulation.CurrentGeneration.Add(new IntegerSolution(random.Next(50000)));
            }

            for (var i = 0; i < feasibleSize; i++)
            {
                InfeasiblePopulation.CurrentGeneration.Add(new IntegerSolution(-random.Next(50000)));
            }
        }

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

        protected void NextGeneration()
        {
            var infeasibleIndividuals = FeasiblePopulation.AdvanceGeneration(
                new NoveltySearchOptions(),
                InfeasiblePopulation,
                Archive,
                random);
            var feasibleIndividuals = InfeasiblePopulation.AdvanceGeneration(
                new NoveltySearchOptions(),
                FeasiblePopulation,
                Archive,
                random);
            
            FeasiblePopulation.CurrentGeneration.AddRange(feasibleIndividuals);
            InfeasiblePopulation.CurrentGeneration.AddRange(infeasibleIndividuals);


            Console.WriteLine("-----------------");
            Console.WriteLine("Feasible Population");
            foreach (var i in FeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((IntegerSolution)i).Number + " Novelty: " + i.Novelty);
            }

            Console.WriteLine("------------------");
            Console.WriteLine("Infeasible Population");
            foreach (var i in InfeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((IntegerSolution)i).Number);
            }
            Console.WriteLine("------------------");
        }
    }
}
