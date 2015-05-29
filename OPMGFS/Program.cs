namespace OPMGFS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System.IO;
    using System.Linq;
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
            Console.SetWindowSize(Console.LargestWindowWidth - 40, Console.WindowHeight + 40);

            var sw = new Stopwatch();
            sw.Start();

            var list = new List<int>();
            for (var i = 15; i < 25; i++)
            {
                list.Add(i);
            }

            Console.WriteLine("Generating base maps.");
            var maps = GetBaseMaps(caRandomSeeds: list, baseMapFolder: string.Empty, fileToWriteTo: "baseMap-seeds(150-159).txt");
            Console.WriteLine("It took {0} milliseconds to generate base maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();

            NewNormalEvolutionTests(maps, sw);
            NewEvolutionTests(maps, sw);

            Console.WriteLine("Everything is done running");
            Console.ReadKey();
        }

        public static void NewNormalEvolutionTests(List<MapPhenotype> maps, Stopwatch sw)
        {
            var randomSeed = 124;

            var random = new Random(randomSeed);
            var folderString = "Evo A";

            var evolutionGenerations = 5;
            var evoPopSize = 5;
            var parents = 1;
            var children = 4;

            Console.WriteLine("Starting evolution {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            RunEvolution(
                maps,
                random,
                numberOfGenerations: evolutionGenerations,
                populationSize: evoPopSize,
                numberOfParents: parents,
                numberOfChildren: children,
                folderName: folderString);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "Evo B";

            evolutionGenerations = 15;
            evoPopSize = 15;
            parents = 3;
            children = 12;

            Console.WriteLine("Starting evolution {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            RunEvolution(
                maps,
                random,
                numberOfGenerations: evolutionGenerations,
                populationSize: evoPopSize,
                numberOfParents: parents,
                numberOfChildren: children,
                folderName: folderString);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "Evo C";

            evolutionGenerations = 45;
            evoPopSize = 5;
            parents = 1;
            children = 4;

            Console.WriteLine("Starting evolution {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            RunEvolution(
                maps,
                random,
                numberOfGenerations: evolutionGenerations,
                populationSize: evoPopSize,
                numberOfParents: parents,
                numberOfChildren: children,
                folderName: folderString);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "Evo D";

            evolutionGenerations = 25;
            evoPopSize = 10;
            parents = 2;
            children = 8;

            Console.WriteLine("Starting evolution {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            RunEvolution(
                maps,
                random,
                numberOfGenerations: evolutionGenerations,
                populationSize: evoPopSize,
                numberOfParents: parents,
                numberOfChildren: children,
                folderName: folderString);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "Evo H";

            evolutionGenerations = 50;
            evoPopSize = 5;
            parents = 1;
            children = 4;

            Console.WriteLine("Starting evolution {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            RunEvolution(
                maps,
                random,
                numberOfGenerations: evolutionGenerations,
                populationSize: evoPopSize,
                numberOfParents: parents,
                numberOfChildren: children,
                folderName: folderString);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "Evo I";

            evolutionGenerations = 50;
            evoPopSize = 10;
            parents = 2;
            children = 8;

            Console.WriteLine("Starting evolution {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            RunEvolution(
                maps,
                random,
                numberOfGenerations: evolutionGenerations,
                populationSize: evoPopSize,
                numberOfParents: parents,
                numberOfChildren: children,
                folderName: folderString);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();
        }

        public static void NewEvolutionTests(List<MapPhenotype> maps, Stopwatch sw)
        {
            NoveltySearchOptions nso;
            var randomSeed = 124;
            var txtFile = "EvolutionWithNoveltyFitness.txt";
            var selectHighestFitness = true;

            var random = new Random(randomSeed);
            var folderString = "EvoNov E + VI Nov";

            var noveltyGenerations = 10;
            var archive = 5;
            var feasible = 50;
            var infeasible = 50;

            var evolutionGenerations = 10;
            var evoPopSize = 25;
            var parents = 6;
            var children = 18;

            random = new Random(randomSeed);
            folderString = "EvoNov J + V Nov";

            noveltyGenerations = 20;
            archive = 1;
            feasible = 60;
            infeasible = 60;

            evolutionGenerations = 10;
            evoPopSize = 5;
            parents = 1;
            children = 4;

            Console.WriteLine("Starting evolution with highest fitness novel maps {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            nso = new NoveltySearchOptions(feasiblePopulationSize: feasible, infeasiblePopulationSize: infeasible, addToArchive: archive);
            RunEvolutionWithNoveltyAsBase(maps, random, runMOEA: false, selectHighestFitness: selectHighestFitness, numberOfNoveltyGenerations: noveltyGenerations, numberOfEvolutionGenerations: evolutionGenerations, noveltySearchOptions: nso, evolutionPopulationSize: evoPopSize, numberOfParents: parents, numberOfChildren: children, selectionStrategy: Enums.SelectionStrategy.ChanceBased, parentSelectionStrategy: Enums.SelectionStrategy.ChanceBased, folderName: folderString, fileToWriteTo: txtFile);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "EvoNov H + VI Nov";

            noveltyGenerations = 50;
            archive = 1;
            feasible = 60;
            infeasible = 60;

            evolutionGenerations = 50;
            evoPopSize = 5;
            parents = 1;
            children = 4;

            Console.WriteLine("Starting evolution with highest fitness novel maps {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            nso = new NoveltySearchOptions(feasiblePopulationSize: feasible, infeasiblePopulationSize: infeasible, addToArchive: archive);
            RunEvolutionWithNoveltyAsBase(maps, random, runMOEA: false, selectHighestFitness: selectHighestFitness, numberOfNoveltyGenerations: noveltyGenerations, numberOfEvolutionGenerations: evolutionGenerations, noveltySearchOptions: nso, evolutionPopulationSize: evoPopSize, numberOfParents: parents, numberOfChildren: children, selectionStrategy: Enums.SelectionStrategy.ChanceBased, parentSelectionStrategy: Enums.SelectionStrategy.ChanceBased, folderName: folderString, fileToWriteTo: txtFile);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "EvoNov I + VII Nov";

            noveltyGenerations = 50;
            archive = 1;
            feasible = 90;
            infeasible = 90;

            evolutionGenerations = 50;
            evoPopSize = 10;
            parents = 2;
            children = 8;

            Console.WriteLine("Starting evolution with highest fitness novel maps {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            nso = new NoveltySearchOptions(feasiblePopulationSize: feasible, infeasiblePopulationSize: infeasible, addToArchive: archive);
            RunEvolutionWithNoveltyAsBase(maps, random, runMOEA: false, selectHighestFitness: selectHighestFitness, numberOfNoveltyGenerations: noveltyGenerations, numberOfEvolutionGenerations: evolutionGenerations, noveltySearchOptions: nso, evolutionPopulationSize: evoPopSize, numberOfParents: parents, numberOfChildren: children, selectionStrategy: Enums.SelectionStrategy.ChanceBased, parentSelectionStrategy: Enums.SelectionStrategy.ChanceBased, folderName: folderString, fileToWriteTo: txtFile);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();




            random = new Random(randomSeed);
            folderString = "EvoNov D + II Nov";

            noveltyGenerations = 25;
            archive = 1;
            feasible = 25;
            infeasible = 25;

            evolutionGenerations = 25;
            evoPopSize = 10;
            parents = 2;
            children = 8;

            Console.WriteLine("Starting evolution with highest fitness novel maps {0} at {1}.", folderString, DateTime.Now.TimeOfDay);
            nso = new NoveltySearchOptions(feasiblePopulationSize: feasible, infeasiblePopulationSize: infeasible, addToArchive: archive);
            RunEvolutionWithNoveltyAsBase(maps, random, runMOEA: false, selectHighestFitness: selectHighestFitness, numberOfNoveltyGenerations: noveltyGenerations, numberOfEvolutionGenerations: evolutionGenerations, noveltySearchOptions: nso, evolutionPopulationSize: evoPopSize, numberOfParents: parents, numberOfChildren: children, selectionStrategy: Enums.SelectionStrategy.ChanceBased, parentSelectionStrategy: Enums.SelectionStrategy.ChanceBased, folderName: folderString, fileToWriteTo: txtFile);
            Console.WriteLine("Evolution with highest fitness novel maps. It took {0} milliseconds to perform evolution with highest fitness novel maps.", sw.ElapsedMilliseconds);
            Console.WriteLine("------");
            sw.Restart();
        }

        #region Search Methods
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
            string fileToWriteTo = "evolutionGenerationTimes.txt",
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

                bestMaps.Add(evolver.Population.OrderByDescending(evoMap => evoMap.Fitness).ToList()[0]);
                MapHelper.SaveGreyscaleNoveltyMap(evolver.Population, string.Format("Base Map {0} NoveltyMap", baseMapCounter), folderName);

                baseMapCounter++;
            }

            MapHelper.SaveGreyscaleNoveltyMap(bestMaps, string.Format(" Best Maps NoveltyMap"), folderName);
            WriteToTextFile(stringToWrite.ToString(), fileToWriteTo, folderName);
        }

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
                
                evolver.RunEvolution(sb);

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

                baseMapCounter++;
            }

            MapHelper.SaveGreyscaleNoveltyMap(bestMaps, string.Format(" Best Maps NoveltyMap"), folderName);
            WriteToTextFile(sb.ToString(), fileToWriteTo, folderName);
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
