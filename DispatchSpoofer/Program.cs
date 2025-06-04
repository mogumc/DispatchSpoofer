using System.Net;
using System.Net.Security;
using Newtonsoft.Json;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace DispatchSpoofer;

internal class ProxyService
{
	public readonly ProxyServer _webProxyServer;
	private readonly Logger _logger;
    private readonly string _webProxyHost;
	public static int _targetPort;
	private static readonly string[] toRedirectDomains =
	{
		".bhsr.com",
		".starrails.com",
		".hoyoverse.com",
		".mihoyo.com"
	};

	public ProxyService(string targetRedirectHost, int targetRedirectPort)
    {
        _logger = MainApp._logger;
		_webProxyHost = "127.0.0.1";
        _webProxyServer = new ProxyServer();
        _webProxyServer.CertificateManager.EnsureRootCertificate();

		_webProxyServer.BeforeRequest += BeforeRequest;
		_webProxyServer.BeforeResponse += OnBeforeResponse;
        _webProxyServer.ServerCertificateValidationCallback += OnCertValidation;

		_targetPort = Random.Shared.Next(10000, 60000);
        SetEndPoint(new ExplicitProxyEndPoint(IPAddress.Parse(_webProxyHost), _targetPort, true));

        _logger.LogInfo($"Starting proxy server at {_webProxyHost}:{_targetPort}.", true);
    }

	private void SetEndPoint(ExplicitProxyEndPoint explicitEP)
	{
		explicitEP.BeforeTunnelConnectRequest += BeforeTunnelConnectRequest;

		_webProxyServer.AddEndPoint(explicitEP);
		_webProxyServer.Start();

		// I hate this normal console output they force you to use
		TextWriter originalOut = Console.Out;
		Console.SetOut(TextWriter.Null);

		try
		{
			_webProxyServer.SetAsSystemHttpProxy(explicitEP);
			_webProxyServer.SetAsSystemHttpsProxy(explicitEP);
		}
		finally
		{
			Console.SetOut(originalOut);
		}

		_logger.LogInfo("Proxy set as system HTTP/HTTPS proxy on Windows.", true);
	}


	public void Shutdown()
	{
		_logger.LogWarning("Shutting down proxy server.");
		_webProxyServer!.Stop();
		_webProxyServer!.Dispose();
		_logger.LogSuccess("Proxy server shut down successfully.");
	}

	private Task BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs args)
	{
		string request = args.HttpClient.Request.RequestUri.ToString();

		_logger.LogInfo($"[RECV] {request}", true);

		args.DecryptSsl = request.Contains(".starrails.com") || request.Contains(".bhsr.com");

		return Task.CompletedTask;
	}

	private Task OnCertValidation(object sender, CertificateValidationEventArgs args)
    {
        if (args.SslPolicyErrors == SslPolicyErrors.None)
        {
			_logger.LogSuccess($"Certificate validation succeeded for: {args.Certificate.Subject}");
            args.IsValid = true;
        }
        else
        {
            _logger.LogError($"Certificate validation failed with errors: {args.SslPolicyErrors}");
        }

        return Task.CompletedTask;
    }

	private Task BeforeRequest(object sender, SessionEventArgs args)
	{
		string hostname = args.HttpClient.Request.RequestUri.Host;
		if (ShouldRedirect(hostname))
		{
			string requestUrl = args.HttpClient.Request.Url;
			Uri local = new Uri($"{MainApp._redirectIp}:{MainApp._redirectPort}/");

			string replacedUrl = new UriBuilder(requestUrl)
			{
				Scheme = local.Scheme,
				Host = local.Host,
				Port = local.Port
			}.Uri.ToString();

			_logger.LogSuccess("Redirecting: " + replacedUrl);
			args.HttpClient.Request.Url = replacedUrl;
		}

		return Task.CompletedTask;
	}

	private bool ShouldRedirect(string url)
	{
		if (toRedirectDomains.Any(d => url.EndsWith(d)))
			return true;
		return false;
	}

	private async Task OnBeforeResponse(object sender, SessionEventArgs args)
	{
		string hostname = args.HttpClient.Request.Url;

		if (hostname.Contains("/query_gateway"))
        {
			Console.WriteLine("gateway");
			string bodyAsStr = await args.GetResponseBodyAsString();
			byte[] body = Convert.FromBase64String(bodyAsStr);
			GatewayHelper helper = new GatewayHelper(body);
			args.SetResponseBodyString(Convert.ToBase64String(helper.Process()));
		}
    }
}

public class MainApp
{
	private static ProxyService? _webProxyServer;
	public static Logger _logger = new Logger("DispatchSpoofer");
	public static HotfixJson _hotfixJson = JsonConvert.DeserializeObject<HotfixJson>(File.ReadAllText("hotfix.json"))!;
	public static readonly bool _doredirect = true;
	public static readonly string _redirectIp = "http://127.0.0.1";
	public static readonly int _redirectPort = 443;

	private static bool _shuttingDown = false;

	private static void Main(string[] args)
	{
		Console.Title = "DispatchSpoofer - TrainGame";
		_webProxyServer = new ProxyService("127.0.0.1", 8888);

		AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
		Console.CancelKeyPress += OnCancelKeyPress;

		Thread.Sleep(-1);
	}

	private static void OnProcessExit(object? sender, EventArgs e)
	{
		ShutdownSafely();
	}

	private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
	{
		e.Cancel = true; // prevent immediate termination
		ShutdownSafely();
	}

	private static void ShutdownSafely()
	{
		if (_shuttingDown) return;
		_shuttingDown = true;

		_logger.LogWarning("Shutdown signal received. Cleaning up...");
		_webProxyServer!.Shutdown();
		_logger.LogSuccess("Shutdown complete.");

        // turn system proxy off, to ensure safety
        NativeProxyHelper.DisableSystemProxy();


		Environment.Exit(0);
	}
}

public class HotfixJson
{
	public string assetBundleUrl = String.Empty;
	public string exResourceUrl = String.Empty;
	public string luaUrl = String.Empty;
	public string ifixUrl = String.Empty;
	public int customMdkResVersion => !string.IsNullOrEmpty(luaUrl) ? Int32.Parse(luaUrl.Split("output_").Last().Split("_").First()) : 0;
	public int customIfixVersion => !string.IsNullOrEmpty(ifixUrl) ? Int32.Parse(ifixUrl.Split("output_").Last().Split("_").First()) : 0;
}