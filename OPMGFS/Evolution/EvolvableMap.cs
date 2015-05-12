namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;

    using OPMGFS.Map;
    using OPMGFS.Map.MapObjects;

    /// <summary>
    /// An individual (genome) in evolution of StarCraft maps.
    /// </summary>
    public class EvolvableMap : Evolvable
    {
        #region Fields
        /// <summary>
        /// The phenotype matching this solution.
        /// </summary>
        private MapPhenotype convertedPhenotype;

        /// <summary>
        /// Whether this solution has been converted to its phenotype.
        /// </summary>
        private bool hasBeenConverted;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolvableMap"/> class.
        /// </summary>
        /// <param name="mapSearchOptions"> The map search options.</param>
        /// <param name="mutationChance"> The mutation chance. </param>
        /// <param name="r"> The random object. </param>
        /// <param name="mapFitnessOptions"> The map fitness options. </param>
        public EvolvableMap(MapSearchOptions mapSearchOptions, double mutationChance, Random r, MapFitnessOptions mapFitnessOptions) : base(mutationChance, r)
        {
            this.MapFitnessOptions = mapFitnessOptions;
            this.MapSearchOptions = mapSearchOptions;
            this.convertedPhenotype = null;
            this.hasBeenConverted = false;
            this.MapPoints = new List<MapPoint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolvableMap"/> class.
        /// </summary>
        /// <param name="mapSearchOptions"> The map search options. </param>
        /// <param name="mutationChance"> The mutation chance. </param>
        /// <param name="r"> The random object. </param>
        /// <param name="mapFitnessOptions"> The map fitness options. </param>
        /// <param name="mapPoints"> The map points. </param>
        public EvolvableMap(MapSearchOptions mapSearchOptions, double mutationChance, Random r, MapFitnessOptions mapFitnessOptions, List<MapPoint> mapPoints)
            : this(mapSearchOptions, mutationChance, r, mapFitnessOptions)
        {
            this.MapPoints = mapPoints;
        }

        /// <summary>
        /// Gets the map search options for this individual.
        /// </summary>
        public MapSearchOptions MapSearchOptions { get; private set; }

        /// <summary>
        /// Gets the map points for this individual.
        /// </summary>
        public List<MapPoint> MapPoints { get; private set; }

        /// <summary>
        /// Gets the phenotype corresponding to this genotype.
        /// </summary>
        public MapPhenotype ConvertedPhenotype
        {
            get
            {
                return this.hasBeenConverted ? this.convertedPhenotype : this.ConvertToPhenotype();
            }

            private set
            {
                this.convertedPhenotype = value;
            }
        }

        /// <summary>
        /// Gets the map fitness options for this individual.
        /// </summary>
        public MapFitnessOptions MapFitnessOptions { get; private set; }

        /// <summary>
        /// Gets the map fitness values for this individual.
        /// </summary>
        public MapFitnessValues MapFitnessValues { get; private set; }

        /// <summary>
        /// Spawns a mutation of this individual.
        /// </summary>
        /// <returns>A newly mutated individual.</returns>
        public override Evolvable SpawnMutation()
        {
            var newPoints = MapConversionHelper.MutateMapPoints(
                this.MapPoints,
                this.MutationChance,
                this.MapSearchOptions,
                this.Random);

            return new EvolvableMap(this.MapSearchOptions, this.MutationChance, this.Random, this.MapFitnessOptions, newPoints);
        }

        /// <summary>
        /// Combines this individual with another to form a new individual.
        /// </summary>
        /// <param name="other">The individual to combine with.</param>
        /// <returns>The newly formed individual.</returns>
        public override Evolvable SpawnRecombination(Evolvable other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the fitness of this individual.
        /// </summary>
        public override void CalculateFitness()
        {
            var mapFitness = new MapFitness(this.ConvertedPhenotype, this.MapFitnessOptions);

            this.Fitness = mapFitness.CalculateFitness();
            this.MapFitnessValues = mapFitness.FitnessValues;
        }

        /// <summary>
        /// Initializes this individual.
        /// </summary>
        public override void InitializeObject()
        {
            this.MapPoints = MapConversionHelper.GenerateInitialMapPoints(this.MapSearchOptions, this.Random);
        }

        /// <summary>
        /// Converts this individual onto an empty map.
        /// </summary>
        /// <param name="xSize"> The width of the map. </param>
        /// <param name="ySize"> The height of the map. </param>
        /// <returns> The <see cref="MapPhenotype"/> that corresponds to this individual.</returns>
        public MapPhenotype ConvertToPhenotype(int xSize, int ySize)
        {
            var map = new MapPhenotype(ySize, xSize);

            map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, new MapSearchOptions(map, MapSearchOptions), this.Random);

            return map;
        }

        /// <summary>
        /// Converts this individual onto a specific map.
        /// </summary>
        /// <param name="map"> The map to convert this individual onto. </param>
        /// <returns> The <see cref="MapPhenotype"/> that corresponds to this individual.</returns>
        public MapPhenotype ConvertToPhenotype(MapPhenotype map)
        {
            map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, new MapSearchOptions(map, this.MapSearchOptions), this.Random);

            return map;
        }

        /// <summary>
        /// Converts this individual to a map based on the base map included in the map search options.
        /// </summary>
        /// <returns> The <see cref="MapPhenotype"/> that corresponds to this individual.</returns>
        public MapPhenotype ConvertToPhenotype()
        {
            var map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, this.MapSearchOptions, this.Random);
            this.ConvertedPhenotype = map.CreateFinishedMap(Enums.Half.Top, this.MapSearchOptions.MapCompletion);
            this.hasBeenConverted = true;
            return this.ConvertedPhenotype;
        }

        public bool IsDominatedBy(EvolvableMap other)
        {
            return this.MapFitnessValues.IsDominatedBy(other.MapFitnessValues);
        }

        public bool Dominates(EvolvableMap other)
        {
            return this.MapFitnessValues.Dominates(other.MapFitnessValues);
        }
    }
}
