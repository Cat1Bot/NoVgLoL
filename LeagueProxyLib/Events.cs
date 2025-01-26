using EmbedIO;

namespace LeagueProxyLib;

public sealed class LeagueProxyEvents
{
    public delegate string ProcessBasicEndpoint(string content, IHttpRequest request);

    public event ProcessBasicEndpoint? HookClientConfigPublic;
    public event ProcessBasicEndpoint? HookClientConfigPlayer;

    private static LeagueProxyEvents? _Instance = null;

    internal static LeagueProxyEvents Instance
    {
        get
        {
            _Instance ??= new LeagueProxyEvents();
            return _Instance;
        }
    }

    private LeagueProxyEvents()
    {
        HookClientConfigPublic = null;
        HookClientConfigPlayer = null;
    }

    private string InvokeProcessBasicEndpoint(ProcessBasicEndpoint? @event, string content, IHttpRequest? request)
    {
        if (@event is null)
            return content;

        foreach (var i in @event.GetInvocationList())
        {
            var result = i.DynamicInvoke(content, request); // Pass 'content' and 'request'
            if (result is not string resultString)
                throw new Exception("Return value of an event is not string!");

            content = resultString;
        }

        return content;
    }

    internal string InvokeClientConfigPublic(string content, IHttpRequest request) => InvokeProcessBasicEndpoint(HookClientConfigPublic, content, request);
    internal string InvokeClientConfigPlayer(string content, IHttpRequest request) => InvokeProcessBasicEndpoint(HookClientConfigPlayer, content, request);
}
