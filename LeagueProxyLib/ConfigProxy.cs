using System.Text;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using System.Net;
using System.IO.Compression;

namespace LeagueProxyLib;

internal sealed class ConfigController : WebApiController
{
    private static HttpClient _Client = new(new HttpClientHandler { UseCookies = false, UseProxy = false, Proxy = null });
    private const string BASE_URL = "https://clientconfig.rpg.riotgames.com";

    private static LeagueProxyEvents _Events => LeagueProxyEvents.Instance;

    [Route(HttpVerbs.Get, "/api/v1/config/public")]
    public async Task GetConfigPublic()
    {
        var response = await ClientConfig(HttpContext.Request);
        var content = await response.Content.ReadAsStringAsync();

        content = _Events.InvokeClientConfigPublic(content, HttpContext.Request);

        await SendResponse(response, content);
    }

    [Route(HttpVerbs.Get, "/api/v1/config/player")]
    public async Task GetConfigPlayer()
    {
        var response = await ClientConfig(HttpContext.Request);
        var content = await response.Content.ReadAsStringAsync();

        content = _Events.InvokeClientConfigPlayer(content, HttpContext.Request);

        await SendResponse(response, content);
    }

    private async Task<HttpResponseMessage> ClientConfig(IHttpRequest request)
    {
        var url = BASE_URL + request.RawUrl;

        using var message = new HttpRequestMessage(HttpMethod.Get, url);

        if (request.Headers["accept-encoding"] is not null)
            message.Headers.TryAddWithoutValidation("Accept-Encoding", request.Headers["accept-encoding"]);

        message.Headers.TryAddWithoutValidation("user-agent", request.Headers["user-agent"]);

        if (request.Headers["x-riot-entitlements-jwt"] is not null)
            message.Headers.TryAddWithoutValidation("X-Riot-Entitlements-JWT", request.Headers["x-riot-entitlements-jwt"]);

        if (request.Headers["authorization"] is not null)
            message.Headers.TryAddWithoutValidation("Authorization", request.Headers["authorization"]);

        if (request.Headers["x-riot-rso-identity-jwt"] is not null)
            message.Headers.TryAddWithoutValidation("X-Riot-RSO-Identity-JWT", request.Headers["x-riot-rso-identity-jwt"]);

        if (request.Headers["baggage"] is not null)
            message.Headers.TryAddWithoutValidation("baggage", request.Headers["baggage"]);

        if (request.Headers["traceparent"] is not null)
            message.Headers.TryAddWithoutValidation("traceparent", request.Headers["traceparent"]);

        message.Headers.TryAddWithoutValidation("Accept", "application/json");

        var response = await _Client.SendAsync(message);

        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
        {
            var originalContent = await response.Content.ReadAsStreamAsync();
            response.Content = new StreamContent(new GZipStream(originalContent, CompressionMode.Decompress));
        }

        return response;
    }

    private async Task SendResponse(HttpResponseMessage response, string content)
    {
        var responseBuffer = Encoding.UTF8.GetBytes(content);

        HttpContext.Response.SendChunked = false;
        HttpContext.Response.ContentType = "application/json";
        HttpContext.Response.ContentLength64 = responseBuffer.Length;
        HttpContext.Response.StatusCode = (int)response.StatusCode;

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Client config request Cloudflare blocked (403), please open issue on GitHub or contact c4t_bot on Discord");
            Console.ResetColor();
        }

        await HttpContext.Response.OutputStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
        HttpContext.Response.OutputStream.Close();
    }
}