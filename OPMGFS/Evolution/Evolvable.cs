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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Evolvable"/> class. 
        /// </summary>
        /// <param name="mutationChance">
        /// The chance of mutation happening.
        /// </param>
        protected Evolvable(double mutationChance)
        {
            this.MutationChance = mutationChance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Evolvable"/> class.
        /// Mutation chance is set to 30% by default.
        /// </summary>
        protected Evolvable()
            : this(0.3)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the fitness of the object (CalculateFitness() must be called first)
        /// </summary>
        public double Fitness { get; protected set; }

        /// <summary>
        /// Gets or sets the chance of mutation happening.
        /// </summary>
        protected double MutationChance { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Spawns a mutation of the object.
        /// </summary>
        /// <param name="r">The random used to spawn the mutation.</param>
        /// <returns>The mutation of the object.</returns>
        public abstract Evolvable SpawnMutation(Random r);

        /// <summary>
        /// Creates a recombination between this evolvable and other.
        /// </summary>
        /// <param name="other">The other evolvable to create a recombination with.</param>
        /// <param name="r">The random used to perform the recombination.</param>
        /// <returns>A recombination between this evolvable and other.</returns>
        public abstract Evolvable SpawnRecombination(Evolvable other, Random r);

        /// <summary>
        /// Calculates the fitness of the object.
        /// </summary>
        public abstract void CalculateFitness();

        /// <summary>
        /// Initializes the object, setting its values.
        /// </summary>
        /// <param name="r">The random used to initialize the object.</param>
        public abstract void InitializeObjects(Random r);

        #endregion
    }
}
