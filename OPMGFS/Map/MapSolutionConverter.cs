namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class for converting map solutions to phenotypes.
    /// </summary>
    public static class MapSolutionConverter
    {
        /// <summary>
        /// Finds the nearest tile of a specific type in the items of the map phenotype.
        /// </summary>
        /// <param name="x"> The starting x-coordinate. </param>
        /// <param name="y"> The starting y-coordinate. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <param name="goal"> The goal item. </param>
        /// <returns> The <see cref="Tuple"/> of coordinates (item 1 is x-coordinate, item 2 is y-coordinate, item 3 is distance from starting point).</returns>
        public static Tuple<int, int, int> FindNearestItemTileOfType(int x, int y, MapPhenotype mp, Enums.Item goal)
        {
            var queue = new List<Tuple<int, int, int>>();
            var discovered = new List<Tuple<int, int, int>>();

            var v = new Tuple<int, int, int>(x, y, 0);

            queue.Add(v);

            while (queue.Count != 0)
            {
                v = queue[0];
                queue.RemoveAt(0);

                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var w = new Tuple<int, int, int>(v.Item1 + i, v.Item2 + j, v.Item3 + 1);

                        if (!mp.InsideTopHalf(w.Item1, w.Item2))
                        {
                            continue;
                        }

                        if (mp.MapItems[w.Item1, w.Item2] == goal)
                        {
                            return w;
                        }

                        if (discovered.Any(t => (t.Item1 == w.Item1) && (t.Item2 == w.Item2)))
                        {
                            continue;
                        }

                        queue.Add(w);
                        discovered.Add(w);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the nearest tile of a specific heightlevel in the height levels of the map phenotype.
        /// </summary>
        /// <param name="x"> The starting x-coordinate. </param>
        /// <param name="y"> The starting y-coordinate. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <param name="goal"> The goal heightlevel. </param>
        /// <returns> The <see cref="Tuple"/> of coordinates (item 1 is x-coordinate, item 2 is y-coordinate, item 3 is distance from starting point).</returns>
        public static Tuple<int, int, int> FindNearestHeightTileOfType(int x, int y, MapPhenotype mp, Enums.HeightLevel goal)
        {
            var queue = new List<Tuple<int, int, int>>();
            var discovered = new List<Tuple<int, int, int>>();

            var v = new Tuple<int, int, int>(x, y, 0);

            queue.Add(v);

            while (queue.Count != 0)
            {
                v = queue[0];
                queue.RemoveAt(0);

                for (var i = -1; i <= 1; i++)
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        var w = new Tuple<int, int, int>(v.Item1 + i, v.Item2 + j, v.Item3 + 1);

                        if (!mp.InsideTopHalf(w.Item1, w.Item2))
                        {
                            continue;
                        }

                        if (mp.HeightLevels[w.Item1, w.Item2] == goal)
                        {
                            return w;
                        }

                        if (discovered.Any(t => (t.Item1 == w.Item1) && (t.Item2 == w.Item2)))
                        {
                            continue;
                        }

                        queue.Add(w);
                        discovered.Add(w);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to place a ramp on a map phenotype.
        /// </summary>
        /// <param name="x"> The x-coordinate to attempt to use as a starting point. </param>
        /// <param name="y"> The y-coordinate to attempt to use as a starting point. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns> The <see cref="bool"/> indicating whether placement was successful. </returns>
        public static bool PlaceRamp(int x, int y, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(x, y)) return false;

            // Horizontal
            if (mp.InsideTopHalf(x + 1, y + 1) && mp.HeightLevels[x + 1, y] == Enums.HeightLevel.Cliff)
            {
                if (PlaceHorizontalRamp(x, y, mp))
                {
                    return true;
                }
            }
            else if (mp.InsideTopHalf(x - 1, y + 1) && mp.HeightLevels[x - 1, y] == Enums.HeightLevel.Cliff)
            {
                if (PlaceHorizontalRamp(x - 1, y, mp))
                {
                    return true;
                }
            }

            if (mp.InsideTopHalf(x + 1, y + 1) && mp.HeightLevels[x, y + 1] == Enums.HeightLevel.Cliff)
            {
                if (PlaceVerticalRamp(x, y, mp))
                {
                    return true;
                }
            }
            else if (mp.InsideTopHalf(x + 1, y - 1) && mp.HeightLevels[x, y - 1] == Enums.HeightLevel.Cliff)
            {
                if (PlaceVerticalRamp(x, y - 1, mp))
                {
                    return true;
                }
            }

            // Northwest-Southeast diagonal
            // TODO: Diagonal ramps
            // Northeast-Southwest diagonal
            // TODO: Diagonal ramps
            return false;
        }

        /// <summary>
        /// Places destructible debris taking up 2x2 spaces on the given map. The debris will not be placed if it is put outside bounds. It will also not overwrite any bases/minerals.
        /// </summary>
        /// <param name="x"> The x-location to attempt to place the debris. </param>
        /// <param name="y"> The y-location to attempt to place the debris. </param>
        /// <param name="mp"> The map phenotype to place the debris on. </param>
        /// <returns>True if the rocks were placed, false if not</returns>
        public static bool PlaceDestructibleRocks(int x, int y, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(x, y))
            {
                return false;
            }

            // HACK: Removed check for occupied tiles (risks being placed on inside a starting position)
            if (mp.InsideTopHalf(x + 1, y) && mp.InsideTopHalf(x, y + 1) && mp.InsideTopHalf(x + 1, y + 1))
            {
                // Bottom-left
                if (mp.HeightLevels[x, y] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable 
                    && mp.MapItems[x, y] != Enums.Item.StartBase && mp.MapItems[x, y] != Enums.Item.XelNagaTower
                    && mp.MapItems[x, y] != Enums.Item.BlueMinerals && mp.MapItems[x, y] != Enums.Item.GoldMinerals
                    && mp.MapItems[x, y] != Enums.Item.Gas)
                {
                    mp.DestructibleRocks[x, y] = true;
                    mp.DestructibleRocks[x + 1, y] = true;
                    mp.DestructibleRocks[x, y + 1] = true;
                    mp.DestructibleRocks[x + 1, y + 1] = true;
                }
            }
            else if (mp.InsideTopHalf(x - 1, y) && mp.InsideTopHalf(x, y + 1) && mp.InsideTopHalf(x - 1, y + 1))
            {
                // Bottom-right
                if (mp.HeightLevels[x - 1, y] != Enums.HeightLevel.Cliff && mp.HeightLevels[x - 1, y] != Enums.HeightLevel.Impassable 
                    && mp.MapItems[x - 1, y] != Enums.Item.StartBase && mp.MapItems[x - 1, y] != Enums.Item.XelNagaTower
                    && mp.MapItems[x - 1, y] != Enums.Item.BlueMinerals && mp.MapItems[x - 1, y] != Enums.Item.GoldMinerals
                    && mp.MapItems[x - 1, y] != Enums.Item.Gas)
                {
                    mp.DestructibleRocks[x, y] = true;
                    mp.DestructibleRocks[x - 1, y] = true;
                    mp.DestructibleRocks[x, y + 1] = true;
                    mp.DestructibleRocks[x - 1, y + 1] = true;
                }
            }
            else if (mp.InsideTopHalf(x + 1, y) && mp.InsideTopHalf(x, y - 1) && mp.InsideTopHalf(x + 1, y - 1))
            {
                // Top-left
                if (mp.HeightLevels[x, y - 1] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y - 1] != Enums.HeightLevel.Impassable
                    && mp.MapItems[x, y - 1] != Enums.Item.StartBase && mp.MapItems[x, y - 1] != Enums.Item.XelNagaTower
                    && mp.MapItems[x, y - 1] != Enums.Item.BlueMinerals && mp.MapItems[x, y - 1] != Enums.Item.GoldMinerals
                    && mp.MapItems[x, y - 1] != Enums.Item.Gas)
                {
                    mp.DestructibleRocks[x, y] = true;
                    mp.DestructibleRocks[x + 1, y] = true;
                    mp.DestructibleRocks[x, y - 1] = true;
                    mp.DestructibleRocks[x + 1, y - 1] = true;
                }
            }
            else if (mp.InsideTopHalf(x - 1, y) && mp.InsideTopHalf(x, y - 1) && mp.InsideTopHalf(x - 1, y - 1))
            {
                // Top-right
                if (mp.HeightLevels[x - 1, y - 1] != Enums.HeightLevel.Cliff && mp.HeightLevels[x - 1, y - 1] != Enums.HeightLevel.Impassable
                    && mp.MapItems[x - 1, y - 1] != Enums.Item.StartBase && mp.MapItems[x - 1, y - 1] != Enums.Item.XelNagaTower
                    && mp.MapItems[x - 1, y - 1] != Enums.Item.BlueMinerals && mp.MapItems[x - 1, y - 1] != Enums.Item.GoldMinerals
                    && mp.MapItems[x - 1, y - 1] != Enums.Item.Gas)
                {
                    mp.DestructibleRocks[x, y] = true;
                    mp.DestructibleRocks[x - 1, y] = true;
                    mp.DestructibleRocks[x, y - 1] = true;
                    mp.DestructibleRocks[x - 1, y - 1] = true;
                }
            }

            return true;
        }

        /// <summary>
        /// The find closest cliff.
        /// </summary>
        /// <param name="startX"> The starting x-coordinate. </param>
        /// <param name="startY"> The starting y-coordinate. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns>The <see cref="Tuple"/> of coordinates (item 1 is x-coordinate, item 2 is y-coordinate, item 3 is distance from starting point).</returns>
        public static Tuple<int, int, int> FindClosestCliff(int startX, int startY, MapPhenotype mp)
        {
            return FindNearestHeightTileOfType(startX, startY, mp, Enums.HeightLevel.Cliff);
        }

        /// <summary>
        /// Places a starting base (24x24) around the given coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="mp">The map phenotype to place the base in.</param>
        /// <returns>True if placement was successful, false if not.</returns>
        public static bool PlaceStartBase(int x, int y, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(x, y))
            {
                return false;
            }

            if (!mp.InsideTopHalf(x - 11, y - 11) || !mp.InsideTopHalf(x - 11, y + 12) || !mp.InsideTopHalf(x + 12, y - 11) ||
                !mp.InsideTopHalf(x + 12, y + 12))
            {
                return false;
            }

            if (IsAreaOccupied(x - 11, y - 11, 24, 24, mp))
            {
                return false;
            }

            var height = mp.HeightLevels[x, y];

            if (height == Enums.HeightLevel.Cliff || height == Enums.HeightLevel.Impassable)
            {
                var neighbours = MapHelper.GetNeighbours(x, y, mp.HeightLevels);

                height = (neighbours[Enums.HeightLevel.Height2] >= neighbours[Enums.HeightLevel.Height1]
                             && neighbours[Enums.HeightLevel.Height2] >= neighbours[Enums.HeightLevel.Height0])
                                ? Enums.HeightLevel.Height2
                                : neighbours[Enums.HeightLevel.Height1] >= neighbours[Enums.HeightLevel.Height0]
                                      ? Enums.HeightLevel.Height1
                                      : Enums.HeightLevel.Height0;
            }

            FlattenArea(height, x - 11, y - 11, 24, 24, mp);
            OccupyArea(x - 11, y - 11, 24, 24, mp);

            for (var sbx = x - 2; sbx < (x - 2 + 5); sbx++)
            {
                for (var sby = y - 2; sby < (y - 2 + 5); sby++)
                {
                    mp.MapItems[sbx, sby] = Enums.Item.StartBase;
                }
            }

            // Minerals
            PlaceMinerals(x - 2, y + 7, mp);
            PlaceMinerals(x, y + 6, mp);
            PlaceMinerals(x - 3, y + 6, mp);
            PlaceMinerals(x - 6, y + 5, mp);
            PlaceMinerals(x - 7, y + 4, mp);
            PlaceMinerals(x - 7, y + 2, mp);
            PlaceMinerals(x - 8, y + 1, mp);
            PlaceMinerals(x - 7, y - 1, mp);

            // Gas
            PlaceGas(x - 8, y - 5, mp);
            PlaceGas(x + 3, y + 6, mp);

            return true;
        }

        /// <summary>
        /// Places a Xel'Naga Tower taking up 2x2 spaces on the given map. The tower will not be placed if it is put outside bounds. It will also not overwrite any bases/minerals.
        /// </summary>
        /// <param name="x">The y-location to attempt to place the Xel'Naga tower.</param>
        /// <param name="y">The x-location to attempt to place the Xel'Naga tower.</param>
        /// <param name="mp">The map phenotype to place the tower on.</param>
        /// <returns>True if placement was successful, false if not.</returns>
        public static bool PlaceXelNagaTower(int x, int y, MapPhenotype mp)
        {
            return PlaceTwoByTwo(x, y, mp, Enums.Item.XelNagaTower);
        }

        /// <summary>
        /// Places a base (16x16) around a given location.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="mp"> The map phenotype to place the base in. </param>
        /// <param name="isGoldBase"> Whether this is a gold base or not. </param>
        /// <returns>True if placement was successful, false if not.</returns>
        public static bool PlaceBase(int x, int y, MapPhenotype mp, bool isGoldBase = false)
        {
            if (!mp.InsideTopHalf(x, y))
            {
                return false;
            }

            if (!mp.InsideTopHalf(x - 7, y - 7) || !mp.InsideTopHalf(x - 7, y + 8) || !mp.InsideTopHalf(x + 8, y - 7) ||
                !mp.InsideTopHalf(x + 8, y + 8))
            {
                return false;
            }

            if (IsAreaOccupied(x - 7, y - 7, 16, 16, mp))
            {
                return false;
            }

            var height = mp.HeightLevels[x, y];

            if (height == Enums.HeightLevel.Cliff || height == Enums.HeightLevel.Impassable)
            {
                var neighbours = MapHelper.GetNeighbours(x, y, mp.HeightLevels);

                height = (neighbours[Enums.HeightLevel.Height2] >= neighbours[Enums.HeightLevel.Height1]
                             && neighbours[Enums.HeightLevel.Height2] >= neighbours[Enums.HeightLevel.Height0])
                                ? Enums.HeightLevel.Height2
                                : neighbours[Enums.HeightLevel.Height1] >= neighbours[Enums.HeightLevel.Height0]
                                      ? Enums.HeightLevel.Height1
                                      : Enums.HeightLevel.Height0;
            }

            FlattenArea(height, x - 7, y - 7, 16, 16, mp);
            OccupyArea(x - 7, y - 7, 16, 16, mp);

            var rx = x - 1;
            var ry = y - 3;

            for (var sbx = rx; sbx < (rx + 5); sbx++)
            {
                for (var sby = ry; sby < (ry + 5); sby++)
                {
                    mp.MapItems[sbx, sby] = Enums.Item.Base;
                }
            }

            // Minerals
            PlaceMinerals(rx, ry + 9, mp, isGoldBase);
            PlaceMinerals(rx - 1, ry + 8, mp, isGoldBase);
            PlaceMinerals(rx + 2, ry + 8, mp, isGoldBase);
            PlaceMinerals(rx - 4, ry + 7, mp, isGoldBase);
            PlaceMinerals(rx - 5, ry + 6, mp, isGoldBase);
            PlaceMinerals(rx - 5, ry + 4, mp, isGoldBase);
            PlaceMinerals(rx - 6, ry + 3, mp, isGoldBase);
            PlaceMinerals(rx - 5, ry + 1, mp, isGoldBase);

            // Gas
            PlaceGas(rx - 6, ry - 3, mp);
            PlaceGas(rx + 5, ry + 8, mp);

            return true;
        }

        /// <summary>
        /// Places a 2x2 tile object (such as destructible debris and Xel'Naga towers).
        /// </summary>
        /// <param name="x">The starting x-coordinate.</param>
        /// <param name="y">The starting y-coordinate.</param>
        /// <param name="mp">The map phenotype to place the object on.</param>
        /// <param name="itemToPlace">The item to place on the map.</param>
        /// <returns>True if placement was successful, false if not.</returns>
        private static bool PlaceTwoByTwo(int x, int y, MapPhenotype mp, Enums.Item itemToPlace)
        {
            if (!mp.InsideTopHalf(x, y))
            {
                return false;
            }

            if (IsAreaOccupied(x, y, 2, 2, mp))
            {
                return false;
            }

            if (mp.InsideTopHalf(x + 1, y) && mp.InsideTopHalf(x, y + 1) && mp.InsideTopHalf(x + 1, y + 1))
            {
                // Bottom-left
                if (mp.HeightLevels[x, y] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x, y], x, y, 2, 2, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x + 1, y] = itemToPlace;
                    mp.MapItems[x, y + 1] = itemToPlace;
                    mp.MapItems[x + 1, y + 1] = itemToPlace;
                }
            }
            else if (mp.InsideTopHalf(x - 1, y) && mp.InsideTopHalf(x, y + 1) && mp.InsideTopHalf(x - 1, y + 1))
            {
                // Bottom-right
                if (mp.HeightLevels[x - 1, y] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x - 1, y], x, y, 2, 2, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x - 1, y] = itemToPlace;
                    mp.MapItems[x, y + 1] = itemToPlace;
                    mp.MapItems[x - 1, y + 1] = itemToPlace;
                }
            }
            else if (mp.InsideTopHalf(x + 1, y) && mp.InsideTopHalf(x, y - 1) && mp.InsideTopHalf(x + 1, y - 1))
            {
                // Top-left
                if (mp.HeightLevels[x, y - 1] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x, y - 1], x, y, 2, 2, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x + 1, y] = itemToPlace;
                    mp.MapItems[x, y - 1] = itemToPlace;
                    mp.MapItems[x + 1, y - 1] = itemToPlace;
                }
            }
            else if (mp.InsideTopHalf(x - 1, y) && mp.InsideTopHalf(x, y - 1) && mp.InsideTopHalf(x - 1, y - 1))
            {
                // Top-right
                if (mp.HeightLevels[x - 1, y - 1] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x - 1, y - 1], x, y, 2, 2, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x - 1, y] = itemToPlace;
                    mp.MapItems[x, y - 1] = itemToPlace;
                    mp.MapItems[x - 1, y - 1] = itemToPlace;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if an area is occupied.
        /// </summary>
        /// <param name="startX">Start coordinate on the X-axis.</param>
        /// <param name="startY">Start coordinate on the Y-axis.</param>
        /// <param name="lengthX">Length on the X-axis.</param>
        /// <param name="lengthY">Length on the Y-axis.</param>
        /// <param name="mp">The map to check on.</param>
        /// <returns>True if area is occupied or is outside bounds, false if not.</returns>
        private static bool IsAreaOccupied(int startX, int startY, int lengthX, int lengthY, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(startX, startY) || !mp.InsideTopHalf(startX + lengthX, startY + lengthY))
            {
                return true;
            }

            for (var i = startX; i < startX + lengthX; i++)
            {
                for (var j = startY; j < startY + lengthY; j++)
                {
                    if (mp.MapItems[i, j] != Enums.Item.None) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Places minerals at x,y and x+1,y positions on a map.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="mp"> The map phenotype to place minerals in. </param>
        /// <param name="isGold"> Whether this is a gold mineral or not. </param>
        private static void PlaceMinerals(int x, int y, MapPhenotype mp, bool isGold = false)
        {
            if (!mp.InsideTopHalf(x, y) || !mp.InsideTopHalf(x + 1, y))
            {
                return;
            }

            FlattenArea(mp.HeightLevels[x, y], x, y, 2, 1, mp);

            mp.MapItems[x, y] = isGold ? Enums.Item.GoldMinerals : Enums.Item.BlueMinerals;
            mp.MapItems[x + 1, y] = isGold ? Enums.Item.GoldMinerals : Enums.Item.BlueMinerals;
        }

        /// <summary>
        /// Places gas in a 3x3 square using x,y as the bottom-left tile.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="mp"> The map phenotype to place gas in. </param>
        private static void PlaceGas(int x, int y, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(x, y) || !mp.InsideTopHalf(x + 2, y) || !mp.InsideTopHalf(x, y + 2) || !mp.InsideTopHalf(x + 2, y + 2))
            {
                return;
            }

            FlattenArea(mp.HeightLevels[x, y], x, y, 3, 3, mp);

            for (var i = x; i < x + 3; i++)
            {
                for (var j = y; j < y + 3; j++)
                {
                    mp.MapItems[i, j] = Enums.Item.Gas;
                }
            }
        }

        /// <summary>
        /// Flattens a square area of the map to a specific height.
        /// </summary>
        /// <param name="height">The height to flatten to.</param>
        /// <param name="startX">X-coordinate of the bottom-left corner.</param>
        /// <param name="startY">Y-coordinate of the bottom-left corner.</param>
        /// <param name="lengthX">The length on the x-axis.</param>
        /// <param name="lengthY">The length on the y-axis.</param>
        /// <param name="mp">The map phenotype to flatten the area on.</param>
        private static void FlattenArea(Enums.HeightLevel height, int startX, int startY, int lengthX, int lengthY, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(startX, startY) || !mp.InsideTopHalf(startX + lengthX, startY + lengthY))
            {
                return;
            }

            for (var i = startX; i < startX + lengthX; i++)
            {
                for (var j = startY; j < startY + lengthY; j++)
                {
                    mp.HeightLevels[i, j] = height;
                }
            }
        }

        /// <summary>
        /// Blocks an area for further use.
        /// </summary>
        /// <param name="startX">Start coordinate on the X-axis.</param>
        /// <param name="startY">Start coordinate on the Y-axis.</param>
        /// <param name="lengthX">Length on the X-axis.</param>
        /// <param name="lengthY">Length on the Y-axis.</param>
        /// <param name="mp">The map to occupy tiles on.</param>
        private static void OccupyArea(int startX, int startY, int lengthX, int lengthY, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(startX, startY) || !mp.InsideTopHalf(startX + lengthX, startY + lengthY))
            {
                return;
            }

            for (var i = startX; i < startX + lengthX; i++)
            {
                for (var j = startY; j < startY + lengthY; j++)
                {
                    mp.MapItems[i, j] = Enums.Item.Occupied;
                }
            }
        }

        /// <summary>
        /// Attempts to place a ramp over a vertical patch of cliff.
        /// </summary>
        /// <param name="x"> The x-coordinate to place the ramp at. </param>
        /// <param name="y"> The y-coordinate to place the ramp at. </param>
        /// <param name="west"> The western ramp type. </param>
        /// <param name="east"> The eastern ramp type. </param>
        /// <param name="ramp"> The type of the ramp. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns> The <see cref="bool"/> indicating whether placement was successful. </returns>
        private static bool PlaceVerticalRamp(int x, int y, Enums.HeightLevel west, Enums.HeightLevel east, Enums.HeightLevel ramp, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(x - 1, y) || !mp.InsideTopHalf(x - 1, y + 1) || !mp.InsideTopHalf(x + 1, y)
                || !mp.InsideTopHalf(x + 1, y + 1))
            {
                return false;
            }

            if (mp.HeightLevels[x - 1, y] == west && mp.HeightLevels[x - 1, y + 1] == west
                && mp.HeightLevels[x + 1, y] == east && mp.HeightLevels[x + 1, y + 1] == east)
            {
                if (mp.InsideTopHalf(x + 2, y) || mp.InsideTopHalf(x + 2, y + 1))
                {
                    if (!IsAreaOccupied(x - 1, y - 1, 4, 4, mp))
                    {
                        if (mp.HeightLevels[x + 2, y] == east
                            && mp.HeightLevels[x + 2, y + 1] == east)
                        {
                            OccupyArea(x - 1, y - 1, 4, 4, mp);

                            mp.HeightLevels[x - 1, y] = ramp;
                            mp.HeightLevels[x - 1, y + 1] = ramp;

                            mp.HeightLevels[x, y] = ramp;
                            mp.HeightLevels[x, y + 1] = ramp;
                            mp.HeightLevels[x, y - 1] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x, y + 2] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x + 1, y] = ramp;
                            mp.HeightLevels[x + 1, y + 1] = ramp;
                            mp.HeightLevels[x + 1, y - 1] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x + 1, y + 2] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x + 2, y] = ramp;
                            mp.HeightLevels[x + 2, y + 1] = ramp;

                            return true;
                        }
                    }
                }

                if (mp.InsideTopHalf(x - 2, y) || mp.InsideTopHalf(x - 2, y + 1))
                {
                    if (!IsAreaOccupied(x - 2, y - 1, 4, 4, mp))
                    {
                        if (mp.HeightLevels[x - 2, y] == west
                            && mp.HeightLevels[x - 2, y + 1] == west)
                        {
                            OccupyArea(x - 2, y - 1, 4, 4, mp);

                            mp.HeightLevels[x - 2, y] = ramp;
                            mp.HeightLevels[x - 2, y + 1] = ramp;

                            mp.HeightLevels[x - 1, y] = ramp;
                            mp.HeightLevels[x - 1, y + 1] = ramp;
                            mp.HeightLevels[x - 1, y - 1] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x - 1, y + 2] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x, y] = ramp;
                            mp.HeightLevels[x, y + 1] = ramp;
                            mp.HeightLevels[x, y - 1] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x, y + 2] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x + 1, y] = ramp;
                            mp.HeightLevels[x + 1, y + 1] = ramp;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to place a ramp over a vertical patch of cliff.
        /// </summary>
        /// <param name="x"> The x-coordinate to place the ramp at. </param>
        /// <param name="y"> The y-coordinate to place the ramp at. </param>
        /// <param name="north"> The northern ramp type. </param>
        /// <param name="south"> The southern ramp type. </param>
        /// <param name="ramp"> The type of the ramp. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns> The <see cref="bool"/> indicating whether placement was successful. </returns>
        private static bool PlaceHorizontalRamp(int x, int y, Enums.HeightLevel north, Enums.HeightLevel south, Enums.HeightLevel ramp, MapPhenotype mp)
        {
            if (!mp.InsideTopHalf(x, y + 1) || !mp.InsideTopHalf(x + 1, y + 1) || !mp.InsideTopHalf(x, y - 1) || !mp.InsideTopHalf(x + 1, y - 1))
            {
                return false;
            }

            if (mp.HeightLevels[x, y + 1] == north && mp.HeightLevels[x + 1, y + 1] == north
                && mp.HeightLevels[x, y - 1] == south && mp.HeightLevels[x + 1, y - 1] == south)
            {
                if (mp.InsideTopHalf(x, y - 2) || mp.InsideTopHalf(x + 1, y - 2))
                {
                    if (!IsAreaOccupied(x - 1, y - 2, 4, 4, mp))
                    {
                        if (mp.HeightLevels[x, y - 2] == south
                            && mp.HeightLevels[x + 1, y - 2] == south)
                        {
                            OccupyArea(x - 1, y - 2, 4, 4, mp);

                            mp.HeightLevels[x, y + 1] = ramp;
                            mp.HeightLevels[x + 1, y + 1] = ramp;

                            mp.HeightLevels[x, y] = ramp;
                            mp.HeightLevels[x + 1, y] = ramp;
                            mp.HeightLevels[x - 1, y] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x + 2, y] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x, y - 1] = ramp;
                            mp.HeightLevels[x + 1, y - 1] = ramp;
                            mp.HeightLevels[x - 1, y - 1] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x + 2, y - 1] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x, y - 2] = ramp;
                            mp.HeightLevels[x + 1, y - 2] = ramp;

                            return true;
                        }
                    }
                }

                if (mp.InsideTopHalf(x, y + 2) || mp.InsideTopHalf(x + 1, y + 2))
                {
                    if (!IsAreaOccupied(x - 1, y - 1, 4, 4, mp))
                    {
                        if (mp.HeightLevels[x, y + 2] == north
                            && mp.HeightLevels[x + 1, y + 2] == north)
                        {
                            OccupyArea(x - 1, y - 1, 4, 4, mp);

                            mp.HeightLevels[x, y + 2] = ramp;
                            mp.HeightLevels[x + 1, y + 2] = ramp;

                            mp.HeightLevels[x, y + 1] = ramp;
                            mp.HeightLevels[x + 1, y + 1] = ramp;
                            mp.HeightLevels[x - 1, y + 1] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x + 2, y + 1] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x, y] = ramp;
                            mp.HeightLevels[x + 1, y] = ramp;
                            mp.HeightLevels[x - 1, y] = Enums.HeightLevel.Cliff;
                            mp.HeightLevels[x + 2, y] = Enums.HeightLevel.Cliff;

                            mp.HeightLevels[x, y - 1] = ramp;
                            mp.HeightLevels[x + 1, y - 1] = ramp;

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to place a ramp on a vertical cliff.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns> The <see cref="bool"/> indicating whether placement was successful. </returns>
        private static bool PlaceVerticalRamp(int x, int y, MapPhenotype mp)
        {
            if ((!mp.InsideTopHalf(x + 1, y) || !mp.InsideTopHalf(x + 1, y + 1))
                    && (!mp.InsideTopHalf(x - 1, y) || !mp.InsideTopHalf(x - 1, y + 1)))
            {
                return false;
            }

            if (PlaceVerticalRamp(
                x,
                y,
                Enums.HeightLevel.Height0,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Ramp01,
                mp))
            {
                return true;
            }

            if (PlaceVerticalRamp(
                x,
                y,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Height0,
                Enums.HeightLevel.Ramp01,
                mp))
            {
                return true;
            }

            if (PlaceVerticalRamp(
                x,
                y,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Height2,
                Enums.HeightLevel.Ramp12,
                mp))
            {
                return true;
            }

            if (PlaceVerticalRamp(
                x,
                y,
                Enums.HeightLevel.Height2,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Ramp12,
                mp))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The place horizontal ramp.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns> The <see cref="bool"/> indicating whether placement was successful.  </returns>
        private static bool PlaceHorizontalRamp(int x, int y, MapPhenotype mp)
        {
            if ((!mp.InsideTopHalf(x, y + 1) || !mp.InsideTopHalf(x + 1, y + 1))
                    && (!mp.InsideTopHalf(x, y - 1) || !mp.InsideTopHalf(x + 1, y - 1)))
            {
                return false;
            }

            if (PlaceHorizontalRamp(
                x,
                y,
                Enums.HeightLevel.Height0,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Ramp01,
                mp))
            {
                return true;
            }

            if (PlaceHorizontalRamp(
                x,
                y,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Height0,
                Enums.HeightLevel.Ramp01,
                mp))
            {
                return true;
            }

            if (PlaceHorizontalRamp(
                x,
                y,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Height2,
                Enums.HeightLevel.Ramp12,
                mp))
            {
                return true;
            }

            if (PlaceHorizontalRamp(
                x,
                y,
                Enums.HeightLevel.Height2,
                Enums.HeightLevel.Height1,
                Enums.HeightLevel.Ramp12,
                mp))
            {
                return true;
            }

            return false;
        }
    }
}
