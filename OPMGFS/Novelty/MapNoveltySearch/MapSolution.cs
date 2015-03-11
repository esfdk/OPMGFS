namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Map;
    using Map.MapObjects;

    /// <summary>
    /// A solution/individual in searching the Starcraft map space.
    /// </summary>
    public class MapSolution : Solution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapSolution"/> class.
        /// </summary>
        public MapSolution()
        {
            this.MapPoints = new List<MapPoint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSolution"/> class.
        /// </summary>
        /// <param name="mapPoints">
        /// The map points to start with.
        /// </param>
        public MapSolution(List<MapPoint> mapPoints)
        {
            this.MapPoints = mapPoints;
        }

        /// <summary>
        /// Gets the map points of this solution.
        /// </summary>
        public List<MapPoint> MapPoints { get; private set; }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override Solution Mutate(Random r)
        {
            const double Chance = 0.3;

            // var maxDistMod = 0;
            // var maxDegreeMod = 10.0;
            // var maxDist = 128;
            // var maxDegree = 180;

            // TODO: Figure out settings locations
            var newPoints = this.MapPoints.Select(mp => r.NextDouble() < Chance ? mp.Mutate(r) : mp).ToList();

            // TODO: Should we ever remove a point? When? When do we stop adding?
            return new MapSolution(newPoints);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override Solution Recombine(Solution other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the novelty of a map. The novelty of the map is the average distance from this to other close solutions.
        /// </summary>
        /// <param name="feasible">The feasible population.</param>
        /// <param name="archive">The novel archive.</param>
        /// <param name="numberOfNeighbours">The number of neighbours used in novelty calculation.</param>
        /// <returns>The novelty of this solution.</returns>
        public override double CalculateNovelty(Population feasible, NovelArchive archive, int numberOfNeighbours)
        {
            // TODO: Control number of elements in novel archive? Per generation?
            var distanceValues = new List<Tuple<double, int, int>>();

            // Feasible population distance
            for (var i = 0; i < feasible.CurrentGeneration.Count; i++)
            {
                if (feasible.CurrentGeneration[i] == this)
                {
                    continue;
                }

                var distance = this.CalculateDistance(feasible.CurrentGeneration[i]);

                var wasAdded = false;

                for (var j = 0; j < distanceValues.Count; j++)
                {
                    if (!(distance < distanceValues[j].Item1))
                    {
                        continue;
                    }

                    distanceValues.Insert(j, new Tuple<double, int, int>(distance, i, 1));
                    wasAdded = true;
                    break;
                }

                if (!wasAdded)
                {
                    distanceValues.Add(new Tuple<double, int, int>(distance, i, 1));
                }
            }

            // Novelty archive distance
            for (var i = 0; i < archive.Archive.Count; i++)
            {
                var distance = this.CalculateDistance(archive.Archive[i]);

                var wasAdded = false;

                for (var j = 0; j < distanceValues.Count; j++)
                {
                    if (!(distance < distanceValues[j].Item1))
                    {
                        continue;
                    }

                    distanceValues.Insert(j, new Tuple<double, int, int>(distance, i, 2));
                    wasAdded = true;
                    break;
                }

                if (!wasAdded)
                {
                    distanceValues.Add(new Tuple<double, int, int>(distance, i, 2));
                }
            }

            var novelty = 0.0;

            for (var i = 0; i < numberOfNeighbours; i++)
            {
                novelty += distanceValues[i].Item1;
            }

            this.Novelty = novelty / numberOfNeighbours;
            return this.Novelty;
        }

        /// <summary>
        /// Converts this individual to a map.
        /// </summary>
        /// <param name="xSize">
        /// The width of the map.
        /// </param>
        /// <param name="ySize">
        /// The height of the map.
        /// </param>
        /// <returns>
        /// The <see cref="MapPhenotype"/>.
        /// </returns>
        public MapPhenotype ConvertToPhenotype(int xSize, int ySize)
        {
            var map = new MapPhenotype(ySize, xSize);

            map = ConvertToPhenotype(map);

            return map;
        }

        /// <summary>
        /// Fills in a map using the map points of this individual. Returns a filled in copy of the original map.
        /// </summary>
        /// <param name="map">The map to fill in.</param>
        /// <returns>The newly filled in map.</returns>
        public MapPhenotype ConvertToPhenotype(MapPhenotype map)
        {
            var newMap = new MapPhenotype(map.HeightLevels, map.MapItems);

            foreach (var mp in this.MapPoints)
            {
                var maxDistance = MaxDistanceAtDegree(map.XSize / 2.0, map.YSize / 2.0, mp.Degree);
                var point = FindPoint(mp.Degree, maxDistance * mp.Distance);

                var xPos = (int)(point.Item1 + (map.XSize / 2.0));
                var yPos = (int)(point.Item2 + (map.YSize / 2.0));

                Console.WriteLine(xPos + " " + yPos);

                if (!newMap.InsideBounds(xPos, yPos))
                {
                    throw new ArgumentOutOfRangeException();
                }

                switch (mp.Type)
                {
                    case Enums.MapPointType.Base:
                        PlaceBase(xPos, yPos, map);
                        break;
                    case Enums.MapPointType.GoldBase:
                        PlaceBase(xPos, yPos, map, isGoldBase: true);
                        break;
                    case Enums.MapPointType.StartBase:
                        PlaceStartBase(xPos, yPos, map);
                        break;
                    case Enums.MapPointType.XelNagaTower:
                        PlaceXelNagaTower(xPos, yPos, map);
                        break;
                    case Enums.MapPointType.Ramp:
                        var location = FindClosestCliff(xPos, yPos, map);
                        PlaceRamp(location.Item1, location.Item2, map);
                        break;
                    case Enums.MapPointType.DestructibleRocks:
                        PlaceDestructibleRocks(xPos, yPos, map);
                        break;
                    default:
                        map.MapItems[yPos, xPos] = Enums.Item.None;
                        break;
                }
            }

            return map;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override string ToString()
        {
            var s = this.MapPoints.Aggregate("--------------------\n", (current, mp) => current + (mp.ToString() + "\n"));

            s += "--------------------";
            return s;
        }

        /// <summary>
        /// The distance between two solutions is the distance of each element in one solution to every other element in the other solution.
        /// </summary>
        /// <param name="other">The solution to measure the distance to.</param>
        /// <returns>The distance between this and the target solution.</returns>
        protected override double CalculateDistance(Solution other)
        {
            if (this.GetType() != other.GetType())
            {
                return double.NegativeInfinity;
            }

            var dist = 0.0;
            var target = (MapSolution)other;

            foreach (var mapPoint in this.MapPoints)
            {
                foreach (var targetPoint in target.MapPoints)
                {
                    dist += Math.Abs(mapPoint.Degree - targetPoint.Degree);
                    dist += Math.Abs(mapPoint.Distance - targetPoint.Distance);
                }
            }

            return dist;
        }

        /// <summary>
        /// Calculates how far this solution is from feasibility.
        /// </summary>
        /// <returns>The distance to feasibility from this solution.</returns>
        protected override double CalculateDistanceToFeasibility()
        {
            // TODO: Make feasibility calculation better
            // TODO: Fix min/max distance & degree
            const double MaxDistance = 1.0;
            const int MaxDegree = 180;
            const int MinDegree = 0;
            const int MinDistance = 0;

            var distance = 0.0;

            foreach (var mp in this.MapPoints)
            {
                // Degree distance
                if (mp.Degree > MaxDegree)
                {
                    distance += mp.Degree - MaxDegree;
                }
                else if (mp.Degree < MinDegree)
                {
                    distance += MaxDegree - mp.Degree;
                }

                // Distance distance!
                if (mp.Distance > MaxDistance)
                {
                    distance += mp.Distance - MaxDistance;
                }
                else if (mp.Degree < MinDistance)
                {
                    distance += MaxDistance - mp.Distance;
                }
            }

            this.DistanceToFeasibility = distance;

            return DistanceToFeasibility;
        }

        /// <summary>
        /// Places a 2x2 tile object (such as destructible debris and Xel'Naga towers).
        /// </summary>
        /// <param name="x">The starting x-coordinate.</param>
        /// <param name="y">The starting y-coordinate.</param>
        /// <param name="mp">The map phenotype to place the object on.</param>
        /// <param name="ItemToPlace">The item to place on the map.</param>
        private static void PlaceTwoByTwo(int x, int y, MapPhenotype mp, Enums.Item ItemToPlace)
        {
            // TODO: Add check to not overwrite base
            if (!mp.InsideBounds(x, y))
            {
                return;
            }

            if (mp.InsideBounds(x + 1, y) && mp.InsideBounds(x, y + 1) && mp.InsideBounds(x + 1, y + 1))
            {
                // Bottom-left
                FlattenArea(mp.HeightLevels[x, y], x, y, 2, 2, mp);
                mp.MapItems[x, y] = ItemToPlace;
                mp.MapItems[x + 1, y] = ItemToPlace;
                mp.MapItems[x, y + 1] = ItemToPlace;
                mp.MapItems[x + 1, y + 1] = ItemToPlace;
            }
            else if (mp.InsideBounds(x - 1, y) && mp.InsideBounds(x, y + 1) && mp.InsideBounds(x - 1, y + 1))
            {
                // Bottom-right
                FlattenArea(mp.HeightLevels[x - 1, y], x, y, 2, 2, mp);
                mp.MapItems[x, y] = ItemToPlace;
                mp.MapItems[x - 1, y] = ItemToPlace;
                mp.MapItems[x, y + 1] = ItemToPlace;
                mp.MapItems[x - 1, y + 1] = ItemToPlace;
            }
            else if (mp.InsideBounds(x + 1, y) && mp.InsideBounds(x, y - 1) && mp.InsideBounds(x + 1, y - 1))
            {
                // Top-left
                FlattenArea(mp.HeightLevels[x, y - 1], x, y, 2, 2, mp);
                mp.MapItems[x, y] = ItemToPlace;
                mp.MapItems[x + 1, y] = ItemToPlace;
                mp.MapItems[x, y - 1] = ItemToPlace;
                mp.MapItems[x + 1, y - 1] = ItemToPlace;
            }
            else if (mp.InsideBounds(x - 1, y) && mp.InsideBounds(x, y - 1) && mp.InsideBounds(x - 1, y - 1))
            {
                // Top-right
                FlattenArea(mp.HeightLevels[x - 1, y - 1], x, y, 2, 2, mp);
                mp.MapItems[x, y] = ItemToPlace;
                mp.MapItems[x - 1, y] = ItemToPlace;
                mp.MapItems[x, y - 1] = ItemToPlace;
                mp.MapItems[x - 1, y - 1] = ItemToPlace;
            }
        }

        /// <summary>
        /// Places a Xel'Naga Tower taking up 2x2 spaces on the given map. The tower will not be placed if it is put outside bounds. It will also not overwrite any bases/minerals.
        /// </summary>
        /// <param name="y">The y-location to attempt to place the Xel'Naga tower.</param>
        /// <param name="x">The x-location to attempt to place the Xel'Naga tower.</param>
        /// <param name="mp">The map phenotype to place the tower on.</param>
        private static void PlaceXelNagaTower(int x, int y, MapPhenotype mp)
        {
            PlaceTwoByTwo(x,y,mp,Enums.Item.XelNagaTower);
        }

        private static void PlaceMinerals(int x, int y, MapPhenotype mp, bool isGold = false)
        {
            if (!mp.InsideBounds(x, y) || !mp.InsideBounds(x + 1, y))
            {
                return;
            }

            FlattenArea(mp.HeightLevels[x, y], x, y, 2, 1, mp);

            mp.MapItems[x, y] = isGold ? Enums.Item.GoldMinerals : Enums.Item.BlueMinerals;
            mp.MapItems[x + 1, y] = isGold ? Enums.Item.GoldMinerals : Enums.Item.BlueMinerals;
        }

        private static void PlaceGas(int x, int y, MapPhenotype mp)
        {
            if (!mp.InsideBounds(x, y) || !mp.InsideBounds(x + 2, y) || !mp.InsideBounds(x, y + 2) || !mp.InsideBounds(x + 2, y + 2))
            {
                return;
            }

            FlattenArea(mp.HeightLevels[x, y], x, y, 3, 3, mp);

            for (var i = x; i < x + 3; i++)
            {
                for (var j = y; j < y + 3; j++)
                {
                    mp.MapItems[i, j] = Enums.Item.Gas;
                }
            }
        }

        private static void PlaceStartBase(int x, int y, MapPhenotype mp)
        {
            if (!mp.InsideBounds(x, y))
            {
                return;
            }

            if (!mp.InsideBounds(x - 11, y - 11) || !mp.InsideBounds(x - 11, y + 12) || !mp.InsideBounds(x + 12, y - 11) ||
                !mp.InsideBounds(x + 12, y + 12))
            {
                return;
            }

            FlattenArea(mp.HeightLevels[x, y], x - 11, y - 11, 24, 24, mp); // TODO: Test

            for (var sbx = (x - 2); sbx < (x - 2 + 5); sbx++)
            {
                for (var sby = (y - 2); sby < (y - 2 + 5); sby++)
                {
                    mp.MapItems[sbx, sby] = Enums.Item.StartBase;
                }
            }

            // Minerals
            PlaceMinerals(x - 2, y + 7, mp);
            PlaceMinerals(x, y + 6, mp);
            PlaceMinerals(x - 3, y + 6, mp);
            PlaceMinerals(x - 6, y + 5, mp);
            PlaceMinerals(x - 7, y + 4, mp);
            PlaceMinerals(x - 7, y + 2, mp);
            PlaceMinerals(x - 8, y + 1, mp);
            PlaceMinerals(x - 7, y - 1, mp);
            // Gas
            PlaceGas(x - 8, y - 5, mp);
            PlaceGas(x + 3, y + 6, mp);
        }

        private static void PlaceBase(int x, int y, MapPhenotype mp, bool isGoldBase = false)
        {
            // TODO: Height level adjustment
            if (!mp.InsideBounds(x, y))
            {
                return;
            }

            if (!mp.InsideBounds(x - 7, y - 7) || !mp.InsideBounds(x - 7, y + 8) || !mp.InsideBounds(x + 8, y - 7) ||
                !mp.InsideBounds(x + 8, y + 8))
            {
                return;
            }

            FlattenArea(mp.HeightLevels[x,y], x - 7, y - 7, 16, 16, mp);

            var rx = x - 1;
            var ry = y - 3;

            for (var sbx = (rx); sbx < (rx + 5); sbx++)
            {
                for (var sby = (ry); sby < (ry + 5); sby++)
                {
                    mp.MapItems[sbx, sby] = Enums.Item.Base;
                }
            }

            // Minerals
            PlaceMinerals(rx, ry + 9, mp, isGoldBase);
            PlaceMinerals(rx - 1, ry + 8, mp, isGoldBase);
            PlaceMinerals(rx + 2, ry + 8, mp, isGoldBase);
            PlaceMinerals(rx - 4, ry + 7, mp, isGoldBase);
            PlaceMinerals(rx - 5, ry + 6, mp, isGoldBase);
            PlaceMinerals(rx - 5, ry + 4, mp, isGoldBase);
            PlaceMinerals(rx - 6, ry + 3, mp, isGoldBase);
            PlaceMinerals(rx - 5, ry + 1, mp, isGoldBase);
            // Gas
            PlaceGas(rx - 6, ry - 3, mp);
            PlaceGas(rx + 5, ry + 8, mp);
        }

        /// <summary>
        /// Flattens a square area of the map to a specific height.
        /// </summary>
        /// <param name="height">The height to flatten to.</param>
        /// <param name="startX">X-coordinate of the bottom-left corner.</param>
        /// <param name="startY">Y-coordinate of the bottom-left corner.</param>
        /// <param name="lengthX">The length on the x-axis.</param>
        /// <param name="lengthY">The length on the y-axis.</param>
        /// <param name="mp">The map phenotype to flatten the area on.</param>
        private static void FlattenArea(Enums.HeightLevel height, int startX, int startY, int lengthX, int lengthY, MapPhenotype mp)
        {
            // TODO: Implement flattening
        }

        /// <summary>
        /// Places destructible debris taking up 2x2 spaces on the given map. The debris will not be placed if it is put outside bounds. It will also not overwrite any bases/minerals.
        /// </summary>
        /// <param name="y">The y-location to attempt to place the debris.</param>
        /// <param name="x">The x-location to attempt to place the debris.</param>
        /// <param name="mp">The map phenotype to place the debris on.</param>
        private static void PlaceDestructibleRocks(int x, int y, MapPhenotype mp)
        {
            PlaceTwoByTwo(x,y,mp,Enums.Item.DestructibleRocks);
        }

        private static Tuple<int, int> FindClosestCliff(int startX, int startY, MapPhenotype mp)
        {
            // TODO: Implement searching for cliff
            throw new NotImplementedException();
        }

        private static void PlaceRamp(int x, int y, MapPhenotype mp)
        {
            // TODO: Find ramps
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the distance to the edge of the map from the centre at a given angle.
        /// </summary>
        /// <param name="xSize">The width of the map.</param>
        /// <param name="ySize">The height of the map.</param>
        /// <param name="degree">The degree to look at.</param>
        /// <returns>The distance to the edge of the map.</returns>
        private static double MaxDistanceAtDegree(double xSize, double ySize, double degree)
        {
            var squared = Math.Pow(xSize, 2) + Math.Pow(ySize, 2);
            var maxDistance = Math.Sqrt(squared);
            var radians = ConvertToRadians(degree);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            var stop = false;

            do
            {
                // TODO: Optimize distance function calculation
                var point = FindPoint(maxDistance, cos, sin);

                if (
                    (!(point.Item1 <= xSize) || (point.Item1 < -xSize))
                    || (!(point.Item2 <= ySize) || (point.Item2 < -ySize)))
                {
                    maxDistance--;
                }
                else
                {
                    stop = true;
                }
            }
            while (!stop);

            return maxDistance;
        }

        /// <summary>
        /// Converts an angle in degrees to radians.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>The angle in radians.</returns>
        private static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        /// <summary>
        /// Finds a point based on an angle and distance.
        /// </summary>
        /// <param name="degree">The angle in degrees.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>The point.</returns>
        private static Tuple<double, double> FindPoint(double degree, double distance)
        {
            var radians = ConvertToRadians(degree);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            return FindPoint(distance, cos, sin);
        }

        /// <summary>
        /// Finds a point based on a distance and the cos and sin values of an angle.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="cos">The cos-value of the angle.</param>
        /// <param name="sin">The sin-value of the angle.</param>
        /// <returns>The point.</returns>
        private static Tuple<double, double> FindPoint(double distance, double cos, double sin)
        {
            var xDist = cos * distance;
            var yDist = sin * distance;

            return new Tuple<double, double>(xDist, yDist);
        }
    }
}
