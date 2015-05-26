// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Evolvable.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the Evolvable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Evolution
{
    using System;

    /// <summary>
    /// An interface used for objects that should be able to evolve.
    /// </summary>
    public abstract class Evolvable
    {
        /// <summary>
        /// Determines if the fitness has been calculated or not.
        /// </summary>
        private bool fitnessCalculated;

        /// <summary>
        /// The total fitness.
        /// </summary>
        private double totalFitness;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Evolvable"/> class. 
        /// </summary>
        /// <param name="mutationChance">
        /// The chance of mutation happening.
        /// </param>
        /// <param name="r">
        /// The random.
        /// </param>
        protected Evolvable(double mutationChance, Random r)
        {
            this.MutationChance = mutationChance;
            this.Random = r;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Evolvable"/> class.
        /// Mutation chance is set to 30% by default.
        /// </summary>
        protected Evolvable()
            : this(0.3, new Random())
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the fitness of the object (if CalculateFitness() has not been called, it will).
        /// </summary>
        public double Fitness
        {
            get
            {
                if (this.fitnessCalculated)
                    return this.totalFitness;

                this.CalculateFitness();
                this.fitnessCalculated = true;
                return this.totalFitness;
            }

            protected set
            {
                this.totalFitness = value;
                this.fitnessCalculated = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the fitness has been calculated or not.
        /// </summary>
        protected bool FitnessCalculated
        {
            get
            {
                return this.fitnessCalculated;
            }

            set
            {
                this.fitnessCalculated = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance of mutation happening.
        /// </summary>
        protected double MutationChance { get; set; }

        /// <summary>
        /// Gets or sets the random object for this evolvable.
        /// </summary>
        protected Random Random { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Spawns a mutation of the object.
        /// </summary>
        /// <returns>The mutation of the object.</returns>
        public abstract Evolvable SpawnMutation();

        /// <summary>
        /// Creates a recombination between this evolvable and other.
        /// </summary>
        /// <param name="other">The other evolvable to create a recombination with.</param>
        /// <returns>A recombination between this evolvable and other.</returns>
        public abstract Evolvable SpawnRecombination(Evolvable other);

        /// <summary>
        /// Calculates the fitness of the object.
        /// </summary>
        public abstract void CalculateFitness();

        /// <summary>
        /// Initializes the object, setting its values.
        /// </summary>
        public abstract void InitializeObject();

        #endregion
    }
}
