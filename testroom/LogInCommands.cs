using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace testroom
{
    class LogInCommands
    {
        //Send username and password and retrive user information
        public static bool UserLogIn(string username, string password)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("https://kosakandraz.com/API/LoginAPI/login.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "username=" + username + "&password=" + password;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();

            string html = string.Empty;

            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            dynamic userlogin = JsonConvert.DeserializeObject(html);

            //MainWindow.ShowError(responseFromServer);

            if (userlogin.response.success == 1)
            {
                MainWindow.DatabaseName = userlogin.response.DatabaseName.ToString();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
