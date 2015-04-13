namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Map;
    using Map.MapObjects;

    /// <summary>
    /// A solution/individual in searching the StarCraft map space.
    /// </summary>
    public class MapSolution : Solution
    {
        /// <summary>
        /// The phenotype matching this solution.
        /// </summary>
        private MapPhenotype convertedPhenotype;

        /// <summary>
        /// Whether this solution has been converted to its phenotype.
        /// </summary>
        private bool hasBeenConverted;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSolution"/> class.
        /// </summary>
        /// <param name="mnso">
        /// The novelty search options.
        /// </param>
        public MapSolution(MapNoveltySearchOptions mnso)
        {
            this.MapPoints = new List<MapPoint>();
            this.SearchOptions = mnso;
            this.convertedPhenotype = null;
            this.hasBeenConverted = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSolution"/> class.
        /// </summary>
        /// <param name="mnso">
        /// The novelty search options.
        /// </param>
        /// <param name="mapPoints">
        /// The map points to start with.
        /// </param>
        public MapSolution(MapNoveltySearchOptions mnso, List<MapPoint> mapPoints)
            : this(mnso)
        {
            this.MapPoints = mapPoints;
        }

        /// <summary>
        /// Gets the map points of this solution.
        /// </summary>
        public List<MapPoint> MapPoints { get; private set; }

        /// <summary>
        /// Gets the search options for this solution.
        /// </summary>
        public MapNoveltySearchOptions SearchOptions { get; private set; }

        /// <summary>
        /// Gets the phenotype corresponding to this genotype.
        /// </summary>
        public MapPhenotype ConvertedPhenotype
        {
            get
            {
                return this.hasBeenConverted ? this.convertedPhenotype : this.ConvertToPhenotype();
            }

            private set
            {
                this.convertedPhenotype = value;
            }
        }

        /// <summary>
        /// Mutates this solution into a new solution.
        /// </summary>
        /// <param name="r">
        /// The random generator to use in mutation.
        /// </param>
        /// <returns>
        /// The newly mutated solution.
        /// </returns>
        public override Solution Mutate(Random r)
        {
            var newPoints = this.MapPoints.Select(mp => r.NextDouble() < this.SearchOptions.MutationChance ? mp.Mutate(r, this.SearchOptions) : mp).ToList();

            var randomNumber = r.NextDouble();

            if (randomNumber < this.SearchOptions.ChanceToAddNewElement)
            {
                if (r.NextDouble() < this.SearchOptions.ChanceToAddBase)
                {
                    if (this.SearchOptions.MaximumNumberOfBases > newPoints.Count(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase))
                    {
                        newPoints.Add(new MapPoint(r.NextDouble(), r.Next(0, 181), Enums.MapPointType.Base, Enums.WasPlaced.NotAttempted));
                    }
                }
                else if (randomNumber < this.SearchOptions.ChanceToAddBase + this.SearchOptions.ChanceToAddGoldBase)
                {
                    if (this.SearchOptions.MaximumNumberOfBases > newPoints.Count(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.GoldBase,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else if (randomNumber < this.SearchOptions.ChanceToAddBase + this.SearchOptions.ChanceToAddGoldBase + this.SearchOptions.ChanceToAddXelNagaTower)
                {
                    if (this.SearchOptions.MaximumNumberOfXelNagaTowers
                        > newPoints.Count(mp => mp.Type == Enums.MapPointType.XelNagaTower))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.XelNagaTower,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else if (randomNumber
                         < this.SearchOptions.ChanceToAddBase + this.SearchOptions.ChanceToAddGoldBase
                         + this.SearchOptions.ChanceToAddXelNagaTower + this.SearchOptions.ChanceToAddDestructibleRocks)
                {
                    Console.WriteLine("this happened");
                    if (this.SearchOptions.MaximumNumberOfDestructibleRocks
                        > newPoints.Count(mp => mp.Type == Enums.MapPointType.DestructibleRocks))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.DestructibleRocks,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else
                {
                    if (this.SearchOptions.MaximumNumberOfRamps
                        > newPoints.Count(mp => mp.Type == Enums.MapPointType.Ramp))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.Ramp,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
            }

            if (r.NextDouble() < this.SearchOptions.ChanceToRemoveElement)
            {
                var potentialMapPoints = new List<MapPoint>();
                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase) > this.SearchOptions.MinimumNumberOfBases)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase));
                }

                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.XelNagaTower) > this.SearchOptions.MinimumNumberOfXelNagaTowers)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.XelNagaTower));
                }

                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.Ramp) > this.SearchOptions.MinimumNumberOfRamps)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.Ramp));
                }
            }
            return new MapSolution(this.SearchOptions, newPoints);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override Solution Recombine(Solution other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the novelty of a map. The novelty of the map is the average distance from this to other close solutions.
        /// </summary>
        /// <param name="feasible">
        /// The feasible population.
        /// </param>
        /// <param name="archive">
        /// The novel archive.
        /// </param>
        /// <param name="numberOfNeighbours">
        /// The number of neighbours used in novelty calculation.
        /// </param>
        /// <returns>
        /// The novelty of this solution.
        /// </returns>
        public override double CalculateNovelty(Population feasible, NovelArchive archive, int numberOfNeighbours)
        {
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

            map = this.ConvertToPhenotype(map);

            return map;
        }

        /// <summary>
        /// Converts this individual to a map.
        /// </summary>
        /// <returns>
        /// The <see cref="MapPhenotype"/>.
        /// </returns>
        public MapPhenotype ConvertToPhenotype()
        {
            var map = this.ConvertToPhenotype(this.SearchOptions.Map);
            this.ConvertedPhenotype = map.CreateCompleteMap(Enums.Half.Top, this.SearchOptions.MapCompletion);
            this.hasBeenConverted = true;
            this.ConvertedPhenotype.PlaceCliffs();
            return this.ConvertedPhenotype;
        }

        /// <summary>
        /// Fills in a map using the map points of this individual. Returns a filled in copy of the original map.
        /// </summary>
        /// <param name="map">
        /// The map to fill in.
        /// </param>
        /// <returns>
        /// The newly filled in map.
        /// </returns>
        public MapPhenotype ConvertToPhenotype(MapPhenotype map)
        {
            var heightLevels = map.HeightLevels.Clone() as Enums.HeightLevel[,];
            var items = map.MapItems.Clone() as Enums.Item[,];

            var newMap = new MapPhenotype(heightLevels, items);

            foreach (var mp in this.MapPoints)
            {
                if (mp.Degree < this.SearchOptions.MinimumDegree || mp.Degree > this.SearchOptions.MaximumDegree
                    || mp.Distance < this.SearchOptions.MinimumDistance || mp.Distance > this.SearchOptions.MaximumDistance)
                {
                    mp.WasPlaced = Enums.WasPlaced.No;
                    continue;
                }

                var maxDistance = MaxDistanceAtDegree(newMap.XSize / 2.0, newMap.YSize / 2.0, mp.Degree);
                var point = FindPoint(mp.Degree, maxDistance * mp.Distance);

                var xPos = (int)(point.Item1 + (newMap.XSize / 2.0));
                var yPos = (int)(point.Item2 + (newMap.YSize / 2.0));

                if (!newMap.InsideTopHalf(xPos, yPos))
                {
                    throw new ArgumentOutOfRangeException();
                }

                bool placed;

                switch (mp.Type)
                {
                    case Enums.MapPointType.Base:
                        placed = MapSolutionConverter.PlaceBase(xPos, yPos, newMap);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= SearchOptions.MaximumDisplacement;
                                 i += SearchOptions.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                 j <= SearchOptions.MaximumDisplacement;
                                 j += SearchOptions.DisplacementAmountPerStep)
                                {
                                    if (MapSolutionConverter.PlaceBase(xPos - i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos + i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos - i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos + i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos - i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos + i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.GoldBase:
                        placed = MapSolutionConverter.PlaceBase(xPos, yPos, newMap, true);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= SearchOptions.MaximumDisplacement;
                                 i += SearchOptions.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                 j <= SearchOptions.MaximumDisplacement;
                                 j += SearchOptions.DisplacementAmountPerStep)
                                {
                                    if (MapSolutionConverter.PlaceBase(xPos - i, yPos + j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos, yPos + j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos + i, yPos + j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos - i, yPos, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos + i, yPos, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos - i, yPos - j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos, yPos - j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceBase(xPos + i, yPos - j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.StartBase:
                        placed = MapSolutionConverter.PlaceStartBase(xPos, yPos, newMap);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= SearchOptions.MaximumDisplacement;
                                 i += SearchOptions.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                 j <= SearchOptions.MaximumDisplacement;
                                 j += SearchOptions.DisplacementAmountPerStep)
                                {
                                    if (MapSolutionConverter.PlaceStartBase(xPos - i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos + i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos - i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos + i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos - i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceStartBase(xPos + i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.XelNagaTower:
                        placed = MapSolutionConverter.PlaceXelNagaTower(xPos, yPos, newMap);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= SearchOptions.MaximumDisplacement;
                                 i += SearchOptions.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                 j <= SearchOptions.MaximumDisplacement;
                                 j += SearchOptions.DisplacementAmountPerStep)
                                {
                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos - i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos + i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos - i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos + i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos - i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (MapSolutionConverter.PlaceXelNagaTower(xPos + i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.Ramp:
                        // TODO: Should we attempt to displace ramps?
                        var location = MapSolutionConverter.FindClosestCliff(xPos, yPos, newMap);
                        if (location == null) break;
                        mp.WasPlaced = MapSolutionConverter.PlaceRamp(location.Item1, location.Item2, newMap) ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.DestructibleRocks:
                        // TODO: Change way destructible rocks work
                        mp.WasPlaced = MapSolutionConverter.PlaceDestructibleRocks(xPos, yPos, newMap) ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    default:
                        newMap.MapItems[yPos, xPos] = Enums.Item.None;
                        break;
                }
            }

            return newMap;
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
        /// <param name="other">
        /// The solution to measure the distance to.
        /// </param>
        /// <returns>
        /// The distance between this and the target solution.
        /// </returns>
        protected override double CalculateDistance(Solution other)
        {
            var dist = 0.0;
            var target = other as MapSolution;

            if (target == null)
            {
                throw new ArgumentException("Input solution must be of type MapSolution!");
            }

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
            this.ConvertToPhenotype();

            var distance = 0.0;

            // Calculate distances between map points
            foreach (var mp in this.MapPoints.Where(mp => mp.WasPlaced != Enums.WasPlaced.Yes))
            {
                var maximumDistance = mp.Type == Enums.MapPointType.StartBase ? this.SearchOptions.MaximumStartBaseDistance : this.SearchOptions.MaximumDistance;
                var minimumDistance = mp.Type == Enums.MapPointType.StartBase ? this.SearchOptions.MinimumStartBaseDistance : this.SearchOptions.MinimumDistance;

                var dist = 0.0;

                // Degree difference
                if (mp.Degree > this.SearchOptions.MaximumDegree)
                {
                    dist += mp.Degree - this.SearchOptions.MaximumDegree;
                }
                else if (mp.Degree < this.SearchOptions.MinimumDegree)
                {
                    dist += this.SearchOptions.MinimumDegree - mp.Degree;
                }

                // Distance difference
                if (mp.Distance > maximumDistance)
                {
                    dist += mp.Distance - maximumDistance;
                }
                else if (mp.Degree < minimumDistance)
                {
                    dist += minimumDistance - mp.Distance;
                }

                dist *= this.SearchOptions.NotPlacedPenaltyModifier;

                distance += dist;
            }

            distance = distance / this.MapPoints.Count();

            // Too many / too few elements placed
            var elements = this.CalculateNumberOfMapPointsOutsideTypeBounds();
            distance += elements.Item1 * this.SearchOptions.TooFewElementsPenalty;
            distance += elements.Item2 * this.SearchOptions.TooManyElementsPenalty;

            // Check if there is a path between start bases
            var sb = this.MapPoints.FirstOrDefault(mp => mp.Type == Enums.MapPointType.StartBase);
            if (sb != null && sb.WasPlaced == Enums.WasPlaced.Yes)
            {
                var startBaseTile = MapSolutionConverter.FindNearestItemTileOfType(
                    this.ConvertedPhenotype.XSize / 2,
                    this.ConvertedPhenotype.YSize,
                    this.ConvertedPhenotype,
                    Enums.Item.StartBase);
                var topBasePoint = new Tuple<int, int>(startBaseTile.Item1, startBaseTile.Item2);
                Tuple<int, int> bottomBasePoint;
                switch (this.SearchOptions.MapCompletion)
                {
                    case Enums.MapFunction.Mirror:
                        bottomBasePoint = new Tuple<int, int>(
                            startBaseTile.Item1,
                            this.ConvertedPhenotype.YSize - startBaseTile.Item2 - 1);
                        break;
                    case Enums.MapFunction.Turn:
                        bottomBasePoint = new Tuple<int, int>(
                            this.ConvertedPhenotype.XSize - startBaseTile.Item1 - 1,
                            this.ConvertedPhenotype.YSize - startBaseTile.Item2 - 1);
                        break;
                    default:
                        bottomBasePoint = new Tuple<int, int>(
                            this.ConvertedPhenotype.XSize - startBaseTile.Item1 - 1,
                            this.ConvertedPhenotype.YSize - startBaseTile.Item2 - 1);
                        break;
                }

                distance =
                    MapPathfinding.FindPathFromTo(this.ConvertedPhenotype.HeightLevels, topBasePoint, bottomBasePoint)
                        .Count == 0
                        ? distance + this.SearchOptions.NoPathBetweenStartBases
                        : distance;
            }
            else
            {
                distance += this.SearchOptions.NoPathBetweenStartBases;
            }

            this.DistanceToFeasibility = distance;

            return this.DistanceToFeasibility;
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

        /// <summary>
        /// Calculates which of the map point types, if any, are out of bounds according to the search options.
        /// </summary>
        /// <returns>A tuple containing the number of nap points that are missing (item1) and the number of map points that are in excess (item2).</returns>
        private Tuple<int, int> CalculateNumberOfMapPointsOutsideTypeBounds()
        {
            var numberOfMapPointsMissing = 0;
            var numberOfTooManyMapPoints = 0;

            var numberOfBases = this.MapPoints.Count(mp => mp.Type == Enums.MapPointType.Base && mp.WasPlaced == Enums.WasPlaced.Yes);
            var numberOfDestructibleRocks = this.MapPoints.Count(mp => mp.Type == Enums.MapPointType.DestructibleRocks && mp.WasPlaced == Enums.WasPlaced.Yes);
            var numberOfRamps = this.MapPoints.Count(mp => mp.Type == Enums.MapPointType.Ramp && mp.WasPlaced == Enums.WasPlaced.Yes);
            var numberOfXelNagaTowers = this.MapPoints.Count(mp => mp.Type == Enums.MapPointType.XelNagaTower && mp.WasPlaced == Enums.WasPlaced.Yes);

            if (numberOfBases < this.SearchOptions.MinimumNumberOfBases)
            {
                numberOfMapPointsMissing += this.SearchOptions.MinimumNumberOfBases - numberOfBases;
            }
            else if (numberOfBases > this.SearchOptions.MaximumNumberOfBases)
            {
                numberOfTooManyMapPoints += numberOfBases - this.SearchOptions.MaximumNumberOfBases;
            }

            if (numberOfDestructibleRocks < this.SearchOptions.MinimumNumberOfDestructibleRocks)
            {
                numberOfMapPointsMissing += this.SearchOptions.MinimumNumberOfDestructibleRocks - numberOfDestructibleRocks;
            }
            else if (numberOfDestructibleRocks > this.SearchOptions.MaximumNumberOfDestructibleRocks)
            {
                numberOfTooManyMapPoints += numberOfDestructibleRocks - this.SearchOptions.MaximumNumberOfDestructibleRocks;
            }

            if (numberOfRamps < this.SearchOptions.MinimumNumberOfRamps)
            {
                numberOfMapPointsMissing += this.SearchOptions.MinimumNumberOfRamps - numberOfRamps;
            }
            else if (numberOfRamps > this.SearchOptions.MaximumNumberOfRamps)
            {
                numberOfTooManyMapPoints += numberOfRamps - this.SearchOptions.MaximumNumberOfRamps;
            }

            if (numberOfXelNagaTowers < this.SearchOptions.MinimumNumberOfXelNagaTowers)
            {
                numberOfMapPointsMissing += this.SearchOptions.MinimumNumberOfXelNagaTowers - numberOfXelNagaTowers;
            }
            else if (numberOfBases > this.SearchOptions.MaximumNumberOfXelNagaTowers)
            {
                numberOfTooManyMapPoints += numberOfXelNagaTowers - this.SearchOptions.MaximumNumberOfXelNagaTowers;
            }

            return new Tuple<int, int>(numberOfMapPointsMissing, numberOfTooManyMapPoints);
        }
    }
}
