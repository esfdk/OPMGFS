namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Collections.Generic;

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

            // ITODO: Implement better "early filling" of feasible/infeasible populations
            for (var i = 0; i < noveltySearchOptions.FeasiblePopulationSize; i++)
            {
                var list = new List<MapPoint>
                               {
                                   new MapPoint(
                                       this.Random.NextDouble(),
                                       this.Random.Next(181),
                                       Enums.MapPointType.StartBase,
                                       Enums.WasPlaced.NotAttempted)
                               };

                for (var j = 0; j < 15; j++)
                {
                    var dist = Random.NextDouble();
                    var degree = Random.Next(181);
                    Enums.MapPointType mpt;
                    switch (j)
                    {
                        case 0:
                            mpt = Enums.MapPointType.Base;
                            break;
                        case 1:
                            mpt = Enums.MapPointType.Base;
                            break;
                        case 2:
                            mpt = Enums.MapPointType.XelNagaTower;
                            break;
                        case 3:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                        case 4:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                        default:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                    }

                    list.Add(new MapPoint(dist, degree, mpt, Enums.WasPlaced.NotAttempted));
                }

                var ms = new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, list);
                FeasiblePopulation.CurrentGeneration.Add(ms);
            }

            for (var i = 0; i < noveltySearchOptions.InfeasiblePopulationSize; i++)
            {
                var list = new List<MapPoint>
                               {
                                   new MapPoint(
                                       -this.Random.NextDouble(),
                                       this.Random.Next(181, 360),
                                       Enums.MapPointType.StartBase,
                                       Enums.WasPlaced.NotAttempted)
                               };

                for (var j = 0; j < 8; j++)
                {
                    var dist = -Random.NextDouble();
                    var degree = Random.Next(181, 360);
                    Enums.MapPointType mpt;
                    switch (j)
                    {
                        case 0:
                            mpt = Enums.MapPointType.Base;
                            break;
                        case 1:
                            mpt = Enums.MapPointType.Base;
                            break;
                        case 2:
                            mpt = Enums.MapPointType.XelNagaTower;
                            break;
                        case 3:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                        case 4:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                        default:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                    }

                    list.Add(new MapPoint(dist, degree, mpt, Enums.WasPlaced.NotAttempted));
                }

                for (var j = 1; j < 7; j++)
                {
                    var dist = Random.NextDouble() + 1.0;
                    var degree = Random.Next(181, 360);
                    Enums.MapPointType mpt;
                    switch (j)
                    {
                        case 0:
                            mpt = Enums.MapPointType.Base;
                            break;
                        case 1:
                            mpt = Enums.MapPointType.Base;
                            break;
                        case 2:
                            mpt = Enums.MapPointType.XelNagaTower;
                            break;
                        case 3:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                        case 4:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                        default:
                            mpt = Enums.MapPointType.Ramp;
                            break;
                    }

                    list.Add(new MapPoint(dist, degree, mpt, Enums.WasPlaced.NotAttempted));
                }

                var ms = new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, list);
                InfeasiblePopulation.CurrentGeneration.Add(ms);
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
