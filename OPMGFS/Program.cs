using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPMGFS
{
    using OPMGFS.Evolution;
    using OPMGFS.Map;
    using OPMGFS.Novelty.IntegerNoveltySearch;

    class Program
    {
        static void Main(string[] args)
        {
            //TestEvolution();
            TestPhenotype();

            Console.ReadKey();
        }

        private static void TestPhenotype()
        {
            Console.WriteLine(Enums.Item.BlueMinerals);
            Console.WriteLine(Enums.GetItemCharValue(Enums.Item.BlueMinerals));

            var height = 10;
            var width = 10;

            var mapHeights = new Enums.HeightLevel[height, width];
            var mapItems = new Enums.Item[height, width];

            for (var i = 0; i < mapItems.GetLength(0); i++)
            {
                for (int j = 0; j < mapItems.GetLength(1); j++)
                {
                    mapItems[i, j] = Enums.Item.None;
                }
            }

            mapHeights[1, 1] = Enums.HeightLevel.Height1;
            mapHeights[4, 6] = Enums.HeightLevel.Height1;
            mapHeights[2, 4] = Enums.HeightLevel.Height1;
            mapHeights[1, 8] = Enums.HeightLevel.Height1;
            mapHeights[4, 3] = Enums.HeightLevel.Height1;

            mapItems[1, 1] = Enums.Item.Base;
            mapItems[0, 1] = Enums.Item.Gas;
            mapItems[1, 0] = Enums.Item.BlueMinerals;
            mapItems[1, 7] = Enums.Item.XelNagaTower;
            mapItems[4, 1] = Enums.Item.Cliff;

            var mp = new MapPhenotype(mapHeights, mapItems);

            // Change the parameters here to test different functionalities
            var newMap = mp.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror); 

            printHeightLevels(mapHeights);
            Console.WriteLine("-------------");
            printMap(mapItems);

            Console.WriteLine("-------------");
            Console.WriteLine("-------------");
            Console.WriteLine("-------------");

            printHeightLevels(newMap.HeightLevels);
            Console.WriteLine("-------------");
            printMap(newMap.MapItems);
        }

        private static void printHeightLevels(Enums.HeightLevel[,] heightLevels)
        {
            for (var i = 0; i < heightLevels.GetLength(0); i++)
            {
                for (int j = 0; j < heightLevels.GetLength(1); j++)
                {
                    Console.Write((int)heightLevels[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static void printMap(Enums.Item[,] items)
        {
            for (var i = 0; i < items.GetLength(0); i++)
            {
                for (int j = 0; j < items.GetLength(1); j++)
                {
                    Console.Write(Enums.GetItemCharValue(items[i, j]));
                }
                Console.WriteLine();
            }
        }

        private static void TestEvolution()
        {
            //var evolverTest = new Evolver<EvolvableDoubleArray>(10, 10, 2, 10, 0.3);
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

            //var integersearcher  = new IntegerSearcher(20, 20);
            //integersearcher.RunGenerations(10);
            
            
            Console.ReadKey();
        }
    }
}
