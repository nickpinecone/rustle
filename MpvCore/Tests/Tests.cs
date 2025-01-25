using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FFAudio.Tests;

public class Tests
{
    [Test]
    public void CanPlayAudio()
    {
        var audio = new Audio("sample.wav");

        _ = Task.Run(() => audio.Play());
        Thread.Sleep(TimeSpan.FromSeconds(1));
        audio.Stop();

        Assert.Pass();
    }

    [Test]
    public async Task CanPauseAudio()
    {
        var audio = new Audio("sample.wav");

        _ = Task.Run(() => audio.Play());
        Thread.Sleep(TimeSpan.FromSeconds(1));
        await audio.Pause();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        await audio.Resume();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        audio.Stop();

        Assert.Pass();
    }

    [Test]
    public void CanReuseAudio()
    {
        var audio = new Audio("sample.wav");

        _ = Task.Run(() => audio.Play());
        Thread.Sleep(TimeSpan.FromSeconds(1));
        audio.Stop();

        _ = Task.Run(() => audio.Play());
        Thread.Sleep(TimeSpan.FromSeconds(1));
        audio.Stop();

        Assert.Pass();
    }
}
