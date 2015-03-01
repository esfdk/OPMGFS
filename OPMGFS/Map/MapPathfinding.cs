// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapPathfinding.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPathfinding type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// The class that handles pathfinding on a map.
    /// </summary>
    public static class MapPathfinding
    {
        /// <summary>
        /// Finds the shortest path on a map using A*.
        /// </summary>
        /// <param name="mapHeightLevels"> The height levels of the map. </param>
        /// <param name="mapItems"> The items contained in the map. </param>
        /// <param name="startPosition"> The position to start the pathfinding from. </param>
        /// <param name="endPosition"> The position to find a path to. </param>
        /// <returns> A list of the positions that make up the path. </returns>
        public static List<Position> FindPathFromTo(Enums.HeightLevel[,] mapHeightLevels, Enums.Item[,] mapItems, Position startPosition, Position endPosition)
        {
            // The lists used.
            var closedList = new List<AStarNode>();
            var openList = new List<AStarNode>();

            // The initial node.
            var startNode = new AStarNode
                                {
                                    Position = startPosition,
                                    CameFrom = null,
                                    GScore = 0d,
                                    FScore = 0d + CostEstimate(startPosition, endPosition)
                                };

            openList.Add(startNode);

            // Loops as long as there are non-visited nodes left.
            while (openList.Count > 0)
            {
                // Grab the first node in the list.
                var current = openList[0];

                // If we have found the end, reconstruct the path and return it.
                if (current.Position.Equals(endPosition))
                    return ReconstructPath(current);

                // Remove the node from the list and add it to the closed.
                openList.Remove(current);
                closedList.Add(current);

                // Find neighbours and iterate over them.
                var neighbours = Neighbours(mapHeightLevels, mapItems, current.Position);
                foreach (var neighbourPos in neighbours)
                {
                    var neighbour = new AStarNode { Position = neighbourPos, };

                    // If the neighbour has been visited already, ignore it.
                    if (closedList.Contains(neighbour))
                        continue;

                    var tentGScore = current.GScore + CostEstimate(current.Position, neighbourPos);

                    if (!openList.Contains(neighbour))
                    {
                        // If open list does not contain the neighbour, add it.
                        neighbour.CameFrom = current;
                        neighbour.GScore = tentGScore;
                        neighbour.FScore = neighbour.GScore + CostEstimate(neighbour.Position, endPosition);
                        openList.Add(neighbour);
                    }
                    else if (openList.Contains(neighbour))
                    {
                        // If the open list contains the neighbour and the tentGScore is lower than the GScore of the already-existing
                        // neighbour, update the neighbour in the list.
                        var neighbourIndex = openList.IndexOf(neighbour);
                        if (tentGScore < openList[neighbourIndex].GScore)
                        {
                            openList[neighbourIndex].CameFrom = current;
                            openList[neighbourIndex].GScore = tentGScore;
                            openList[neighbourIndex].FScore = neighbour.GScore
                                                              + CostEstimate(neighbour.Position, endPosition);
                        }
                    }
                }

                openList = openList.OrderBy(o => o.FScore).ToList();
            }

            var list = new List<Position>();
            return list;
        }

        /// <summary>
        /// Reconstructs a path starting at the given node.
        /// </summary>
        /// <param name="finalNode"> The node to reconstruct the path from. </param>
        /// <returns> The complete path, starting from from the initial position. </returns>
        private static List<Position> ReconstructPath(AStarNode finalNode)
        {
            var path = new List<Position>();
            var currentNode = finalNode;

            while (currentNode.CameFrom != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }

            path.Add(currentNode.Position);
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Gets the estimated cost of traveling directly from one position to another.
        /// </summary>
        /// <param name="start"> The start position. </param>
        /// <param name="end"> The end position. </param>
        /// <returns> The cost estimate. </returns>
        private static double CostEstimate(Position start, Position end)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(start.Item1 - end.Item1), 2) 
                            + Math.Pow(Math.Abs(start.Item2 - end.Item2), 2));
        }

        /// <summary>
        /// Gets the neighbours for the given position.
        /// </summary>
        /// <param name="mapHeightLevels"> The height levels of the map. </param>
        /// <param name="mapItems"> The items in the map. </param>
        /// <param name="position"> The position to find neighbours for. </param>
        /// <returns> A list of the neighbours. </returns>
        private static List<Position> Neighbours(Enums.HeightLevel[,] mapHeightLevels, Enums.Item[,] mapItems, Position position)
        {
            var neighbours = new List<Position>();
            var possibleMoves = new[] { -1, 0, 1 };

            // Iterate over all combinations of neighbours.
            // Don't even bother converting this to LINQ :D
            foreach (var possibleHeightMove in possibleMoves)
            {
                foreach (var possibleWidthMove in possibleMoves)
                {
                    // If we are looking at the current position, don't do anything.
                    if (possibleHeightMove == 0 && possibleWidthMove == 0)
                        continue;

                    var posToCheck = new Position(position.Item1 + possibleHeightMove, position.Item2 + possibleMoves[possibleWidthMove]);

                    // If the move is valid, add it to the list of neighbours.
                    if (ValidMove(mapHeightLevels, mapItems, position, posToCheck))
                        neighbours.Add(posToCheck);
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Determines if moving from one position to another is a valid move.
        /// </summary>
        /// <param name="mapHeightLevels"> The height levels of the map. </param>
        /// <param name="mapItems"> The items in the map. </param>
        /// <param name="fromPos"> The position to travel from. </param>
        /// <param name="toPos">The position to travel to. </param>
        /// <returns> True if the move is valid; false otherwise. </returns>
        private static bool ValidMove(Enums.HeightLevel[,] mapHeightLevels, Enums.Item[,] mapItems, Position fromPos, Position toPos)
        {
            // If not within map, it is not a valid move.
            if (!WithinMapBounds(mapHeightLevels.GetLength(0), mapHeightLevels.GetLength(1), toPos)) return false;

            // If the height difference is 2 or higher, it is not a valid move.
            if (
                Math.Abs(
                    (int)mapHeightLevels[fromPos.Item1, fromPos.Item2] - (int)mapHeightLevels[toPos.Item1, toPos.Item2])
                >= 2) return false;

            // If the positions are not adjacent, it is not a valid move.
            if (Math.Abs(fromPos.Item1 - toPos.Item1) >= 2 || Math.Abs(fromPos.Item2 - toPos.Item2) >= 2) return false;

            // If the target contains a cliff or a Xel'Naga tower, it is not a valid move.
            switch (mapItems[toPos.Item1, toPos.Item2])
            {
                case Enums.Item.Cliff:
                case Enums.Item.XelNagaTower:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a position is within the bounds of the map.
        /// </summary>
        /// <param name="height">Height of the map.</param>
        /// <param name="width">Width of the map.</param>
        /// <param name="position">Position to check.</param>
        /// <returns>True if within the bounds of the map; false otherwise.</returns>
        private static bool WithinMapBounds(int height, int width, Position position)
        {
            if (position.Item1 < 0 || position.Item1 >= height) return false;
            if (position.Item2 < 0 || position.Item2 >= width) return false;
            return true;
        }

        /// <summary>
        /// Checks if the move is valid.
        /// </summary>
        /// <param name="fromPos"> The position to move from. </param>
        /// <param name="toPos"> The position to move to. </param>
        /// <param name="heightDifference">The height difference between from and to. Do NOT use Math.Abs for this, as it
        /// is important if from is higher or lower than to.</param>
        /// <param name="fromItem"> The item at the start position. </param>
        /// <param name="toItem"> The item at the end position. </param>
        /// <returns> True if the move is valid; false otherwise. </returns>
        private static bool ValidMove(Position fromPos, Position toPos, int heightDifference, Enums.Item fromItem, Enums.Item toItem)
        {
            // TODO: Remove is the other method works properly.
            switch (heightDifference)
            {
                // If toPos is 2 higher, it is never a valid move.
                case 3:
                case 2:
                    return false;

                // If toPos is 1 higher, it is only a valid move if fromPos has a ramp that leads in the correct direction.
                case 1:
                    // If height is equal, we are only ever checking for east-west.
                    if (fromPos.Item1 == toPos.Item1 && fromItem == Enums.Item.RampEastWest) return true;

                    // If width is the same, we are only ever checking for north-south.
                    if (fromPos.Item2 == toPos.Item2 && fromItem == Enums.Item.RampNorthSouth) return true;

                    // If both height and width are lower or higher, we are checking for northwest-southeast.
                    if (((fromPos.Item1 < toPos.Item1 && fromPos.Item2 < toPos.Item2) || (fromPos.Item1 > toPos.Item1 && fromPos.Item2 > toPos.Item2))
                        && fromItem == Enums.Item.RampNorthwestSoutheast) return true;

                    // If height is higher and width lower (or the other way around), we are checking for northeast-southwest.
                    if (((fromPos.Item1 < toPos.Item1 && fromPos.Item2 > toPos.Item2) || (fromPos.Item1 > toPos.Item1 && fromPos.Item2 < toPos.Item2))
                        && fromItem == Enums.Item.RampNorthwestSoutheast) return true;

                    return false;

                // If fromPos is 1 higher, it is only a valid move if toPos has a ramp that leads in the correct direction.
                case -1:
                    // If height is equal, we are only ever checking for east-west.
                    if (fromPos.Item1 == toPos.Item1 && toItem == Enums.Item.RampEastWest) return true;

                    // If width is the same, we are only ever checking for north-south.
                    if (fromPos.Item2 == toPos.Item2 && toItem == Enums.Item.RampNorthSouth) return true;

                    // If both height and width are lower or higher, we are checking for northwest-southeast.
                    if (((fromPos.Item1 < toPos.Item1 && fromPos.Item2 < toPos.Item2) || (fromPos.Item1 > toPos.Item1 && fromPos.Item2 > toPos.Item2))
                        && toItem == Enums.Item.RampNorthwestSoutheast) return true;

                    // If height is higher and width lower (or the other way around), we are checking for northeast-southwest.
                    if (((fromPos.Item1 < toPos.Item1 && fromPos.Item2 > toPos.Item2) || (fromPos.Item1 > toPos.Item1 && fromPos.Item2 < toPos.Item2))
                        && toItem == Enums.Item.RampNorthwestSoutheast) return true;

                    return false;

                // If fromPos is 2 or 3 higher, it is never a valid move.
                case -2:
                case -3:
                    return false;

                // If there is no height difference, it is a valid move as long as we do not hit a cliff or the side of a ramp.
                case 0:
                    if (toItem == Enums.Item.Cliff) return false;

                    // If height is equal, we are only ever checking for east-west.
                    if (fromPos.Item1 == toPos.Item1)
                    {
                        // If fromPos is an east-west ramp and toPos isn't (or the other way around), valid move.
                        if (fromItem != Enums.Item.RampEastWest && toItem == Enums.Item.RampEastWest) return true;
                        if (fromItem == Enums.Item.RampEastWest && toItem != Enums.Item.RampEastWest) return true;

                        // If both fromPos and toPos are north-south ramps, they are connected and it is a valid move.
                        if (fromItem == Enums.Item.RampNorthSouth && toItem == Enums.Item.RampNorthSouth) return true;

                        return true;
                    }

                    // If width is the same, we are only ever checking for north-south.
                    if (fromPos.Item2 == toPos.Item2 && toItem == Enums.Item.RampNorthSouth) return true;

                    // If both height and width are lower or higher, we are checking for northwest-southeast.
                    if (((fromPos.Item1 < toPos.Item1 && fromPos.Item2 < toPos.Item2) || (fromPos.Item1 > toPos.Item1 && fromPos.Item2 > toPos.Item2))
                        && toItem == Enums.Item.RampNorthwestSoutheast) return true;

                    // If height is higher and width lower (or the other way around), we are checking for northeast-southwest.
                    if (((fromPos.Item1 < toPos.Item1 && fromPos.Item2 > toPos.Item2) || (fromPos.Item1 > toPos.Item1 && fromPos.Item2 < toPos.Item2))
                        && toItem == Enums.Item.RampNorthwestSoutheast) return true;
                    break;
            }

            return false;
        }

        /// <summary>
        /// The node used in A* pathfinding.
        /// </summary>
        public class AStarNode
        {
            /// <summary>
            /// Gets or sets the position the node belongs to.
            /// </summary>
            public Position Position { get; set; }

            /// <summary>
            /// Gets or sets the node that leads to this node.
            /// </summary>
            public AStarNode CameFrom { get; set; }

            /// <summary>
            /// Gets or sets the g score of the node.
            /// </summary>
            public double GScore { get; set; }

            /// <summary>
            /// Gets or sets the f score of the node.
            /// </summary>
            public double FScore { get; set; }

            /// <summary>
            /// Equality comparator.
            /// </summary>
            /// <param name="obj"> The object to compare to. </param>
            /// <returns> True if the objects are equal; false otherwise. </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((AStarNode)obj);
            }

            /// <summary>
            /// Gets the hashcode of the node.
            /// </summary>
            /// <returns> The hashcode. </returns>
            public override int GetHashCode()
            {
                return this.Position != null ? this.Position.GetHashCode() : 0;
            }

            /// <summary>
            /// Equality comparer
            /// </summary>
            /// <param name="other"> The object to compare to. </param>
            /// <returns> True if their positions are equal; false otherwise. </returns>
            protected bool Equals(AStarNode other)
            {
                return Equals(this.Position, other.Position);
            }
        }
    }
}
