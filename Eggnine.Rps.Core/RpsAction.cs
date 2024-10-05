//  ©️ 2024 by RF At EggNine All Rights Reserved
namespace Eggnine.Rps.Core
{

    /// <summary>
    /// Actions that can be taken in Rps
    /// </summary>
    public enum RpsAction
    {
        /// <summary>
        /// No action taken, the default?
        /// </summary>
        None = 0,
        /// <summary>
        /// Rock
        /// </summary>
        Rock,
        /// <summary>
        /// Paper
        /// </summary>
        Paper,
        /// <summary>
        /// Scissors
        /// </summary>
        Scissors,
        /// <summary>
        /// Random picks one of the other actions
        /// </summary>
        Random,
    }
}
