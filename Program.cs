using System.Threading.Tasks;
using Rayer.Library;

namespace Rayer;

public static class Program
{
    public static async Task Main()
    {
        var player = new MpvPlayer();

        await player.PlayAsync("http://rdstream-0625.dez.ovh:8000/radio.mp3");

        await Task.Delay(5000);

        await player.PauseAsync();
        
        await Task.Delay(5000);
        //
        // await player.ResumeAsync();
        //
        // await Task.Delay(2000);
        //
        // await player.StopAsync();
    }
}