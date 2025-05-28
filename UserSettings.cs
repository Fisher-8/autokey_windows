using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    public class UserSettings
    {
        public bool Use1 { get; set; } = true;
        public bool Use2 { get; set; } = true;
        public bool Use3 { get; set; } = true;
        public bool Use4 { get; set; } = true;
        public bool Use5 { get; set; } = false;
        public bool Use6 { get; set; } = false;
        public string Input1 { get; set; } = "1";
        public string Input2 { get; set; } = "2";
        public string Input3 { get; set; } = "3";
        public string Input4 { get; set; } = "4";
        public string Input5 { get; set; } = "5";
        public string Input6 { get; set; } = "6";
        public string Delay1 { get; set; } = "50";
        public string Delay2 { get; set; } = "50";
        public string Delay3 { get; set; } = "50";
        public string Delay4 { get; set; } = "50";
        public string Delay5 { get; set; } = "50";
        public string Delay6 { get; set; } = "50";
        public string RepeatDelay { get; set; } = "1000";
        public string HotKey { get; set; } = "E";

        private static string SettingsFilePath => 
          Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".config");

        public static UserSettings Load()
        {
            if(File.Exists(SettingsFilePath))
            {
                try
                {
                    using (var reader = new StreamReader(SettingsFilePath))
                    {
                        var serializer = new XmlSerializer(typeof(UserSettings));
                        return (UserSettings)serializer.Deserialize(reader);
                    }
                }
                catch
                {
                    return new UserSettings();
                }
            }
            else
            {
                return new UserSettings();
            }
        }

        public void Save()
        {
            using (var writer = new StreamWriter(SettingsFilePath))
            {
                var serializer = new XmlSerializer(typeof(UserSettings));
                serializer.Serialize(writer, this);
            }
        }
    }
}
