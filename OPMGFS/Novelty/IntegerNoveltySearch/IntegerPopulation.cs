namespace OPMGFS.Novelty.IntegerNoveltySearch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A population in Integer Novelty Search.
    /// </summary>
    public class IntegerPopulation : Population
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerPopulation"/> class.
        /// </summary>
        /// <param name="isFeasiblePopulation">
        /// The is feasible population.
        /// </param>
        /// <param name="populationSize">
        /// The population size.
        /// </param>
        public IntegerPopulation(bool isFeasiblePopulation, int populationSize)
            : base(isFeasiblePopulation, populationSize)
        {
            this.CurrentGeneration = new List<Solution>();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override List<Solution> AdvanceGeneration(NoveltySearchOptions nso, Population other, NovelArchive na, Random r, int numberOfChildren)
        {
            var children = new List<Solution>();

            for (var i = 0; i < numberOfChildren; i++)
            {
                children.Add(this.CurrentGeneration[i % this.CurrentGeneration.Count].Mutate(r));
            }
            
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

            if (this.IsFeasiblePopulation)
            {
                var newToArchive = nso.AddToArchive;

                for (var i = 0; i < this.PopulationSize; i++)
                {
                    var index = 0;
                    var highestNovelty = -5000.0;

                    for (var j = 0; j < allIndividuals.Count; j++)
                    {
                        if (!(allIndividuals[j].Novelty > highestNovelty))
                        {
                            continue;
                        }

                        highestNovelty = allIndividuals[j].Novelty;
                        index = j;
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

            if (!this.IsFeasiblePopulation)
            {
                for (var i = 0; i < this.PopulationSize; i++)
                {
                    var index = 0;
                    var highestDistance = double.MaxValue;

                    for (var j = 0; j < allIndividuals.Count; j++)
                    {
                        if (!(allIndividuals[j].DistanceToFeasibility < highestDistance))
                        {
                            continue;
                        }

                        highestDistance = allIndividuals[j].Novelty;
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
