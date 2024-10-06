
using System;
using System.Threading.Tasks;
using Eggnine.Rps.Core;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Range = Eggnine.Rps.Common.Range;

namespace Eggnine.Rps.Core.Tests;

[TestClass]
public class RpsEngineTests
{
    private IRpsEngine GetSystemUnderTest()
    {
        return new RpsEngine(new ConsoleLogger<RpsEngine>(), 5000);
    }

    private class ConsoleLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine(formatter(state, exception));
        }
    }

    [TestMethod]
    public async Task WhenTwoPlayersActWithDifferentActionsOneScoresOneAndOneScoresNegativeOne()
    {
        IRpsPlayer playerOne = A.Fake<IRpsPlayer>();
        A.CallTo(() => playerOne.Id).Returns(Guid.NewGuid());
        IRpsPlayer playerTwo = A.Fake<IRpsPlayer>();
        A.CallTo(() => playerTwo.Id).Returns(Guid.NewGuid());
        IRpsEngine rpsEngine = GetSystemUnderTest();
        await rpsEngine.ActAsync(playerOne, RpsAction.Rock);
        await rpsEngine.ActAsync(playerTwo, RpsAction.Scissors);
        long score = await rpsEngine.GetScoreAsync(playerOne);
        Assert.AreEqual(1, score, "PlayerOne Score");
        score = await rpsEngine.GetScoreAsync(playerTwo);
        Assert.AreEqual(-1, score, "PlayerTwo Score");
    }

    [TestMethod]
    [RpsEngineTestDataAttribute]
    public async Task WhenFivePlayersFiveTurnsAssertedScores(string testName, RpsEngineTestData testData)
    {
        Range range = new(0,4);
        IRpsEngine engine = GetSystemUnderTest();
        foreach(long turn in range)
        {
            await testData.PlayTurnAsync(turn, engine);
            await testData.AssertScoresAsync(turn, engine);
        }
    }
}