using Terminal.Gui;

namespace Riff;

public static class Program
{
    public static void Main()
    {
        Application.Init();

        var quitAction = () => Application.RequestStop();
        var quitItem = new MenuBarItem("Quit", "", quitAction);
        var menu = new MenuBar([quitItem]);

        Application.Top.Add(menu);
        Application.Run(Application.Top);

        Application.Shutdown();
    }
}
