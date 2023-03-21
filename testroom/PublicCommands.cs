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
        public static void ShowError(double errortype, string? screenname)
        {
            //ERROR CODE 0: Something went wrong when clearing values of the dysplay elements
            if (errortype == 0)
            {
                ErrorWindow.ErrorMessage = "Something went wrong when clearing displayed elements on the " + screenname + ". Please contact system support!";
            }
            //ERROR CODE 1: Something went wrong when clearing values of the input elements
            else if (errortype == 1)
            {
                ErrorWindow.ErrorMessage = "Something went wrong when clearing input elements on the " + screenname + ". Please contact system support!";
            }
            //ERROR CODE 1.1: Something went wrong when clearing values of the input elements
            else if (errortype == 1.1)
            {
                ErrorWindow.ErrorMessage = "Something went wrong when clearing date values on the " + screenname + ". Please contact system support!";
            }
            //ERROR CODE 2: Something went wrong when displaying elements
            else if (errortype == 2)
            {
                ErrorWindow.ErrorMessage = "Something went wrong. Please check your internet connection and try again. \n \nIf this error continues please contact system support!";
            }
            //ERROR CODE 3: Something went wrong when displaying elements
            else if (errortype == 3)
            {
                ErrorWindow.ErrorMessage = "Internal error. Please contact system support!";
            }

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
