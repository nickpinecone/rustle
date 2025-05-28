using System;
using System.Threading.Tasks;

namespace Rustle;

public static class Program
{
    public static async Task Main()
    {
        var player = new Mpv.Player();
        await player.Initialize();

        player.MediaTitleChange += (sender, title) =>
        {
            Console.WriteLine($"I got the change: {title}");
        };
        
        player.PlayError += (sender, error) =>
        {
            Console.WriteLine($"I got the error: {error}");
        };

        await player.PlayAsync("http://rdstream-0625.dez.ovh:8000/radio.mp3");

        var volume = await player.GetVolumeAsync();
        
        Console.WriteLine(volume);

        Console.Read();
    }
}