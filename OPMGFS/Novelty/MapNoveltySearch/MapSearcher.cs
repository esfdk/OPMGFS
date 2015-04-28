namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;

    using OPMGFS.Map;
    using OPMGFS.Map.MapObjects;

    /// <summary>
    /// Searches for novel maps.
    /// </summary>
    public class MapSearcher : NoveltySearcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapSearcher"/> class.
        /// </summary>
        /// <param name="r">
        /// The random to use in searching. 
        /// </param>
        /// <param name="mapSearchOptions">
        /// The map options for this search.
        /// </param>
        /// <param name="noveltySearchOptions">
        /// The novelty Search Options.
        /// </param>
        public MapSearcher(Random r, MapSearchOptions mapSearchOptions, NoveltySearchOptions noveltySearchOptions) : base(r)
        {
            this.MapSearchOptions = mapSearchOptions;
            this.NoveltySearchOptions = noveltySearchOptions;
            this.FeasiblePopulation = new MapPopulation(true, noveltySearchOptions.FeasiblePopulationSize);
            this.InfeasiblePopulation = new MapPopulation(false, noveltySearchOptions.InfeasiblePopulationSize);
            this.Archive = new MapNovelArchive();

            while (this.FeasiblePopulation.CurrentGeneration.Count < noveltySearchOptions.FeasiblePopulationSize)
            {
                var list = MapConversionHelper.GenerateInitialMapPoints(mapSearchOptions, r);
                var ms = new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, list);

                if (ms.IsFeasible)
                {
                    this.FeasiblePopulation.CurrentGeneration.Add(ms);
                }
                else if (InfeasiblePopulation.CurrentGeneration.Count < noveltySearchOptions.InfeasiblePopulationSize)
                {
                    this.InfeasiblePopulation.CurrentGeneration.Add(ms);
                }
            }

            while (this.InfeasiblePopulation.CurrentGeneration.Count < noveltySearchOptions.InfeasiblePopulationSize)
            {
                var list = MapConversionHelper.GenerateInitialMapPoints(mapSearchOptions, r);

                for (var j = 0; j < noveltySearchOptions.InfeasiblePopulationSize / 2.0; j++)
                {
                    var item = list[j];

                    var distance = r.Next(2) == 1 ? (r.Next(2) == 1 ? (item.Distance + 1.0) : (item.Distance - 1.0)) : item.Distance;
                    var degree = r.Next(2) == 1 ? (r.Next(2) == 1 ? (item.Degree + 180) : (item.Degree - 180)) : item.Degree;

                    var mp = new MapPoint(distance, degree, item.Type, Enums.WasPlaced.NotAttempted);

                    list[j] = mp;
                }

                var ms = new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, list);
                this.InfeasiblePopulation.CurrentGeneration.Add(ms);
            }

            foreach (var ms in this.FeasiblePopulation.CurrentGeneration)
            {
                var novelty = ms.CalculateNovelty(
                    this.FeasiblePopulation,
                    this.Archive,
                    noveltySearchOptions.NumberOfNeighbours);

                if (novelty >= noveltySearchOptions.MinimumNovelty)
                {
                    this.Archive.Archive.Add(ms);
                }
            }
        }

        /// <summary>
        /// Gets the options for this search.
        /// </summary>
        public MapSearchOptions MapSearchOptions { get; private set; }

        /// <summary>
        /// Gets the options for this search.
        /// </summary>
        public NoveltySearchOptions NoveltySearchOptions { get; private set; }

        /// <summary>
        /// Runs a number of generations to search for maps.
        /// </summary>
        /// <param name="generations">Number of generations to run.</param>
        public void RunGenerations(int generations)
        {   
            for (var i = 0; i < generations; i++)
            {
                this.NextGeneration();
            }
        }

        /// <summary>
        /// Advances the search to the next generation.
        /// </summary>
        protected override void NextGeneration()
        {
            var infeasibleIndividuals = FeasiblePopulation.AdvanceGeneration(
                new NoveltySearchOptions(),
                InfeasiblePopulation,
                Archive,
                Random);
            var feasibleIndividuals = InfeasiblePopulation.AdvanceGeneration(
                new NoveltySearchOptions(),
                FeasiblePopulation,
                Archive,
                Random);

            FeasiblePopulation.CurrentGeneration.AddRange(feasibleIndividuals);
            InfeasiblePopulation.CurrentGeneration.AddRange(infeasibleIndividuals);    
        }
    }
}
