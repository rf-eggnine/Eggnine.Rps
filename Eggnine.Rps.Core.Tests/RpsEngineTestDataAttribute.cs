using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eggnine.Rps.Core.Tests;

public class RpsEngineTestDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object[] {"Scenario One", RpsEngineTestData.ScenarioOne};
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        return data?[0]?.ToString() ?? "test name was null";
    }
}