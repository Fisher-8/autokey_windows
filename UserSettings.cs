using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    public class UserSettings
    {
        public bool bUse1 { get; set; } = true;
        public bool bUse2 { get; set; } = true;
        public bool bUse3 { get; set; } = true;
        public bool bUse4 { get; set; } = true;
        public bool bUse5 { get; set; } = false;
        public bool bUse6 { get; set; } = false;
        public string sInput1 { get; set; } = "1";
        public string sInput2 { get; set; } = "2";
        public string sInput3 { get; set; } = "3";
        public string sInput4 { get; set; } = "4";
        public string sInput5 { get; set; } = "5";
        public string sInput6 { get; set; } = "6";
        public string sDelay1 { get; set; } = "50";
        public string sDelay2 { get; set; } = "50";
        public string sDelay3 { get; set; } = "50";
        public string sDelay4 { get; set; } = "50";
        public string sDelay5 { get; set; } = "50";
        public string sDelay6 { get; set; } = "50";
        public string sDelayRepeat { get; set; } = "1000";
        public string sHotKey { get; set; } = "E";
        public string sBehavior { get; set; } = "0";

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
