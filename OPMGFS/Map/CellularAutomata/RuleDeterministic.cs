// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleDeterministic.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPhenotype type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map.CellularAutomata
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class that represents a deterministic rule.
    /// </summary>
    public class RuleDeterministic : Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDeterministic"/> class. 
        /// </summary>
        public RuleDeterministic()
        {
            this.Deterministic = true;
            this.Self = null;
            this.Conditions = new List<Tuple<int, Enums.HeightLevel>>();
            this.TransformTo = Enums.HeightLevel.Height0;
        }

        /// <summary>
        /// Gets or sets the conditions the rule should fulfill in order to be applied.
        /// </summary>
        public List<Tuple<int, Enums.HeightLevel>> Conditions { get; set; }
    }
}
