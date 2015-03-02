using System;
using System.Collections.Generic;
using System.Linq;
using OPMGFS.Novelty.IntegerNoveltySearch;

namespace OPMGFS.Novelty.MapNoveltySearch
{
    public class MapPopulation : Population
    {
        public MapPopulation(bool isFeasiblePopulation, int populationSize)
            : base(isFeasiblePopulation, populationSize)
        {
            this.CurrentGeneration = new List<Solution>();
        }

        public override List<Solution> AdvanceGeneration(NoveltySearchOptions nso, Population other, NovelArchive na, Random r)
        {
            var children = this.CurrentGeneration.Select(individual => (MapSolution)individual.Mutate(r)).ToList();
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

            allIndividuals.RemoveAll(returnedPopulation.Contains);

            if (IsFeasiblePopulation)
            {
                var newToArchive = nso.AddToArchive;

                for (var i = 0; i < this.PopulationSize; i++)
                {
                    var index = 0;
                    var highestNovelty = -5000.0;

                    for (var j = 0; j < allIndividuals.Count; j++)
                    {
                        if (allIndividuals[j].Novelty > highestNovelty)
                        {
                            highestNovelty = allIndividuals[j].Novelty;
                            index = j;
                        }
                    }


                    if (newToArchive > 0)
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

            if (!IsFeasiblePopulation)
            {
                for (var i = 0; i < this.PopulationSize; i++)
                {
                    var index = 0;
                    var highestDistance = double.MaxValue;

                    for (var j = 0; j < allIndividuals.Count; j++)
                    {
                        if (allIndividuals[j].DistanceToFeasibility < highestDistance)
                        {
                            highestDistance = allIndividuals[j].Novelty;
                            index = j;
                        }
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
