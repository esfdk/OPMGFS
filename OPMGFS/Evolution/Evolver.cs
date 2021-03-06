﻿namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using OPMGFS.Map;

    /// <summary>
    /// The evolver.
    /// </summary>
    /// <typeparam name="T">A type that is a subtype of Evolvable.</typeparam>
    public class Evolver<T> where T : Evolvable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Evolver{T}"/> class. 
        /// </summary>
        /// <param name="numberOfGenerations">The number of generations to evolve over.</param>
        /// <param name="populationSize">The size of the population to evolve.</param>
        /// <param name="numberOfParents">The number of children to spawn mutations from.</param>
        /// <param name="numberOfChildren">The number of children to create for every generation.</param>
        /// <param name="mutationChance">The chance of mutation happening in the parents.</param>
        /// <param name="r">The random object used in this evolver.</param>
        /// <param name="initializationArguments">The arguments used when initializing new objects.</param>
        public Evolver(int numberOfGenerations, int populationSize, int numberOfParents, int numberOfChildren, double mutationChance, Random r, object[] initializationArguments = null)
        {
            this.Random = r;
            this.Population = new List<T>();

            this.ParentSelectionStrategy = Enums.SelectionStrategy.HighestFitness;
            this.PopulationSelectionStrategy = Enums.SelectionStrategy.HighestFitness;
            this.PopulationStrategy = Enums.PopulationStrategy.Mutation;

            this.NumberOfGenerations = numberOfGenerations;
            this.PopulationSize = populationSize;
            this.NumberOfParents = numberOfParents;
            this.NumberOfChildren = numberOfChildren;
            this.MutationChance = mutationChance;
            this.InitializationArguments = initializationArguments ?? new object[] { this.MutationChance, this.Random };
        }

        /// <summary>
        /// Gets the random used for this evolver.
        /// </summary>
        public Random Random { get; private set; }

        /// <summary>
        /// Gets the current population.
        /// </summary>
        public List<T> Population { get; private set; }

        /// <summary>
        /// Gets or sets a value that determines how parents are selected for the population step.
        /// </summary>
        public Enums.SelectionStrategy ParentSelectionStrategy { get; set; }

        /// <summary>
        /// Gets or sets a value that determines how the next population is selected.
        /// </summary>
        public Enums.SelectionStrategy PopulationSelectionStrategy { get; set; }

        /// <summary>
        /// Gets or sets a value that determines how a new population is created.
        /// </summary>
        public Enums.PopulationStrategy PopulationStrategy { get; set; }

        /// <summary>
        /// Gets or sets the initialization arguments.
        /// </summary>
        private object[] InitializationArguments { get; set; }

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
        /// Starts, and handles, the evolution.
        /// </summary>
        /// <param name="sb">The string builder to write time to.</param>
        /// <returns>The best individual at the end of the evolution.</returns>
        public Evolvable Evolve(StringBuilder sb)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < this.NumberOfGenerations; i++)
            {
                var candidates = this.SelectParents();
                this.SpawnChildren(candidates);
                this.EvaluatePopulation();
                this.SelectPopulationForNextGeneration();

                sb.AppendLine(string.Format("\tIt took {0} ms to advance the evolution to generation {1}.", sw.ElapsedMilliseconds, i + 1));

                var pop = this.Population.OrderByDescending(s => s.Fitness).ToList();
                var average = this.Population.Sum(s => s.Fitness) / this.Population.Count;
                sb.AppendLine(string.Format("\t\t The highest fitness in the population is {0}", pop[0].Fitness));
                sb.AppendLine(string.Format("\t\t The average fitness in the population is {0}", average));
                sb.AppendLine(string.Format("\t\t The lowest fitness in the population is {0}", pop[this.Population.Count - 1].Fitness));

                sw.Restart();
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
        /// <param name="sb">The string builder to append time to.</param>
        public void Initialize(StringBuilder sb)
        {
            var sw = new Stopwatch();
            sw.Start();
            this.GenerateInitialPopulation();
            this.EvaluatePopulation();
            sb.AppendLine(string.Format("\tIt took {0} ms to generate and evaluate initial population.", sw.ElapsedMilliseconds));
        }

        /// <summary>
        /// Initializes and evaluates the initial population.
        /// </summary>
        /// <param name="initialPopulation">The initial population to use.</param>
        public void Initialize(IEnumerable<T> initialPopulation)
        {
            this.Population.AddRange(initialPopulation);
            this.EvaluatePopulation();
        }

        /// <summary>
        /// Initializes and evaluates the initial population.
        /// </summary>
        /// <param name="initialPopulation">The initial population to use.</param>
        /// <param name="sb">The string builder to append time to.</param>
        public void Initialize(IEnumerable<T> initialPopulation, StringBuilder sb)
        {
            var sw = new Stopwatch();
            sw.Start();
            this.Population.AddRange(initialPopulation);
            this.EvaluatePopulation();
            sb.AppendLine(string.Format("\tIt took {0} ms to generate and evaluate initial population.", sw.ElapsedMilliseconds));
        }

        /// <summary>
        /// Generates the initial population for the evolution.
        /// </summary>
        private void GenerateInitialPopulation()
        {
            for (var i = 0; i < this.PopulationSize; i++)
            {
                var temp = (T)Activator.CreateInstance(typeof(T), this.InitializationArguments);
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
        }

        /// <summary>
        /// Selects the candidates to spawn children from.
        /// </summary>
        /// <returns>A list containing the candidates.</returns>
        private List<T> SelectParents()
        {
            List<int> parentIndicies;

            switch (this.ParentSelectionStrategy)
            {
                case Enums.SelectionStrategy.HighestFitness:
                    parentIndicies = this.SelectionHighestFitness(this.NumberOfParents);
                    break;

                case Enums.SelectionStrategy.ChanceBased:
                    parentIndicies = this.SelectionChanceBased(this.NumberOfParents);
                    break;

                default:
                    parentIndicies = this.SelectionHighestFitness(this.NumberOfParents);
                    break;
            }

            return parentIndicies.Select(parentIndicy => this.Population[parentIndicy]).ToList();
        }

        /// <summary>
        /// Spawns children from the candidates chosen and adds them directly to the population.
        /// </summary>
        /// <param name="candidates">A list of the candidates to create children from.</param>
        private void SpawnChildren(List<T> candidates)
        {
            for (var i = 0; i < this.NumberOfChildren; i++)
            {
                T tempChild;

                switch (this.PopulationStrategy)
                {
                    case Enums.PopulationStrategy.Mutation:
                        tempChild = (T)candidates[i % this.NumberOfParents].SpawnMutation();
                        break;

                    case Enums.PopulationStrategy.Recombination:
                        tempChild = (T)candidates[i % this.NumberOfParents].SpawnRecombination(candidates[(i + 1) % this.NumberOfParents]);
                        break;

                    default:
                        tempChild = (T)candidates[i % this.NumberOfParents].SpawnMutation();
                        break;
                }

                this.Population.Add(tempChild);
            }
        }

        /// <summary>
        /// Selects the population for the next generation from among the current population.
        /// </summary>
        private void SelectPopulationForNextGeneration()
        {
            List<int> populationIndicies;

            switch (this.ParentSelectionStrategy)
            {
                case Enums.SelectionStrategy.HighestFitness:
                    populationIndicies = this.SelectionHighestFitness(this.PopulationSize);
                    break;

                case Enums.SelectionStrategy.ChanceBased:
                    populationIndicies = this.SelectionChanceBased(this.PopulationSize);
                    break;

                default:
                    populationIndicies = this.SelectionHighestFitness(this.PopulationSize);
                    break;
            }

            var tempPopulation = populationIndicies.Select(parentIndicy => this.Population[parentIndicy]).ToList();
            this.Population = tempPopulation;
        }

        /// <summary>
        /// Selects the individuals with the highest fitness.
        /// </summary>
        /// <returns>A list that contains the indexes of the selected individuals.</returns>
        /// <param name="amountToFind">The number of individuals to find.</param>
        private List<int> SelectionHighestFitness(int amountToFind)
        {
            var individualIndicies = new List<int>();

            double[] lowestFitness = { 1000000d };
            var lowestIndex = -1;

            // Iterate over the entire population
            for (var i = 0; i < this.Population.Count; i++)
            {
                // If we have not found enough for the new population, just add the individual
                if (individualIndicies.Count < amountToFind)
                {
                    individualIndicies.Add(i);

                    // If we have filled the list, find the individual with the lowest fitness and save its index, in case it needs to be replaced.
                    if (individualIndicies.Count < amountToFind) 
                        continue;

                    foreach (var parentIndicy in individualIndicies.Where(parentIndicy => this.Population[parentIndicy].Fitness < lowestFitness[0]))
                    {
                        lowestFitness[0] = this.Population[parentIndicy].Fitness;
                        lowestIndex = parentIndicy;
                    }
                }
                else
                {
                    // If the fitness of the individual being considered is higher than the lowest fitness of best ones so far,
                    // remove the old individual and add the new individual.
                    if (this.Population[i].Fitness > lowestFitness[0])
                    {
                        individualIndicies.Remove(lowestIndex);
                        individualIndicies.Add(i);

                        lowestFitness[0] = 1000000d;
                        lowestIndex = -1;

                        // Find the individual with the lowest fitness and save its index, in case it needs to be replaced.
                        foreach (var parentIndicy in individualIndicies.Where(parentIndicy => this.Population[parentIndicy].Fitness < lowestFitness[0]))
                        {
                            lowestFitness[0] = this.Population[parentIndicy].Fitness;
                            lowestIndex = parentIndicy;
                        }
                    }
                }
            }

            return individualIndicies;
        }

        /// <summary>
        /// Selects individuals through chance, where a higher fitness means a higher chance of being selected.
        /// </summary>
        /// <returns>A list that contains the indexes of the selected individuals.</returns>
        /// <param name="amountToFind">The number of individuals to find.</param>
        private List<int> SelectionChanceBased(int amountToFind)
        {
            var parentIndicies = new List<int>();

            var highestFitness = -1000000d;
            var lowestFitness = 1000000d;

            // Find lowest and highest fitness so we can normalize fitness values.
            foreach (var individual in this.Population)
            {
                if (individual.Fitness > highestFitness) highestFitness = individual.Fitness;
                if (individual.Fitness < lowestFitness) lowestFitness = individual.Fitness;
            }

            // Add the individual with the highest fitness.
            for (var i = 0; i < this.Population.Count; i++)
            {
                if (!(Math.Abs(this.Population[i].Fitness - highestFitness) < EvolutionOptions.Tolerance))
                    continue;

                parentIndicies.Add(i);
                break;
            }

            while (parentIndicies.Count < amountToFind)
            {
                // Iterate over the population
                for (var i = 0; i < this.Population.Count; i++)
                {
                    if (parentIndicies.Count >= amountToFind) break;
                    if (parentIndicies.Contains(i)) continue;

                    // Calculate the normalized value of the individual and use that as the chance of being selected.
                    var normalizedValue = (this.Population[i].Fitness - lowestFitness)
                                          / (highestFitness - lowestFitness);

                    if (this.Random.NextDouble() < normalizedValue)
                        parentIndicies.Add(i);
                }
            }

            return parentIndicies;
        }
    }
}
