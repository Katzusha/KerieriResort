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

namespace testroom
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public static string ErrorMessage;
        System.Media.SoundPlayer player;

        public ErrorWindow()
        {
            InitializeComponent();

            ErrorMessageOutput.Text = ErrorMessage;

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings["HaHaFunny"].Value == "true")
            {
                string[] randomerror = {"I have no clue what just went wrong. Just do the basic procedures and check things up like" +
            " like internet and stuf \nHere is some useles error code: C30034", "Something went wrong with the output. Please check all the inserted " +
            "data and try again.\nError code: C40012", "Opsie wipsie something went wrong!\nPlease check some useles internet stuf and go cry in ur room.\n" +
            "Here is the error code becouse I Will Not explain it to you: C42069", "Error.... \n\n Just error"};

                Random rand = new Random();
                int index = rand.Next(randomerror.Length);

                ErrorMessageOutput.Text = randomerror[index];

                player = new System.Media.SoundPlayer(@"D:\Projects\Kerieri\Kerieri\Sounds\errorsound.wav");
                player.Play();
            }
        }

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

        private void okbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
