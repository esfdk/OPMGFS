namespace OPMGFS.Map.MapObjects
{
    using System;

    /// <summary>
    /// A point on the map described as percentage of distance to edge at a specific angle.
    /// </summary>
    public class MapPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapPoint"/> class.
        /// </summary>
        /// <param name="distance">
        /// The distance in percent.
        /// </param>
        /// <param name="degree">
        /// The degree.
        /// </param>
        /// <param name="type">
        /// The type of the point.
        /// </param>
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
            var maxDistMod = 0.05;
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
