namespace OPMGFS.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OPMGFS.Map.MapObjects;

    /// <summary>
    /// Helper class for converting map solutions to phenotypes.
    /// </summary>
    public static class MapConversionHelper
    {
        #region Public Methods
        /// <summary>
        /// Mutates a list of map points.
        /// </summary>
        /// <param name="mapPoints">The list of map points to mutate.</param>
        /// <param name="mutationChance">The chance that a mutation will happen.</param>
        /// <param name="mso">The map search options.</param>
        /// <param name="r">The random option to use.</param>
        /// <returns>The mutated list of map points.</returns>
        public static List<MapPoint> MutateMapPoints(List<MapPoint> mapPoints, double mutationChance, MapSearchOptions mso, Random r)
        {
            var newPoints = mapPoints.Select(mp => r.NextDouble() < mutationChance ? mp.Mutate(r, mso) : mp).ToList();

            var randomNumber = r.NextDouble();

            if (randomNumber < mso.ChanceToAddNewElement)
            {
                if (r.NextDouble() < mso.ChanceToAddBase)
                {
                    if (mso.MaximumNumberOfBases > newPoints.Count(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.Base,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else if (randomNumber < mso.ChanceToAddBase + mso.ChanceToAddGoldBase)
                {
                    if (mso.MaximumNumberOfBases > newPoints.Count(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.GoldBase,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else if (randomNumber < mso.ChanceToAddBase + mso.ChanceToAddGoldBase + mso.ChanceToAddXelNagaTower)
                {
                    if (mso.MaximumNumberOfXelNagaTowers
                        > newPoints.Count(mp => mp.Type == Enums.MapPointType.XelNagaTower))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.XelNagaTower,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else if (randomNumber
                         < mso.ChanceToAddBase + mso.ChanceToAddGoldBase
                         + mso.ChanceToAddXelNagaTower + mso.ChanceToAddDestructibleRocks)
                {
                    if (mso.MaximumNumberOfDestructibleRocks
                        > newPoints.Count(mp => mp.Type == Enums.MapPointType.DestructibleRocks))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.DestructibleRocks,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
                else
                {
                    if (mso.MaximumNumberOfRamps
                        > newPoints.Count(mp => mp.Type == Enums.MapPointType.Ramp))
                    {
                        newPoints.Add(
                            new MapPoint(
                                r.NextDouble(),
                                r.Next(0, 181),
                                Enums.MapPointType.Ramp,
                                Enums.WasPlaced.NotAttempted));
                    }
                }
            }

            if (r.NextDouble() < mso.ChanceToRemoveElement)
            {
                var potentialMapPoints = new List<MapPoint>();
                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase) > mso.MinimumNumberOfBases)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.Base || mp.Type == Enums.MapPointType.GoldBase));
                }

                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.XelNagaTower) > mso.MinimumNumberOfXelNagaTowers)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.XelNagaTower));
                }

                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.Ramp) > mso.MinimumNumberOfRamps)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.Ramp));
                }

                if (newPoints.Count(mp => mp.Type == Enums.MapPointType.DestructibleRocks) > mso.MinimumNumberOfDestructibleRocks)
                {
                    potentialMapPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.DestructibleRocks));
                }

                if (potentialMapPoints.Count > 0)
                {
                    var index = r.Next(0, potentialMapPoints.Count);
                    var element = potentialMapPoints[index];
                    newPoints.Remove(element);
                }
            }

            var finalNewPoints = new List<MapPoint>();
            finalNewPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.StartBase));
            finalNewPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.Base));
            finalNewPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.GoldBase));
            finalNewPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.XelNagaTower));
            finalNewPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.DestructibleRocks));
            finalNewPoints.AddRange(newPoints.Where(mp => mp.Type == Enums.MapPointType.Ramp));

            return finalNewPoints;
        }

        /// <summary>
        /// Calculates which of the map point types, if any, are out of bounds according to the search options.
        /// </summary>
        /// <param name="mapPoints">
        /// The map points.
        /// </param>
        /// <param name="mso">
        /// The map search options.
        /// </param>
        /// <returns>
        /// A tuple containing the number of nap points that are missing (item1) and the number of map points that are in excess (item2).
        /// </returns>
        public static Tuple<int, int> CalculateNumberOfMapPointsOutsideTypeBounds(List<MapPoint> mapPoints, MapSearchOptions mso)
        {
            var numberOfMapPointsMissing = 0;
            var numberOfTooManyMapPoints = 0;

            var numberOfBases = mapPoints.Count(mp => mp.Type == Enums.MapPointType.Base && mp.WasPlaced == Enums.WasPlaced.Yes);
            var numberOfDestructibleRocks = mapPoints.Count(mp => mp.Type == Enums.MapPointType.DestructibleRocks && mp.WasPlaced == Enums.WasPlaced.Yes);
            var numberOfRamps = mapPoints.Count(mp => mp.Type == Enums.MapPointType.Ramp && mp.WasPlaced == Enums.WasPlaced.Yes);
            var numberOfXelNagaTowers = mapPoints.Count(mp => mp.Type == Enums.MapPointType.XelNagaTower && mp.WasPlaced == Enums.WasPlaced.Yes);

            if (numberOfBases < mso.MinimumNumberOfBases)
            {
                numberOfMapPointsMissing += mso.MinimumNumberOfBases - numberOfBases;
            }
            else if (numberOfBases > mso.MaximumNumberOfBases)
            {
                numberOfTooManyMapPoints += numberOfBases - mso.MaximumNumberOfBases;
            }

            if (numberOfDestructibleRocks < mso.MinimumNumberOfDestructibleRocks)
            {
                numberOfMapPointsMissing += mso.MinimumNumberOfDestructibleRocks - numberOfDestructibleRocks;
            }
            else if (numberOfDestructibleRocks > mso.MaximumNumberOfDestructibleRocks)
            {
                numberOfTooManyMapPoints += numberOfDestructibleRocks - mso.MaximumNumberOfDestructibleRocks;
            }

            if (numberOfRamps < mso.MinimumNumberOfRamps)
            {
                numberOfMapPointsMissing += mso.MinimumNumberOfRamps - numberOfRamps;
            }
            else if (numberOfRamps > mso.MaximumNumberOfRamps)
            {
                numberOfTooManyMapPoints += numberOfRamps - mso.MaximumNumberOfRamps;
            }

            if (numberOfXelNagaTowers < mso.MinimumNumberOfXelNagaTowers)
            {
                numberOfMapPointsMissing += mso.MinimumNumberOfXelNagaTowers - numberOfXelNagaTowers;
            }
            else if (numberOfBases > mso.MaximumNumberOfXelNagaTowers)
            {
                numberOfTooManyMapPoints += numberOfXelNagaTowers - mso.MaximumNumberOfXelNagaTowers;
            }

            return new Tuple<int, int>(numberOfMapPointsMissing, numberOfTooManyMapPoints);
        }

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
        /// Attempts to locate the tile of the same type as the input map point, starting at the original location of the map point.
        /// </summary>
        /// <param name="mapPoint">The map point to search for.</param>
        /// <param name="mp">The map phenotype.</param>
        /// <returns>The position in the format of {x-coordinate, y-coordinate, distance from initial point}.</returns>
        public static Tuple<int, int, int> FindPositionBasedOnMapPoint(MapPoint mapPoint, MapPhenotype mp)
        {
            var maxDistance = MaxDistanceAtDegree(mp.XSize / 2.0, mp.YSize / 2.0, mapPoint.Degree);
            var point = CalculatePoint(mapPoint.Degree, maxDistance * mapPoint.Distance);
            var xPos = (int)(point.Item1 + (mp.XSize / 2.0));
            var yPos = (int)(point.Item2 + (mp.YSize / 2.0));

            Enums.Item type;

            switch (mapPoint.Type)
            {
                case Enums.MapPointType.Base:
                    type = Enums.Item.Base;
                    break;
                case Enums.MapPointType.DestructibleRocks:
                    type = Enums.Item.DestructibleRocks;
                    break;
                case Enums.MapPointType.GoldBase:
                    type = Enums.Item.Base;
                    break;
                case Enums.MapPointType.StartBase:
                    type = Enums.Item.StartBase;
                    break;
                case Enums.MapPointType.XelNagaTower:
                    type = Enums.Item.XelNagaTower;
                    break;
                default:
                    type = Enums.Item.StartBase;
                    break;
            }

            var position = FindNearestItemTileOfType(
                xPos,
                yPos,
                mp,
                type);

            return position;
        }

        /// <summary>
        /// Fills in a map using the map points of this individual. Returns a filled in copy of the original map.
        /// </summary>
        /// <param name="mapPoints"> The map points to place. </param>
        /// <param name="mso"> The map search options. </param>
        /// <param name="random">The random object to use in conversion.</param>
        /// <returns> The newly filled in map. </returns>
        public static MapPhenotype ConvertToPhenotype(List<MapPoint> mapPoints, MapSearchOptions mso, Random random)
        {
            var heightLevels = mso.Map.HeightLevels.Clone() as Enums.HeightLevel[,];
            var items = mso.Map.MapItems.Clone() as Enums.Item[,];
            var rocks = mso.Map.DestructibleRocks.Clone() as bool[,];

            var newMap = new MapPhenotype(heightLevels, items, rocks);
            newMap.UpdateCliffPositions(mso.Map.CliffPositions);

            foreach (var mp in mapPoints)
            {
                if (mp.Degree < mso.MinimumDegree || mp.Degree > mso.MaximumDegree
                    || mp.Distance < mso.MinimumDistance || mp.Distance > mso.MaximumDistance)
                {
                    mp.WasPlaced = Enums.WasPlaced.No;
                    continue;
                }

                var maxDistance = MaxDistanceAtDegree(newMap.XSize / 2.0, newMap.YSize / 2.0, mp.Degree);
                var point = CalculatePoint(mp.Degree, maxDistance * mp.Distance);

                var xPos = (int)(point.Item1 + (newMap.XSize / 2.0));
                var yPos = (int)(point.Item2 + (newMap.YSize / 2.0));

                if (!newMap.InsideTopHalf(xPos, yPos))
                {
                    throw new ArgumentOutOfRangeException();
                }

                bool placed;

                switch (mp.Type)
                {
                    case Enums.MapPointType.Base:
                        placed = PlaceBase(xPos, yPos, newMap);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= mso.MaximumDisplacement;
                                 i += mso.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                     j <= mso.MaximumDisplacement;
                                     j += mso.DisplacementAmountPerStep)
                                {
                                    if (PlaceBase(xPos - i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos + i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos - i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos + i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos - i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos + i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.GoldBase:
                        placed = PlaceBase(xPos, yPos, newMap, true);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= mso.MaximumDisplacement;
                                 i += mso.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                     j <= mso.MaximumDisplacement;
                                     j += mso.DisplacementAmountPerStep)
                                {
                                    if (PlaceBase(xPos - i, yPos + j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos, yPos + j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos + i, yPos + j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos - i, yPos, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos + i, yPos, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos - i, yPos - j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos, yPos - j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceBase(xPos + i, yPos - j, newMap, true))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.StartBase:
                        placed = PlaceStartBase(xPos, yPos, newMap);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= mso.MaximumDisplacement;
                                 i += mso.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                     j <= mso.MaximumDisplacement;
                                     j += mso.DisplacementAmountPerStep)
                                {
                                    if (PlaceStartBase(xPos - i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos + i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos - i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos + i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos - i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceStartBase(xPos + i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.XelNagaTower:
                        placed = PlaceXelNagaTower(xPos, yPos, newMap);
                        if (!placed)
                        {
                            for (var i = 1;
                                 i <= mso.MaximumDisplacement;
                                 i += mso.DisplacementAmountPerStep)
                            {
                                for (var j = 1;
                                     j <= mso.MaximumDisplacement;
                                     j += mso.DisplacementAmountPerStep)
                                {
                                    if (PlaceXelNagaTower(xPos - i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos + i, yPos + j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos - i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos + i, yPos, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos - i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }

                                    if (PlaceXelNagaTower(xPos + i, yPos - j, newMap))
                                    {
                                        placed = true;
                                        break;
                                    }
                                }

                                if (placed)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.Ramp:
                        placed = PlaceRamp(xPos, yPos, newMap);
                        
                        var cliffPos = newMap.CliffPositions.ToList();

                        if (cliffPos.Count > 0)
                        {
                            var displacementAttempts = 0;
                            while (!placed)
                            {
                                var hash = Math.Abs((mp.Degree * mp.Distance).GetHashCode());
                                var index = (hash + displacementAttempts) % cliffPos.Count;
                                var pos = cliffPos[index];
                                placed = PlaceRamp(pos.Item1, pos.Item2, newMap);
                                displacementAttempts += mso.DisplacementAmountPerStep;
                                if (displacementAttempts > mso.MaximumDisplacement)
                                {
                                    break;
                                }
                            }
                        }

                        mp.WasPlaced = placed ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    case Enums.MapPointType.DestructibleRocks:
                        mp.WasPlaced = PlaceDestructibleRocks(xPos, yPos, newMap, random) ? Enums.WasPlaced.Yes : Enums.WasPlaced.No;
                        break;
                    default:
                        newMap.MapItems[yPos, xPos] = Enums.Item.None;
                        break;
                }
            }

            return newMap;
        }

        /// <summary>
        /// Generates an initial set of map points.
        /// </summary>
        /// <param name="mapSearchOptions">The map search options.</param>
        /// <param name="r">The random object.</param>
        /// <returns>The list of initial map points.</returns>
        public static List<MapPoint> GenerateInitialMapPoints(MapSearchOptions mapSearchOptions, Random r)
        {
            var mapPoints = new List<MapPoint>
                                {
                                    new MapPoint(
                                        (r.NextDouble()
                                         * (mapSearchOptions.MaximumStartBaseDistance - mapSearchOptions.MinimumStartBaseDistance))
                                        + mapSearchOptions.MinimumStartBaseDistance,
                                        (r.NextDouble() * (mapSearchOptions.MaximumDegree - mapSearchOptions.MinimumDegree)) + mapSearchOptions.MinimumDegree,
                                        Enums.MapPointType.StartBase,
                                        Enums.WasPlaced.NotAttempted)
                                };

            var numberOfBases = r.Next(mapSearchOptions.MinimumNumberOfBases, mapSearchOptions.MaximumNumberOfBases + 1);
            for (var i = 0; i < numberOfBases; i++)
            {
                var gold = r.NextDouble() < mapSearchOptions.ChanceToAddGoldBase;
                mapPoints.Add(new MapPoint(
                    (r.NextDouble() * (mapSearchOptions.MaximumDistance - mapSearchOptions.MinimumDistance)) + mapSearchOptions.MinimumDistance,
                    (r.NextDouble() * (mapSearchOptions.MaximumDegree - mapSearchOptions.MinimumDegree)) + mapSearchOptions.MinimumDegree,
                    gold ? Enums.MapPointType.GoldBase : Enums.MapPointType.Base,
                    Enums.WasPlaced.NotAttempted));
            }

            var numberOfXelNaga = r.Next(mapSearchOptions.MinimumNumberOfXelNagaTowers, mapSearchOptions.MaximumNumberOfXelNagaTowers + 1);
            for (var i = 0; i < numberOfXelNaga; i++)
            {
                mapPoints.Add(new MapPoint(
                    (r.NextDouble() * (mapSearchOptions.MaximumDistance - mapSearchOptions.MinimumDistance)) + mapSearchOptions.MinimumDistance,
                    (r.NextDouble() * (mapSearchOptions.MaximumDegree - mapSearchOptions.MinimumDegree)) + mapSearchOptions.MinimumDegree,
                    Enums.MapPointType.XelNagaTower,
                    Enums.WasPlaced.NotAttempted));
            }

            var numberOfDestructibleRocks = r.Next(mapSearchOptions.MinimumNumberOfDestructibleRocks, mapSearchOptions.MaximumNumberOfDestructibleRocks + 1);
            for (var i = 0; i < numberOfDestructibleRocks; i++)
            {
                mapPoints.Add(new MapPoint(
                    (r.NextDouble() * (mapSearchOptions.MaximumDistance - mapSearchOptions.MinimumDistance)) + mapSearchOptions.MinimumDistance,
                    (r.NextDouble() * (mapSearchOptions.MaximumDegree - mapSearchOptions.MinimumDegree)) + mapSearchOptions.MinimumDegree,
                    Enums.MapPointType.DestructibleRocks,
                    Enums.WasPlaced.NotAttempted));
            }

            var numberOfRamps = r.Next(mapSearchOptions.MinimumNumberOfRamps, mapSearchOptions.MaximumNumberOfRamps + 1);
            for (var i = 0; i < numberOfRamps; i++)
            {
                mapPoints.Add(new MapPoint(
                    (r.NextDouble() * (mapSearchOptions.MaximumDistance - mapSearchOptions.MinimumDistance)) + mapSearchOptions.MinimumDistance,
                    (r.NextDouble() * (mapSearchOptions.MaximumDegree - mapSearchOptions.MinimumDegree)) + mapSearchOptions.MinimumDegree,
                    Enums.MapPointType.Ramp,
                    Enums.WasPlaced.NotAttempted));
            }

            return mapPoints;
        }
        #endregion

        #region Flatten/Occupy Methods
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
            // TODO: Melnyk - Implement variations of flattening
            ////var centreY = startY + (lengthY / 2);
            ////for (var y = startY; y <= startY + lengthY; y++)
            ////{
            ////    var distanceToCentreY = y - centreY;
            ////    var xStartMod = distanceToCentreY > 0 ? distanceToCentreY - 1 : Math.Abs(distanceToCentreY);

            ////    var xStart = startX + xStartMod;
            ////    var xEnd = startX + lengthX - xStartMod;

            ////    for (var x = xStart; x <= xEnd; x++)
            ////    {
            ////        if (mp.HeightLevels[x, y] == Enums.HeightLevel.Cliff && height != Enums.HeightLevel.Cliff)
            ////        {
            ////            mp.CliffPositions.Remove(new Tuple<int, int>(x, y));
            ////        }

            ////        mp.HeightLevels[x, y] = height;

            ////        if (height == Enums.HeightLevel.Cliff)
            ////        {
            ////            mp.CliffPositions.Add(new Tuple<int, int>(x, y));
            ////        }
            ////    }
            ////}

            if (!mp.InsideTopHalf(startX, startY) || !mp.InsideTopHalf(startX + lengthX, startY + lengthY))
            {
                return;
            }

            for (var i = startX; i <= startX + lengthX; i++)
            {
                for (var j = startY; j <= startY + lengthY; j++)
                {
                    if (mp.HeightLevels[i, j] == Enums.HeightLevel.Cliff && height != Enums.HeightLevel.Cliff)
                    {
                        mp.CliffPositions.Remove(new Tuple<int, int>(i, j));
                    }

                    mp.HeightLevels[i, j] = height;

                    if (height == Enums.HeightLevel.Cliff)
                    {
                        mp.CliffPositions.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
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

            for (var i = startX; i <= startX + lengthX; i++)
            {
                for (var j = startY; j <= startY + lengthY; j++)
                {
                    if (mp.MapItems[i, j] != Enums.Item.None) return true;
                }
            }

            return false;
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

            for (var i = startX; i <= startX + lengthX; i++)
            {
                for (var j = startY; j <= startY + lengthY; j++)
                {
                    mp.MapItems[i, j] = Enums.Item.Occupied;
                }
            }
        }
        #endregion

        #region Place Map Point Methods

        /// <summary>
        /// Places a starting base (24x24) around the given coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="mp">The map phenotype to place the base in.</param>
        /// <returns>True if placement was successful, false if not.</returns>
        private static bool PlaceStartBase(int x, int y, MapPhenotype mp)
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

            FlattenArea(height, x - 11, y - 11, 23, 23, mp);
            OccupyArea(x - 11, y - 11, 23, 23, mp);

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
        /// Places a base (16x16) around a given location.
        /// </summary>
        /// <param name="x"> The x-coordinate. </param>
        /// <param name="y"> The y-coordinate. </param>
        /// <param name="mp"> The map phenotype to place the base in. </param>
        /// <param name="isGoldBase"> Whether this is a gold base or not. </param>
        /// <returns>True if placement was successful, false if not.</returns>
        private static bool PlaceBase(int x, int y, MapPhenotype mp, bool isGoldBase = false)
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

            if (IsAreaOccupied(x - 7, y - 7, 15, 15, mp))
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

            FlattenArea(height, x - 7, y - 7, 15, 15, mp);
            OccupyArea(x - 7, y - 7, 15, 15, mp);

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
        /// Checks if destructible rocks can be placed in a specified area.
        /// </summary>
        /// <param name="startX">The starting x-coordinate.</param>
        /// <param name="startY">The starting y-coordinate.</param>
        /// <param name="lengthX">The length on the x-axis.</param>
        /// <param name="lengthY">The length on the y-axis.</param>
        /// <param name="mp">The map phenotype to check.</param>
        /// <returns>True if all tiles in the area can have destructible rocks placed on them, false if not.</returns>
        private static bool DestructibleRocksPlacableInArea(int startX, int startY, int lengthX, int lengthY, MapPhenotype mp)
        {
            for (var x = startX; x <= startX + lengthX; x++)
            {
                for (var y = startY; y <= startY + lengthY; y++)
                {
                    if (mp.HeightLevels[x, y] == Enums.HeightLevel.Cliff || mp.HeightLevels[x, y] == Enums.HeightLevel.Impassable
                        || mp.MapItems[x, y] == Enums.Item.StartBase || mp.MapItems[x, y] == Enums.Item.XelNagaTower
                        || mp.MapItems[x, y] == Enums.Item.BlueMinerals || mp.MapItems[x, y] == Enums.Item.GoldMinerals
                        || mp.MapItems[x, y] == Enums.Item.Gas)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Places destructible rocks in the specified area.
        /// </summary>
        /// <param name="startX">The starting x-coordinate.</param>
        /// <param name="startY">The starting y-coordinate.</param>
        /// <param name="lengthX">The length on the x-axis.</param>
        /// <param name="lengthY">The length on the y-axis.</param>
        /// <param name="mp">The map phenotype to place destructible rocks on.</param>
        private static void PlaceDestructibleRocks(int startX, int startY, int lengthX, int lengthY, MapPhenotype mp)
        {
            for (var x = startX; x <= startX + lengthX; x++)
            {
                for (var y = startY; y <= startY + lengthY; y++)
                {
                    mp.DestructibleRocks[x, y] = true;
                }
            }
        }

        /// <summary>
        /// Places destructible debris taking up 2x2 spaces on the given map. The debris will not be placed if it is put outside bounds. It will also not overwrite any bases/minerals.
        /// </summary>
        /// <param name="x"> The x-location to attempt to place the debris. </param>
        /// <param name="y"> The y-location to attempt to place the debris. </param>
        /// <param name="mp"> The map phenotype to place the debris on. </param>
        /// <param name="random">The random object to use.</param>
        /// <returns>True if the rocks were placed, false if not</returns>
        private static bool PlaceDestructibleRocks(int x, int y, MapPhenotype mp, Random random)
        {
            if (!mp.InsideTopHalf(x, y))
            {
                return false;
            }

            int xMod;
            int yMod;

            switch (random.Next(3))
            {
                case 0:
                    xMod = 1;
                    yMod = 1;
                    break;
                case 1:
                    xMod = 3;
                    yMod = 3;
                    break;
                case 2:
                    xMod = 5;
                    yMod = 5;
                    break;
                default:
                    xMod = 1;
                    yMod = 1;
                    break;
            }

            // HACK: Removed check for occupied tiles (risks being placed on inside a starting position)
            while (xMod >= 1)
            {
                if (mp.InsideTopHalf(x + xMod, y) && mp.InsideTopHalf(x, y + yMod) && mp.InsideTopHalf(x + xMod, y + yMod))
                {
                    // Bottom-left
                    if (DestructibleRocksPlacableInArea(x, y, xMod, yMod, mp))
                    {
                        PlaceDestructibleRocks(x, y, xMod, yMod, mp);
                        return true;
                    }
                }

                if (mp.InsideTopHalf(x - xMod, y) && mp.InsideTopHalf(x, y + yMod) && mp.InsideTopHalf(x - xMod, y + yMod))
                {
                    // Bottom-right
                    if (DestructibleRocksPlacableInArea(x - xMod, y, xMod, yMod, mp))
                    {
                        PlaceDestructibleRocks(x - xMod, y, xMod, yMod, mp);
                        return true;
                    }
                }

                if (mp.InsideTopHalf(x + xMod, y) && mp.InsideTopHalf(x, y - yMod) && mp.InsideTopHalf(x + xMod, y - yMod))
                {
                    // Top-left
                    if (DestructibleRocksPlacableInArea(x, y - yMod, xMod, yMod, mp))
                    {
                        PlaceDestructibleRocks(x, y - yMod, xMod, yMod, mp);
                        return true;
                    }
                }

                if (mp.InsideTopHalf(x - xMod, y) && mp.InsideTopHalf(x, y - yMod) && mp.InsideTopHalf(x - xMod, y - yMod))
                {
                    // Top-right
                    if (DestructibleRocksPlacableInArea(x - xMod, y - yMod, xMod, yMod, mp))
                    {
                        PlaceDestructibleRocks(x - xMod, y - yMod, xMod, yMod, mp);
                        return true;
                    }
                }

                xMod -= 2;
                yMod -= 2;
            }

            return false;
        }

        /// <summary>
        /// Places a Xel'Naga Tower taking up 2x2 spaces on the given map. The tower will not be placed if it is put outside bounds. It will also not overwrite any bases/minerals.
        /// </summary>
        /// <param name="x">The y-location to attempt to place the Xel'Naga tower.</param>
        /// <param name="y">The x-location to attempt to place the Xel'Naga tower.</param>
        /// <param name="mp">The map phenotype to place the tower on.</param>
        /// <returns>True if placement was successful, false if not.</returns>
        private static bool PlaceXelNagaTower(int x, int y, MapPhenotype mp)
        {
            return PlaceTwoByTwo(x, y, mp, Enums.Item.XelNagaTower);
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
                    FlattenArea(mp.HeightLevels[x, y], x, y, 1, 1, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x + 1, y] = itemToPlace;
                    mp.MapItems[x, y + 1] = itemToPlace;
                    mp.MapItems[x + 1, y + 1] = itemToPlace;
                    return true;
                }
            }

            if (IsAreaOccupied(x - 1, y, 1, 1, mp))
            {
                return false;
            }

            if (mp.InsideTopHalf(x - 1, y) && mp.InsideTopHalf(x, y + 1) && mp.InsideTopHalf(x - 1, y + 1))
            {
                // Bottom-right
                if (mp.HeightLevels[x - 1, y] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x - 1, y], x - 1, y, 1, 1, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x - 1, y] = itemToPlace;
                    mp.MapItems[x, y + 1] = itemToPlace;
                    mp.MapItems[x - 1, y + 1] = itemToPlace;
                    return true;
                }
            }

            if (IsAreaOccupied(x, y - 1, 1, 1, mp))
            {
                return false;
            }

            if (mp.InsideTopHalf(x + 1, y) && mp.InsideTopHalf(x, y - 1) && mp.InsideTopHalf(x + 1, y - 1))
            {
                // Top-left
                if (mp.HeightLevels[x, y - 1] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x, y - 1], x, y - 1, 1, 1, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x + 1, y] = itemToPlace;
                    mp.MapItems[x, y - 1] = itemToPlace;
                    mp.MapItems[x + 1, y - 1] = itemToPlace;
                    return true;
                }
            }

            if (IsAreaOccupied(x - 1, y - 1, 1, 1, mp))
            {
                return false;
            }

            if (mp.InsideTopHalf(x - 1, y) && mp.InsideTopHalf(x, y - 1) && mp.InsideTopHalf(x - 1, y - 1))
            {
                // Top-right
                if (mp.HeightLevels[x - 1, y - 1] != Enums.HeightLevel.Cliff && mp.HeightLevels[x, y] != Enums.HeightLevel.Impassable)
                {
                    FlattenArea(mp.HeightLevels[x - 1, y - 1], x - 1, y - 1, 1, 1, mp);
                    mp.MapItems[x, y] = itemToPlace;
                    mp.MapItems[x - 1, y] = itemToPlace;
                    mp.MapItems[x, y - 1] = itemToPlace;
                    mp.MapItems[x - 1, y - 1] = itemToPlace;
                    return true;
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
        #endregion

        #region Place Ramp Methods
        /// <summary>
        /// Attempts to place a ramp on a map phenotype.
        /// </summary>
        /// <param name="x"> The x-coordinate to attempt to use as a starting point. </param>
        /// <param name="y"> The y-coordinate to attempt to use as a starting point. </param>
        /// <param name="mp"> The map phenotype. </param>
        /// <returns> The <see cref="bool"/> indicating whether placement was successful. </returns>
        private static bool PlaceRamp(int x, int y, MapPhenotype mp)
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

            // TODO: Melnyk - Diagonal ramps
            // Northwest-Southeast diagonal
            // Northeast-Southwest diagonal
            return false;
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

            var westHigherLevel = false;
            switch (west)
            {
                case Enums.HeightLevel.Height2:
                    if (east == Enums.HeightLevel.Height1 || east == Enums.HeightLevel.Height0)
                    {
                        westHigherLevel = true;
                    }

                    break;

                case Enums.HeightLevel.Height1:
                    if (east == Enums.HeightLevel.Height0)
                    {
                        westHigherLevel = true;
                    }

                    break;
            }

            if (mp.HeightLevels[x - 1, y] == west && mp.HeightLevels[x - 1, y + 1] == west
                && mp.HeightLevels[x + 1, y] == east && mp.HeightLevels[x + 1, y + 1] == east)
            {
                if (westHigherLevel)
                {
                    if (mp.InsideTopHalf(x - 3, y) || mp.InsideTopHalf(x - 3, y + 1))
                    {
                        if (!IsAreaOccupied(x - 3, y, 1, 2, mp) && !IsAreaOccupied(x - 2, y - 1, 4, 4, mp))
                        {
                            if (mp.HeightLevels[x - 2, y] == west && mp.HeightLevels[x - 2, y + 1] == west
                                && mp.HeightLevels[x - 3, y] == west && mp.HeightLevels[x - 3, y + 1] == west)
                            {
                                OccupyArea(x - 2, y - 1, 4, 4, mp);

                                mp.HeightLevels[x - 3, y] = ramp;
                                mp.HeightLevels[x - 3, y + 1] = ramp;

                                mp.HeightLevels[x - 2, y] = ramp;
                                mp.HeightLevels[x - 2, y + 1] = ramp;
                                mp.HeightLevels[x - 2, y - 1] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x - 2, y + 2] = Enums.HeightLevel.Cliff;

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
                                mp.HeightLevels[x + 1, y - 1] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 1, y + 2] = Enums.HeightLevel.Cliff;

                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 3, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 3, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 2, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 2, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 1, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 1, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y + 1));

                                mp.CliffPositions.Add(new Tuple<int, int>(x - 2, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 2, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 1, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 1, y + 2));

                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (mp.InsideTopHalf(x - 1, y) || mp.InsideTopHalf(x - 1, y + 1))
                    {
                        if (!IsAreaOccupied(x - 1, y - 1, 4, 4, mp) && !IsAreaOccupied(x + 3, y, 1, 2, mp))
                        {
                            if (mp.HeightLevels[x + 2, y] == east && mp.HeightLevels[x + 2, y + 1] == east
                                && mp.HeightLevels[x + 3, y] == east && mp.HeightLevels[x + 3, y + 1] == east)
                            {
                                OccupyArea(x - 1, y - 1, 4, 4, mp);

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
                                mp.HeightLevels[x + 1, y - 1] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 1, y + 2] = Enums.HeightLevel.Cliff;

                                mp.HeightLevels[x + 2, y] = ramp;
                                mp.HeightLevels[x + 2, y + 1] = ramp;
                                mp.HeightLevels[x + 2, y - 1] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 2, y + 2] = Enums.HeightLevel.Cliff;

                                mp.HeightLevels[x + 3, y] = ramp;
                                mp.HeightLevels[x + 3, y + 1] = ramp;

                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 1, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x - 1, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 2, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 2, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 3, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 3, y + 1));

                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 1, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 1, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2, y + 2));

                                return true;
                            }
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
            if (!mp.InsideTopHalf(x, y - 1) || !mp.InsideTopHalf(x + 1, y - 1) 
                || !mp.InsideTopHalf(x, y + 1) || !mp.InsideTopHalf(x + 1, y + 1))
            {
                return false;
            }

            var northHigherLevel = false;
            switch (north)
            {
                case Enums.HeightLevel.Height2:
                    if (south == Enums.HeightLevel.Height1 || south == Enums.HeightLevel.Height0)
                    {
                        northHigherLevel = true;
                    }

                    break;

                case Enums.HeightLevel.Height1:
                    if (south == Enums.HeightLevel.Height0)
                    {
                        northHigherLevel = true;
                    }

                    break;
            }

            if (mp.HeightLevels[x, y + 1] == north && mp.HeightLevels[x + 1, y + 1] == north
                && mp.HeightLevels[x, y - 1] == south && mp.HeightLevels[x + 1, y - 1] == south)
            {
                if (northHigherLevel)
                {
                    if (mp.InsideTopHalf(x, y - 3) || mp.InsideTopHalf(x + 1, y - 3))
                    {
                        if (!IsAreaOccupied(x, y - 3, 2, 1, mp) && !IsAreaOccupied(x - 1, y - 2, 4, 4, mp))
                        {
                            if (mp.HeightLevels[x, y - 2] == south && mp.HeightLevels[x + 1, y + 2] == south
                                && mp.HeightLevels[x, y - 3] == south && mp.HeightLevels[x + 1, y + 3] == south)
                            {
                                OccupyArea(x - 1, y - 2, 4, 4, mp);

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
                                mp.HeightLevels[x - 1, y - 1] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 2, y - 1] = Enums.HeightLevel.Cliff;

                                mp.HeightLevels[x, y - 2] = ramp;
                                mp.HeightLevels[x + 1, y - 2] = ramp;
                                mp.HeightLevels[x - 1, y - 2] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 2, y - 2] = Enums.HeightLevel.Cliff;

                                mp.HeightLevels[x, y - 3] = ramp;
                                mp.HeightLevels[x + 1, y - 3] = ramp;

                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y - 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y - 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y - 2));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y - 2));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y - 3));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y - 3));

                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1,    y + 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2,    y + 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1,    y));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2,    y));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1,    y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2,    y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1,    y - 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2,    y - 2));

                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (mp.InsideTopHalf(x, y + 3) || mp.InsideTopHalf(x + 1, y + 3))
                    {
                        if (!IsAreaOccupied(x - 1, y - 1, 4, 4, mp) && !IsAreaOccupied(x, y + 3, 2, 1, mp))
                        {
                            if (mp.HeightLevels[x, y + 2] == north && mp.HeightLevels[x + 1, y + 2] == north
                                && mp.HeightLevels[x, y + 3] == north && mp.HeightLevels[x + 1, y + 3] == north)
                            {
                                OccupyArea(x - 1, y - 1, 4, 4, mp);

                                mp.HeightLevels[x, y + 3] = ramp;
                                mp.HeightLevels[x + 1, y + 3] = ramp;

                                mp.HeightLevels[x, y + 2] = ramp;
                                mp.HeightLevels[x + 1, y + 2] = ramp;
                                mp.HeightLevels[x - 1, y + 2] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 2, y + 2] = Enums.HeightLevel.Cliff;

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
                                mp.HeightLevels[x - 1, y - 1] = Enums.HeightLevel.Cliff;
                                mp.HeightLevels[x + 2, y - 1] = Enums.HeightLevel.Cliff;

                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y + 3));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y + 3));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y + 2));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y + 2));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y + 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x,     y - 1));
                                mp.CliffPositions.Remove(new Tuple<int, int>(x + 1, y - 1));

                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2, y + 2));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y + 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2, y + 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2, y));
                                mp.CliffPositions.Add(new Tuple<int, int>(x - 1, y - 1));
                                mp.CliffPositions.Add(new Tuple<int, int>(x + 2, y - 1));

                                return true;
                            }
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
        #endregion

        #region Math Methods
        /// <summary>
        /// Converts an angle in degrees to radians.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>The angle in radians.</returns>
        private static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        /// <summary>
        /// Finds a point based on an angle and distance.
        /// </summary>
        /// <param name="degree">The angle in degrees.</param>
        /// <param name="distance">The distance.</param>
        /// <returns>The point.</returns>
        private static Tuple<double, double> CalculatePoint(double degree, double distance)
        {
            var radians = ConvertToRadians(degree);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            return CalculatePoint(distance, cos, sin);
        }

        /// <summary>
        /// Finds a point based on a distance and the cos and sin values of an angle.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="cos">The cos-value of the angle.</param>
        /// <param name="sin">The sin-value of the angle.</param>
        /// <returns>The point.</returns>
        private static Tuple<double, double> CalculatePoint(double distance, double cos, double sin)
        {
            var xDist = cos * distance;
            var yDist = sin * distance;

            return new Tuple<double, double>(xDist, yDist);
        }

        /// <summary>
        /// Calculates the distance to the edge of the map from the centre at a given angle.
        /// </summary>
        /// <param name="xSize">The width of the map.</param>
        /// <param name="ySize">The height of the map.</param>
        /// <param name="degree">The degree to look at.</param>
        /// <returns>The distance to the edge of the map.</returns>
        private static double MaxDistanceAtDegree(double xSize, double ySize, double degree)
        {
            var squared = Math.Pow(xSize, 2) + Math.Pow(ySize, 2);
            var maxDistance = Math.Sqrt(squared);
            var radians = ConvertToRadians(degree);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            var stop = false;

            do
            {
                var point = CalculatePoint(maxDistance, cos, sin);

                if (
                    (!(point.Item1 <= xSize) || (point.Item1 < -xSize))
                    || (!(point.Item2 <= ySize) || (point.Item2 < -ySize)))
                {
                    maxDistance--;
                }
                else
                {
                    stop = true;
                }
            }
            while (!stop);

            return maxDistance;
        }
        #endregion
    }
}
