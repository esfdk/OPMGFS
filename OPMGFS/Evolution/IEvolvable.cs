// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvolvable.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the IEvolvable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Evolution
{
    /// <summary>
    /// An interface used for objects that should be able to evolve.
    /// </summary>
    public abstract class IEvolvable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IEvolvable"/> class. 
        /// </summary>
        /// <param name="mutationChance">
        /// The chance of mutation happening.
        /// </param>
        protected IEvolvable(double mutationChance)
        {
            this.MutationChance = mutationChance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IEvolvable"/> class.
        /// Mutation chance is set to 30% by default.
        /// </summary>
        protected IEvolvable()
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
        /// <returns>The mutation of the object.</returns>
        public abstract IEvolvable SpawnMutation();

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
