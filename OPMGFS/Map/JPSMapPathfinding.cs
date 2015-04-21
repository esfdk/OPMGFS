namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Position = System.Tuple<int, int>;

    /// <summary>
    /// A class that does pathfinding by using the Jump Point Search algorithm.
    /// Inspired by http://gamedevelopment.tutsplus.com/tutorials/how-to-speed-up-a-pathfinding-with-the-jump-point-search-algorithm--gamedev-5818
    /// </summary>
    public class JPSMapPathfinding
    {
        #region Fields

        /// <summary>
        /// The map that pathfinding is done on.
        /// </summary>
        private readonly Enums.HeightLevel[,] map;

        /// <summary>
        /// A list of the different ways one can go from any position in order to find neighbours.
        /// </summary>
        private readonly List<Position> neighbourDirections = new List<Position> 
                        {
                            new Position(1, 1),
                            new Position(1, -1),
                            new Position(-1, 1),
                            new Position(-1, -1),
                            new Position(0, 1),
                            new Position(0, -1),
                            new Position(1, 0),
                            new Position(-1, 0)
                        };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JPSMapPathfinding"/> class.
        /// </summary>
        /// <param name="mapHeightLevels"> The map height levels. </param>
        public JPSMapPathfinding(Enums.HeightLevel[,] mapHeightLevels)
        {
            // TODO: Add option to include destructible rocks
            this.map = mapHeightLevels;
        }

        #endregion

        #region Properties
        #endregion

        #region Public Methods

        /// <summary>
        /// Finds a path from the given start position to the given end position using Jump Point Search.
        /// </summary>
        /// <param name="startPosition"> The position to start at. </param>
        /// <param name="endPosition"> The position to end at. </param>
        /// <returns> A list representing the path from the start position to the end position. </returns>
        public List<Position> FindPathFromTo(Position startPosition, Position endPosition)
        {
            var openList = new List<Node>();
            var closedList = new List<Node>();

            var startNode = new Node(startPosition);
            var endNode = new Node(endPosition);

            startNode.FScore = this.CostEstimate(startNode.Position, endNode.Position);
            openList.Add(startNode);

            // While there are positions left to check in the open list, do stuff.
            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                openList.RemoveAt(0);

                // If the end node has been found, return the path.
                if (currentNode.Equals(endNode))
                {
                    return this.ReconstructPath(currentNode);
                }

                closedList.Add(currentNode.Clone());

                // Find successors
                var successors = this.IdentifySuccessors(currentNode, endNode);
                var anythingChangedOrAdded = false;

                foreach (var jumpNode in successors)
                {
                    if (closedList.Contains(jumpNode)) continue;

                    var tentativeGScore = currentNode.GScore + this.CostEstimate(currentNode.Position, jumpNode.Position);

                    if (!openList.Contains(jumpNode))
                    {
                        // If the open list does not contain the jump node, add it to the list.
                        jumpNode.Parent = currentNode;
                        jumpNode.GScore = tentativeGScore;
                        jumpNode.FScore = jumpNode.GScore + this.CostEstimate(jumpNode.Position, endNode.Position);

                        openList.Add(jumpNode);
                        anythingChangedOrAdded = true;
                    }
                    else
                    {
                        // If the open list does contain the jump node, update the jump node in the open list.
                        var jumpNodeIndex = openList.IndexOf(jumpNode);
                        if (tentativeGScore >= openList[jumpNodeIndex].GScore) continue;
                        
                        jumpNode.Parent = currentNode;
                        jumpNode.GScore = tentativeGScore;
                        jumpNode.FScore = jumpNode.GScore + this.CostEstimate(jumpNode.Position, endNode.Position);

                        openList[jumpNodeIndex] = jumpNode;
                        anythingChangedOrAdded = true;
                    }
                }

                // If a node was added or updated, sort the list again.
                if (anythingChangedOrAdded)
                    openList = openList.OrderBy(x => x.FScore).ToList();
            }

            return new List<Position>();
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Reconstructs the path that has been traversed.
        /// </summary>
        /// <param name="finalNode"> The node the path ends at. </param>
        /// <returns> A list that represents the path from the starting node to the final node. </returns>
        private List<Position> ReconstructPath(Node finalNode)
        {
            var path = new List<Position>();
            var currentNode = finalNode;
            var nodeToAimFor = currentNode.Parent;

            while (nodeToAimFor != null)
            {
                // The positions
                var cnPos = currentNode.Position;
                var aimPos = nodeToAimFor.Position;

                // The difference between the positions
                var diffX = Math.Abs(aimPos.Item1 - cnPos.Item1);
                var diffY = Math.Abs(aimPos.Item2 - cnPos.Item2);

                // The furthest we have to move
                var longestMove = (diffX >= diffY) ? diffX : diffY;

                // The directions
                var dirX = Math.Min(Math.Max(-1, aimPos.Item1 - cnPos.Item1), 1);
                var dirY = Math.Min(Math.Max(-1, aimPos.Item2 - cnPos.Item2), 1);

                // Adding the positions between the nodes
                for (var i = 0; i < longestMove; i++)
                    path.Add(new Position(cnPos.Item1 + (dirX * i), cnPos.Item2 + (dirY * i)));

                // Updating for next iteration.
                currentNode = nodeToAimFor;
                nodeToAimFor = currentNode.Parent;
            }

            path.Add(currentNode.Position);
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Finds the successors of the current node.
        /// </summary>
        /// <param name="current"> The current node. </param>
        /// <param name="end"> The end node. </param>
        /// <returns> A list of the successors to the current node. </returns>
        private List<Node> IdentifySuccessors(Node current, Node end)
        {
            var successors = new List<Node>();
            var neighbours = this.NodeNeighbours(current);

            foreach (var neighbour in neighbours)
            {
                var dirX = Math.Min(Math.Max(-1, neighbour.Position.Item1 - current.Position.Item1), 1);
                var dirY = Math.Min(Math.Max(-1, neighbour.Position.Item2 - current.Position.Item2), 1);

                var jumpPoint = this.Jump(current.Position, dirX, dirY, end);

                if (jumpPoint != null)
                    successors.Add(jumpPoint);
            }

            return successors;
        }

        /// <summary>
        /// Jumps from the position in the direction given until a blocked position, forced neighbour or
        /// the end node is found.
        /// </summary>
        /// <param name="current"> The position to start the jump from. </param>
        /// <param name="dirX"> The x-direction to jump in. </param>
        /// <param name="dirY"> The y-direction to jump in. </param>
        /// <param name="end"> The end node. </param>
        /// <returns> The first node found that another jump should be made from. </returns>
        private Node Jump(Position current, int dirX, int dirY, Node end)
        {
            var nextX = current.Item1 + dirX;
            var nextY = current.Item2 + dirY;
            var nextNode = new Node(nextX, nextY);

            // If the world is blocked in the given direction, return null.
            if (this.PositionIsBlocked(nextX, nextY, current)) return null;

            // If we have found the target, return the node.
            if (nextNode.Equals(end)) return nextNode;

            var offsetX = nextX;
            var offsetY = nextY;

            // Diagonal movement
            if (dirX != 0 && dirY != 0)
            {
                // Keep running until something is found.
                while (true)
                {
                    // Used for height level comparison in PositionIsBlocked.
                    var tempPos = new Position(offsetX, offsetY);

                    // Check for forced neighbours.
                    if ((!this.PositionIsBlocked(offsetX - dirX, offsetY + dirY, tempPos)
                        && this.PositionIsBlocked(offsetX - dirX, offsetY, tempPos))
                        || (!this.PositionIsBlocked(offsetX + dirX, offsetY - dirY, tempPos)
                        && this.PositionIsBlocked(offsetX, offsetY - dirY, tempPos)))
                    {
                        return new Node(offsetX, offsetY);
                    }

                    // Check if jumping horizontally and diagonally from the next position
                    // finds a forced neighbour or not.
                    if (this.Jump(new Position(offsetX, offsetY), dirX, 0, end) != null
                        || this.Jump(new Position(offsetX, offsetY), 0, dirY, end) != null)
                    {
                        return new Node(offsetX, offsetY);
                    }
                    
                    // If no forced neighbour has been found, move to the next position.
                    offsetX += dirX;
                    offsetY += dirY;

                    // If the new position is blocked or outside the bounds of the world, don't go any further.
                    if (this.PositionIsBlocked(offsetX, offsetY, tempPos)) 
                        return null;

                    // If we have found the goal, return that.
                    if (offsetX == end.Position.Item1 && offsetY == end.Position.Item2)
                    {
                        return new Node(offsetX, offsetY);
                    }
                }
            }

            // Horizontal movement
            if (dirX != 0)
            {
                while (true)
                {
                    // Used for height level comparison in PositionIsBlocked.
                    var tempPos = new Position(offsetX, nextY);

                    // Check for forced neighbours.
                    if ((!this.PositionIsBlocked(offsetX + dirX, nextY + 1, tempPos)
                         && this.PositionIsBlocked(offsetX, nextY + 1, tempPos))
                        || (!this.PositionIsBlocked(offsetX + dirX, nextY - 1, tempPos)
                            && this.PositionIsBlocked(offsetX, nextY - 1, tempPos)))
                    {
                        return new Node(offsetX, nextY);
                    }

                    // If no forced neighbour has been found, move to the next position.
                    offsetX += dirX;

                    // If the new position is blocked or outside the bounds of the world, don't go any further.
                    if (this.PositionIsBlocked(offsetX, nextY, tempPos)) 
                        return null;

                    // If we have found the goal, return that.
                    if (offsetX == end.Position.Item1 && nextY == end.Position.Item2)
                    {
                        return new Node(offsetX, nextY);
                    }
                }
            }

            // Vertical movement
            if (dirY != 0)
            {
                while (true)
                {
                    // Used for height level comparison in PositionIsBlocked.
                    var tempPos = new Position(nextX, offsetY);

                    // Check for forced neighbours.
                    if ((!this.PositionIsBlocked(nextX + 1, offsetY + dirY, tempPos) 
                        && this.PositionIsBlocked(nextX + 1, offsetY, tempPos))
                        || (!this.PositionIsBlocked(nextX - 1, offsetY + dirY, tempPos) 
                            && this.PositionIsBlocked(nextX - 1, offsetY, tempPos)))
                    {
                        return new Node(nextX, offsetY);
                    }

                    // If no forced neighbour has been found, move to the next position.
                    offsetY += dirY;

                    // If the new position is blocked or outside the bounds of the world, don't go any further.
                    if (this.PositionIsBlocked(nextX, offsetY, tempPos)) return null;

                    // If we have found the goal, return that.
                    if (nextX == end.Position.Item1 && offsetY == end.Position.Item2)
                    {
                        return new Node(nextX, offsetY);
                    }
                }
            }

            return this.Jump(nextNode.Position, dirX, dirY, end);
        }

        /// <summary>
        /// Gets a list of the neighbours of the current node, taking into account direction and forced neighbourhood.
        /// </summary>
        /// <param name="current"> The node to find neighbours for. </param>
        /// <returns> A list of the neighbour nodes. </returns>
        private List<Node> NodeNeighbours(Node current)
        {
            var neighbours = new List<Node>();

            // If current is the initial position, just add all neighbours.
            if (current.Parent == null)
            {
                foreach (var pos in this.neighbourDirections)
                {
                    var newX = current.Position.Item1 + pos.Item1;
                    var newY = current.Position.Item2 + pos.Item2;
                    if (!this.PositionIsBlocked(newX, newY, current.Position))
                        neighbours.Add(new Node(newX, newY));
                }

                return neighbours;
            }

            //// ... else, add neighbours depending on direction according to JPS rules

            var curPos = current.Position;
            var curX = curPos.Item1;
            var curY = curPos.Item2;
            var dirX = Math.Min(Math.Max(-1, current.Position.Item1 - current.Parent.Position.Item1), 1);
            var dirY = Math.Min(Math.Max(-1, current.Position.Item2 - current.Parent.Position.Item2), 1);

            if (dirX != 0 && dirY != 0)
            {
                //// Diagonal movement
                //// 3 | . . . .
                //// 2 | 4 1 3 .
                //// 1 | x c 2 .
                //// 0 | p x 5 .
                ////    --------
                ////     0 1 2 3
                
                // 1
                if (!this.PositionIsBlocked(curX, curY + dirY, curPos))
                    neighbours.Add(new Node(curX, curY + dirY));

                // 2
                if (!this.PositionIsBlocked(curX + dirX, curY, curPos))
                    neighbours.Add(new Node(curX + dirX, curY));

                // 3
                if (!this.PositionIsBlocked(curX + dirX, curY + dirY, curPos))
                    neighbours.Add(new Node(curX + dirX, curY + dirY));

                // 4
                if (this.PositionIsBlocked(curX - dirX, curY, curPos)
                    && !this.PositionIsBlocked(curX, curY + dirY, curPos))
                    neighbours.Add(new Node(curX - dirX, curY + dirY));

                // 5
                if (this.PositionIsBlocked(curX, curY - dirY, curPos)
                    && !this.PositionIsBlocked(curX + dirX, curY, curPos))
                    neighbours.Add(new Node(curX - dirX, curY - dirY));
            }
            else
            {
                if (dirX == 0)
                {
                    //// Vertical movement
                    //// 3 | . . . .
                    //// 2 | 3 1 2 .
                    //// 1 | x c x .
                    //// 0 | . p . .
                    ////    --------
                    ////     0 1 2 3

                    if (!this.PositionIsBlocked(curX, curY + dirY, curPos))
                    {
                        // 1
                        neighbours.Add(new Node(curX, curY + dirY));

                        // 2
                        if (this.PositionIsBlocked(curX + 1, curY, curPos))
                            neighbours.Add(new Node(curX + 1, curY + dirY));

                        // 3
                        if (this.PositionIsBlocked(curX - 1, curY, curPos))
                            neighbours.Add(new Node(curX - 1, curY + dirY));
                    }
                    else
                    {
                        if (!this.PositionIsBlocked(curX + 1, curY + dirY, curPos))
                            neighbours.Add(new Node(curX + 1, curY + dirY));

                        if (!this.PositionIsBlocked(curX - 1, curY + dirY, curPos))
                            neighbours.Add(new Node(curX - 1, curY + dirY));
                    }
                }
                else
                {
                    //// Horizontal movement
                    //// 3 | . . . .
                    //// 2 | . x 2 .
                    //// 1 | p c 1 .
                    //// 0 | . x 3 .
                    ////    --------
                    ////     0 1 2 3

                    if (!this.PositionIsBlocked(curX + dirX, curY, curPos))
                    {
                        // 1
                        neighbours.Add(new Node(curX + dirX, curY));

                        // 2
                        if (this.PositionIsBlocked(curX, curY + 1, curPos))
                            neighbours.Add(new Node(curX + dirX, curY + 1));

                        // 3
                        if (this.PositionIsBlocked(curX, curY - 1, curPos))
                            neighbours.Add(new Node(curX + dirX, curY - 1));
                    }
                    else
                    {
                        if (!this.PositionIsBlocked(curX + dirX, curY + 1, curPos))
                            neighbours.Add(new Node(curX + dirX, curY + 1));

                        if (!this.PositionIsBlocked(curX + dirX, curY - 1, curPos))
                            neighbours.Add(new Node(curX + dirX, curY - 1));
                    }
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Gets the estimated cost of traveling directly from one position to another.
        /// </summary>
        /// <param name="start"> The start position. </param>
        /// <param name="end"> The end position. </param>
        /// <returns> The cost estimate. </returns>
        private double CostEstimate(Position start, Position end)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(start.Item1 - end.Item1), 2)
                            + Math.Pow(Math.Abs(start.Item2 - end.Item2), 2));
        }

        /// <summary>
        /// Checks if the given position in the world is blocked (Cliff, Impassable, out of bounds or too much of a height difference).
        /// </summary>
        /// <param name="x"> The x-coordinate to check. </param>
        /// <param name="y"> The y-coordinate to check. </param>
        /// <param name="cameFrom"> The position to compare height difference to. </param>
        /// <returns> True if the position is blocked; false otherwise. </returns>
        private bool PositionIsBlocked(int x, int y, Position cameFrom)
        {
            if (!MapHelper.WithinMapBounds(x, y, this.map.GetLength(0), this.map.GetLength(1))) 
                return true;

            if (this.map[x, y] == Enums.HeightLevel.Cliff || this.map[x, y] == Enums.HeightLevel.Impassable) 
                return true;

            if (Math.Abs((int)this.map[x, y] - (int)this.map[cameFrom.Item1, cameFrom.Item2]) >= 2) 
                return true;

            return false;
        }

        #endregion

        /// <summary>
        /// A class that represents a node in the pathfinding.
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            /// <param name="pos"> The position of the node. </param>
            public Node(Position pos)
            {
                this.Position = pos;
                this.GScore = 0;
                this.FScore = 0;
                this.Parent = null;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            /// <param name="x"> The x-coordinate of the position of the node. </param>
            /// <param name="y"> The y-coordinate of the position of the node. </param>
            public Node(int x, int y)
            {
                this.Position = new Position(x, y);
                this.GScore = 0;
                this.FScore = 0;
                this.Parent = null;
            }

            /// <summary>
            /// Gets the position of the node.
            /// </summary>
            public Position Position { get; private set; }

            /// <summary>
            /// Gets or sets the GScore of the node.
            /// </summary>
            public double GScore { get; set; }

            /// <summary>
            /// Gets or sets the FScore of the node.
            /// </summary>
            public double FScore { get; set; }

            /// <summary>
            /// Gets or sets the parent of the node.
            /// </summary>
            public Node Parent { get; set; }

            /// <summary>
            /// Creates a clone of the node.
            /// </summary>
            /// <returns> A clone of the node. </returns>
            public Node Clone()
            {
                var clonedNode = new Node(Position) { FScore = this.FScore, GScore = this.GScore, Parent = this.Parent };
                return clonedNode;
            }

            /// <summary>
            /// Compares another object to this node to see if they are equal.
            /// </summary>
            /// <param name="obj"> The object to compare to. </param>
            /// <returns> True if they are equal; false otherwise. </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;

                return this.Equals((Node)obj);
            }

            /// <summary>
            /// Gets the hashcode of the node.
            /// </summary>
            /// <returns> The hashcode of the node. </returns>
            public override int GetHashCode()
            {
                return this.Position != null ? this.Position.GetHashCode() : 0;
            }

            /// <summary>
            /// Compares this node to another node to see if they are equal.
            /// </summary>
            /// <param name="other"> The node to compare to. </param>
            /// <returns> True if they are equal; false otherwise. </returns>
            [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
            private bool Equals(Node other)
            {
                return Equals(this.Position, other.Position);
            }
        }
    }
}
