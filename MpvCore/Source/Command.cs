using System.Diagnostics;

public class Command
{
    private Process? _process = null;

    public string Name { get; private set; }
    public bool Running { get; private set; }
    public bool Paused { get; private set; }

    public Command(string name)
    {
        Name = name;
    }

    public async void Start()
    {
    }

    public async void Stop()
    {
    }

    public async void Pause()
    {
    }

    public async void Resume()
    {
    }
}
