using System.Threading.Tasks;

public class Player
{
    private Command? _command = null;

    public bool Paused => _command?.Paused ?? false;

    public async Task Play(string path)
    {
        if (_command != null && _command.Paused)
        {
            await _command.Resume();
        }

        if (_command == null)
        {
            _command = new Command($"ffplay -nodisp -loglevel error -autoexit {path}");
            _command.Start();
        }
    }

    public void Stop()
    {
        if (_command != null)
        {
            _command.Stop();
            _command = null;
        }
    }

    public async Task Pause()
    {
        if (_command != null)
        {
            await _command.Pause();
        }
    }
}
