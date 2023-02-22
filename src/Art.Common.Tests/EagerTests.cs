using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Art.Common.Tests;

public class EagerTests
{
    [Test]
    public async Task UnlimitedEager_CompletesFullyWithSynchronousResults()
    {
        var v = CounterAsync(5, 5, 0.003).EagerAsync();
        var v2 = v.GetAsyncEnumerator();
        await Task.Delay(TimeSpan.FromSeconds(0.5));
        AssertCounterSectionCompletedSynchronously(v2, 5, 5);
        Task<bool> tE = v2.MoveNextAsync().AsTask();
        Assert.That(tE.IsCompletedSuccessfully, Is.True);
        Assert.That(tE.Result, Is.EqualTo(false));
    }

    [Test]
    public async Task LimitedEager_PartiallyCompletesWithSynchronousResults()
    {
        const int initialLimit = 5;
        const double longDelaySeconds = 0.5;
        var v = CounterAsync(5, 15, 0.003).EagerAsync(initialLimit);
        var v2 = v.GetAsyncEnumerator();
        await Task.Delay(TimeSpan.FromSeconds(longDelaySeconds));
        int i = 5, max = 5 + 15;
        while (i < max)
        {
            int iterationCount = Math.Min(initialLimit, max - i);
            AssertCounterSectionCompletedSynchronously(v2, i, iterationCount);
            i += iterationCount;
            if (i == max)
            {
                break;
            }
            Task<bool> t2 = v2.MoveNextAsync().AsTask();
            Assert.That(t2.IsCompleted, Is.False);
            await t2;
            Assert.That(t2.IsCompletedSuccessfully, Is.True);
            Assert.That(t2.Result, Is.EqualTo(true));
            Assert.That(v2.Current, Is.EqualTo(i));
            i++;
            await Task.Delay(TimeSpan.FromSeconds(longDelaySeconds));
        }
        Task<bool> tE = v2.MoveNextAsync().AsTask();
        await tE;
        Assert.That(tE.IsCompletedSuccessfully, Is.True);
        Assert.That(tE.Result, Is.EqualTo(false));
    }

    private static void AssertCounterSectionCompletedSynchronously(IAsyncEnumerator<int> v2, int start, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Task<bool> t = v2.MoveNextAsync().AsTask();
            Assert.That(t.IsCompletedSuccessfully, Is.True);
            Assert.That(t.Result, Is.EqualTo(true));
            Assert.That(v2.Current, Is.EqualTo(start + i));
        }
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
