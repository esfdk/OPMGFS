namespace OPMGFS.Novelty.MapNoveltySearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OPMGFS.Map;
    using OPMGFS.Map.MapObjects;

    public class MapSolution : Solution
    {
        public List<MapPoint> MapPoints { get; private set; }

        public MapSolution(List<MapPoint> mapPoints)
        {
            this.MapPoints = mapPoints;
        }

        public MapSolution()
        {
            this.MapPoints = new List<MapPoint>();
        }

        public override Solution Mutate(Random r)
        {
            var chance = 0.3;
            var maxDistMod = 0;
            var maxDegreeMod = 10.0;
            var maxDist = 128;
            var maxDegree = 180;

            // TODO: Figure out settings locations
            // TODO: Should we limit mutation to be ONLY towards feasibility? (eg. when degree > maxDegree, only allow it to go lower?) Probably
            var newPoints = new List<MapPoint>();
            
            foreach (var mp in MapPoints)
            {
                if (r.NextDouble() < chance)
                {
                    newPoints.Add(mp.Mutate(r));
                }
                else
                {
                    newPoints.Add(mp);
                }
            }

            /*
            if (r.NextDouble() < chance)
            {
                // TODO: Should we ever remove a point? When? When do we stop adding?
                var dist = r.NextDouble()*maxDist;
                var degree = r.NextDouble()*maxDegree;
                Enums.MapPointType mpt;
                switch (r.Next(5))
                {
                    case 0:
                        mpt = Enums.MapPointType.Base;
                        break;
                    case 1:
                        mpt = Enums.MapPointType.GoldBase;
                        break;
                    case 2:
                        mpt = Enums.MapPointType.Ramp;
                        break;
                    case 3:
                        mpt = Enums.MapPointType.XelNagaTower;
                        break;
                    case 4:
                        mpt = Enums.MapPointType.StartBase;
                        break;
                    default:
                        mpt = Enums.MapPointType.Base;
                        break;
                }
                newPoints.Add(new MapPoint(dist,degree,mpt));
            }*/

            return new MapSolution(newPoints);
        }

        public override Solution Recombine(Solution other)
        {
            throw new NotImplementedException();
        }

        public override double CalculateNovelty(Population feasible, NovelArchive archive, int numberOfNeighbours)
        {
            // TODO: Control number of elements in novel archive? Per generation?

            var distanceValues = new List<Tuple<double, int, int>>();

            // Feasible population distance
            for (var i = 0; i < feasible.CurrentGeneration.Count; i++)
            {
                if (feasible.CurrentGeneration[i] == this)
                {
                    continue;
                }

                var distance = this.CalculateDistance(feasible.CurrentGeneration[i]);

                var wasAdded = false;

                for (var j = 0; j < distanceValues.Count; j++)
                {
                    if (distance < distanceValues[j].Item1)
                    {
                        distanceValues.Insert(j, new Tuple<double, int, int>(distance, i, 1));
                        wasAdded = true;
                        break;
                    }
                }

                if (!wasAdded)
                {
                    distanceValues.Add(new Tuple<double, int, int>(distance, i, 1));
                }
            }

            // Novelty archive distance
            for (var i = 0; i < archive.Archive.Count; i++)
            {
                var distance = this.CalculateDistance(archive.Archive[i]);

                var wasAdded = false;

                for (var j = 0; j < distanceValues.Count; j++)
                {

                    if (distance < distanceValues[j].Item1)
                    {
                        distanceValues.Insert(j, new Tuple<double, int, int>(distance, i, 2));
                        wasAdded = true;
                        break;
                    }
                }

                if (!wasAdded)
                {
                    distanceValues.Add(new Tuple<double, int, int>(distance, i, 2));
                }
            }

            var novelty = 0.0;

            for (var i = 0; i < numberOfNeighbours; i++)
            {
                novelty += distanceValues[i].Item1;
            }

            this.Novelty = novelty / numberOfNeighbours;
            return this.Novelty;
        }

        protected override double CalculateDistance(Solution other)
        {
            if (this.GetType() != other.GetType())
            {
                return double.NegativeInfinity;
            }

            var dist = 0.0;
            var target = (MapSolution) other;

            // TODO: Novelty of a solution is the distance of each element in a solution to every other element in the solution?

            foreach (var mapPoint in this.MapPoints)
            {
                foreach (var targetPoint in target.MapPoints)
                {
                    dist += Math.Abs(mapPoint.Degree - targetPoint.Degree);
                    dist += Math.Abs(mapPoint.Distance - targetPoint.Distance);
                }
            }

            // TODO: Implement different map point sizes
            /*
            if (target.MapPoints.Count < this.MapPoints.Count)
            {
                throw new NotImplementedException("target had too few map points");
            }
            else if (target.MapPoints.Count > this.MapPoints.Count)
            {
                throw new NotImplementedException("target had too many map points");
            }
            for (var i = 0; i < this.MapPoints.Count; i++)
            {
                throw new NotImplementedException("actually not yet implemented");
            }*/

            return dist;
        }

        protected override double CalculateDistanceToFeasibility()
        {
            // TODO: Fix min/max distance & degree

            var maxDistance = 1.0;
            var maxDegree = 180;
            var minDegree = 0;
            var minDistance = 0;

            var distance = 0.0;

            foreach (var mp in this.MapPoints)
            {
                // Degree distance
                if (mp.Degree > maxDegree)
                {
                    distance += mp.Degree - maxDegree;
                }
                else if (mp.Degree < minDegree)
                {
                    distance += maxDegree - mp.Degree;
                }

                // Distance distance!
                if (mp.Distance > maxDistance)
                {
                    distance += mp.Distance - maxDistance;
                }
                else if (mp.Degree < minDistance)
                {
                    distance += maxDistance - mp.Distance;
                }
            }

            this.DistanceToFeasibility = distance;

            return DistanceToFeasibility;
        }

        public MapPhenotype ConvertToPhenotype(int xSize, int ySize)
        {
            var map = new MapPhenotype(xSize, ySize);

            foreach (var mp in this.MapPoints)
            {
                var maxDistance = MaxDistanceAtDegree(xSize / 2.0, ySize / 2.0, mp.Degree);
                Console.WriteLine(maxDistance);
                var point = FindPoint(mp.Degree, maxDistance * mp.Distance);

                var xPos = point.Item1 + (xSize / 2.0);
                var yPos = point.Item2 + (ySize / 2.0);

                switch (mp.Type)
                {
                    case Enums.MapPointType.Base:
                        map.MapItems[(int)yPos, (int)xPos] = Enums.Item.BlueMinerals;
                        break;
                    case Enums.MapPointType.GoldBase:
                        map.MapItems[(int)yPos, (int)xPos] = Enums.Item.GoldMinerals;
                        break;
                    case Enums.MapPointType.StartBase:
                        map.MapItems[(int)yPos, (int)xPos] = Enums.Item.Base;
                        break;
                    case Enums.MapPointType.XelNagaTower:
                        map.MapItems[(int)yPos, (int)xPos] = Enums.Item.XelNagaTower;
                        break;
                    default:
                        map.MapItems[(int)yPos, (int)xPos] = Enums.Item.DestructibleRocks;
                        break;
                }
            }

            return map;
        }

        private static double MaxDistanceAtDegree(double xSize, double ySize, double degree)
        {
            var cSquared = Math.Pow(xSize, 2) + Math.Pow(ySize, 2);
            var maxDistance = Math.Sqrt(cSquared);
            var radians = ConvertToRadians(degree);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            var stop = false;

            do
            {
                // TODO: Optimize distance function calculation
                var point = FindPoint(maxDistance, cos, sin);

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

        private static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        private static Tuple<double, double> FindPoint(double degree, double distance)
        {
            var radians = ConvertToRadians(degree);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            return FindPoint(distance, cos, sin);
        }

        private static Tuple<double, double> FindPoint(double distance, double cos, double sin)
        {
            var xDist = cos * distance;
            var yDist = sin * distance;

            return new Tuple<double, double>(xDist, yDist);
        }

        public override string ToString()
        {
            var s = this.MapPoints.Aggregate("--------------------\n", (current, mp) => current + (mp.ToString() + "\n"));

            s += "--------------------";
            return s;
        }
    }
}
