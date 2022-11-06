using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace testroom
{
    class ReservationCommands
    {
        //TODO: Update API for new database
        public static dynamic GetAll()
        {
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ReservationsAPI/GetAll.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "DatabaseName=" + MainWindow.DatabaseName;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            //request.ContentType = "application/x-www-form-urlencoded";
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

                //MainWindow.ShowError(html);
            }
            dynamic AllClassiffications = JsonConvert.DeserializeObject(html);

            return AllClassiffications;
        }

        public static dynamic GetSearched(string searched)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ReservationsAPI/GetSearched.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "searched=" + searched + "&DatabaseName=" + MainWindow.DatabaseName;
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

                //MainWindow.ShowError(html);
            }
            dynamic SearchedReservations = JsonConvert.DeserializeObject(html);

            return SearchedReservations;
        }

        public static dynamic GetAvailableEssentials(string ClassifficationId)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ReservationsAPI/GetAllEssentials.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "ClassifficationId=" + ClassifficationId.ToString() + "&DatabaseName=" + MainWindow.DatabaseName;
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

                //MainWindow.ShowError(html);
            }
            dynamic AvailableEssentials = JsonConvert.DeserializeObject(html);

            return AvailableEssentials;
        }

        public static bool PostReservationInformation (string ClassifficationId, string FromDate, string ToDate, string Price, string Comment)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(MainWindow.APIconnection + "/ReservationsAPI/PostReservations.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "ClassifficationId=" + ClassifficationId.ToString() + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&Price=" + Price + "&Comment=" + Comment + "&DatabaseName=" + MainWindow.DatabaseName;
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

                //MainWindow.ShowError(html);
            }
            dynamic AvailableEssentials = JsonConvert.DeserializeObject(html);

            if (AvailableEssentials.Success == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
