namespace OPMGFS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    using OPMGFS.Evolution;
    using OPMGFS.Map;
    using OPMGFS.Map.CellularAutomata;
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

            var sw = new Stopwatch();
            sw.Start();

            var list = new List<int>();
            for (var i = 0; i < 2; i++)
            {
                list.Add(i);
            }

            Console.WriteLine("Generating base maps.");
            var maps = GetBaseMaps(caRandomSeeds: list);
            Console.WriteLine("It took {0} milliseconds to generate base maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();
            for (var i = 0; i < maps.Count; i++) maps[i].SaveMapToPngFile(string.Format("{0}", i), itemMap: false);
            
            Console.WriteLine("Starting evolution");
            RunEvolution(maps, new Random(0), numberOfGenerations: 5, populationSize: 50, numberOfParents: 6, numberOfChildren: 18);
            Console.WriteLine("Evolution done. It took  {0} milliseconds to perform evolution.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();
            
           Console.WriteLine("Starting novelty search.");
            var nso = new NoveltySearchOptions(
                addToArchive: 5,
                feasiblePopulationSize: 50,
                infeasiblePopulationSize: 50);
            RunNoveltySearch(maps, new Random(0), numberOfGenerations: 5, noveltySearchOptions: nso);
            Console.WriteLine("Novelty search done. It took {0} milliseconds to perform novelty search.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();
            
            ////var baseMaps = GetBaseMaps(groupPoints: 4, caGenerations: 0, smoothingGenerations: 0, caRandomSeeds: new List<int> { 100001 });
            ////baseMaps[0].SaveMapToPngFile("baseMap1", "bm", itemMap: false);

            ////RunEvolutionWithNoveltyHighFitnessAsBase(GetBaseMaps(), new Random());

            ////TestEnclosedArea();

            Console.WriteLine("Everything is done running");
            Console.ReadKey();
        }

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
            string fileToWriteTo = "evolutionGenerationTimes.txt",
            double lowestFitnessLevelForPrint = double.MinValue)
        {
            var stringToWrite = new StringBuilder();
            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();

            var sw = new Stopwatch();
            sw.Start();
            var baseMapCounter = 0;
            foreach (var map in maps)
            {
                sw.Restart();
                stringToWrite.AppendLine(string.Format("Performing evolution on base map number {0}.", baseMapCounter));
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

                evolver.Initialize(stringToWrite);
                evolver.Evolve(stringToWrite);
                var variationValue = 0;

                foreach (var individual in evolver.Population)
                {
                    variationValue++;
                    if (individual.Fitness >= lowestFitnessLevelForPrint)
                    {
                        individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, individual.Fitness), folderName, false);
                    }
                }

                stringToWrite.AppendLine(string.Format("It took {0} ms to perform evolution on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                stringToWrite.AppendLine();

                baseMapCounter++;
            }

            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

        public static void RunNoveltySearch(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            NoveltySearchOptions noveltySearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfGenerations = 10,
            double lowestFitnessLevelForPrint = double.MinValue,
            string folderName = "MapNovelty",
            string fileToWriteTo = "noveltySearchGenerationTimes.txt")
        {
            var stringToWrite = new StringBuilder();

            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();
            var nso = noveltySearchOptions ?? new NoveltySearchOptions();

            var sw = new Stopwatch();
            sw.Start();
            var baseMapCounter = 0;
            foreach (var map in maps)
            {
                sw.Restart();
                stringToWrite.AppendLine(string.Format("Performing novelty search on base map number {0}.", baseMapCounter));
                var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
                var items = map.MapItems.Clone() as Enums.Item[,];
                var baseMap = new MapPhenotype(heightLevels, items);
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
                baseMap.SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var searcher = new MapSearcher(r, mso, nso);

                searcher.RunGenerations(numberOfGenerations, stringToWrite);

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

                stringToWrite.AppendLine(string.Format("It took {0} ms to perform novelty search on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                stringToWrite.AppendLine();

                baseMapCounter++;
            }

            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

        public static void RunEvolutionWithNoveltyHighFitnessAsBase(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            NoveltySearchOptions noveltySearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfNoveltyGenerations = 10,
            int numberOfEvolutionGenerations = 10,
            int populationSize = 10,
            int numberOfParents = 3,
            int numberOfChildren = 8,
            double mutationChance = 0.3,
            Enums.SelectionStrategy selectionStrategy = Enums.SelectionStrategy.HighestFitness,
            Enums.SelectionStrategy parentSelectionStrategy = Enums.SelectionStrategy.HighestFitness,
            Enums.PopulationStrategy populationStrategy = Enums.PopulationStrategy.Mutation,
            string folderName = "MapNoveltyEvolution",
            string fileToWriteTo = "NoveltyEvolutionHighestFitnessSearchGenerationTimes.txt",
            double lowestFitnessLevelForPrint = double.MinValue)
        {
            var stringToWrite = new StringBuilder();
            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();
            var nso = noveltySearchOptions ?? new NoveltySearchOptions();
            var listOfArchives = new List<List<Solution>>();
            
            // Novelty search
            var sw = new Stopwatch();
            sw.Start();
            var baseMapCounter = 0;
            foreach (var map in maps)
            {
                sw.Restart();
                stringToWrite.AppendLine(string.Format("Performing novelty search on base map number {0}.", baseMapCounter));
                var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
                var items = map.MapItems.Clone() as Enums.Item[,];
                var baseMap = new MapPhenotype(heightLevels, items);
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
                baseMap.SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var searcher = new MapSearcher(r, mso, nso);

                searcher.RunGenerations(numberOfNoveltyGenerations, stringToWrite);

                listOfArchives.Add(searcher.Archive.Archive);

                stringToWrite.AppendLine(string.Format("It took {0} ms to perform novelty search on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                stringToWrite.AppendLine();

                baseMapCounter++;
            }

            // Evolution
            sw.Restart();
            baseMapCounter = 0;
            foreach (var map in maps)
            {
                sw.Restart();
                stringToWrite.AppendLine(string.Format("Performing evolution on base map number {0}.", baseMapCounter));
                mso = new MapSearchOptions(map, mso);
                var evolver = new Evolver<EvolvableMap>(
                    numberOfEvolutionGenerations,
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

                var evolvableMaps = new List<EvolvableMap>();

                var archive = listOfArchives[baseMapCounter];

                foreach (var solution in archive)
                {
                    var ms = (MapSolution)solution;
                    var evolvableMap = new EvolvableMap(mso, mutationChance, r, mfo, ms.MapPoints);
                    evolvableMap.CalculateFitness();
                    evolvableMaps.Add(evolvableMap);
                }

                var solutions = new List<EvolvableMap>();

                while (solutions.Count < populationSize)
                {
                    var highestFitness = double.MinValue;
                    var index = -1;
                    
                    for (var i = 0; i < evolvableMaps.Count; i++)
                    {
                        if (evolvableMaps[i].Fitness > highestFitness)
                        {
                            highestFitness = evolvableMaps[i].Fitness;
                            index = i;
                        }
                    }

                    solutions.Add(evolvableMaps[index]);
                        
                    evolvableMaps.RemoveAt(index);
                }

                stringToWrite.AppendLine(string.Format("It took {0} ms to find maps with the highest fitness on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                sw.Restart();

                evolver.Initialize(solutions, stringToWrite);
                evolver.Evolve(stringToWrite);
                var variationValue = 0;

                foreach (var individual in evolver.Population)
                {
                    variationValue++;
                    if (individual.Fitness >= lowestFitnessLevelForPrint)
                    {
                        individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, individual.Fitness), folderName, false);
                    }
                }

                stringToWrite.AppendLine(string.Format("It took {0} ms to perform evolution on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                stringToWrite.AppendLine();

                baseMapCounter++;
            }

            // MOEA

            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

        public static List<MapPhenotype> GetBaseMaps(
            int mapSize = 128,
            double oddsOfHeight = 0.5,
            double oddsOfHeight2 = 0.25,
            int maxRangeToGroupPoints = 15,
            int groupPoints = 0,
            bool generateHeight2 = true,
            List<int> caRandomSeeds = null,
            List<Rule> caRuleset = null,
            int sections = 0, 
            int maxLength = 50, 
            double placementIntervals = 0.1, 
            int maxPathNoiseDisplacement = 3, 
            int maxWidth = 4,
            int caGenerations = 10,
            bool generateHeight2ThroughRules = true,
            int smoothingNormalNeighbourhood = 2,
            int smoothingExtNeighbourhood = 6,
            int smoothingGenerations = 10,
            List<Rule> smoothingRuleSet = null,
            string fileToWriteTo = "baseMapGeneration.txt",
            string baseMapFolder = "BaseMaps")
        {
            var stringToWrite = new StringBuilder();

            var baseMaps = new List<MapPhenotype>();

            CellularAutomata ca;
            var sw = new Stopwatch();
            sw.Start();

            if (caRandomSeeds == null || caRandomSeeds.Count == 0)
            {
                ca = new CellularAutomata(mapSize, mapSize, Enums.Half.Top, oddsOfHeight, oddsOfHeight2, maxRangeToGroupPoints, groupPoints, generateHeight2);
                if (caRuleset != null)
                {
                    ca.SetRuleset(caRuleset);
                }

                ca.CreateImpassableTerrain(sections, maxLength, placementIntervals, maxPathNoiseDisplacement, maxWidth);

                ca.RunGenerations(caGenerations, generateHeight2ThroughRules);
                var map = new MapPhenotype(ca.Map, new Enums.Item[mapSize, mapSize]);
                map.SmoothTerrain(smoothingNormalNeighbourhood, smoothingExtNeighbourhood, smoothingGenerations, smoothingRuleSet);
                map.PlaceCliffs();
                map.SmoothCliffs();
                map.UpdateCliffPositions(Enums.Half.Top);
                baseMaps.Add(map);

                stringToWrite.AppendLine(string.Format("It took {0} ms to generate the base map.", sw.ElapsedMilliseconds));
            }
            else
            {
                var i = 0;
                foreach (var seed in caRandomSeeds)
                {
                    sw.Restart();
                    
                    var random = new Random(seed);
                    ca = new CellularAutomata(mapSize, mapSize, Enums.Half.Top, oddsOfHeight, oddsOfHeight2, maxRangeToGroupPoints, groupPoints, generateHeight2, random);
                    if (caRuleset != null)
                    {
                        ca.SetRuleset(caRuleset);
                    }

                    ca.CreateImpassableTerrain(sections, maxLength, placementIntervals, maxPathNoiseDisplacement, maxWidth);

                    ca.RunGenerations(caGenerations, generateHeight2ThroughRules);
                    var map = new MapPhenotype(ca.Map, new Enums.Item[mapSize, mapSize]);
                    map.SmoothTerrain(smoothingNormalNeighbourhood, smoothingExtNeighbourhood, smoothingGenerations, smoothingRuleSet, random);
                    map.PlaceCliffs();
                    map.SmoothCliffs();
                    map.UpdateCliffPositions(Enums.Half.Top);
                    baseMaps.Add(map);
                    
                    stringToWrite.AppendLine(string.Format("It took {0} ms to generate base map number {1}.", sw.ElapsedMilliseconds, i));
                    i++;
                }
            }

            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, baseMapFolder);

            return baseMaps;
        }

        public static void WriteToTextFile(string stringToWrite, string fileToWriteTo, string folder = "")
        {
            var mapDir = Path.Combine(MapHelper.GetImageDirectory(), @"Finished Maps");
            if (!folder.Equals(string.Empty))
                mapDir = Path.Combine(mapDir, folder);
            Directory.CreateDirectory(mapDir);
            var file = new StreamWriter(Path.Combine(mapDir, fileToWriteTo));

            file.Write(stringToWrite);
            file.Close();
        }
        #endregion
    }
}
