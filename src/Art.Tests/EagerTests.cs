using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Art.Tests;

public class EagerTests
{
    [Test]
    public async Task TestEager1Async()
    {
        var v = CounterAsync(10, 10, 0.003).EagerAsync();
        var v2 = v.GetAsyncEnumerator();
        await Task.Delay(TimeSpan.FromSeconds(0.1));
        TestEager1(v2, 10, 10);
    }

    private static void TestEager1(IAsyncEnumerator<int> v2, int start, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Task<bool> t = v2.MoveNextAsync().AsTask();
            Assert.That(t.IsCompletedSuccessfully, Is.True);
            Assert.That(t.Result, Is.EqualTo(true));
            Assert.That(v2.Current, Is.EqualTo(start + i));
        }
        Task<bool> tE = v2.MoveNextAsync().AsTask();
        Assert.That(tE.IsCompletedSuccessfully, Is.True);
        Assert.That(tE.Result, Is.EqualTo(false));
    }

    private static async IAsyncEnumerable<int> CounterAsync(int start, int count, double delay)
    {
        foreach (int value in Enumerable.Range(start, count))
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            yield return value;
        }
    }
}
