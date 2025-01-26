using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LeagueProxyLib;

internal sealed class RiotClient
{
    public RiotClient()
    {

    }

    public Process? Launch(string configServerUrl, IEnumerable<string>? args = null)
    {
        var path = GetPath();
        if (path is null)
            return null;

        IEnumerable<string> allArgs = [$"--client-config-url={configServerUrl}", "--launch-product=league_of_legends", "--launch-patchline=live", .. args ?? []];

        return Process.Start(path, allArgs);
    }

    private string? GetPath()
    {
        string installPath;

        if (OperatingSystem.IsMacOS())
        {
            installPath = "/Users/Shared/Riot Games/RiotClientInstalls.json";
        }
        else
        {
            installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                       "Riot Games/RiotClientInstalls.json");
        }

        if (File.Exists(installPath))
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText(installPath));
                var rcPaths = new List<string?>
                {
                    data?["rc_default"]?.ToString(),
                    data?["rc_live"]?.ToString(),
                    data?["rc_beta"]?.ToString()
                };

                var validPath = rcPaths.FirstOrDefault(File.Exists);
                if (validPath != null)
                    return validPath;
            }
            catch
            {
            }
        }

        if (OperatingSystem.IsMacOS())
        {
            return "/Users/Shared/Riot Games/Riot Client.app/Contents/MacOS/RiotClientServices";
        }
        else
        {
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
            {
                var potentialPath = Path.Combine(drive.RootDirectory.FullName, "Riot Games", "Riot Client", "RiotClientServices.exe");
                if (File.Exists(potentialPath))
                    return potentialPath;
            }
        }

        return null;
    }
}