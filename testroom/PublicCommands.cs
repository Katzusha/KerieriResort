using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testroom
{
    class PublicCommands
    {
        public static void ShowError(string errorcode)
        {
            ErrorWindow.ErrorMessage = errorcode;

            ErrorWindow window = new ErrorWindow();
            window.ShowDialog();
        }

        public static void UpdateAppSettingsValue(string key, string value) 
        {
            //Change appsettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save();

            //Refresh the file so it is updated firmly
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void CreateAppSettingsValue(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
