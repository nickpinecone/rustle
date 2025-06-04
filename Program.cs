using System.Threading.Tasks;
using Terminal.Gui;

namespace Rustle;

public static class Program
{
    private static Label _trackName = null!;
    private static Mpv.Player _player = null!;

    private static async Task InitializePlayer()
    {
        await _player.Initialize();

        _player.MediaTitleChange += (sender, title) =>
        {
            Application.MainLoop.Invoke(() => { _trackName.Text = title; });
        };

        await _player.PlayAsync("http://rdstream-0625.dez.ovh:8000/radio.mp3");
    }

    public static async Task Main()
    {
        Application.Init();
        
        Application.Top.ColorScheme = new ColorScheme()
        {
            Normal = Application.Driver.MakeAttribute(Color.Black, Color.Black),
        };

        _trackName = new Label("Rustle")
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Height = 1,
        };
        
        Application.Top.Add(_trackName);

        _player = new Mpv.Player();
        _ = InitializePlayer();

        Application.Run();
        Application.Shutdown();

        await _player.DisposeAsync();
    }
}