using System;
using System.Collections.Generic;
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

        public ErrorWindow()
        {
            InitializeComponent();

            ErrorMessageOutput.Text = ErrorMessage;
        }

        #region ANIMATIONS
        private void BlueButton_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void BlueButton_Leave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        #endregion

        private void okbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
