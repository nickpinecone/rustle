using System.Threading.Tasks;

public static class Program
{
    public static async Task Main()
    {
        var player = new Player();

        await player.Play("~/Downloads/sample-15s.wav");
    }
}
