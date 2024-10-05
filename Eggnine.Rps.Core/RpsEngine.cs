//  ©️ 2024 by RF At EggNine All Rights Reserved
using System.Collections.Generic;
using System.Numerics;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Eggnine.Rps.Core
{
    public class RpsEngine : IRpsEngine, IDisposable
    {
        private readonly IDictionary<long, IDictionary<IRpsPlayer, RpsAction>> _playerActionsByTurn;
        private long _turn;
        private long _turnTimer;
        private SemaphoreSlim _semaphore;
        private bool _disposed;
        public RpsEngine()
        {
            _playerActionsByTurn = new Dictionary<long, IDictionary<IRpsPlayer, RpsAction>>();
            _turn = 0;
            _turnTimer = 0;
            _semaphore = new SemaphoreSlim(1);
        }
        public async Task ActAsync(IRpsPlayer player, RpsAction action, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            if (action != RpsAction.Rock
                && action != RpsAction.Scissors
                && action != RpsAction.Paper)
            {
                return;
            }
            if (player == null)
            {
                return;
            }
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if(_turnTimer > 0 && DateTime.UtcNow.Ticks > 5000)
                {
                    _turnTimer = 0;
                    _turn++;
                }
                cancellationToken.ThrowIfCancellationRequested();
                _playerActionsByTurn.TryAdd(_turn, new Dictionary<IRpsPlayer, RpsAction>());
                IDictionary<IRpsPlayer, RpsAction> playerActions = _playerActionsByTurn[_turn];
                playerActions.TryAdd(player, action);
                if(_turnTimer == 0 && playerActions.Keys.Count > 1)
                {
                    _turnTimer = DateTime.UtcNow.Ticks;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Task<long> GetScoreAsync(IRpsPlayer player, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            long turn = _turn;
            if (!TryGet(_playerActionsByTurn, turn, out IDictionary<IRpsPlayer,RpsAction> playerActions))
            {
                return Task.FromResult(0L);
            }
            if (!TryGet(playerActions, player, out RpsAction action))
            {
                return Task.FromResult(0L);
            }
            return GetScoreOnTurnAsync(turn, action, cancellationToken);
        }

        public Task<long> GetScoreOnTurnAsync(long turn, RpsAction action, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            if (!TryGet(_playerActionsByTurn, _turn, out IDictionary<IRpsPlayer, RpsAction> playerActions))
            {
                return Task.FromResult(0L);
            }
            int rocks = playerActions.Values.Count(v => v == RpsAction.Rock);
            int scissors = playerActions.Values.Count(v => v == RpsAction.Scissors);
            int papers = playerActions.Values.Count(v => v == RpsAction.Paper);
            return Task.FromResult((long)(action switch 
            {
                RpsAction.Rock => scissors - papers,
                RpsAction.Scissors => papers - rocks,
                RpsAction.Paper => rocks - scissors,
                _ => 0,
            }));
        }

        public Task<long> GetTurnAsync(CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            return Task.FromResult(_turn);
        }

        private bool TryGet(IDictionary<IRpsPlayer, RpsAction> dictionary, IRpsPlayer player, out RpsAction rpsAction)
        {
            try
            {
                rpsAction = dictionary[player];
                return true;
            }
            catch
            {
                rpsAction = RpsAction.None;
                return false;
            }
        }
        private bool TryGet(IDictionary<long, IDictionary<IRpsPlayer, RpsAction>> dictionary, long turn, out IDictionary<IRpsPlayer,RpsAction> playerActions)
        {
            try
            {
                playerActions = dictionary[turn];
                return playerActions != null;
            }
            catch
            {
                playerActions = new Dictionary<IRpsPlayer, RpsAction>();
                return false;
            }
        }

        private void CheckIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(RpsEngine));
            }
        }

        public void Dispose()
        {
            if(!_disposed)
            {
                _disposed = true;
                _semaphore.Dispose();
            }
        }
    }
}
