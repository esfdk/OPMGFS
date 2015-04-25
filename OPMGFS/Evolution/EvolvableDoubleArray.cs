// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvolvableDoubleArray.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the EvolvableDoubleArray type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Evolution
{
    using System;
    using System.Linq;

    /// <summary>
    /// The evolvable double array (used for testing)
    /// </summary>
    public class EvolvableDoubleArray : Evolvable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvolvableDoubleArray"/> class. 
        /// </summary>
        /// <param name="mutationChance"> The chance of mutation happening. </param>
        /// <param name="r">The random.</param>
        public EvolvableDoubleArray(double mutationChance, Random r)
            : base(mutationChance, r)
        {
            this.Numbers = new double[10];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolvableDoubleArray"/> class. 
        /// Mutation chance is 30% by default.
        /// </summary>
        public EvolvableDoubleArray()
        {
            this.Numbers = new double[10];
        }

        /// <summary>
        /// Gets the array of numbers represented by this object.
        /// </summary>
        public double[] Numbers { get; private set; }

        /// <summary>
        /// Sets the numbers of the array to the given numbers.
        /// </summary>
        /// <param name="newNumbers">The new numbers of the array.</param>
        public void SetNumbers(double[] newNumbers)
        {
            this.Numbers = newNumbers;
        }

        /// <summary>
        /// Spawns a mutation of this object.
        /// </summary>
        /// <returns>A mutation of this object.</returns>
        public override Evolvable SpawnMutation()
        {
            var tempDArray = new EvolvableDoubleArray(this.MutationChance, this.Random);
            var tempArray = new double[10];

            for (var i = 0; i < this.Numbers.Length; i++)
            {
                tempArray[i] = this.Numbers[i];

                if (this.Random.NextDouble() > this.MutationChance)
                    continue;

                tempArray[i] += this.Random.NextDouble() - 0.5;
            }

            tempDArray.SetNumbers(tempArray);

            return tempDArray;
        }

        /// <summary>
        /// Creates a recombination between this object and other.
        /// </summary>
        /// <param name="other">The object to recombine with this object. Must be an EvolvableDoubleArray.</param>
        /// <returns>A recombination between the two objects.</returns>
        public override Evolvable SpawnRecombination(Evolvable other)
        {
            if (other.GetType() != this.GetType()) return this;
            var tempOther = (EvolvableDoubleArray)other;

            var tempDArray = new EvolvableDoubleArray(this.MutationChance, this.Random);
            var tempArray = new double[this.Numbers.Length];

            for (int i = 0; i < this.Numbers.Length; i++)
            {
                tempArray[i] = ((this.Numbers[i] + tempOther.Numbers[i]) / 2) + (this.Random.NextDouble() - 0.5);
            }

            tempDArray.SetNumbers(tempArray);

            return tempDArray;
        }

        /// <summary>
        /// Calculates the fitness of the object.
        /// </summary>
        public override void CalculateFitness()
        {
            this.Fitness = this.Numbers.Sum();
        }

        /// <summary>
        /// Initializes the values of the double array
        /// </summary>
        public override void InitializeObject()
        {
            for (var i = 0; i < this.Numbers.Length; i++)
                this.Numbers[i] = this.Random.NextDouble() - 0.5;
        }
    }
}
