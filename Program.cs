using System;
using System.Text.Json;
using System.Threading.Tasks;
using Rustle.Library;

namespace Rustle;

public static class Program
{
    public static async Task Main()
    {
        var player = new MpvPlayer();
        await player.Initialize();

        var some = new JsonElement();

        player.MediaTitleChange += (sender, title) =>
        {
            Console.WriteLine($"I got the change: {title}");
        };

        await player.PlayAsync("http://rdstream-0625.dez.ovh:8000/radio.mp3");

        var volume = await player.GetVolumeAsync();
        
        Console.WriteLine(volume);

        Console.Read();
    }
}