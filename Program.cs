using System;
using System.Threading.Tasks;
using Rayer.Library;

namespace Rayer;

public static class Program
{
    public static async Task Main()
    {
        var player = new MpvPlayer();

        await player.PlayAsync("http://rdstream-0625.dez.ovh:8000/radio.mp3");
        
        await player.SetVolumeAsync(50);

        var volume = await player.GetVolumeAsync();
        Console.WriteLine(volume);
        
        var isPaused = await player.GetPausedAsync();
        Console.WriteLine(isPaused);

        // await Task.Delay(5000);
        //
        // await player.PauseAsync();
        //
        // await Task.Delay(5000);
        //
        // await player.ResumeAsync();
        //
        // await Task.Delay(5000);
        //
        // await player.WaitAsync();
        //
        // await player.StopAsync();
    }
}