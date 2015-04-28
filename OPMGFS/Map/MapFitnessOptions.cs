namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;

    public class MapFitnessOptions
    {
        /// <summary>
        /// The significance of the space around the start base.
        /// </summary>
        public readonly int BaseSpaceSignificance;

        /// <summary>
        /// The significance of the height of the start base.
        /// </summary>
        public readonly int BaseHeightSignificance;

        /// <summary>
        /// The significance of the distance between the bases.
        /// </summary>
        public readonly int PathBetweenStartBasesSignificance;

        public readonly int PathStartMaxGroundDistance;

        public readonly int PathStartMinGroundDistance;

        public readonly int PathStartMaxDirectDistance;

        public readonly int PathStartMinDirectDistance;

        /// <summary>
        /// The significance of how many times a new height is reached.
        /// </summary>
        public readonly int NewHeightReachedSignificance;

        public readonly int NewHeightReachedMax;

        public readonly int NewHeightReachedMin;

        /// <summary>
        /// The significance of how far there is to the nearest expansion.
        /// </summary>
        public readonly int DistanceToNaturalSignificance;

        public readonly int PathNatMaxGroundDistance;

        public readonly int PathNatMinGroundDistance;

        public readonly int PathNatMaxDirectDistance;

        public readonly int PathNatMinDirectDistance;

        /// <summary>
        /// The significance of how far there is to the nearest expansion.
        /// </summary>
        public readonly int DistanceToExpansionsSignificance;

        public readonly int PathExpMaxGroundDistance;

        public readonly int PathExpMinGroundDistance;

        public readonly List<Tuple<int, double>> PathExpGroundDistances;

        public readonly int PathExpMaxDirectDistance;

        public readonly int PathExpMinDirectDistance;

        public readonly List<Tuple<int, double>> PathExpDirectDistances;

        /// <summary>
        /// The significance of how many expansions that are available.
        /// </summary>
        public readonly int ExpansionsAvailableSignificance;

        public readonly int ExpansionsAvailableMax;

        public readonly int ExpansionsAvailableMin;

        /// <summary>
        /// The significance of how many choke points that are available.
        /// </summary>
        public readonly int ChokePointsSignificance;

        public readonly int ChokePointsMax;

        public readonly int ChokePointsMin;

        public readonly int ChokePointsWidth;

        /// <summary>
        /// Every x (the value) will be checked for a choke point.
        /// </summary>
        public readonly int ChokePointSearchStep;

        public MapFitnessOptions(
            int baseSpaceSignificance = 5, 
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
            int pathNatMaxGroundDistance = 50, 
            int pathNatMaxDirectDistance = 20, 
            int pathNatMinDirectDistance = 40, 
            int pathNatMinGroundDistance = 20,
            int distanceToExpansionsSignificance = 5,
            List<Tuple<int, double>> pathExpMaxDistances = null,
            int pathExpMaxGroundDistance = 90,
            int pathExpMinGroundDistance = 20,
            List<Tuple<int, double>> pathExpDirectDistances = null,
            int pathExpMaxDirectDistance = 80,
            int pathExpMinDirectDistance = 20, 
            int expansionsAvailableSignificance = 5, 
            int expansionsAvailableMax = 8, 
            int expansionsAvailableMin = 2,
            int chokePointsSignificance = 5, 
            int chokePointsMax = 5, 
            int chokePointsMin = 0, 
            int chokePointsWidth = 2,
            int chokePointSearchStep = 3)
        {
            this.BaseSpaceSignificance = baseSpaceSignificance;
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
            this.ExpansionsAvailableSignificance = expansionsAvailableSignificance;
            this.PathExpMaxDirectDistance = pathExpMaxDirectDistance;
            this.PathExpMinDirectDistance = pathExpMinDirectDistance;
            this.ExpansionsAvailableMax = expansionsAvailableMax;
            this.ExpansionsAvailableMin = expansionsAvailableMin;
            this.ChokePointsSignificance = chokePointsSignificance;
            this.ChokePointsMax = chokePointsMax;
            this.ChokePointsMin = chokePointsMin;
            this.ChokePointsWidth = chokePointsWidth;
            this.ChokePointSearchStep = chokePointSearchStep;
            this.PathExpDirectDistances = pathExpDirectDistances;

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
        }
    }
}
