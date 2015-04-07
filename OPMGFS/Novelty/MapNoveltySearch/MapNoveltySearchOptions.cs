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
        /// <param name="mapCompletion">
        /// The completion method to use when converting to phenotype.
        /// </param>
        /// <param name="maximumDisplacement">
        /// The maximum Displacement.
        /// </param>
        /// <param name="displacementAmountPerStep">
        /// The displacement Amount Per Step.
        /// </param>
        /// <param name="tooFewElementsPenalty">
        /// The element Not Placed Penalty.
        /// </param>
        /// <param name="tooManyElementsPenalty">
        /// The too Many Elements Penalty.
        /// </param>
        /// <param name="noPathBetweenStartBases">
        /// The penalty to apply to map phenotypes that do not have a path between starting bases.
        /// </param>
        /// <param name="notPlacedPenalty">
        /// The amount to add to the distance when a map point was not placed during conversion. 
        /// </param>
        /// <param name="notPlacedPenaltyModifier">
        /// The amount to modify the distance of angle/distance when a map point was not placed during conversion.
        /// </param>
        /// <param name="minimumStartBaseDistance">
        /// The minimum Start Base Distance.
        /// </param>
        /// <param name="maximumStartBaseDistance">
        /// The maximum Start Base Distance.
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
        /// <param name="minimumDistanceModifier">
        /// The minimum Distance Modifier.
        /// </param>
        /// <param name="maximumDegreeModifier">
        /// The maximum Degree Modifier.
        /// </param>
        /// <param name="minimumDegreeModifier">
        /// The minimum Degree Modifier.
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
            Enums.MapFunction mapCompletion = Enums.MapFunction.Turn,
            int maximumDisplacement = 10,
            int displacementAmountPerStep = 1,
            double tooFewElementsPenalty = 5, 
            double tooManyElementsPenalty = 5,
            double noPathBetweenStartBases = 100,
            double notPlacedPenalty = 10,
            double notPlacedPenaltyModifier = 1.5, 
            double minimumStartBaseDistance = 0.3, 
            double maximumStartBaseDistance = 1.0,
            double maximumDegree = 180,
            double minimumDegree = 0,
            double maximumDistance = 1.0,
            double minimumDistance = 0.15,
            double maximumDistanceModifier = 0.05,
            double minimumDistanceModifier = 0.01,
            double maximumDegreeModifier = 15,
            double minimumDegreeModifier = 1,
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
            this.MaximumStartBaseDistance = maximumStartBaseDistance;
            this.MinimumStartBaseDistance = minimumStartBaseDistance;
            this.TooManyElementsPenalty = tooManyElementsPenalty;
            this.TooFewElementsPenalty = tooFewElementsPenalty;
            this.DisplacementAmountPerStep = displacementAmountPerStep;
            this.MaximumDisplacement = maximumDisplacement;
            this.NoPathBetweenStartBases = noPathBetweenStartBases;
            this.MapCompletion = mapCompletion;
            this.NotPlacedPenalty = notPlacedPenalty;
            this.MaximumNumberOfXelNagaTowers = maximumNumberOfXelNagaTowers;
            this.MinimumNumberOfXelNagaTowers = minimumNumberOfXelNagaTowers;
            this.MaximumNumberOfDestructibleRocks = maximumNumberOfDestructibleRocks;
            this.MinimumNumberOfDestructibleRocks = minimumNumberOfDestructibleRocks;
            this.MaximumNumberOfRamps = maximumNumberOfRamps;
            this.MinimumNumberOfRamps = minimumNumberOfRamps;
            this.MaximumNumberOfBases = maximumNumberOfBases;
            this.MinimumNumberOfBases = minimumNumberOfBases;
            this.MinimumDegreeModifier = minimumDegreeModifier;
            this.MaximumDegreeModifier = maximumDegreeModifier;
            this.MinimumDistanceModifier = minimumDistanceModifier;
            this.MaximumDistanceModifier = maximumDistanceModifier;
            this.MinimumDistance = minimumDistance;
            this.MaximumDistance = maximumDistance;
            this.MinimumDegree = minimumDegree;
            this.MaximumDegree = maximumDegree;
            this.NotPlacedPenaltyModifier = notPlacedPenaltyModifier;
            this.Map = mp;
        }

        /// <summary>
        /// Gets the map that is being searched.
        /// </summary>
        public MapPhenotype Map { get; private set; }

        /// <summary>
        /// Gets the maximum amount that the search should try to displace a location placement.
        /// </summary>
        public int MaximumDisplacement { get; private set; }

        /// <summary>
        /// Gets the amount that a location should be displaced per displacement attempt.
        /// </summary>
        public int DisplacementAmountPerStep { get; private set; }

        /// <summary>
        /// Gets the penalty that should be applied when there are too few of a type of element.
        /// </summary>
        public double TooFewElementsPenalty { get; private set; }

        /// <summary>
        /// Gets the penalty that should be applied when there are too many of a type of element.
        /// </summary>
        public double TooManyElementsPenalty { get; private set; }

        /// <summary>
        /// Gets the penalty that should be applied to feasibility if there is no path between starting bases.
        /// </summary>
        public double NoPathBetweenStartBases { get; private set; }

        /// <summary>
        /// Gets the distance to add whenever a map point was not placed when calculating distance.
        /// </summary>
        public double NotPlacedPenalty { get; private set; }

        /// <summary>
        /// Gets the distance to add whenever a map point was not placed when calculating distance.
        /// </summary>
        public double NotPlacedPenaltyModifier { get; private set; }

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
        /// Gets the minimum distance modifier.
        /// </summary>
        public double MinimumDistanceModifier { get; private set; }

        /// <summary>
        /// Gets the maximum degree modifier.
        /// </summary>
        public double MaximumDegreeModifier { get; private set; }

        /// <summary>
        /// Gets the minimum degree modifier.
        /// </summary>
        public double MinimumDegreeModifier { get; private set; }
        
        /// <summary>
        /// Gets the minimum distance of a start base.
        /// </summary>
        public double MinimumStartBaseDistance { get; private set; }

        /// <summary>
        /// Gets the maximum distance of a start base.
        /// </summary>
        public double MaximumStartBaseDistance { get; private set; }

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

        /// <summary>
        /// Gets the map completion function.
        /// </summary>
        public Enums.MapFunction MapCompletion { get; private set; }
    }
}
