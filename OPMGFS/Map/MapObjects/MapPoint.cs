namespace OPMGFS.Map.MapObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using OPMGFS.Novelty.MapNoveltySearch;

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
        /// <param name="wasPlaced">
        /// Whether the map point was placed in the phenotype during conversion.
        /// </param>
        public MapPoint(double distance, double degree, Enums.MapPointType type, Enums.WasPlaced wasPlaced)
        {
            this.Distance = distance;
            this.Degree = degree;
            this.Type = type;
            this.WasPlaced = wasPlaced;
        }

        /// <summary>
        /// Gets the distance from the center of the map to this point.
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        /// Gets the angle of the point from the center of the map.
        /// </summary>
        public double Degree { get; private set; }

        /// <summary>
        /// Gets the type of the point.
        /// </summary>
        public Enums.MapPointType Type { get; private set; }

        /// <summary>
        /// Gets or sets the enum indicating whether placement was successful.
        /// </summary>
        public Enums.WasPlaced WasPlaced { get; set; }

        /// <summary>
        /// Mutates the point into a new point of the same type, but with different distance and angle.
        /// </summary>
        /// <param name="r">
        /// The random generator to use for mutation.
        /// </param>
        /// <param name="mnso">The search options.</param>
        /// <returns>
        /// The <see cref="MapPoint"/> created by the mutation.
        /// </returns>
        public MapPoint Mutate(Random r, MapNoveltySearchOptions mnso)
        {
            var positiveChange = r.Next(2);
            var newDistance = positiveChange == 1 
                                ? this.Distance + (r.NextDouble() * mnso.MaximumDistanceModifier) 
                                : this.Distance - (r.NextDouble() * mnso.MaximumDistanceModifier);

            positiveChange = r.Next(2);
            var newDegree = positiveChange == 1
                                ? this.Degree + (r.NextDouble() * mnso.MaximumDegreeModifier)
                                : this.Degree - (r.NextDouble() * mnso.MaximumDegreeModifier);
            newDegree = (newDegree + 360) % 360;

            return new MapPoint(newDistance, newDegree, this.Type, Enums.WasPlaced.NotAttempted);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public override string ToString()
        {
            return string.Format("Distance: {0}, Degree: {1}, Type: {2}", this.Distance, this.Degree, this.Type);
        }
    }
}
