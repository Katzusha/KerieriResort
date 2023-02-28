using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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

namespace resorttestroom
{
    /// <summary>
    /// Interaction logic for DeletionWindow.xaml
    /// </summary>
    public partial class DeletionWindow : Window
    {
        public DeletionWindow()
        {
            InitializeComponent();
        }

        public static bool confirmed = false;

        //Enter and leave animations for red buttons
        #region RED BUTTON animations
        private void RedBtn_Enter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            //Animation for buttons size to make transformate it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            //Animations for buttons background color to transforme it from transparrent to red
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();

            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#00" + Resources["FalseBrush"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#FF" + Resources["FalseBrush"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            Cursor = Cursors.Hand;
        }
        private void RedBtn_Leave(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            //Animation for buttons size to make transformate it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            //Animations for buttons background color to transforme it from red back to transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#FF" + Resources["FalseBrush"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#00" + Resources["FalseBrush"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            Cursor = Cursors.Arrow;
        }
        #endregion

        //Enter and leave animations for blue buttons
        #region BLUE BUTTON animations
        private void BlueBtn_Enter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

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
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

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
        #endregion

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            confirmed = false;
            this.Close();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            confirmed = true;
            this.Close();
        }
    }
}
