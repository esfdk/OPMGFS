namespace OPMGFS.Map
{
    /// <summary>
    /// Novelty Search Options for map searching.
    /// </summary>
    public class MapSearchOptions
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSearchOptions"/> class. 
        /// </summary>
        /// <param name="mp"> The map phenotype to search on. </param>
        /// <param name="mapCompletion"> The completion method to use when converting to phenotype. </param>
        /// <param name="chanceToAddBase"> The chance To Add Base. </param>
        /// <param name="chanceToAddGoldBase"> The chance To Add Gold Base. </param>
        /// <param name="chanceToAddXelNagaTower"> The chance To Add Xel'Naga Tower. </param>
        /// <param name="chanceToAddDestructibleRocks"> The chance To Add Destructible Rocks. </param>
        /// <param name="chanceToAddNewElement"> The chance that an element will be added to a solution during mutation. </param>
        /// <param name="chanceToRemoveElement"> The chance that an element will be removed from a solution during mutation. </param>
        /// <param name="maximumDisplacement"> The maximum Displacement. </param>
        /// <param name="displacementAmountPerStep"> The displacement Amount Per Step. </param>
        /// <param name="tooFewElementsPenalty"> The element Not Placed Penalty. </param>
        /// <param name="tooManyElementsPenalty"> The too Many Elements Penalty. </param>
        /// <param name="noPathBetweenStartBasesPenalty"> The penalty to apply to map phenotypes that do not have a path between starting bases. </param>
        /// <param name="notPlacedPenalty"> The amount to add to the distance when a map point was not placed during conversion.  </param>
        /// <param name="notPlacedPenaltyModifier"> The amount to modify the distance of angle/distance when a map point was not placed during conversion. </param>
        /// <param name="minimumStartBaseDistance"> The minimum Start Base Distance. </param>
        /// <param name="maximumStartBaseDistance"> The maximum Start Base Distance. </param>
        /// <param name="maximumDegree"> The maximum Degree. </param>
        /// <param name="minimumDegree"> The minimum Degree. </param>
        /// <param name="maximumDistance"> The maximum Distance. </param>
        /// <param name="minimumDistance"> The minimum Distance. </param>
        /// <param name="maximumDistanceModifier"> The maximum Distance Modifier. </param>
        /// <param name="minimumDistanceModifier"> The minimum Distance Modifier. </param>
        /// <param name="maximumDegreeModifier"> The maximum Degree Modifier. </param>
        /// <param name="minimumDegreeModifier"> The minimum Degree Modifier. </param>
        /// <param name="minimumNumberOfBases"> The minimum Number Of Bases. </param>
        /// <param name="maximumNumberOfBases"> The maximum Number Of Bases. </param>
        /// <param name="minimumNumberOfRamps"> The minimum Number Of Ramps. </param>
        /// <param name="maximumNumberOfRamps"> The maximum Number Of Ramps. </param>
        /// <param name="minimumNumberOfDestructibleRocks"> The minimum Number Of Destructible Rocks. </param>
        /// <param name="maximumNumberOfDestructibleRocks"> The maximum Number Of Destructible Rocks. </param>
        /// <param name="minimumNumberOfXelNagaTowers"> The minimum Number Of Xel'Naga Towers. </param>
        /// <param name="maximumNumberOfXelNagaTowers"> The maximum Number Of Xel'Naga Towers. </param>
        public MapSearchOptions(
            MapPhenotype mp,
            Enums.MapFunction mapCompletion = Enums.MapFunction.Turn,
            double chanceToAddBase = 0.15,
            double chanceToAddGoldBase = 0.05,
            double chanceToAddXelNagaTower = 0.05,
            double chanceToAddDestructibleRocks = 0.45, 
            double chanceToAddNewElement = 0.3, 
            double chanceToRemoveElement = 0.3,
            int maximumDisplacement = 10,
            int displacementAmountPerStep = 1,
            double tooFewElementsPenalty = 5, 
            double tooManyElementsPenalty = 5,
            double noPathBetweenStartBasesPenalty = 100,
            double notPlacedPenalty = 10,
            double notPlacedPenaltyModifier = 1.5, 
            double minimumStartBaseDistance = 0.4, 
            double maximumStartBaseDistance = 0.8,
            double maximumDegree = 180,
            double minimumDegree = 0,
            double maximumDistance = 1.0,
            double minimumDistance = 0.15,
            double maximumDistanceModifier = 0.1,
            double minimumDistanceModifier = 0.01,
            double maximumDegreeModifier = 20,
            double minimumDegreeModifier = 1,
            int minimumNumberOfBases = 1,
            int maximumNumberOfBases = 4,
            int minimumNumberOfRamps = 6,
            int maximumNumberOfRamps = 18,
            int minimumNumberOfDestructibleRocks = 4,
            int maximumNumberOfDestructibleRocks = 8,
            int minimumNumberOfXelNagaTowers = 1,
            int maximumNumberOfXelNagaTowers = 1)
        {
            this.ChanceToAddBase = chanceToAddBase;
            this.ChanceToAddXelNagaTower = chanceToAddXelNagaTower;
            this.ChanceToAddDestructibleRocks = chanceToAddDestructibleRocks;
            this.ChanceToAddGoldBase = chanceToAddGoldBase;
            this.ChanceToRemoveElement = chanceToRemoveElement;
            this.ChanceToAddNewElement = chanceToAddNewElement;
            this.MaximumStartBaseDistance = maximumStartBaseDistance;
            this.MinimumStartBaseDistance = minimumStartBaseDistance;
            this.TooManyElementsPenalty = tooManyElementsPenalty;
            this.TooFewElementsPenalty = tooFewElementsPenalty;
            this.DisplacementAmountPerStep = displacementAmountPerStep;
            this.MaximumDisplacement = maximumDisplacement;
            this.NoPathBetweenStartBasesPenalty = noPathBetweenStartBasesPenalty;
            this.MapCompletion = mapCompletion;
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
            this.NotPlacedPenalty = notPlacedPenalty;
            this.NotPlacedPenaltyModifier = notPlacedPenaltyModifier;
            this.Map = mp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapSearchOptions"/> class.
        /// </summary>
        /// <param name="mp">
        /// The map phenotype.
        /// </param>
        /// <param name="mso">
        /// The map search options to copy..
        /// </param>
        public MapSearchOptions(MapPhenotype mp, MapSearchOptions mso)
        {
            this.ChanceToAddBase = mso.ChanceToAddBase;
            this.ChanceToAddGoldBase = mso.ChanceToAddGoldBase;
            this.ChanceToAddDestructibleRocks = mso.ChanceToAddDestructibleRocks;
            this.ChanceToAddXelNagaTower = mso.ChanceToAddXelNagaTower;
            this.ChanceToRemoveElement = mso.ChanceToRemoveElement;
            this.ChanceToAddNewElement = mso.ChanceToAddNewElement;
            this.MaximumStartBaseDistance = mso.MaximumStartBaseDistance;
            this.MinimumStartBaseDistance = mso.MinimumStartBaseDistance;
            this.TooManyElementsPenalty = mso.TooManyElementsPenalty;
            this.TooFewElementsPenalty = mso.TooFewElementsPenalty;
            this.DisplacementAmountPerStep = mso.DisplacementAmountPerStep;
            this.MaximumDisplacement = mso.MaximumDisplacement;
            this.NoPathBetweenStartBasesPenalty = mso.NoPathBetweenStartBasesPenalty;
            this.MapCompletion = mso.MapCompletion;
            this.MaximumNumberOfXelNagaTowers = mso.MaximumNumberOfXelNagaTowers;
            this.MinimumNumberOfXelNagaTowers = mso.MinimumNumberOfXelNagaTowers;
            this.MaximumNumberOfDestructibleRocks = mso.MaximumNumberOfDestructibleRocks;
            this.MinimumNumberOfDestructibleRocks = mso.MinimumNumberOfDestructibleRocks;
            this.MaximumNumberOfRamps = mso.MaximumNumberOfRamps;
            this.MinimumNumberOfRamps = mso.MinimumNumberOfRamps;
            this.MaximumNumberOfBases = mso.MaximumNumberOfBases;
            this.MinimumNumberOfBases = mso.MinimumNumberOfBases;
            this.MinimumDegreeModifier = mso.MinimumDegreeModifier;
            this.MaximumDegreeModifier = mso.MaximumDegreeModifier;
            this.MinimumDistanceModifier = mso.MinimumDistanceModifier;
            this.MaximumDistanceModifier = mso.MaximumDistanceModifier;
            this.MinimumDistance = mso.MinimumDistance;
            this.MaximumDistance = mso.MaximumDistance;
            this.MinimumDegree = mso.MinimumDegree;
            this.MaximumDegree = mso.MaximumDegree;
            this.NotPlacedPenalty = mso.NotPlacedPenalty;
            this.NotPlacedPenaltyModifier = mso.NotPlacedPenaltyModifier;

            this.Map = mp;
        }
        #endregion

        /// <summary>
        /// Gets the map that is being searched.
        /// </summary>
        public MapPhenotype Map { get; private set; }

        /// <summary>
        /// Gets the map completion function.
        /// </summary>
        public Enums.MapFunction MapCompletion { get; private set; }

        #region Chance To Add Elements
        /// <summary>
        /// Gets the chance that a new map point will be a base.
        /// </summary>
        public double ChanceToAddBase { get; private set; }

        /// <summary>
        /// Gets the chance that a new map point will be a Xel'Naga tower.
        /// </summary>
        public double ChanceToAddXelNagaTower { get; private set; }

        /// <summary>
        /// Gets the chance that a new map point will be destructible rocks.
        /// </summary>
        public double ChanceToAddDestructibleRocks { get; private set; }

        /// <summary>
        /// Gets the chance that a new map point will be a gold base.
        /// </summary>
        public double ChanceToAddGoldBase { get; private set; }

        /// <summary>
        /// Gets the chance to add an element to a solution.
        /// </summary>
        public double ChanceToAddNewElement { get; private set; }

        /// <summary>
        /// Gets the chance to remove an element from a solution.
        /// </summary>
        public double ChanceToRemoveElement { get; private set; }
        #endregion

        #region Displacement Options
        /// <summary>
        /// Gets the maximum amount that the search should try to displace a location placement.
        /// </summary>
        public int MaximumDisplacement { get; private set; }

        /// <summary>
        /// Gets the amount that a location should be displaced per displacement attempt.
        /// </summary>
        public int DisplacementAmountPerStep { get; private set; }
        #endregion

        #region Feasibility Penalties
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
        public double NoPathBetweenStartBasesPenalty { get; private set; }

        /// <summary>
        /// Gets the distance to add whenever a map point was not placed when calculating distance.
        /// </summary>
        public double NotPlacedPenalty { get; private set; }

        /// <summary>
        /// Gets the distance to add whenever a map point was not placed when calculating distance.
        /// </summary>
        public double NotPlacedPenaltyModifier { get; private set; }
        #endregion

        #region Map Point Degree/Distance Options
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
        #endregion

        #region Number of Map Elements
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
        /// Gets the minimum number of Xel'Naga towers.
        /// </summary>
        public int MinimumNumberOfXelNagaTowers { get; private set; }

        /// <summary>
        /// Gets the maximum number of Xel'Naga towers.
        /// </summary>
        public int MaximumNumberOfXelNagaTowers { get; private set; }
        #endregion
    }
}
