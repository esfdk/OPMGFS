namespace OPMGFS
{
    using System;
    using System.Collections.Generic;

    using OPMGFS.Map;
    using OPMGFS.Map.CellularAutomata;
    using OPMGFS.Map.MapObjects;
    using OPMGFS.Novelty.MapNoveltySearch;

    class Program
    {
        static void Main(string[] args)
        {
            ////Console.SetWindowPosition();
            Console.SetWindowSize(Console.LargestWindowWidth - 40, Console.WindowHeight + 40);
            ////TestEvolution();
            ////TestPhenotype();
            ////TestNovelty();
            TestCA();

            Console.ReadKey();
        }

        private static void TestCA()
        {
            var ca = new CellularAutomata(50, 50, Enums.Half.Top);
            ca.SetRuleset(getCARules());
            var map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            string heights, items;
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(heights);

            ////ca.NextGeneration();
            ////map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            ////map.GetMapStrings(out heights, out items);
            ////Console.WriteLine(heights);

            for (int generations = 0; generations < 25; generations++)
            {
                ca.NextGeneration();
            }

            map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(heights);

            foreach (var nb in MapHelper.GetNeighbours(3, 46, map.HeightLevels, RuleEnums.Neighbourhood.MooreExtended))
            {
                Console.WriteLine(nb.Key + " " + nb.Value);
            }

            ////ca.PlaceCliffs();
            ////map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            ////map.GetMapStrings(out heights, out items);
            ////Console.WriteLine(heights);

            //ca.AddImpassableTerrain(3, 5);

            //map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            //map.GetMapStrings(out heights, out items);
            //Console.WriteLine(heights);
        }

        private static List<Rule> getCARules()
        {
            var ruleExtBasicHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2)
                                          {
                                              Neighbourhood =
                                                  RuleEnums.Neighbourhood
                                                  .MooreExtended
                                          };
            ruleExtBasicHeight2.AddCondition(18, Enums.HeightLevel.Height1);

            var ruleBasicHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2);
            ruleBasicHeight2.AddCondition(5, Enums.HeightLevel.Height2);

            var ruleBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleBasicHeight1.AddCondition(5, Enums.HeightLevel.Height1);

            var ruleAdvHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height0);
            ruleAdvHeight1.AddCondition(3, Enums.HeightLevel.Height1);
            ruleAdvHeight1.AddCondition(3, Enums.HeightLevel.Height2);

            var ruleAdvHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2, Enums.HeightLevel.Height1);
            ruleAdvHeight2.AddCondition(3, Enums.HeightLevel.Height1);
            ruleAdvHeight2.AddCondition(3, Enums.HeightLevel.Height2);

            var ruleList = new List<Rule>
                               {
                                   ruleExtBasicHeight2
                                   , ruleBasicHeight2
                                   , ruleBasicHeight1
                                   //, ruleAdvHeight1
                                   //, ruleAdvHeight2
                               };
            return ruleList;
        }

        private static void TestPhenotype()
        {
            Console.WriteLine(Enums.Item.BlueMinerals);
            Console.WriteLine(Enums.GetCharValue(Enums.Item.BlueMinerals));
            Console.WriteLine(Enums.GetCharValue(Enums.HeightLevel.Cliff));

            const int XSize = 50;
            const int YSize = 30;

            var mapHeights = new Enums.HeightLevel[XSize, YSize];
            var mapItems = new Enums.Item[XSize, YSize];

            for (var i = 0; i <= 3; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    mapHeights[i, j] = Enums.HeightLevel.Height1;
                    if (j <= 4) mapHeights[i, j] = Enums.HeightLevel.Height2;
                }
            }

            ////mapHeights[10, 5] = Enums.HeightLevel.Cliff;

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
            ////mapItems[1, 4] = Enums.Item.Cliff;
            ////mapItems[2, 4] = Enums.Item.Cliff;
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
            ////mapItems[4, 8] = Enums.Item.Cliff;
            ////mapItems[4, 9] = Enums.Item.Cliff;

            mapHeights[5, 7] = Enums.HeightLevel.Cliff;
            mapHeights[6, 7] = Enums.HeightLevel.Cliff;

            var mapPhenotype = new MapPhenotype(mapHeights, mapItems);
            
            // printHeightLevels(mapPhenotype.HeightLevels);
            // ---------------------------
            // CreateCompleteMap Timing test
            // ---------------------------
            /*
            var sw = new Stopwatch();
            var turnTime = 0d;
            var mirrorTime = 0d;
            var iterations = 100;
            for (int i = 0; i < iterations; i++)
            {
                sw.Reset();
                sw.Start();
                mapPhenotype.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn);
                sw.Stop();
                turnTime += sw.ElapsedMilliseconds;

                sw.Reset();
                sw.Start();
                mapPhenotype.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
                sw.Stop();
                mirrorTime += sw.ElapsedMilliseconds;

                //Console.WriteLine("Iteration " + i + " turn took " + sw.ElapsedMilliseconds + " - Mirror took " + sw.ElapsedMilliseconds);
            }

            Console.WriteLine("Average mirror time: " + mirrorTime / iterations);
            Console.WriteLine("Average turn time:   " + turnTime / iterations);
            //printHeightLevels(mapPhenotype.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn).HeightLevels);

            //string heightLevels, items;
            //mapPhenotype.GetMapStrings(out heightLevels, out items);
            //Console.WriteLine(heightLevels);
            //Console.WriteLine(items);
             */

            // mapItems[1, 0] = Enums.Item.Cliff;
            // mapHeights[1, 1] = Enums.HeightLevel.Ramp01;
            // var neighbours = MapPathfinding.Neighbours(mapHeights, mapItems, new Tuple<int, int>(1, 1));

            // ---------------------------
            // Pathfinding test
            // ---------------------------
            /*
            var path = MapPathfinding.FindPathFromTo(mapHeights, new Tuple<int, int>(0, 0), new Tuple<int, int>(9, 0));


            foreach (var tuple in path)
            {
                Console.WriteLine(tuple.Item1 + ", " + tuple.Item2);
                mapItems[tuple.Item1, tuple.Item2] = Enums.Item.XelNagaTower;
            }

            string heightLevels, items;
            mapPhenotype.GetMapStrings(out heightLevels, out items);
            Console.WriteLine(heightLevels);
            Console.WriteLine(items);

            Console.WriteLine((int)Enums.Item.XelNagaTower);
            */
            //// ---------------------------

            // ---------------------------
            // CreateCompleteMap test
            // ---------------------------
            /*
            // Change the parameters here to test different functionalities
            var newMap = mapPhenotype.CreateCompleteMap(Enums.Half.Bottom, Enums.MapFunction.Mirror);

            string heightLevels, items;
            newMap.GetMapStrings(out heightLevels, out items);
            Console.WriteLine(heightLevels);
            Console.WriteLine(items);
             * */
        }

        private static void TestEvolution()
        {
            ////var evolverTest = new Evolver<EvolvableDoubleArray>(10, 10, 2, 10, 0.3);
            ////evolverTest.ParentSelectionStrategy = Options.SelectionStrategy.ChanceBased;
            ////evolverTest.PopulationSelectionStrategy = Options.SelectionStrategy.ChanceBased;
            ////evolverTest.PopulationStrategy = Options.PopulationStrategy.Recombination;

            ////for (var i = 0; i < evolverTest.Population.Count; i++)
            ////{
            ////    Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
            ////    //foreach (var number in evolvableDoubleArray.Numbers)
            ////    //{
            ////    //    Console.Write(number + ", ");
            ////    //}
            ////    Console.Write("]");
            ////    Console.WriteLine();
            ////}

            ////Console.WriteLine();
            ////Console.WriteLine("------------------");
            ////Console.WriteLine("Starting Evolution");
            ////Console.WriteLine("------------------");
            ////Console.WriteLine();

            ////var best = (EvolvableDoubleArray)evolverTest.Evolve();

            ////for (var i = 0; i < evolverTest.Population.Count; i++)
            ////{
            ////    Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
            ////    //foreach (var number in evolvableDoubleArray.Numbers)
            ////    //{
            ////    //    Console.Write(number + ", ");
            ////    //}
            ////    Console.Write("]");
            ////    Console.WriteLine();
            ////}

            ////Console.WriteLine();
            ////Console.WriteLine("The best one had fitness " + best.Fitness + " and looked like this: [");
            ////foreach (var number in best.Numbers)
            ////{
            ////    Console.Write(number + ", ");
            ////}
            ////Console.Write("]");

            ////Console.ReadLine();

            ////var integersearcher  = new IntegerSearcher(20, 20);
            ////integersearcher.RunGenerations(10);
        }

        private static void TestNovelty()
        {
            var ms = new MapSearcher(new Random(), 5, 5);

            ////var solution = (MapSolution)ms.FeasiblePopulation.CurrentGeneration[0];
            var solution = new MapSolution(new List<MapPoint>() { new MapPoint(0.5, 45, Enums.MapPointType.StartBase), new MapPoint(0.5, 135, Enums.MapPointType.Base) });
            var map = solution.ConvertToPhenotype(64, 64);
            string heights, items;
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(items);
            Console.WriteLine(solution);
            ////ms.RunGenerations(1);
        }
    }
}
