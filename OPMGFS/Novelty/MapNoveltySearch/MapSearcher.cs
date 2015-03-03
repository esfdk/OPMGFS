namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Collections.Generic;

    using OPMGFS.Map;
    using OPMGFS.Map.MapObjects;

    public class MapSearcher : NoveltySearcher
    {
        public MapSearcher(int feasibleSize, int infeasibleSize)
        {
            this.FeasiblePopulation = new MapPopulation(true, feasibleSize);
            this.InfeasiblePopulation = new MapPopulation(false, infeasibleSize);
            this.Archive = new MapNovelArchive();
            this.Random = new Random();

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

                    list.Add(new MapPoint(dist, degree,mpt));
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

                    list.Add(new MapPoint(dist, degree, mpt));
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

                    list.Add(new MapPoint(dist, degree, mpt));
                }

                var ms = new MapSolution(list);
                InfeasiblePopulation.CurrentGeneration.Add(ms);
            }
        }

        public void RunGenerations(int generations)
        {/*
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
                Console.WriteLine(((MapSolution)ms));
            }

            Console.WriteLine("------------------");
            Console.WriteLine("Infeasible Population");
            foreach (var ms in InfeasiblePopulation.CurrentGeneration)
            {
                Console.WriteLine(((MapSolution)ms));
            }
            Console.WriteLine("------------------");
        }
    }
}
