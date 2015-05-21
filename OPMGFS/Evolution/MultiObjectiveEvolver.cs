namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using OPMGFS.Map;

    public class MultiObjectiveEvolver
    {
        public MultiObjectiveEvolver(
            int numberOfGenerations,
            int populationSize,
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
        /// Gets or sets the chance of mutation happening.
        /// </summary>
        private double MutationChance { get; set; }

        private MapSearchOptions MapSearchOptions { get; set; }

        private MapFitnessOptions MapFitnessOptions { get; set; }

        #region NSGA-II

        /// <summary>
        /// Runs the NSGA-II evolution.
        /// </summary>
        /// <param name="sb">The string builder to add timings to.</param>
        /// <param name="initialPopulation">The initial population of the evolution.</param>
        /// <returns> The best map found. </returns>
        public EvolvableMap RunEvolution(StringBuilder sb = null, IEnumerable<EvolvableMap> initialPopulation = null)
        {
            var sw = new Stopwatch();
            sw.Start();

            List<MOEASolution> parents = null;

            if (initialPopulation != null)
            {
                parents = initialPopulation.Select(x => new MOEASolution(x)).ToList();
            }

            parents = parents ?? this.CreateInitialPopulation();

            EvaluatePopulation(parents);
            var offspringPopulation = this.MakeNewPopulation(parents);
            EvaluatePopulation(offspringPopulation);

            if (sb != null) sb.AppendLine(string.Format("\tInitial population took {0} ms to create and evaluate", sw.ElapsedMilliseconds));
            if (sb != null) sb.AppendLine();

            List<MOEASolution> combinedPopulation;
            List<List<MOEASolution>> nonDominatedFronts;

            // For every generation
            for (var generation = 0; generation < this.NumberOfGenerations; generation++)
            {
                sw.Restart();

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

                var missingParents = this.PopulationSize - nextParents.Count;
                for (var i = 0; i < missingParents; i++)
                    nextParents.Add(currentFront[i]);

                parents = nextParents;
                offspringPopulation = this.MakeNewPopulation(parents);

                var generationTime = sw.ElapsedMilliseconds;

                EvaluatePopulation(offspringPopulation);

                if (sb != null) sb.AppendLine(string.Format("\tGeneration {0} took {1} ms to run. {2} ms was spent on evaluating new offspring, {3} on the evolution itself.", generation, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds - generationTime, generationTime));
                var fitnessValues = new List<double>();
                fitnessValues.AddRange(parents.Select(x => x.Map.Fitness));
                fitnessValues.AddRange(offspringPopulation.Select(x => x.Map.Fitness));
                fitnessValues = fitnessValues.OrderByDescending(x => x).ToList();

                if (sb != null) sb.AppendLine(string.Format("\t\tHighest Fitness: {0}", fitnessValues[0]));
                if (sb != null) sb.AppendLine(string.Format("\t\tAverage Fitness: {0}", fitnessValues.Sum(x => x) / fitnessValues.Count));
                if (sb != null) sb.AppendLine(string.Format("\t\tLowest Fitness:  {0}", fitnessValues[fitnessValues.Count - 1]));
                if (sb != null) sb.AppendLine();
            }

            //// Select best result
            combinedPopulation = new List<MOEASolution>();
            combinedPopulation.AddRange(parents);
            combinedPopulation.AddRange(offspringPopulation);

            foreach (var moeaSolution in combinedPopulation)
            {
                this.Population.Add(moeaSolution.Map);
            }

            // Sort the combination
            nonDominatedFronts = this.FastNonDominatedSearch(combinedPopulation);
            var best = nonDominatedFronts[0].OrderBy(p => p.Rank).ThenByDescending(p => p.Distance).ToList();

            return best[0].Map;
        }

        /// <summary>
        /// Calculates the fitness for every solution in the given population.
        /// </summary>
        /// <param name="pop"> The population to evaluate. </param>
        private static void EvaluatePopulation(IEnumerable<MOEASolution> pop)
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

                // Mark boundaries
                front[0].Distance = double.NegativeInfinity;
                front[front.Count - 1].Distance = double.PositiveInfinity;

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

        /// <summary>
        /// A class that represents a solution in the multi-objective evolutionary algorithm search.
        /// </summary>
        private class MOEASolution
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MOEASolution"/> class. 
            /// </summary>
            /// <param name="map"> The map to represent. </param>
            public MOEASolution(EvolvableMap map)
            {
                this.Rank = 0;
                this.DominationCount = 0;
                this.Distance = 0;
                this.Map = map;
                this.DominatedSolutions = new List<MOEASolution>();
            }

            /// <summary>
            /// Gets or sets the rank (the front) of the solution.
            /// </summary>
            public int Rank { get; set; }

            /// <summary>
            /// Gets or sets the number of other solutions that dominate this solution.
            /// </summary>
            public int DominationCount { get; set; }

            /// <summary>
            /// Gets or sets the crowding distance of this solution.
            /// </summary>
            public double Distance { get; set; }

            /// <summary>
            /// Gets the map the solution represents.
            /// </summary>
            public EvolvableMap Map { get; private set; }

            /// <summary>
            /// Gets the list of map fitness values.
            /// </summary>
            public List<double> MapFitnessValuesList 
            { 
                get
                {
                    return this.Map.MapFitnessValues.FitnessList();
                } 
            }

            /// <summary>
            /// Gets or sets the list of solutions dominated by this solution.
            /// </summary>
            public List<MOEASolution> DominatedSolutions { get; set; }

            /// <summary>
            /// Determines if this solution dominates the other given solution.
            /// </summary>
            /// <param name="other"> The solution to check for dominance. </param>
            /// <returns> True if this solution dominates the other solution; false otherwise. </returns>
            public bool Dominates(MOEASolution other)
            {
                return this.Map.Dominates(other.Map);
            }
        }

        #endregion
    }
}
