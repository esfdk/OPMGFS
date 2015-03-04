namespace OPMGFS.Novelty.IntegerNoveltySearch
{
    using System;

    public class IntegerSearcher : NoveltySearcher
    {
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

        protected override void NextGeneration()
        {
            var infeasibleIndividuals = FeasiblePopulation.AdvanceGeneration(
                new NoveltySearchOptions(),
                InfeasiblePopulation,
                Archive,
                Random);
            var feasibleIndividuals = InfeasiblePopulation.AdvanceGeneration(
                new NoveltySearchOptions(),
                FeasiblePopulation,
                Archive,
                Random);
            
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
