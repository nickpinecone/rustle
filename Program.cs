using System;
using System.Threading.Tasks;
using Rustle.Library;

namespace Rustle;

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
        
        var title = await player.GetMediaTitleAsync();
        Console.WriteLine(title);
        
        await player.PlayAsync("http://rdstream-0625.dez.ovh:8000/radio.mp3");
        
        await player.SetVolumeAsync(50);

        volume = await player.GetVolumeAsync();
        Console.WriteLine(volume);
        
        isPaused = await player.GetPausedAsync();
        Console.WriteLine(isPaused);
        
        title = await player.GetMediaTitleAsync();
        Console.WriteLine(title);

        await player.WaitAsync();
    }
}