using jsreport.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Configuration = System.Configuration.Configuration;

namespace resorttestroom
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            CalculatePricePerNight.IsChecked = bool.Parse(configuration.AppSettings.Settings["CalculatePerNight"].Value);
            CalculatePricePerPearson.IsChecked = bool.Parse(configuration.AppSettings.Settings["CalculatePerPearson"].Value);
            CalculateUnderaged.IsChecked = bool.Parse(configuration.AppSettings.Settings["CalculateUnderaged"].Value);
            AgeLimit.Text = configuration.AppSettings.Settings["CalculateUnderagedAge"].Value;

            if (bool.Parse(configuration.AppSettings.Settings["CalculatePerPearson"].Value))
            {
                

                if (bool.Parse(configuration.AppSettings.Settings["CalculateUnderaged"].Value))
                {
                    
                }
                else
                {
                    AgeLimit.IsEnabled = false;
                    AgeLimitLabel.Foreground = Brushes.Gray;
                }
            }
            else
            {
                CalculateUnderaged.IsEnabled = false;
                CalculateUnderaged.Foreground = Brushes.Gray;
            }
        }

        private void BlueBtn_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            //Animation for buttons size to make transformate it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Hand;
        }
        private void BlueBtn_Leave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            //Animation for buttons size to make transformate it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Arrow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (CalculatePricePerNight.IsChecked == true)
            {
                configuration.AppSettings.Settings["CalculatePerNight"].Value = "true";
            }
            else
            {
                configuration.AppSettings.Settings["CalculatePerNight"].Value = "false";
            }

            if (CalculatePricePerPearson.IsChecked == true)
            {
                configuration.AppSettings.Settings["CalculatePerPearson"].Value = "true";
            }
            else
            {
                configuration.AppSettings.Settings["CalculatePerPearson"].Value = "false";
            }

            if (CalculateUnderaged.IsChecked == true)
            {
                configuration.AppSettings.Settings["CalculateUnderaged"].Value = "true";
            }
            else
            {
                configuration.AppSettings.Settings["CalculateUnderaged"].Value = "false";
            }

            configuration.AppSettings.Settings["CalculateUnderagedAge"].Value = AgeLimit.Text;

            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");

            this.Close();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CalculatePricePerPearson_Checked(object sender, RoutedEventArgs e)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            CalculateUnderaged.IsEnabled = true;
            CalculateUnderaged.Foreground = Brushes.White;

            if (CalculateUnderaged.IsChecked == false)
            {
                AgeLimit.IsEnabled = false;
                AgeLimitLabel.Foreground = Brushes.Gray;
            }
            else
            {
                AgeLimit.IsEnabled = true;
                AgeLimitLabel.Foreground = Brushes.White;
            }
        }

        private void CalculateUnderaged_Checked(object sender, RoutedEventArgs e)
        {
            AgeLimit.IsEnabled = true;
            AgeLimitLabel.Foreground = Brushes.White;
        }

        private void CalculatePricePerPearson_Unchecked(object sender, RoutedEventArgs e)
        {
            CalculateUnderaged.IsEnabled = false;
            CalculateUnderaged.Foreground = Brushes.Gray;
            AgeLimitLabel.Foreground = Brushes.Gray;
            AgeLimit.IsEnabled = false;
        }

        private void CalculateUnderaged_Unchecked(object sender, RoutedEventArgs e)
        {
            AgeLimit.IsEnabled = false;
            AgeLimitLabel.Foreground = Brushes.Gray;
        }
    }
}
