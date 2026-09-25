using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ProxyGuarder
{
    internal class AppConfig
    {
        public string TargetExe { get; set; }
        public List<string> Blacklist { get; set; }

        public AppConfig()
        {
            TargetExe = string.Empty;
            Blacklist = new List<string> { "jxufe-wifi" };
        }

        private static string ConfigDir
        {
            get
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ProxyGuarder");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        private static string ConfigFile
        {
            get { return Path.Combine(ConfigDir, "config.json"); }
        }

        public static AppConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    string json = File.ReadAllText(ConfigFile);
                    var serializer = new JavaScriptSerializer();
                    AppConfig config = serializer.Deserialize<AppConfig>(json);
                    if (config != null)
                    {
                        if (config.Blacklist == null)
                        {
                            config.Blacklist = new List<string>();
                        }
                        if (!config.Blacklist.Exists(x => string.Equals(x, "jxufe-wifi", StringComparison.OrdinalIgnoreCase)))
                        {
                            config.Blacklist.Insert(0, "jxufe-wifi");
                            config.Save();
                        }
                        if (string.IsNullOrWhiteSpace(config.TargetExe))
                        {
                            config.TargetExe = string.Empty;
                        }
                        return config;
                    }
                }
            }
            catch
            {
                // 配置损坏时回退到默认值
            }

            return new AppConfig();
        }

        public void Save()
        {
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(this);
            File.WriteAllText(ConfigFile, json, System.Text.Encoding.UTF8);
        }

        public bool IsBlacklisted(string networkName)
        {
            if (string.IsNullOrWhiteSpace(networkName))
            {
                return false;
            }

            foreach (string item in Blacklist)
            {
                if (!string.IsNullOrWhiteSpace(item) &&
                    string.Equals(item.Trim(), networkName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}




