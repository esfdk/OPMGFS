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
    using System.Linq;

    /// <summary>
    /// The evolvable double array (used for testing)
    /// </summary>
    public class EvolvableDoubleArray : IEvolvable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvolvableDoubleArray"/> class. 
        /// </summary>
        /// <param name="mutationChance"> The chance of mutation happening. </param>
        public EvolvableDoubleArray(double mutationChance)
            : base(mutationChance)
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
        public override IEvolvable SpawnMutation()
        {
            var tempDArray = new EvolvableDoubleArray(this.MutationChance);
            var tempArray = new double[10];

            for (var i = 0; i < this.Numbers.Length; i++)
            {
                tempArray[i] = this.Numbers[i];

                if (Program.Random.NextDouble() > this.MutationChance)
                    continue;

                tempArray[i] += Program.Random.NextDouble() - 0.5;
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
                this.Numbers[i] = Program.Random.NextDouble() - 0.5;
        }
    }
}
