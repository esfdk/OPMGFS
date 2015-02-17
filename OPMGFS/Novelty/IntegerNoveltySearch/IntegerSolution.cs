namespace OPMGFS.Novelty.IntegerNoveltySearch
{
    using System;
    using System.Collections.Generic;

    public class IntegerSolution : Solution
    {
        public IntegerSolution(int number)
        {
            this.Number = number;
        }

        public int Number { get; private set; }

        public override Solution Mutate(Random r)
        {
            var newValue = this.Number + (r.Next(20000) - 10000);
            return new IntegerSolution(newValue);
        }

        public override Solution Recombine(Solution other)
        {
            throw new NotImplementedException();
        }

        public override double CalculateNovelty(Population feasible, Population infeasible, NovelArchive archive, int numberOfNeighbours)
        {
            var distanceValues = new List<Tuple<double, int, int>>();

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

        protected override bool CheckFeasibility()
        {
            this.CalculateDistanceToFeasibility();
            if (this.DistanceToFeasibility <= 0)
            {
                this.isFeasibilityCalculated = true;
                this.IsFeasible = true;
                return true;
            }

            return false;
        }

        protected override double CalculateDistance(Solution other)
        {
            return Math.Abs(this.Number - ((IntegerSolution)other).Number);
        }

        protected override double CalculateDistanceToFeasibility()
        {
            this.DistanceToFeasibility = 0 - this.Number;

            return DistanceToFeasibility;
        }
    }
}
