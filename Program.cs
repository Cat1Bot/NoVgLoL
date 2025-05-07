using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NoVgLoL
{
    class Program
    {
        private static bool RemoveVanguard()
        {
            if (!OperatingSystem.IsWindows())
            {
                return true;
            }

            string vgkpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Riot Vanguard", "installer.exe");

            if (!File.Exists(vgkpath))
            {
                return true;
            }

            try
            {
                using var process = Process.Start(vgkpath, "--quiet");
                if (process != null)
                {
                    Console.WriteLine(" [INFO] Attempting to uninstall Vanguard...");

                    process.WaitForExit();
                    Thread.Sleep(5000);

                    for (int i = 0; i < 30; i++)
                    {
                        if (!File.Exists(vgkpath))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(" [OKAY] Vanguard uninstallation completed, starting League now...");
                            Console.ResetColor();
                            return true;
                        }

                        Thread.Sleep(1000);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(" [WARN] Vanguard uninstallation failed, try running this app as administrator.");
                    Console.ResetColor();
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" [ERROR] Failed to start Vanguard uninstallation process.");
                    Console.ResetColor();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" [ERROR] Vanguard uninstallation aborted: {ex}");
                Console.ResetColor();
                return false;
            }
        }

        public static async Task Main(string[] args)
        {
            var leagueProxy = new LeagueProxy();

            leagueProxy.Start();

            if (!RemoveVanguard())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [ERROR] Vanguard uninstall failed, try running app as administator.");
                Console.ResetColor();
                leagueProxy.Stop();
                return;
            }

            Console.WriteLine(" [INFO] Starting RCS process...");

            var process = leagueProxy.LaunchRCS(args);
            if (process is null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [ERROR] Failed to launch RCS process. Try running this app as administrator.");
                Console.ResetColor();
                leagueProxy.Stop();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [OKAY] Started RCS process.");
            Console.ResetColor();

            await process.WaitForExitAsync();
            Console.WriteLine(" [INFO] RCS process exited");
            leagueProxy.Stop();
        }
    }
}