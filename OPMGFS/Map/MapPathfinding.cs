﻿namespace OPMGFS.Map
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
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the shortest path on a map using A*.
        /// </summary>
        /// <param name="mapHeightLevels"> The height levels of the map. </param>
        /// <param name="startPosition"> The position to start the pathfinding from. </param>
        /// <param name="endPosition"> The position to find a path to. </param>
        /// <returns> A list of the positions that make up the path. </returns>
        public static List<Position> FindPathFromTo(Enums.HeightLevel[,] mapHeightLevels, Position startPosition, Position endPosition)
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
                var neighbours = Neighbours(mapHeightLevels, current.Position);
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
        /// Gets the neighbours for the given position.
        /// </summary>
        /// <param name="mapHeightLevels"> The height levels of the map. </param>
        /// <param name="position"> The position to find neighbours for. </param>
        /// <returns> A list of the neighbours. </returns>
        public static IEnumerable<Position> Neighbours(Enums.HeightLevel[,] mapHeightLevels, Position position)
        {
            var neighbours = new List<Position>();
            var possibleMoves = new[] { -1, 0, 1 };

            // Iterate over all combinations of neighbours.
            // Don't even bother converting this to LINQ :D
            foreach (var possibleXMove in possibleMoves)
            {
                foreach (var possibleYMove in possibleMoves)
                {
                    // If we are looking at the current position, don't do anything.
                    if (possibleXMove == 0 && possibleYMove == 0)
                        continue;

                    var posToCheck = new Position(position.Item1 + possibleXMove, position.Item2 + possibleYMove);

                    // If the move is valid, add it to the list of neighbours.
                    if (MapHelper.ValidMove(mapHeightLevels, position, posToCheck))
                        neighbours.Add(posToCheck);
                }
            }

            return neighbours;
        }
        #endregion

        #region Private Methods
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
        #endregion

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

                return this.Equals((AStarNode)obj);
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
                return object.Equals(this.Position, other.Position);
            }
        }
    }
}
