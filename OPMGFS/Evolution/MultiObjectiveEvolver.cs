namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using OPMGFS.Map;

    public class MultiObjectiveEvolver
    {
        private Dictionary<EvolvableMap, List<EvolvableMap>> mapsDominatedByDictionary = new Dictionary<EvolvableMap, List<EvolvableMap>>();

        public MultiObjectiveEvolver(
            int numberOfGenerations,
            int populationSize,
            int numberOfParents,
            int numberOfChildren,
            double mutationChance,
            Random r,
            MapSearchOptions mso,
            MapFitnessOptions mfo)
        {
            this.MapSearchOptions = mso;
            this.MapFitnessOptions = mfo;
            this.Random = r;
            this.Population = new List<EvolvableMap>();

            this.NumberOfGenerations = numberOfGenerations;
            this.PopulationSize = populationSize;
            this.NumberOfParents = numberOfParents;
            this.NumberOfChildren = numberOfChildren;
            this.MutationChance = mutationChance;
        }

        /// <summary>
        /// Gets the random used for this evolver.
        /// </summary>
        public Random Random { get; private set; }

        /// <summary>
        /// Gets the current population.
        /// </summary>
        public List<EvolvableMap> Population { get; private set; }

        /// <summary>
        /// Gets or sets the number of generations for the evolution.
        /// </summary>
        private int NumberOfGenerations { get; set; }

        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        private int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the number of parents to choose for populating a new generation.
        /// </summary>
        private int NumberOfParents { get; set; }

        /// <summary>
        /// Gets or sets the number of children to spawn during evolution.
        /// </summary>
        private int NumberOfChildren { get; set; }

        /// <summary>
        /// Gets or sets the chance of mutation happening.
        /// </summary>
        private double MutationChance { get; set; }

        private MapSearchOptions MapSearchOptions { get; set; }

        private MapFitnessOptions MapFitnessOptions { get; set; }

        /// <summary>
        /// Starts, and handles, the evolution.
        /// </summary>
        /// <returns>The best individual at the end of the evolution.</returns>
        public Evolvable Evolve()
        {
            for (var i = 0; i < this.NumberOfGenerations; i++)
            {
                Console.WriteLine("Starting generation {0}", i);
                var candidates = this.SelectParents();
                this.SpawnChildren(candidates);
                this.EvaluatePopulation();
                this.SelectPopulationForNextGeneration();
            }

            Evolvable best = null;

            foreach (var individual in this.Population)
            {
                if (best == null) best = individual;
                if (individual.Fitness > best.Fitness) best = individual;
            }

            return best;
        }

        /// <summary>
        /// Initializes and evaluates the initial population.
        /// </summary>
        public void Initialize()
        {
            this.GenerateInitialPopulation();
            this.EvaluatePopulation();
        }

        /// <summary>
        /// Initializes and evaluates the initial population.
        /// </summary>
        /// <param name="initialPopulation">The initial population to use.</param>
        public void Initialize(IEnumerable<EvolvableMap> initialPopulation)
        {
            this.Population.AddRange(initialPopulation);
            this.EvaluatePopulation();
        }

        /// <summary>
        /// Generates the initial population for the evolution.
        /// </summary>
        private void GenerateInitialPopulation()
        {
            for (var i = 0; i < this.PopulationSize; i++)
            {
                var temp = new EvolvableMap(this.MapSearchOptions, this.MutationChance, this.Random, this.MapFitnessOptions);
                temp.InitializeObject();
                this.Population.Add(temp);
            }
        }

        /// <summary>
        /// Evaluates every individual in the population.
        /// </summary>
        private void EvaluatePopulation()
        {
            mapsDominatedByDictionary = new Dictionary<EvolvableMap, List<EvolvableMap>>();
            Console.WriteLine("Starting evaluation of population");
            var sw = new Stopwatch();
            sw.Start();
            foreach (var individual in this.Population)
            {
                individual.CalculateFitness();
            }

            for (var i = 0; i < this.Population.Count; i++)
            {
                var map1 = this.Population[i];
                List<EvolvableMap> map1List;

                if (!this.mapsDominatedByDictionary.TryGetValue(map1, out map1List))
                {
                    map1List = new List<EvolvableMap>();
                    this.mapsDominatedByDictionary[map1] = map1List;
                }

                for (var j = i; j < this.Population.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var map2 = this.Population[j];
                    List<EvolvableMap> map2List;

                    if (!this.mapsDominatedByDictionary.TryGetValue(map2, out map2List))
                    {
                        map2List = new List<EvolvableMap>();
                        this.mapsDominatedByDictionary[map2] = map2List;
                    }

                    if (map1.IsDominatedBy(map2))
                    {
                        map1List.Add(map2);
                    }
                    else if (map2.IsDominatedBy(map1))
                    {
                        map2List.Add(map1);
                    }
                }
            }
            Console.WriteLine("Finished evaluation. It took {0} milliseconds", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Selects the candidates to spawn children from.
        /// </summary>
        /// <returns>A list containing the candidates.</returns>
        private List<EvolvableMap> SelectParents()
        {
            Console.WriteLine("Selecting parents");
            return this.SelectLeastDominatedIndividuals(this.NumberOfParents);
        }

        /// <summary>
        /// Spawns children from the candidates chosen and adds them directly to the population.
        /// </summary>
        /// <param name="candidates">A list of the candidates to create children from.</param>
        private void SpawnChildren(List<EvolvableMap> candidates)
        {
            Console.WriteLine("Spawning Children");
            for (var i = 0; i < this.NumberOfChildren; i++)
            {
                var tempChild = (EvolvableMap)candidates[i % this.NumberOfParents].SpawnMutation();

                this.Population.Add(tempChild);
            }
        }

        /// <summary>
        /// Selects the population for the next generation from among the current population.
        /// </summary>
        private void SelectPopulationForNextGeneration()
        {
            Console.WriteLine("Selecting population for next generation");
            this.Population = this.SelectLeastDominatedIndividuals(this.PopulationSize);
        }

        private List<EvolvableMap> SelectLeastDominatedIndividuals(int amountToFind)
        {
            var candidates = new List<EvolvableMap>();

            while (candidates.Count < amountToFind)
            {
                var nonDominatedMapFound = false;
                foreach (var map in this.mapsDominatedByDictionary.Where(map => map.Value.Count == 0))
                {
                    candidates.Add(map.Key);
                    if (candidates.Count >= amountToFind)
                    {
                        return candidates;
                    }

                    nonDominatedMapFound = true;
                }

                foreach (var parent in candidates)
                {
                    this.mapsDominatedByDictionary.Remove(parent);
                    foreach (var map in this.mapsDominatedByDictionary)
                    {
                        map.Value.Remove(parent);
                    }
                }

                if (!nonDominatedMapFound)
                {
                    break;
                }
            }

            while (candidates.Count < amountToFind)
            {
                var numberOfDominaters = this.PopulationSize + 1;
                EvolvableMap leastDominatedMap = null;
                var fitness = double.NegativeInfinity;

                foreach (var map in this.mapsDominatedByDictionary)
                {
                    var dominators = map.Value.Count;

                    if (dominators < numberOfDominaters)
                    {
                        leastDominatedMap = map.Key;
                        fitness = map.Key.Fitness;
                        numberOfDominaters = dominators;
                    }

                    if (dominators == numberOfDominaters)
                    {
                        if (map.Key.Fitness > fitness)
                        {
                            leastDominatedMap = map.Key;
                            fitness = map.Key.Fitness;
                            numberOfDominaters = dominators;
                        }
                    }
                }

                if (leastDominatedMap == null)
                {
                    throw new Exception("Could not find a least dominated map.");
                }

                candidates.Add(leastDominatedMap);

                this.mapsDominatedByDictionary.Remove(leastDominatedMap);
                foreach (var map in this.mapsDominatedByDictionary)
                {
                    map.Value.Remove(leastDominatedMap);
                }
            }

            return candidates;
        }


        public EvolvableMap RunEvolution()
        {
            var sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Creating initial populations");
            var parents = this.CreateInitialPopulation();
            this.EvaluatePopulation(parents);
            var offspringPopulation = this.MakeNewPopulation(parents);
            this.EvaluatePopulation(offspringPopulation);
            Console.WriteLine("Initial population took {0} ms to create and evaluate", sw.ElapsedMilliseconds);
            Console.WriteLine("----------------------");

            List<MOEASolution> combinedPopulation;
            List<List<MOEASolution>> nonDominatedFronts;

            // For every generation
            for (var generation = 0; generation < this.NumberOfGenerations; generation++)
            {
                sw.Restart();
                Console.WriteLine("Starting generation {0}", generation);
                // Combine the parents and offspring
                combinedPopulation = new List<MOEASolution>();
                combinedPopulation.AddRange(parents);
                combinedPopulation.AddRange(offspringPopulation);

                // Sort the combination
                nonDominatedFronts = this.FastNonDominatedSearch(combinedPopulation);
                var nextParents = new List<MOEASolution>();
                var frontCounter = 0;
                var currentFront = nonDominatedFronts[frontCounter];

                // Take parents from fronts, until the next front will overfill the parent list
                while (nextParents.Count + currentFront.Count <= this.PopulationSize)
                {
                    this.CrowdingDistanceAssignment(currentFront);
                    nextParents.AddRange(currentFront);

                    frontCounter += 1;
                    currentFront = nonDominatedFronts[frontCounter];
                }

                // Get eventual remaining parents from the next front
                currentFront = nonDominatedFronts[frontCounter];
                this.CrowdingDistanceAssignment(currentFront);
                currentFront = currentFront.OrderBy(p => p.Rank).ThenByDescending(p => p.Distance).ToList();

                for (var i = 0; i < this.PopulationSize - nextParents.Count; i++)
                    nextParents.Add(currentFront[i]);

                parents = nextParents;
                offspringPopulation = this.MakeNewPopulation(parents);
                this.EvaluatePopulation(offspringPopulation);
                Console.WriteLine("Generation {0} took {1} ms to run", generation, sw.ElapsedMilliseconds);
                Console.WriteLine("----------------------");
            }

            //// Select best result
            combinedPopulation = new List<MOEASolution>();
            combinedPopulation.AddRange(parents);
            combinedPopulation.AddRange(offspringPopulation);

            // Sort the combination
            nonDominatedFronts = this.FastNonDominatedSearch(combinedPopulation);
            var best = nonDominatedFronts[0].OrderBy(p => p.Rank).ThenByDescending(p => p.Distance).ToList();

            return best[0].Map;
        }

        private void EvaluatePopulation(List<MOEASolution> pop)
        {
            foreach (var moeaSolution in pop)
                moeaSolution.Map.CalculateFitness();
        }

        /// <summary>
        /// Creates the initial population.
        /// </summary>
        /// <returns> The solutions of the initial population. </returns>
        private List<MOEASolution> CreateInitialPopulation()
        {
            var pop = new List<MOEASolution>();

            for (var i = 0; i < this.PopulationSize; i++)
            {
                var temp = new EvolvableMap(this.MapSearchOptions, this.MutationChance, this.Random, this.MapFitnessOptions);
                temp.InitializeObject();
                pop.Add(new MOEASolution(temp));
            }

            return pop;
        }

        /// <summary>
        /// Calculates the crowding distance for every solution in a given front.
        /// </summary>
        /// <param name="front"> The front to calculate the crowding distances for. </param>
        private void CrowdingDistanceAssignment(List<MOEASolution> front)
        {
            // Initialize distances
            foreach (var moeaSolution in front)
                moeaSolution.Distance = 0d;

            // Get the max values for each objective.
            var maxValues = this.MapFitnessOptions.MaxSignificanceList();

            // For every objective...
            for (var objective = 0; objective < maxValues.Count; objective++)
            {
                // Sort the front by the objective in ascending order
                front = front.OrderBy(p => p.MapFitnessValuesList[objective]).ToList();

                // TODO: Figure out what that infinite value is supposed to do (see 'crowding-distance-assignment' in http://www.iitk.ac.in/kangal/Deb_NSGA-II.pdf)

                // Calculate the distance for every solution ('cept boundries).
                for (var popIndex = 1; popIndex < front.Count - 1; popIndex++)
                {
                    front[popIndex].Distance += (front[popIndex + 1].MapFitnessValuesList[objective]
                                                 - front[popIndex - 1].MapFitnessValuesList[objective])
                                                / (maxValues[objective] - 0);
                }
            }
        }

        /// <summary>
        /// Creates a new population through tournament selection (10% of the population or 2, whichever is highest) and mutation.
        /// </summary>
        /// <param name="parents"> The list of solutions to generate offspring from. </param>
        /// <returns> The new population. </returns>
        private List<MOEASolution> MakeNewPopulation(List<MOEASolution> parents)
        {
            var offspring = new List<MOEASolution>();
            var parentCount = parents.Count;
            var maxCompetitors = Math.Round(this.PopulationSize * 0.1);
            if (maxCompetitors <= 1) maxCompetitors = 2;

            for (var i = 0; i < this.PopulationSize; i++)
            {
                var best = parents[this.Random.Next(parentCount)];

                for (var competitorIndex = 1; competitorIndex < maxCompetitors; competitorIndex++)
                {
                    var competitor = parents[this.Random.Next(parentCount)];
                    if (competitor.Rank > best.Rank) best = competitor;
                    else if (competitor.Rank == best.Rank && competitor.Map.Fitness > best.Map.Fitness) best = competitor;
                }

                var tempOffspring = new MOEASolution((EvolvableMap)best.Map.SpawnMutation());
                offspring.Add(tempOffspring);
            }

            return offspring;
        }

        /// <summary>
        /// Sorts the population in multiple non-dominated fronts.
        /// </summary>
        /// <param name="pop"> The population to sort. </param>
        /// <returns> The fronts. </returns>
        private List<List<MOEASolution>> FastNonDominatedSearch(List<MOEASolution> pop)
        {
            var fronts = new List<List<MOEASolution>> { new List<MOEASolution>() };

            // Finds domination count and dominated solutions for every solution.
            for (var p = 0; p < pop.Count; p++)
            {
                var solution = pop[p];

                solution.DominatedSolutions = new List<MOEASolution>();
                solution.DominationCount = 0;

                for (var q = 0; q < pop.Count; q++)
                {
                    if (p == q) continue;
                    var otherSolution = pop[q];

                    if (solution.Dominates(otherSolution))
                        solution.DominatedSolutions.Add(otherSolution);
                    if (otherSolution.Dominates(solution)) 
                        solution.DominationCount += 1;
                }

                // If a solution is not dominated, add it to the initial front.
                if (solution.DominationCount == 0)
                {
                    solution.Rank = 0;
                    fronts[0].Add(solution);
                }
            }

            // Iterate over all fronts.
            var frontCounter = 0;
            while (fronts[frontCounter].Count > 0)
            {
                var nextFront = new List<MOEASolution>();

                // For every solution in the current front
                for (var p = 0; p < fronts[frontCounter].Count; p++)
                {
                    var currentSolution = fronts[frontCounter][p];

                    // Reduce the domination count of every dominated solution by 1
                    foreach (var dominatedSolution in currentSolution.DominatedSolutions)
                    {
                        dominatedSolution.DominationCount -= 1;

                        // If a solution is no longer dominated, add it to the next front.
                        if (dominatedSolution.DominationCount == 0)
                        {
                            dominatedSolution.Rank = frontCounter + 1;
                            nextFront.Add(dominatedSolution);
                        }
                    }
                }

                frontCounter += 1;
                fronts.Add(nextFront);
            }

            return fronts;
        }


        private class MOEASolution
        {
            public int Rank { get; set; }

            public int DominationCount { get; set; }

            public double Distance { get; set; }

            public EvolvableMap Map { get; set; }

            public List<double> MapFitnessValuesList 
            { 
                get
                {
                    return this.Map.MapFitnessValues.FitnessList();
                } 
            }

            public List<MOEASolution> DominatedSolutions { get; set; }

            public MOEASolution(EvolvableMap map)
            {
                this.Rank = 0;
                this.DominationCount = 0;
                this.Distance = 0;
                this.Map = map;
                this.DominatedSolutions = new List<MOEASolution>();
            }

            public bool IsDominatedBy(MOEASolution other)
            {
                return this.Map.IsDominatedBy(other.Map);
            }

            public bool Dominates(MOEASolution other)
            {
                return this.Map.Dominates(other.Map);
            }
        }
    }
}
