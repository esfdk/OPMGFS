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

            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mapHeights[i, j] = Enums.HeightLevel.Height1;
                    if (j <= 4) mapHeights[i, j] = Enums.HeightLevel.Height2;
                }
            }

            mapHeights[1, 4] = Enums.HeightLevel.Ramp12;
            mapHeights[2, 4] = Enums.HeightLevel.Ramp12;
            mapHeights[1, 5] = Enums.HeightLevel.Ramp12;
            mapHeights[2, 5] = Enums.HeightLevel.Ramp12;

            mapHeights[4, 7] = Enums.HeightLevel.Ramp01;
            mapHeights[4, 8] = Enums.HeightLevel.Ramp01;
            mapHeights[5, 7] = Enums.HeightLevel.Ramp01;
            mapHeights[5, 8] = Enums.HeightLevel.Ramp01;
            mapHeights[6, 7] = Enums.HeightLevel.Ramp01;
            mapHeights[6, 8] = Enums.HeightLevel.Ramp01;

            mapHeights[0, 4] = Enums.HeightLevel.Cliff;
            mapHeights[0, 5] = Enums.HeightLevel.Cliff;
            mapHeights[0, 6] = Enums.HeightLevel.Cliff;
            //mapItems[1, 4] = Enums.Item.Cliff;
            //mapItems[2, 4] = Enums.Item.Cliff;
            mapHeights[3, 4] = Enums.HeightLevel.Cliff;
            mapHeights[3, 5] = Enums.HeightLevel.Cliff;
            mapHeights[3, 6] = Enums.HeightLevel.Cliff;

            mapHeights[4, 0] = Enums.HeightLevel.Cliff;
            mapHeights[4, 1] = Enums.HeightLevel.Cliff;
            mapHeights[4, 2] = Enums.HeightLevel.Cliff;
            mapHeights[4, 3] = Enums.HeightLevel.Cliff;
            mapHeights[4, 4] = Enums.HeightLevel.Cliff;
            mapHeights[4, 5] = Enums.HeightLevel.Cliff;
            mapHeights[4, 6] = Enums.HeightLevel.Cliff;
            mapHeights[4, 7] = Enums.HeightLevel.Cliff;
            //mapItems[4, 8] = Enums.Item.Cliff;
            //mapItems[4, 9] = Enums.Item.Cliff;

            mapHeights[5, 7] = Enums.HeightLevel.Cliff;
            mapHeights[6, 7] = Enums.HeightLevel.Cliff;

            //mapItems[1, 0] = Enums.Item.Cliff;
            //mapHeights[1, 1] = Enums.HeightLevel.Ramp01;
            //var neighbours = MapPathfinding.Neighbours(mapHeights, mapItems, new Tuple<int, int>(1, 1));

            var path = MapPathfinding.FindPathFromTo(mapHeights, new Tuple<int, int>(0, 0), new Tuple<int, int>(9, 0));


            foreach (var tuple in path)
            {
                Console.WriteLine(tuple.Item1 + ", " + tuple.Item2);
                mapItems[tuple.Item1, tuple.Item2] = Enums.Item.XelNagaTower;
            }

            printHeightLevels(mapHeights);
            //Console.WriteLine("-------------");
            printMap(mapItems);
            //Console.WriteLine("-------------");

            //foreach (var neighbour in neighbours)
            //{
            //    Console.WriteLine(neighbour.Item1 + ", " + neighbour.Item2);
            //}


            /*
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
            printMap(newMap.MapItems);*/
        }

        private static void printHeightLevels(Enums.HeightLevel[,] heightLevels)
        {
            for (int i = 0; i < heightLevels.GetLength(0) + 2; i++)
                Console.Write("-");

            Console.WriteLine();
            for (var i = 0; i < heightLevels.GetLength(0); i++)
            {
                Console.Write("|");
                for (int j = 0; j < heightLevels.GetLength(1); j++)
                {
                    Console.Write((int)heightLevels[i, j]);
                }
                Console.Write("|");
                Console.WriteLine();
            }

            for (int i = 0; i < heightLevels.GetLength(0) + 2; i++)
                Console.Write("-");
            Console.WriteLine();
        }

        private static void printMap(Enums.Item[,] items)
        {
            for (int i = 0; i < items.GetLength(0) + 2; i++)
                Console.Write("-");

            Console.WriteLine();
            for (var i = 0; i < items.GetLength(0); i++)
            {
                Console.Write("|");
                for (int j = 0; j < items.GetLength(1); j++)
                {
                    Console.Write(Enums.GetItemCharValue(items[i, j]));
                }
                Console.Write("|");
                Console.WriteLine();
            }

            for (int i = 0; i < items.GetLength(0) + 2; i++)
                Console.Write("-");
            Console.WriteLine();
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
        }
    }
}
