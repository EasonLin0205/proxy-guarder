using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProxyGuarder
{
    internal static class NetworkManager
    {
        public static List<string> GetConnectedNetworkNames()
        {
            var result = new List<string>();

            string ssid = GetWifiSsid();
            if (!string.IsNullOrWhiteSpace(ssid))
            {
                result.Add(ssid);
            }

            return result;
        }

        private static string GetWifiSsid()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show interfaces",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8
                };

                using (Process p = Process.Start(psi))
                {
                    string stdout = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    {
                        return null;
                    }

                    foreach (string raw in stdout.Split('\n'))
                    {
                        string line = raw.Trim();
                        if (line.StartsWith("SSID", StringComparison.OrdinalIgnoreCase))
                        {
                            int idx = line.IndexOf(':');
                            if (idx < 0)
                            {
                                idx = line.IndexOf('：');
                            }
                            if (idx >= 0)
                            {
                                return line.Substring(idx + 1).Trim();
                            }
                        }
                    }
                }
            }
            catch
            {
                // 获取失败时返回 null
            }

            return null;
        }
    }
}
