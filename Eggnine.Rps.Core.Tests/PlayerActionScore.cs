using System;

namespace Eggnine.Rps.Core.Tests;

public class PlayerActionScore
{
    public PlayerActionScore(IRpsPlayer player, RpsAction action, long score)
    {
        Player = player;
        Action = action;
        Score = score;
    }
    public IRpsPlayer Player {get;}
    public RpsAction Action {get;}
    public long Score {get;}
}