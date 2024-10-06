using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eggnine.Rps.Core.Tests;

public class RpsEngineTestData
{
    private readonly IDictionary<long, IEnumerable<PlayerActionScore>> _playerActionsAndScores;
    public RpsEngineTestData(IDictionary<long, IEnumerable<PlayerActionScore>> playerActionsAndScores)
    {
        _playerActionsAndScores = playerActionsAndScores;
    }

    public static RpsEngineTestData ScenarioOne
    {
        get
        {
            IRpsPlayer playerOne = A.Fake<IRpsPlayer>();
            Guid playerOneId = Guid.NewGuid();
            A.CallTo(() => playerOne.Id).Returns(playerOneId);
            IRpsPlayer playerTwo = A.Fake<IRpsPlayer>();
            Guid playerTwoId = Guid.NewGuid();
            A.CallTo(() => playerTwo.Id).Returns(playerTwoId);
            IRpsPlayer playerThree = A.Fake<IRpsPlayer>();
            Guid playerThreeId = Guid.NewGuid();
            A.CallTo(() => playerThree.Id).Returns(playerThreeId);
            IRpsPlayer playerFour = A.Fake<IRpsPlayer>();
            Guid playerFourId = Guid.NewGuid();
            A.CallTo(() => playerFour.Id).Returns(playerFourId);
            IRpsPlayer playerFive = A.Fake<IRpsPlayer>();
            Guid playerFiveId = Guid.NewGuid();
            A.CallTo(() => playerFive.Id).Returns(playerFiveId);
            return new RpsEngineTestData(new Dictionary<long, IEnumerable<PlayerActionScore>>()
            {
                {
                    0, new PlayerActionScore[]
                    {
                        new PlayerActionScore(playerOne, RpsAction.Rock, 3),
                        new PlayerActionScore(playerTwo, RpsAction.Rock, 3),
                        new PlayerActionScore(playerThree, RpsAction.Scissors, -2),
                        new PlayerActionScore(playerFour, RpsAction.Scissors, -2),
                        new PlayerActionScore(playerFive, RpsAction.Scissors, -2),
                    }
                },
                {
                    1, new PlayerActionScore[]
                    {
                        new PlayerActionScore(playerOne, RpsAction.Paper, 3 - 1),
                        new PlayerActionScore(playerTwo, RpsAction.Scissors, 3 + 1),
                        new PlayerActionScore(playerThree, RpsAction.Rock, -2 + 0),
                        new PlayerActionScore(playerFour, RpsAction.Paper, -2 + -1),
                        new PlayerActionScore(playerFive, RpsAction.Scissors, -2 + 1),
                    }
                },
                {
                    2, new PlayerActionScore[]
                    {
                        new PlayerActionScore(playerOne, RpsAction.Scissors, 3 - 1),
                        new PlayerActionScore(playerTwo, RpsAction.Scissors, 3 + 1),
                        new PlayerActionScore(playerThree, RpsAction.Scissors, -2 + 0),
                        new PlayerActionScore(playerFour, RpsAction.Scissors, -2 + -1),
                        new PlayerActionScore(playerFive, RpsAction.Scissors, -2 + 1),
                    }
                },
                {
                    3, new PlayerActionScore[]
                    {
                        new PlayerActionScore(playerOne, RpsAction.Paper, 3 - 1 + 1),
                        new PlayerActionScore(playerTwo, RpsAction.Paper, 3 + 1 + 1),
                        new PlayerActionScore(playerThree, RpsAction.Paper, -2 + 0 + 1),
                        new PlayerActionScore(playerFour, RpsAction.Paper, -2 + -1 + 1),
                        new PlayerActionScore(playerFive, RpsAction.Rock, -2 + 1 - 4),
                    }
                },
                {
                    4, new PlayerActionScore[]
                    {
                        new PlayerActionScore(playerOne, RpsAction.Paper, 3 - 1 + 1 + 0),
                        new PlayerActionScore(playerTwo, RpsAction.Rock, 3 + 1 + 1 + 1),
                        new PlayerActionScore(playerThree, RpsAction.Scissors, -2 + 0 + 1 - 1),
                        new PlayerActionScore(playerFour, RpsAction.Rock, -2 + -1 + 1 + 1),
                        new PlayerActionScore(playerFive, RpsAction.Scissors, -2 + 1 - 4 - 1),
                    }
                },
            });
        }
    }

    public IEnumerable<RpsEngineTestData> Scenarios()
    {
        yield return ScenarioOne;
    }

    public async Task PlayTurnAsync(long turn, IRpsEngine engine)
    {
        bool firstPass = true;
        foreach(PlayerActionScore playerActionScore in _playerActionsAndScores[turn])
        {
            if(firstPass)
            {
                firstPass = false;
                if(turn > 0)
                {
                    Assert.AreEqual(turn - 1, await engine.GetTurnAsync(), "turn has changed before it was expected");
                }
            }
            await engine.ActAsync(playerActionScore.Player, playerActionScore.Action);
            Assert.AreEqual(turn, await engine.GetTurnAsync(), "turn has changed");
        }
        await Task.Delay(1000);
    }

    public async Task AssertScoresAsync(long turn, IRpsEngine engine)
    {
        foreach(PlayerActionScore playerActionScore in _playerActionsAndScores[turn])
        {
            long actual = await engine.GetScoreAsync(playerActionScore.Player);
            Assert.AreEqual(playerActionScore.Score, actual,
                $"Player with id {playerActionScore.Player.Id} on turn {turn}, score was incorrect");
        }
    }
}