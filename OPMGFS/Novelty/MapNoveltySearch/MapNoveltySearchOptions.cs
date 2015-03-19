namespace OPMGFS.Novelty.MapNoveltySearch
{
    using OPMGFS.Map;

    /// <summary>
    /// Novelty Search Options for map searching.
    /// </summary>
    public class MapNoveltySearchOptions : NoveltySearchOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapNoveltySearchOptions"/> class. 
        /// </summary>
        /// <param name="mp">
        /// The map phenotype to search on.
        /// </param>
        /// <param name="distanceNotPlaced">
        /// The amount to add to the distance when a map point was not placed during conversion. 
        /// </param>
        /// <param name="maximumDegree">
        /// The maximum Degree.
        /// </param>
        /// <param name="minimumDegree">
        /// The minimum Degree.
        /// </param>
        /// <param name="maximumDistance">
        /// The maximum Distance.
        /// </param>
        /// <param name="minimumDistance">
        /// The minimum Distance.
        /// </param>
        /// <param name="maximumDistanceModifier">
        /// The maximum Distance Modifier.
        /// </param>
        /// <param name="minimumDistanceModifer">
        /// The minimum Distance Modifer.
        /// </param>
        /// <param name="maximumDegreeModifier">
        /// The maximum Degree Modifier.
        /// </param>
        /// <param name="minimumDegreeModifer">
        /// The minimum Degree Modifer.
        /// </param>
        /// <param name="minimumNumberOfBases">
        /// The minimum Number Of Bases.
        /// </param>
        /// <param name="maximumNumberOfBases">
        /// The maximum Number Of Bases.
        /// </param>
        /// <param name="minimumNumberOfRamps">
        /// The minimum Number Of Ramps.
        /// </param>
        /// <param name="maximumNumberOfRamps">
        /// The maximum Number Of Ramps.
        /// </param>
        /// <param name="minimumNumberOfDestructibleRocks">
        /// The minimum Number Of Destructible Rocks.
        /// </param>
        /// <param name="maximumNumberOfDestructibleRocks">
        /// The maximum Number Of Destructible Rocks.
        /// </param>
        /// <param name="minimumNumberOfXelNagaTowers">
        /// The minimum Number Of Xel Naga Towers.
        /// </param>
        /// <param name="maximumNumberOfXelNagaTowers">
        /// The maximum Number Of Xel Naga Towers.
        /// </param>
        /// <param name="mutate">
        /// Whether the novelty search should use mutation. 
        /// </param>
        /// <param name="recombine">
        /// Whether the novelty search should use recombination. 
        /// </param>
        /// <param name="mutationChance">
        /// The chance of a mutation happening. 
        /// </param>
        /// <param name="twoPointCrossover">
        /// Whether the novelty search should use single point or two point crossover when recombining. 
        /// </param>
        /// <param name="numberOfNeighbours">
        /// The number of neighbours to use in calculation of novelty. 
        /// </param>
        /// <param name="addToArchive">
        /// The amount of individuals to add to the novel archive. 
        /// </param>
        /// <param name="minimumNovelty">
        /// The minimum novelty required for an individual to be added to the novel archive.
        /// </param>
        public MapNoveltySearchOptions(
            MapPhenotype mp,
            double distanceNotPlaced = 10,
            double maximumDegree = 180,
            double minimumDegree = 0,
            double maximumDistance = 1.0,
            double minimumDistance = 0.0,
            double maximumDistanceModifier = 0.05,
            double minimumDistanceModifer = 0.01,
            double maximumDegreeModifier = 15,
            double minimumDegreeModifer = 1,
            int minimumNumberOfBases = 1,
            int maximumNumberOfBases = 2,
            int minimumNumberOfRamps = 3,
            int maximumNumberOfRamps = 5,
            int minimumNumberOfDestructibleRocks = 1,
            int maximumNumberOfDestructibleRocks = 3,
            int minimumNumberOfXelNagaTowers = 1,
            int maximumNumberOfXelNagaTowers = 1,
            bool mutate = true,
            bool recombine = false,
            double mutationChance = 0.3,
            bool twoPointCrossover = false,
            int numberOfNeighbours = 1,
            int addToArchive = 1,
            double minimumNovelty = 0)
            : base(mutate, recombine, mutationChance, twoPointCrossover, numberOfNeighbours, addToArchive, minimumNovelty)
        {
            this.MaximumNumberOfXelNagaTowers = maximumNumberOfXelNagaTowers;
            this.MinimumNumberOfXelNagaTowers = minimumNumberOfXelNagaTowers;
            this.MaximumNumberOfDestructibleRocks = maximumNumberOfDestructibleRocks;
            this.MinimumNumberOfDestructibleRocks = minimumNumberOfDestructibleRocks;
            this.MaximumNumberOfRamps = maximumNumberOfRamps;
            this.MinimumNumberOfRamps = minimumNumberOfRamps;
            this.MaximumNumberOfBases = maximumNumberOfBases;
            this.MinimumNumberOfBases = minimumNumberOfBases;
            this.MinimumDegreeModifer = minimumDegreeModifer;
            this.MaximumDegreeModifier = maximumDegreeModifier;
            this.MinimumDistanceModifer = minimumDistanceModifer;
            this.MaximumDistanceModifier = maximumDistanceModifier;
            this.MinimumDistance = minimumDistance;
            this.MaximumDistance = maximumDistance;
            this.MinimumDegree = minimumDegree;
            this.MaximumDegree = maximumDegree;
            this.DistanceNotPlaced = distanceNotPlaced;
            this.Map = mp;
        }

        /// <summary>
        /// Gets the map that is being searched.
        /// </summary>
        public MapPhenotype Map { get; private set; }

        /// <summary>
        /// Gets the distance to add whenever a map point was not placed when calculating distance.
        /// </summary>
        public double DistanceNotPlaced { get; private set; }

        /// <summary>
        /// Gets the maximum degree.
        /// </summary>
        public double MaximumDegree { get; private set; }

        /// <summary>
        /// Gets the minimum degree.
        /// </summary>
        public double MinimumDegree { get; private set; }

        /// <summary>
        /// Gets the maximum distance.
        /// </summary>
        public double MaximumDistance { get; private set; }

        /// <summary>
        /// Gets the minimum distance.
        /// </summary>
        public double MinimumDistance { get; private set; }

        /// <summary>
        /// Gets the maximum distance modifier.
        /// </summary>
        public double MaximumDistanceModifier { get; private set; }

        /// <summary>
        /// Gets the minimum distance modifer.
        /// </summary>
        public double MinimumDistanceModifer { get; private set; }

        /// <summary>
        /// Gets the maximum degree modifier.
        /// </summary>
        public double MaximumDegreeModifier { get; private set; }

        /// <summary>
        /// Gets the minimum degree modifer.
        /// </summary>
        public double MinimumDegreeModifer { get; private set; }

        /// <summary>
        /// Gets the minimum number of bases.
        /// </summary>
        public int MinimumNumberOfBases { get; private set; }

        /// <summary>
        /// Gets the maximum number of bases.
        /// </summary>
        public int MaximumNumberOfBases { get; private set; }

        /// <summary>
        /// Gets the minimum number of ramps.
        /// </summary>
        public int MinimumNumberOfRamps { get; private set; }

        /// <summary>
        /// Gets the maximum number of ramps.
        /// </summary>
        public int MaximumNumberOfRamps { get; private set; }

        /// <summary>
        /// Gets the minimum number of destructible rocks.
        /// </summary>
        public int MinimumNumberOfDestructibleRocks { get; private set; }

        /// <summary>
        /// Gets the maximum number of destructible rocks.
        /// </summary>
        public int MaximumNumberOfDestructibleRocks { get; private set; }

        /// <summary>
        /// Gets the minimum number of xel naga towers.
        /// </summary>
        public int MinimumNumberOfXelNagaTowers { get; private set; }

        /// <summary>
        /// Gets the maximum number of xel naga towers.
        /// </summary>
        public int MaximumNumberOfXelNagaTowers { get; private set; }
    }
}
