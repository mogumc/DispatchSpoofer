using System.Runtime.InteropServices;

internal static class NativeProxyHelper
{
	private const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
	private const int INTERNET_OPTION_REFRESH = 37;

	[DllImport("wininet.dll", SetLastError = true)]
	private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

	public static void DisableSystemProxy()
	{
		if (!OperatingSystem.IsWindows())
		{
			Console.WriteLine("[NativeProxyHelper] Skipped disabling system proxy: not running on Windows.");
			return;
		}

		try
		{
			using Microsoft.Win32.RegistryKey? registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
				@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", writable: true);

			if (registry != null)
			{
				registry.SetValue("ProxyEnable", 0);
				registry.SetValue("ProxyServer", "", Microsoft.Win32.RegistryValueKind.String);
			}

			InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
			InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[NativeProxyHelper] Failed to disable system proxy: {ex.Message}");
		}
	}

}
