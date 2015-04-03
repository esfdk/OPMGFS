namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// A class that can calculate fitness value for a map.
    /// </summary>
    public class MapFitness
    {
        /// <summary>
        /// The significance of the space around the start base.
        /// </summary>
        private const int BaseSpaceSignificance = 5;

        /// <summary>
        /// The significance of the height of the start base.
        /// </summary>
        private const int BaseHeightSignificance = 5;

        /// <summary>
        /// The significance of the distance between the bases.
        /// </summary>
        private const int PathBetweenBasesSignificance = 5;

        /// <summary>
        /// The significance of how many times a new height is reached.
        /// </summary>
        private const int NewHeightReachedSignificance = 5;

        /// <summary>
        /// The significance of how many times a new height is reached.
        /// </summary>
        private const int DistanceToExpansionSignificance = 5;

        /// <summary>
        /// The significance of how many expansions that are available.
        /// </summary>
        private const int ExpansionsAvailableSignificance = 5;

        /// <summary>
        /// The significance of how many choke points that are available.
        /// </summary>
        private const int ChokePointsSignificance = 5;

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
        private List<Position> pathBetweenBases;

        /// <summary>
        /// The positions of the normal bases found.
        /// </summary>
        private List<Position> bases;

        /// <summary>
        /// The highest level found.
        /// </summary>
        private Enums.HeightLevel heightestLevel = Enums.HeightLevel.Height0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapFitness"/> class. 
        /// </summary>
        /// <param name="map"> The map to calculate fitness for. </param>
        public MapFitness(MapPhenotype map)
        {
            this.ySize = map.YSize;
            this.xSize = map.XSize;
            this.map = map;
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
                        && this.map.HeightLevels[tempX, tempY] > this.heightestLevel)
                    {
                        this.heightestLevel = this.map.HeightLevels[tempX, tempY];
                    }

                    // Check if the area is a base
                    if (this.map.MapItems[tempX, tempY] == Enums.Item.Base
                        && !this.CloseToAny(new Position(tempX, tempY), this.bases))
                    {
                        this.bases.Add(new Position(tempX + 2, tempY - 2));
                    }
                }
            }

            this.pathBetweenBases = MapPathfinding.FindPathFromTo(
                this.map.HeightLevels,
                this.startBasePosition1,
                this.startBasePosition2);

            fitness += this.BaseSpace();
            Console.WriteLine("1: " + fitness);
            fitness += this.BaseHeightLevel();
            Console.WriteLine("2: " + fitness);
            fitness += this.PathBetweenStartBases();
            Console.WriteLine("3: " + fitness);
            fitness += this.NewHeightReached();
            Console.WriteLine("4: " + fitness);
            fitness += this.DistanceToNearestExpansion();
            Console.WriteLine("5: " + fitness);
            fitness += this.ExpansionsAvailable();
            Console.WriteLine("6: " + fitness);
            fitness += this.ChokePoints();
            Console.WriteLine("7: " + fitness);

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

            var maxReachable = (((Radius * 2) + 1) * ((Radius * 2) + 1)) - Math.Pow(5, 2);
            var actualReachable = reachable.Count - Math.Pow(5, 2);

            // Normalizes the value to between 0.0 and 1.0
            var normalized = (actualReachable - 0) / (maxReachable - 0);
            return normalized * BaseSpaceSignificance;
        }

        /// <summary>
        /// Checks what height level the start base has been placed at.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 describing how high the base is compared to the highest possible point on the map. </returns>
        private double BaseHeightLevel()
        {
            var actualBaseHeight = (double)(int)this.map.HeightLevels[this.startBasePosition1.Item1, this.startBasePosition1.Item2];
            var maxHeight = (double)(int)this.heightestLevel;

            var normalized = (actualBaseHeight - 0d) / (maxHeight - 0d);
            return normalized * BaseHeightSignificance;
        }

        /// <summary>
        /// Finds a value for the distance between the start bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how far the bases are from each other. </returns>
        private double PathBetweenStartBases()
        {
            if (this.pathBetweenBases.Count <= 0) 
                return -100000d;

            var minPath = (this.ySize * 0.1) + (this.xSize * 0.1);
            var maxPath = (this.ySize * 0.70) + (this.xSize * 0.70);
            var actualPath = this.pathBetweenBases.Count > maxPath
                                 ? maxPath - (this.pathBetweenBases.Count - maxPath)
                                 : this.pathBetweenBases.Count;

            var normalized = (actualPath - minPath) / (maxPath - minPath);
            return normalized * PathBetweenBasesSignificance;
        }

        /// <summary>
        /// Figures out how many times a new height is reached during the path between the bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how many times a new height is reached. </returns>
        private double NewHeightReached()
        {
            const double MinHeightChange = 0d;
            const double MaxHeightChange = 5d;

            if (this.pathBetweenBases.Count <= 0) 
                return 0d;

            var actualChanges = 0d;
            var previousHeightLevel =
                this.map.HeightLevels[this.pathBetweenBases[0].Item1, this.pathBetweenBases[0].Item2];

            // Figure out how many times a new height is reached by looking at when ramps are encountered.
            foreach (var node in this.pathBetweenBases)
            {
                if ((this.map.HeightLevels[node.Item1, node.Item2] == Enums.HeightLevel.Ramp01
                     || this.map.HeightLevels[node.Item1, node.Item2] == Enums.HeightLevel.Ramp12)
                    && this.map.HeightLevels[node.Item1, node.Item2] != previousHeightLevel)
                {
                    actualChanges += 1d;
                }

                previousHeightLevel = this.map.HeightLevels[node.Item1, node.Item2];
            }

            actualChanges = (actualChanges > MaxHeightChange)
                                ? MaxHeightChange - (actualChanges - MaxHeightChange)
                                : actualChanges;

            var normalized = (actualChanges - MinHeightChange) / (MaxHeightChange - MinHeightChange);
            return normalized * NewHeightReachedSignificance;
        }

        /// <summary>
        /// Figures out how close the closest expansion is to the start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how many times a new height is reached. </returns>
        private double DistanceToNearestExpansion()
        {
            // Find the nearest expansion, ignoring whether pathing is possible or not.
            var nearestExpansion = MapHelper.FindNearestTileOfType(
                this.startBasePosition1.Item1,
                this.startBasePosition1.Item2,
                this.map.MapItems,
                Enums.Item.Base);

            if (nearestExpansion == null)
                return -100000d;

            nearestExpansion = new Position(nearestExpansion.Item1 + 2, nearestExpansion.Item2 - 2); // The middle of the base

            // Attempt to find a path to the expansion.
            var pathToNearest = MapPathfinding.FindPathFromTo(
                this.map.HeightLevels,
                this.startBasePosition1,
                nearestExpansion);

            var maxPathToExpansion = this.ySize * 0.4;
            var minPathToExpansion = this.ySize * 0.1;

            var normalized = (pathToNearest.Count - minPathToExpansion) / (maxPathToExpansion - minPathToExpansion);
            return normalized * DistanceToExpansionSignificance;
        }

        /// <summary>
        /// Figures out how many expansions that are available for each start base.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how many times a new height is reached. </returns>
        private double ExpansionsAvailable()
        {
            //// ReSharper disable RedundantCast
            var maxExpasions = (double)(int)(this.ySize / 40);
            var minExpansions = (double)(int)(this.ySize / 20);
            //// ReSharper restore RedundantCast

            var normalized = ((this.bases.Count / 2d) - minExpansions) / (maxExpasions - minExpansions);
            return normalized * ExpansionsAvailableSignificance;
        }

        /// <summary>
        /// Figures out how many choke points that are available on the road between the two start bases.
        /// </summary>
        /// <returns> A number between 0.0 and 1.0 based on how many choke points that are found. </returns>
        private double ChokePoints()
        {
            var chokePoints = 0;

            var previousHeightLevel =
                this.map.HeightLevels[this.startBasePosition1.Item1, this.startBasePosition1.Item2];

            for (var nodeIndex = 0; nodeIndex < this.pathBetweenBases.Count; nodeIndex++)
            {
                var node = this.pathBetweenBases[nodeIndex];
                
                if (this.map.HeightLevels[node.Item1, node.Item2] != Enums.HeightLevel.Ramp01
                     || this.map.HeightLevels[node.Item1, node.Item2] != Enums.HeightLevel.Ramp12)
                    continue;
                if (this.map.HeightLevels[node.Item1, node.Item2] == previousHeightLevel) continue;
                if (!this.IsRampChokePoint(this.pathBetweenBases[nodeIndex + 1])) continue;
                
                chokePoints++;
            }

            var normalized = (chokePoints - 2) / (4 - 2);
            return normalized * ChokePointsSignificance;
        }

        /// <summary>
        /// Checks if the given position is close to any of the positions in the list.
        /// </summary>
        /// <param name="pos"> The position to check. </param>
        /// <param name="positions"> The list of positions to compare to. </param>
        /// <param name="range"> The range that defines "close to". </param>
        /// <returns> True if the position is close to any of the positions in the list; false otherwise. </returns>
        private bool CloseToAny(Position pos, IEnumerable<Position> positions, int range = 3)
        {
            return positions.Any(position => Math.Abs(pos.Item1 - position.Item1) <= range || Math.Abs(pos.Item2 - position.Item2) <= range);
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
            Console.WriteLine(actualWidth);
            return actualWidth <= width;
        }
    }
}
