using System;
using System.Collections.Generic;
using System.Linq;
using OPMGFS.Map.MapObjects;

namespace OPMGFS.Novelty.MapNoveltySearch
{
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
            var maxDistMod = 10.0;
            var maxDegreeMod = 10.0;
            var maxDist = 128;
            var maxDegree = 180;

            // TODO: Figure out settings locations
            // TODO: Should ALL variables change when searching?
            // TODO: Should we limit mutation to be ONLY towards feasibility? (eg. when degree > maxDegree, only allow it to go lower?)
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

            var maxDistance = 128;
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
                else if (mp.Degree < minDegree)
                {
                    distance += maxDistance - mp.Distance;
                }
            }

            this.DistanceToFeasibility = distance;

            return DistanceToFeasibility;
        }

        public override string ToString()
        {
            var s = this.MapPoints.Aggregate("--------------------\n", (current, mp) => current + (mp.ToString() + "\n"));

            s += "--------------------";
            return s;
        }
    }
}
