using Swan.Logging;
using System.Diagnostics;

namespace LeagueProxyLib;

public class LeagueProxy
{
    private ProxyServer<ConfigController> _ConfigServer;

    private RiotClient _RiotClient;
    private CancellationTokenSource? _ServerCTS;

    public LeagueProxyEvents Events => LeagueProxyEvents.Instance;

    public LeagueProxy()
    {
        _ConfigServer = new ProxyServer<ConfigController>(29150);

        _RiotClient = new RiotClient();
        _ServerCTS = null;
    }

    private void TerminateRiotServices()
    {
        string[] riotProcesses = { "RiotClientServices", "LeagueClient" };

        foreach (var processName in riotProcesses)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);

                foreach (var process in processes)
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error stopping {processName}, Please open issue on Github.");
                Console.ResetColor();
            }
        }
    }

    public void Start(out string configServerUrl)
    {
        if (_ServerCTS is not null)
            throw new Exception("Proxy servers are already running!");

        Logger.UnregisterLogger<ConsoleLogger>();
        TerminateRiotServices();
        _ServerCTS = new CancellationTokenSource();
        _ConfigServer.Start(_ServerCTS.Token);
        configServerUrl = _ConfigServer.Url;
    }

    public void Stop()
    {
        if (_ServerCTS is null)
            throw new Exception("Proxy servers are not running!");

        _ServerCTS.Cancel();
        _ServerCTS = null;
    }

    public Process? LaunchRCS(IEnumerable<string>? args = null)
    {
        if (_ServerCTS is null)
            throw new Exception("Proxy servers are not running!");

        return _RiotClient.Launch(_ConfigServer.Url, args);
    }

    public Process? StartAndLaunchRCS(IEnumerable<string>? args = null)
    {
        if (_ServerCTS is not null)
            throw new Exception("Proxy servers are already running!");

        Start(out _);
        return LaunchRCS(args);
    }
}
