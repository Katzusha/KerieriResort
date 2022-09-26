using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using jsreport.Client;
using System.Net;
using jsreport.Local;
using jsreport.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Dynamic;
using Newtonsoft.Json.Converters;

namespace testroom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PUBLIC VALUES AND COMMANDS
        public static MySqlConnection conn = new MySqlConnection("server=152.89.234.155;user=kosakand_admin;database=kosakand_iAsistent;port=3306;" +
                    "password=uclWRX~uV2jq");
        public static MySqlCommand cmd;

        public static string ClassifficationType = string.Empty;

        public static string DatabaseName = "";

        public static string APIconnection = "https://kosakandraz.com/API";
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
            //When application start we need to make sure that the login window is the first window that we show
            //Regardles of programming activity
            InitializeComponent();

            ControlGrid.Visibility = Visibility.Hidden;
            LogInGrid.Visibility = Visibility.Visible;

            ReservationsGrid.Visibility = Visibility.Visible;
            HomeGridNoResultsLabel.Visibility = Visibility.Hidden;
            CreateReservationGrid.Visibility = Visibility.Hidden;

            InitializeComponent();

            StartClock();
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            MenuClock.Content = DateTime.Now.ToString("HH:mm");
            MenuDate.Content = DateTime.Now.ToString("dd.MM.yyyy");
        }

        #region PUBLIC COMMANDS
        //Public command to reset all variables and grid children
        public void ClearAll()
        {
            loginusernameinput.Clear();
            loginpasswordinput.Clear();

            HomeGridScrollViewer.Children.Clear();
            HomeGridScrollViewer.RowDefinitions.Clear();

            CreateReservationGridClassifficationCombobox.Items.Clear();

            CreateReservationGridFromDateCalendar.SelectedDates.Clear();
            CreateReservationGridToDateCalendar.SelectedDates.Clear();

            CreateReservationGridAvailableEssentialsGrid.Children.Clear();
            CreateReservationGridAvailableEssentialsGrid.RowDefinitions.Clear();

            CreateReservationGridMainGuestFirstnameInput.Clear();
            CreateReservationGridMainGuestSurnameInput.Clear();
            CreateReservationGridMainReservantBirthCalendar.SelectedDates.Clear();
            CreateReservationGridMainGuestEmailInput.Clear();
            CreateReservationGridMainGuestPhoneNumberInput.Clear();
            CreateReservationGridMainGuestCountryInput.Clear();
            CreateReservationGridMainGuestPostNumberInput.Clear();
            CreateReservationGridMainGuestAddressInput.Clear();
            CreateReservationGridMainGuestCertifiedNumberInput.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            loginusernamelabel.Content = DateTime.Now.ToString("HH:mm:ss");
        }

        //Becouse of all the animations, instead of using normal commands, we need to use tasks becouse they execute in the background
        #region PUBLIC TASKS

        #region LogIn
        //Check if the user is in database and retryve information in what company this user works
        public async Task<bool> LogIn()
        {
            await Task.Delay(500);

            string username = Encryption(loginusernameinput.Text);
            string password = Encryption(loginpasswordinput.Password.ToString());

            try
            {
                Cursor = Cursors.Wait;
                if (LogInCommands.UserLogIn(username, password))
                {
                    Cursor = Cursors.Arrow;

                    return true;
                }
                else
                {
                    Cursor = Cursors.Arrow;

                    return false;
                }


            }
            catch (Exception ex)
            {
                Cursor = Cursors.Arrow;

                PublicCommands.ShowError(ex.Message);

                return false;
            }
        }
        #endregion

        #region Reservations
        //Generate buttons for all the Reservations in json file
        public async Task<bool> GetAllReservations()
        {
            await Task.Delay(500);

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
                        newrow.Height = new GridLength(250);
                        HomeGridScrollViewer.RowDefinitions.Add(newrow);

                        foreach (var information in getall)
                        {
                            //If there are four children in previus row generate new row same as privius
                            if (col == 4)
                            {
                                newrow = new RowDefinition();
                                newrow.Height = new GridLength(250);
                                HomeGridScrollViewer.RowDefinitions.Add(newrow);

                                col = 0;
                                row++;
                            }

                            //Declare children speciffications
                            Button button = new Button();
                            button.Name = "ReservationId" + information.Id;
                            string firstname = information.Firstname.ToString();
                            button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                                "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                            button.Style = (Style)this.Resources["HomeGeneratedButton"];
                            //button.Click += new RoutedEventHandler(ShowSubjectsGrades);
                            button.Background = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255));

                            //Add generated children to the grid
                            Grid.SetColumn(button, col);
                            Grid.SetRow(button, row);
                            HomeGridScrollViewer.Children.Add(button);

                            col++;
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
            await Task.Delay(500);

            ClearAll();

            ReservationsGrid.Visibility = Visibility.Visible;
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

                foreach (var information in GetSearched)
                {
                    if (col == 4)
                    {
                        //If there are four children in previus row generate new row same as privius
                        newrow = new RowDefinition();
                        newrow.Height = new GridLength(250);
                        HomeGridScrollViewer.RowDefinitions.Add(newrow);

                        col = 0;
                        row++;
                    }

                    //Declare children speciffications
                    Button button = new Button();
                    button.Name = "ReservationId" + information.Id;
                    string firstname = information.Firstname.ToString();
                    button.Content = firstname.Substring(0, 1) + ". " + information.Surname + "\nCl.: " + information.Name +
                                "\nFrom : " + information.FromDate + "\nTo: " + information.ToDate;
                    button.Style = (Style)this.Resources["HomeGeneratedButton"];
                    //button.Click += new RoutedEventHandler(ShowSubjectsGrades);
                    button.Background = new SolidColorBrush(Color.FromArgb(127, 0, 0, 255));

                    //Add generated children to the grid
                    Grid.SetColumn(button, col);
                    Grid.SetRow(button, row);
                    HomeGridScrollViewer.Children.Add(button);

                    col++;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        //Generate children for CreateReservation combobox
        public async Task<bool> GetAllClassifficaitonsForCombobox()
        {
            await Task.Delay(500);

            try
            {
                ClearAll();

                CreateReservationGridClassifficationCombobox.Items.Add("None");

                dynamic GetClassiffications = ClassifficationCommands.GetAll();

                foreach (var information in GetClassiffications)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Name = "ClassifficationId" + information.Id;
                    item.Content = information.Name;

                    CreateReservationGridClassifficationCombobox.Items.Add(item);
                }

                return true;
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);
                return false;
            }
            
        }

        //TODO: Generate available essentials
        public async Task<bool> GetAvailableEssentialsOfClassiffication()
        {
            await Task.Delay(500);

            ComboBoxItem item = (ComboBoxItem)CreateReservationGridClassifficationCombobox.SelectedItem;

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

            return false;
        }

        #endregion

        #endregion

        #endregion

        //All the animations
        #region ANIMATIONS
        //Enter and leave animations for red buttons
        #region RED BUTTON animations
        private void RedBtn_Enter(object sender, MouseEventArgs e)
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

            //Animations for buttons background color to transforme it from transparrent to red
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(0, 255, 0, 0);
            myColorAnimation.To = Color.FromArgb(255, 255, 0, 0);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;

            Cursor = Cursors.Hand;
        }
        private void RedBtn_Leave(object sender, MouseEventArgs e)
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

            //Animations for buttons background color to transforme it from red back to transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(255, 255, 0, 0);
            myColorAnimation.To = Color.FromArgb(0, 255, 0, 0);
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
        #endregion

        //Enter and leave animations for green buttons
        #region GREEN BUTTON animations
        private void GreenBtn_Enter(object sender, MouseEventArgs e)
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

        //Gotfocus and lostfocus animations for textboxes
        #region TEXTBOX INPUT animation
        private void LogInUsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 500 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void LogInUsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = (TextBox)sender;

            //Animation for textboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 500;
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
            PasswordBox text = (PasswordBox)sender;

            //Animation for textboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 500 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);
            myDoubleAnimation.To = 50 * 1.1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            text.BeginAnimation(TextBox.HeightProperty, myDoubleAnimation);
        }
        private void PasswordBoxInput_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox text = (PasswordBox)sender;

            //Animation for passwordboxes size to transforme it back to 100% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = 500;
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
            myDoubleAnimation.To = 480;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(23, 23, 23);
            myColorAnimation.To = Color.FromRgb(0, 0, 225);
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
            myColorAnimation.From = Color.FromRgb(0, 0, 255);
            myColorAnimation.To = Color.FromRgb(23, 23, 23);
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
            myDoubleAnimation.To = 440;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(23, 23, 23);
            myColorAnimation.To = Color.FromRgb(0, 0, 225);
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
            myColorAnimation.From = Color.FromRgb(0, 0, 255);
            myColorAnimation.To = Color.FromRgb(23, 23, 23);
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
            myDoubleAnimation.To = 650;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(TextBox.WidthProperty, myDoubleAnimation);

            //Animation for buttons background to transforme from transperent to blue
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromRgb(23, 23, 23);
            myColorAnimation.To = Color.FromRgb(0, 0, 225);
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
            myColorAnimation.From = Color.FromRgb(0, 0, 255);
            myColorAnimation.To = Color.FromRgb(23, 23, 23);
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
            myColorAnimation.From = Color.FromRgb(23, 23, 23);
            myColorAnimation.To = Color.FromRgb(255, 79, 79);
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
            myColorAnimation.From = Color.FromRgb(255, 79, 79);
            myColorAnimation.To = Color.FromRgb(23, 23, 23);
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
            TextBox text = (TextBox)sender;

            //Animation for searchboxes size to transforme it to 110% it's size
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myDoubleAnimation.To = 50 * 1.1;
            text.BeginAnimation(HeightProperty, myDoubleAnimation);
        }
        private void HomeGridSearch_MouseLeave(object sender, MouseEventArgs e)
        {
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
        private void GeneratedBtn_Enter(object sender, MouseEventArgs e)
        {
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
            myColorAnimation.From = Color.FromArgb(127, 0, 0, 255);
            myColorAnimation.To = Color.FromArgb(255, 0, 0, 255);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        private void GeneratedBtn_Leave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            Cursor = Cursors.Arrow;

            //Animation for buttons size to transforme it for 5px smaller
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.To = new Thickness(10, 10, 10, 10);
            thicknessAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            button.BeginAnimation(MarginProperty, thicknessAnimation);

            //Animation for buttons background to transforme it back from blue to kinda transparent
            SolidColorBrush myBrush = new SolidColorBrush();
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Color.FromArgb(255, 0, 0, 255);
            myColorAnimation.To = Color.FromArgb(127, 0, 0, 255);
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            button.Background = myBrush;
        }
        #endregion

        //Loading animations for all times where application needs to proccess something
        #region LOADING animation
        public  void LoadingAnimation()
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
        }
        #endregion

        //Next and back
        #region GRIDSWIPE animation
        private void SwipeGridLeft(object currentgrid, object nextgrid)
        {
            Grid CurrentGrid = (Grid)currentgrid;
            Grid NextGrid = (Grid)nextgrid;

            //Animation to show main reservant information screen
            ThicknessAnimation LogInAnimation = new ThicknessAnimation();
            LogInAnimation.To = new Thickness(0, 0, 0, 0);
            LogInAnimation.From = new Thickness(System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0, -(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0);
            LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            NextGrid    .BeginAnimation(MarginProperty, LogInAnimation);

            //Animation to hide reservation information screen
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

            //Animation to show main reservant information screen
            ThicknessAnimation LogInAnimation = new ThicknessAnimation();
            LogInAnimation.From = new Thickness(0, 0, 0, 0);
            LogInAnimation.To = new Thickness(System.Windows.SystemParameters.PrimaryScreenWidth + 1000, 0, -(System.Windows.SystemParameters.PrimaryScreenWidth + 1000), 0);
            LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            CurrentGrid.BeginAnimation(MarginProperty, LogInAnimation);

            //Animation to hide reservation information screen
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

            var isLogIn = await LogIn();
            
            //If there is a user start this loop
            if (isLogIn)
            {
                //Generate all reservations from users database
                var isGetAllSearched = await GetAllReservations();

                ControlGrid.Visibility = Visibility.Visible;

                //Animation to hide login screen
                ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                LogInAnimation.To = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
                LogInAnimation.From = new Thickness(0, 0, 0, 0);
                LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                LogInGrid.BeginAnimation(MarginProperty, LogInAnimation);

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

        //Username input on login screen
        private async void loginusernameinput_KeyUp(object sender, KeyEventArgs e)
        {
            //If Key.Enter is clicked while username input is focused
            if (e.Key == Key.Enter)
            {
                //Begin loading animation
                LoadingAnimation();

                var isLogIn = await LogIn();

                //If there is a user start this loop
                if (isLogIn)
                {
                    //Generate all reservations from users database
                    var isGetAllSearched = await GetAllReservations();

                    ControlGrid.Visibility = Visibility.Visible;

                    //Animation to hide login screen
                    ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                    LogInAnimation.To = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
                    LogInAnimation.From = new Thickness(0, 0, 0, 0);
                    LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                    LogInGrid.BeginAnimation(MarginProperty, LogInAnimation);

                    //Animation to show control screen
                    ThicknessAnimation ControlAnimation = new ThicknessAnimation();
                    ControlAnimation.From = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
                    ControlAnimation.To = new Thickness(0, 0, 0, 0);
                    ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                    ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

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
        }

        //Password input on login screen
        private async void loginpasswordinput_KeyUp(object sender, KeyEventArgs e)
        {
            //If Key.Enter is clicked while password input is focused
            if (e.Key == Key.Enter)
            {
                //Begin loading animation
                LoadingAnimation();

                var isLogIn = await LogIn();

                //If there is a user start this loop
                if (isLogIn)
                {
                    //Generate all reservations from users database
                    var isGetAllSearched = await GetAllReservations();

                    ControlGrid.Visibility = Visibility.Visible;

                    //Animation to hide login screen
                    ThicknessAnimation LogInAnimation = new ThicknessAnimation();
                    LogInAnimation.To = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
                    LogInAnimation.From = new Thickness(0, 0, 0, 0);
                    LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                    LogInGrid.BeginAnimation(MarginProperty, LogInAnimation);

                    //Animation to show control screen
                    ThicknessAnimation ControlAnimation = new ThicknessAnimation();
                    ControlAnimation.From = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
                    ControlAnimation.To = new Thickness(0, 0, 0, 0);
                    ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                    ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

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
        }
        #endregion

        #region MENU BUTTONS
        //Reservations button on control screen
        private async void MenuReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Start the loading animation
                LoadingAnimation();

                //Show the reservatons grid and hide no results label
                ControlGrid.Visibility = Visibility.Visible;
                ReservationsGrid.Visibility = Visibility.Visible;
                ReservationsGrid.Margin = new Thickness(0, 0, 0, 0);
                HomeGridNoResultsLabel.Visibility = Visibility.Hidden;
                CreateReservationGrid.Visibility = Visibility.Hidden;

                //Clear all elements
                ClearAll();

                //Generate all reservations
                var isGetAllReservations = await GetAllReservations();

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
        private void MenuClassifficationBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        //LogOut button on control screen
        private void MenuLogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            //Animation to show login screen
            ThicknessAnimation LogInAnimation = new ThicknessAnimation();
            LogInAnimation.To = new Thickness(0, 0, 0, 0);
            LogInAnimation.From = new Thickness(0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000, 0, 0);
            LogInAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
            LogInGrid.BeginAnimation(MarginProperty, LogInAnimation);

            //Animation to hide control screen
            ThicknessAnimation ControlAnimation = new ThicknessAnimation();
            ControlAnimation.To = new Thickness(0, 0, 0, System.Windows.SystemParameters.PrimaryScreenHeight + 1000);
            ControlAnimation.From = new Thickness(0, 0, 0, 0);
            ControlAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
            ControlGrid.BeginAnimation(MarginProperty, ControlAnimation);

            LogInGrid.Visibility = Visibility.Visible;
            loginfaillabel.Visibility = Visibility.Hidden;
        }
        #endregion

        #region RESERVATIONS GRID ACTIONS
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
            }
        }

        //Add reservation button on reservations grid
        private async void AddReservationsBtn_Click(object sender, RoutedEventArgs e)
        {
            //Begin loading animation
            LoadingAnimation();

            //Fill the Combobox with all the classifficaitons
            var isGetAllClassiffications = await GetAllClassifficaitonsForCombobox();

            //Switch displayed grids
            CreateReservationGrid.Visibility = Visibility.Visible;
            ReservationsGrid.Visibility = Visibility.Hidden;

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
            CreateReservationGridReservationInformationProgress.Foreground = Brushes.White;

            //Reset all the values needed for the Creation
            CreateReservationProgress = 1;
            CreateReservationGridNextBtn.Content = "Next";
            CreateReservationGridBackBtn.Content = "Cancel";

            //End the loading animation
            LoadedAnimation();
        }
        #endregion

        #region CLASSIFFICATIONS GRID ACTIONS

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

        #endregion

        public int CreateReservationProgress = 1;
        private async void CreateReservationGridNextBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CreateReservationProgress == 1)
                {
                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridMainReservantInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridLeft(CreateReservationGridReservationInformationGrid, CreateReservationGridMainReservantInformationGrid);

                    CreateReservationGridMainReservantInformationProgress.Foreground = Brushes.White;

                    CreateReservationGridBackBtn.Content = "Back";
                }
                else if (CreateReservationProgress == 2)
                {
                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridSideReservantInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridLeft(CreateReservationGridMainReservantInformationGrid, CreateReservationGridSideReservantInformationGrid);

                    CreateReservationGridSideGuestsInformationProgress.Foreground = Brushes.White;
                }
                else if (CreateReservationProgress == 3)
                {
                    CreateReservationGridNextBtn.Content = "Done";

                    //CreateReservationGridReservationInformationGrid.Visibility = Visibility.Hidden;
                    CreateReservationGridPaymentInformationGrid.Visibility = Visibility.Visible;

                    SwipeGridLeft(CreateReservationGridSideReservantInformationGrid, CreateReservationGridPaymentInformationGrid);

                    CreateReservationGridPaymentInformationProgress.Foreground = Brushes.White;
                }
                else if (CreateReservationProgress == 4)
                {
                    LoadingAnimation();

                    string json = "{\"DocumentName\": \"0001_" + CreateReservationGridClassifficationCombobox.Text + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + CreateReservationGridMainGuestSurnameInput.Text + "\", " +
                        "\"DocumentNumber\": \"" + CreateReservationGridClassifficationCombobox.Text + "_" + CreateReservationGridFromDateCalendar.SelectedDate.ToString().Replace("/", "-").Replace("00:00:00", "") + "_" + CreateReservationGridToDateCalendar.SelectedDate.ToString().Replace("/", "-").Replace("00:00:00", "") + "\", " +
                        "\"CreatedDate\": \"" + DateTime.Now.ToString("dd-MM-yyyy") + "\", " +
                        "\"FromDate\": \"" + CreateReservationGridFromDateCalendar.SelectedDate.ToString().Replace("-", "").Replace("00:00:00", "") + "\", " +
                        "\"ToDate\": \"" + CreateReservationGridToDateCalendar.SelectedDate.ToString().Replace("-", "").Replace("00:00:00", "") + "\", " +
                        "\"CustomerName\": \"" + CreateReservationGridMainGuestFirstnameInput.Text + " " + CreateReservationGridMainGuestSurnameInput.Text + "\", " +
                        "\"CustomerAddress\": \"" + CreateReservationGridMainGuestAddressInput.Text + "\", \"" + CreateReservationGridMainGuestPostNumberInput.Text + "\" : \"" + CreateReservationGridMainGuestCityInput.Text + "\", " +
                        "\"CustomerContact\": \"" + CreateReservationGridMainGuestEmailInput.Text + "\", " +
                        "\"Items\":[{\"Quantity\": 1, \"Item\": \"Classiffication " + CreateReservationGridClassifficationCombobox.Text + "\", \"Price\": \"200,00\"}";

                    bool ischeckedchek = false;
                    foreach (object child in CreateReservationGridAvailableEssentialsGrid.Children)
                    {
                        if (child.GetType().ToString() == "System.Windows.Controls.CheckBox")
                        {
                            ischeckedchek = false;

                            CheckBox checkbox = (CheckBox)child;

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

                    dynamic pdfinfo = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

                    PDF pdf = new PDF(1, pdfinfo);

                    CreateReservationGrid.Visibility = Visibility.Hidden;
                    ReservationsGrid.Visibility = Visibility.Visible;

                    var isGetAllReservations = await GetAllReservations();

                    LoadedAnimation();
                }
                else
                {
                    PublicCommands.ShowError("Something went wrong. Please contact system support.");
                }

                CreateReservationProgress += 1;                
            }
            catch (Exception ex)
            {
                PublicCommands.ShowError(ex.Message);
            }
        }

        private void CreateReservationGridBackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CreateReservationProgress == 1)
            {
                //Begin loading animation
                LoadingAnimation();

                CreateReservationGrid.Visibility = Visibility.Hidden;
                ReservationsGrid.Visibility = Visibility.Visible;

                GetAllReservations();

                //End loading animation
                LoadedAnimation();
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

        private void CreateReservationGridClassifficationCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CreateReservationGridClassifficationCombobox.SelectedIndex == 1)
            {
                CreateReservationGridAvailableEssentialsGrid.Children.Clear();
                CreateReservationGridAvailableEssentialsGrid.RowDefinitions.Clear();

                GetAvailableEssentialsOfClassiffication();
            }
        }
    }
}