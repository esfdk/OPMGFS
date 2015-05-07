namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// A class that can calculate fitness value for a map.
    /// </summary>
    public class MapFitness
    {
        /// <summary>
        /// The options for the map fitness.
        /// </summary>
        private readonly MapFitnessOptions mfo;

        /// <summary>
        /// The pathfinding used.
        /// </summary>
        private readonly JPSMapPathfinding mapPathfinding;

        /// <summary>
        /// The Y-size of the map.
        /// </summary>
        private readonly int ySize;

        /// <summary>
        /// The X-size of the map.
        /// </summary>
        private readonly int xSize;

        /// <summary>
        /// The map the fitness is being calculated for.
        /// </summary>
        private readonly MapPhenotype map;

        /// <summary>
        /// The position of the top-most start base.
        /// </summary>
        private Position startBasePosition1;

        /// <summary>
        /// The position of the bottom-most start base.
        /// </summary>
        private Position startBasePosition2;

        /// <summary>
        /// The position of the top-most Xel'Naga tower.
        /// </summary>
        private Position xelNagaPosition1;

        /// <summary>
        /// The position of the bottom-most Xel'Naga tower.
        /// </summary>
        private Position xelNagaPosition2;

        /// <summary>
        /// The path between bases.
        /// </summary>
        private List<Position> pathBetweenStartBases;

        /// <summary>
        /// The positions of the normal bases found.
        /// </summary>
        private List<Position> bases;

        /// <summary>
        /// The highest level found.
        /// </summary>
        private Enums.HeightLevel heighestLevel = Enums.HeightLevel.Height0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapFitness"/> class. 
        /// </summary>
        /// <param name="map"> The map to calculate fitness for. </param>
        /// <param name="mfo"> The map fitness options. </param>
        public MapFitness(
            MapPhenotype map,
            MapFitnessOptions mfo)
        {
            this.ySize = map.YSize;
            this.xSize = map.XSize;
            this.map = map;
            this.mapPathfinding = new JPSMapPathfinding(map.HeightLevels, map.MapItems, map.DestructibleRocks);
            this.mfo = mfo;
        }

        /// <summary>
        /// Calculates the fitness for the map.
        /// </summary>
        /// <returns> A double representing the fitness of the map. The higher, the better. </returns>
        public double CalculateFitness()
        {
            var fitness = 0.0d;
            this.bases = new List<Position>();

            // Find a startbase. No need to find all start bases, as the other base should be a complete mirror of this base.
            for (var tempY = this.ySize - 1; tempY >= 0; tempY--)
            {
                for (var tempX = 0; tempX < this.xSize; tempX++)
                {
                    // Find start base 1
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.StartBase
                        && this.startBasePosition2 == null)
                    {
                        this.startBasePosition2 = new Tuple<int, int>(tempX + 2, tempY - 2);
                    }

                    // Find start base 2
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.StartBase
                        && this.startBasePosition1 == null
                        && this.startBasePosition2 != null)
                    {
                        if (Math.Abs(tempX - this.startBasePosition2.Item1) > 3
                            || Math.Abs(tempY - this.startBasePosition2.Item2) > 3)
                        {
                            this.startBasePosition1 = new Position(tempX + 2, tempY - 2);
                        }
                    }

                    // Check for highest level
                    if ((this.map.HeightLevels[tempX, tempY] == Enums.HeightLevel.Height1
                         || this.map.HeightLevels[tempX, tempY] == Enums.HeightLevel.Height2)
                        && this.map.HeightLevels[tempX, tempY] > this.heighestLevel)
                    {
                        this.heighestLevel = this.map.HeightLevels[tempX, tempY];
                    }

                    var tempPos = new Position(tempX, tempY);

                    // Check if the area is a base
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.Base
                        && !MapHelper.CloseToAny(tempPos, this.bases, 5))
                    {
                        this.bases.Add(new Position(tempX + 2, tempY - 2));
                    }

                    // Save the first XelNaga tower found
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.XelNagaTower
                        && this.xelNagaPosition2 == null)
                    {
                        this.xelNagaPosition2 = tempPos;
                    }

                    // Save the second XelNaga tower found
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.XelNagaTower
                        && this.xelNagaPosition2 != null
                        && this.xelNagaPosition1 == null
                        && !MapHelper.CloseTo(tempPos, this.xelNagaPosition2, 4))
                    {
                        this.xelNagaPosition1 = tempPos;
                    }
                }
            }

            // If no start bases are found, the map is not feasible and running fitness calculations is not worth it.
            if (this.startBasePosition1 == null || this.startBasePosition2 == null) return -200000;

            this.pathBetweenStartBases = this.mapPathfinding.FindPathFromTo(
                this.startBasePosition1,
                this.startBasePosition2,
                this.mfo.PathfindingIgnoreDestructibleRocks);

            fitness += this.BaseSpace();
            Console.WriteLine("Base Space:                         {0}", fitness);
            var prevFitness = fitness;

            fitness += this.BaseHeightLevel();
            Console.WriteLine("Base Height Level:                  {0}", fitness - prevFitness);
            prevFitness = fitness;

            fitness += this.PathBetweenStartBases();
            Console.WriteLine("Path Between Start Bases:           {0}", fitness - prevFitness);
            prevFitness = fitness;

            fitness += this.NewHeightReached();
            Console.WriteLine("New Height Reached:                 {0}", fitness - prevFitness);
            prevFitness = fitness;

            fitness += this.DistanceToNaturalExpansion();
            Console.WriteLine("Distance to Natural Expansions:     {0}", fitness - prevFitness);
            prevFitness = fitness;

            fitness += this.DistanceToNonNaturalExpansions();
            Console.WriteLine("Distance To Non Natural Expansions: {0}", fitness - prevFitness);
            prevFitness = fitness;

            fitness += this.ExpansionsAvailable();
            Console.WriteLine("Expansions Available:               {0}", fitness - prevFitness);
            prevFitness = fitness;

            var sw = new Stopwatch();
            sw.Start();
            fitness += this.ChokePoints();
            Console.WriteLine("Choke Points:                       {0}, with time {1} millis", fitness - prevFitness, sw.ElapsedMilliseconds);
            prevFitness = fitness;
            sw.Stop();

            fitness += this.XelNagaPlacement();
            Console.WriteLine("Xel'Naga Placement:                 {0}", fitness - prevFitness);

            // ITODO: Grooss - Consider base openess
            
            //// Used to check Xel'Naga vision
            ////for (var tempY = this.ySize - 1; tempY >= 0; tempY--)
            ////{
            ////    for (var tempX = 0; tempX < this.xSize; tempX++)
            ////    {
            ////        if (MapHelper.CloseTo(new Position(tempX, tempY), this.xelNagaPosition1, this.mfo.DistanceToXelNaga)
            ////            || MapHelper.CloseTo(new Position(tempX, tempY), this.xelNagaPosition2, this.mfo.DistanceToXelNaga))
            ////            this.map.MapItems[tempX, tempY] = Enums.Item.GoldMinerals;
            ////    }
            ////}

            return fitness;
        }

        public int FreeTilesAroundBase(int index)
        {
            // HACK: Will bug if base placement is changed
            var counter = 0;

            var baseLocation = bases[index];

            var modifier = 0;

            if (baseLocation.Item2 < this.map.YSize / 2)
            {
                modifier = 1;
            }

            var bottomLineY = baseLocation.Item2 - 7 - (modifier * 3);
            var upperLineY = baseLocation.Item2 + 10 - (modifier * 3);
            var leftSideX = baseLocation.Item1 - 9 + modifier;
            var rightSideX = baseLocation.Item1 + 8 + modifier;

            for (var y = bottomLineY; y <= upperLineY; y++)
            {
                var xIncrease = (y == bottomLineY || y == upperLineY) ? 1 : 17;

                for (var x = leftSideX; x <= rightSideX; x += xIncrease)
                {
                    if (this.map.HeightLevels[x, y] != Enums.HeightLevel.Cliff
                        && this.map.HeightLevels[x, y] != Enums.HeightLevel.Impassable
                        && this.map.MapItems[x, y] != Enums.Item.Base
                        && this.map.MapItems[x, y] != Enums.Item.StartBase
                        && this.map.MapItems[x, y] != Enums.Item.GoldMinerals
                        && this.map.MapItems[x, y] != Enums.Item.BlueMinerals
                        && this.map.MapItems[x, y] != Enums.Item.Gas
                        && this.map.MapItems[x, y] != Enums.Item.XelNagaTower)
                    {
                        counter++;
                        this.map.HeightLevels[x,y] = Enums.HeightLevel.Impassable;
                    }
                }
            }

            return counter;
        }

        public int FreeTilesAroundStartBase()
        {
            var counter = 0;

            var baseLocation = startBasePosition2;
            var bottomLineY = baseLocation.Item2 - 12;
            var upperLineY = baseLocation.Item2 + 13;
            var leftSideX = baseLocation.Item1 - 12;
            var rightSideX = baseLocation.Item1 + 13;

            for (var y = bottomLineY; y <= upperLineY; y++)
            {
                var xIncrease = (y == bottomLineY || y == upperLineY) ? 1 : 25;

                for (var x = leftSideX; x <= rightSideX; x += xIncrease)
                {
                    if (this.map.HeightLevels[x, y] != Enums.HeightLevel.Cliff
                        && this.map.HeightLevels[x, y] != Enums.HeightLevel.Impassable
                        && this.map.MapItems[x, y] != Enums.Item.Base
                        && this.map.MapItems[x, y] != Enums.Item.StartBase
                        && this.map.MapItems[x, y] != Enums.Item.GoldMinerals
                        && this.map.MapItems[x, y] != Enums.Item.BlueMinerals
                        && this.map.MapItems[x, y] != Enums.Item.Gas
                        && this.map.MapItems[x, y] != Enums.Item.XelNagaTower)
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        /// <summary>
        /// Checks that there is space for building around the base. All spaces within 5 range of the main base (7 from middle of the base)
        /// should be traversable. Does not account for items (gas/minerals, etc.)
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance describing how large a percentage of the tiles that are open. </returns>
        private double BaseSpace()
        {
            const int Radius = 10;

            // Finds the reachable tiles.
            List<Position> reachable;

            if (this.mfo.BaseSpaceIgnoreDestructibleRocks)
                reachable = MapHelper.GetReachableTilesFrom(
                    this.startBasePosition1.Item1,
                    this.startBasePosition1.Item2,
                    this.map.HeightLevels,
                    Radius);
            else
                reachable = MapHelper.GetReachableTilesFrom(
                    this.startBasePosition1.Item1,
                    this.startBasePosition1.Item2,
                    this.map.HeightLevels,
                    Radius,
                    this.map.DestructibleRocks);

            var max = (((Radius * 2) + 1) * ((Radius * 2) + 1)) - Math.Pow(5, 2);
            // ReSharper disable once ConvertToConstant.Local
            var min = 0d;
            var actual = reachable.Count - Math.Pow(5, 2);

            // Normalizes the value to between 0.0 and 1.0
            var normalized = (actual - min) / (max - min);
            return normalized * this.mfo.BaseSpaceSignificance;
        }

        /// <summary>
        /// Checks what height level the start base has been placed at.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 describing how high the base is compared to the highest possible point on the map. </returns>
        private double BaseHeightLevel()
        {
            var max = (double)(int)this.heighestLevel;
            // ReSharper disable once ConvertToConstant.Local
            var min = 0d;
            var actual = (double)(int)this.map.HeightLevels[this.startBasePosition1.Item1, this.startBasePosition1.Item2];

            var normalized = (actual - min) / (max - min);
            return normalized * this.mfo.BaseHeightSignificance;
        }

        /// <summary>
        /// Finds a value for the distance between the start bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance based on how far the bases are from each other. </returns>
        private double PathBetweenStartBases()
        {
            if (this.pathBetweenStartBases.Count <= 0)
                return -100000d;

            // Ground path distance
            var maxGround = (this.ySize * 0.70) + (this.xSize * 0.70);
            if (maxGround > this.mfo.PathStartMaxGroundDistance) maxGround = this.mfo.PathStartMaxGroundDistance;

            var minGround = (this.ySize * 0.1) + (this.xSize * 0.1);
            if (minGround > this.mfo.PathStartMinGroundDistance) minGround = this.mfo.PathStartMinGroundDistance;

            var actualGround = this.pathBetweenStartBases.Count > maxGround
                            ? maxGround - (this.pathBetweenStartBases.Count - maxGround)
                            : this.pathBetweenStartBases.Count;
            if (actualGround < minGround) actualGround = minGround;

            var normalizedGround = (actualGround - minGround) / (maxGround - minGround);

            // Direct line distance
            var maxDirect = Math.Sqrt(Math.Pow(this.xSize * 0.7, 2) + Math.Pow(this.ySize * 0.7, 2));
            if (maxDirect > this.mfo.PathStartMaxDirectDistance) maxDirect = this.mfo.PathStartMaxDirectDistance;

            var minDirect = Math.Sqrt(Math.Pow(this.xSize * 0.1, 2) + Math.Pow(this.ySize * 0.1, 2));
            if (minDirect > this.mfo.PathStartMinDirectDistance) minDirect = this.mfo.PathStartMinDirectDistance;

            var direct = Math.Sqrt(
                                Math.Pow(Math.Abs(this.startBasePosition1.Item1 - this.startBasePosition2.Item1), 2)
                                + Math.Pow(Math.Abs(this.startBasePosition1.Item2 - this.startBasePosition2.Item2), 2));
            var actualDirect = direct > maxDirect
                                ? maxDirect - (direct - maxDirect)
                                : direct;
            if (actualDirect < minDirect) actualDirect = minDirect;

            var normalizedDirect = (actualDirect - minDirect) / (maxDirect - minDirect);

            return ((normalizedGround + normalizedDirect) / 2) * this.mfo.PathBetweenStartBasesSignificance;
        }

        /// <summary>
        /// Figures out how many times a new height is reached during the path between the bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance based on how many times a new height is reached. </returns>
        private double NewHeightReached()
        {
            if (this.pathBetweenStartBases.Count <= 0)
                return 0d;

            var actual = 0d;
            var previousHeightLevel =
                this.map.HeightLevels[this.pathBetweenStartBases[0].Item1, this.pathBetweenStartBases[0].Item2];

            // Figure out how many times a new height is reached by looking at when ramps are encountered.
            foreach (var node in this.pathBetweenStartBases)
            {
                if ((this.map.HeightLevels[node.Item1, node.Item2] == Enums.HeightLevel.Ramp01
                     || this.map.HeightLevels[node.Item1, node.Item2] == Enums.HeightLevel.Ramp12)
                    && this.map.HeightLevels[node.Item1, node.Item2] != previousHeightLevel)
                {
                    actual += 1d;
                }

                previousHeightLevel = this.map.HeightLevels[node.Item1, node.Item2];
            }

            var max = this.mfo.NewHeightReachedMax;
            var min = this.mfo.NewHeightReachedMin;
            actual = (actual > max)
                        ? max - (actual - max)
                        : actual;
            if (actual < min) actual = min;

            var normalized = (actual - min) / (max - min);
            return normalized * this.mfo.NewHeightReachedSignificance;
        }

        /// <summary>
        /// Figures out how close the the non-natural expansions are to the start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance based on how far the non-natural expansions are from the start base. </returns>
        private double DistanceToNonNaturalExpansions()
        {
            if (this.bases.Count <= 0) return 0d;

            var noOfBasesPerStart = this.bases.Count / 2;
            var closestExpansions = new List<Tuple<int, double>>();

            for (var i = 0; i < this.bases.Count; i++)
            {
                // Calculate distance to expansion
                var tempBase = this.bases[i];
                var distance = Math.Abs(this.startBasePosition1.Item1 - tempBase.Item1)
                               + Math.Abs(this.startBasePosition1.Item2 - tempBase.Item2);

                // If list isn't "full" yet, add to the list and go to next base.
                if (closestExpansions.Count < noOfBasesPerStart)
                {
                    closestExpansions.Add(new Tuple<int, double>(i, distance));
                    continue;
                }

                // Find the expansion in closest that is the furthest from the base.
                var tempClosest = new Tuple<int, double>(-1, -1);
                for (var j = 0; j < closestExpansions.Count; j++)
                {
                    var exp = closestExpansions[j];
                    if (exp.Item2 > tempClosest.Item2)
                        tempClosest = new Tuple<int, double>(j, exp.Item2);
                }

                // If the current expansion is closer than the furthes one in the closest list, replace the one in the list.
                if (distance < tempClosest.Item2)
                    closestExpansions[tempClosest.Item1] = new Tuple<int, double>(i, distance);
            }

            // If there are not enough expansions, return 0 as fitness.
            if (closestExpansions.Count <= 1)
                return 0;

            // Removes the natural expansion from the list.
            closestExpansions = closestExpansions.OrderBy(x => x.Item2).ToList();
            closestExpansions.RemoveAt(0);

            // Calculate total normalized value for paths to the nearest expansions.
            var totalNormalized = 0.0d;
            for (var i = 0; i < closestExpansions.Count; i++)
            {
                var tempExpansion = this.bases[closestExpansions[i].Item1];
                var pathToExpansion = this.mapPathfinding.FindPathFromTo(this.startBasePosition1, tempExpansion, this.mfo.PathfindingIgnoreDestructibleRocks);

                // Ground distance
                var minGround = this.mfo.PathExpMinGroundDistance;
                double maxGround;

                switch (i)
                {
                    case 0:
                        maxGround = this.mfo.PathExpGroundDistances[0].Item2;
                        break;

                    case 1:
                        maxGround = this.mfo.PathExpGroundDistances[1].Item2;
                        break;

                    case 2:
                        maxGround = this.mfo.PathExpGroundDistances[2].Item2;
                        break;

                    default:
                        maxGround = this.mfo.PathExpGroundDistances[3].Item2;
                        break;
                }

                maxGround = (this.ySize * maxGround) + (this.xSize * maxGround);
                if (maxGround > this.mfo.PathExpMaxGroundDistance) maxGround = this.mfo.PathExpMaxGroundDistance;

                var actualDistance = (pathToExpansion.Count > maxGround)
                                         ? maxGround - (pathToExpansion.Count - maxGround)
                                         : pathToExpansion.Count;
                if (actualDistance < minGround) actualDistance = minGround;

                var normalizedGround = (actualDistance - minGround) / (maxGround - minGround);

                // Direct (flight) distanc
                double maxDirect;

                switch (i)
                {
                    case 0:
                        maxDirect = this.mfo.PathExpDirectDistances[0].Item2;
                        break;

                    case 1:
                        maxDirect = this.mfo.PathExpDirectDistances[1].Item2;
                        break;

                    case 2:
                        maxDirect = this.mfo.PathExpDirectDistances[2].Item2;
                        break;

                    default:
                        maxDirect = this.mfo.PathExpDirectDistances[3].Item2;
                        break;
                }

                maxDirect = Math.Sqrt(Math.Pow(this.xSize * maxDirect, 2) + Math.Pow(this.ySize * maxDirect, 2));
                if (maxDirect > this.mfo.PathExpMaxDirectDistance) maxDirect = this.mfo.PathExpMaxDirectDistance;

                var minDirect = Math.Sqrt(Math.Pow(this.xSize * 0.1, 2) + Math.Pow(this.ySize * 0.1, 2));
                if (minDirect > this.mfo.PathExpMinDirectDistance) minDirect = this.mfo.PathExpMinDirectDistance;

                var direct = Math.Sqrt(
                                    Math.Pow(Math.Abs(this.startBasePosition1.Item1 - tempExpansion.Item1), 2)
                                    + Math.Pow(Math.Abs(this.startBasePosition1.Item2 - tempExpansion.Item2), 2));
                var actualDirect = direct > maxDirect
                                    ? maxDirect - (direct - maxDirect)
                                    : direct;
                if (actualDirect < minDirect) actualDirect = minDirect;

                var normalizedDirect = (actualDirect - minDirect) / (maxDirect - minDirect);

                totalNormalized += (normalizedGround + normalizedDirect) / 2;
            }

            return (totalNormalized / closestExpansions.Count) * this.mfo.DistanceToExpansionsSignificance;
        }

        /// <summary>
        /// Figures out how close the closest expansion is to the start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance based on how far the nearest expansion is from the start base. </returns>
        private double DistanceToNaturalExpansion()
        {
            // Find the nearest expansion (if any)
            Position nearestExpansion = null;
            var closestDistance = 100000;
            foreach (var @base in this.bases)
            {
                var distance = Math.Abs(this.startBasePosition1.Item1 - @base.Item1)
                               + Math.Abs(this.startBasePosition1.Item2 - @base.Item2);

                if (distance >= closestDistance) continue;

                nearestExpansion = @base;
                closestDistance = distance;
            }

            // Attempt to find a path to the expansion.
            var pathToNearest = this.mapPathfinding.FindPathFromTo(
                this.startBasePosition1,
                nearestExpansion,
                this.mfo.PathfindingIgnoreDestructibleRocks);

            if (pathToNearest.Count == 0)
                return -100000d;

            // Ground distance
            var maxGround = (this.xSize * 0.4) + (this.ySize * 0.4);
            if (maxGround > this.mfo.PathNatMaxGroundDistance) maxGround = this.mfo.PathNatMaxGroundDistance;

            var minGround = (this.xSize * 0.1) + (this.ySize * 0.1);
            if (minGround > this.mfo.PathNatMinGroundDistance) minGround = this.mfo.PathNatMinGroundDistance;

            var actualGround = (pathToNearest.Count > maxGround)
                                     ? maxGround - (pathToNearest.Count - maxGround)
                                     : pathToNearest.Count;
            if (actualGround < minGround) actualGround = minGround;

            var normalizedGround = (actualGround - minGround) / (maxGround - minGround);

            // Direct distance
            var maxDirect = (this.xSize * 0.3) + (this.ySize * 0.3);
            if (maxDirect > this.mfo.PathNatMaxDirectDistance) maxDirect = this.mfo.PathNatMaxDirectDistance;

            var minDirect = (this.xSize * 0.1) + (this.ySize * 0.1);
            if (minDirect > this.mfo.PathNatMinDirectDistance) minDirect = this.mfo.PathNatMinDirectDistance;

            var actualDirect = (pathToNearest.Count > maxDirect)
                                     ? maxDirect - (pathToNearest.Count - maxDirect)
                                     : pathToNearest.Count;
            if (actualDirect < minDirect) actualDirect = minDirect;

            var normalizedDirect = (actualDirect - minDirect) / (maxDirect - minDirect);

            return ((normalizedGround + normalizedDirect) / 2) * this.mfo.DistanceToNaturalSignificance;
        }

        /// <summary>
        /// Figures out how many expansions that are available for each start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance based on how many expansions that are available for every start base. </returns>
        private double ExpansionsAvailable()
        {
            var max = this.mfo.ExpansionsAvailableMax;
            var min = this.mfo.ExpansionsAvailableMin;

            var actual = this.bases.Count / 2d;

            // If there are less bases than minimum, set number of bases to minimum.
            if (actual < min) actual = min;

            actual = (actual > max)
                        ? (int)(max - (actual - max))
                        : (int)actual;
            if (actual < min) actual = min;

            var normalized = (actual - min) / (max - min);
            return normalized * this.mfo.ExpansionsAvailableSignificance;
        }

        /// <summary>
        /// Figures out how many choke points that are available on the road between the two start bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 multiplied by significance based on how many choke points that are found. </returns>
        private double ChokePoints()
        {
            var chokePoints = 0d;
            var chokePointList = new List<Position>();

            var previousHeightLevel =
                this.map.HeightLevels[this.startBasePosition1.Item1, this.startBasePosition1.Item2];

            for (var nodeIndex = 0; nodeIndex < this.pathBetweenStartBases.Count; nodeIndex++)
            {
                var node = this.pathBetweenStartBases[nodeIndex];

                if (this.map.HeightLevels[node.Item1, node.Item2] == Enums.HeightLevel.Ramp01
                     || this.map.HeightLevels[node.Item1, node.Item2] == Enums.HeightLevel.Ramp12)
                {
                    //// If we encounter a ramp, perform a choke point check on ramps

                    if (this.map.HeightLevels[node.Item1, node.Item2] == previousHeightLevel) continue;
                    if (!this.IsRampChokePoint(this.pathBetweenStartBases[nodeIndex + 1], this.mfo.ChokePointsWidth)) continue;

                    chokePoints++;
                    chokePointList.Add(node);
                    this.map.MapItems[node.Item1, node.Item2] = Enums.Item.XelNagaTower;
                }
                else if (nodeIndex % this.mfo.ChokePointSearchStep == 0)
                {
                    //// Otherwise, only perform a choke point check every x steps.

                    if (!this.IsPositionChokePoint(node, this.mfo.ChokePointsWidth)) continue;

                    chokePoints++;
                    chokePointList.Add(node);
                    this.map.MapItems[node.Item1, node.Item2] = Enums.Item.XelNagaTower;
                }
            }

            double max = this.mfo.ChokePointsMax;
            double min = this.mfo.ChokePointsMin;
            var actual = (chokePoints > max)
                            ? max - (chokePoints - max)
                            : chokePoints;
            if (actual < min) actual = min;

            var normalized = (actual - min) / (max - min);
            return normalized * this.mfo.ChokePointsSignificance;
        }

        /// <summary>
        /// Figures out how many steps of the path between the start bases the two Xel'Naga towers cover.
        /// If the towers cover more than one base, their worth is halved. If they cover a start base, their worth is cut in four (these two stack).
        /// </summary>
        /// <returns> A value between 0.0 and 1.0 multiplied by significance based on how many steps that are covered. </returns>
        private double XelNagaPlacement()
        {
            // If no Xel'Naga tower are found, return 0.
            if (this.xelNagaPosition2 == null) return 0d;

            double stepsCovered2 =
                this.pathBetweenStartBases.Count(
                    step => MapHelper.CloseTo(step, this.xelNagaPosition2, this.mfo.DistanceToXelNaga));

            double max2 = this.mfo.StepsInXelNagaRangeMax;
            double min2 = this.mfo.StepsInXelNagaRangeMin;
            var actual2 = (stepsCovered2 > max2)
                            ? max2 - (stepsCovered2 - max2)
                            : stepsCovered2;
            if (actual2 < min2) actual2 = min2;

            var normalized2 = (actual2 - min2) / (max2 - min2);
            var basesInVision2 = this.bases.Count(@base => MapHelper.CloseTo(this.xelNagaPosition2, @base));
            var startBaseInVision2 = MapHelper.CloseTo(this.xelNagaPosition2, this.startBasePosition1)
                                      || MapHelper.CloseTo(this.xelNagaPosition2, this.startBasePosition2);

            if (basesInVision2 >= 2) normalized2 /= 2;
            if (startBaseInVision2) normalized2 /= 4;

            // If only one Xel'Naga tower is found, return the significance for just that one, but halved.
            if (this.xelNagaPosition1 == null)
                return (normalized2 * this.mfo.XelNagaPlacementSignificance) / 2d;

            double stepsCovered1 =
                this.pathBetweenStartBases.Count(
                    step => MapHelper.CloseTo(step, this.xelNagaPosition1, this.mfo.DistanceToXelNaga));

            double max1 = this.mfo.StepsInXelNagaRangeMax;
            double min1 = this.mfo.StepsInXelNagaRangeMin;
            var actual1 = (stepsCovered1 > max1)
                            ? max1 - (stepsCovered1 - max1)
                            : stepsCovered1;
            if (actual1 < min1) actual1 = min1;

            var normalized1 = (actual1 - min1) / (max1 - min1);
            var basesInVision1 = this.bases.Count(@base => MapHelper.CloseTo(this.xelNagaPosition1, @base));
            var startBaseInVision1 = MapHelper.CloseTo(this.xelNagaPosition1, this.startBasePosition1)
                                      || MapHelper.CloseTo(this.xelNagaPosition1, this.startBasePosition2);

            if (basesInVision1 >= 2) normalized1 /= 2;
            if (startBaseInVision1) normalized1 /= 4;

            return ((normalized1 + normalized2) / 2d) * this.mfo.XelNagaPlacementSignificance;
        }

        /// <summary>
        /// Checks if the given position is a choke point by looking in 8 directions.
        /// </summary>
        /// <param name="pos"> The position to check. </param>
        /// <param name="width"> The width of a choke point. </param>
        /// <returns> True if the position is a choke point; false otherwise. </returns>
        private bool IsPositionChokePoint(Position pos, int width)
        {
            // The directions represented as a position, along with how far there is to the nearest cliff.
            var directions = new List<Tuple<Position, int>>
                            {
                                new Tuple<Position, int>(new Position(-1, 0),  -1), // West
                                new Tuple<Position, int>(new Position(1, 0),   -1), // East
                                new Tuple<Position, int>(new Position(0, -1),  -1), // South
                                new Tuple<Position, int>(new Position(0, 1),   -1), // North
                                new Tuple<Position, int>(new Position(1, -1),  -1), // South-east
                                new Tuple<Position, int>(new Position(-1, 1),  -1), // North-west
                                new Tuple<Position, int>(new Position(-1, -1), -1), // South-west
                                new Tuple<Position, int>(new Position(1, 1),   -1)  // North-east
                            };

            // The opposite directions.
            var directionCombinations = new Dictionary<int, int>
                            {
                                { 0, 1 }, // West + East
                                { 2, 3 }, // South + north
                                { 4, 5 }, // South-east + north-west
                                { 6, 7 }  // South-west + north-east
                            };

            // Find the nearest cliff in each of the 8 directions.
            for (var i = 0; i < width + 1; i++)
            {
                foreach (var dc in directionCombinations)
                {
                    var dir1 = directions[dc.Key].Item1;
                    var dir2 = directions[dc.Value].Item1;

                    var newPos1 = new Position(pos.Item1 + (dir1.Item1 * i), pos.Item2 + (dir1.Item2 * i));
                    var newPos2 = new Position(pos.Item1 + (dir2.Item1 * i), pos.Item2 + (dir2.Item2 * i));

                    if (MapHelper.WithinMapBounds(newPos1, this.map.XSize, this.map.YSize))
                    {
                        if (directions[dc.Key].Item2 < 0
                            && this.map.HeightLevels[newPos1.Item1, newPos1.Item2] == Enums.HeightLevel.Cliff)
                            directions[dc.Key] = new Tuple<Position, int>(dir1, i);
                    }

                    if (MapHelper.WithinMapBounds(newPos2, this.map.XSize, this.map.YSize))
                    {
                        if (directions[dc.Value].Item2 < 0
                            && this.map.HeightLevels[newPos2.Item1, newPos2.Item2] == Enums.HeightLevel.Cliff)
                            directions[dc.Value] = new Tuple<Position, int>(dir2, i);
                    }
                }
            }

            // The directions to check the ranges between to get a rather clear view
            var finalCombinations = new List<Position>
                                        {
                                            new Position(0, 1),
                                            new Position(0, 4),
                                            new Position(0, 7),
                                            new Position(1, 6),
                                            new Position(1, 5),
                                            new Position(2, 3),
                                            new Position(2, 5),
                                            new Position(2, 7),
                                            new Position(3, 6),
                                            new Position(3, 4)
                                        };

            // Check the range to cliffs for every interesting direction combination
            foreach (var fc in finalCombinations)
            {
                var dir1Value = directions[fc.Item1].Item2;
                var dir2Value = directions[fc.Item2].Item2;

                if (dir1Value < 0) continue;
                if (dir2Value < 0) continue;

                // -1 because otherwise we count the start position twice.
                if ((dir1Value + dir2Value) - 1 <= width) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the position is a ramp with a choke point of the given width or not.
        /// </summary>
        /// <param name="pos"> The position to check for a choke point. </param>
        /// <param name="width"> The width a place should have to be considered a choke point. </param>
        /// <returns> True if the place is a choke point; false otherwise. </returns>
        private bool IsRampChokePoint(Position pos, int width)
        {
            var originalRampType = this.map.HeightLevels[pos.Item1, pos.Item2];
            var horizontalFirstEncounter = this.map.HeightLevels[pos.Item1, pos.Item2];
            var verticalFirstEncounter = this.map.HeightLevels[pos.Item1, pos.Item2];

            // Figure out what is hit first when going either right or up
            for (var i = 0; i < 10; i++)
            {
                // Check to the right
                if (MapHelper.WithinMapBounds(pos.Item1, pos.Item2 + i, this.xSize, this.ySize))
                {
                    var right = this.map.HeightLevels[pos.Item1, pos.Item2 + i];
                    if (horizontalFirstEncounter == originalRampType
                        && right != originalRampType)
                        horizontalFirstEncounter = this.map.HeightLevels[pos.Item1, pos.Item2 + i];
                }

                // Check up
                if (MapHelper.WithinMapBounds(pos.Item1 + i, pos.Item2, this.xSize, this.ySize))
                {
                    var up = this.map.HeightLevels[pos.Item1 + i, pos.Item2];
                    if (verticalFirstEncounter == originalRampType
                        && up != originalRampType)
                        verticalFirstEncounter = this.map.HeightLevels[pos.Item1 + i, pos.Item2];
                }
            }

            // Decide which way the the edges of the ramp are.
            var directionChange = (horizontalFirstEncounter == Enums.HeightLevel.Cliff
                                    && verticalFirstEncounter != Enums.HeightLevel.Cliff)
                                      ? new Position(0, 1)
                                      : new Position(1, 0);

            var firstPos = new Position(pos.Item1, pos.Item2);
            var secondPos = new Position(pos.Item1, pos.Item2);

            // Find the two sides of the ramp.
            for (var i = 0; i < 10; i++)
            {
                // If not within the map, don't bother.
                if (!MapHelper.WithinMapBounds(
                        pos.Item1 + (i * directionChange.Item1),
                        pos.Item2 + (i * directionChange.Item2),
                        this.xSize,
                        this.ySize)) continue;

                if (!MapHelper.WithinMapBounds(
                        pos.Item1 - (i * directionChange.Item1),
                        pos.Item2 - (i * directionChange.Item2),
                        this.xSize,
                        this.ySize)) continue;

                if (firstPos.Equals(pos)
                    && this.map.HeightLevels[pos.Item1 + (i * directionChange.Item1), pos.Item2 + (i * directionChange.Item2)] == Enums.HeightLevel.Cliff)
                    firstPos = new Position(
                        pos.Item1 + (i * directionChange.Item1),
                        pos.Item2 + (i * directionChange.Item2));

                if (secondPos.Equals(pos)
                    && this.map.HeightLevels[pos.Item1 - (i * directionChange.Item1), pos.Item2 - (i * directionChange.Item2)] == Enums.HeightLevel.Cliff)
                    secondPos = new Position(
                        pos.Item1 - (i * directionChange.Item1),
                        pos.Item2 - (i * directionChange.Item2));
            }

            // Check the actual width.
            var actualWidth = Math.Abs(firstPos.Item1 - secondPos.Item1) + Math.Abs(firstPos.Item2 - secondPos.Item2) - 1;
            return actualWidth <= width;
        }
    }
}
