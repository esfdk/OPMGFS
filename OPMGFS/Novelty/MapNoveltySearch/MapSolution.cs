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
        #region Fields
        /// <summary>
        /// The phenotype matching this solution.
        /// </summary>
        private MapPhenotype convertedPhenotype;

        /// <summary>
        /// Whether this solution has been converted to its phenotype.
        /// </summary>
        private bool hasBeenConverted;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MapSolution"/> class.
        /// </summary>
        /// <param name="mso">
        /// The map search options.
        /// </param>
        /// <param name="noveltySearchOptions">
        /// The novelty search options.
        /// </param>
        public MapSolution(MapSearchOptions mso, NoveltySearchOptions noveltySearchOptions)
        {
            this.MapSearchOptions = mso;
            this.NoveltySearchOptions = noveltySearchOptions;
            this.MapPoints = new List<MapPoint>();
            this.convertedPhenotype = null;
            this.hasBeenConverted = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSolution"/> class.
        /// </summary>
        /// <param name="mso">
        /// The map search options.
        /// </param>
        /// <param name="nso">
        /// The novelty search options.
        /// </param>
        /// <param name="mapPoints">
        /// The map points to start with.
        /// </param>
        public MapSolution(MapSearchOptions mso, NoveltySearchOptions nso, List<MapPoint> mapPoints)
            : this(mso, nso)
        {
            this.MapPoints = mapPoints;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the map points of this solution.
        /// </summary>
        public List<MapPoint> MapPoints { get; private set; }

        /// <summary>
        /// Gets the search options for this solution.
        /// </summary>
        public MapSearchOptions MapSearchOptions { get; private set; }

        /// <summary>
        /// Gets the novelty search options for this solution.
        /// </summary>
        public NoveltySearchOptions NoveltySearchOptions { get; private set; }

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
        #endregion

        #region Public Methods
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
            var newPoints = MapConversionHelper.MutateMapPoints(
                this.MapPoints,
                this.NoveltySearchOptions.MutationChance,
                this.MapSearchOptions,
                r);

            return new MapSolution(this.MapSearchOptions, this.NoveltySearchOptions, newPoints);
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

            map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, new MapSearchOptions(map, MapSearchOptions));

            return map;
        }

        /// <summary>
        /// The convert to phenotype.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <returns>
        /// The <see cref="MapPhenotype"/>.
        /// </returns>
        public MapPhenotype ConvertToPhenotype(MapPhenotype map)
        {
            map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, new MapSearchOptions(map, this.MapSearchOptions));

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
            var map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, this.MapSearchOptions);
            this.ConvertedPhenotype = map.CreateCompleteMap(Enums.Half.Top, this.MapSearchOptions.MapCompletion);
            this.hasBeenConverted = true;
            this.ConvertedPhenotype.PlaceCliffs();
            this.ConvertedPhenotype.SmoothTerrain(this.MapSearchOptions.SmoothingNormalNeighborhood, this.MapSearchOptions.SmoothingExtendedNeighborhood, this.MapSearchOptions.SmoothingGenerations, this.MapSearchOptions.SmoothingRuleset);
            return this.ConvertedPhenotype;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override string ToString()
        {
            var s = this.MapPoints.Aggregate("--------------------\n", (current, mp) => current + (mp.ToString() + "\n"));

            s += "--------------------";
            return s;
        }
        #endregion

        #region Protected Methods
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
                var maximumDistance = mp.Type == Enums.MapPointType.StartBase ? this.MapSearchOptions.MaximumStartBaseDistance : this.MapSearchOptions.MaximumDistance;
                var minimumDistance = mp.Type == Enums.MapPointType.StartBase ? this.MapSearchOptions.MinimumStartBaseDistance : this.MapSearchOptions.MinimumDistance;

                var dist = 0.0;

                // Degree difference
                if (mp.Degree > this.MapSearchOptions.MaximumDegree)
                {
                    dist += mp.Degree - this.MapSearchOptions.MaximumDegree;
                }
                else if (mp.Degree < this.MapSearchOptions.MinimumDegree)
                {
                    dist += this.MapSearchOptions.MinimumDegree - mp.Degree;
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

                dist *= this.MapSearchOptions.NotPlacedPenaltyModifier;

                distance += dist;
            }

            distance = distance / this.MapPoints.Count();

            // Too many / too few elements placed
            var elements = MapConversionHelper.CalculateNumberOfMapPointsOutsideTypeBounds(this.MapPoints, this.MapSearchOptions);
            distance += elements.Item1 * this.MapSearchOptions.TooFewElementsPenalty;
            distance += elements.Item2 * this.MapSearchOptions.TooManyElementsPenalty;

            // Check if there is a path between start bases
            var sb = this.MapPoints.FirstOrDefault(mp => mp.Type == Enums.MapPointType.StartBase);
            if (sb != null && sb.WasPlaced == Enums.WasPlaced.Yes)
            {
                var startBaseTile = MapConversionHelper.FindNearestItemTileOfType(
                    this.ConvertedPhenotype.XSize / 2,
                    this.ConvertedPhenotype.YSize,
                    this.ConvertedPhenotype,
                    Enums.Item.StartBase);
                var topBasePoint = new Tuple<int, int>(startBaseTile.Item1, startBaseTile.Item2);
                Tuple<int, int> bottomBasePoint;
                switch (this.MapSearchOptions.MapCompletion)
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

                var jps = new JPSMapPathfinding(this.ConvertedPhenotype.HeightLevels, this.ConvertedPhenotype.DestructibleRocks);

                distance =
                    jps.FindPathFromTo(topBasePoint, bottomBasePoint).Count == 0
                        ? distance + this.MapSearchOptions.NoPathBetweenStartBasesPenalty
                        : distance;
            }
            else
            {
                distance += this.MapSearchOptions.NoPathBetweenStartBasesPenalty;
            }

            this.DistanceToFeasibility = distance;

            return this.DistanceToFeasibility;
        }
        #endregion
    }
}
