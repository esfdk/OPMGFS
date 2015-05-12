namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;
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
        }

        /// <summary>
        /// Selects the candidates to spawn children from.
        /// </summary>
        /// <returns>A list containing the candidates.</returns>
        private List<EvolvableMap> SelectParents()
        {
            return this.SelectLeastDominatedIndividuals(this.NumberOfParents);
        }

        /// <summary>
        /// Spawns children from the candidates chosen and adds them directly to the population.
        /// </summary>
        /// <param name="candidates">A list of the candidates to create children from.</param>
        private void SpawnChildren(List<EvolvableMap> candidates)
        {
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
            this.Population = this.SelectLeastDominatedIndividuals(this.PopulationSize);
        }

        private List<EvolvableMap> SelectLeastDominatedIndividuals(int amountToFind)
        {
            mapsDominatedByDictionary = new Dictionary<EvolvableMap, List<EvolvableMap>>();
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

                if (nonDominatedMapFound)
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
    }
}
