namespace OPMGFS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    using OPMGFS.Map;
    using OPMGFS.Map.CellularAutomata;
    using OPMGFS.Map.MapObjects;
    using OPMGFS.Novelty.MapNoveltySearch;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class Program
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public static void Main(string[] args)
        {
            ////Console.SetWindowPosition();
            Console.SetWindowSize(Console.LargestWindowWidth - 40, Console.WindowHeight + 40);
            ////TestEvolution();
            ////TestPhenotype();
            ////TestPhenotypeConversion();
            ////TestCA();
            ////TestMapNoveltySearch();
            TestFitness();

            Console.WriteLine("Everything is done running");
            Console.ReadKey();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        private static void TestFitness()
        {
            const int Height = 128;
            const int Width = 128;

            var ruleBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleBasicHeight1.AddCondition(6, Enums.HeightLevel.Height1);

            var ca = new CellularAutomata(Width, Height, Enums.Half.Top, generateHeight2: false);
            ca.SetRuleset(new List<Rule> { ruleBasicHeight1 });
            ca.RunGenerations(generateHeight2ThroughRules: false);

            var map = new MapPhenotype(ca.Map, new Enums.Item[Width, Height]);
            map.SmoothTerrain();
            map.PlaceCliffs();

            var mapSolution = new MapSolution(new MapNoveltySearchOptions(map));
            mapSolution.MapPoints.Add(new MapPoint(0.5, 45, Enums.MapPointType.StartBase, Enums.WasPlaced.NotAttempted));
            mapSolution.MapPoints.Add(new MapPoint(0.5, 50, Enums.MapPointType.Ramp, Enums.WasPlaced.No));
            map = mapSolution.ConvertedPhenotype;

            map.SaveMapToPngFile();

            var mapFitness = new MapFitness(map);
            var sw = new Stopwatch();
            sw.Start();
            var fitness = mapFitness.CalculateFitness();
            sw.Stop();
            Console.WriteLine(fitness + " - " + sw.ElapsedMilliseconds);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        private static void TestCA()
        {
            const int Height = 128;
            const int Width = 128;
            const string Folder = "Test4";

            var ruleBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleBasicHeight1.AddCondition(6, Enums.HeightLevel.Height1);

            var ca = new CellularAutomata(Width, Height, Enums.Half.Top, generateHeight2: false);
            ca.SetRuleset(new List<Rule> { ruleBasicHeight1 });
            ////ca.SetRuleset(GetCARules());

            var map = new MapPhenotype(ca.Map, new Enums.Item[Width, Height]);
            string heights, items;
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(heights);

            ////ca.NextGeneration();
            ////map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            ////map.GetMapStrings(out heights, out items);
            ////Console.WriteLine(heights);
            
            map = new MapPhenotype(ca.Map, new Enums.Item[Width, Height]);
            map = map.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn);
            map.SaveMapToPngFile("1", Folder);
            Thread.Sleep(1000);

            ca.RunGenerations(generateHeight2ThroughRules: false);

            map = new MapPhenotype(ca.Map, new Enums.Item[Width, Height]);
            map = map.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn);
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(heights);

            map.SaveMapToPngFile("2", Folder);

            Thread.Sleep(1000);

            map.SmoothTerrain();
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(heights);

            map.SaveMapToPngFile("3", Folder);

            map.PlaceCliffs();

            Thread.Sleep(1000);

            map.SaveMapToPngFile("4", Folder);

            ////map = new MapPhenotype(ca.Map, new Enums.Item[width, height]);
            ////map = map.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
            ////map.GetMapStrings(out heights, out items);
            ////Console.WriteLine(heights);

            ////foreach (var nb in MapHelper.GetNeighbours(3, height - 3, map.HeightLevels, RuleEnums.Neighbourhood.MooreExtended))
            ////{
            ////    Console.WriteLine(nb.Key + " " + nb.Value);
            ////}

            ////ca.PlaceCliffs();
            ////map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            ////map.GetMapStrings(out heights, out items);
            ////Console.WriteLine(heights);

            ////ca.AddImpassableTerrain(3, 5);

            ////map = new MapPhenotype(ca.Map, new Enums.Item[50, 50]);
            ////map.GetMapStrings(out heights, out items);
            ////Console.WriteLine(heights);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        private static List<Rule> GetCARules()
        {
            var ruleExtBasicHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2)
                                          {
                                              Neighbourhood =
                                                  RuleEnums.Neighbourhood
                                                  .MooreExtended
                                          };
            ruleExtBasicHeight2.AddCondition(18, Enums.HeightLevel.Height1);

            var ruleExtBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height0)
            {
                Neighbourhood =
                    RuleEnums.Neighbourhood
                    .MooreExtended
            };
            ruleExtBasicHeight1.AddCondition(18, Enums.HeightLevel.Height1);

            var ruleExtAdvHeight2 = new RuleDeterministic(Enums.HeightLevel.Height2)
            {
                Neighbourhood =
                    RuleEnums.Neighbourhood
                    .MooreExtended
            };
            ruleExtAdvHeight2.AddCondition(18, Enums.HeightLevel.Height2);

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

            var ruleRemoveHeight0 = new RuleDeterministic(Enums.HeightLevel.Height1, Enums.HeightLevel.Height0);
            ruleRemoveHeight0.AddCondition(2, Enums.HeightLevel.Height0, RuleEnums.Comparison.LessThanEqualTo);

            var ruleList = new List<Rule>
                               {
                                   ruleExtBasicHeight2, ruleExtBasicHeight1, ruleExtAdvHeight2, ruleBasicHeight2, ruleBasicHeight1, ruleAdvHeight1, ruleRemoveHeight0
                               };
            return ruleList;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
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

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
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

        private static void TestMapNoveltySearch()
        {
            var folderString = "test 3";

            var ca = new CellularAutomata(128, 128, Enums.Half.Top);
            ca.SetRuleset(GetCARules());
            for (var i = 0; i < 10; i++)
            {
                ca.NextGeneration();
            }

            var map = new MapPhenotype(ca.Map, new Enums.Item[128, 128]);
            map.SmoothTerrain();

            map.PlaceCliffs();

            map.SaveMapToPngFile(folder: folderString);

            var ms = new MapSearcher(new Random(), 10, 10, new MapNoveltySearchOptions(map));

            ms.RunGenerations(5);

            foreach (var solution in ms.Archive.Archive)
            {
                ((MapSolution)solution).ConvertedPhenotype.SaveMapToPngFile(folder: folderString);
            }
        }

        private static void TestPhenotypeConversion()
        {
            var map = new MapPhenotype(64, 64);

            map.HeightLevels[1, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[2, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[3, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[1, 34] = Enums.HeightLevel.Impassable;
            map.HeightLevels[2, 34] = Enums.HeightLevel.Impassable;
            map.HeightLevels[3, 34] = Enums.HeightLevel.Impassable;
            map.HeightLevels[1, 33] = Enums.HeightLevel.Height0;
            map.HeightLevels[2, 33] = Enums.HeightLevel.Height0;
            map.HeightLevels[3, 33] = Enums.HeightLevel.Height0;
            map.HeightLevels[1, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[2, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[3, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[1, 31] = Enums.HeightLevel.Height1;
            map.HeightLevels[2, 31] = Enums.HeightLevel.Height1;
            map.HeightLevels[3, 31] = Enums.HeightLevel.Height1;
            map.HeightLevels[1, 30] = Enums.HeightLevel.Height1;
            map.HeightLevels[2, 30] = Enums.HeightLevel.Height1;
            map.HeightLevels[3, 30] = Enums.HeightLevel.Height1;
            map.HeightLevels[1, 29] = Enums.HeightLevel.Impassable;
            map.HeightLevels[2, 29] = Enums.HeightLevel.Impassable;
            map.HeightLevels[3, 29] = Enums.HeightLevel.Impassable;

            map.HeightLevels[58, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[59, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[58, 34] = Enums.HeightLevel.Height0;
            map.HeightLevels[59, 34] = Enums.HeightLevel.Height0;
            map.HeightLevels[58, 33] = Enums.HeightLevel.Height0;
            map.HeightLevels[59, 33] = Enums.HeightLevel.Height0;
            map.HeightLevels[58, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[59, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[58, 31] = Enums.HeightLevel.Height1;
            map.HeightLevels[59, 31] = Enums.HeightLevel.Height1;
            map.HeightLevels[58, 30] = Enums.HeightLevel.Impassable;
            map.HeightLevels[59, 30] = Enums.HeightLevel.Impassable;
            map.HeightLevels[58, 29] = Enums.HeightLevel.Impassable;
            map.HeightLevels[59, 29] = Enums.HeightLevel.Impassable;

            map.HeightLevels[32, 55] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 55] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 54] = Enums.HeightLevel.Height1;
            map.HeightLevels[33, 54] = Enums.HeightLevel.Height1;
            map.HeightLevels[32, 53] = Enums.HeightLevel.Height1;
            map.HeightLevels[33, 53] = Enums.HeightLevel.Height1;
            map.HeightLevels[32, 52] = Enums.HeightLevel.Cliff;
            map.HeightLevels[33, 52] = Enums.HeightLevel.Cliff;
            map.HeightLevels[32, 51] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 51] = Enums.HeightLevel.Height2;
            map.HeightLevels[32, 50] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 50] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 49] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 49] = Enums.HeightLevel.Impassable;
            
            map.HeightLevels[32, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 35] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 34] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 34] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 33] = Enums.HeightLevel.Height1;
            map.HeightLevels[33, 33] = Enums.HeightLevel.Height1;
            map.HeightLevels[32, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[33, 32] = Enums.HeightLevel.Cliff;
            map.HeightLevels[32, 31] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 31] = Enums.HeightLevel.Height2;
            map.HeightLevels[32, 30] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 30] = Enums.HeightLevel.Height2;
            map.HeightLevels[32, 29] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 29] = Enums.HeightLevel.Impassable;
            
            map.HeightLevels[1, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[1, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[1, 4] = Enums.HeightLevel.Impassable;
            map.HeightLevels[2, 2] = Enums.HeightLevel.Height0;
            map.HeightLevels[2, 3] = Enums.HeightLevel.Height0;
            map.HeightLevels[2, 4] = Enums.HeightLevel.Height0;
            map.HeightLevels[3, 2] = Enums.HeightLevel.Height0;
            map.HeightLevels[3, 3] = Enums.HeightLevel.Height0;
            map.HeightLevels[3, 4] = Enums.HeightLevel.Height0;
            map.HeightLevels[4, 2] = Enums.HeightLevel.Cliff;
            map.HeightLevels[4, 3] = Enums.HeightLevel.Cliff;
            map.HeightLevels[4, 4] = Enums.HeightLevel.Cliff;
            map.HeightLevels[5, 2] = Enums.HeightLevel.Height1;
            map.HeightLevels[5, 3] = Enums.HeightLevel.Height1;
            map.HeightLevels[5, 4] = Enums.HeightLevel.Height1;
            map.HeightLevels[6, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[6, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[6, 4] = Enums.HeightLevel.Impassable;
            map.HeightLevels[7, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[7, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[7, 4] = Enums.HeightLevel.Impassable;

            map.HeightLevels[31, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[31, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[31, 4] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 4] = Enums.HeightLevel.Impassable;
            map.HeightLevels[33, 2] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 3] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 4] = Enums.HeightLevel.Height2;
            map.HeightLevels[34, 2] = Enums.HeightLevel.Cliff;
            map.HeightLevels[34, 3] = Enums.HeightLevel.Cliff;
            map.HeightLevels[34, 4] = Enums.HeightLevel.Cliff;
            map.HeightLevels[35, 2] = Enums.HeightLevel.Height1;
            map.HeightLevels[35, 3] = Enums.HeightLevel.Height1;
            map.HeightLevels[35, 4] = Enums.HeightLevel.Height1;
            map.HeightLevels[36, 2] = Enums.HeightLevel.Height1;
            map.HeightLevels[36, 3] = Enums.HeightLevel.Height1;
            map.HeightLevels[36, 4] = Enums.HeightLevel.Height1;
            map.HeightLevels[37, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[37, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[37, 4] = Enums.HeightLevel.Impassable;

            map.HeightLevels[31, 22] = Enums.HeightLevel.Impassable;
            map.HeightLevels[31, 23] = Enums.HeightLevel.Impassable;
            map.HeightLevels[31, 24] = Enums.HeightLevel.Impassable;
            map.HeightLevels[32, 22] = Enums.HeightLevel.Height2;
            map.HeightLevels[32, 23] = Enums.HeightLevel.Height2;
            map.HeightLevels[32, 24] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 22] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 23] = Enums.HeightLevel.Height2;
            map.HeightLevels[33, 24] = Enums.HeightLevel.Height2;
            map.HeightLevels[34, 22] = Enums.HeightLevel.Cliff;
            map.HeightLevels[34, 23] = Enums.HeightLevel.Cliff;
            map.HeightLevels[34, 24] = Enums.HeightLevel.Cliff;
            map.HeightLevels[35, 22] = Enums.HeightLevel.Height1;
            map.HeightLevels[35, 23] = Enums.HeightLevel.Height1;
            map.HeightLevels[35, 24] = Enums.HeightLevel.Height1;
            map.HeightLevels[36, 22] = Enums.HeightLevel.Impassable;
            map.HeightLevels[36, 23] = Enums.HeightLevel.Impassable;
            map.HeightLevels[36, 24] = Enums.HeightLevel.Impassable;
            map.HeightLevels[37, 22] = Enums.HeightLevel.Impassable;
            map.HeightLevels[37, 23] = Enums.HeightLevel.Impassable;
            map.HeightLevels[37, 24] = Enums.HeightLevel.Impassable;

            map.HeightLevels[51, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[51, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[51, 4] = Enums.HeightLevel.Impassable;
            map.HeightLevels[52, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[52, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[52, 4] = Enums.HeightLevel.Impassable;
            map.HeightLevels[53, 2] = Enums.HeightLevel.Height0;
            map.HeightLevels[53, 3] = Enums.HeightLevel.Height0;
            map.HeightLevels[53, 4] = Enums.HeightLevel.Height0;
            map.HeightLevels[54, 2] = Enums.HeightLevel.Cliff;
            map.HeightLevels[54, 3] = Enums.HeightLevel.Cliff;
            map.HeightLevels[54, 4] = Enums.HeightLevel.Cliff;
            map.HeightLevels[55, 2] = Enums.HeightLevel.Height1;
            map.HeightLevels[55, 3] = Enums.HeightLevel.Height1;
            map.HeightLevels[55, 4] = Enums.HeightLevel.Height1;
            map.HeightLevels[56, 2] = Enums.HeightLevel.Height1;
            map.HeightLevels[56, 3] = Enums.HeightLevel.Height1;
            map.HeightLevels[56, 4] = Enums.HeightLevel.Height1;
            map.HeightLevels[57, 2] = Enums.HeightLevel.Impassable;
            map.HeightLevels[57, 3] = Enums.HeightLevel.Impassable;
            map.HeightLevels[57, 4] = Enums.HeightLevel.Impassable;

            var mnso = new MapNoveltySearchOptions(map);

            ////var ms = new MapSearcher(new Random(), 5, 5, mnso);

            ////var solution = (MapSolution)ms.FeasiblePopulation.CurrentGeneration[0];
            var solution = new MapSolution(
                mnso,
                new List<MapPoint>
                    {
                        new MapPoint(0.9, 90, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                                                   new MapPoint(0.2, 90, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                                                   new MapPoint(1, 180, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                                                   new MapPoint(0.8, 0, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes),
                                                   new MapPoint(0.9, 270, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                                                   new MapPoint(0.2, 270, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                                                   new MapPoint(1, 225, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                                                   new MapPoint(0.8, 315, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes)
                                               });

            solution.ConvertToPhenotype(map);
            string heights, items;
            map.GetMapStrings(out heights, out items);
            Console.WriteLine(heights);
            Console.WriteLine(solution);
            map.SaveMapToPngFile();
            ////ms.RunGenerations(1);
        }
    }
}
