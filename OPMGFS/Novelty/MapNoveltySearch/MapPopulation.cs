namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// A population of maps used in map novelty search.
    /// </summary>
    public class MapPopulation : Population
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapPopulation"/> class.
        /// </summary>
        /// <param name="isFeasiblePopulation"> Whether this is a feasible or infeasible population. </param>
        /// <param name="populationSize"> The population size. </param>
        public MapPopulation(bool isFeasiblePopulation, int populationSize)
            : base(isFeasiblePopulation, populationSize)
        {
            this.CurrentGeneration = new List<Solution>();
        }

        /// <summary>
        /// Advances a generation of map solutions using mutation.
        /// </summary>
        /// <param name="nso"> The search options. </param>
        /// <param name="other"> The other population (infeasible or feasible). </param>
        /// <param name="na"> The novel archive. </param>
        /// <param name="r"> The random used in mutation. </param>
        /// <returns> The list of solutions in the next generation. </returns>
        public override List<Solution> AdvanceGeneration(NoveltySearchOptions nso, Population other, NovelArchive na, Random r)
        {
            //Console.WriteLine("---");
            //Console.WriteLine("Advancing {0} generation", this.IsFeasiblePopulation ? "feasible" : "infeasible");
            
            //var sw = new Stopwatch();
            //sw.Start();
            
            var children = this.CurrentGeneration.Select(individual => (MapSolution)individual.Mutate(r)).ToList();
            //Console.WriteLine("It took {0} milliseconds to mutate children.", sw.ElapsedMilliseconds);
            //sw.Restart();

            var allIndividuals = this.CurrentGeneration.ToList();
            allIndividuals.AddRange(children);

            var newPopulation = new List<Solution>();

            var returnedPopulation = new List<Solution>();

            foreach (var i in children)
            {
                i.CalculateNovelty(this, na, nso.NumberOfNeighbours);

                if (this.IsFeasiblePopulation)
                {
                    if (!i.IsFeasible)
                    {
                        returnedPopulation.Add(i);
                    }
                }
                else
                {
                    if (i.IsFeasible)
                    {
                        returnedPopulation.Add(i);
                    }
                }
            }

            //Console.WriteLine("It took {0} milliseconds to calculate novelty of children.", sw.ElapsedMilliseconds);
            allIndividuals.RemoveAll(returnedPopulation.Contains);

            if (this.IsFeasiblePopulation && allIndividuals.Count > 0)
            {
                var newToArchive = nso.AddToArchive;

                for (var i = 0; i < this.PopulationSize; i++)
                {
                    if (allIndividuals.Count <= 0)
                    {
                        break;
                    }

                    var index = 0;
                    var highestNovelty = double.MinValue;

                    for (var j = 0; j < allIndividuals.Count; j++)
                    {
                        if (!(allIndividuals[j].Novelty > highestNovelty))
                        {
                            continue;
                        }

                        highestNovelty = allIndividuals[j].Novelty;
                        index = j;
                    }

                    if (newToArchive > 0 && allIndividuals[index].Novelty > nso.MinimumNovelty)
                    {
                        na.Archive.Add(allIndividuals[index]);
                        newToArchive--;
                    }
                    else
                    {
                        newPopulation.Add(allIndividuals[index]);
                    }

                    allIndividuals.RemoveAt(index);
                }   
            }

            if (!this.IsFeasiblePopulation && allIndividuals.Count > 0)
            {
                for (var i = 0; i < this.PopulationSize; i++)
                {
                    if (allIndividuals.Count <= 0)
                    {
                        break;
                    }

                    var index = 0;
                    var lowestDistance = double.MaxValue;

                    for (var j = 0; j < allIndividuals.Count; j++)
                    {
                        if (!(allIndividuals[j].DistanceToFeasibility < lowestDistance))
                        {
                            continue;
                        }

                        lowestDistance = allIndividuals[j].DistanceToFeasibility;
                        index = j;
                    }

                    newPopulation.Add(allIndividuals[index]);
                    allIndividuals.RemoveAt(index);
                }
            }

            this.CurrentGeneration = newPopulation;
            return returnedPopulation;
        }
    }
}
