using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPMGFS
{
    using OPMGFS.Evolution;
    using OPMGFS.Novelty.IntegerNoveltySearch;

    class Program
    {
        static void Main(string[] args)
        {
            TestEvolution();
        }

        private static void TestEvolution()
        {
            var evolverTest = new Evolver<EvolvableDoubleArray>(10, 10, 2, 10, 0.3);
            //evolverTest.ParentSelectionStrategy = Options.SelectionStrategy.ChanceBased;
            //evolverTest.PopulationSelectionStrategy = Options.SelectionStrategy.ChanceBased;
            //evolverTest.PopulationStrategy = Options.PopulationStrategy.Recombination;

            //for (var i = 0; i < evolverTest.Population.Count; i++)
            //{
            //    Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
            //    //foreach (var number in evolvableDoubleArray.Numbers)
            //    //{
            //    //    Console.Write(number + ", ");
            //    //}
            //    Console.Write("]");
            //    Console.WriteLine();
            //}

            //Console.WriteLine();
            //Console.WriteLine("------------------");
            //Console.WriteLine("Starting Evolution");
            //Console.WriteLine("------------------");
            //Console.WriteLine();

            //var best = (EvolvableDoubleArray)evolverTest.Evolve();

            //for (var i = 0; i < evolverTest.Population.Count; i++)
            //{
            //    Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
            //    //foreach (var number in evolvableDoubleArray.Numbers)
            //    //{
            //    //    Console.Write(number + ", ");
            //    //}
            //    Console.Write("]");
            //    Console.WriteLine();
            //}

            //Console.WriteLine();
            //Console.WriteLine("The best one had fitness " + best.Fitness + " and looked like this: [");
            //foreach (var number in best.Numbers)
            //{
            //    Console.Write(number + ", ");
            //}
            //Console.Write("]");

            //Console.ReadLine();

            var integersearcher  = new IntegerSearcher(20, 20);
            integersearcher.RunGenerations(10);
            Console.ReadKey();
        }
    }
}
