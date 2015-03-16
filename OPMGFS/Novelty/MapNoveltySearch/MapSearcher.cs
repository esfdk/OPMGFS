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
        /// <param name="r"> The random to use in searching. </param>
        /// <param name="feasibleSize"> The feasible size. </param>
        /// <param name="infeasibleSize"> The infeasible size. </param>
        public MapSearcher(Random r, int feasibleSize, int infeasibleSize) : base(r)
        {
            this.FeasiblePopulation = new MapPopulation(true, feasibleSize);
            this.InfeasiblePopulation = new MapPopulation(false, infeasibleSize);
            this.Archive = new MapNovelArchive();

            for (var i = 0; i < feasibleSize; i++)
            {
                var list = new List<MapPoint>();

                for (var j = 0; j < 5; j++)
                {
                    var dist = Random.NextDouble();
                    var degree = Random.Next(181);
                    Enums.MapPointType mpt;
                    switch (j)
                    {
                        case 0:
                            mpt = Enums.MapPointType.StartBase;
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
                            mpt = Enums.MapPointType.Base;
                            break;
                    }

                    list.Add(new MapPoint(dist, degree, mpt, Enums.WasPlaced.NotAttempted));
                }

                var ms = new MapSolution(list);
                FeasiblePopulation.CurrentGeneration.Add(ms);
            }

            for (var i = 0; i < feasibleSize; i++)
            {
                var list = new List<MapPoint>();

                for (var j = 0; j < 3; j++)
                {
                    var dist = -Random.NextDouble();
                    var degree = Random.Next(181, 360);
                    Enums.MapPointType mpt;
                    switch (j)
                    {
                        case 0:
                            mpt = Enums.MapPointType.StartBase;
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
                            mpt = Enums.MapPointType.Base;
                            break;
                    }

                    list.Add(new MapPoint(dist, degree, mpt, Enums.WasPlaced.NotAttempted));
                }

                for (var j = 0; j < 2; j++)
                {
                    var dist = Random.NextDouble() + 1.0;
                    var degree = Random.Next(181, 360);
                    Enums.MapPointType mpt;
                    switch (j)
                    {
                        case 0:
                            mpt = Enums.MapPointType.StartBase;
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
                            mpt = Enums.MapPointType.Base;
                            break;
                    }

                    list.Add(new MapPoint(dist, degree, mpt, Enums.WasPlaced.NotAttempted));
                }

                var ms = new MapSolution(list);
                InfeasiblePopulation.CurrentGeneration.Add(ms);
            }
        }

        /// <summary>
        /// Runs a number of generations to search for maps.
        /// </summary>
        /// <param name="generations">Number of generations to run.</param>
        public void RunGenerations(int generations)
        {
            /*
            Console.WriteLine("Generation 0");

            Console.WriteLine("-----------------");
            Console.WriteLine("Feasible Population");
            foreach (var ms in FeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((MapSolution)ms));
            }

            Console.WriteLine("------------------");
            Console.WriteLine("Infeasible Population");
            foreach (var ms in InfeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((MapSolution)ms));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("-------------------");
            for (var i = 0; i < generations; i++)
            {
                Console.WriteLine("Generation " + i);

                this.NextGeneration();
            }*/
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

            Console.WriteLine("-----------------");
            Console.WriteLine("Feasible Population");
            foreach (var ms in FeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(ms);
            }

            Console.WriteLine("------------------");
            Console.WriteLine("Infeasible Population");
            foreach (var ms in InfeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(ms);
            }

            Console.WriteLine("------------------");
        }
    }
}
