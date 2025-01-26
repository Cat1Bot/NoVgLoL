using EmbedIO;
using EmbedIO.WebApi;

namespace LeagueProxyLib;

internal sealed class ProxyServer<T> where T : WebApiController, new()
{
    private WebServer _WebServer;
    private int _Port;

    public string Url => $"http://127.0.0.1:{_Port}";

    public ProxyServer(int port)
    {
        _Port = port;

        _WebServer = new WebServer(o => o
                .WithUrlPrefix(Url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithWebApi("/", m => m
                    .WithController<T>()
                );
    }

    public void Start(CancellationToken cancellationToken = default)
    {
        _WebServer.Start(cancellationToken);
    }

    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        return _WebServer.RunAsync(cancellationToken);
    }
}
