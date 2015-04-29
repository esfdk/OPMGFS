namespace OPMGFS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using OPMGFS.Evolution;
    using OPMGFS.Map;
    using OPMGFS.Map.CellularAutomata;
    using OPMGFS.Map.MapObjects;
    using OPMGFS.Novelty;
    using OPMGFS.Novelty.MapNoveltySearch;

    using Position = System.Tuple<int, int>;

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
            ////TestFitness();
            ////TestPathfinding();

            var sw = new Stopwatch();
            sw.Start();

            var list = new List<int>();
            for (var i = 0; i < 2; i++)
            {
                list.Add(0);
            }

            Console.WriteLine("Generating base maps.");
            var maps = GetBaseMaps(caRandomSeeds: list);
            Console.WriteLine("It took {0} milliseconds to generate base maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();
            for(var i = 0; i < maps.Count; i++) maps[i].SaveMapToPngFile(string.Format("{0}", i));
            return;

            Console.WriteLine("Starting evolution");
            RunEvolution(maps, new Random(0), numberOfGenerations: 1, populationSize: 50, numberOfParents: 6, numberOfChildren: 18);
            Console.WriteLine("Evolution done. It took  {0} milliseconds to perform evolution.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Starting novelty search.");
            var nso = new NoveltySearchOptions(
                addToArchive: 5,
                feasiblePopulationSize: 50,
                infeasiblePopulationSize: 50);
            RunNoveltysearch(maps, new Random(0), numberOfGenerations: 2, noveltySearchOptions: nso);
            Console.WriteLine("Novelty search done. It took {0} milliseconds to perform novelty search.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Everything is done running");
            Console.ReadKey();
        }

        #region Test Methods
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        private static void TestPathfinding()
        {
            const int Height = 128;
            const int Width = 128;

            var map = new MapPhenotype(new Enums.HeightLevel[Width, Height], new Enums.Item[Width, Height]);
            map.HeightLevels[32, 32] = Enums.HeightLevel.Height1;
            for (int i = 0; i < 6; i++)
            {
                map.HeightLevels[29 + i, 29 + i] = Enums.HeightLevel.Height1;
                map.HeightLevels[93 + i, 93 + i] = Enums.HeightLevel.Height1;
            }

            map.PlaceCliffs();

            var mapSolution = new MapSolution(new MapSearchOptions(map), new NoveltySearchOptions());
            mapSolution.MapPoints.Add(new MapPoint(0.5, 45, Enums.MapPointType.StartBase, Enums.WasPlaced.NotAttempted));
            ////mapSolution.MapPoints.Add(new MapPoint(0.4, 50, Enums.MapPointType.Ramp, Enums.WasPlaced.NotAttempted));
            map = mapSolution.ConvertedPhenotype;

            map.HeightLevels[32, 19] = Enums.HeightLevel.Ramp01;
            map.HeightLevels[96, 108] = Enums.HeightLevel.Ramp01;

            ////map.SmoothTerrain();

            var start = new Position(32, 32);
            var end = new Position(96, 96);

            map.SaveMapToPngFile();

            var sw = new Stopwatch();
            sw.Start();
            var aStar = MapPathfinding.FindPathFromTo(map.HeightLevels, start, end);
            sw.Stop();
            Console.WriteLine("AStar route length: " + aStar.Count + " took " + sw.ElapsedMilliseconds + " milliseconds");
            sw.Restart();
            var jps = new JPSMapPathfinding(map.HeightLevels, map.DestructibleRocks);
            var jpsPath = jps.FindPathFromTo(start, end);
            sw.Stop();
            Console.WriteLine("JPS route length: " + jpsPath.Count + " took " + sw.ElapsedMilliseconds + " milliseconds");

            var map1 = (Enums.HeightLevel[,])map.HeightLevels.Clone();
            var map2 = (Enums.HeightLevel[,])map.HeightLevels.Clone();

            foreach (var tuple in aStar)
            {
                map1[tuple.Item1, tuple.Item2] = Enums.HeightLevel.Impassable;
            }

            foreach (var tuple in jpsPath)
            {
                map2[tuple.Item1, tuple.Item2] = Enums.HeightLevel.Impassable;
            }

            map = new MapPhenotype(map1, new Enums.Item[Width, Height]);
            map.SaveMapToPngFile();

            map = new MapPhenotype(map2, new Enums.Item[Width, Height]);
            map.SaveMapToPngFile();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        private static void TestFitness()
        {
            const int Height = 128;
            const int Width = 128;

            var ruleBasicHeight1 = new RuleDeterministic(Enums.HeightLevel.Height1);
            ruleBasicHeight1.AddCondition(6, Enums.HeightLevel.Height1);

            var ca = new CellularAutomata(Width, Height, Enums.Half.Top, generateHeight2: false, r: new Random(100000));
            ca.SetRuleset(new List<Rule> { ruleBasicHeight1 });
            ca.RunGenerations(generateHeight2ThroughRules: false);

            var map = new MapPhenotype((Enums.HeightLevel[,])ca.Map.Clone(), new Enums.Item[Width, Height]);

            map.SmoothTerrain();
            map.PlaceCliffs();

            var mapSolution = new MapSolution(new MapSearchOptions(map), new NoveltySearchOptions());
            mapSolution.MapPoints.Add(new MapPoint(0.5, 45, Enums.MapPointType.StartBase, Enums.WasPlaced.NotAttempted));
            mapSolution.MapPoints.Add(new MapPoint(0.75, 90, Enums.MapPointType.Base, Enums.WasPlaced.NotAttempted));
            mapSolution.MapPoints.Add(new MapPoint(0.5, 50, Enums.MapPointType.Ramp, Enums.WasPlaced.No));
            map = mapSolution.ConvertedPhenotype;

            map.SaveMapToPngFile();

            ////var start = new Position(32, 32);
            ////var end = new Position(96, 96);
            ////JPSMapPathfinding jps = new JPSMapPathfinding(map.HeightLevels);
            ////var path = jps.FindPathFromTo(start, end);
            ////foreach (var tuple in path)
            ////{
            ////    map.HeightLevels[tuple.Item1, tuple.Item2] = Enums.HeightLevel.Impassable;
            ////}

            ////map.HeightLevels[63, 65] = Enums.HeightLevel.Ramp12;

            ////map.SaveMapToPngFile();


            var mapFitness = new MapFitness(map, new MapFitnessOptions());
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

            var ca = new CellularAutomata(Width, Height, Enums.Half.Top, r: new Random(100000));

            var map = new MapPhenotype(ca.Map, new Enums.Item[Width, Height]);
            map.SaveMapToPngFile("1_original", Folder);

            ca.RunGenerations();
            map = new MapPhenotype(ca.Map, new Enums.Item[Width, Height]);
            map.SmoothTerrain();

            map = map.CreateFinishedMap(Enums.Half.Top, Enums.MapFunction.Turn);
            map.SaveMapToPngFile("Finished", Folder);


            ////map.SaveMapToPngFile("2_CA", Folder);

            ////map = map.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn);
            ////map.SaveMapToPngFile("3_smoothTerrain", Folder);

            ////map.PlaceCliffs();
            ////map.SaveMapToPngFile("4_cliffs", Folder);

            ////map = map.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn);
            ////map.SmoothCliffs();
            ////map.SaveMapToPngFile("5_smoothedCliffs", Folder);

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
            var evolverTest = new Evolver<EvolvableDoubleArray>(10, 10, 2, 10, 0.3, new Random());
            evolverTest.ParentSelectionStrategy = Enums.SelectionStrategy.ChanceBased;
            evolverTest.PopulationSelectionStrategy = Enums.SelectionStrategy.ChanceBased;
            evolverTest.PopulationStrategy = Enums.PopulationStrategy.Recombination;

            for (var i = 0; i < evolverTest.Population.Count; i++)
            {
                Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
                Console.Write("]");
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine("Starting Evolution");
            Console.WriteLine("------------------");
            Console.WriteLine();

            var best = (EvolvableDoubleArray)evolverTest.Evolve();

            for (var i = 0; i < evolverTest.Population.Count; i++)
            {
                Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
                //foreach (var number in evolvableDoubleArray.Numbers)
                //{
                //    Console.Write(number + ", ");
                //}
                Console.Write("]");
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("The best one had fitness " + best.Fitness + " and looked like this: [");
            foreach (var number in best.Numbers)
            {
                Console.Write(number + ", ");
            }
            Console.Write("]");

            Console.ReadLine();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
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

            var ms = new MapSearcher(new Random(), new MapSearchOptions(map), new NoveltySearchOptions());

            ms.RunGenerations(5);

            foreach (var solution in ms.Archive.Archive)
            {
                ((MapSolution)solution).ConvertedPhenotype.SaveMapToPngFile(folder: folderString);
            }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        private static void TestPhenotypeConversion()
        {
            var map = new MapPhenotype(128, 128);

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

            var mso = new MapSearchOptions(map);
            var nso = new NoveltySearchOptions();

            ////var ms = new MapSearcher(new Random(), 5, 5, mnso);

            ////var solution = (MapSolution)ms.FeasiblePopulation.CurrentGeneration[0];
            var solution = new MapSolution(
                mso,
                nso,
                new List<MapPoint>
                    {
                        ////new MapPoint(0.9, 135, Enums.MapPointType.StartBase, Enums.WasPlaced.Yes),
                        ////new MapPoint(0.2, 160, Enums.MapPointType.Base, Enums.WasPlaced.Yes),
                        new MapPoint(0.6, 90, Enums.MapPointType.GoldBase, Enums.WasPlaced.Yes),
                        new MapPoint(0.6, 90, Enums.MapPointType.DestructibleRocks, Enums.WasPlaced.Yes),
                        new MapPoint(0.4, 150, Enums.MapPointType.XelNagaTower, Enums.WasPlaced.Yes),
                        new MapPoint(0.2, 11, Enums.MapPointType.DestructibleRocks, Enums.WasPlaced.Yes),
                        ////new MapPoint(0.2, 90, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                        ////new MapPoint(1, 180, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                        ////new MapPoint(0.8, 0, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes),
                        ////new MapPoint(0.9, 270, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                        ////new MapPoint(0.2, 270, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                        ////new MapPoint(1, 225, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes), 
                        ////new MapPoint(0.8, 315, Enums.MapPointType.Ramp, Enums.WasPlaced.Yes)
                    });

            var newMap = solution.ConvertToPhenotype(map);
            newMap.SaveMapToPngFile();

            ((MapSolution)solution.Mutate(new Random(10))).ConvertToPhenotype(map).SaveMapToPngFile("-1");
            ((MapSolution)solution.Mutate(new Random(100))).ConvertToPhenotype(map).SaveMapToPngFile("-2");
            ((MapSolution)solution.Mutate(new Random(1000))).ConvertToPhenotype(map).SaveMapToPngFile("-3");
            ////ms.RunGenerations(1);
        }
        #endregion

        #region Search Methods
        public static void RunEvolution(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfGenerations = 10,
            int populationSize = 10,
            int numberOfParents = 3,
            int numberOfChildren = 8,
            double mutationChance = 0.3,
            Enums.SelectionStrategy selectionStrategy = Enums.SelectionStrategy.HighestFitness,
            Enums.SelectionStrategy parentSelectionStrategy = Enums.SelectionStrategy.HighestFitness,
            Enums.PopulationStrategy populationStrategy = Enums.PopulationStrategy.Mutation,
            string folderName = "MapEvolution",
            double lowestFitnessLevelForPrint = double.MinValue)
        {
            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();
            var baseMapCounter = 0;
            foreach (var map in maps)
            {
                var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
                var items = map.MapItems.Clone() as Enums.Item[,];
                var baseMap = new MapPhenotype(heightLevels, items);
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
                baseMap.SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var evolver = new Evolver<EvolvableMap>(
                    numberOfGenerations,
                    populationSize,
                    numberOfParents,
                    numberOfChildren,
                    mutationChance,
                    r,
                    new object[] { mso, mutationChance, r, mfo })
                                  {
                                      PopulationSelectionStrategy = selectionStrategy,
                                      ParentSelectionStrategy = parentSelectionStrategy,
                                      PopulationStrategy = populationStrategy
                                  };

                evolver.Evolve();
                var variationValue = 0;

                foreach (var individual in evolver.Population)
                {
                    variationValue++;
                    if (individual.Fitness >= lowestFitnessLevelForPrint)
                    {
                        individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, individual.Fitness), folderName, false);
                    }
                }

                baseMapCounter++;
            }
        }

        public static void RunNoveltysearch(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            NoveltySearchOptions noveltySearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfGenerations = 10,
            double lowestFitnessLevelForPrint = double.MinValue,
            string folderName = "MapNovelty")
        {
            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();
            var nso = noveltySearchOptions ?? new NoveltySearchOptions();
            var baseMapCounter = 0;
            foreach (var map in maps)
            {
                var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
                var items = map.MapItems.Clone() as Enums.Item[,];
                var baseMap = new MapPhenotype(heightLevels, items);
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
                baseMap.SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var searcher = new MapSearcher(r, mso, nso);

                searcher.RunGenerations(numberOfGenerations);

                var variationValue = 0;
                foreach (var solution in searcher.Archive.Archive)
                {
                    var individual = (MapSolution)solution;
                    variationValue++;
                    var fitness = new MapFitness(individual.ConvertedPhenotype, mfo).CalculateFitness();
                    if (fitness >= lowestFitnessLevelForPrint)
                    {
                        individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, fitness), folderName, false);
                    }
                    else
                    {
                        Console.WriteLine("fitness too low: " + fitness);
                    }
                }

                baseMapCounter++;
            }
        }

        public static List<MapPhenotype> GetBaseMaps(
            int mapSize = 128,
            double oddsOfHeight = 0.5,
            double oddsOfHeight2 = 0.25,
            int groupPoints = 0,
            bool generateHeight2 = true,
            List<int> caRandomSeeds = null,
            List<Rule> caRuleset = null,
            int? drops = null,
            int? radius = null,
            int caGenerations = 10,
            bool generateHeight2ThroughRules = true,
            int smoothingNormalNeighbourhood = 2,
            int smoothingExtNeighbourhood = 6,
            int smoothingGenerations = 10,
            List<Rule> smoothingRuleSet = null)
        {
            var baseMaps = new List<MapPhenotype>();

            CellularAutomata ca;

            if (caRandomSeeds == null || caRandomSeeds.Count == 0)
            {
                ca = new CellularAutomata(mapSize, mapSize, Enums.Half.Top, oddsOfHeight, oddsOfHeight2, groupPoints, generateHeight2);
                if (caRuleset != null)
                {
                    ca.SetRuleset(caRuleset);
                }

                if (drops != null && radius != null)
                {
                    ca.AddImpassableTerrain((int)drops, (int)radius);
                }

                ca.RunGenerations(caGenerations, generateHeight2ThroughRules);
                var map = new MapPhenotype(ca.Map, new Enums.Item[mapSize, mapSize]);
                map.SmoothTerrain(smoothingNormalNeighbourhood, smoothingExtNeighbourhood, smoothingGenerations, smoothingRuleSet);
                map.PlaceCliffs();
                map.UpdateCliffPositions(Enums.Half.Top);
                baseMaps.Add(map);
            }
            else
            {
                foreach (var seed in caRandomSeeds)
                {
                    var random = new Random(seed);
                    ca = new CellularAutomata(mapSize, mapSize, Enums.Half.Top, oddsOfHeight, oddsOfHeight2, groupPoints, generateHeight2, random);
                    if (caRuleset != null)
                    {
                        ca.SetRuleset(caRuleset);
                    }

                    if (drops != null && radius != null)
                    {
                        ca.AddImpassableTerrain((int)drops, (int)radius);
                    }

                    ca.RunGenerations(caGenerations, generateHeight2ThroughRules);
                    var map = new MapPhenotype(ca.Map, new Enums.Item[mapSize, mapSize]);
                    map.SmoothTerrain(smoothingNormalNeighbourhood, smoothingExtNeighbourhood, smoothingGenerations, smoothingRuleSet, random);
                    map.PlaceCliffs();
                    map.UpdateCliffPositions(Enums.Half.Top);
                    baseMaps.Add(map);
                }
            }

            return baseMaps;
        }
        #endregion
    }
}
