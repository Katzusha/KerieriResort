using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using resorttestroom;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace testroom
{
    public partial class MainWindow : Window
    {
        #region PUBLIC VALUES AND COMMANDS
        public static MySqlConnection conn = new MySqlConnection("server=152.89.234.190;user=kerieri_Client;database=kerieri_Clients;port=3306;" +
                    "password=keriericlient123");
        public static MySqlCommand cmd;

        public static string ClassifficationType = string.Empty;

        public static string DatabaseName = "";

        public static string Username = "";

        public static string APIconnection = "https://kerieri.eu/API";
        #endregion

        //Encryption and Decryption is use from external source becouse of the combination of C# encryption/decryption and php encryption/decryption
        #region ENCRYPTION AND DECRYPTION
        public string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            // Convert the plainText string into a byte array
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Encrypt the input plaintext string
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);

            // Complete the encryption process
            cryptoStream.FlushFinalBlock();

            // Convert the encrypted data from a MemoryStream to a byte array
            byte[] cipherBytes = memoryStream.ToArray();

            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();

            // Convert the encrypted byte array to a base64 encoded string
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

            // Return the encrypted data as a string
            return cipherText;
        }

        public string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;
        }

        public string Encryption(string encrypt)
        {
            string password = "3sc3RLrpd17";

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            string encrypted = this.EncryptString(encrypt, key, iv);

            return encrypted;
        }

        public string Decryption(string decrypt)
        {
            string password = "3sc3RLrpd17";

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            string decrypted = this.DecryptString(decrypt, key, iv);

            return decrypted;
        }
        #endregion


        public MainWindow()
        {
            //When application start we need to make sure that the login window is the first window that we show regardles of programming activity
            InitializeComponent();

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en-US");

            ControlGrid.Visibility = Visibility.Hidden;
            LogInScreen.Visibility = Visibility.Visible;

            ReservationsScreen.Visibility = Visibility.Visible;
            HomeGridNoResultsLabel.Visibility = Visibility.Hidden;
            ClassifficationScreen.Visibility = Visibility.Hidden;
            CreateClassifficationScreen.Visibility = Visibility.Hidden;
            CreateReservationScreen.Visibility = Visibility.Hidden;
            SettingsScreen.Visibility = Visibility.Hidden;
            ReservationGridSpanSelection.SelectedIndex = 0;
            DashboardScreen.Visibility = Visibility.Hidden;

            StartClock();

            LoadingAnimationGrid.Visibility = Visibility.Hidden;

            var doubleanimation = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(2)));
            var rotatetransform = new RotateTransform();
            LoadingSpinner.RenderTransform = rotatetransform;
            LoadingSpinner.RenderTransformOrigin = new Point(0.5, 0.5);
            doubleanimation.RepeatBehavior = RepeatBehavior.Forever;
            rotatetransform.BeginAnimation(RotateTransform.AngleProperty, doubleanimation);

            CreateReservationGridMainReservantBirthCalendar.BlackoutDates.Add(new CalendarDateRange((DateTime)DateTime.Now.AddYears(-18), new DateTime(2100, 12, 31)));
            CreateReservationGridMainReservantBirthCalendar.DisplayDate = DateTime.Now.AddYears(-18);
        }

        //In bottom left corner Update commands for date and time
        #region Clock and Date
        private void StartClock()
        {
            //Declare new clock
            DispatcherTimer timer = new DispatcherTimer();

            //Update clock every 5 seconds
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            //Update clock and date
            MenuClock.Content = DateTime.Now.ToString("HH:mm");
            MenuDate.Content = DateTime.Now.ToString("dd.MM.yyyy");
        }
        #endregion


        #region PUBLIC COMMANDS
        //Public command to reset all variables and grid children
        public void ClearAll()
        {
            try
            {
                loginusernameinput.Clear();
                loginpasswordinput.Clear();

                HomeGridScrollViewer.Children.Clear();
                HomeGridScrollViewer.RowDefinitions.Clear();

                ClassifficaitonsGridScrollViewer.Children.Clear();
                ClassifficaitonsGridScrollViewer.RowDefinitions.Clear();

                CreateReservationGridClassifficationCombobox.Items.Clear();

                try
                {
                    CreateReservationGridFromDateCalendar.BlackoutDates.Clear();
                    CreateReservationGridFromDateCalendar.SelectedDates.Clear();
                    CreateReservationGridFromDateCalendar.DisplayDate = DateTime.Today;
                }
                catch { }
                try
                {
                    CreateReservationGridToDateCalendar.BlackoutDates.Clear();
                    CreateReservationGridToDateCalendar.SelectedDates.Clear();
                    CreateReservationGridToDateCalendar.DisplayDate = DateTime.Today;
                }
                catch { }

                CreateReservationGridAvailableEssentialsGrid.Children.Clear();
                CreateReservationGridAvailableEssentialsGrid.RowDefinitions.Clear();

                CreateReservationGridMainGuestFirstnameInput.Clear();
                CreateReservationGridMainGuestSurnameInput.Clear();
                CreateReservationGridMainReservantBirthCalendar.SelectedDates.Clear();
                CreateReservationGridMainReservantBirthCalendar.DisplayDate = DateTime.Today;
                CreateReservationGridMainGuestEmailInput.Clear();
                CreateReservationGridMainGuestPhoneNumberInput.Clear();
                CreateReservationGridMainGuestCountryInput.Clear();
                CreateReservationGridMainGuestPostNumberInput.Clear();
                CreateReservationGridMainGuestCityInput.Clear();
                CreateReservationGridMainGuestAddressInput.Clear();
                CreateReservationGridMainGuestCertifiedNumberInput.Clear();
                CreateReservationGridSideGuestAddedGrid.Children.Clear();
                CreateReservationGridSideGuestAddedGrid.RowDefinitions.Clear();
                CreateReservationGridSideGuestFirstnameInput.Clear();
                CreateReservationGridSideGuestSurnameInput.Clear();
                CreateReservationGridSideGuestBirthCalendar.SelectedDates.Clear();
                CreateReservationGridSideGuestBirthCalendar.DisplayDate = DateTime.Today;
                CreateReservationGridMainReservantBirthCalendar.DisplayDate = DateTime.Now.AddYears(-18);
                CreateReservationGridPaymentInformationCreditCardCheckBox.IsChecked = false;
                CreateReservationGridPaymentInformationCashCheckBox.IsChecked = false;
                CreateReservationGridPaymentinfoTaxinput.Clear();
                CreateReservationGridPaymentInformationPdfPreview.CloseDocument();
                CreateReservationGridPaymentInformationPriceInput.Clear();
                CreateReservationGridPaymentInformationCommentInput.Clear();

                CreateReservationGridClassifficationLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridFromDateLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridToDateLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestEmaillabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestPhoneNumberlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestCountrylabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestAddresslabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestPostNumberlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestCitylabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridMainGuestCertifiedNumberlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridSideGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridSideGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                CreateReservationGridSideGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];


                CreateClassifficationGridMaxReservantsInput.Clear();
                CreateClassifficationGridNameInput.Clear();
                CreateClassifficationGridPriceInput.Clear();
                CreateClassifficationGridSerialNumberInput.Clear();
                CreateClassifficationGridSizeInput.Clear();
                CreateClassifficationGridIncludedEssentials.Children.Clear();
                CreateClassifficationGridIncludedEssentials.RowDefinitions.Clear();

                DashboardClassifficationsGrid.Children.Clear();
                DashboardClassifficationsGrid.RowDefinitions.Clear();
                DashboardClassifficationsGrid.ColumnDefinitions.Clear();
                DashboardClassifficationsGridRows.RowDefinitions.Clear();
                DashboardClassifficationsGridRows.Children.Clear();
                DashboardClassifficationsGridColumns.ColumnDefinitions.Clear();
                DashboardClassifficationsGridColumns.Children.Clear();

                DashboardScreenAvailableCapacityLabel.Content = "0.00%";
                DashboardScreenAvailableClassifficationsLabel.Content = "00";
                DashboardScreenUnavailableCapacityLabel.Content = "0.00%";
                DashboardScreenUnavailableClassifficationsLabel.Content = "00";

                DashboardScreenClassifficationsGridScrollViewer.ScrollToVerticalOffset(Top);
                DashboardScreenClassifficationsGridScrollViewer.ScrollToHorizontalOffset(Left);
            }
            catch
            {
                PublicCommands.ShowError("Something went wrong. Please contact system support.");
            }
        }

        private void PreviewNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PreviewDecimal(object sender, TextCompositionEventArgs e)
        {
            
        }

        //Becouse of all the animations, instead of using normal commands, we need to use tasks becouse they execute in the background
        #region PUBLIC TASKS

        #region Reservations
        

        //Generate children for CreateReservation combobox

        #endregion

        #endregion

        #endregion


        #region ANIMATIONS
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

        //Enter and leave animations for green buttons
        #region GREEN BUTTON animations
        private void GreenBtn_Enter(object sender, MouseEventArgs e)
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

            //Animations for buttons background color to transforme it from transparrent to green
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(0, 0, 255, 0);
            myColorAnimation.To = Color.FromArgb(255, 0, 255, 0);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            Cursor = Cursors.Hand;
        }
        private void GreenBtn_Leave(object sender, MouseEventArgs e)
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

            //Animations for buttons background color to transforme it from green back to transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(255, 0, 255, 0);
            myColorAnimation.To = Color.FromArgb(0, 0, 255, 0);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            Cursor = Cursors.Arrow;
        }
        #endregion

        //Enter and leave animations for + buttons
        #region ADD BUTTON animations
        private void AddBtn_Enter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;

            Button button = (Button)sender;

            //Animation for buttons size to transforme it to 100px width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 100;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Change its text from "+" to "Add"
            button.Content = "Add";
        }
        private void AddBtn_Leave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;

            Button button = (Button)sender;

            //Animation for buttons size to transforme it back to 50px width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Change its text from "Add" back to "+"
            button.Content = "+";
        }
        #endregion

        //Enter and leave animations for Settings btn
        #region SETTINGS BUTTON animations
        private void SettingsBtn_Enter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;

            Button button = (Button)sender;

            //Animation for buttons size to transforme it to 100px width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 120;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Change its text from "+" to "Add"
            button.Content = "Settings";
        }
        private void SettingsBtn_Leave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;

            Button button = (Button)sender;

            //Animation for buttons size to transforme it back to 50px width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Change its text from "Add" back to "+"
            button.Content = "⚙";
        }
        #endregion

        //Gotfocus and lostfocus animations for textboxes
        #region TEXTBOX INPUT animation
        private void NormalTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 450 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void NormalTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 450;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }

        private void MiniTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 120 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void MiniTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 120;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }

        private void MediumTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 300 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.05;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void MediumTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 300;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        #endregion

        //Gotfocus and lostfocus animations for passwordboxes
        #region PASWORDBOX INPUT animation
        private void PasswordBoxInput_GotFocus(object sender, RoutedEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            PasswordBox text = (PasswordBox)sender;

            //Animation for textboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 450 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.05;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void PasswordBoxInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            PasswordBox text = (PasswordBox)sender;

            //Animation for passwordboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 450;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }


        #endregion

        //Enter and leave animations for menu buttons
        #region MENU BUTTON animation
        private void MenuDashboardButton_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            //Animation for buttons size to transforme to 500px it's width
            button.Background = Brushes.Blue;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 470;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#00" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#FF" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "RESERVATIONS";

            Cursor = Cursors.Hand;
        }
        private void MenuDashboardButton_Leave(object sender, MouseEventArgs e)
        {

            Button button = (Button)sender;

            //Animation for buttons size to transforme it back to 150px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#FF" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#00" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "🗀";

            Cursor = Cursors.Arrow;
        }
        private void MenuHomeButton_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            //Animation for buttons size to transforme to 500px it's width
            button.Background = Brushes.Blue;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 420;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#00" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#FF" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "RESERVATIONS";

            Cursor = Cursors.Hand;
        }
        private void MenuHomeButton_Leave(object sender, MouseEventArgs e)
        {

            Button button = (Button)sender;

            //Animation for buttons size to transforme it back to 150px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#FF" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#00" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "🗀";

            Cursor = Cursors.Arrow;
        }
        private void MenuClassifficationsBtn_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            button.Background = Brushes.Blue;

            //Animation for buttons size to transforme to 500px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 620;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#00" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#FF" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "CLASSIFFICATIONS";

            Cursor = Cursors.Hand;
        }
        private void MenuClassifficationsBtn_Leave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            //Animation for buttons size to transforme it back to 150px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#FF" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#00" + Resources["MenuColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "🛏";

            Cursor = Cursors.Arrow;
        }
        private void MenuLogOutButton_Enter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            button.Background = Brushes.Blue;

            //Animation for buttons size to transforme to 300px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 380;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to red
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#00" + Resources["FalseColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#FF" + Resources["FalseColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "LogOut";

            Cursor = Cursors.Hand;
        }
        private void MenuLogOutButton_Leave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            //Animation for buttons size to transforme it back to 150px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to red
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = (Color)ColorConverter.ConvertFromString("#FF" + Resources["FalseColor"].ToString().Replace("#FF", ""));
            myColorAnimation.To = (Color)ColorConverter.ConvertFromString("#00" + Resources["FalseColor"].ToString().Replace("#FF", ""));
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            //button.Content = "☠";

            Cursor = Cursors.Arrow;
        }
        #endregion

        //Gotfocus, lostfocus, enter and leave animations for search boxes
        #region SEARCH BOX animations
        private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for buttons size to transforme to 500px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 500;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            text.Text = "";
        }
        private void SearchInput_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for buttons size to transforme back to 100px it's width
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 100;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            text.Text = "🔍";
        }
        private void HomeGridSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            TextBox text = (TextBox)sender;

            //Animation for searchboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myDoubleAnimation.To = 50 * 1.1;
            text.BeginAnimation(HeightProperty, myDoubleAnimation);
        }
        private void HomeGridSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            TextBox text = (TextBox)sender;

            //Animation for searchboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(HeightProperty, myDoubleAnimation);
        }
        #endregion

        //Enter and leave animations for settings buttons
        #region SETTINGS BUTTON animations
        private void HomeSettingsBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Hand;

            //Animation for buttons size to transforme button to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myDoubleAnimation.To = 50 * 1.1;
            button.BeginAnimation(HeightProperty, myDoubleAnimation);
        }
        private void HomeSettingsBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            //Animation for buttons size to transforme button back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Arrow;
        }
        #endregion

        //Enter and leave animations for generated buttons
        #region GENERATED BUTTON animations
        private void GeneratedCassifficationAndReservationBtn_Enter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Hand;

            //Animation for buttons size to transforme it for 5px bigger
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(0, 0, 0, 0);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it from kinda transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(127, 101, 101, 255);
            myColorAnimation.To = Color.FromArgb(255, 101, 101, 255);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        private void GeneratedCassifficationAndReservationBtn_Leave(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Arrow;

            //Animation for buttons size to transforme it for 5px smaller
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(10, 2, 10, 2);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it back from blue to kinda transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(255, 101, 101, 255);
            myColorAnimation.To = Color.FromArgb(127, 101, 101, 255);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        private void GeneratedAddedSideGuestBtn_Enter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Arrow;

            //Animation for buttons size to transforme it for 5px smaller
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(5, 5, 0, 5);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it back from blue to kinda transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(62, 62, 62);
            myColorAnimation.To = Color.FromRgb(101, 101, 255);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        private void GeneratedAddedSideGuestBtn_Leave(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Arrow;

            //Animation for buttons size to transforme it for 5px smaller
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(10, 10, 0, 10);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it back from blue to kinda transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(101, 101, 255);
            myColorAnimation.To = Color.FromRgb(62, 62, 62);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        private void GeneratedAddedSideGuestDeleteBtn_Enter(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Arrow;

            //Animation for buttons size to transforme it for 5px smaller
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(0, 5, 5, 5);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it back from blue to kinda transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(62, 62, 62);
            myColorAnimation.To = Color.FromRgb(245, 83, 83);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        private void GeneratedAddedSideGuestDeleteBtn_Leave(object sender, MouseEventArgs e)
        {
            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (bool.Parse(configuration.AppSettings.Settings["Animations"].Value) == false)
            {
                return;
            }

            Button button = (Button)sender;

            Cursor = Cursors.Arrow;

            //Animation for buttons size to transforme it for 5px smaller
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(0, 10, 10, 10);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it back from blue to kinda transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(245, 83, 83);
            myColorAnimation.To = Color.FromRgb(62, 62, 62);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        #endregion

        //Enter and leave animations for PDF preview buttons
        #region PDF PREVIEW animations
        //Enter and leave animations for preview load
        #region PREVIEW BUTTON animations
        private void PreviewBtn_Enter(object sender, MouseEventArgs e)
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
            myDoubleAnimation.To = 55;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 55;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Hand;
        }
        private void PreviewBtn_Leave(object sender, MouseEventArgs e)
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
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Arrow;
        }
        #endregion


        #region PRINT AND SAVE BUTTON animations
        private void SaveAndPrintBtn_Enter(object sender, MouseEventArgs e)
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
            myDoubleAnimation.To = 205;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 55;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Hand;
        }
        private void SaveAndPrintBtn_Leave(object sender, MouseEventArgs e)
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
            myDoubleAnimation.To = 200;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Arrow;
        }
        #endregion


        #region SAVE BUTTON animations
        private void SaveBtn_Enter(object sender, MouseEventArgs e)
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
            myDoubleAnimation.To = 125;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 55;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Hand;
        }
        private void SaveBtn_Leave(object sender, MouseEventArgs e)
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
            myDoubleAnimation.To = 120;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);

            Cursor = Cursors.Arrow;
        }
        #endregion
        #endregion

        //Loading animations for all times where application needs to proccess something
        #region LOADING animation
        public void LoadingAnimation()
        {
            //Animations for background lines to transforme it to black and white
            SolidColorBrush MyBrushLight = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(0, 0, 255);
            myColorAnimation.To = Color.FromRgb(150, 150, 150);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            MyBrushLight.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);

            SolidColorBrush MyBrushDark = new SolidColorBrush();
            myColorAnimation.From = Color.FromRgb(0, 0, 180);
            myColorAnimation.To = Color.FromRgb(90, 90, 90);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            MyBrushDark.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);

            ///Animation for background lines to move it further apart
            //ThicknessAnimation BackgroundBottomDarkAnimation = new ThicknessAnimation();
            //BackgroundBottomDarkAnimation.To = new Thickness(-1000, 413, -1573, -100);
            //BackgroundBottomDarkAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundBottomDark.BeginAnimation(MarginProperty, BackgroundBottomDarkAnimation);

            //ThicknessAnimation BackgroundBottomLightAnimation = new ThicknessAnimation();
            //BackgroundBottomLightAnimation.To = new Thickness(-1000, 482, -1067, -100);
            //BackgroundBottomLightAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundBottomLight.BeginAnimation(MarginProperty, BackgroundBottomLightAnimation);

            //ThicknessAnimation BackgroundTopDarkAnimation = new ThicknessAnimation();
            //BackgroundTopDarkAnimation.To = new Thickness(-1521, -100, -1000, 336);
            //BackgroundTopDarkAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundTopDark.BeginAnimation(MarginProperty, BackgroundTopDarkAnimation);

            //ThicknessAnimation BackgroundTopLightAnimation = new ThicknessAnimation();
            //BackgroundTopLightAnimation.To = new Thickness(-944, -100, -1000, 282);
            //BackgroundTopLightAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundTopLight.BeginAnimation(MarginProperty, BackgroundTopLightAnimation);

            BackgroundBottomDark.Background = MyBrushDark;
            BackgroundBottomLight.Background = MyBrushLight;
            BackgroundTopDark.Background = MyBrushDark;
            BackgroundTopLight.Background = MyBrushLight;

            LoadingAnimationGrid.Visibility = Visibility.Visible;
        }
        public void LoadedAnimation()
        {
            //Animation for background lines to transforme it back to color from black and white
            SolidColorBrush MyBrushLight = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(150, 150, 150);
            myColorAnimation.To = Color.FromRgb(0, 0, 255);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            MyBrushLight.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);

            SolidColorBrush MyBrushDark = new SolidColorBrush();
            myColorAnimation.From = Color.FromRgb(90, 90, 90);
            myColorAnimation.To = Color.FromRgb(0, 0, 180);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            MyBrushDark.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);

            ///Animation for background lines to move it back closer
            //ThicknessAnimation BackgroundBottomDarkAnimation = new ThicknessAnimation();
            //BackgroundBottomDarkAnimation.To = new Thickness(-1000, 413, -1373, -100);
            //BackgroundBottomDarkAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundBottomDark.BeginAnimation(MarginProperty, BackgroundBottomDarkAnimation);

            //ThicknessAnimation BackgroundBottomLightAnimation = new ThicknessAnimation();
            //BackgroundBottomLightAnimation.To = new Thickness(-1000, 482, -1267, -100);
            //BackgroundBottomLightAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundBottomLight.BeginAnimation(MarginProperty, BackgroundBottomLightAnimation);

            //ThicknessAnimation BackgroundTopDarkAnimation = new ThicknessAnimation();
            //BackgroundTopDarkAnimation.To = new Thickness(-1321, -100, -1000, 336);
            //BackgroundTopDarkAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundTopDark.BeginAnimation(MarginProperty, BackgroundTopDarkAnimation);

            //ThicknessAnimation BackgroundTopLightAnimation = new ThicknessAnimation();
            //BackgroundTopLightAnimation.To = new Thickness(-1144, -100, -1000, 282);
            //BackgroundTopLightAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            //BackgroundTopLight.BeginAnimation(MarginProperty, BackgroundTopLightAnimation);

            BackgroundBottomDark.Background = MyBrushDark;
            BackgroundBottomLight.Background = MyBrushLight;
            BackgroundTopDark.Background = MyBrushDark;
            BackgroundTopLight.Background = MyBrushLight;

            LoadingAnimationGrid.Visibility = Visibility.Hidden;
        }
        #endregion

        //Next and back
        #region GRIDSWIPE animation
        private void SwipeGridLeft(object currentgrid, object nextgrid)
        {
            Grid CurrentGrid = (Grid)currentgrid;
            Grid NextGrid = (Grid)nextgrid;

            //Animation to move right grid to the display
            ThicknessAnimation LogInAnimation = new ThicknessAnimation();
            LogInAnimation.To = new Thickness(0, 0, 0, 0);
            LogInAnimation.From = new Thickness(System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0, -(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0);
            LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            NextGrid.BeginAnimation(MarginProperty, LogInAnimation);

            //Animation to move displayed grid to the left
            ThicknessAnimation ControlAnimation = new ThicknessAnimation();
            ControlAnimation.To = new Thickness(-(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0, System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0);
            ControlAnimation.From = new Thickness(0, 0, 0, 0);
            ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            CurrentGrid.BeginAnimation(MarginProperty, ControlAnimation);
        }

        private void SwipeGridRight(object currentgrid, object previusgrid)
        {
            Grid CurrentGrid = (Grid)currentgrid;
            Grid PreviusGrid = (Grid)previusgrid;

            //Animation to move the left grid to the display
            ThicknessAnimation LogInAnimation = new ThicknessAnimation();
            LogInAnimation.From = new Thickness(0, 0, 0, 0);
            LogInAnimation.To = new Thickness(System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0, -(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0);
            LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            CurrentGrid.BeginAnimation(MarginProperty, LogInAnimation);

            //Animation to move the displayed grid to the right
            ThicknessAnimation ControlAnimation = new ThicknessAnimation();
            ControlAnimation.From = new Thickness(-(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0, System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0);
            ControlAnimation.To = new Thickness(0, 0, 0, 0);
            ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            PreviusGrid.BeginAnimation(MarginProperty, ControlAnimation);
        }
        #endregion
        #endregion


        #region LOGIN SCREEN
        //Exit button on login screen
        private void loginexitbutton_Click(object sender, RoutedEventArgs e)
        {
            //Close appliaciton
            this.Close();
        }

        //LogIn button on login screen
        private async void loginloginbutton_Click(object sender, RoutedEventArgs e)
        {
            //Begin loading animation
            LoadingAnimation();

            await Task.Delay(100);

            string username = Encryption(loginusernameinput.Text);
            string password = Encryption(loginpasswordinput.Password.ToString());

            try
            {
                if (LogInCommands.UserLogIn(username, password))
                {
                    //Generate all reservations from users database
                    var isGetAllSearched = await GetAllReservations(null);

                    Username = username;

                    ControlGrid.Visibility = Visibility.Visible;

                    //Animation to hide login screen
                    ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                    LogInAnimation.To = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
                    LogInAnimation.From = new Thickness(0, 0, 0, 0);
                    LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                    LogInScreen.BeginAnimation(MarginProperty, LogInAnimation);

                    //Animation to show control screen
                    ThicknessAnimation ControlAnimation = new ThicknessAnimation();
                    ControlAnimation.From = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
                    ControlAnimation.To = new Thickness(0, 0, 0, 0);
                    ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                    ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

                    loginfaillabel.Visibility = Visibility.Hidden;

                    //End the loading animation
                    LoadedAnimation();
                }
                else
                {
                    //If there is no user in database then show error label
                    loginfaillabel.Visibility = Visibility.Visible;

                    //End the loading animation
                    LoadedAnimation();
                }


            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Something went wrong. Please contact system support.");
            }
        }

        //Username input on login screen
        private async void loginusernameinput_KeyUp(object sender, KeyEventArgs e)
        {
            //If Key.Enter is clicked while username input is focused
            if (e.Key == Key.Enter)
            {
                //Begin loading animation
                LoadingAnimation();

                await Task.Delay(100);

                string username = Encryption(loginusernameinput.Text);
                string password = Encryption(loginpasswordinput.Password.ToString());

                try
                {
                    if (LogInCommands.UserLogIn(username, password))
                    {
                        //Generate all reservations from users database
                        var isGetAllSearched = await GetAllReservations(null);

                        ControlGrid.Visibility = Visibility.Visible;

                        //Animation to hide login screen
                        ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                        LogInAnimation.To = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
                        LogInAnimation.From = new Thickness(0, 0, 0, 0);
                        LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                        LogInScreen.BeginAnimation(MarginProperty, LogInAnimation);

                        //Animation to show control screen
                        ThicknessAnimation ControlAnimation = new ThicknessAnimation();
                        ControlAnimation.From = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
                        ControlAnimation.To = new Thickness(0, 0, 0, 0);
                        ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                        ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

                        loginfaillabel.Visibility = Visibility.Hidden;

                        //End the loading animation
                        LoadedAnimation();
                    }
                    else
                    {
                        //If there is no user in database then show error label
                        loginfaillabel.Visibility = Visibility.Visible;

                        //End the loading animation
                        LoadedAnimation();
                    }


                }
                catch (Exception ex)
                {
                    PublicCommands.ShowError("Something went wrong. Please contact system support.");
                }
            }
        }

        //Password input on login screen
        private async void loginpasswordinput_KeyUp(object sender, KeyEventArgs e)
        {
            //If Key.Enter is clicked while password input is focused
            if (e.Key == Key.Enter)
            {
                //Begin loading animation
                LoadingAnimation();

                await Task.Delay(100);

                string username = Encryption(loginusernameinput.Text);
                string password = Encryption(loginpasswordinput.Password.ToString());

                try
                {
                    if (LogInCommands.UserLogIn(username, password))
                    {
                        //Generate all reservations from users database
                        var isGetAllSearched = await GetAllReservations(null);

                        ControlGrid.Visibility = Visibility.Visible;

                        //Animation to hide login screen
                        ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                        LogInAnimation.To = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
                        LogInAnimation.From = new Thickness(0, 0, 0, 0);
                        LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                        LogInScreen.BeginAnimation(MarginProperty, LogInAnimation);

                        //Animation to show control screen
                        ThicknessAnimation ControlAnimation = new ThicknessAnimation();
                        ControlAnimation.From = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
                        ControlAnimation.To = new Thickness(0, 0, 0, 0);
                        ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                        ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

                        loginfaillabel.Visibility = Visibility.Hidden;

                        //End the loading animation
                        LoadedAnimation();
                    }
                    else
                    {
                        //If there is no user in database then show error label
                        loginfaillabel.Visibility = Visibility.Visible;

                        //End the loading animation
                        LoadedAnimation();
                    }


                }
                catch (Exception ex)
                {
                    PublicCommands.ShowError("Something went wrong. Please contact system support.");
                }
            }
        }
        #endregion


        #region MENU BUTTONS
        //Dashboard button on control screen
        private async void MenuDashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingAnimation();

                ClearAll();

                ControlGrid.Visibility = Visibility.Visible;
                ReservationsScreen.Visibility = Visibility.Hidden;
                ReservationsScreen.Margin = new Thickness(0, 0, 0, 0);
                HomeGridNoResultsLabel.Visibility = Visibility.Hidden;
                ClassifficationScreen.Visibility = Visibility.Hidden;
                CreateClassifficationScreen.Visibility = Visibility.Hidden;
                CreateReservationScreen.Visibility = Visibility.Hidden;
                SettingsScreen.Visibility = Visibility.Hidden;

                DashboardScreen.Visibility = Visibility.Visible;

                var CalendarInfo = await GetCalendarInfo();

                LoadedAnimation();
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);

                LoadedAnimation();
            }
        }

        void DashboardClassifficationsGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            DashboardClassifficationsGridRowsScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            DashboardClassifficationsGridColumnsScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private async Task<bool> GetCalendarInfo()
        {
            await Task.Delay(100);

            dynamic classiffications = ReservationCommands.GetCalendarClassiffications();
            DateTime maxdate = DateTime.Now;
            DateTime mindate = DateTime.Now;

            int x = 0;

            foreach (var information in classiffications)
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                DashboardClassifficationsGrid.RowDefinitions.Add(newrow);

                newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                DashboardClassifficationsGridRows.RowDefinitions.Add(newrow);

                Label label = new Label();
                label.Content = information.Name;
                label.Style = (Style)this.Resources["Label"];
                label.VerticalAlignment = VerticalAlignment.Center;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(label, x);
                DashboardClassifficationsGridRows.Children.Add(label);

                try
                {
                    maxdate = DateTime.Parse(information.MaxDate.ToString());
                    mindate = DateTime.Parse(information.MinDate.ToString());

                    DashboardScreenUnavailableCapacityLabel.Content = information.Filled.ToString() + "%";
                    DashboardScreenUnavailableClassifficationsLabel.Content = information.Unavailable.ToString();

                    DashboardScreenAvailableCapacityLabel.Content = (1 -double.Parse(information.Filled.ToString())).ToString() + "%";
                    DashboardScreenAvailableClassifficationsLabel.Content = information.Available.ToString();
                }
                catch { }

                x++;
            }

            int whileloop = 0;

            while (true)
            {
                ColumnDefinition newcol = new ColumnDefinition();
                newcol.Width = new GridLength(200);
                DashboardClassifficationsGrid.ColumnDefinitions.Add(newcol);

                newcol = new ColumnDefinition();
                newcol.Width = new GridLength(200);
                DashboardClassifficationsGridColumns.ColumnDefinitions.Add(newcol);

                

                Label label = new Label();
                label.Content = mindate.AddDays(whileloop - 1).ToString("yyyy-MM-dd");
                label.Style = (Style)this.Resources["Label"];
                label.VerticalAlignment = VerticalAlignment.Center;
                label.HorizontalAlignment = HorizontalAlignment.Center;

                if (mindate.AddDays(whileloop - 1) == DateTime.Today)
                {
                    label.Background = (SolidColorBrush)Resources["PrimaryBrush"];
                    label.Height = 50;
                    label.Width = 200;
                    label.VerticalContentAlignment = VerticalAlignment.Center;
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                }

                Grid.SetColumn(label, whileloop);
                DashboardClassifficationsGridColumns.Children.Add(label);

                whileloop++;

                if (mindate.AddDays(whileloop - 1) > maxdate)
                {
                    break;
                }
            }

            dynamic calendarinfo = ReservationCommands.GetCalendarInfo();

            foreach (var information in calendarinfo)
            {
                foreach (var room in DashboardClassifficationsGridRows.Children)
                {
                    Label label = (Label)room;
                    if (label.Content.ToString() == information.Classiffication.ToString())
                    {
                        foreach (var date in DashboardClassifficationsGridColumns.Children)
                        {
                            Label flabel = (Label)date;

                            if (DateTime.Parse(flabel.Content.ToString()) == DateTime.Parse(information.FromDate.ToString()))
                            {
                                Button btn = new Button();
                                string[] name = information.Name.ToString().Split('-');
                                btn.Content = name[0].Substring(0, 1) + ". " + name[1];
                                btn.Style = (Style)this.Resources["GeneratedDashboardButton"];
                                Grid.SetColumn(btn, Grid.GetColumn(flabel));

                                foreach (var todate in DashboardClassifficationsGridColumns.Children)
                                {
                                    Random rnd = new Random();

                                    btn.Background = (SolidColorBrush)Resources["DashboardButtonColor" + rnd.Next(1, 6).ToString()];

                                    Label tlabel = (Label)todate;
                                    if (DateTime.Parse(tlabel.Content.ToString()) == DateTime.Parse(information.ToDate.ToString()))
                                    {
                                        Grid.SetRow(btn, Grid.GetRow(label));
                                        Grid.SetColumnSpan(btn, (DateTime.Parse(tlabel.Content.ToString()).Date - DateTime.Parse(flabel.Content.ToString()).Date).Days + 1);
                                    }
                                }

                                DashboardClassifficationsGrid.Children.Add(btn);

                            }
                        }
                    }
                }
            }

            return true;
        }

        //Reservations button on control screen
        private async void MenuReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Start the loading animation
                LoadingAnimation();

                //Show the reservatons grid and hide no results label
                ControlGrid.Visibility = Visibility.Visible;
                ReservationsScreen.Visibility = Visibility.Visible;
                ReservationsScreen.Margin = new Thickness(0, 0, 0, 0);
                HomeGridNoResultsLabel.Visibility = Visibility.Hidden;
                ClassifficationScreen.Visibility = Visibility.Hidden;
                CreateClassifficationScreen.Visibility = Visibility.Hidden;
                CreateReservationScreen.Visibility = Visibility.Hidden;
                SettingsScreen.Visibility = Visibility.Hidden;
                DashboardScreen.Visibility = Visibility.Hidden;


                //Clear all elements
                ClearAll();

                //Generate all reservations
                var isGetAllReservations = await GetAllReservations(null);

                //End the loading animation
                LoadedAnimation();
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);

                //End the loading animation
                LoadedAnimation();
            }
        }

        //Classiffications button on control screen
        private async void MenuClassifficationBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Start the loading animation
                LoadingAnimation();

                //Show the reservatons grid and hide no results label
                ControlGrid.Visibility = Visibility.Visible;
                ReservationsScreen.Visibility = Visibility.Hidden;
                ClassifficationScreen.Visibility = Visibility.Visible;
                ReservationsScreen.Margin = new Thickness(0, 0, 0, 0);
                HomeGridNoResultsLabel.Visibility = Visibility.Hidden;
                CreateReservationScreen.Visibility = Visibility.Hidden;
                CreateClassifficationScreen.Visibility = Visibility.Hidden;
                SettingsScreen.Visibility = Visibility.Hidden;
                DashboardScreen.Visibility = Visibility.Hidden;

                //Clear all elements
                ClearAll();

                //Generate all reservations
                var isGetAllClassiffications = await GetAllClassiffications();

                //End the loading animation
                LoadedAnimation();
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);

                //End the loading animation
                LoadedAnimation();
            }
        }

        //LogOut button on control screen
        private void MenuLogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            //Reset the grid view
            ReservationsScreen.Visibility = Visibility.Visible;
            CreateReservationScreen.Visibility = Visibility.Hidden;
            CreateClassifficationScreen.Visibility = Visibility.Hidden;
            ClassifficationScreen.Visibility = Visibility.Hidden;
            SettingsScreen.Visibility = Visibility.Hidden;
            DashboardScreen.Visibility = Visibility.Hidden;

            //Animation to show login screen
            ThicknessAnimation LogInAnimation = new ThicknessAnimation();
            LogInAnimation.To = new Thickness(0, 0, 0, 0);
            LogInAnimation.From = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
            LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
            LogInScreen.BeginAnimation(MarginProperty, LogInAnimation);

            //Animation to hide control screen
            ThicknessAnimation ControlAnimation = new ThicknessAnimation();
            ControlAnimation.To = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
            ControlAnimation.From = new Thickness(0, 0, 0, 0);
            ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
            ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

            LogInScreen.Visibility = Visibility.Visible;
            loginfaillabel.Visibility = Visibility.Hidden;
        }

        //Settings button on control screen
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            //Hide all posible grids and make sure the settings grid is the only one visible
            ReservationsScreen.Visibility = Visibility.Hidden;
            CreateReservationScreen.Visibility = Visibility.Hidden;
            SettingsScreen.Visibility = Visibility.Visible;
            CreateClassifficationScreen.Visibility = Visibility.Hidden;
            ClassifficationScreen.Visibility = Visibility.Hidden;
            DashboardScreen.Visibility = Visibility.Hidden;

            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Set all the variables from app.config file
            CalculatePricePerPearson.IsChecked = bool.Parse(configuration.AppSettings.Settings["CalculatePerPearson"].Value);
            CalculateUnderaged.IsChecked = bool.Parse(configuration.AppSettings.Settings["CalculateUnderaged"].Value);
            AgeLimit.Text = configuration.AppSettings.Settings["CalculateUnderagedAge"].Value;
            TaxInput.Text = configuration.AppSettings.Settings["Tax"].Value;

            //Check what needs to be disabled
            if (bool.Parse(configuration.AppSettings.Settings["CalculatePerPearson"].Value))
            {
                if (bool.Parse(configuration.AppSettings.Settings["CalculateUnderaged"].Value) == false)
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
        #endregion


        #region GRID ACTIONS
        #region RESERVATIONS GRID actions
        //Generate buttons for all the Reservations in json file
        public async Task<bool> GetAllReservations(int? status)
        {
            await Task.Delay(100);

            try
            {
                ClearAll();

                //Get json file for all the reservations
                dynamic getall = ReservationCommands.GetAll();


                if (getall != null)
                {
                    try
                    {
                        //Declare row definition for the generated children
                        int col = 0;
                        int row = 0;
                        RowDefinition newrow = new RowDefinition();
                        newrow.Height = new GridLength(50);
                        HomeGridScrollViewer.RowDefinitions.Add(newrow);

                        foreach (var information in getall)
                        {
                            if (status == null)
                            {
                                newrow = new RowDefinition();
                                newrow.Height = new GridLength(50);
                                HomeGridScrollViewer.RowDefinitions.Add(newrow);

                                //Declare children speciffications
                                Button button = new Button();
                                button.Name = "ReservationId" + information.Id;
                                string firstname = information.Firstname.ToString();
                                //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                                //    "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                                button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                                //Current
                                if (information.Status == 0)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 101, 101, 255);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;

                                    //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                    //Add generated children to the grid
                                    Grid.SetColumn(button, col);
                                    Grid.SetRow(button, row);
                                    HomeGridScrollViewer.Children.Add(button);
                                }
                                //Past
                                else if (information.Status == -1)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 242, 179, 59);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;

                                    //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                    //Add generated children to the grid
                                    Grid.SetColumn(button, col);
                                    Grid.SetRow(button, row);
                                    HomeGridScrollViewer.Children.Add(button);
                                }
                                //Incoming
                                else if (information.Status == 1)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 74, 148, 74);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;

                                    //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                    //Add generated children to the grid
                                    Grid.SetColumn(button, col);
                                    Grid.SetRow(button, row);
                                    HomeGridScrollViewer.Children.Add(button);
                                }

                                else if (information.Status == -2)
                                {
                                    continue;
                                }

                                Label label = new Label();
                                label.Content = firstname + " " + information.Surname;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 0);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.Name;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 1);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.FromDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 2);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.ToDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 3);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                row++;
                            }

                            if (information.Status == status)
                            {

                                newrow = new RowDefinition();
                                newrow.Height = new GridLength(50);
                                HomeGridScrollViewer.RowDefinitions.Add(newrow);

                                //Declare children speciffications
                                Button button = new Button();
                                button.Name = "ReservationId" + information.Id;
                                string firstname = information.Firstname.ToString();
                                //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                                //    "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                                button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                                //Current
                                if (information.Status == 0)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 101, 101, 255);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;
                                }
                                //Past
                                else if (information.Status == -1)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 242, 179, 59);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;
                                }
                                //Incoming
                                else if (information.Status == 1)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 74, 148, 74);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;
                                }
                                //Dismissed
                                else if (information.Status == -2)
                                {
                                    SolidColorBrush myBrush = new SolidColorBrush();
                                    ColorAnimation myColorAnimation = new ColorAnimation();
                                    myColorAnimation.To = Color.FromArgb(255, 255, 79, 79);
                                    myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                    myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                    button.BorderBrush = myBrush;
                                }

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);

                                Label label = new Label();
                                label.Content = firstname + " " + information.Surname;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 0);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.Name;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 1);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.FromDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 2);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.ToDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 3);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                row++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PublicCommands.ShowError(ex.Message);
                    }
                }
                else
                {
                    //If there is no children inserted in grid, show No Results
                    HomeGridNoResultsLabel.Visibility = Visibility.Visible;
                }

                HomeGridScrollViewerSetter.ScrollToVerticalOffset(0);

                return true;
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);

                return false;
            }
        }

        //Generate buttons for searched Reservations in json file
        public async Task<bool> GetSearchedReservations()
        {
            await Task.Delay(100);

            ClearAll();

            ReservationsScreen.Visibility = Visibility.Visible;
            HomeGridNoResultsLabel.Visibility = Visibility.Hidden;

            //Get json file for the searched reservations
            dynamic GetSearched = ReservationCommands.GetSearched(HomeGridSearch.Text);

            if (GetSearched != null)
            {
                //Declare row definition for the generated children
                int col = 0;
                int row = 0;
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(250);
                HomeGridScrollViewer.RowDefinitions.Add(newrow);

                ComboBoxItem item = (ComboBoxItem)ReservationGridSpanSelection.SelectedItem;

                if (item.Content.ToString() == "All")
                {
                    foreach (var information in GetSearched)
                    {
                        if (information.Success == 1)
                        {
                            newrow = new RowDefinition();
                            newrow.Height = new GridLength(50);
                            HomeGridScrollViewer.RowDefinitions.Add(newrow);

                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ReservationId" + information.Id;
                            string firstname = information.Firstname.ToString();
                            //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                            //    "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                            button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                            //Current
                            if (information.Status == 0)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 101, 101, 255);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);
                            }
                            //Past
                            else if (information.Status == -1)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 242, 179, 59);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);
                            }
                            //Incoming
                            else if (information.Status == 1)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 74, 148, 74);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);
                            }

                            Label label = new Label();
                            label.Content = firstname + " " + information.Surname;
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            Grid.SetColumn(label, 0);
                            Grid.SetRow(label, row);
                            HomeGridScrollViewer.Children.Add(label);

                            label = new Label();
                            label.Content = information.Name;
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            Grid.SetColumn(label, 1);
                            Grid.SetRow(label, row);
                            HomeGridScrollViewer.Children.Add(label);

                            label = new Label();
                            label.Content = information.FromDate;
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            Grid.SetColumn(label, 2);
                            Grid.SetRow(label, row);
                            HomeGridScrollViewer.Children.Add(label);

                            label = new Label();
                            label.Content = information.ToDate;
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            Grid.SetColumn(label, 3);
                            Grid.SetRow(label, row);
                            HomeGridScrollViewer.Children.Add(label);

                            row++;
                        }
                    }
                }

                else if (item.Content.ToString() == "Incoming")
                {
                    foreach (var information in GetSearched)
                    {
                        if (information.Success == 1)
                        {
                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ReservationId" + information.Id;
                            string firstname = information.Firstname.ToString();
                            //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                            //            "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                            button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                            //Incoming
                            if (information.Status == 1)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 74, 148, 74);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);

                                Label label = new Label();
                                label.Content = firstname + " " + information.Surname;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 0);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.Name;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 1);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.FromDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 2);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.ToDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 3);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                row++;
                            }
                        }
                    }
                }

                else if (item.Content.ToString() == "Past")
                {
                    foreach (var information in GetSearched)
                    {
                        if (information.Success == 1)
                        {
                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ReservationId" + information.Id;
                            string firstname = information.Firstname.ToString();
                            //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                            //            "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                            button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                            //Incoming
                            if (information.Status == -1)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 242, 179, 59);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);

                                Label label = new Label();
                                label.Content = firstname + " " + information.Surname;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 0);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.Name;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 1);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.FromDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 2);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.ToDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 3);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                row++;
                            }
                        }
                    }
                }

                else if (item.Content.ToString() == "Current")
                {
                    foreach (var information in GetSearched)
                    {
                        if (information.Success == 1)
                        {
                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ReservationId" + information.Id;
                            string firstname = information.Firstname.ToString();
                            //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                            //"\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                            button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                            //Incoming
                            if (information.Status == 0)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 101, 101, 255);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);

                                Label label = new Label();
                                label.Content = firstname + " " + information.Surname;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 0);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.Name;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 1);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.FromDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 2);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.ToDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 3);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                row++;
                            }
                        }
                    }
                }

                else if (item.Content.ToString() == "Dismissed")
                {
                    foreach (var information in GetSearched)
                    {
                        if (information.Success == 1)
                        {
                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ReservationId" + information.Id;
                            string firstname = information.Firstname.ToString();
                            //button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                            //            "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                            button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];

                            //Incoming
                            if (information.Status == -2)
                            {
                                SolidColorBrush myBrush = new SolidColorBrush();
                                ColorAnimation myColorAnimation = new ColorAnimation();
                                myColorAnimation.To = Color.FromArgb(255, 255, 79, 79);
                                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
                                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
                                button.BorderBrush = myBrush;

                                //button.Click += new RoutedEventHandler(ShowSubjectsGrades);

                                //Add generated children to the grid
                                Grid.SetColumn(button, col);
                                Grid.SetRow(button, row);
                                HomeGridScrollViewer.Children.Add(button);

                                Label label = new Label();
                                label.Content = firstname + " " + information.Surname;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 0);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.Name;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 1);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.FromDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 2);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                label = new Label();
                                label.Content = information.ToDate;
                                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                                Grid.SetColumn(label, 3);
                                Grid.SetRow(label, row);
                                HomeGridScrollViewer.Children.Add(label);

                                col++;
                            }
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        //SearchBox on reservations grid
        private async void HomeGridSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                if (HomeGridSearch.IsFocused)
                {
                    //If Enter key is pressed
                    if (e.Key == Key.Enter)
                    {
                        //Start the loading animation
                        LoadingAnimation();

                        //Generate all the searched reservations 
                        var isGetSearched = await GetSearchedReservations();

                        //If there were no generated children show No Results label
                        if (HomeGridScrollViewer.Children.Count == 0)
                        {
                            HomeGridNoResultsLabel.Visibility = Visibility.Visible;
                        }

                        Cursor = Cursors.Arrow;

                        //End the loading animation
                        LoadedAnimation();
                    }
                }
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);

                //End the loading animation
                LoadedAnimation();

                HomeGridNoResultsLabel.Visibility = Visibility.Visible;
            }
        }

        //Add reservation button on reservations grid
        private async void AddReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Begin loading animation
                LoadingAnimation();

                //Fill the Combobox with all the classifficaitons
                await Task.Delay(100);

                ClearAll();

                CreateReservationGridClassifficationCombobox.Items.Add("None");
                CreateReservationGridClassifficationCombobox.SelectedIndex = 0;

                dynamic GetClassiffications = ClassifficationCommands.GetAll();

                foreach (var information in GetClassiffications)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Name = "ClassifficationId" + information.Id;
                    item.Content = information.Name;

                    CreateReservationGridClassifficationCombobox.Items.Add(item);
                }

                //Switch displayed grids
                CreateReservationScreen.Visibility = Visibility.Visible;
                ReservationsScreen.Visibility = Visibility.Hidden;

                //Animation to show main reservant information screen
                ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                LogInAnimation.To = new Thickness(0, 0, 0, 0);
                LogInAnimation.From = new Thickness(System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0, -(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0);
                LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
                CreateReservationGridReservationInformationGrid.BeginAnimation(MarginProperty, LogInAnimation);

                //Make sure that every grid will be displayed in the right order
                CreateReservationGridReservationInformationGrid.Visibility = Visibility.Visible;
                CreateReservationGridMainReservantInformationGrid.Visibility = Visibility.Hidden;
                CreateReservationGridSideReservantInformationGrid.Visibility = Visibility.Hidden;
                CreateReservationGridPaymentInformationGrid.Visibility = Visibility.Hidden;

                //Resert progression bar
                CreateReservationGridReservationInformationProgress.Foreground = Brushes.Gray;
                CreateReservationGridMainReservantInformationProgress.Foreground = Brushes.Gray;
                CreateReservationGridSideGuestsInformationProgress.Foreground = Brushes.Gray;
                CreateReservationGridPaymentInformationProgress.Foreground = Brushes.Gray;

                //Start the progress bar
                CreateReservationGridReservationInformationProgress.Foreground = (SolidColorBrush)Resources["FontBrush"];

                //Reset all the values needed for the Creation
                CreateReservationProgress = 1;
                CreateReservationGridNextBtn.Content = "Next";
                CreateReservationGridBackBtn.Content = "Cancel";

                BlackOutPastDates();

                //RowDefinition newrow = new RowDefinition();
                //newrow.Height = new GridLength(150);
                //CreateReservationGridSideGuestAddedGrid.RowDefinitions.Add(newrow);

                //End the loading animation
                LoadedAnimation();
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Something went wrong. Please contact system support.");

                LoadedAnimation();
            }
        }

        public void BlackOutPastDates()
        {
            try
            {
                CreateReservationGridFromDateCalendar.BlackoutDates.Clear();
                CreateReservationGridToDateCalendar.BlackoutDates.Clear();

                CreateReservationGridFromDateCalendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(1900, 1, 1), DateTime.Today.AddDays(-1)));
                CreateReservationGridToDateCalendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(1900, 1, 1), DateTime.Today.AddDays(-1)));

            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Something went wrong. Please contact system support.");
            }
        }

        public int CreateReservationProgress = 1;
        //NextGrid button on every grid inside of create reservation grid
        private async void CreateReservationGridNextBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CreateReservationProgress == 1)
                {
                    bool classifficaiton = false;
                    if (CreateReservationGridClassifficationCombobox.SelectedIndex <= 0)
                    {
                        CreateReservationGridClassifficationLabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        classifficaiton = false;
                    }
                    else
                    {
                        CreateReservationGridClassifficationLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        classifficaiton = true;
                    }
                    bool fromdate = false;
                    if (CreateReservationGridFromDateCalendar.SelectedDate == null)
                    {
                        CreateReservationGridFromDateLabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        fromdate = false;
                    }
                    else
                    {
                        CreateReservationGridFromDateLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        fromdate = true;
                    }
                    bool todate = false;
                    if (CreateReservationGridToDateCalendar.SelectedDate == null)
                    {
                        CreateReservationGridToDateLabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        todate = false;
                    }
                    else
                    {
                        CreateReservationGridToDateLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        todate = true;
                    }
                    
                    if (classifficaiton && fromdate && todate)
                    {
                        //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                        CreateReservationGridMainReservantInformationGrid.Visibility = Visibility.Visible;

                        SwipeGridLeft(CreateReservationGridReservationInformationGrid, CreateReservationGridMainReservantInformationGrid);

                        CreateReservationGridMainReservantInformationProgress.Foreground = (SolidColorBrush)Resources["FontBrush"];

                        CreateReservationGridBackBtn.Content = "Back";

                        CreateReservationProgress += 1;
                    }
                }
                else if (CreateReservationProgress == 2)
                {
                    bool firstname = false;
                    if (CreateReservationGridMainGuestFirstnameInput.Text.Length <= 1)
                    {
                        CreateReservationGridMainGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        firstname = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        firstname = true;
                    }

                    bool surname = false;
                    if (CreateReservationGridMainGuestSurnameInput.Text.Length <= 2)
                    {
                        CreateReservationGridMainGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        surname = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        surname = true;
                    }

                    bool birth = false;
                    if (CreateReservationGridMainReservantBirthCalendar.SelectedDate == null)
                    {
                        CreateReservationGridMainGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        birth = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        birth = true;
                    }

                    bool email = false;
                    Regex emailregex = new Regex(@".*@\w*\.\w*");
                    if (emailregex.IsMatch(CreateReservationGridMainGuestEmailInput.Text) == false)
                    {
                        CreateReservationGridMainGuestEmaillabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        email = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestEmaillabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        email = true;
                    }

                    bool phonenumber = false;
                    if (CreateReservationGridMainGuestPhoneNumberInput.Text.Length <= 7)
                    {
                        CreateReservationGridMainGuestPhoneNumberlabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        phonenumber = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestPhoneNumberlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        phonenumber = true;
                    }

                    bool country = false;
                    if (CreateReservationGridMainGuestCountryInput.Text.Length <= 2)
                    {
                        CreateReservationGridMainGuestCountrylabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        country = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestCountrylabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        country = true;
                    }

                    bool Address = false;
                    if (CreateReservationGridMainGuestAddressInput.Text.Length <= 2)
                    {
                        CreateReservationGridMainGuestAddresslabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        Address = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestAddresslabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        Address = true;
                    }

                    bool postnumber = false;
                    if (CreateReservationGridMainGuestPostNumberInput.Text.Length <= 1)
                    {
                        CreateReservationGridMainGuestPostNumberlabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        postnumber = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestPostNumberlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        postnumber = true;
                    }

                    bool city = false;
                    if (CreateReservationGridMainGuestCityInput.Text.Length < 1)
                    {
                        CreateReservationGridMainGuestCitylabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        city = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestCitylabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        city = true;
                    }

                    bool certifiednumber = false;
                    if (CreateReservationGridMainGuestCertifiedNumberInput.Text.Length <= 3)
                    {
                        CreateReservationGridMainGuestCertifiedNumberlabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        certifiednumber = false;
                    }
                    else
                    {
                        CreateReservationGridMainGuestCertifiedNumberlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                        certifiednumber = true;
                    }

                    if (firstname && surname && birth && email && phonenumber && country && Address && postnumber && city && certifiednumber)
                    {
                        //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                        CreateReservationGridSideReservantInformationGrid.Visibility = Visibility.Visible;

                        SwipeGridLeft(CreateReservationGridMainReservantInformationGrid, CreateReservationGridSideReservantInformationGrid);

                        CreateReservationGridSideGuestsInformationProgress.Foreground = (SolidColorBrush)Resources["FontBrush"];

                        CreateReservationProgress += 1;
                    }
                }
                else if (CreateReservationProgress == 3)
                {
                    CreateReservationGridNextBtn.Content = "Done";

                    bool ischeckedchek = false;
                    ComboBoxItem item = (ComboBoxItem)CreateReservationGridClassifficationCombobox.SelectedItem;
                    int id = Int32.Parse(item.Name.ToString().Replace("ClassifficationId", ""));

                    double price = (((CreateReservationGridToDateCalendar.SelectedDate.Value.Date - CreateReservationGridFromDateCalendar.SelectedDate.Value.Date).TotalDays + 1) * ClassifficationCommands.GetClassifficationPrice(id));
                    //Generate all the items that are being payed (property, breakfast...)
                    foreach (object child in CreateReservationGridAvailableEssentialsGrid.Children)
                    {
                        if (child.GetType().ToString() == "System.Windows.Controls.CheckBox")
                        {
                            CheckBox checkbox = (CheckBox)child;

                            ischeckedchek = false;

                            if (checkbox.IsChecked == true)
                            {
                                ischeckedchek = true;
                            }
                        }
                        else if (child.GetType().ToString() == "System.Windows.Controls.TextBox")
                        {
                            TextBox textbox = (TextBox)child;

                            if (ischeckedchek == true)
                            {
                                price = price + Convert.ToDouble(textbox.Text.Replace("€", ""));
                            }
                        }
                    }

                    CreateReservationGridPaymentInformationPriceInput.Text = (price).ToString();

                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridPaymentInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridLeft(CreateReservationGridSideReservantInformationGrid, CreateReservationGridPaymentInformationGrid);

                    CreateReservationGridPaymentInformationProgress.Foreground = (SolidColorBrush)Resources["FontBrush"];

                    Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    CreateReservationGridPaymentinfoTaxinput.Text = configuration.AppSettings.Settings["Tax"].Value;

                    CreateReservationProgress++;
                }
                else if (CreateReservationProgress == 4)
                {
                    LoadingAnimation();

                    try
                    {
                        //Generate pdf
                        PDF pdf = new PDF(1, CalculateReceipt());

                        ComboBoxItem classiffication = (ComboBoxItem)CreateReservationGridClassifficationCombobox.SelectedItem;

                        if (CreateReservationGridPaymentInformationCreditCardCheckBox.IsChecked == true || CreateReservationGridPaymentInformationCashCheckBox.IsChecked == true)
                        {
                            CreateReservationGridPaymentInformationCreditCardCheckBox.Foreground = (SolidColorBrush)Resources["FontBrush"];
                            CreateReservationGridPaymentInformationCashCheckBox.Foreground = (SolidColorBrush)Resources["FontBrush"];

                            if (ReservationCommands.PostReservationInformation(classiffication.Name.Replace("ClassifficationId", ""), CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridPaymentInformationPriceInput.Text, CreateReservationGridPaymentInformationCommentInput.Text.ToString(), Username))
                            {
                                if (ReservationCommands.PostMainReservantInformation(classiffication.Name.Replace("ClassifficationId", ""), CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridMainGuestFirstnameInput.Text, CreateReservationGridMainGuestSurnameInput.Text, CreateReservationGridMainReservantBirthCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridMainGuestEmailInput.Text, CreateReservationGridMainGuestPhoneNumberInput.Text, CreateReservationGridMainGuestCountryInput.Text, CreateReservationGridMainGuestPostNumberInput.Text, CreateReservationGridMainGuestCityInput.Text, CreateReservationGridMainGuestAddressInput.Text, "Male", CreateReservationGridMainGuestCertifiedNumberInput.Text, Username))
                                {
                                    foreach (var child in CreateReservationGridAvailableEssentialsGrid.Children)
                                    {

                                        if (child is CheckBox)
                                        {
                                            CheckBox checkbox = (CheckBox) child;

                                            if (checkbox.IsChecked == true)
                                            {
                                                ReservationCommands.PostEssentialInformation(classiffication.Name.Replace("ClassifficationId", ""), CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), checkbox.Content.ToString(), Username);

                                                //PublicCommands.ShowError(classiffication.Name.Replace("ClassifficationId", "") + CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd") + CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd") + checkbox.Content.ToString() + Username);
                                            }
                                        }
                                    }

                                    bool sidereservants = true;

                                    foreach (var child in CreateReservationGridSideGuestAddedGrid.Children)
                                    {
                                        string Firstname, Surname, Birth = string.Empty;

                                        Button btn = (Button)child;

                                        if (btn.Content.ToString().Contains("⨉") == false)
                                        {
                                            Grid parent = (Grid)btn.Parent;
                                            int index = parent.Children.IndexOf(btn);
                                            int rowindex = Grid.GetRow(btn);
                                            RowDefinition row = (RowDefinition)parent.RowDefinitions[rowindex];
                                            string[] buttoninfo = btn.Content.ToString().Split('\n');

                                            //Fill all the inuts from children info
                                            Firstname = btn.Name.Replace('_', ' ');
                                            Surname = buttoninfo[0].ToString().Substring(3);
                                            Birth = buttoninfo[1];

                                            if (ReservationCommands.PostSideReservantInformation(classiffication.Name.Replace("ClassifficationId", ""), CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("yyyy-MM-dd"), Firstname, Surname, Birth, Username))
                                            {
                                                sidereservants = true;
                                            }
                                            else
                                            {
                                                sidereservants = false;
                                            }
                                        }
                                    }

                                    if (sidereservants)
                                    {
                                        //Return back to the reservations screen
                                        CreateReservationScreen.Visibility = Visibility.Hidden;
                                        ReservationsScreen.Visibility = Visibility.Visible;
                                        HomeGridNoResultsLabel.Visibility = Visibility.Hidden;

                                        var isGetAllReservations = await GetAllReservations(null);

                                        CreateReservationProgress += 1;
                                    }
                                    else
                                    {
                                        PublicCommands.ShowError("Writtings in the server is currupted. Please contact system support!");
                                    }
                                }
                                else
                                {
                                    PublicCommands.ShowError("Writing in the server is currupted. Please contact system support!");
                                }
                            }
                            else
                            {
                                PublicCommands.ShowError("Reservation culdn't be created. Please check your internet connection and try again.");
                            }
                        }
                        else
                        {
                            CreateReservationGridPaymentInformationCreditCardCheckBox.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                            CreateReservationGridPaymentInformationCashCheckBox.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                        }
                    }
                    catch (Exception ex) 
                    {
                        PublicCommands.ShowError(ex.Message);
                        //PublicCommands.ShowError("Something is wrong with the server. Please check your internet connection and try again.");
                    }

                    LoadedAnimation();
                }
                else
                {
                    PublicCommands.ShowError("Something went wrong. Please contact system support.");
                }
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);
            }
        }

        private async void CreateReservationGridBackBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CreateReservationProgress == 1)
                {
                    try
                    {
                        //Begin loading animation
                        LoadingAnimation();

                        await Task.Delay(100);

                        CreateReservationScreen.Visibility = Visibility.Hidden;
                        ReservationsScreen.Visibility = Visibility.Visible;
                        HomeGridNoResultsLabel.Visibility = Visibility.Hidden;

                        var isGetAllReservations = await GetAllReservations(null);

                        //End loading animation
                        LoadedAnimation();
                    }
                    catch
                    {
                        LoadedAnimation();
                    }
                }
                else if (CreateReservationProgress == 2)
                {
                    CreateReservationGridBackBtn.Content = "Cancel";

                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridMainReservantInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridRight(CreateReservationGridMainReservantInformationGrid, CreateReservationGridReservationInformationGrid);

                    CreateReservationGridMainReservantInformationProgress.Foreground = Brushes.Gray;
                }
                else if (CreateReservationProgress == 3)
                {
                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridSideReservantInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridRight(CreateReservationGridSideReservantInformationGrid, CreateReservationGridMainReservantInformationGrid);

                    CreateReservationGridSideGuestsInformationProgress.Foreground = Brushes.Gray;
                }
                else if (CreateReservationProgress == 4)
                {
                    CreateReservationGridNextBtn.Content = "Next";

                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridPaymentInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridRight(CreateReservationGridPaymentInformationGrid, CreateReservationGridSideReservantInformationGrid);

                    CreateReservationGridPaymentInformationProgress.Foreground = Brushes.Gray;
                }
                else
                {
                    PublicCommands.ShowError("Something went wrong. Please contact system support.");
                }

                CreateReservationProgress -= 1;
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Something went wrong. Please contact system support.");
            }
        }

        private async void CreateReservationGridClassifficationCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadingAnimation();

                await Task.Delay(100);

                if (CreateReservationGridClassifficationCombobox.SelectedIndex <= 0 || CreateReservationGridClassifficationCombobox.Items.Count != 0)
                {
                    CreateReservationGridAvailableEssentialsGrid.Children.Clear();
                    CreateReservationGridAvailableEssentialsGrid.RowDefinitions.Clear();

                    ComboBoxItem item = (ComboBoxItem)CreateReservationGridClassifficationCombobox.SelectedItem;

                    if (item == null)
                    {
                        return;
                    }
                    dynamic AvailableEssentials = ReservationCommands.GetAvailableEssentials(item.Name.Replace("ClassifficationId", ""));

                    int row = 0;

                    foreach (var information in AvailableEssentials)
                    {
                        if (information.Success == 0)
                        {
                            break;
                        }

                        RowDefinition newrow = new RowDefinition();
                        newrow.Height = new GridLength(60);
                        CreateReservationGridAvailableEssentialsGrid.RowDefinitions.Add(newrow);


                        CheckBox button = new CheckBox();
                        button.Name = "EssentialId" + information.Id;
                        button.Content = information.Name;
                        button.Style = (Style)this.Resources["GeneratedCheckBox"];

                        TextBox textbox = new TextBox();
                        textbox.Text = information.Price + "€";
                        textbox.Style = (Style)this.Resources["CheckBogGeneratedButtonPrice"];

                        Grid.SetColumn(button, 0);
                        Grid.SetRow(button, row);
                        CreateReservationGridAvailableEssentialsGrid.Children.Add(button);

                        Grid.SetColumn(textbox, 1);
                        Grid.SetRow(textbox, row);
                        CreateReservationGridAvailableEssentialsGrid.Children.Add(textbox);

                        row++;
                    }

                    BlackOutPastDates();

                    dynamic BlackOutDates = ReservationCommands.GetBlackoutDates(item.Name.Replace("ClassifficationId", ""));

                    foreach (var information in BlackOutDates)
                    {
                        CreateReservationGridFromDateCalendar.SelectedDates.Clear();
                        CreateReservationGridFromDateCalendar.BlackoutDates.Add(new CalendarDateRange((DateTime.Parse(information.FromDate.ToString())).AddDays(1), (DateTime.Parse(information.ToDate.ToString())).AddDays(-1)));
                        CreateReservationGridToDateCalendar.SelectedDates.Clear();
                        CreateReservationGridToDateCalendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Parse(information.FromDate.ToString()), DateTime.Parse(information.ToDate.ToString())));
                    }
                }

                LoadedAnimation();
            }
            catch (Exception ex)
            {
                LoadedAnimation();
            }
        }

        private void CreateReservationGridSideGuestAddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool firstname = false;
                if (CreateReservationGridSideGuestFirstnameInput.Text.Length <= 1)
                {
                    CreateReservationGridSideGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                    firstname = false;
                }
                else
                {
                    CreateReservationGridSideGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                    firstname = true;
                }

                bool surname = false;
                if (CreateReservationGridSideGuestSurnameInput.Text.Length <= 1)
                {
                    CreateReservationGridSideGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                    surname = false;
                }
                else
                {
                    CreateReservationGridSideGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                    surname = true;
                }

                bool birth = false;
                if (CreateReservationGridSideGuestBirthCalendar.SelectedDate == null)
                {
                    CreateReservationGridSideGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FalseBrush"];
                    birth = false;
                }
                else
                {
                    CreateReservationGridSideGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                    birth = true;
                }

                if (firstname && surname && birth)
                {
                    CreateReservationGridSideGuestFirstnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                    CreateReservationGridSideGuestSurnamelabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
                    CreateReservationGridSideGuestBirthlabel.Foreground = (SolidColorBrush)Resources["FontBrush"];

                    //Declare children speciffications
                    Button buttonAddedSideGuest = new Button();
                    buttonAddedSideGuest.Content = CreateReservationGridSideGuestFirstnameInput.Text.Substring(0, 1) + ". " + CreateReservationGridSideGuestSurnameInput.Text +
                        "\n" + CreateReservationGridSideGuestBirthCalendar.SelectedDate.Value.ToString("yyyy-MM-dd");
                    buttonAddedSideGuest.Name = CreateReservationGridSideGuestFirstnameInput.Text.Replace(' ', '_');
                    buttonAddedSideGuest.Style = (Style)this.Resources["GeneratedAddedSideGuestButton"];
                    buttonAddedSideGuest.Click += new RoutedEventHandler(EditAddedSideGuestBtn_Click);

                    int rows = CreateReservationGridSideGuestAddedGrid.RowDefinitions.Count;

                    //Create new row
                    RowDefinition newrow = new RowDefinition();
                    newrow.Height = new GridLength(150);
                    CreateReservationGridSideGuestAddedGrid.RowDefinitions.Add(newrow);

                    //Add children to grid
                    Grid.SetRow(buttonAddedSideGuest, rows);
                    Grid.SetColumn(buttonAddedSideGuest, 0);
                    CreateReservationGridSideGuestAddedGrid.Children.Add(buttonAddedSideGuest);

                    //Declare children speciffications
                    Button buttonAddedSideGuestDelete = new Button();
                    buttonAddedSideGuestDelete.Style = (Style)this.Resources["GeneratedAddedSideGuestDeleteButton"];
                    buttonAddedSideGuestDelete.Click += new RoutedEventHandler(EditAddedSideGuestDeleteBtn_Click);

                    //Add children to row
                    Grid.SetRow(buttonAddedSideGuestDelete, rows);
                    Grid.SetColumn(buttonAddedSideGuestDelete, 1);
                    CreateReservationGridSideGuestAddedGrid.Children.Add(buttonAddedSideGuestDelete);

                    //Clear all inputs
                    CreateReservationGridSideGuestFirstnameInput.Clear();
                    CreateReservationGridSideGuestSurnameInput.Clear();
                    CreateReservationGridSideGuestBirthCalendar.SelectedDate = null;
                }
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Please contact system support!");
            }
        }

        private void EditAddedSideGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get children info
                Button btn = (Button)sender;
                Grid parent = (Grid)btn.Parent;
                int index = parent.Children.IndexOf(btn);
                int rowindex = Grid.GetRow(btn);
                RowDefinition row = (RowDefinition)parent.RowDefinitions[rowindex];
                string[] buttoninfo = btn.Content.ToString().Split('\n');

                //Fill all the inuts from children info
                CreateReservationGridSideGuestFirstnameInput.Text = btn.Name.Replace('_', ' ');
                CreateReservationGridSideGuestSurnameInput.Text = buttoninfo[0].ToString().Substring(3);
                DateTime datetime = new DateTime();
                datetime = DateTime.Parse(buttoninfo[1]);
                CreateReservationGridSideGuestBirthCalendar.SelectedDate = datetime;

                try
                {
                    //Delete children and their delete button
                    CreateReservationGridSideGuestAddedGrid.Children.RemoveAt(index);
                    CreateReservationGridSideGuestAddedGrid.Children.RemoveAt(index);

                    //Hide the row
                    row.Height = new GridLength(0);
                }
                catch (Exception ex)
                {
                    PublicCommands.ShowError("Please contact system support!");
                }
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Something is wrong with the inputed data.");
            }
        }

        private void EditAddedSideGuestDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get children information
                Button btn = (Button)sender;
                Grid parent = (Grid)btn.Parent;
                int index = parent.Children.IndexOf(btn);
                index--;
                int rowindex = Grid.GetRow(btn);
                RowDefinition row = (RowDefinition)parent.RowDefinitions[rowindex];

                try
                {
                    //Delete children and their delete button
                    CreateReservationGridSideGuestAddedGrid.Children.RemoveAt(index);
                    CreateReservationGridSideGuestAddedGrid.Children.RemoveAt(index);

                    //Hide the row
                    row.Height = new GridLength(0);
                }
                catch (Exception ex)
                {
                    PublicCommands.ShowError("Please contact system support!");
                }
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Please contact system support!");
            }
        }

        private void CreateReservationGridPaymentInformationCreditCardCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //If the checkbox is checked than uncheck the other
            if (CreateReservationGridPaymentInformationCashCheckBox.IsChecked == true)
            {
                CreateReservationGridPaymentInformationCashCheckBox.IsChecked = false;
            }
        }

        private void CreateReservationGridPaymentInformationCashCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //If the checkbox is checked than uncheck the other
            if (CreateReservationGridPaymentInformationCreditCardCheckBox.IsChecked == true)
            {
                CreateReservationGridPaymentInformationCreditCardCheckBox.IsChecked = false;
            }
        }

        private void CreateReservationGridPaymentInformationPreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingAnimation();

                //Make sure that the document isn't open at the time
                CreateReservationGridPaymentInformationPdfPreview.CloseDocument();

                //Generate pdf
                PDF pdf = new PDF(3, CalculateReceipt());

                //Show the pdf preview
                CreateReservationGridPaymentInformationPdfPreview.LoadDocument("D:\\preview.pdf");

                LoadedAnimation();
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError("Something went wrong. Please contact system support.");
                PublicCommands.ShowError(ex.GetBaseException().Message);
                LoadedAnimation();
            }
        }


        public dynamic CalculateReceipt()
        {
            ComboBoxItem item = (ComboBoxItem)CreateReservationGridClassifficationCombobox.SelectedItem;
            int id = Int32.Parse(item.Name.ToString().Replace("ClassifficationId", ""));

            double price = ((CreateReservationGridToDateCalendar.SelectedDate.Value.Date - CreateReservationGridFromDateCalendar.SelectedDate.Value.Date).TotalDays * ClassifficationCommands.GetClassifficationPrice(id));

            //Generate information about bill and customer
            string json = "{\"DocumentName\": \"0001_" + CreateReservationGridClassifficationCombobox.Text + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + CreateReservationGridMainGuestSurnameInput.Text + "\", " +
                "\"DocumentNumber\": \"" + CreateReservationGridClassifficationCombobox.Text + "_" + CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("dd-MM-yyyy") + "_" + CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("dd-MM-yyyy") + "\", " +
                "\"CreatedDate\": \"" + DateTime.Now.ToString("dd-MM-yyyy") + "\", " +
                "\"FromDate\": \"" + CreateReservationGridFromDateCalendar.SelectedDate.Value.ToString("dd-MM-yyyy") + "\", " +
                "\"ToDate\": \"" + CreateReservationGridToDateCalendar.SelectedDate.Value.ToString("dd-MM-yyyy") + "\", " +
                "\"CustomerName\": \"" + CreateReservationGridMainGuestFirstnameInput.Text + " " + CreateReservationGridMainGuestSurnameInput.Text + "\", " +
                "\"CustomerAddress\": \"" + CreateReservationGridMainGuestAddressInput.Text + "\", \"" + CreateReservationGridMainGuestPostNumberInput.Text + "\" : \"" + CreateReservationGridMainGuestCityInput.Text + "\", " +
                "\"CustomerContact\": \"" + CreateReservationGridMainGuestEmailInput.Text + "\", " +
                "\"Items\":[{\"Quantity\": 1, \"Item\": \"Classiffication " + CreateReservationGridClassifficationCombobox.Text + "\", \"Price\": \"" + price.ToString() + "\"}";

            //Open app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //If the CalculatePerPearson is true
            if (bool.Parse(configuration.AppSettings.Settings["CalculatePerPearson"].Value))
            {
                int NumberOfGuests = 1;

                //If the Calculate Underaged is true, check teh underaged age settings and calculate how many people is abowe that age
                if (bool.Parse(configuration.AppSettings.Settings["CalculateUnderaged"].Value))
                {
                    //For each add one to NumberOfGuests
                    foreach (var sender in CreateReservationGridSideGuestAddedGrid.Children)
                    {
                        Button btn = (Button)sender;
                        Grid parent = (Grid)btn.Parent;

                        if ((parent.Children.IndexOf(btn) % 2) == 0)
                        {
                            string[] buttoninfo = btn.Content.ToString().Split('\n');

                            CreateReservationGridSideGuestFirstnameInput.Text = btn.Name.Replace('_', ' ');
                            CreateReservationGridSideGuestSurnameInput.Text = buttoninfo[0].ToString().Substring(3);
                            DateTime datetime = new DateTime();
                            datetime = DateTime.Parse(buttoninfo[1]);

                            if (datetime < (DateTime.Today.AddYears(-(Int32.Parse(configuration.AppSettings.Settings["CalculateUnderagedAge"].Value)))))
                            {
                                NumberOfGuests++;
                            }
                        }
                    }

                    //Add json string with generated data
                    json = json + ",{\"Quantity\": \"" + NumberOfGuests.ToString() + "\", \"Item\": \"Guests\", \"Price\": \"20.50\"}";
                }
                else
                {
                    //Add json string with generated data
                    json = json + ",{\"Quantity\": \"" + ((CreateReservationGridSideGuestAddedGrid.Children.Count / 2) + 1).ToString() + "\", \"Item\": \"Guests\", \"Price\": \"20.50\"}";
                }
            }


            bool ischeckedchek = false;
            //Generate all the items that are being payed (property, breakfast...)
            foreach (object child in CreateReservationGridAvailableEssentialsGrid.Children)
            {
                if (child.GetType().ToString() == "System.Windows.Controls.CheckBox")
                {
                    CheckBox checkbox = (CheckBox)child;

                    ischeckedchek = false;

                    if (checkbox.IsChecked == true)
                    {
                        json = json + ", {\"Quantity\": 1, \"Item\": \"" + checkbox.Content + "\"";

                        ischeckedchek = true;
                    }
                }
                else if (child.GetType().ToString() == "System.Windows.Controls.TextBox")
                {
                    TextBox textbox = (TextBox)child;

                    if (ischeckedchek == true)
                    {
                        json = json + ", \"Price\": \"" + textbox.Text.Replace("€", "") + "\"}";
                    }
                }
            }

            json = json + "]}";

            //Convert string to json
            dynamic pdfinfo = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            return pdfinfo;
        }

        private void PreviewCreateReservationGridPaymentInfoDdvInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CreateReservationGridFromDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateReservationGridToDateCalendar.BlackoutDates.Clear();
            CreateReservationGridToDateCalendar.SelectedDates.Clear();
            CreateReservationGridToDateCalendar.DisplayDate = (DateTime)CreateReservationGridFromDateCalendar.SelectedDate;
            CreateReservationGridToDateCalendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(1900, 1, 1), (DateTime)CreateReservationGridFromDateCalendar.SelectedDate.Value));

            Mouse.Capture(null);
        }

        private async void ReservationGridSpanSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadingAnimation();

            HomeGridNoResultsLabel.Visibility = Visibility.Hidden;

            await Task.Delay(100);

            ComboBoxItem item = (ComboBoxItem)ReservationGridSpanSelection.SelectedItem;

            if (item.Content.ToString() == "All")
            {
                var isGetAll = await GetAllReservations(null);
            }
            else if (item.Content.ToString() == "Current")
            {
                var isGetCurrent = await GetAllReservations(0);
            }
            else if (item.Content.ToString() == "Past")
            {
                var isGetPast = await GetAllReservations(-1);
            }
            else if (item.Content.ToString() == "Incoming")
            {
                var isGetIncoming = await GetAllReservations(1);
            }
            else if (item.Content.ToString() == "Dismissed")
            {
                var isGetDismissed = await GetAllReservations(-2);
            }

            if (HomeGridScrollViewer.Children.Count == 0)
            {
                HomeGridNoResultsLabel.Visibility = Visibility.Visible;
            }

            LoadedAnimation();
        }
        #endregion


        #region CLASSIFFICATIONS GRID actions
        public async Task<bool> GetAllClassiffications()
        {
            await Task.Delay(100);

            try
            {
                ClearAll();

                //Get json file for all the reservations
                dynamic getall = ClassifficationCommands.GetAll();


                if (getall != null)
                {
                    try
                    {
                        //Declare row definition for the generated children
                        int col = 0;
                        int row = 0;


                        foreach (var information in getall)
                        {
                            RowDefinition newrow = new RowDefinition();
                            newrow.Height = new GridLength(50);
                            ClassifficaitonsGridScrollViewer.RowDefinitions.Add(newrow);

                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ClassifficationId" + information.Id;
                            //string name = information.Name.ToString();
                            //button.Content = "Room: " + information.Name + "\n" + "SN: " + information.SerialNumber + "\n" + "Price: " + information.Price + "\n" + information.Size;
                            button.Style = (Style)this.Resources["GeneratedReservationAndClassifficationButton"];
                            //button.Click += new RoutedEventHandler(ShowSubjectsGrades);
                            //Add generated children to the grid
                            Grid.SetColumn(button, col);
                            Grid.SetRow(button, row);
                            ClassifficaitonsGridScrollViewer.Children.Add(button);

                            Label label = new Label();
                            label.Content = information.Name;
                            Grid.SetColumn(label, 0);
                            Grid.SetRow(label, row);
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            ClassifficaitonsGridScrollViewer.Children.Add(label);
                            label = new Label();
                            label.Content = information.SerialNumber;
                            Grid.SetColumn(label, 1);
                            Grid.SetRow(label, row);
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            ClassifficaitonsGridScrollViewer.Children.Add(label);
                            label = new Label();
                            label.Content = information.Price + "€";
                            Grid.SetColumn(label, 2);
                            Grid.SetRow(label, row);
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            ClassifficaitonsGridScrollViewer.Children.Add(label);
                            label = new Label();
                            label.Content = information.Size + "m square";
                            Grid.SetColumn(label, 3);
                            Grid.SetRow(label, row);
                            label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                            ClassifficaitonsGridScrollViewer.Children.Add(label);

                            row++;
                        }
                    }
                    catch (Exception ex)
                    {
                        PublicCommands.ShowError(ex.Message);
                    }
                }
                else
                {
                    //If there is no children inserted in grid, show No Results
                    ClassifficaitonsGridScrollViewer.Visibility = Visibility.Visible;
                }

                HomeGridScrollViewerSetter.ScrollToVerticalOffset(0);

                return true;
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);

                return false;
            }
        }

        #region Preview Classiffication Price input
        //string PreviewClassifficationPriceInputString = "";
        //private void PreviewClassifficationPriceInput(object sender, TextCompositionEventArgs e)
        //{
        //    try
        //    {
        //        Regex regex = new Regex(@"[^0-9]+");

        //        if (regex.IsMatch(e.Text) == false)
        //        {
        //            PreviewClassifficationPriceInputString = PreviewClassifficationPriceInputString + e.Text;

        //            if (PreviewClassifficationPriceInputString.Length == 1)
        //            {
        //                ClassifficationPriceInput.Text = "0.0" + PreviewClassifficationPriceInputString + "€";
        //            }
        //            else if (PreviewClassifficationPriceInputString.Length == 2)
        //            {
        //                ClassifficationPriceInput.Text = "0." + PreviewClassifficationPriceInputString + "€";
        //            }
        //            else if (PreviewClassifficationPriceInputString.Length >= 3)
        //            {
        //                ClassifficationPriceInput.Text = PreviewClassifficationPriceInputString.Substring(0,
        //                    (PreviewClassifficationPriceInputString.Length - 2)) + "." +
        //                    PreviewClassifficationPriceInputString.Substring(PreviewClassifficationPriceInputString.Length - 2) + "€";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        PublicCommands.ShowError(ex.Message);
        //    }
        //}

        //private void ClassifficationPriceInput_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Key == Key.Back)
        //        {
        //            PreviewClassifficationPriceInputString = PreviewClassifficationPriceInputString.Remove(PreviewClassifficationPriceInputString.Length - 1);
        //            if (PreviewClassifficationPriceInputString.Length == 0)
        //            {
        //                ClassifficationPriceInput.Text = "0.00" + "€";
        //            }
        //            else if (PreviewClassifficationPriceInputString.Length == 1)
        //            {
        //                ClassifficationPriceInput.Text = "0.0" + PreviewClassifficationPriceInputString + "€";
        //            }
        //            else if (PreviewClassifficationPriceInputString.Length == 2)
        //            {
        //                ClassifficationPriceInput.Text = "0." + PreviewClassifficationPriceInputString + "€";
        //            }
        //            else if (PreviewClassifficationPriceInputString.Length >= 3)
        //            {
        //                ClassifficationPriceInput.Text = PreviewClassifficationPriceInputString.Substring(0,
        //                    (PreviewClassifficationPriceInputString.Length - 2)) + "." +
        //                    PreviewClassifficationPriceInputString.Substring(PreviewClassifficationPriceInputString.Length - 2) + "€";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        PublicCommands.ShowError(ex.Message);
        //    }
        //}
        #endregion

        private void AddClassifficaionsBtn_Click(object sender, RoutedEventArgs e)
        {
            ClassifficationScreen.Visibility = Visibility.Hidden;
            CreateClassifficationScreen.Visibility = Visibility.Visible;

            dynamic getall = EssentialCommands.GetAll();


            int row = 0;
            foreach(var information in getall)
            {
                if (information.Success == 0)
                {
                    break;
                }

                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(60);
                CreateClassifficationGridIncludedEssentials.RowDefinitions.Add(newrow);


                CheckBox button = new CheckBox();
                button.Name = "EssentialId" + information.Id;
                button.Content = information.Name;
                button.Style = (Style)this.Resources["GeneratedCheckBox"];

                Grid.SetColumn(button, 0);
                Grid.SetRow(button, row);
                CreateClassifficationGridIncludedEssentials.Children.Add(button);

                row++;
            }
        }

        private void CreateClassifficationGridBackBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            ClassifficationScreen.Visibility = Visibility.Visible;
            CreateClassifficationScreen.Visibility = Visibility.Hidden;
        }
        #endregion


        #region SETTINGS GRID actions
        private void SettingsSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //Load app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Set every object to the right state (enabled, disabled) and right value (checked, unchecked)
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

            configuration.AppSettings.Settings["Tax"].Value = TaxInput.Text;

            configuration.AppSettings.Settings["Animations"].Value = AnimationsCheckbox.IsChecked.ToString();

            //Save app.config chages
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");

            SettingsScreen.Visibility = Visibility.Hidden;
            ReservationsScreen.Visibility = Visibility.Visible;

            MenuReservationsBtn_Click(sender, e);
        }

        //Preview for numbers only input
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CalculatePricePerPearson_Checked(object sender, RoutedEventArgs e)
        {
            //Load app.config file
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Enable checkbox
            CalculateUnderaged.IsEnabled = true;
            CalculateUnderaged.Foreground = (SolidColorBrush)Resources["FontBrush"];

            //If underaged isn't checked
            if (CalculateUnderaged.IsChecked == false)
            {
                //Disable age limit input
                AgeLimit.IsEnabled = false;
                AgeLimitLabel.Foreground = Brushes.Gray;
            }
            else
            {
                //Enable age limit
                AgeLimit.IsEnabled = true;
                AgeLimitLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
            }
        }

        private void CalculateUnderaged_Checked(object sender, RoutedEventArgs e)
        {
            //Enable age limit input
            AgeLimit.IsEnabled = true;
            AgeLimitLabel.Foreground = (SolidColorBrush)Resources["FontBrush"];
        }

        private void CalculatePricePerPearson_Unchecked(object sender, RoutedEventArgs e)
        {
            //Disable underaged checkbox and age limit input
            CalculateUnderaged.IsEnabled = false;
            CalculateUnderaged.Foreground = Brushes.Gray;
            AgeLimitLabel.Foreground = Brushes.Gray;
            AgeLimit.IsEnabled = false;
        }

        private void CalculateUnderaged_Unchecked(object sender, RoutedEventArgs e)
        {
            //Disable age limit input
            AgeLimit.IsEnabled = false;
            AgeLimitLabel.Foreground = Brushes.Gray;
        }
        #endregion
        #endregion

        private void CalendarLoseFocus(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void Calendar_GotFocus(object sender, RoutedEventArgs e)
        {
            Calendar calendar = (Calendar)sender;

            FocusManager.SetFocusedElement(calendar, null);
        }

        private void CreateClassifficationGridNextBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}