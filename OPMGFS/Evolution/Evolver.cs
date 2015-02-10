// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Evolver.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the Evolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The evolver.
    /// </summary>
    /// <typeparam name="T">A type that is a subtype of IEvolvable.</typeparam>
    public class Evolver<T> where T : IEvolvable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Evolver{T}"/> class. 
        /// </summary>
        /// <param name="numberOfGenerations">The number of generations to evolve over.</param>
        /// <param name="populationSize">The size of the population to evolve.</param>
        /// <param name="numberOfCandidates">The number of children to spawn mutations from.</param>
        /// <param name="numberOfChildren">The number of children to create for every generation.</param>
        /// <param name="mutationChance">The chance of mutation happening in the parents.</param>
        public Evolver(int numberOfGenerations, int populationSize, int numberOfCandidates, int numberOfChildren, double mutationChance)
        {
            this.NumberOfGenerations = numberOfGenerations;
            this.PopulationSize = populationSize;
            this.NumberOfCandidates = numberOfCandidates;
            this.NumberOfChildren = numberOfChildren;
            this.MutationChance = mutationChance;

            this.Population = new List<T>();

            // this.GenerateInitialPopulation();
            // this.EvaluatePopulation();
        }

        /// <summary>
        /// Gets the current population.
        /// </summary>
        public List<T> Population { get; private set; }

        /// <summary>
        /// Gets or sets the number of generations for the evolution.
        /// </summary>
        private int NumberOfGenerations { get; set; }

        /// <summary>
        /// Gets or sets the size of the population.
        /// </summary>
        private int PopulationSize { get; set; }

        /// <summary>
        /// Gets or sets the number of candidates to choose for mutation.
        /// </summary>
        private int NumberOfCandidates { get; set; }

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
        public IEvolvable Evolve()
        {
            this.GenerateInitialPopulation();
            this.EvaluatePopulation();

            for (var i = 0; i < this.NumberOfGenerations; i++)
            {
                var candidates = this.SelectParents();
                this.SpawnChildren(candidates);
                this.EvaluatePopulation();
                this.SelectPopulationForNextGeneration();
            }

            IEvolvable best = null;

            foreach (var individual in this.Population)
            {
                if (best == null) best = individual;
                if (individual.Fitness > best.Fitness) best = individual;
            }

            return best;

            // 1. Create initial population
            // 2. Evaluate each candidate
            // 3. For each generation:
            //     a. Select parents
            //     b. Create children
            //     c. Evaluate children
            //     d. Select individuals for next generation
        }

        /// <summary>
        /// Generates the initial population for the evolution.
        /// </summary>
        private void GenerateInitialPopulation()
        {
            for (var i = 0; i < this.PopulationSize; i++)
            {
                Console.WriteLine("Generating " + i);

                var temp = (T)Activator.CreateInstance(typeof(T), new object[] { this.MutationChance });
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
            var parentIndicies = new List<int>();

            // Iterate over all parents
            for (var i = 0; i < this.PopulationSize; i++)
            {
                // If we have not found enough parents, just add the individual
                if (parentIndicies.Count < this.NumberOfCandidates)
                {
                    parentIndicies.Add(i);
                }
                else
                {
                    // ... Else, find the selected parent with the lowest fitness
                    var lowestFitness = 100000d;
                    var lowestIndex = 0;
                    foreach (var parentIndicy in parentIndicies)
                    {
                        if (this.Population[parentIndicy].Fitness < lowestFitness)
                        {
                            lowestFitness = this.Population[parentIndicy].Fitness;
                            lowestIndex = parentIndicy;
                        }
                    }

                    // ... and if its fitness is lower than the fitness of the individual being considered,
                    // remove the old parent and add the new individual
                    if (this.Population[i].Fitness > lowestFitness)
                    {
                        parentIndicies.Remove(lowestIndex);
                        parentIndicies.Add(i);
                    }
                }
            }

            //Console.WriteLine("-------");
            //foreach (var d in parentIndicies)
            //{
            //    Console.WriteLine(d + " " + this.Population[d].Fitness);
            //}

            return parentIndicies.Select(parentIndicy => this.Population[parentIndicy]).ToList();
        }

        /// <summary>
        /// Spawns children from the candidates chosen and adds them directly to the population.
        /// TODO: Should maybe not add them directly to population?
        /// </summary>
        /// <param name="candidates">A list of the candidates to create children from.</param>
        private void SpawnChildren(List<T> candidates)
        {
            for (var i = 0; i < this.NumberOfChildren; i++)
            {
                var tempChild = (T)candidates[i % this.NumberOfCandidates].SpawnMutation();
                this.Population.Add(tempChild);
            }
        }

        /// <summary>
        /// Selects the population for the next generation from among the current population.
        /// </summary>
        private void SelectPopulationForNextGeneration()
        {
            var populationIndicies = new List<int>();

            // Iterate over the entire population
            for (var i = 0; i < this.Population.Count; i++)
            {
                // If we have not found enough individuals, just add the individual
                if (populationIndicies.Count < this.PopulationSize)
                {
                    populationIndicies.Add(i);
                }
                else
                {
                    // ... Else, find the selected individuals with the lowest fitness
                    var lowestFitness = 100000d;
                    var lowestIndex = 0;
                    foreach (var parentIndicy in populationIndicies)
                    {
                        if (this.Population[parentIndicy].Fitness < lowestFitness)
                        {
                            lowestFitness = this.Population[parentIndicy].Fitness;
                            lowestIndex = parentIndicy;
                        }
                    }

                    // ... and if its fitness is lower than the fitness of the individual being considered,
                    // remove the old individuals and add the new individual
                    if (this.Population[i].Fitness > lowestFitness)
                    {
                        populationIndicies.Remove(lowestIndex);
                        populationIndicies.Add(i);
                    }
                }
            }

            this.Population = populationIndicies.Select(parentIndicy => this.Population[parentIndicy]).ToList();
        }
    }
}
