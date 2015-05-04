﻿namespace OPMGFS.Evolution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using OPMGFS.Map;
    using OPMGFS.Map.MapObjects;

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

        public EvolvableMap(MapSearchOptions mso, double mutationChance, Random r, MapFitnessOptions mapFitnessOptions) : base(mutationChance, r)
        {
            this.MapFitnessOptions = mapFitnessOptions;
            this.MapSearchOptions = mso;
            this.convertedPhenotype = null;
            this.hasBeenConverted = false;
            this.MapPoints = new List<MapPoint>();
        }

        public EvolvableMap(MapSearchOptions mso, double mutationChance, Random r, MapFitnessOptions mapFitnessOptions, List<MapPoint> mapPoints)
            : this(mso, mutationChance, r, mapFitnessOptions)
        {
            this.MapPoints = mapPoints;
        }

        public MapSearchOptions MapSearchOptions { get; private set; }

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

        public MapFitnessOptions MapFitnessOptions { get; private set; }

        public override Evolvable SpawnMutation()
        {
            var newPoints = MapConversionHelper.MutateMapPoints(
                this.MapPoints,
                this.MutationChance,
                this.MapSearchOptions,
                this.Random);

            return new EvolvableMap(this.MapSearchOptions, this.MutationChance, this.Random, this.MapFitnessOptions, newPoints);
        }

        public override Evolvable SpawnRecombination(Evolvable other)
        {
            throw new NotImplementedException();
        }

        public override void CalculateFitness()
        {
            var mapFitness = new MapFitness(this.ConvertedPhenotype, this.MapFitnessOptions);

            this.Fitness = mapFitness.CalculateFitness();
        }

        public override void InitializeObject()
        {
            this.MapPoints = MapConversionHelper.GenerateInitialMapPoints(this.MapSearchOptions, this.Random);
        }

        /// <summary>
        /// Converts this individual to a map.
        /// </summary>
        /// <param name="xSize">
        /// The width of the map.
        /// </param>
        /// <param name="ySize">
        /// The height of the map.
        /// </param>
        /// <returns>
        /// The <see cref="MapPhenotype"/>.
        /// </returns>
        public MapPhenotype ConvertToPhenotype(int xSize, int ySize)
        {
            var map = new MapPhenotype(ySize, xSize);

            map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, new MapSearchOptions(map, MapSearchOptions));

            return map;
        }

        /// <summary>
        /// The convert to phenotype.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <returns>
        /// The <see cref="MapPhenotype"/>.
        /// </returns>
        public MapPhenotype ConvertToPhenotype(MapPhenotype map)
        {
            map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, new MapSearchOptions(map, this.MapSearchOptions));

            return map;
        }

        /// <summary>
        /// Converts this individual to a map.
        /// </summary>
        /// <returns>
        /// The <see cref="MapPhenotype"/>.
        /// </returns>
        public MapPhenotype ConvertToPhenotype()
        {
            var map = MapConversionHelper.ConvertToPhenotype(this.MapPoints, this.MapSearchOptions);
            this.ConvertedPhenotype = map.CreateFinishedMap(Enums.Half.Top, this.MapSearchOptions.MapCompletion);
            this.hasBeenConverted = true;
            return this.ConvertedPhenotype;
        }
    }
}
