namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class that holds all the options for the map fitness calculations.
    /// </summary>
    public class MapFitnessOptions
    {
        #region Overall Fitness

        /// <summary>
        /// The maximum fitness possible.
        /// </summary>
        public readonly double MaxTotalFitness;

        #endregion

        #region Pathfinding

        /// <summary>
        /// Whether pathfinding should ignore destructible rocks or not.
        /// </summary>
        public readonly bool PathfindingIgnoreDestructibleRocks;

        #endregion

        #region Base Space

        /// <summary>
        /// The significance of the space around the start base.
        /// </summary>
        public readonly int BaseSpaceSignificance;

        /// <summary>
        /// Whether the base space calculation should ignore destructible rocks or not.
        /// </summary>
        public readonly bool BaseSpaceIgnoreDestructibleRocks;

        #endregion

        #region Base Height

        /// <summary>
        /// The significance of the height of the start base.
        /// </summary>
        public readonly int BaseHeightSignificance;

        #endregion

        #region Path Between Start Bases

        /// <summary>
        /// The significance of the distance between the start bases.
        /// </summary>
        public readonly int PathBetweenStartBasesSignificance;

        /// <summary>
        /// The maximum ground distance between the start bases.
        /// </summary>
        public readonly int PathStartMaxGroundDistance;

        /// <summary>
        /// The minimum ground distance between the start bases.
        /// </summary>
        public readonly int PathStartMinGroundDistance;

        /// <summary>
        /// The maximum direct distance between the start bases.
        /// </summary>
        public readonly int PathStartMaxDirectDistance;

        /// <summary>
        /// The minimum direct distance between the start bases.
        /// </summary>
        public readonly int PathStartMinDirectDistance;

        #endregion

        #region New Height Reached

        /// <summary>
        /// The significance of how many times a new height is reached.
        /// </summary>
        public readonly int NewHeightReachedSignificance;

        /// <summary>
        /// The maximum number of times a new height should be reached during the path between the start bases.
        /// </summary>
        public readonly int NewHeightReachedMax;

        /// <summary>
        /// The minimum number of times a new height should be reached during the path between the start bases.
        /// </summary>
        public readonly int NewHeightReachedMin;

        #endregion

        #region Distance To Natural Expansion

        /// <summary>
        /// The significance of how far there is to the natural expansion.
        /// </summary>
        public readonly int DistanceToNaturalSignificance;

        /// <summary>
        /// The maximum ground distance between a start bases and its natural expansion
        /// </summary>
        public readonly int PathNatMaxGroundDistance;

        /// <summary>
        /// The minimum ground distance between a start bases and its natural expansion
        /// </summary>
        public readonly int PathNatMinGroundDistance;

        /// <summary>
        /// The maximum direct distance between a start bases and its natural expansion
        /// </summary>
        public readonly int PathNatMaxDirectDistance;

        /// <summary>
        /// The minimum direct distance between a start bases and its natural expansion
        /// </summary>
        public readonly int PathNatMinDirectDistance;

        #endregion

        #region Distance To Non Natural Expansions

        /// <summary>
        /// The significance of how far there is to the nearest expansion.
        /// </summary>
        public readonly int DistanceToExpansionsSignificance;

        /// <summary>
        /// The maximum ground distance from a start base to any non-natural expansion.
        /// </summary>
        public readonly int PathExpMaxGroundDistance;

        /// <summary>
        /// The minimum ground distance from a start base to any non-natural expansion.
        /// </summary>
        public readonly int PathExpMinGroundDistance;

        /// <summary>
        /// The maximum ground distance based on the size of the map to a non-natural expansion based on how many expansions that have been considered already.
        /// </summary>
        public readonly List<Tuple<int, double>> PathExpGroundDistances;

        /// <summary>
        /// The maximum direct distance from a start base to any non-natural expansion.
        /// </summary>
        public readonly int PathExpMaxDirectDistance;

        /// <summary>
        /// The minimum direct distance from a start base to any non-natural expansion.
        /// </summary>
        public readonly int PathExpMinDirectDistance;

        /// <summary>
        /// The maximum direct distance based on the size of the map to a non-natural expansion based on how many expansions that have been considered already.
        /// </summary>
        public readonly List<Tuple<int, double>> PathExpDirectDistances;

        #endregion

        #region Expansions Available

        /// <summary>
        /// The significance of how many expansions that are available.
        /// </summary>
        public readonly int ExpansionsAvailableSignificance;

        /// <summary>
        /// The maximum number of expansions available.
        /// </summary>
        public readonly int ExpansionsAvailableMax;

        /// <summary>
        /// The minimum number of expansions available.
        /// </summary>
        public readonly int ExpansionsAvailableMin;

        #endregion

        #region Choke Points

        /// <summary>
        /// The significance of how many choke points that are available.
        /// </summary>
        public readonly int ChokePointsSignificance;

        /// <summary>
        /// The maximum number of choke points.
        /// </summary>
        public readonly int ChokePointsMax;

        /// <summary>
        /// The minimum number of choke points.
        /// </summary>
        public readonly int ChokePointsMin;

        /// <summary>
        /// The maximum width of an area that is considered a choke point.
        /// </summary>
        public readonly int ChokePointsWidth;

        /// <summary>
        /// Every x (the value) will be checked for a choke point.
        /// </summary>
        public readonly int ChokePointSearchStep;

        #endregion

        #region Xel'Naga Placement

        /// <summary>
        /// The significance of how much of the path between start bases the Xel'Naga tower's can see.
        /// </summary>
        public readonly int XelNagaPlacementSignificance;

        /// <summary>
        /// The range before a point is considered within range of the Xel'Naga tower.
        /// </summary>
        public readonly int DistanceToXelNaga;

        /// <summary>
        /// The maximum number of tiles on the path between start bases the Xel'Naga tower should give vision of.
        /// </summary>
        public readonly int StepsInXelNagaRangeMax;

        /// <summary>
        /// The minimum number of tiles on the path between start bases the Xel'Naga tower should give vision of.
        /// </summary>
        public readonly int StepsInXelNagaRangeMin;

        #endregion

        #region Base Openess
        /// <summary>
        /// The significance of how open the area surrounding start bases is.
        /// </summary>
        public readonly int StartBaseOpenessSignificance;

        /// <summary>
        /// The significance of how open the area surrounding bases is.
        /// </summary>
        public readonly int BaseOpenessSignificance;

        /// <summary>
        /// The minimum number of tiles that should be open around a start base.
        /// </summary>
        public readonly int OpenStartBaseTilesMinimum;

        /// <summary>
        /// The maximum number of tiles that should be open around a start base.
        /// </summary>
        public readonly int OpenStartBaseTilesMaximum;

        /// <summary>
        /// The minimum number of tiles that should be open around a base.
        /// </summary>
        public readonly int OpenBaseTilesMinimum;

        /// <summary>
        /// The maximum number of tiles that should be open around a base.
        /// </summary>
        public readonly int OpenBaseTilesMaximum;

        /// <summary>
        /// The maximum number of directions that should be open at a start base.
        /// </summary>
        public readonly int StartBaseApproachDirectionMaximum;

        /// <summary>
        /// The minimum number of directions that should be open at a start base.
        /// </summary>
        public readonly int StartBaseApproachDirectionMinimum;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MapFitnessOptions"/> class.
        /// </summary>
        /// <param name="pathfindingIgnoreDestructibleRocks"> Whether pathfinding should ignore destructible rocks or not. </param>
        /// <param name="baseSpaceSignificance"> The significance of the space around the start base. </param>
        /// <param name="baseSpaceIgnoreDestructibleRocks"> Whether the base space calculation should ignore destructible rocks or not. </param>
        /// <param name="baseHeightSignificance"> The significance of the height of the start base. </param>
        /// <param name="pathBetweenStartBasesSignificance"> The significance of the distance between the start bases. </param>
        /// <param name="pathStartMaxGroundDistance"> The maximum ground distance between the start bases. </param>
        /// <param name="pathStartMinGroundDistance"> The minimum ground distance between the start bases. </param>
        /// <param name="pathStartMaxDirectDistance"> The maximum direct distance between the start bases. </param>
        /// <param name="pathStartMinDirectDistance"> The minimum direct distance between the start bases. </param>
        /// <param name="newHeightReachedSignificance"> The significance of how many times a new height is reached. </param>
        /// <param name="newHeightReachedMax"> The maximum number of times a new height should be reached during the path between the start bases. </param>
        /// <param name="newHeightReachedMin"> The minimum number of times a new height should be reached during the path between the start bases. </param>
        /// <param name="distanceToNaturalSignificance"> The significance of how far there is to the natural expansion. </param>
        /// <param name="pathNatMaxGroundDistance"> The maximum ground distance between a start bases and its natural expansion </param>
        /// <param name="pathNatMinGroundDistance"> The minimum ground distance between a start bases and its natural expansion </param>
        /// <param name="pathNatMaxDirectDistance"> The maximum direct distance between a start bases and its natural expansion </param>
        /// <param name="pathNatMinDirectDistance"> The minimum direct distance between a start bases and its natural expansion </param>
        /// <param name="distanceToExpansionsSignificance"> The significance of how far there is to the nearest expansion. </param>
        /// <param name="pathExpMaxGroundDistance"> The maximum ground distance from a start base to any non-natural expansion. </param>
        /// <param name="pathExpMinGroundDistance"> The minimum ground distance from a start base to any non-natural expansion. </param>
        /// <param name="pathExpMaxDirectDistance"> The maximum direct distance from a start base to any non-natural expansion. </param>
        /// <param name="pathExpMinDirectDistance"> The minimum direct distance from a start base to any non-natural expansion. </param>
        /// <param name="pathExpMaxDistances"> The maximum ground distance based on the size of the map to a non-natural expansion based on how many expansions that have been considered already. </param>
        /// <param name="pathExpDirectDistances"> The maximum direct distance based on the size of the map to a non-natural expansion based on how many expansions that have been considered already.</param>
        /// <param name="expansionsAvailableSignificance"> The significance of how many expansions that are available. </param>
        /// <param name="expansionsAvailableMax"> The maximum number of expansions available. </param>
        /// <param name="expansionsAvailableMin"> The minimum number of expansions available. </param>
        /// <param name="chokePointsSignificance"> The significance of how many choke points that are available. </param>
        /// <param name="chokePointsMax"> The maximum number of choke points. </param>
        /// <param name="chokePointsMin"> The minimum number of choke points. </param>
        /// <param name="chokePointsWidth"> The maximum width of an area that is considered a choke point. </param>
        /// <param name="chokePointSearchStep"> Every x (the value) will be checked for a choke point. </param>
        /// <param name="xelNagaPlacementSignificance"> The significance of how much of the path between start bases the Xel'Naga tower's can see. </param>
        /// <param name="distanceToXelNaga"> The range before a point is considered within range of the Xel'Naga tower. </param>
        /// <param name="stepsInXelNagaRangeMax"> The maximum number of tiles on the path between start bases the Xel'Naga tower should give vision of. </param>
        /// <param name="stepsInXelNagaRangeMin"> The minimum number of tiles on the path between start bases the Xel'Naga tower should give vision of. </param>
        /// <param name="startBaseOpenessSignificance">The significance of how open the start base is.</param>
        /// <param name="baseOpenessSignificance">The significance of how open bases are.</param>
        /// <param name="openStartBaseTilesMinimum">The minimum number of tiles that must be open around a start base.</param>
        /// <param name="openStartBaseTilesMaximum">The maximum number of tiles that must be open around a start base.</param>
        /// <param name="openBaseTilesMinimum">The minimum number of tiles that must be open around a base.</param>
        /// <param name="openBaseTilesMaximum">The maximum number of tiles that must be open around a base.</param>
        /// <param name="startBaseApproachDirectionMinimum">The minimum number of directions that are open to approach for the start base.</param>
        /// <param name="startBaseApproachDirectionMaximum">The maximum number of directions that are open to approach for the start base</param>
        public MapFitnessOptions(
            bool pathfindingIgnoreDestructibleRocks = true,
            int baseSpaceSignificance = 5, 
            bool baseSpaceIgnoreDestructibleRocks = true,
            int baseHeightSignificance = 5,
            int pathBetweenStartBasesSignificance = 5, 
            int pathStartMaxGroundDistance = 90, 
            int pathStartMinGroundDistance = 20, 
            int pathStartMaxDirectDistance = 80, 
            int pathStartMinDirectDistance = 20,
            int newHeightReachedSignificance = 5, 
            int newHeightReachedMax = 6, 
            int newHeightReachedMin = 0,
            int distanceToNaturalSignificance = 5,
            int pathNatMaxGroundDistance = 40,
            int pathNatMinGroundDistance = 0,
            int pathNatMaxDirectDistance = 30, 
            int pathNatMinDirectDistance = 0, 
            int distanceToExpansionsSignificance = 5,
            int pathExpMaxGroundDistance = 90,
            int pathExpMinGroundDistance = 20,
            List<Tuple<int, double>> pathExpMaxDistances = null,
            int pathExpMaxDirectDistance = 80,
            int pathExpMinDirectDistance = 20,
            List<Tuple<int, double>> pathExpDirectDistances = null,
            int expansionsAvailableSignificance = 5, 
            int expansionsAvailableMax = 4, 
            int expansionsAvailableMin = 1,
            int chokePointsSignificance = 5, 
            int chokePointsMax = 18, 
            int chokePointsMin = 0, 
            int chokePointsWidth = 2,
            int chokePointSearchStep = 1, 
            int xelNagaPlacementSignificance = 5, 
            int distanceToXelNaga = 22, 
            int stepsInXelNagaRangeMax = 30, 
            int stepsInXelNagaRangeMin = 0, 
            int startBaseOpenessSignificance = 5, 
            int baseOpenessSignificance = 3, 
            int openStartBaseTilesMinimum = 10, 
            int openStartBaseTilesMaximum = 85, 
            int openBaseTilesMinimum = 16, 
            int openBaseTilesMaximum = 56, 
            int startBaseApproachDirectionMinimum = 1, 
            int startBaseApproachDirectionMaximum = 3)
        {
            // TODO: Figure out good default values for this thing.
            this.PathfindingIgnoreDestructibleRocks = pathfindingIgnoreDestructibleRocks;
            this.BaseSpaceSignificance = baseSpaceSignificance;
            this.BaseSpaceIgnoreDestructibleRocks = baseSpaceIgnoreDestructibleRocks;
            this.BaseHeightSignificance = baseHeightSignificance;
            this.PathBetweenStartBasesSignificance = pathBetweenStartBasesSignificance;
            this.PathStartMaxGroundDistance = pathStartMaxGroundDistance;
            this.PathStartMinGroundDistance = pathStartMinGroundDistance;
            this.PathStartMaxDirectDistance = pathStartMaxDirectDistance;
            this.PathStartMinDirectDistance = pathStartMinDirectDistance;
            this.NewHeightReachedSignificance = newHeightReachedSignificance;
            this.NewHeightReachedMax = newHeightReachedMax;
            this.NewHeightReachedMin = newHeightReachedMin;
            this.DistanceToNaturalSignificance = distanceToNaturalSignificance;
            this.PathNatMaxGroundDistance = pathNatMaxGroundDistance;
            this.PathNatMaxDirectDistance = pathNatMaxDirectDistance;
            this.PathNatMinDirectDistance = pathNatMinDirectDistance;
            this.PathNatMinGroundDistance = pathNatMinGroundDistance;
            this.DistanceToExpansionsSignificance = distanceToExpansionsSignificance;
            this.PathExpMaxGroundDistance = pathExpMaxGroundDistance;
            this.PathExpMinGroundDistance = pathExpMinGroundDistance;
            this.PathExpMaxDirectDistance = pathExpMaxDirectDistance;
            this.PathExpMinDirectDistance = pathExpMinDirectDistance;
            this.ExpansionsAvailableSignificance = expansionsAvailableSignificance;
            this.ExpansionsAvailableMax = expansionsAvailableMax;
            this.ExpansionsAvailableMin = expansionsAvailableMin;
            this.ChokePointsSignificance = chokePointsSignificance;
            this.ChokePointsMax = chokePointsMax;
            this.ChokePointsMin = chokePointsMin;
            this.ChokePointsWidth = chokePointsWidth;
            this.ChokePointSearchStep = chokePointSearchStep;
            this.XelNagaPlacementSignificance = xelNagaPlacementSignificance;
            this.DistanceToXelNaga = distanceToXelNaga;
            this.StepsInXelNagaRangeMax = stepsInXelNagaRangeMax;
            this.StepsInXelNagaRangeMin = stepsInXelNagaRangeMin;
            this.StartBaseOpenessSignificance = startBaseOpenessSignificance;
            this.BaseOpenessSignificance = baseOpenessSignificance;
            this.OpenStartBaseTilesMinimum = openStartBaseTilesMinimum;
            this.OpenStartBaseTilesMaximum = openStartBaseTilesMaximum;
            this.OpenBaseTilesMinimum = openBaseTilesMinimum;
            this.OpenBaseTilesMaximum = openBaseTilesMaximum;
            this.StartBaseApproachDirectionMinimum = startBaseApproachDirectionMinimum;
            this.StartBaseApproachDirectionMaximum = startBaseApproachDirectionMaximum;
            this.PathExpDirectDistances = pathExpDirectDistances;

            this.MaxTotalFitness = 
                this.BaseSpaceSignificance
                + this.BaseHeightSignificance
                + this.PathBetweenStartBasesSignificance
                + this.NewHeightReachedSignificance
                + this.DistanceToNaturalSignificance
                + this.DistanceToExpansionsSignificance
                + this.ExpansionsAvailableSignificance
                + this.ChokePointsSignificance
                + this.XelNagaPlacementSignificance
                + this.StartBaseOpenessSignificance
                + this.BaseOpenessSignificance;

            if (pathExpMaxDistances == null)
            { 
                this.PathExpGroundDistances = new List<Tuple<int, double>> 
                        {
                            new Tuple<int, double>(1, 0.3),
                            new Tuple<int, double>(2, 0.5),
                            new Tuple<int, double>(3, 0.7),
                            new Tuple<int, double>(4, 0.9)
                        };
            }
            else
            {
                this.PathExpGroundDistances = pathExpMaxDistances;
            }

            if (pathExpDirectDistances == null)
            {
                this.PathExpDirectDistances = new List<Tuple<int, double>> 
                        {
                            new Tuple<int, double>(1, 0.25),
                            new Tuple<int, double>(2, 0.45),
                            new Tuple<int, double>(3, 0.65),
                            new Tuple<int, double>(4, 0.85)
                        };
            }
            else
            {
                this.PathExpDirectDistances = pathExpDirectDistances;
            }
        }
    }
}
