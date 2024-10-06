//  ©️ 2024 by RF At EggNine All Rights Reserved
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Range = Eggnine.Rps.Common.Range;
using System.Collections.Concurrent;

namespace Eggnine.Rps.Core
{
    public class RpsEngine : IRpsEngine, IDisposable
    {
        private readonly IDictionary<long, IDictionary<IRpsPlayer, RpsAction>> _playerActionsByTurn;
        private readonly ILogger _logger;
        private readonly long _turnTime;
        private long _turn;
        private long _turnTimer;
        private SemaphoreSlim _semaphore;
        private bool _disposed;
        public RpsEngine(ILogger<RpsEngine> logger, long turnTime = 5000,
            long turn = 0, IDictionary<long, IDictionary<IRpsPlayer, RpsAction>>? playerActionsByTurn = null)
        {
            _logger = logger;
            _playerActionsByTurn = playerActionsByTurn ?? new Dictionary<long, IDictionary<IRpsPlayer, RpsAction>>();
            _turn = turn;
            _turnTimer = 0;
            _turnTime = turnTime;
            _semaphore = new SemaphoreSlim(1);
        }
        public async Task ActAsync(IRpsPlayer player, RpsAction action, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            if (!action.Validate(false))
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
                if(_turnTimer > 0 && DateTime.UtcNow.Ticks > _turnTimer + _turnTime)
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

        public async Task<long> GetScoreAsync(IRpsPlayer player, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            _logger.LogTrace("Entering {MethodName}", nameof(GetScoreAsync));
            try
            {
                long turn = _turn;
                ConcurrentBag<long> scores = new();
                await Task.Run(() => new Range(0,turn).AsParallel().ForAll(async turn =>
                {
                    _logger.LogDebug("Getting Score for turn {Turn}", turn);
                    if (!TryGet(_playerActionsByTurn, turn, out IDictionary<IRpsPlayer,RpsAction> playerActions))
                    {
                        return;
                    }
                    if (!TryGet(playerActions, player, out RpsAction action))
                    {
                        return;
                    }
                    scores.Add(await GetScoreOnTurnAsync(turn, action, cancellationToken));
                }));
                return scores.Sum();
            }
            finally
            {
                _logger.LogTrace("Exiting {MethodName}", nameof(GetScoreAsync));
            }
        }

        public Task<long> GetScoreOnTurnAsync(long turn, RpsAction action, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            _logger.LogDebug("Getting Score on turn {Turn}", turn);
            if (!TryGet(_playerActionsByTurn, turn, out IDictionary<IRpsPlayer, RpsAction> playerActions))
            {
                return Task.FromResult(0L);
            }
            long rocks = playerActions.Values.LongCount(v => v.Equals(RpsAction.Rock));
            long scissors = playerActions.Values.LongCount(v => v.Equals(RpsAction.Scissors));
            long papers = playerActions.Values.LongCount(v => v.Equals(RpsAction.Paper));
            long score = action switch 
            {
                RpsAction.Rock => scissors - papers,
                RpsAction.Scissors => papers - rocks,
                RpsAction.Paper => rocks - scissors,
                _ => 0,
            };
            _logger.LogDebug("Score on turn {Turn} for action {Action} was {Score}", turn, action, score);
            return Task.FromResult(score);
        }

        public Task<long> GetTurnAsync(CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();
            return Task.FromResult(_turn);
        }

        public Task<long> GetActionsOnTurnAsync(long turn, RpsAction action, CancellationToken cancellationToken = default)
        {
            if(!action.Validate(false))
            {
                throw new ArgumentException($"Invalid action {action}");
            }
            if (!TryGet(_playerActionsByTurn, _turn, out IDictionary<IRpsPlayer, RpsAction> playerActions))
            {
                return Task.FromResult(0L);
            }
            return Task.FromResult(playerActions.Values.LongCount(v => v == action));
        }

        public Task<RpsAction> GetActionOnTurnAsync(long turn, IRpsPlayer player, CancellationToken cancellationToken = default)
        {
            if (!TryGet(_playerActionsByTurn, _turn, out IDictionary<IRpsPlayer, RpsAction> playerActions))
            {
                return Task.FromResult(RpsAction.None);
            }
            if (!TryGet(playerActions, player, out RpsAction action))
            {
                return Task.FromResult(RpsAction.None);
            }
            return Task.FromResult(action);
        }

        private bool TryGet(IDictionary<IRpsPlayer, RpsAction> dictionary, IRpsPlayer player, out RpsAction rpsAction)
        {
            if(dictionary.ContainsKey(player))
            {
                rpsAction = dictionary[player];
                return true;
            }
            rpsAction = RpsAction.None;
            return false;
        }
        private bool TryGet(IDictionary<long, IDictionary<IRpsPlayer, RpsAction>> dictionary, long turn, out IDictionary<IRpsPlayer,RpsAction> playerActions)
        {
            if(dictionary.ContainsKey(turn))
            {
                playerActions = dictionary[turn];
                return playerActions != null;
            }
            playerActions = new Dictionary<IRpsPlayer, RpsAction>();
            return false;
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
