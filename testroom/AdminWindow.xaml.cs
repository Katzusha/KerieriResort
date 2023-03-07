using Google.Protobuf.Reflection;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using testroom;

namespace resorttestroom
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {

        MySqlConnection conn = MainWindow.conn;

        public bool createclient = false;
        public string editclientid = "";

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

        public AdminWindow()
        {
            InitializeComponent();

            GenerateClients();
        }

        public void ClearClientInpuit()
        {
            AdminScreenCompanyNameInput.Clear();
            AdminScreenCompanyDescriptionInput.Clear();
            AdminScreenCompanyEmailInput.Clear();
            AdminScreenCompanyPhoneNumberInput.Clear();
            AdminScreenCompanyStationaryNumberInput.Clear();
            AdminScreenCompanyCountryInput.Clear();
            AdminScreenPostNumberInput.Clear();
            AdminScreenAddressInput.Clear();
            AdminScreenDatabaseInput.Clear();

            AdminScreenSaveClientBtn.IsEnabled = false;
        }

        public async void GenerateClients()
        {
            createclient = false;

            ClearClientInpuit();

            AdminScreenCompanysGrid.Children.Clear();
            AdminScreenCompanysGrid.RowDefinitions.Clear();

            AdminScreenSaveClientBtn.Content = "Save";
            AdminScreenSaveClientBtn.IsEnabled = true;

            dynamic clients = AdminCommands.GetClients();

            int row = 0;

            foreach(var information in clients)
            {
                RowDefinition newrow = new RowDefinition();
                newrow.Height = new GridLength(50);
                AdminScreenCompanysGrid.RowDefinitions.Add(newrow);

                //Declare children speciffications
                Button button = new Button();
                button.Name = "ClientId" + information.Id;
                button.Style = (Style)this.Resources["GeneratedCompanyButton"];

                //Add generated children to the grid
                Grid.SetColumn(button, 0);
                Grid.SetColumnSpan(button, 2);
                Grid.SetRow(button, row);
                AdminScreenCompanysGrid.Children.Add(button);

                Label label = new Label();
                label.Content = information.Name.ToString();
                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);
                AdminScreenCompanysGrid.Children.Add(label);
                label = new Label();
                label.Content = information.DatabaseName.ToString();
                label.Style = (Style)this.Resources["GeneratedReservationAndClassifficationLabel"];
                Grid.SetColumn(label, 1);
                Grid.SetRow(label, row);
                AdminScreenCompanysGrid.Children.Add(label);

                row++;
            }
        }

        private void ClientBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            dynamic clientinfo = AdminCommands.GetClientsInfo(btn.Name.ToString().Replace("ClientId", ""));

            editclientid = btn.Name.ToString().Replace("ClientId", "");

            foreach(var information in clientinfo)
            {
                AdminScreenCompanyNameInput.Text = information.Name.ToString();
                AdminScreenCompanyDescriptionInput.Text = information.Description.ToString();
                AdminScreenCompanyEmailInput.Text = information.InfoEmail.ToString();
                AdminScreenCompanyPhoneNumberInput.Text = information.PhoneNumber.ToString();
                AdminScreenCompanyStationaryNumberInput.Text = information.StationaryNumber.ToString();
                AdminScreenCompanyCountryInput.Text = information.Country.ToString();
                AdminScreenPostNumberInput.Text = information.PostCode.ToString();
                AdminScreenAddressInput.Text = information.Address.ToString();
                AdminScreenDatabaseInput.Text = information.DatabaseName.ToString();
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

        //Enter and leave animations for green buttons
        #region GREEN BUTTON animations
        private void GrayBtn_Enter(object sender, MouseEventArgs e)
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
        private void GrayBtn_Leave(object sender, MouseEventArgs e)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AdminScreenAddClientBtn_Click(object sender, RoutedEventArgs e)
        {
            createclient = true;

            ClearClientInpuit();

            AdminScreenSaveClientBtn.Content = "Create";
            AdminScreenSaveClientBtn.IsEnabled = true;
        }

        private void AdminScreenSaveClientBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (createclient)
                {
                    if (AdminCommands.CreateClient(AdminScreenCompanyNameInput.Text, AdminScreenCompanyEmailInput.Text, AdminScreenCompanyPhoneNumberInput.Text, AdminScreenCompanyStationaryNumberInput.Text, AdminScreenCompanyCountryInput.Text, AdminScreenPostNumberInput.Text, AdminScreenAddressInput.Text, AdminScreenCompanyDescriptionInput.Text, AdminScreenDatabaseInput.Text))
                    {
                        if (AdminCommands.CreateCompany(AdminScreenCompanyNameInput.Text, AdminScreenCompanyEmailInput.Text, AdminScreenCompanyPhoneNumberInput.Text, AdminScreenCompanyStationaryNumberInput.Text, AdminScreenCompanyCountryInput.Text, AdminScreenPostNumberInput.Text, AdminScreenAddressInput.Text, AdminScreenCompanyDescriptionInput.Text, AdminScreenDatabaseInput.Text))
                        {
                            bool user = AdminCommands.CreateUser(AdminScreenCompanyNameInput.Text, AdminScreenCompanyEmailInput.Text, Encryption(AdminScreenCompanyNameInput.Text.ToLower()), Encryption("admin"));

                            ClearClientInpuit();

                            GenerateClients();
                        }
                    }
                    else
                    {
                        PublicCommands.ShowError(2, null);
                    }
                }
                else
                {
                    if (AdminCommands.EditCompany(editclientid, AdminScreenCompanyNameInput.Text, AdminScreenCompanyEmailInput.Text, AdminScreenCompanyPhoneNumberInput.Text, AdminScreenCompanyStationaryNumberInput.Text, AdminScreenCompanyCountryInput.Text, AdminScreenPostNumberInput.Text, AdminScreenAddressInput.Text, AdminScreenCompanyDescriptionInput.Text, AdminScreenDatabaseInput.Text))
                    {
                        if (AdminCommands.EditClient(editclientid, AdminScreenCompanyNameInput.Text, AdminScreenCompanyEmailInput.Text, AdminScreenCompanyPhoneNumberInput.Text, AdminScreenCompanyStationaryNumberInput.Text, AdminScreenCompanyCountryInput.Text, AdminScreenPostNumberInput.Text, AdminScreenAddressInput.Text, AdminScreenCompanyDescriptionInput.Text, AdminScreenDatabaseInput.Text))
                        {
                            ClearClientInpuit();

                            GenerateClients();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.ErrorException = ex.Message;
                PublicCommands.ShowError(2, null);
            }
        }
    }
}
