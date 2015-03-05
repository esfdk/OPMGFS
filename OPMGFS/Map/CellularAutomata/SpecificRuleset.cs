// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ruleset.cs" company="Derps">
//   jmel & jcgr
// </copyright>
// <summary>
//   Defines the MapPhenotype type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OPMGFS.Map.CellularAutomata
{
    using Position = System.Tuple<int, int>;

    /// <summary>
    /// The ruleset for the cellular automata.
    /// </summary>
    public class SpecificRuleset : Ruleset
    {
        /// <summary>
        /// Gets the next generation following the ruleset.
        /// </summary>
        /// <param name="map"> The map to progress. </param>
        /// <param name="caXStart"> The x start position of the cellular automata. </param>
        /// <param name="caXEnd"> The x end position of the cellular automata. </param>
        /// <param name="caYStart"> The y start position of the cellular automata. </param>
        /// <param name="caYEnd"> The y end position of the cellular automata. </param>
        /// <returns> A map representing the next generation. </returns>
        public override Enums.HeightLevel[,] NextGeneration(
            Enums.HeightLevel[,] map,
            int caXStart,
            int caXEnd,
            int caYStart,
            int caYEnd)
        {
            var tempMap = (Enums.HeightLevel[,])map.Clone();

            for (var y = caYStart; y < caYEnd; y++)
            {
                for (var x = caXStart; x < caXEnd; x++)
                {
                    if (this.NumberOfNeighboursOfType(x, y, map, Enums.HeightLevel.Height1) >= 5
                        && map[x, y] != Enums.HeightLevel.Height2)
                        tempMap[x, y] = Enums.HeightLevel.Height1;
                    else if (this.NumberOfNeighboursOfType(x, y, map, Enums.HeightLevel.Height2) >= 5)
                        tempMap[x, y] = Enums.HeightLevel.Height2;
                    else if (this.NumberOfNeighboursOfType(x, y, map, Enums.HeightLevel.Height1) >= 5
                            && this.NumberOfNeighboursOfType(x, y, map, Enums.HeightLevel.Height2) >= 2)
                        tempMap[x, y] = Enums.HeightLevel.Height2;
                    else if (this.NumberOfNeighboursOfType(x, y, map, Enums.HeightLevel.Height1) >= 7)
                        tempMap[x, y] = Enums.HeightLevel.Height1;
                }
            }

            return tempMap;
        }
    }
}
