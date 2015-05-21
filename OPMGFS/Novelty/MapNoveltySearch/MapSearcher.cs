namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Diagnostics;
    using System.Text;

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
        public MapSearcher(Random r, MapSearchOptions mapSearchOptions, NoveltySearchOptions noveltySearchOptions)
            : base(r, noveltySearchOptions)
        {
            this.MapSearchOptions = mapSearchOptions;
            this.FeasiblePopulation = new MapPopulation(true, noveltySearchOptions.FeasiblePopulationSize);
            this.InfeasiblePopulation = new MapPopulation(false, noveltySearchOptions.InfeasiblePopulationSize);
            this.Archive = new MapNovelArchive();

            var numberOfAttempts = 0;

            while (this.FeasiblePopulation.CurrentGeneration.Count < noveltySearchOptions.FeasiblePopulationSize)
            {
                var list = MapConversionHelper.GenerateInitialMapPoints(mapSearchOptions, r);
                var ms = new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, list, r);

                if (ms.IsFeasible)
                {
                    this.FeasiblePopulation.CurrentGeneration.Add(ms);
                }
                else if (InfeasiblePopulation.CurrentGeneration.Count < noveltySearchOptions.InfeasiblePopulationSize)
                {
                    this.InfeasiblePopulation.CurrentGeneration.Add(ms);
                }

                // HACK: Should probably be a setting and/or timer instead of number of iterations
                if (numberOfAttempts > 1000)
                {
                    break;
                }

                numberOfAttempts++;
            }

            while (this.InfeasiblePopulation.CurrentGeneration.Count < noveltySearchOptions.InfeasiblePopulationSize)
            {
                var list = MapConversionHelper.GenerateInitialMapPoints(mapSearchOptions, r);

                for (var j = 0; j < list.Count; j++)
                {
                    var item = list[j];

                    var distance = r.Next(2) == 1 ? (r.Next(2) == 1 ? (item.Distance + 1.0) : (item.Distance - 1.0)) : item.Distance;
                    var degree = r.Next(2) == 1 ? (r.Next(2) == 1 ? (item.Degree + 180) : (item.Degree - 180)) : item.Degree;

                    var mp = new MapPoint(distance, degree, item.Type, Enums.WasPlaced.NotAttempted);

                    list[j] = mp;
                }

                var ms = new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, list, r);
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
        /// Runs a number of generations to search for maps.
        /// </summary>
        /// <param name="generations">Number of generations to run.</param>
        /// <param name="sb">The string builder to append timings to.</param>
        public void RunGenerations(int generations, StringBuilder sb)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < generations; i++)
            {
                this.NextGeneration();
                sb.AppendLine(string.Format("\tIt took {0} ms to advance the novelty search to generation {1}.", sw.ElapsedMilliseconds, i + 1));
                sw.Restart();
            }
        }
    }
}
