namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// A class that can calculate fitness value for a map.
    /// </summary>
    public class MapFitness
    {
        /// <summary>
        /// The significance of the space around the start base.
        /// </summary>
        private readonly int baseSpaceSignificance = 5;

        /// <summary>
        /// The significance of the height of the start base.
        /// </summary>
        private readonly int baseHeightSignificance = 5;

        /// <summary>
        /// The significance of the distance between the bases.
        /// </summary>
        private readonly int pathBetweenBasesSignificance = 5;

        /// <summary>
        /// The significance of how many times a new height is reached.
        /// </summary>
        private readonly int newHeightReachedSignificance = 5;

        /// <summary>
        /// The significance of how far there is to the nearest expansion.
        /// </summary>
        private readonly int distanceToExpansionSignificance = 5;

        /// <summary>
        /// The significance of how many expansions that are available.
        /// </summary>
        private readonly int expansionsAvailableSignificance = 5;

        /// <summary>
        /// The significance of how many choke points that are available.
        /// </summary>
        private readonly int chokePointsSignificance = 5;

        /// <summary>
        /// Every x (the value) will be checked for a choke point.
        /// </summary>
        private readonly int chokePointSearchStep = 3;

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
        /// The position of the first start base.
        /// </summary>
        private Position startBasePosition1;

        /// <summary>
        /// The position of the first second base.
        /// </summary>
        private Position startBasePosition2;

        /// <summary>
        /// The path between bases.
        /// </summary>
        private List<Position> pathBetweenStartBases;

        /// <summary>
        /// The positions of the normal bases found.
        /// </summary>
        private List<Position> bases;

        /// <summary>
        /// The furthest an expansion should be from the main base, depending
        /// </summary>
        private List<Tuple<int, double>> expansionMaxDistances;

        /// <summary>
        /// The highest level found.
        /// </summary>
        private Enums.HeightLevel heighestLevel = Enums.HeightLevel.Height0;

        /// <summary>
        /// The pathfinding used.
        /// </summary>
        private JPSMapPathfinding mapPathfinding;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapFitness"/> class. 
        /// </summary>
        /// <param name="map"> The map to calculate fitness for. </param>
        /// <param name="baseSpaceSignificance"> The significance of the space around the start base. </param>
        /// <param name="baseHeightSignificance"> The significance of the height of the start base. </param>
        /// <param name="pathBetweenBasesSignificance"> The significance of the distance between the bases. </param>
        /// <param name="newHeightReachedSignificance"> The significance of how many times a new height is reached. </param>
        /// <param name="distanceToExpansionSignificance"> The significance of how far there is to the nearest expansion. </param>
        /// <param name="expansionsAvailableSignificance"> The significance of how many expansions that are available. </param>
        /// <param name="chokePointsSignificance"> The significance of how many choke points that are available. </param>
        public MapFitness(
            MapPhenotype map,
            int baseSpaceSignificance = 5,
            int baseHeightSignificance = 5,
            int pathBetweenBasesSignificance = 5,
            int newHeightReachedSignificance = 5,
            int distanceToExpansionSignificance = 5,
            int expansionsAvailableSignificance = 5,
            int chokePointsSignificance = 5)
        {
            // TODO: Grooss - Implement settings 
            this.ySize = map.YSize;
            this.xSize = map.XSize;
            this.map = map;
            this.mapPathfinding = new JPSMapPathfinding(map.HeightLevels, map.DestructibleRocks);
            this.expansionMaxDistances = new List<Tuple<int, double>> 
                    {
                        new Tuple<int, double>(1, 0.3),
                        new Tuple<int, double>(2, 0.5),
                        new Tuple<int, double>(3, 0.7),
                        new Tuple<int, double>(4, 0.9)
                    };

            this.baseSpaceSignificance = baseSpaceSignificance;
            this.baseHeightSignificance = baseHeightSignificance;
            this.pathBetweenBasesSignificance = pathBetweenBasesSignificance;
            this.newHeightReachedSignificance = newHeightReachedSignificance;
            this.distanceToExpansionSignificance = distanceToExpansionSignificance;
            this.expansionsAvailableSignificance = expansionsAvailableSignificance;
            this.chokePointsSignificance = chokePointsSignificance;
        }

        /*
        public MapFitness(MapPhenotype map, ) : base(map)
        {
        }
        */

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

                    // Check if the area is a base
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.Base
                        && !MapHelper.CloseToAny(new Position(tempX, tempY), this.bases))
                    {
                        this.bases.Add(new Position(tempX + 2, tempY - 2));
                    }
                }
            }

            if (this.startBasePosition1 == null || this.startBasePosition2 == null) return -200000;

            var sw = new Stopwatch();
            sw.Start();
            this.pathBetweenStartBases = this.mapPathfinding.FindPathFromTo(
                this.startBasePosition1,
                this.startBasePosition2);
            //Console.WriteLine("Path Between Bases: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.BaseSpace();
            //Console.WriteLine("Base Space: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.BaseHeightLevel();
            //Console.WriteLine("Base Height Level: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.PathBetweenStartBases();
            //Console.WriteLine("Path Between Start Bases: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.NewHeightReached();
            //Console.WriteLine("New Height Reached: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.DistanceToAllExpansions();
            //Console.WriteLine("Distance To Nearest Expansion: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.ExpansionsAvailable();
            //Console.WriteLine("Expansions Available: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            fitness += this.ChokePoints();
            //Console.WriteLine("Choke Points: " + sw.ElapsedMilliseconds + " - " + fitness);
            sw.Restart();

            //// Fitness:
            ////  X Base space (amount of tiles around the base that are passable)
            ////  X Base height (is the start base on heigh ground)
            ////  X Distance between start bases
            ////  X How many times is a new height reached?
            ////  X Distance to nearest expansion
            ////  X Expansions available
            ////  X Choke points (find route between bases and look for ramps, then find their width)
            
            return fitness;
        }

        /// <summary>
        /// Checks that there is space for building around the base. All spaces within 5 range of the main base (7 from middle of the base)
        /// should be traversable. Does not account for items (gas/minerals, etc.)
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 describing how large a percentage of the tiles that are open. </returns>
        private double BaseSpace()
        {
            const int Radius = 10;

            // Finds the reachable tiles.
            var reachable = MapHelper.GetReachableTilesFrom(
                this.startBasePosition1.Item1,
                this.startBasePosition1.Item2,
                this.map.HeightLevels,
                Radius);

            var max = (((Radius * 2) + 1) * ((Radius * 2) + 1)) - Math.Pow(5, 2);
            var min = 0;
            var actual = reachable.Count - Math.Pow(5, 2);

            // Normalizes the value to between 0.0 and 1.0
            var normalized = (actual - min) / (max - min);
            return normalized * this.baseSpaceSignificance;
        }

        /// <summary>
        /// Checks what height level the start base has been placed at.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 describing how high the base is compared to the highest possible point on the map. </returns>
        private double BaseHeightLevel()
        {
            var max = (double)(int)this.heighestLevel;
            var min = 0d;
            var actual = (double)(int)this.map.HeightLevels[this.startBasePosition1.Item1, this.startBasePosition1.Item2];

            var normalized = (actual - min) / (max - min);
            return normalized * this.baseHeightSignificance;
        }

        /// <summary>
        /// Finds a value for the distance between the start bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how far the bases are from each other. </returns>
        private double PathBetweenStartBases()
        {
            if (this.pathBetweenStartBases.Count <= 0) 
                return -100000d;

            // Ground path distance
            var maxGround = (this.ySize * 0.70) + (this.xSize * 0.70);
            var minGround = (this.ySize * 0.1) + (this.xSize * 0.1);
            var actualGround = this.pathBetweenStartBases.Count > maxGround
                            ? maxGround - (this.pathBetweenStartBases.Count - maxGround)
                            : this.pathBetweenStartBases.Count;
            var normalizedGround = (actualGround - minGround) / (maxGround - minGround);

            // Direct line distance
            var maxDirect = Math.Sqrt(Math.Pow(this.xSize * 0.7, 2) + Math.Pow(this.ySize * 0.7, 2));
            var minDirect = Math.Sqrt(Math.Pow(this.xSize * 0.1, 2) + Math.Pow(this.ySize * 0.1, 2));
            var direct = Math.Sqrt(
                                Math.Pow(Math.Abs(this.startBasePosition1.Item1 - this.startBasePosition2.Item1), 2)
                                + Math.Pow(Math.Abs(this.startBasePosition1.Item2 - this.startBasePosition2.Item2), 2));
            var actualDirect = direct > maxDirect
                                ? maxDirect - (direct - maxDirect)
                                : direct;
            var normalizedDirect = (actualDirect - minDirect) / (maxDirect - minDirect);

            return ((normalizedGround + normalizedDirect) / 2) * this.pathBetweenBasesSignificance;
        }

        /// <summary>
        /// Figures out how many times a new height is reached during the path between the bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how many times a new height is reached. </returns>
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

            const double Max = 5d;
            const double Min = 0d;
            actual = (actual > Max)
                        ? Max - (actual - Max)
                        : actual;

            var normalized = (actual - Min) / (Max - Min);
            return normalized * this.newHeightReachedSignificance;
        }

        /// <summary>
        /// Figures out how close the X closest expansions are to the start base, where X is half the expansions.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how far the X nearest expansion are from the start base. </returns>
        private double DistanceToAllExpansions()
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

            // Calculate total normalized value for paths to the nearest expansions.
            var totalNormalized = 0.0d;
            for (int i = 0; i < closestExpansions.Count; i++)
            {
                var tempExpansion = this.bases[closestExpansions[i].Item1];
                var pathToExpansion = this.mapPathfinding.FindPathFromTo(this.startBasePosition1, tempExpansion);

                // Ground distance
                var min = 0.1;
                var max = 0.1;
                var actualDistance = (pathToExpansion.Count > max)
                                         ? max - (pathToExpansion.Count - max)
                                         : pathToExpansion.Count;

                switch (i)
                {
                    case 0:
                        max = this.expansionMaxDistances[0].Item2;
                        break;

                    case 1:
                        max = this.expansionMaxDistances[1].Item2;
                        break;

                    case 2:
                        max = this.expansionMaxDistances[2].Item2;
                        break;

                    default:
                        max = this.expansionMaxDistances[3].Item2;
                        break;
                }

                var normalizedGround = (actualDistance - min) / (max - min);

                // Direct (flight) distance
                var maxDirect = Math.Sqrt(Math.Pow(this.xSize * 0.7, 2) + Math.Pow(this.ySize * 0.7, 2));
                var minDirect = Math.Sqrt(Math.Pow(this.xSize * 0.1, 2) + Math.Pow(this.ySize * 0.1, 2));
                var direct = Math.Sqrt(
                                    Math.Pow(Math.Abs(this.startBasePosition1.Item1 - tempExpansion.Item1), 2)
                                    + Math.Pow(Math.Abs(this.startBasePosition1.Item2 - tempExpansion.Item2), 2));
                var actualDirect = direct > maxDirect
                                    ? maxDirect - (direct - maxDirect)
                                    : direct;
                var normalizedDirect = (actualDirect - minDirect) / (maxDirect - minDirect);

                totalNormalized += (normalizedGround + normalizedDirect) / 2;
            }

            return (totalNormalized / closestExpansions.Count) * this.distanceToExpansionSignificance;
        }

        /// <summary>
        /// Figures out how close the closest expansion is to the start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how far the nearest expansion is from the start base. </returns>
        private double DistanceToNearestExpansion()
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
                nearestExpansion);

            if (pathToNearest.Count == 0)
                return -100000d;

            var max = this.ySize * 0.4;
            var min = this.ySize * 0.1;
            var actualDistance = (pathToNearest.Count > max)
                                     ? max - (pathToNearest.Count - max)
                                     : pathToNearest.Count;

            var normalized = (actualDistance - min) / (max - min);
            return normalized * this.distanceToExpansionSignificance;
        }

        /// <summary>
        /// Figures out how many expansions that are available for each start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how many expansions that are available for every start base. </returns>
        private double ExpansionsAvailable()
        {
            //// ReSharper disable RedundantCast
            var max = (double)(int)(this.ySize / 40);
            var min = (double)(int)(this.ySize / 60);
            //// ReSharper restore RedundantCast

            // ITODO: Fix for too few bases
            var actual = (this.bases.Count / 2d > max) 
                            ? max - ((this.bases.Count  / 2d) - max)
                            : this.bases.Count / 2d;

            var normalized = ((actual / 2d) - min) / (max - min);
            return normalized * this.expansionsAvailableSignificance;
        }

        /// <summary>
        /// Figures out how many choke points that are available on the road between the two start bases.
        /// </summary>
        /// <param name="chokePointWidth"> The choke Point Width. </param>
        /// <returns> A number between 0.0 and 1.0 based on how many choke points that are found. </returns>
        private double ChokePoints(int chokePointWidth = 2)
        {
            var chokePoints = 0;

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
                    if (!this.IsRampChokePoint(this.pathBetweenStartBases[nodeIndex + 1], chokePointWidth)) continue;

                    chokePoints++;
                }
                else if (nodeIndex % this.chokePointSearchStep == 0)
                {
                    //// Otherwise, only perform a choke point check every x steps.

                    if (!this.IsPositionChokePoint(this.pathBetweenStartBases[nodeIndex], chokePointWidth)) continue;

                    chokePoints++;
                }
            }

            const int Max = 4;
            const int Min = 0;
            var actual = (chokePoints > Max) 
                            ? Max - (chokePoints - Max) 
                            : chokePoints;

            var normalized = (actual - Max) / (Max - Min);
            return normalized * this.chokePointsSignificance;
        }

        /// <summary>
        /// Checks if the given position is a choke point by looking in 8 directions.
        /// </summary>
        /// <param name="pos"> The position to check. </param>
        /// <param name="width"> The width of a choke point. </param>
        /// <returns> True if the position is a choke point; false otherwise. </returns>
        private bool IsPositionChokePoint(Position pos, int width = 2)
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
        private bool IsRampChokePoint(Position pos, int width = 2)
        {
            var originalRampType = this.map.HeightLevels[pos.Item1, pos.Item2];
            var horizontalFirstEncounter = this.map.HeightLevels[pos.Item1, pos.Item2];
            var verticalFirstEncounter = this.map.HeightLevels[pos.Item1, pos.Item2];

            // Figure out what is hit first when going either right or up
            for (var i = 0; i < 10; i++)
            {   
                // ITODO: Grooss noob ffs abandon report; det kan vaere outside map CYKA BLYAT!!1!1!!! no wards
                var right = this.map.HeightLevels[pos.Item1, pos.Item2 + i];
                var up = this.map.HeightLevels[pos.Item1 + i, pos.Item2];
                if (horizontalFirstEncounter == originalRampType
                    && right != originalRampType) 
                    horizontalFirstEncounter = this.map.HeightLevels[pos.Item1, pos.Item2 + i];
                if (verticalFirstEncounter == originalRampType
                    && up != originalRampType) 
                    verticalFirstEncounter = this.map.HeightLevels[pos.Item1 + i, pos.Item2];
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
