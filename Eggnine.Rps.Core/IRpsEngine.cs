//  ©️ 2024 by RF At EggNine All Rights Reserved
using System.Threading;
using System.Threading.Tasks;

namespace Eggnine.Rps.Core
{
    /// <summary>
    /// The Rps action engine
    /// </summary>
    public interface IRpsEngine
    {
        /// <summary>
        /// Takes an action from a player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ActAsync(IRpsPlayer player, RpsAction action, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a player's score
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetScoreAsync(IRpsPlayer player, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current turn counter
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetTurnAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the score of the action on the past turn
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetScoreOnTurnAsync(long turn, RpsAction action, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the number of the given action that were played on the given turn
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetActionsOnTurnAsync(long turn, RpsAction action, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the action taken by the given player on the given turn
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="player"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<RpsAction> GetActionOnTurnAsync(long turn, IRpsPlayer player, CancellationToken cancellationToken = default);
    }
}

