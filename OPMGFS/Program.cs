namespace OPMGFS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    using OPMGFS.Evolution;
    using OPMGFS.Map;
    using OPMGFS.Map.CellularAutomata;
    using OPMGFS.Novelty;
    using OPMGFS.Novelty.MapNoveltySearch;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// The class that runs the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Initializes the program.
        /// </summary>
        /// <param name="args"> The args. </param>
        public static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth - 40, Console.WindowHeight + 40);

            const int RandomGeneratorSeed = 124;

            const int BaseMapStartSeed = 15;
            const int BaseMapEndSeed = 25;

            const int EvoGenerations = 10;
            const int EvoPopSize = 25;
            const int Parents = 6;
            const int Children = 18;

            const int MOEAGenerations = 10;
            const int MOEAPopSize = 25;

            const int NoveltyGenerations = 10;
            const int Feasible = 30;
            const int Infeasible = 30;
            const int Neighbours = 5;
            const int Archive = 5;

            var list = new List<int>();
            for (var i = BaseMapStartSeed; i < BaseMapEndSeed; i++)
                list.Add(i);

            var random = new Random(RandomGeneratorSeed);

            var sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Generating base maps.");
            var maps = GetBaseMaps(
                caRandomSeeds: list,
                baseMapFolder: string.Format("BaseMaps-start{0}-end{1}", BaseMapStartSeed, BaseMapEndSeed));
            Console.WriteLine("It took {0} milliseconds to generate base maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");

            var mapList = maps.Select(x => x.CreateFinishedMap(Enums.Half.Top, Enums.MapFunction.Turn)).ToList();
            MapHelper.SaveGreyscaleNoveltyMap(mapList, " BaseMaps", string.Format("BaseMaps-start{0}-end{1}", BaseMapStartSeed, BaseMapEndSeed));
            sw.Restart();

            Console.WriteLine("Starting evolution");
            RunEvolution(
                maps,
                random,
                numberOfGenerations: EvoGenerations,
                populationSize: EvoPopSize,
                numberOfParents: Parents,
                numberOfChildren: Children,
                folderName: string.Format("Evolution-gen{0}-pop{1}-par{2}-child{3}", EvoGenerations, EvoPopSize, Parents, Children));
            Console.WriteLine("Evolution done. It took  {0} milliseconds to perform evolution.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Starting multiobjective evolution");
            RunMultiobjectiveEvolution(
                maps,
                random,
                numberOfGenerations: MOEAGenerations,
                populationSize: MOEAPopSize,
                folderName: string.Format("MOEA-gen{0}-pop{1}", MOEAGenerations, MOEAPopSize));
            Console.WriteLine("Multiobjective evolution done. It took {0} milliseconds.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Starting novelty search.");
            var nso = new NoveltySearchOptions(
                feasiblePopulationSize: Feasible,
                infeasiblePopulationSize: Infeasible,
                numberOfNeighbours: Neighbours,
                addToArchive: Archive);
            RunNoveltySearch(
                maps,
                random,
                numberOfGenerations: 100,
                noveltySearchOptions: nso,
                folderName: string.Format("NoveltySearch-gen{0}-feas{1}-infeas{2}-neighbors{3}-addtoarch{4}", NoveltyGenerations, Feasible, Infeasible, Neighbours, Archive));
            Console.WriteLine("Novelty search done. It took {0} milliseconds to perform novelty search.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Starting evolution with highest fitness novel maps.");
            nso = new NoveltySearchOptions(feasiblePopulationSize: 90, infeasiblePopulationSize: 90);
            RunEvolutionWithNoveltyAsBase(
                maps,
                random,
                numberOfNoveltyGenerations: NoveltyGenerations,
                numberOfEvolutionGenerations: EvoGenerations,
                numberOfMOEAGenerations: MOEAGenerations,
                noveltySearchOptions: nso,
                evolutionPopulationSize: EvoPopSize,
                moeaPopulationSize: MOEAPopSize,
                numberOfParents: Children,
                numberOfChildren: Parents,
                folderName:
                    string.Format(
                        "NoEvHighFitness-noveltygen{0}-feas{1}-infeas{2}-evogen{3}-evopop{4}-evopar{5}-evochild{6}-moeagen{7}-moeapop{8}",
                        NoveltyGenerations,
                        Feasible,
                        Infeasible,
                        EvoGenerations,
                        EvoPopSize,
                        Parents,
                        Children,
                        MOEAGenerations,
                        MOEAPopSize));
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Starting evolution with highest novelty maps.");
            RunEvolutionWithNoveltyAsBase(
                maps,
                random,
                numberOfNoveltyGenerations: NoveltyGenerations,
                numberOfEvolutionGenerations: EvoGenerations,
                numberOfMOEAGenerations: MOEAGenerations,
                noveltySearchOptions: nso,
                evolutionPopulationSize: EvoPopSize,
                moeaPopulationSize: MOEAPopSize,
                numberOfParents: Children,
                numberOfChildren: Parents,
                folderName:
                    string.Format(
                        "NoEvHighNovelty-noveltygen{0}-feas{1}-infeas{2}-evogen{3}-evopop{4}-evopar{5}-evochild{6}-moeagen{7}-moeapop{8}",
                        NoveltyGenerations,
                        Feasible,
                        Infeasible,
                        EvoGenerations,
                        EvoPopSize,
                        Parents,
                        Children,
                        MOEAGenerations,
                        MOEAPopSize),
                selectHighestFitness: false);
            Console.WriteLine("Evolution with highest novelty maps. It took {0} milliseconds to perform evolution with highest novelty maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            Console.WriteLine("Everything is done running");
            Console.ReadKey();
        }

        #region Search Methods
        /// <summary>
        /// Runs evolution with the given settings.
        /// </summary>
        /// <param name="maps"> The maps to run evolution on. </param>
        /// <param name="r"> The random number generator. </param>
        /// <param name="mapSearchOptions"> The map search options. </param>
        /// <param name="mapFitnessOptions"> The map fitness options. </param>
        /// <param name="numberOfGenerations"> The number of generations to run. </param>
        /// <param name="populationSize"> The population size of the evolution. </param>
        /// <param name="numberOfParents"> The number of parents used in the evolution. </param>
        /// <param name="numberOfChildren"> The number of children spawned per generation. </param>
        /// <param name="mutationChance"> The chance of mutation happening. </param>
        /// <param name="selectionStrategy"> The selection strategy. </param>
        /// <param name="parentSelectionStrategy"> The parent selection strategy. </param>
        /// <param name="populationStrategy"> The population strategy. </param>
        /// <param name="folderName"> The folder to save results to in "Images/Finished Maps". </param>
        /// <param name="fileToWriteTo"> The file to write timings and fitness per generation to. </param>
        /// <param name="lowestFitnessLevelForPrint"> The fitness required before a map is printed. </param>
        public static void RunEvolution(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfGenerations = 10,
            int populationSize = 20,
            int numberOfParents = 6,
            int numberOfChildren = 16,
            double mutationChance = 0.3,
            Enums.SelectionStrategy selectionStrategy = Enums.SelectionStrategy.ChanceBased,
            Enums.SelectionStrategy parentSelectionStrategy = Enums.SelectionStrategy.ChanceBased,
            Enums.PopulationStrategy populationStrategy = Enums.PopulationStrategy.Mutation,
            string folderName = "MapEvolution",
            string fileToWriteTo = "EvolutionGenerationTimes.txt",
            double lowestFitnessLevelForPrint = double.MinValue)
        {
            var stringToWrite = new StringBuilder();
            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();
            var bestMaps = new List<EvolvableMap>();

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
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn).SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

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
                var bestMap = evolver.Evolve(stringToWrite);
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

                bestMaps.Add(evolver.Population.OrderByDescending(evoMap => evoMap.Fitness).ToList()[0]);
                MapHelper.SaveGreyscaleNoveltyMap(evolver.Population, string.Format("Base Map {0} NoveltyMap", baseMapCounter), folderName);

                Console.WriteLine("The map fitness values of base map {0} were: {1}", baseMapCounter, ((EvolvableMap)bestMap).MapFitnessValues);

                baseMapCounter++;
            }
            
            MapHelper.SaveGreyscaleNoveltyMap(bestMaps, string.Format(" Best Maps NoveltyMap"), folderName);
            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

        /// <summary>
        /// Runs MOEA on a set of maps.
        /// </summary>
        /// <param name="maps"> The maps to run evolution on. </param>
        /// <param name="r"> The random number generator. </param>
        /// <param name="mapSearchOptions"> The map search options. </param>
        /// <param name="mapFitnessOptions"> The map fitness options. </param>
        /// <param name="numberOfGenerations"> The number of generations to run. </param>
        /// <param name="populationSize"> The population size. </param>
        /// <param name="mutationChance"> The chance of mutation happening. </param>
        /// <param name="folderName"> The folder to save results to in "Images/Finished Maps". </param>
        /// <param name="fileToWriteTo"> The file to write timings and fitness per generation to. </param>
        /// <param name="lowestFitnessLevelForPrint"> The fitness required before a map is printed. </param>
        public static void RunMultiobjectiveEvolution(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfGenerations = 10,
            int populationSize = 25,
            double mutationChance = 0.3,
            string folderName = "MapMultiObjectiveEvolution",
            string fileToWriteTo = "MultiObjectiveEvolutionGenerationTimes.txt",
            double lowestFitnessLevelForPrint = double.MinValue)
        {
            var sb = new StringBuilder();
            var sw = new Stopwatch();

            var mso = mapSearchOptions ?? new MapSearchOptions(null);
            var mfo = mapFitnessOptions ?? new MapFitnessOptions();
            var bestMaps = new List<EvolvableMap>();
            var baseMapCounter = 0;
            foreach (var map in maps)
            {
                sw.Restart();
                sb.AppendLine(string.Format("Starting evolution for base map number {0}", baseMapCounter));

                var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
                var items = map.MapItems.Clone() as Enums.Item[,];
                var baseMap = new MapPhenotype(heightLevels, items);
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn).SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var evolver = new MultiObjectiveEvolver(
                    numberOfGenerations,
                    populationSize,
                    mutationChance,
                    r,
                    mso,
                    mfo);
                
                var bestMap = evolver.RunEvolution(sb);

                var variationValue = 0;

                foreach (var individual in evolver.Population)
                {
                    variationValue++;
                    if (individual.Fitness >= lowestFitnessLevelForPrint)
                    {
                        individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, individual.Fitness), folderName, false);
                    }
                }

                sb.AppendLine(string.Format("Evolution for base map number {0} took {1} ms", baseMapCounter, sw.ElapsedMilliseconds));
                sb.AppendLine();

                bestMaps.Add(evolver.Population.OrderByDescending(evoMap => evoMap.Fitness).ToList()[0]);
                MapHelper.SaveGreyscaleNoveltyMap(evolver.Population, string.Format("Base Map {0} NoveltyMap", baseMapCounter), folderName);
                Console.WriteLine("The map fitness values of base map {0} were: {1}", baseMapCounter, bestMap.MapFitnessValues);

                baseMapCounter++;
            }

            MapHelper.SaveGreyscaleNoveltyMap(bestMaps, string.Format(" Best Maps NoveltyMap"), folderName);
            WriteToTextFile(sb.ToString(), fileToWriteTo, folderName);
        }

        /// <summary>
        /// Runs novelty search on a given set of maps with the given settings.
        /// </summary>
        /// <param name="maps"> The maps to run evolution on. </param>
        /// <param name="r"> The random number generator. </param>
        /// <param name="mapSearchOptions"> The map search options. </param>
        /// <param name="noveltySearchOptions"> The novelty search options. </param>
        /// <param name="mapFitnessOptions"> The map fitness options. </param>
        /// <param name="numberOfGenerations"> The number of generations to run. </param>
        /// <param name="folderName"> The folder to save results to in "Images/Finished Maps". </param>
        /// <param name="fileToWriteTo"> The file to write timings and fitness per generation to. </param>
        /// <param name="lowestFitnessLevelForPrint"> The fitness required before a map is printed. </param>
        public static void RunNoveltySearch(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            NoveltySearchOptions noveltySearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfGenerations = 10,
            string folderName = "MapNovelty",
            string fileToWriteTo = "NoveltySearchGenerationTimes.txt",
            double lowestFitnessLevelForPrint = double.MinValue)
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
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn).SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var searcher = new MapSearcher(r, mso, nso);

                searcher.RunGenerations(numberOfGenerations, stringToWrite);

                var archiveMaps = new List<MapPhenotype>();
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

                    archiveMaps.Add(individual.ConvertedPhenotype);
                }

                stringToWrite.AppendLine(string.Format("It took {0} ms to perform novelty search on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                stringToWrite.AppendLine();

                MapHelper.SaveGreyscaleNoveltyMap(archiveMaps, string.Format("Base Map {0} NoveltyMap", baseMapCounter), folderName);

                baseMapCounter++;
            }

            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

        /// <summary>
        /// Runs standard evolution and/or MOEA seeded with maps found by the constrained novelty search.
        /// </summary>
        /// <param name="maps"> The maps to run evolution on. </param>
        /// <param name="r"> The random number generator. </param>
        /// <param name="mapSearchOptions"> The map search options. </param>
        /// <param name="noveltySearchOptions"> The novelty search options. </param>
        /// <param name="mapFitnessOptions"> The map fitness options. </param>
        /// <param name="numberOfNoveltyGenerations"> The number of generations to run for the novelty search. </param>
        /// <param name="numberOfEvolutionGenerations"> The number of generations to run for the standard evolution. </param>
        /// <param name="numberOfMOEAGenerations"> The number of generations to run for the MOEA. </param>
        /// <param name="evolutionPopulationSize"> The population size of the standard evolution. </param>
        /// <param name="numberOfParents"> The number of parents for the standard evolution. </param>
        /// <param name="numberOfChildren"> The number of children spawned per generation in the standard evolution. </param>
        /// <param name="evolutionMutationChance"> The chance of mutation happening during evolution. </param>
        /// <param name="moeaPopulationSize"> The population size of the MOEA. </param>
        /// <param name="moeaMutationChance"> The chance of mutation happening in the MOEA. </param>
        /// <param name="selectionStrategy"> The selection strategy used in the standard evolution. </param>
        /// <param name="parentSelectionStrategy"> The parent selection strategy used in the standard evolution. </param>
        /// <param name="populationStrategy"> The population strategy used in the standard evolution. </param>
        /// <param name="folderName"> The folder to save results to in "Images/Finished Maps". </param>
        /// <param name="fileToWriteTo"> The file to write timings and fitness per generation to. </param>
        /// <param name="selectHighestFitness"> Determines if the maps for seeding are chosen by highest fitness or highest novelty. </param>
        /// <param name="lowestFitnessLevelForPrint"> The fitness required before a map is printed. </param>
        /// <param name="runEvo"> Determines if the standard evolution should be run. </param>
        /// <param name="runMOEA"> Determines if the MOEA should be run. </param>
        public static void RunEvolutionWithNoveltyAsBase(
            List<MapPhenotype> maps,
            Random r,
            MapSearchOptions mapSearchOptions = null,
            NoveltySearchOptions noveltySearchOptions = null,
            MapFitnessOptions mapFitnessOptions = null,
            int numberOfNoveltyGenerations = 10,
            int numberOfEvolutionGenerations = 10,
            int numberOfMOEAGenerations = 10,
            int evolutionPopulationSize = 20,
            int numberOfParents = 6,
            int numberOfChildren = 16,
            double evolutionMutationChance = 0.3,
            int moeaPopulationSize = 25,
            double moeaMutationChance = 0.3,
            Enums.SelectionStrategy selectionStrategy = Enums.SelectionStrategy.ChanceBased,
            Enums.SelectionStrategy parentSelectionStrategy = Enums.SelectionStrategy.ChanceBased,
            Enums.PopulationStrategy populationStrategy = Enums.PopulationStrategy.Mutation,
            string folderName = "MapNoveltyEvolution",
            string fileToWriteTo = "NoveltyEvolutionHighestFitnessSearchGenerationTimes.txt",
            bool selectHighestFitness = true,
            double lowestFitnessLevelForPrint = double.MinValue,
            bool runEvo = true,
            bool runMOEA = true)
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
                baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Turn).SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                mso = new MapSearchOptions(map, mso);
                var searcher = new MapSearcher(r, mso, nso);

                searcher.RunGenerations(numberOfNoveltyGenerations, stringToWrite);

                listOfArchives.Add(searcher.Archive.Archive);

                stringToWrite.AppendLine(string.Format("It took {0} ms to perform novelty search on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                stringToWrite.AppendLine();

                baseMapCounter++;
            }

            baseMapCounter = 0;

            sw.Restart();
            var solutions = new List<List<EvolvableMap>>();

            if (selectHighestFitness)
            {
                stringToWrite.AppendLine(string.Format("Sorting initial population based by highest fitness."));
                foreach (var map in maps)
                {
                    var evolvableMaps = new List<EvolvableMap>();
                    var archive = listOfArchives[baseMapCounter];

                    foreach (var solution in archive)
                    {
                        var ms = (MapSolution)solution;
                        mso = new MapSearchOptions(map, mso);
                        var evolvableMap = new EvolvableMap(mso, evolutionMutationChance, r, mfo, ms.MapPoints);
                        evolvableMap.CalculateFitness();
                        evolvableMaps.Add(evolvableMap);
                    }

                    solutions.Add(evolvableMaps.OrderByDescending(s => s.Fitness).ToList());

                    baseMapCounter++;
                }

                stringToWrite.AppendLine(string.Format("It took {0} ms to find maps with the highest fitness.", sw.ElapsedMilliseconds));
                sw.Restart();
            }
            else
            {
                stringToWrite.AppendLine(string.Format("Sorting initial population based by highest novelty."));
                foreach (var map in maps)
                {
                    var evolvableMaps = new List<EvolvableMap>();
                    var archive = listOfArchives[baseMapCounter];

                    var tempArchive = archive.OrderByDescending(s => s.Novelty);

                    foreach (var solution in tempArchive)
                    {
                        var ms = (MapSolution)solution;
                        mso = new MapSearchOptions(map, mso);
                        var evolvableMap = new EvolvableMap(mso, evolutionMutationChance, r, mfo, ms.MapPoints);
                        evolvableMap.CalculateFitness();
                        evolvableMaps.Add(evolvableMap);
                    }

                    solutions.Add(evolvableMaps);

                    baseMapCounter++;
                }

                stringToWrite.AppendLine(string.Format("It took {0} ms to find maps with the highest novelty.", sw.ElapsedMilliseconds));
                sw.Restart();
            }
            
            // Evolution
            if (runEvo)
            {
                sw.Restart();
                var bestMaps = new List<EvolvableMap>();
                baseMapCounter = 0;
                foreach (var map in maps)
                {
                    sw.Restart();
                    stringToWrite.AppendLine(string.Format("Performing evolution on base map number {0}.", baseMapCounter));
                    mso = new MapSearchOptions(map, mso);
                    var evolver = new Evolver<EvolvableMap>(
                        numberOfEvolutionGenerations,
                        evolutionPopulationSize,
                        numberOfParents,
                        numberOfChildren,
                        evolutionMutationChance,
                        r,
                        new object[] { mso, evolutionMutationChance, r, mfo })
                    {
                        PopulationSelectionStrategy = selectionStrategy,
                        ParentSelectionStrategy = parentSelectionStrategy,
                        PopulationStrategy = populationStrategy
                    };

                    evolver.Initialize(solutions[baseMapCounter].Take(evolutionPopulationSize), stringToWrite);
                    evolver.Evolve(stringToWrite);
                    var variationValue = 0;

                    foreach (var individual in evolver.Population)
                    {
                        variationValue++;
                        if (individual.Fitness >= lowestFitnessLevelForPrint)
                        {
                            individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("Evo_Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, individual.Fitness), folderName, false);
                        }
                    }

                    stringToWrite.AppendLine(string.Format("It took {0} ms to perform evolution on base map number {1}.", sw.ElapsedMilliseconds, baseMapCounter));
                    stringToWrite.AppendLine();

                    bestMaps.Add(evolver.Population.OrderByDescending(evoMap => evoMap.Fitness).ToList()[0]);
                    MapHelper.SaveGreyscaleNoveltyMap(evolver.Population, string.Format("Evo_Base Map {0} NoveltyMap", baseMapCounter), folderName);

                    baseMapCounter++;
                }

                MapHelper.SaveGreyscaleNoveltyMap(bestMaps, string.Format("Evo_Best Maps NoveltyMap"), folderName);
            }
            
            // MOEA
            if (runMOEA)
            {
                baseMapCounter = 0;
                var bestMaps = new List<EvolvableMap>();
                foreach (var map in maps)
                {
                    sw.Restart();
                    stringToWrite.AppendLine(string.Format("Performing multi objective evolution for base map number {0}", baseMapCounter));

                    var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
                    var items = map.MapItems.Clone() as Enums.Item[,];
                    var baseMap = new MapPhenotype(heightLevels, items);
                    baseMap.CreateCompleteMap(Enums.Half.Top, Enums.MapFunction.Mirror);
                    baseMap.SaveMapToPngFile(string.Format("Base Map {0}", baseMapCounter), folderName, false);

                    mso = new MapSearchOptions(map, mso);
                    var evolver = new MultiObjectiveEvolver(
                        numberOfMOEAGenerations,
                        moeaPopulationSize,
                        moeaMutationChance,
                        r,
                        mso,
                        mfo);

                    evolver.RunEvolution(stringToWrite, solutions[baseMapCounter].Take(moeaPopulationSize));

                    var variationValue = 0;

                    foreach (var individual in evolver.Population)
                    {
                        variationValue++;
                        if (individual.Fitness >= lowestFitnessLevelForPrint)
                        {
                            individual.ConvertedPhenotype.SaveMapToPngFile(string.Format("MOEA_Base Map {0}_Map {1}_Fitness {2}", baseMapCounter, variationValue, individual.Fitness), folderName, false);
                        }
                    }

                    stringToWrite.AppendLine(string.Format("It took {0} ms to perform multi objective evolution on base map number {1}", sw.ElapsedMilliseconds, baseMapCounter));
                    stringToWrite.AppendLine();

                    bestMaps.Add(evolver.Population.OrderByDescending(evoMap => evoMap.Fitness).ToList()[0]);
                    MapHelper.SaveGreyscaleNoveltyMap(evolver.Population, string.Format("MOEA_Base Map {0} NoveltyMap", baseMapCounter), folderName);

                    baseMapCounter++;
                }

                MapHelper.SaveGreyscaleNoveltyMap(bestMaps, string.Format("MOEA_Best Maps NoveltyMap"), folderName);
            }

            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

        /// <summary>
        /// Creates base maps using a cellular automaton with the given settings.
        /// </summary>
        /// <param name="mapSize"> The height and width of the base maps. </param>
        /// <param name="oddsOfHeight"> The chance of height1 happening. </param>
        /// <param name="oddsOfHeight2"> The chance of height2 happening.</param>
        /// <param name="maxRangeToGroupPoints"> The max range to the group points. </param>
        /// <param name="groupPoints"> The number of points where terrain should be grouped during the initial seeding. </param>
        /// <param name="generateHeight2"> Determines if the cellular automata should generate height2 or not. </param>
        /// <param name="caRandomSeeds"> The random seeds to use for the CA's random generator. </param>
        /// <param name="caRuleset"> The ruleset to use by the CA. If left as null, the default ruleset is used. </param>
        /// <param name="sections"> The number of impassable terrain sections. </param>
        /// <param name="maxLength"> The max length of impassable terrain sections. </param>
        /// <param name="placementIntervals"> The interval at which areas are placed in the impassable terrain section. </param>
        /// <param name="maxPathNoiseDisplacement"> The max displacement for an area in the impassable terrain.</param>
        /// <param name="maxWidth"> The max width of a point. </param>
        /// <param name="caGenerations"> The number of generations run by the CA. </param>
        /// <param name="generateHeight2ThroughRules"> Determines if height2 should be generated through rules or not.</param>
        /// <param name="smoothingNormalNeighbourhood"> If the number of neighbours in the normal Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingExtNeighbourhood"> If the number of neighbours in the extended Moore neighbourhood is less than or equal to this number, smoothing happens. </param>
        /// <param name="smoothingGenerations"> The number of smoothing generations run. </param>
        /// <param name="smoothingRuleSet"> The ruleset used for smoothing. If left as null, the default smoothing ruleset will be used. </param>
        /// <param name="fileToWriteTo"> The file to write the timings to. </param>
        /// <param name="baseMapFolder"> The folder to print the base maps to in "Images/Finished Maps". </param>
        /// <returns> A list of base maps. </returns>
        public static List<MapPhenotype> GetBaseMaps(
            int mapSize = 128,
            double oddsOfHeight = 0.4,
            double oddsOfHeight2 = 0.2,
            int maxRangeToGroupPoints = 15,
            int groupPoints = 3,
            bool generateHeight2 = true,
            List<int> caRandomSeeds = null,
            List<Rule> caRuleset = null,
            int sections = 4, 
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
            string fileToWriteTo = "BaseMapGeneration.txt",
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

        /// <summary>
        /// Writes a string to a txt file.
        /// </summary>
        /// <param name="stringToWrite"> The string to write. </param>
        /// <param name="fileToWriteTo"> The name of the file to write to. </param>
        /// <param name="folder"> The folder to write to in "Images/Finished Maps". </param>
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

        public static void GenerateTranslatedMaps(int amountToGenerate, int seedStart)
        {
            var sw = new Stopwatch();
            for (var i = seedStart; i < seedStart + amountToGenerate; i++)
            {
                sw.Restart();
                var random123 = new Random(i);
                var ca = new CellularAutomata(64, 64, Enums.Half.Top, maxRangeToGroupPoint: 7, r: random123);
                ca.RunGenerations();
                ca.CreateImpassableTerrain(maxLength: 25, maxPathNoiseDisplacement: 1, maxWidth: 2);

                var map = new MapPhenotype(ca.Map, new Enums.Item[64, 64]);
                map.SmoothTerrain();

                var newMap = MapHelper.IncreaseSizeOfMap(map, 2);
                newMap.SmoothTerrain(random: random123);
                newMap.PlaceCliffs();

                newMap.SmoothCliffs();
                newMap.UpdateCliffPositions(Enums.Half.Top);
                newMap.SaveMapToPngFile(string.Format("map {0}_4_translated_post_smooth_cliffs", i), itemMap: false);
                Console.WriteLine(sw.ElapsedMilliseconds);
            }

            Console.ReadKey();
        }
        #endregion
    }
}
