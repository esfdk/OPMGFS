using System;

namespace OPMGFS.Map.MapObjects
{
    public class MapPoint
    {
        public MapPoint(double distance, double degree, Enums.MapPointType type)
        {
            Distance = distance;
            Degree = degree;
            Type = type;
        }

        public double Distance { get; private set; }

        public double Degree { get; private set; }

        public Enums.MapPointType Type { get; private set; }

        public MapPoint Mutate(Random r)
        {
            var maxDistMod = 10.0;
            var maxDegreeMod = 10.0;

            var positiveChange = r.Next(2);
            var newDistance = positiveChange == 1 ? this.Distance + (r.NextDouble() * maxDistMod) : this.Distance - (r.NextDouble() * maxDistMod);
            positiveChange = r.Next(2);
            var newDegree = positiveChange == 1 ? this.Degree + (r.NextDouble() * maxDegreeMod) : this.Degree - (r.NextDouble() * maxDegreeMod);
            newDegree = (newDegree + 360)%360;
            return new MapPoint(newDistance, newDegree, this.Type);
        }

        public override string ToString()
        {
            return string.Format("Distance: {0}, Degree: {1}, Type: {2}", Distance, Degree, Type);
        }
    }
}
