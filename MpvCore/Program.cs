using System;
using System.Threading.Tasks;

namespace FFAudio;

public static class Program
{
    public static async Task Main()
    {
        var player = new FFPlayer("/usr/bin/ffplay");
        var recorder = new FFRecorder("/usr/bin/ffmpeg", "pulse");

        await player.Play("sample-15s.wav");
        await player.Wait();
    }
}
